using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

                string propertyName = reader.GetString();
                reader.Read();

                switch (propertyName) {
                    case "bookmark_bar":
                        roots.BookmarkBar = JsonSerializer.Deserialize<BookmarkFolder>(ref reader, options);
                        break;
                    case "other":
                        roots.Other = JsonSerializer.Deserialize<BookmarkFolder>(ref reader, options);
                        break;
                    case "synced":
                        roots.Synced = JsonSerializer.Deserialize<BookmarkFolder>(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
            throw new JsonException("Unexpected end of JSON while deserializing Roots");
        }

        public override void Write(Utf8JsonWriter writer, BookmarkRoots value, JsonSerializerOptions options) {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
