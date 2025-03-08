using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChromiumBookmarkManager {
    internal class BookmarkUrlConverter : JsonConverter<BookmarkUrl> {
        public override BookmarkUrl Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            return JsonSerializer.Deserialize<BookmarkUrl>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, BookmarkUrl value, JsonSerializerOptions options) {
            writer.WriteStartObject();

            writer.WriteString("date_added", value.DateAdded);
            writer.WriteString("date_last_used", value.DateLastUsed);
            writer.WriteString("guid", value.Guid);
            writer.WriteString("id", value.Id);
            writer.WriteString("name", value.Name);
            writer.WriteBoolean("show_icon", value.ShowIcon);
            writer.WriteString("source", value.Source);
            writer.WriteString("type", value.Type);
            writer.WriteString("url", value.Url);
            writer.WriteNumber("visit_count", value.VisitCount);

            writer.WriteEndObject();
        }
    }
}