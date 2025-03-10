using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace SapphTools.BookmarkManager.Chromium {
    [JsonConverter(typeof(BookmarkFileConverter))]
    public class BookmarkFile {

        [JsonPropertyName("roots")]
        [JsonInclude]
        internal BookmarkRoots Roots { get; set; }

        [JsonPropertyName("version")]
        [JsonInclude]
        internal int Version => 1;

        [JsonIgnore]
        public int FolderCount => Roots.FolderCount;

        [JsonIgnore]
        public int UrlCount => Roots.UrlCount;

        public BookmarkFile(BookmarkRoots roots) {
            Roots = roots;
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
        private BookmarkFile Clone() {
            return JsonSerializer.Deserialize<BookmarkFile>(
                JsonSerializer.Serialize<BookmarkFile>(this, BookmarkSerialization.Options),
                BookmarkSerialization.Options
            )!;
        }
        #region Deprecated Methods
        [Obsolete("Use static Deserialize method for initialization.")]
        public BookmarkFile(string path) {
            _ = LoadFile(path, out BookmarkFile? newFile);
            Roots = newFile!.Roots;
        }
        [Obsolete("Use static Deserialize method for initialization.")]
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
        [Obsolete("Use Serialize method to return string and handle file write within your project.")]
        public void WriteFile(string path) {
            File.WriteAllText(path, Serialize());
        }
        [Obsolete("Update signature to void Merge(BookmarkFile otherFile)")]
        public bool Merge(BookmarkFile other, out BookmarkFile result) {
            try {
                result = Clone();
                result.Merge(other);
                return true;
            } catch {
                result = new BookmarkFile(new BookmarkRoots());
                return false;
            }
        }
        #endregion
    }
}
#nullable disable
