// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Linq;
using sbtw.Editor.Platform;

namespace sbtw.Desktop.Windows.Extensions
{
    public static class PickerFilterExtensions
    {
        public static string ToFilterString(this PickerFilter filter)
            => $"{filter.Description}|{string.Join(';', filter.Files)}";

        public static string ToFilterString(this IEnumerable<PickerFilter> filters)
            => string.Join('|', filters.Select(f => f.ToFilterString()));
    }
}
