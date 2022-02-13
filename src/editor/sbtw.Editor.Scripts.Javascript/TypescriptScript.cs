// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace sbtw.Editor.Scripts.Javascript
{
    public class TypescriptScript : JavascriptScript
    {
        public TypescriptScript(V8ScriptEngine engine, RuntimeUtilities utilities, string path)
            : base(engine, utilities, path)
        {
            Engine.DocumentSettings.Loader = new TypescriptDocumentLoader();
        }

        protected override DocumentInfo GetDocumentInfo(out string code)
            => Utilities.Transpile(Path, File.ReadAllText(Path), out code);
    }
}
