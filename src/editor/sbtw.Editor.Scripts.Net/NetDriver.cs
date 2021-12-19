// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace sbtw.Editor.Scripts.Net
{
    public class NetDriver
    {
        public event Action<string> OnMessage;
        public event Action<string> OnError;
        public bool HasDriver => hasDriver ??= GetSdks().Any();

        private bool? hasDriver = null;
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public void Build(string path) => BuildAsync(path).Wait();
        public void Clean(string path) => CleanAsync(path).Wait();
        public void Restore(string path) => RestoreAsync(path).Wait();
        public string Perform(params string[] args) => PerformAsync(args: args).Result;
        public IEnumerable<string> GetSdks() => GetSdksAsync().Result;

        public async Task BuildAsync(string path, CancellationToken token = default) => await PerformAsync(token, "build", path);
        public async Task CleanAsync(string path, CancellationToken token = default) => await PerformAsync(token, "clean", path);
        public async Task RestoreAsync(string path, CancellationToken token = default) => await PerformAsync(token, "restore", path);

        public async Task<IEnumerable<string>> GetSdksAsync(CancellationToken token = default)
        {
            try
            {
                string msg = await PerformAsync(token, "--list-sdks");
                return msg.Split('\n').Select(s => s.Split(' ').FirstOrDefault());
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> PerformAsync(CancellationToken token = default, params string[] args)
        {
            await semaphore.WaitAsync();

            var process = Process.Start(new ProcessStartInfo
            {
                Arguments = string.Join(' ', args),
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            });

            process.EnableRaisingEvents = true;

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            process.OutputDataReceived += (_, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    OnMessage?.Invoke(e.Data);
            };

            process.ErrorDataReceived += (_, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    OnError?.Invoke(e.Data);
            };

            await process.WaitForExitAsync(token);

            if (token.IsCancellationRequested)
                process.Kill();

            if (process.ExitCode != 0)
            {
                string err = await process.StandardError.ReadToEndAsync();
                process.Dispose();
                throw new Exception($"The process has exited with a non-zero exit code: {process.ExitCode}\n{err}");
            }

            string msg = await process.StandardOutput.ReadToEndAsync();
            process.Dispose();

            semaphore.Release();

            return msg;
        }
    }
}
