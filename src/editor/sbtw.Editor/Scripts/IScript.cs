// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Scripts
{
    public interface IScript
    {
        string Path { get; }
        string Name { get; }

        byte[] OpenFile(string path);
        object SetValue(string name, object value);
        T SetValue<T>(string name, T value);
        object GetValue(string name);
        T GetValue<T>(string name);
        ScriptElementGroup GetGroup(string name);
        void SetVideo(string path, int offset);
    }
}
