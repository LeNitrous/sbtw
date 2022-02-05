// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Platform;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Languages
{
    public class LanguageStore : IDisposable
    {
        private readonly List<ILanguage> languages = new List<ILanguage>();
        private bool isDisposed;

        public IReadOnlyList<ILanguage> Languages => languages;
        public IEnumerable<string> Extensions => Languages.SelectMany(lang => lang.Extensions);

        public void Register(ILanguage language)
            => languages.Add(language);

        public void Unregister(ILanguage language)
            => languages.Remove(language);

        public async Task<IEnumerable<Script>> CompileAsync(Storage storage, IEnumerable<string> ignore = null, CancellationToken token = default)
            => (await Task.WhenAll(languages.Select(s => s.CompileAsync(storage, ignore, token)))).SelectMany(c => c);

        public IEnumerable<Script> Compile(Storage storage, IEnumerable<string> ignore = null)
            => CompileAsync(storage, ignore).Result;

        public ILanguage GetLanguageFor(string file)
        {
            foreach (var lang in languages)
            {
                if (lang.Extensions.Contains(Path.GetExtension(file)))
                    return lang;
            }

            return null;
        }

        public void Reset()
        {
            foreach (var lang in languages)
                (lang as Language)?.Reset();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed && !disposing)
                return;

            foreach (var lang in languages)
                lang?.Dispose();

            isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
