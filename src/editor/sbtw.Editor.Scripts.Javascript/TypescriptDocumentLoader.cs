// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using System.Threading.Tasks;
using Microsoft.ClearScript;

namespace sbtw.Editor.Scripts.Javascript
{
    public class TypescriptDocumentLoader : DocumentLoader
    {
        public override async Task<Document> LoadDocumentAsync(DocumentSettings settings, DocumentInfo? sourceInfo, string specifier, DocumentCategory category, DocumentContextCallback contextCallback)
        {
            Default.DiscardCachedDocuments();
            var document = await Default.LoadDocumentAsync(settings, sourceInfo, specifier, category, contextCallback);

            using var reader = new StreamReader(document.Contents);
            using var typescript = new RuntimeUtilities();
            var transpiled = typescript.Transpile(document.Info.Uri.AbsolutePath, await reader.ReadToEndAsync(), out string source);

            return new StringDocument(transpiled, source);
        }
    }
}
