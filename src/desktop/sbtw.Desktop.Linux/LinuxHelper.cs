// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Diagnostics;
using System.Threading.Tasks;

namespace sbtw.Desktop.Linux
{
    public static class LinuxHelper
    {
        public static async Task<string> ExecuteAsync(string command)
        {
            using var process = new Process();
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = command;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            await process.WaitForExitAsync();

            return await process.StandardOutput.ReadToEndAsync();
        }

        public static string Execute(string command) => ExecuteAsync(command).Result;
    }
}
