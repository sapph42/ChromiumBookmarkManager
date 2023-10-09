#nullable enable
#pragma warning disable IDE1006 // Naming Styles
using ChromeBookmarkMerge;

namespace ChromiumBookmarkManager {
    internal class BookmarkUrl : BookmarkItem {
        public class MetaInfo {
            public string? power_bookmark_meta { get; set; } = null;
        }
        public MetaInfo? meta_info { get; set; } = null;
        public new string type { get; } = "url";
        public string? url { get; set; } = null;
        public bool Equals(BookmarkUrl? other) {
            if (other is null)
                return false;
            return url == other.url;
        }
        public override bool Equals(object obj) => Equals(obj as BookmarkUrl);
        public override int GetHashCode() {
            if (url is null)
                return 0;
            return url.GetHashCode();
        }
    }
}
#pragma warning restore IDE1006 // Naming Styles
