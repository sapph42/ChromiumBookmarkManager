using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

#nullable enable
namespace SapphTools.BookmarkManager.Chromium {
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
        public override int UrlCount => Children.Sum(bi => bi.UrlCount + bi.FileVal);

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
        public override void Merge(BookmarkFolder other) {
            throw new NotImplementedException();
        }
        public override void Merge(BookmarkFolder otherFolder, HashSet<int> globalIds, ref int nextAvailable) {
            DateAdded = UlongStringMin(DateAdded, otherFolder.DateAdded);
            DateLastUsed = UlongStringMax(DateLastUsed, otherFolder.DateLastUsed);
            DateModified = UlongStringMax(DateModified, otherFolder.DateModified);
            if (DateAdded == otherFolder.DateAdded)
                Guid = otherFolder.Guid;
            if (otherFolder.Children.Count == 0)
                return;

            if (Children.Count == 0){
                Children = otherFolder.Children;
                return;
            }

            var folderLookup = Children.OfType<BookmarkFolder>().ToDictionary(f => f.Id, f => f);
            var urlLookup = Children.OfType<BookmarkUrl>().ToDictionary(u => (u.Id, u.Url), u => u);

            foreach (BookmarkItem item in otherFolder.Children) {
                if (item is BookmarkUrl url) {
                    if (urlLookup.TryGetValue((url.Id, url.Url), out var existingUrl))
                        existingUrl.Merge(url);
                    else {
                        Children.Add(url);
                    }
                } else if (item is BookmarkFolder folder) {
                    if (folderLookup.TryGetValue(folder.Id, out var existingFolder)) {
                        if (UlongStringToDate(folder.DateAdded) < UlongStringToDate(existingFolder.DateAdded))
                            existingFolder.Id = folder.Id;
                        existingFolder.Merge(folder, globalIds, ref nextAvailable);
                    }
                    else {
                        Children.Add(folder);
                    }
                }
            }
            FixDuplicateIds(globalIds, ref nextAvailable);
        }
        private void FixDuplicateIds(HashSet<int> globalIds, ref int nextAvailable) {
            if (nextAvailable < 4)
                nextAvailable = 4;
            AssignUniqueIds(globalIds, ref nextAvailable);
        }
        private void AssignUniqueIds(HashSet<int> globalIds, ref int nextAvailable) {
            int folderId = int.Parse(Id);
            if (folderId > 3 && !globalIds.Add(folderId))
                Id = GenerateUniqueId(globalIds, ref nextAvailable);
            foreach (var child in Children) {
                int childId = int.Parse(child.Id);
                if (child is BookmarkUrl && childId > 3 && !globalIds.Add(childId))
                    Id = GenerateUniqueId(globalIds, ref nextAvailable);
                else if (child is BookmarkFolder childFolder)
                    childFolder.AssignUniqueIds(globalIds, ref nextAvailable);
            }
        }
        private string GenerateUniqueId(HashSet<int> existingIds, ref int nextAvailable) {
            while (existingIds.Contains(nextAvailable) || nextAvailable <= 3)
                nextAvailable++;
            existingIds.Add(nextAvailable);
            return nextAvailable.ToString();
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
