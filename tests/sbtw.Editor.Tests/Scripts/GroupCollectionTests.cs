// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using NUnit.Framework;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Tests.Scripts
{
    public class GroupCollectionTests
    {
        [Test]
        public void TestGroupAdding()
        {
            var groups = new GroupCollection();

            Assert.That(groups, Is.Empty);

            groups.Add(new Group("Test"));

            Assert.That(groups.Count, Is.EqualTo(1));

            groups.Add(new Group("Test"));

            Assert.That(groups.Count, Is.EqualTo(1));

            groups.Add(new Group("Test 2"));

            Assert.That(groups.Count, Is.EqualTo(2));
        }

        [Test]
        public void TestGroupRemoving()
        {
            var groups = new GroupCollection();
            var a = new Group("A");

            groups.Add(a);

            Assert.That(groups, Is.Not.Empty);
            Assert.That(a.Provider, Is.Not.Null);
            Assert.That(groups.Remove(a), Is.True);
            Assert.That(groups, Is.Empty);
            Assert.That(a.Provider, Is.Null);
            Assert.That(groups.Remove(a), Is.False);
        }

        [Test]
        public void TestGroupPropertyChange()
        {
            var groups = new GroupCollection();
            var a = new Group("A");

            groups.Add(a);

            int count = 0;
            groups.GroupPropertyChanged += _ => count++;

            a.Target.Value = ExportTarget.Difficulty;

            Assert.That(count, Is.EqualTo(1));

            groups.Remove(a);

            a.Target.Value = ExportTarget.Storyboard;

            Assert.That(count, Is.EqualTo(1));
        }
    }
}
