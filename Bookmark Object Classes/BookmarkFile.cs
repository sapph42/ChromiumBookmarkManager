using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace SapphTools.BookmarkManager.Chromium {
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
                return Deserialize(json, out file);
            } catch {
                return false;
            }
        }
        public void Merge(BookmarkFile otherFile) {
            Roots.Merge(otherFile.Roots);
        }
        public static bool Deserialize(string json, out BookmarkFile? file) {
            file = null;
            try {
                file = JsonSerializer.Deserialize<BookmarkFile>(json, BookmarkSerialization.Options);
                return true;
            } catch {
                return false;
            }
        }
        public string Serialize() {
            return JsonSerializer.Serialize<BookmarkFile>(this, BookmarkSerialization.Options);
        }
    }
}
#nullable disable
