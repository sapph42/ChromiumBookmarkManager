#nullable enable
#pragma warning disable IDE1006 // Naming Styles
namespace ChromeBookmarkMerge {
    internal class ChromeBookmarkItem {
        public class MetaInfo {
            public string? power_bookmark_meta { get; set; } = null;
        }
        public string? date_added { get; set; } = null;
        public string? date_last_used { get; set; } = null;
        public string? guid { get; set; } = null;
        public string? id { get; set; } = null;
        public MetaInfo meta_info { get; set; } = new MetaInfo();
        public string? name { get; set; } = null;
        public string? type { get; set; } = null;
        public string? url { get; set; } = null;
        public bool Equals(ChromeBookmarkItem? other) {
            if (other is null)
                return false;
            return url == other.url;
        }
        public override bool Equals(object obj) => Equals(obj as ChromeBookmarkItem);
        public override int GetHashCode() {
            if (url is null)
                return 0;
            return url.GetHashCode();
        }
    }
}
#pragma warning restore IDE1006 // Naming Styles
