// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using osu.Framework.Platform;
using osu.Game.Rulesets;

namespace sbtw.Editor.Tests
{
    public abstract class GameHostTest
    {
        protected static TestEditor LoadEditor(GameHost host)
        {
            var editor = new TestEditor();
            Task.Factory.StartNew(() => host.Run(editor), TaskCreationOptions.LongRunning)
                .ContinueWith(t => Assert.Fail($"Host threw an exception ${t.Exception}"), TaskContinuationOptions.OnlyOnFaulted);

            wait(() => editor.IsLoaded);

            bool loaded = false;
            host.UpdateThread.Scheduler.Add(() => host.UpdateThread.Scheduler.Add(() => loaded = true));

            wait(() => loaded);

            return editor;
        }

        private static void wait(Func<bool> condition, int timeout = 60000)
        {
            Task task = Task.Run(() =>
            {
                while (!condition())
                    Thread.Sleep(200);
            });

            Assert.IsTrue(task.Wait(timeout), @"Failed to load editor in time.");
        }

        protected class TestEditor : EditorBase
        {
            public new RulesetStore RulesetStore => base.RulesetStore;
        }
    }
}
