// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using sbtw.Editor.Scripts.Assets;

namespace sbtw.Editor.IO.Serialization
{
    public class AssetConverter : JsonConverter<Asset>
    {
        public override Asset ReadJson(JsonReader reader, Type objectType, Asset existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (hasExistingValue)
                return existingValue;

            var entry = serializer.Deserialize<Dictionary<string, string>>(reader);

            if (!entry.ContainsKey("Type") || !entry.ContainsKey("Path"))
                return null;

            var asset = Activator.CreateInstance(Type.GetType(entry["Type"])) as Asset;
            asset.Path = entry["Path"];

            return asset;
        }

        public override void WriteJson(JsonWriter writer, Asset value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(value.GetType().FullName);
            writer.WritePropertyName("Path");
            writer.WriteValue(value.Path);
            writer.WriteEndObject();
        }
    }
}
