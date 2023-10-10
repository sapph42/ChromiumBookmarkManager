using System;
using System.Collections.Generic;
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
        public BookmarkFile(string BookmarkFilePath) {
            FilePath = BookmarkFilePath;
            LoadFile();
        }
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
        public bool Merge(BookmarkFile OtherFile, out BookmarkFile Result) {
            Result = new BookmarkFile();
            if (!LoadFile() && !OtherFile.LoadFile()) 
                return false;
            if (fileJson is null && OtherFile.fileJson is null)
                return false;
            if (fileInfo is null || OtherFile.fileInfo is null)
                return false;
            if (!fileInfo.Exists || fileJson is null) {
                Result.fileJson = OtherFile.fileJson;
                return true;
            }
            if (!OtherFile.fileInfo.Exists || OtherFile.fileJson is null) {
                Result.fileJson = fileJson;
                return true;
            }
            try {
                Dictionary<string, BookmarkFolder> roots = new Dictionary<string, BookmarkFolder>();
                Dictionary<string, BookmarkFolder> otherRoots = new Dictionary<string, BookmarkFolder>();
                foreach (var root in fileJson.SelectToken("$.roots")!.Children<JProperty>()) {
                    if (root.First != null)
                        roots.Add(root.Name, new BookmarkFolder(root.First));
                }
                foreach (var root in OtherFile.fileJson.SelectToken("$.roots")!.Children<JProperty>()) {
                    if (root.First != null)
                        otherRoots.Add(root.Name, new BookmarkFolder(root.First));
                }
                foreach(var root in roots) {
                    if (otherRoots.ContainsKey(root.Key)) {
                        otherRoots[root.Key].Union(root.Value);
                    } else {
                        otherRoots.Add(root.Key, root.Value);
                    }
                }
                JObject outputParent = OtherFile.fileJson;
                outputParent.RemoveAll();
                JObject outputRoots = new JObject();
                foreach (var root in otherRoots) {
                    outputRoots.Add(new JProperty(root.Key, JProperty.FromObject(root.Value)));
                }
                outputParent.Add(new JProperty("roots", outputRoots));
                outputParent.Add(new JProperty("version", 1));
                Result.fileJson = (JObject?)outputParent;
            } catch (Exception ex) {
                Result = new BookmarkFile();
                return false;
            }
            return true;
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
