using System.Collections.Generic;
using System;
using System.Text.Json.Serialization;

namespace SapphTools.BookmarkManager.Chromium {
#nullable enable
    [JsonConverter(typeof(BookmarkItemConverter))]
    public abstract class BookmarkItem {

        [JsonPropertyName("date_added")]
        public string DateAdded { get; set; } = "0";

        [JsonPropertyName("date_last_used")]
        public string DateLastUsed { get; set; } = "0";

        [JsonPropertyName("guid")]
        public string Guid { get; set; } = new Guid().ToString();

        [JsonPropertyName("id")]
        public string Id { get; set; } = "4"; //Ids are 1 indexed. 1-3 are reserved for root folders.

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
        public abstract void Merge(BookmarkItem other, HashSet<int> globalIds, ref int nextAvailable);
        protected static string UlongStringMax(string? a, string? b) {
            if (ulong.TryParse(a, out ulong aCast) && ulong.TryParse(b, out ulong bCast)) {
                if (aCast == 0 && b != null)
                    return b;
                return aCast > bCast ? a! : b!;
            }

            return a ?? b ?? "0";
        }
        protected static string UlongStringMin(string? a, string? b) {
            if (ulong.TryParse(a, out ulong aCast) && ulong.TryParse(b, out ulong bCast))
                return aCast < bCast ? a! : b!;

            return a ?? b ?? "0";
        }
        protected static DateTime UlongStringToDate(string ulongString) {
            if (ulong.TryParse(ulongString, out ulong microseconds)) {
                DateTime epoch = new DateTime(1601, 1, 1);
                return epoch.AddTicks((long)(microseconds * 10)); // Convert microseconds to ticks
            }
            return DateTime.MaxValue; // Return a max value if parsing fails
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
        public abstract void Merge(T other, HashSet<int> globalIds, ref int nextAvailable);
        public override void Merge (BookmarkItem other) {
            if (other is BookmarkUrl url)
                Merge(url);
            else
                throw new NotImplementedException("Invoking this signature of Merge is not permitted for BookmarkFolder");
        }
        public override void Merge(BookmarkItem other, HashSet<int> globalIds, ref int nextAvailable) {
            if (other is BookmarkFolder folder)
                Merge(folder, globalIds, ref nextAvailable);
            else if (other is BookmarkUrl url)
                Merge(url);
        }
    }
}
#nullable disable