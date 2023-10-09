using System;
using System.Collections.Generic;
using System.Linq;
using ChromeBookmarkMerge;
using Newtonsoft.Json.Linq;
#nullable enable
namespace ChromiumBookmarkManager {
    internal class BookmarkFolder : BookmarkItem {
#pragma warning disable IDE1006 // Naming Styles
        public List<BookmarkItem>? children { get; set; } = null;
        public string? date_modified { get; set; } = null;
        public new string type { get; } = "folder";
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
            children = new List<BookmarkItem>();
            JToken target = token.SelectToken("$.children")!;
            foreach (JToken child in target) {
                if (child.Value<string>("type") == "url")
                    children.Add(child.ToObject<BookmarkUrl>()!);
                if (child.Value<string>("type") == "folder") {
                    BookmarkFolder newFolder = new BookmarkFolder();
                    newFolder.ImportJToken(child);
                    children.Add(newFolder);
                }
            }
        }
        public JToken ExportJToken() {
            return JToken.FromObject(this);
        }
        public void Union(BookmarkFolder other) {
            string childFolderNamesJoined = string.Join(",", other.children.Where(c => c.GetType().Equals(typeof(BookmarkFolder))).Cast<BookmarkFolder>().Select(f => f.name).ToArray());
            if (other.name != name)
                return;
            date_modified = UlongStringMax(date_modified, other.date_modified);
            if (other.children is null)
                return;
            children ??= new List<BookmarkItem>(); //if children is null, instantiate
            foreach (BookmarkItem child in other.children) {
                if (child.GetType().Equals(typeof(BookmarkUrl))) {
                    if (!children.Contains(child))
                        children.Add(child);
                }
                if (child.GetType().Equals(typeof(BookmarkFolder))) {
                    foreach (object localchild in children) {
                        if (localchild.GetType().Equals(typeof(BookmarkUrl)))
                            continue;
                        BookmarkFolder templocal = (BookmarkFolder)localchild;
                        BookmarkFolder tempother = (BookmarkFolder)child;
                        if (templocal.name == tempother.name) {
                            ((BookmarkFolder)localchild).Union((BookmarkFolder)child);
                        }
                        continue;
                    }
                }
            }
            List<object> otherchildfolders = other.children.Where(c => c.GetType().Equals(typeof(BookmarkFolder))).ToList<object>();
            if (otherchildfolders.Count == 0) {
                return; //The other folder has no child folders are this level.  Merge unneccessary
            }
            List<BookmarkFolder> typedOtherFolders = otherchildfolders.Cast<BookmarkFolder>().ToList<BookmarkFolder>();
            List<string?> otherFolderNames = typedOtherFolders.Select(f => f.name).ToList<string?>();
            List<object> childfolders = children.Where(c => c.GetType().Equals(typeof(BookmarkFolder))).ToList<object>();
            if (childfolders.Count == 0) {
                // this folder has no child folders at this level. Absorb all child folders from other folder.
                foreach (BookmarkFolder folder in typedOtherFolders) {
                    children.Add(folder);
                }
                return;
            }
            List<BookmarkFolder> typedFolders = childfolders.Cast<BookmarkFolder>().ToList<BookmarkFolder>();
            List<string?> folderNames = typedFolders.Select(f => f.name).ToList<string?>();
            List<string?> needsCopyingNames = otherFolderNames.Except(folderNames).ToList<string?>();
            if (needsCopyingNames is null) {
                return; //All folders match between both this and other folder.  Merge will be handled above.
            }
            IEnumerable<BookmarkFolder> needsCopyingFolders = typedOtherFolders.Where(c => needsCopyingNames.Contains(c.name));
            foreach (BookmarkFolder folder in needsCopyingFolders) {
                children.Add(folder);
            }
        }
        public bool Equals(BookmarkFolder? other) {
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
            foreach (BookmarkUrl child in children.Where(c => c is BookmarkUrl).Cast<BookmarkUrl>()) {
                if (!other.children.Contains(child))
                    return false;
            }
            foreach (BookmarkUrl child in other.children.Where(c => c is BookmarkUrl).Cast<BookmarkUrl>()) {
                if (!children.Contains(child))
                    return false;
            }
            foreach (BookmarkFolder child in children.Where(c => c is BookmarkFolder).Cast<BookmarkFolder>()) {
                if (!other.children.Contains(child))
                    return false;
            }
            foreach (BookmarkFolder child in other.children.Where(c => c is BookmarkFolder).Cast<BookmarkFolder>()) {
                if (!children.Contains(child))
                    return false;
            }
            return true;
        }
        public override bool Equals(object obj) => Equals(obj as BookmarkFolder);
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
