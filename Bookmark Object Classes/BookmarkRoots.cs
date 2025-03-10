using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SapphTools.BookmarkManager.Chromium {
    public enum Roots {
        Bookmark_Bar = 1,
        Other = 2,
        Synced = 3
    }

    [JsonConverter(typeof(BookmarkRootsConverter))]
    public class BookmarkRoots {

        [JsonPropertyName("bookmark_bar")]
        [JsonInclude]
        public BookmarkFolder BookmarkBar { get; set; }

        [JsonPropertyName("other")]
        [JsonInclude]
        public BookmarkFolder Other { get; set; }

        [JsonPropertyName("synced")]
        [JsonInclude]
        public BookmarkFolder Synced { get; set; }

        [JsonIgnore]
        public int FolderCount => BookmarkBar.FolderCount + Other.FolderCount + Synced.FolderCount + 3;

        [JsonIgnore]
        public int UrlCount => BookmarkBar.UrlCount + Other.UrlCount + Synced.UrlCount;
        public BookmarkRoots() {
            BookmarkBar = GenerateDefaultRoot(Roots.Bookmark_Bar);
            Other = GenerateDefaultRoot(Roots.Other);
            Synced = GenerateDefaultRoot(Roots.Other);
        }
        public BookmarkRoots(BookmarkFolder bookmark_bar, BookmarkFolder other, BookmarkFolder synced) {
            BookmarkBar = bookmark_bar;
            Other = other;
            Synced = synced;
        }
        public void Merge(BookmarkRoots otherRoots) {
            HashSet<int> globalIds = new HashSet<int>();
            int nextAvailable = 1;
            BookmarkBar.Merge(otherRoots.BookmarkBar, globalIds, ref nextAvailable);
            Other.Merge(otherRoots.Other, globalIds, ref nextAvailable);
            Synced.Merge(otherRoots.Synced, globalIds, ref nextAvailable);
        }
        public static BookmarkFolder GenerateDefaultRoot(Roots root) {
            List<BookmarkItem> noChildren = new List<BookmarkItem>();
            string now = NowToBookmark();
            string id = ((int)root).ToString();
            string guid = new Guid().ToString();
            const string noSource = "unknown";
            const string neverUsed = "0";
            return root switch {
                Roots.Other => new BookmarkFolder(noChildren, now, now, neverUsed, guid, id, "other", noSource),
                Roots.Synced => new BookmarkFolder(noChildren, now, now, neverUsed, guid, id, "synced", noSource),
                _ => new BookmarkFolder(noChildren, now, now, neverUsed, guid, id, "bookmark_bar", noSource),
            };
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
    }
}
