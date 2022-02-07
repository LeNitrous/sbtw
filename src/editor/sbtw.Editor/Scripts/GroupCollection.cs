// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections;
using System.Collections.Generic;
using osu.Framework.Bindables;
using sbtw.Editor.Extensions;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Scripts
{
    public class GroupCollection : IList<Group>, ICollection<Group>, IEnumerable<Group>, IEnumerable
    {
        public IBindableList<Group> Bindable => groups;
        public int Count => groups.Count;
        public bool IsReadOnly => false;

        public Group this[int index]
        {
            get => groups[index];
            set => Insert(index, value);
        }

        private readonly BindableList<Group> groups = new BindableList<Group>();

        public GroupCollection(IEnumerable<Group> initial = null)
        {
            if (initial != null)
                AddRange(initial);
        }

        public void Add(Group item)
        {
            item.Target.ValueChanged += handleTargetChange;
            item.Visible.ValueChanged += handleVisibilityChange;
            groups.Add(item);
        }

        public void AddRange(IEnumerable<Group> items)
        {
            foreach (var item in items)
                Add(item);
        }

        public bool Remove(Group item)
        {
            if (!Contains(item))
                return false;

            item.Target.ValueChanged -= handleTargetChange;
            item.Visible.ValueChanged -= handleVisibilityChange;
            groups.Remove(item);

            return true;
        }

        public void RemoveAt(int index)
        {
            if (index > groups.Count && index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            var group = groups[index];
            Remove(group);
        }

        public void Clear()
        {
            foreach (var group in groups)
                Remove(group);
        }

        public void Insert(int index, Group item)
        {
            item.Target.ValueChanged += handleTargetChange;
            item.Visible.ValueChanged += handleVisibilityChange;
            groups.Insert(index, item);
        }

        public bool Contains(Group item)
            => groups.Contains(item);

        public void CopyTo(Group[] array, int arrayIndex)
            => groups.CopyTo(array, arrayIndex);

        public IEnumerator<Group> GetEnumerator()
            => groups.GetEnumerator();

        public int IndexOf(Group item)
            => groups.IndexOf(item);

        IEnumerator IEnumerable.GetEnumerator()
            => groups.GetEnumerator();

        private void handleVisibilityChange(ValueChangedEvent<bool> e)
            => groups.TriggerChange();

        private void handleTargetChange(ValueChangedEvent<ExportTarget> e)
            => groups.TriggerChange();
    }
}
