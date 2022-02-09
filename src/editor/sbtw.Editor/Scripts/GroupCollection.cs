// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using osu.Framework.Bindables;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Scripts
{
    public class GroupCollection : IList<Group>, ICollection<Group>, IEnumerable<Group>, IEnumerable
    {
        public event Action GroupPropertyChanged;
        public readonly BindableList<Group> Bindable = new BindableList<Group>();
        public int Count => Bindable.Count;
        public bool IsReadOnly => false;

        public Group this[int index]
        {
            get => Bindable[index];
            set => Insert(index, value);
        }

        public GroupCollection(IEnumerable<Group> initial = null)
        {
            Bindable.CollectionChanged += handleCollectionChange;

            if (initial != null)
                AddRange(initial);
        }

        public void Add(Group item)
        {
            if (Contains(item) || Bindable.Any(g => g.Name == item.Name))
                return;

            Bindable.Add(item);
        }

        public void AddRange(IEnumerable<Group> items)
        {
            foreach (var item in items)
                Add(item);
        }

        public bool Remove(Group item)
        {
            if (!Contains(item) || !Bindable.Any(g => g.Name == item.Name))
                return false;

            Bindable.Remove(item);

            return true;
        }

        public void RemoveAt(int index)
            => Bindable.RemoveAt(index);

        public void Clear()
            => Bindable.Clear();

        public void Insert(int index, Group item)
        {
            if (Contains(item) || item.Provider != null)
                return;

            Bindable.Insert(index, item);
        }

        public bool Contains(Group item)
            => Bindable.Contains(item);

        public void CopyTo(Group[] array, int arrayIndex)
            => Bindable.CopyTo(array, arrayIndex);

        public IEnumerator<Group> GetEnumerator()
            => Bindable.GetEnumerator();

        public int IndexOf(Group item)
            => Bindable.IndexOf(item);

        IEnumerator IEnumerable.GetEnumerator()
            => Bindable.GetEnumerator();

        private void handleVisibilityChange(ValueChangedEvent<bool> e)
            => GroupPropertyChanged?.Invoke();

        private void handleTargetChange(ValueChangedEvent<ExportTarget> e)
            => GroupPropertyChanged?.Invoke();

        private void registerGroup(Group item)
        {
            item.Provider = this;
            item.Target.ValueChanged += handleTargetChange;
            item.Visible.ValueChanged += handleVisibilityChange;
        }

        private void unregisterGroup(Group item)
        {
            item.Provider = null;
            item.Target.ValueChanged -= handleTargetChange;
            item.Visible.ValueChanged -= handleVisibilityChange;
        }

        private void handleCollectionChange(object sender, NotifyCollectionChangedEventArgs evt)
        {
            switch (evt.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in evt.NewItems.Cast<Group>())
                            registerGroup(item);

                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in evt.OldItems.Cast<Group>())
                            unregisterGroup(item);

                        break;
                    }

                case NotifyCollectionChangedAction.Reset:
                    {
                        foreach (var item in evt.OldItems.Cast<Group>())
                            unregisterGroup(item);

                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        foreach (var item in evt.NewItems.Cast<Group>())
                            registerGroup(item);

                        foreach (var item in evt.OldItems.Cast<Group>())
                            unregisterGroup(item);

                        break;
                    }
            }
        }
    }
}
