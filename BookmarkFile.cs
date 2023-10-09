using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#nullable enable
namespace ChromiumBookmarkManager {
    public class BookmarkFile {
        private JObject? fileJson;
        private FileInfo? fileInfo;
        public string? FilePath { get; set; }
        public BookmarkFile() { }
        public BookmarkFile(string BookmarkFilePath) => FilePath = BookmarkFilePath;
        private bool LoadFile() {
            if (FilePath is null)
                return false;
            fileInfo = new FileInfo(FilePath);
            if (!fileInfo.Exists) 
                return false;
            string file_contents;
            try {
                using (FileStream fileStream = fileInfo.OpenRead()) {
                    using StreamReader streamReader = new StreamReader(fileStream);
                    file_contents = streamReader.ReadToEnd();
                }
                fileJson = JObject.Parse(file_contents);
                fileJson.Remove("checksum");
            } catch {
                return false;
            }
            return true;
        }
        public BookmarkFile? Merge(BookmarkFile OtherFile) {
            if (!LoadFile() && !OtherFile.LoadFile()) 
                return null;
            if (fileJson is null && OtherFile.fileJson is null)
                return null;
            BookmarkFile destination = new BookmarkFile();
            if (fileInfo is null || OtherFile.fileInfo is null) 
                return null;
            if (!fileInfo.Exists || fileJson is null) {
                destination.fileJson = OtherFile.fileJson;
                return destination;
            }
            if (!OtherFile.fileInfo.Exists || OtherFile.fileJson is null) {
                destination.fileJson = fileJson;
                return destination;
            }
            try {
                BookmarkFolder this_bookmark_bar = new BookmarkFolder();
                BookmarkFolder this_other = new BookmarkFolder();
                BookmarkFolder other_bookmark_bar = new BookmarkFolder();
                BookmarkFolder other_other = new BookmarkFolder();
                this_bookmark_bar.ImportJToken(fileJson.SelectToken("$.roots.bookmark_bar")!);
                this_other.ImportJToken(fileJson.SelectToken("$.roots.other")!);
                other_bookmark_bar.ImportJToken(OtherFile.fileJson.SelectToken("$.roots.bookmark_bar")!);
                other_other.ImportJToken(OtherFile.fileJson.SelectToken("$.roots.other")!);
                other_bookmark_bar.Union(this_bookmark_bar);
                other_other.Union(this_other);
                destination.fileJson = OtherFile.fileJson;
                destination.fileJson.SelectToken("$.roots.bookmark_bar")!.Replace(other_bookmark_bar.ExportJToken());
                destination.fileJson.SelectToken("$.roots.other")!.Replace(other_other.ExportJToken());
                return destination;
            } catch {
                return null;
            }
        }
        public void WriteFile(string? file_name = null) {
            if (fileJson is null)
                throw new NullReferenceException("No JSON data loaded in BookmarkFile object");
            if (file_name is null) {
                if (FilePath is null)
                    throw new NullReferenceException("No path provided.");
                else
                    file_name = FilePath;
            }
            FileInfo target = new FileInfo(file_name);
            target.Delete();
            using (FileStream fileStream = target.OpenWrite()) {
                byte[] data = new UTF8Encoding(true).GetBytes(fileJson.ToString(Formatting.Indented));
                fileStream.Write(data, 0, data.Length);
            }
        }
    }
}
#nullable disable