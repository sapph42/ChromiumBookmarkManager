using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

#nullable enable
namespace ChromiumBookmarkManager {
    [JsonConverter(typeof(BookmarkFolderConverter))]
    public  class BookmarkFolder : BookmarkItem<BookmarkFolder> {

        [JsonPropertyName("children")]
        [JsonInclude]
        public List<BookmarkItem> Children { get; set; } = new List<BookmarkItem>();

        [JsonPropertyName("date_modified")]
        [JsonInclude]
        public string? DateModified { get; set; } = null;

        [JsonPropertyName("type")]
        [JsonInclude]
        public override string Type => "folder";

        [JsonIgnore]
        public override int FolderCount => Children.Sum(bi => bi.FolderCount + bi.FolderVal);

        [JsonIgnore]
        public override int UrlCount => Children.Sum(bi => bi.UrlCount + bi.FolderVal);

        [JsonIgnore]
        public override int FolderVal { get; } = 1;

        [JsonIgnore]
        public override int FileVal { get; } = 0;
        public BookmarkFolder() {
            Children = new List<BookmarkItem>();
            string now = NowToBookmark();
            DateModified = now;
            DateAdded = now;
            DateLastUsed = "0";
            Id = "-1";
            Guid = new Guid().ToString();
            Name = "Default";
            Source = "extension";
        }
        public BookmarkFolder(
            List<BookmarkItem> children,
            string date_modified,
            string type,
            string date_added,
            string date_last_used,
            string guid,
            string id,
            string name,
            string source) : 
            this(children, date_modified, date_added, date_last_used, guid, id, name, source) { }
        public BookmarkFolder(
            List<BookmarkItem> children,
            string date_modified,
            string date_added,
            string date_last_used,
            string guid,
            string id,
            string name,
            string source) {
            Children = children;
            DateModified = date_modified;
            DateAdded = date_added;
            DateLastUsed = date_last_used;
            this.Guid = guid;
            Id = id;
            Name = name;
            Source = source;
        }
        public override void Merge(BookmarkFolder otherFolder) {
            DateAdded = UlongStringMin(DateAdded, otherFolder.DateAdded);
            DateLastUsed = UlongStringMax(DateLastUsed, otherFolder.DateLastUsed);
            DateModified = UlongStringMax(DateModified, otherFolder.DateModified);
            if (DateAdded == otherFolder.DateAdded)
                Guid = otherFolder.Guid;
            if (Children.Count == 0)
                Children = otherFolder.Children;
            if (otherFolder.Children.Count == 0)
                return;
            foreach (BookmarkItem item in otherFolder.Children) {
                if (item is BookmarkUrl url) {
                    if (!Children.Contains(url))
                        Children.Add(url);
                    else {
                        BookmarkUrl urlMatch = (BookmarkUrl)Children.First(bu => bu is BookmarkUrl urlMatch && bu.Id == url.Id && bu.Name == url.Name && urlMatch.Url == url.Url);
                        urlMatch.Merge(url);
                    }
                } else if (item is BookmarkFolder folder) {
                    if (Children.All(bf => bf.Id != folder.Id && bf.Name != folder.Name))
                        Children.Add(folder);
                    else {
                        BookmarkFolder folderMatch = (BookmarkFolder)Children.First(bf => bf.Id == folder.Id && bf.Name == folder.Name && bf is BookmarkFolder);
                        folderMatch.Merge(folder);
                    }
                }
            }
        }
        public void Union(BookmarkFolder other) {
            string childFolderNamesJoined = string.Join(",", other.Children.Where(c => c.GetType().Equals(typeof(BookmarkFolder))).Cast<BookmarkFolder>().Select(f => f.Name).ToArray());
            if (other.Name != Name)
                return;
            DateModified = UlongStringMax(DateModified, other.DateModified);
            if (other.Children is null)
                return;
            Children ??= new List<BookmarkItem>(); //if children is null, instantiate
            foreach (BookmarkItem child in other.Children) {
                if (child.GetType().Equals(typeof(BookmarkUrl))) {
                    if (!Children.Contains(child))
                        Children.Add(child);
                }
                if (child.GetType().Equals(typeof(BookmarkFolder))) {
                    foreach (object localchild in Children) {
                        if (localchild.GetType().Equals(typeof(BookmarkUrl)))
                            continue;
                        BookmarkFolder templocal = (BookmarkFolder)localchild;
                        BookmarkFolder tempother = (BookmarkFolder)child;
                        if (templocal.Name == tempother.Name) {
                            ((BookmarkFolder)localchild).Union((BookmarkFolder)child);
                        }
                        continue;
                    }
                }
            }
            List<object> otherchildfolders = other.Children.Where(c => c.GetType().Equals(typeof(BookmarkFolder))).ToList<object>();
            if (otherchildfolders.Count == 0) {
                return; //The other folder has no child folders are this level.  Merge unneccessary
            }
            List<BookmarkFolder> typedOtherFolders = otherchildfolders.Cast<BookmarkFolder>().ToList<BookmarkFolder>();
            List<string?> otherFolderNames = typedOtherFolders.Select(f => f.Name).ToList<string?>();
            List<object> childfolders = Children.Where(c => c.GetType().Equals(typeof(BookmarkFolder))).ToList<object>();
            if (childfolders.Count == 0) {
                // this folder has no child folders at this level. Absorb all child folders from other folder.
                foreach (BookmarkFolder folder in typedOtherFolders) {
                    Children.Add(folder);
                }
                return;
            }
            List<BookmarkFolder> typedFolders = childfolders.Cast<BookmarkFolder>().ToList<BookmarkFolder>();
            List<string?> folderNames = typedFolders.Select(f => f.Name).ToList<string?>();
            List<string?> needsCopyingNames = otherFolderNames.Except(folderNames).ToList<string?>();
            if (needsCopyingNames is null) {
                return; //All folders match between both this and other folder.  Merge will be handled above.
            }
            IEnumerable<BookmarkFolder> needsCopyingFolders = typedOtherFolders.Where(c => needsCopyingNames.Contains(c.Name));
            foreach (BookmarkFolder folder in needsCopyingFolders) {
                Children.Add(folder);
            }
        }

        private static string NowToBookmark() {
            DateTime chromiumEpoch = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime nowLocal = DateTime.Now;
            DateTime nowUtc = nowLocal.ToUniversalTime();
            TimeSpan diff = nowUtc - chromiumEpoch;
            long microsecondCount = (long)diff.TotalSeconds * 1_000_000
                + diff.Milliseconds * 1_000;
            return microsecondCount.ToString();
        }
        public bool Equals(BookmarkFolder? other) {
            if (other is null)
                return false;
            if (Name != other.Name)
                return false;
            if (Id != other.Id)
                return false;
            if (Guid != other.Guid)
                return false;
            if (Children is null && other.Children is null)
                return true;
            if (Children is null ^ other.Children is null)
                return false;
            if (Children is null)
                throw new NullReferenceException();
            if (other.Children is null)
                throw new NullReferenceException();
            if (Children.Count == 0 && other.Children.Count == 0)
                return true;
            foreach (BookmarkUrl child in Children.Where(c => c is BookmarkUrl).Cast<BookmarkUrl>()) {
                if (!other.Children.Contains(child))
                    return false;
            }
            foreach (BookmarkUrl child in other.Children.Where(c => c is BookmarkUrl).Cast<BookmarkUrl>()) {
                if (!Children.Contains(child))
                    return false;
            }
            foreach (BookmarkFolder child in Children.Where(c => c is BookmarkFolder).Cast<BookmarkFolder>()) {
                if (!other.Children.Contains(child))
                    return false;
            }
            foreach (BookmarkFolder child in other.Children.Where(c => c is BookmarkFolder).Cast<BookmarkFolder>()) {
                if (!Children.Contains(child))
                    return false;
            }
            return true;
        }
        public override bool Equals(object obj) => Equals(obj as BookmarkFolder);
        public override int GetHashCode() {
            if (Children is null) {
                if (Name is null)
                    return 0;
                return Name.GetHashCode();
            }
            return (Name, Children).GetHashCode();
        }
    }
}
