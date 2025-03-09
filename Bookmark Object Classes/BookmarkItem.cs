using System.Text.Json.Serialization;

namespace ChromiumBookmarkManager {
#nullable enable
    [JsonConverter(typeof(BookmarkItemConverter))]
    public abstract class BookmarkItem {

        [JsonPropertyName("date_added")]
        public string? DateAdded { get; set; } = null;

        [JsonPropertyName("date_last_used")]
        public string? DateLastUsed { get; set; } = null;

        [JsonPropertyName("guid")]
        public string? Guid { get; set; } = null;

        [JsonPropertyName("id")]
        public string? Id { get; set; } = null;

        [JsonPropertyName("name")]
        public string? Name { get; set; } = null;

        [JsonPropertyName("source")]
        public string? Source { get; set; } = null;

        [JsonPropertyName("type")]
        public virtual string? Type { get; } = null;

        [JsonIgnore]
        public abstract int FolderCount { get; }

        [JsonIgnore]
        public abstract int UrlCount { get; }

        [JsonIgnore]
        public abstract int FolderVal { get; }

        [JsonIgnore]
        public abstract int FileVal { get; }
        public abstract void Merge(BookmarkItem other);
        protected static string? UlongStringMax(string? a, string? b) {
            if (ulong.TryParse(a, out ulong aCast) && ulong.TryParse(b, out ulong bCast))
                return aCast > bCast ? a : b;

            return a ?? b;
        }
        protected static string? UlongStringMin(string? a, string? b) {
            if (ulong.TryParse(a, out ulong aCast) && ulong.TryParse(b, out ulong bCast))
                return aCast < bCast ? a : b;

            return a ?? b;
        }
    }

    [JsonConverter(typeof(BookmarkItemConverter))]
    public abstract class BookmarkItem<T> : BookmarkItem where T : BookmarkItem<T> {

        [JsonIgnore]
        public abstract override int FolderCount { get; }

        [JsonIgnore]
        public abstract override int UrlCount { get; }

        [JsonIgnore]
        public abstract override int FolderVal { get; }

        [JsonIgnore]
        public abstract override int FileVal { get; }
        public abstract void Merge(T other);
        public override void Merge(BookmarkItem other) {
            if (other is BookmarkFolder folder)
                Merge(folder);
            else if (other is BookmarkUrl url)
                Merge(url);
        }
    }
}
#nullable disable