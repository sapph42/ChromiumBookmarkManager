using System.Text.Json.Serialization;

namespace ChromiumBookmarkManager {
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
        public int FolderCount => BookmarkBar.FolderCount + Other.FolderCount + Synced.FolderCount;

        [JsonIgnore]
        public int UrlCount => BookmarkBar.UrlCount + Other.UrlCount + Synced.UrlCount;
        public BookmarkRoots(BookmarkFolder bookmark_bar, BookmarkFolder other, BookmarkFolder synced) {
            BookmarkBar = bookmark_bar;
            Other = other;
            Synced = synced;
        }
        public void Merge(BookmarkRoots otherRoots) {
            BookmarkBar.Merge(otherRoots.BookmarkBar);
            Other.Merge(otherRoots.Other);
            Synced.Merge(otherRoots.Synced);
        }
    }
}
