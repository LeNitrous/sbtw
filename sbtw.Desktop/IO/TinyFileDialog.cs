// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace sbtw.Desktop.IO
{
    public static class TinyFileDialog
    {
        public static string OpenFileDialog(IEnumerable<string> filters, string filterDescription, bool allowMultiple = true)
            => Marshal.PtrToStringAnsi(tinyfd_openFileDialog("Open File", Environment.GetFolderPath(Environment.SpecialFolder.Personal), filters.Count(), filters.ToArray(), filterDescription, allowMultiple ? 1 : 0));

        public static string SaveFileDialog(string filename, IEnumerable<string> filters, string filterDescription)
            => Marshal.PtrToStringAnsi(tinyfd_saveFileDialog("Save File", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename), filters.Count(), filters.ToArray(), filterDescription));

        public static string OpenFolderDialog()
            => Marshal.PtrToStringAnsi(tinyfd_selectFolderDialog("Open Folder", Environment.GetFolderPath(Environment.SpecialFolder.Personal)));

        private const string library = "tinyfiledialogs64";

        [DllImport(library, CallingConvention = CallingConvention.Cdecl)]
        private static extern void tinyfd_beep();

        // False Positive: https://github.com/dotnet/roslyn-analyzers/issues/5479
#pragma warning disable CA2101

        [DllImport(library, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int tinyfd_notifyPopup(string aTitle, string aMessage, string aIconType);

        [DllImport(library, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int tinyfd_messageBox(string aTitle, string aMessage, string aDialogTyle, string aIconType, int aDefaultButton);

        [DllImport(library, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr tinyfd_inputBox(string aTitle, string aMessage, string aDefaultInput);

        [DllImport(library, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr tinyfd_saveFileDialog(string aTitle, string aDefaultPathAndFile, int aNumOfFilterPatterns, string[] aFilterPatterns, string aSingleFilterDescription);

        [DllImport(library, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr tinyfd_openFileDialog(string aTitle, string aDefaultPathAndFile, int aNumOfFilterPatterns, string[] aFilterPatterns, string aSingleFilterDescription, int aAllowMultipleSelects);

        [DllImport(library, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr tinyfd_selectFolderDialog(string aTitle, string aDefaultPathAndFile);

        [DllImport(library, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr tinyfd_colorChooser(string aTitle, string aDefaultHexRGB, byte[] aDefaultRGB, byte[] aoResultRGB);

#pragma warning restore CA2101

        [DllImport(library, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern int tinyfd_notifyPopupW(string aTitle, string aMessage, string aIconType);

        [DllImport(library, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern int tinyfd_messageBoxW(string aTitle, string aMessage, string aDialogTyle, string aIconType, int aDefaultButton);

        [DllImport(library, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr tinyfd_inputBoxW(string aTitle, string aMessage, string aDefaultInput);

        [DllImport(library, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr tinyfd_saveFileDialogW(string aTitle, string aDefaultPathAndFile, int aNumOfFilterPatterns, string[] aFilterPatterns, string aSingleFilterDescription);

        [DllImport(library, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr tinyfd_openFileDialogW(string aTitle, string aDefaultPathAndFile, int aNumOfFilterPatterns, string[] aFilterPatterns, string aSingleFilterDescription, int aAllowMultipleSelects);

        [DllImport(library, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr tinyfd_selectFolderDialogW(string aTitle, string aDefaultPathAndFile);

        [DllImport(library, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr tinyfd_colorChooserW(string aTitle, string aDefaultHexRGB, byte[] aDefaultRGB, byte[] aoResultRGB);

        [DllImport(library, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr tinyfd_getGlobalChar(string aCharVariableName);

        [DllImport(library, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int tinyfd_getGlobalInt(string aIntVariableName);

        [DllImport(library, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int tinyfd_setGlobalInt(string aIntVariableName, int aValue);

    }
}
