using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SapphTools.BookmarkManager.Chromium {
    internal class BookmarkUrlConverter : JsonConverter<BookmarkUrl> {
        public override BookmarkUrl Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected StartObject token at the beginning of BookmarkUrl");

            var bookmarkUrl = new BookmarkUrl();

            while (reader.Read()) {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return bookmarkUrl; // End of object, return the constructed BookmarkUrl

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Expected PropertyName token");

                string propertyName = reader.GetString();
                reader.Read(); // Move to value

                switch (propertyName) {
                    case "date_added":
                        bookmarkUrl.DateAdded = reader.GetString();
                        break;
                    case "date_last_used":
                        bookmarkUrl.DateLastUsed = reader.GetString();
                        break;
                    case "guid":
                        bookmarkUrl.Guid = reader.GetString();
                        break;
                    case "id":
                        bookmarkUrl.Id = reader.GetString();
                        break;
                    case "name":
                        bookmarkUrl.Name = reader.GetString();
                        break;
                    case "source":
                        bookmarkUrl.Source = reader.GetString();
                        break;
                    case "type":
                        string type = reader.GetString();
                        if (type != "url")
                            throw new JsonException($"Unexpected type '{type}', expected 'url'");
                        break;
                    case "url":
                        bookmarkUrl.Url = reader.GetString();
                        break;
                    case "visit_count":
                        bookmarkUrl.VisitCount = reader.GetInt32();
                        break;
                    case "show_icon":
                        bookmarkUrl.ShowIcon = reader.GetBoolean();
                        break;
                    case "meta_info":
                        if (reader.TokenType == JsonTokenType.StartObject) {
                            bookmarkUrl.MetaInfo = JsonSerializer.Deserialize<MetaInfo>(ref reader, options);
                        } else {
                            throw new JsonException("Expected StartObject token for meta_info property");
                        }
                        break;
                    default:
                        reader.Skip(); // Ignore unknown properties
                        break;
                }
            }

            throw new JsonException("Unexpected end of JSON while deserializing BookmarkUrl");
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