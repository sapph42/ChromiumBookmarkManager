using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace ChromiumBookmarkManager {
    public class BookmarkFile {

        [JsonPropertyName("roots")]
        [JsonInclude]
        BookmarkRoots Roots { get; set; }

        [JsonPropertyName("version")]
        [JsonInclude]
        int Version => 1;

        [JsonIgnore]
        public int FolderCount => Roots.FolderCount;

        [JsonIgnore]
        public int UrlCount => Roots.FolderCount;

        public BookmarkFile(BookmarkRoots roots) {
            Roots = roots;
        }
        public static bool LoadFile(string path, out BookmarkFile? file) {
            file = null;
            if (string.IsNullOrWhiteSpace(path))
                return false;
            if (!File.Exists(path))
                return false;
            try {
                string json = File.ReadAllText(path);
                file = JsonSerializer.Deserialize<BookmarkFile>(json, BookmarkSerialization.Options);
            } catch {
                return false;
            }
            return true;
        }
        public void Merge(BookmarkFile otherFile) {
            Roots.Merge(otherFile.Roots);
        }
        public string WriteFile() {
            return JsonSerializer.Serialize<BookmarkFile>(this, BookmarkSerialization.Options);
        }
    }
}
#nullable disable
