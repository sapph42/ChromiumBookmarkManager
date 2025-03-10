using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace SapphTools.BookmarkManager.Chromium {
    public class BookmarkFileConverter : JsonConverter<BookmarkFile> {
        public override BookmarkFile Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected StartObject token");

            BookmarkFile bookmarkFile = new BookmarkFile (new BookmarkRoots());

            while (reader.Read()) {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return bookmarkFile;
                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Expected PropertyName token");

                string propertyName = reader.GetString() ?? "";
                reader.Read(); // Move to value

                switch (propertyName) {
                    case "roots":
                        bookmarkFile.Roots = JsonSerializer.Deserialize<BookmarkRoots>(ref reader, options) ?? new BookmarkRoots();
                        break;
                    case "version":
                        try {
                            if (reader.GetInt16() != 1)
                                throw new JsonException("Versions higher than 1 not supported");
                        } catch (Exception e) { 
                            Debug.WriteLine(e.Message); 
                        }

                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            throw new JsonException("Unexpected end of JSON while deserializing BookmarkFile");
        }

        public override void Write(Utf8JsonWriter writer, BookmarkFile value, JsonSerializerOptions options) {
            writer.WriteStartObject();

            writer.WritePropertyName("roots");
            JsonSerializer.Serialize(writer, value.Roots, options);
            writer.WriteNumber("version", value.Version);

            writer.WriteEndObject();
        }
    }
}
#nullable disable
