using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChromiumBookmarkManager {
    internal class BookmarkFolderConverter : JsonConverter<BookmarkFolder> {
        public override BookmarkFolder Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            return JsonSerializer.Deserialize<BookmarkFolder>(ref reader, options);
        }
        public override void Write(Utf8JsonWriter writer, BookmarkFolder value, JsonSerializerOptions options) {
            writer.WriteStartObject();

            if (value.Children is null || value.Children.Count == 0) {
                writer.WritePropertyName("children");
                writer.WriteStartArray();
                writer.WriteEndArray();
            } else {
                JsonSerializer.Serialize(writer, value.Children, options);
            }
            writer.WriteString("date_added", value.DateAdded);
            writer.WriteString("date_modified", value.DateModified);
            writer.WriteString("guid", value.Guid);
            writer.WriteString("id", value.Id);
            writer.WriteString("name", value.Name);
            writer.WriteString("source", value.Source);
            writer.WriteString("type", value.Type);

            writer.WriteEndObject();
        }
    }
}