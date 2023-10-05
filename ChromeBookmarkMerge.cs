using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChromeBookmarkMerge {
    public class ChromeBookmarkMerge {
        private FileInfo bookmark_file_1;
        private FileInfo bookmark_file_2;
        public string InputFile1 {
            get => InputFile1; 
            set {
                InputFile1 = value;
                bookmark_file_1 = new FileInfo(value);
            }
        }
        public string InputFile2 { 
            get => InputFile2; 
            set {
                InputFile2 = value;
                bookmark_file_2 = new FileInfo(value);
            } 
        }

        public ChromeBookmarkMerge(string InputFile1, string InputFile2) {
            this.InputFile1 = InputFile1;
            this.InputFile2 = InputFile2;
        }
        public bool Merge(string Destination) {
            if (!bookmark_file_1.Exists && !bookmark_file_2.Exists) {
                return false;
            }
            FileInfo destination_file = new FileInfo(Destination);
            if (!destination_file.Directory.Exists) {
                return false;
            }
            foreach (Process chromeproc in Process.GetProcessesByName("chrome")) {
                chromeproc.Kill();
                chromeproc.WaitForExit();
            }
            if (!bookmark_file_1.Exists) {
                bookmark_file_2.CopyTo(Destination);
                return true;
            }
            if (!bookmark_file_2.Exists) {
                bookmark_file_1.CopyTo(Destination);
                return true;
            }
            try {
                string file1_contents;
                string file2_contents;
                using (FileStream fileStream = bookmark_file_1.OpenRead()) {
                    using StreamReader streamReader = new StreamReader(fileStream);
                    file1_contents = streamReader.ReadToEnd();
                }
                using (FileStream fileStream = bookmark_file_2.OpenRead()) {
                    using StreamReader streamReader = new StreamReader(fileStream);
                    file2_contents = streamReader.ReadToEnd();
                }
                JObject file1_Json = JObject.Parse(file1_contents);
                JObject file2_Json = JObject.Parse(file2_contents);
                file1_Json.Remove("checksum");
                file2_Json.Remove("checksum");
                ChromeBookmarkFolder file1_bookmark_bar = new ChromeBookmarkFolder();
                ChromeBookmarkFolder file1_other = new ChromeBookmarkFolder();
                ChromeBookmarkFolder file2_bookmark_bar = new ChromeBookmarkFolder();
                ChromeBookmarkFolder file2_other = new ChromeBookmarkFolder();
                file1_bookmark_bar.ImportJToken(file1_Json.SelectToken("$.roots.bookmark_bar")!);
                file1_other.ImportJToken(file1_Json.SelectToken("$.roots.other")!);
                file2_bookmark_bar.ImportJToken(file2_Json.SelectToken("$.roots.bookmark_bar")!);
                file2_other.ImportJToken(file2_Json.SelectToken("$.roots.other")!);
                file2_bookmark_bar.Union(file1_bookmark_bar);
                file2_other.Union(file1_other);
                file2_Json.SelectToken("$.roots.bookmark_bar")!.Replace(file2_bookmark_bar.ExportJToken());
                file2_Json.SelectToken("$.roots.other")!.Replace(file2_other.ExportJToken());
                destination_file.Delete();
                using (FileStream fileStream = destination_file.OpenWrite()) {
                    byte[] data = new UTF8Encoding(true).GetBytes(file2_Json.ToString(Formatting.Indented));
                    fileStream.Write(data, 0, data.Length);
                }
                return true;
            } catch {
                return false;
            }
        }
    }
}
