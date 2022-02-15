// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.IO.Serialization
{
    public class GroupCollectionConverter : JsonConverter<GroupCollection>
    {
        public override GroupCollection ReadJson(JsonReader reader, Type objectType, GroupCollection existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = existingValue ?? new GroupCollection();
            var entries = serializer.Deserialize<IEnumerable<Group>>(reader);
            value.AddRange(entries);
            return value;
        }

        public override void WriteJson(JsonWriter writer, GroupCollection value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.Bindable);
        }
    }
}
