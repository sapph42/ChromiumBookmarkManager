using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChromiumBookmarkManager {
    internal class BookmarkItemConverter : JsonConverter<BookmarkItem> {
        public override BookmarkItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            JsonElement root = doc.RootElement;
            if (root.TryGetProperty("type", out JsonElement typeElement)) {
                string type = typeElement.GetString();
                if (type == "folder")
                    return JsonSerializer.Deserialize<BookmarkFolder>(root.GetRawText(), options);
                else if (type == "url")
                    return JsonSerializer.Deserialize<BookmarkUrl>(root.GetRawText(), options);
            }
            throw new JsonException("Unknown bookmark item type");
        }
        public override void Write(Utf8JsonWriter writer, BookmarkItem value, JsonSerializerOptions options) {
            if (value is BookmarkFolder folder)
                JsonSerializer.Serialize(writer, folder, options);
            else if (value is BookmarkUrl url)
                JsonSerializer.Serialize(writer, url, options);
            else
                throw new JsonException("Unknown BookmarkItem type");
        }
    }
}
