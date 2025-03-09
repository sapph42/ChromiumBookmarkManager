using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChromiumBookmarkManager {
    internal class BookmarkFolderConverter : JsonConverter<BookmarkFolder> {
        public override BookmarkFolder Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected StartObject token");

            var folder = new BookmarkFolder();

            while (reader.Read()) {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return folder;
                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Expected PropertyName token");

                string propertyName = reader.GetString();
                reader.Read();
                
                switch (propertyName) {
                    case "date_added":
                        folder.DateAdded = reader.GetString();
                        break;
                    case "date_last_used":
                        folder.DateLastUsed = reader.GetString();
                        break;
                    case "date_modified":
                        folder.DateModified = reader.GetString();
                        break;
                    case "guid":
                        folder.Guid = reader.GetString();
                        break;
                    case "id":
                        folder.Id = reader.GetString();
                        break;
                    case "source":
                        folder.Source = reader.GetString();
                        break;
                    case "type":
                        string type = reader.GetString();
                        if (type != "folder")
                            throw new JsonException($"Unexpected type '{type}', expected 'folder'");
                        break;
                    case "children":
                        if (reader.TokenType == JsonTokenType.StartArray) {
                            var children = new List<BookmarkItem>();
                            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray) {
                                var child = JsonSerializer.Deserialize<BookmarkItem>(ref reader, options);
                                if (child != null)
                                    children.Add(child);
                            }
                            folder.Children = children;
                        } else {
                            throw new JsonException("Expected StartArray token");
                        }
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            throw new JsonException("Unexpected end of JSON while deserializing BookmarkFolder");
        }
        public override void Write(Utf8JsonWriter writer, BookmarkFolder value, JsonSerializerOptions options) {
            writer.WriteStartObject();

            if (value.Children is null || value.Children.Count == 0) {
                writer.WritePropertyName("children");
                writer.WriteStartArray();
                writer.WriteEndArray();
            } else {
                writer.WritePropertyName("children");
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