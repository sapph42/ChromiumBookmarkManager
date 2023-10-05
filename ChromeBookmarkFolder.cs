using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Linq;
#nullable enable
namespace ChromeBookmarkMerge {
    internal class ChromeBookmarkFolder {
#pragma warning disable IDE1006 // Naming Styles
        public List<object>? children { get; set; } = null;
        public string? date_added { get; set; } = null;
        public string? date_last_used { get; set; } = null;
        public string? date_modified { get; set; } = null;
        public string? guid { get; set; } = null;
        public string? id { get; set; } = null;
        public string? name { get; set; } = null;
        public string? type { get; set; } = null;
        private static string? UlongStringMax(string? a, string? b) {
            if (a is null && b is null)
                return null;
            if (a is null)
                return b;
            if (b is null)
                return a;
            return UInt64.Parse(a) > UInt64.Parse(b) ? a : b;
        }
#pragma warning restore IDE1006 // Naming Styles
        public void ImportJToken(JToken token) {
            date_added = token.Value<string>("date_added");
            date_last_used = token.Value<string>("date_last_used");
            date_modified = token.Value<string>("date_modified");
            guid = token.Value<string>("guid");
            id = token.Value<string>("id");
            name = token.Value<string>("name");
            type = "folder";
            children = new List<object>();
            JToken target = token.SelectToken("$.children")!;
            //if (target.SelectToken("$Value") is null)
            //    target = target.SelectToken("$[0]")!;
            foreach (JToken child in target) {
                if (child.Value<string>("type") == "url")
                    children.Add(child.ToObject<ChromeBookmarkItem>()!);
                if (child.Value<string>("type") == "folder") {
                    ChromeBookmarkFolder newFolder = new ChromeBookmarkFolder();
                    newFolder.ImportJToken(child);
                    children.Add(newFolder);
                }
            }
        }
        public JToken ExportJToken() {
            return JToken.FromObject(this);
        }
        public void Union(ChromeBookmarkFolder other, string value="live") {
            Debug.WriteLine($"Union started on {name}");
            string childFolderNamesJoined = string.Join(",", other.children.Where(c => c.GetType().Equals(typeof(ChromeBookmarkFolder))).Cast<ChromeBookmarkFolder>().Select(f => f.name).ToArray());
            if (other.name != name)
                return;
            date_modified = UlongStringMax(date_modified, other.date_modified);
            if (other.children is null)
                return;
            children ??= new List<object>(); //if children is null, instantiate
            foreach (object child in other.children) {
                if (child.GetType().Equals(typeof(ChromeBookmarkItem))) {
                    if (!children.Contains(child))
                        children.Add(child);
                }
                if (child.GetType().Equals(typeof(ChromeBookmarkFolder))) {
                    Debug.WriteLine($"Testing {value} folder: {((ChromeBookmarkFolder)child).name}");
                    foreach (object localchild in children) {
                        if (localchild.GetType().Equals(typeof(ChromeBookmarkItem)))
                            continue;
                        Debug.WriteLine($" -- Against backup folder: {((ChromeBookmarkFolder)localchild).name}");
                        ChromeBookmarkFolder templocal = (ChromeBookmarkFolder)localchild;
                        ChromeBookmarkFolder tempother = (ChromeBookmarkFolder)child;
                        if (templocal.name == tempother.name) {
                            Debug.WriteLine($" ---- Union perform");
                            ((ChromeBookmarkFolder)localchild).Union((ChromeBookmarkFolder)child);
                        }
                        continue;
                    }
                }
            }
            List<object> otherchildfolders = other.children.Where(c => c.GetType().Equals(typeof(ChromeBookmarkFolder))).ToList<object>();
            if (otherchildfolders.Count == 0) {
                Debug.WriteLine($"Union complete on {name} - 85");
                return; //The other folder has no child folders are this level.  Merge unneccessary
            }
            List<ChromeBookmarkFolder> typedOtherFolders = otherchildfolders.Cast<ChromeBookmarkFolder>().ToList<ChromeBookmarkFolder>();
            List<string?> otherFolderNames = typedOtherFolders.Select(f => f.name).ToList<string?>();
            List<object> childfolders = children.Where(c => c.GetType().Equals(typeof(ChromeBookmarkFolder))).ToList<object>();
            if (childfolders.Count == 0) {
                // this folder has no child folders at this level. Absorb all child folders from other folder.
                foreach (ChromeBookmarkFolder folder in typedOtherFolders) {
                    Debug.WriteLine($"Copying {folder.name} to {name}");
                    children.Add(folder);
                }
                Debug.WriteLine($"Union complete on {name} - 97");
                return;
            }
            List<ChromeBookmarkFolder> typedFolders = childfolders.Cast<ChromeBookmarkFolder>().ToList<ChromeBookmarkFolder>();
            List<string?> folderNames = typedFolders.Select(f => f.name).ToList<string?>();
            List<string?> needsCopyingNames = otherFolderNames.Except(folderNames).ToList<string?>();
            if (needsCopyingNames is null) {
                Debug.WriteLine($"Union complete on {name} - 104");
                return; //All folders match between both this and other folder.  Merge will be handled above.
            }
            IEnumerable<ChromeBookmarkFolder> needsCopyingFolders = typedOtherFolders.Where(c => needsCopyingNames.Contains(c.name));
            foreach (ChromeBookmarkFolder folder in needsCopyingFolders) {
                Debug.WriteLine($"Copying {folder.name} to {name}");
                children.Add(folder);
            }
            Debug.WriteLine($"Union complete on {name} - 112");
        }
        public bool Equals(ChromeBookmarkFolder? other) {
            if (other is null)
                return false;
            if (name != other.name)
                return false;
            if (children is null && other.children is null)
                return true;
            if (children is null ^ other.children is null)
                return false;
            if (children is null)
                throw new NullReferenceException();
            if (other.children is null)
                throw new NullReferenceException();
            if (children.Count == 0 && other.children.Count == 0)
                return true;
            foreach (ChromeBookmarkItem child in children.Where(c => c is ChromeBookmarkItem).Cast<ChromeBookmarkItem>()) {
                if (!other.children.Contains(child))
                    return false;
            }
            foreach (ChromeBookmarkItem child in other.children.Where(c => c is ChromeBookmarkItem).Cast<ChromeBookmarkItem>()) {
                if (!children.Contains(child))
                    return false;
            }
            foreach (ChromeBookmarkFolder child in children.Where(c => c is ChromeBookmarkFolder).Cast<ChromeBookmarkFolder>()) {
                if (!other.children.Contains(child))
                    return false;
            }
            foreach (ChromeBookmarkFolder child in other.children.Where(c => c is ChromeBookmarkFolder).Cast<ChromeBookmarkFolder>()) {
                if (!children.Contains(child))
                    return false;
            }
            return true;
        }
        public override bool Equals(object obj) => Equals(obj as ChromeBookmarkFolder);
        public override int GetHashCode() {
            if (children is null) {
                if (name is null)
                    return 0;
                return name.GetHashCode();
            }
            return (name, children).GetHashCode();
        }
    }
}
