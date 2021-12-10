// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using Python.Runtime;

namespace sbtw.Game.Scripting
{
    public class PythonExecutionException : Exception
    {
        public PythonExecutionException(string file, PythonException e)
            : base(get_message(file, e.Message), e)
        {
        }

        private static string get_message(string file, string message)
        {
            return $"File \"{file}\" has errors:\n{message.Replace("\n", "\n\t")}";
        }
    }
}
