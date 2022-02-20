// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NativeFileDialogs.AutoGen;
using sbtw.Editor.Platform;

namespace sbtw.Desktop.Linux
{
    public class NfdPicker : Picker
    {
        private static NfdnfilteritemT[] toNfdFilters(IReadOnlyList<PickerFilter> filters)
        {
            return filters.Select((filter, _i) => new NfdnfilteritemT
            {
                Name = filter.Description,
                Spec = string.Join(@",", filter.Files
                    .Select((file) => file.StartsWith("*.") ? file[2..] : file)),
            }).ToArray();
        }

        protected sealed override async Task<IEnumerable<string>> OpenFileAsync(string title, string suggestedPath, IReadOnlyList<PickerFilter> filters, bool allowMultiple)
        {
            return await Task.Run(() =>
            {
                nfd.NFD_Init();

                NfdnfilteritemT[] nfdFilters = toNfdFilters(filters);
                string[] files = Array.Empty<string>();

                unsafe
                {
                    if (allowMultiple)
                    {
                        IntPtr outPathsPtr;
                        NfdresultT result = nfd.NFD_OpenDialogMultipleN(&outPathsPtr, nfdFilters, (uint)nfdFilters.Length, suggestedPath);
                        switch (result)
                        {
                            case NfdresultT.NFD_OKAY:
                                uint numPaths = 0;
                                nfd.NFD_PathSetGetCount(outPathsPtr, ref numPaths);
                                files = new string[numPaths];
                                for (uint i = 0; i < numPaths; i++)
                                {
                                    sbyte* outPathPtr;
                                    nfd.NFD_PathSetGetPathN(outPathsPtr, i, &outPathPtr);
                                    files[i] = new string(outPathPtr);
                                }
                                nfd.NFD_PathSetFree(outPathsPtr);
                                break;
                            case NfdresultT.NFD_CANCEL:
                                nfd.NFD_PathSetFree(outPathsPtr);
                                break;
                            default:
                                nfd.NFD_PathSetFree(outPathsPtr);
                                string err = $@"NFD error: {nfd.NFD_GetError()}";
                                nfd.NFD_Quit();
                                throw new IOException(err);
                        }
                    }
                    else
                    {
                        sbyte* outPathPtr;
                        NfdresultT result = nfd.NFD_OpenDialogN(&outPathPtr, nfdFilters, (uint)nfdFilters.Length, suggestedPath);
                        switch (result)
                        {
                            case NfdresultT.NFD_OKAY:
                                files = new string[] { new string(outPathPtr) };
                                break;
                            case NfdresultT.NFD_CANCEL:
                                break;
                            default:
                                string err = $@"NFD error: {nfd.NFD_GetError()}";
                                nfd.NFD_Quit();
                                throw new IOException(err);
                        }
                    }
                }

                nfd.NFD_Quit();
                return files;
            });
        }

        protected sealed override async Task<string> OpenFolderAsync(string title, string suggestedPath)
        {
            return await Task.Run(() =>
            {
                nfd.NFD_Init();

                string folder = null;

                unsafe
                {
                    sbyte* outPathPtr;
                    NfdresultT result = nfd.NFD_PickFolderN(&outPathPtr, suggestedPath);
                    switch (result)
                    {
                        case NfdresultT.NFD_OKAY:
                            folder = new string(outPathPtr);
                            break;
                        case NfdresultT.NFD_CANCEL:
                            break;
                        default:
                            string err = $@"NFD error: {nfd.NFD_GetError()}";
                            nfd.NFD_Quit();
                            throw new IOException(err);
                    }
                }

                nfd.NFD_Quit();
                return folder;
            });
        }

        protected sealed override async Task<string> SaveFileAsync(string title, string suggestedFileName, string suggestedPath, IReadOnlyList<PickerFilter> filters)
        {
            return await Task.Run(() =>
            {
                nfd.NFD_Init();

                NfdnfilteritemT[] nfdFilters = toNfdFilters(filters);
                string folder = null;

                unsafe
                {
                    sbyte* savePathPtr;
                    NfdresultT result = nfd.NFD_SaveDialogN(&savePathPtr, nfdFilters, (uint)nfdFilters.Length, suggestedPath, suggestedFileName);
                    switch (result)
                    {
                        case NfdresultT.NFD_OKAY:
                            folder = new string(savePathPtr);
                            break;
                        case NfdresultT.NFD_CANCEL:
                            break;
                        default:
                            string err = $@"NFD error: {nfd.NFD_GetError()}";
                            nfd.NFD_Quit();
                            throw new IOException(err);
                    }
                }

                nfd.NFD_Quit();
                return folder;
            });
        }
    }
}
