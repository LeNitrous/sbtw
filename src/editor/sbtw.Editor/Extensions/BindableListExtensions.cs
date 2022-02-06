// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using osu.Framework.Bindables;

namespace sbtw.Editor.Extensions
{
    public static class BindableListExtensions
    {
        public static void TriggerChange<T>(this BindableList<T> bindable)
        {
            MethodInfo method = bindable.GetType().GetMethod("notifyCollectionChanged", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(bindable, new object[] { new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Enumerable.Empty<T>()) });
        }
    }
}
