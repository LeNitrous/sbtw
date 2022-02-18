// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace sbtw.Editor.Studios
{
    public abstract class Studio : IEquatable<Studio>
    {
        public abstract string Name { get; }

        public abstract string FriendlyName { get; }

        public abstract void Open(string path, int line = 0, int column = 0);

        public bool Run(string args, bool wait = false) => RunAsync(args, wait).Result;

        public async Task<bool> RunAsync(string args, bool wait = false, CancellationToken token = default)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = Name,
                Arguments = args,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true,
            });

            if (wait)
            {
                await process.WaitForExitAsync(token);
                return process.ExitCode == 0;
            }

            return true;
        }

        public bool Equals(Studio other)
            => other.Name == Name && other.FriendlyName == FriendlyName;

        public override bool Equals(object obj)
            => obj is Studio studio && Equals(studio);

        public override int GetHashCode()
            => HashCode.Combine(Name, FriendlyName);
    }
}
