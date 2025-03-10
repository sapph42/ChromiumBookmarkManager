using System;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace SapphTools.BookmarkManager.Chromium {
    public class BookmarkRootsConverter : JsonConverter<BookmarkRoots> {
        public override BookmarkRoots Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected StartObject token");

            var roots = new BookmarkRoots();
            while (reader.Read()) {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return roots;
                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Expected PropertyName token");

                string propertyName = reader.GetString() ?? "";
                reader.Read();

                BookmarkFolder? root = JsonSerializer.Deserialize<BookmarkFolder>(ref reader, options);

                switch (propertyName) {
                    case "bookmark_bar":
                        if (root is null)
                            roots.BookmarkBar = BookmarkRoots.GenerateDefaultRoot(Roots.Bookmark_Bar);
                        else
                            roots.BookmarkBar = root;
                        break;
                    case "other":
                        if (root is null)
                            roots.BookmarkBar = BookmarkRoots.GenerateDefaultRoot(Roots.Other);
                        else
                            roots.Other = root;
                        break;
                    case "synced":
                        if (root is null)
                            roots.BookmarkBar = BookmarkRoots.GenerateDefaultRoot(Roots.Synced);
                        else
                            roots.Synced = root;
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
            throw new JsonException("Unexpected end of JSON while deserializing Roots");
        }

        public override void Write(Utf8JsonWriter writer, BookmarkRoots value, JsonSerializerOptions options) {
            writer.WriteStartObject();

            writer.WritePropertyName("bookmark_bar");
            JsonSerializer.Serialize(writer, value.BookmarkBar, options);
            writer.WritePropertyName("other");
            JsonSerializer.Serialize(writer, value.Other, options);
            writer.WritePropertyName("synced");
            JsonSerializer.Serialize(writer, value.Synced, options);

            writer.WriteEndObject();
        }
    }
}
#nullable disable
