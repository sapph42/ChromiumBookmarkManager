using System;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace SapphTools.BookmarkManager.Chromium {
    internal class BookmarkItemConverter : JsonConverter<BookmarkItem> {
        public override BookmarkItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            JsonElement root = doc.RootElement;
            if (root.TryGetProperty("type", out JsonElement typeElement)) {
                string type = typeElement.GetString() ?? "";
                if (type == "folder") {
                    BookmarkFolder? folder = JsonSerializer.Deserialize<BookmarkFolder>(root.GetRawText(), options);
                    if (folder is null)
                        throw new JsonException("Could not deserialize folder");
                    else
                        return folder;
                } else if (type == "url") {
                    BookmarkUrl? url = JsonSerializer.Deserialize < BookmarkUrl >(root.GetRawText(), options);
                    if (url is null)
                        throw new JsonException("Could not deserialize url");
                    else
                        return url;
                }
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
#nullable disable
