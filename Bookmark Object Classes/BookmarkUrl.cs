#nullable enable
using System.Text.Json.Serialization;

namespace ChromiumBookmarkManager {
    [JsonConverter(typeof(BookmarkUrlConverter))]
    public class BookmarkUrl : BookmarkItem<BookmarkUrl> {

        [JsonPropertyName("meta_info")]
        [JsonInclude]
        public MetaInfo? MetaInfo { get; set; } = null;

        [JsonPropertyName("show_icon")]
        [JsonInclude]
        public bool ShowIcon { get; set; } = false;

        [JsonPropertyName("type")]
        [JsonInclude]
        public override string Type => "url";

        [JsonPropertyName("Url")]
        [JsonInclude]
        public string? Url { get; set; } = null;

        [JsonPropertyName("visit_count")]
        [JsonInclude]
        public int VisitCount { get; set; } = 0;
        [JsonIgnore]
        public override int FolderCount { get; } = 0;

        [JsonIgnore]
        public override int UrlCount { get; } = 0;

        [JsonIgnore]
        public override int FolderVal { get; } = 0;

        [JsonIgnore]
        public override int FileVal { get; } = 1;
        public BookmarkUrl() { }
        public BookmarkUrl(
            MetaInfo? metaInfo,
            bool showIcon,
            string? url,
            int visitCount,
            string type,
            string date_added,
            string date_last_used,
            string guid,
            string id,
            string name,
            string source) :
            this(metaInfo, showIcon, url, visitCount, date_added, date_last_used, guid, id, name, source) { }
        public BookmarkUrl(
            MetaInfo? metaInfo, 
            bool showIcon, 
            string? url, 
            int visitCount, 
            string date_added,
            string date_last_used,
            string guid,
            string id,
            string name,
            string source) {
            MetaInfo = metaInfo;
            ShowIcon = showIcon;
            Url = url;
            VisitCount = visitCount;
            DateAdded = date_added;
            DateLastUsed = date_last_used;
            this.Guid = guid;
            Id = id;
            Name = name;
            Source = source;
        }

        public override void Merge(BookmarkUrl otherUrl) {
            DateAdded = UlongStringMin(DateAdded, otherUrl.DateAdded);
            DateLastUsed = UlongStringMax(DateLastUsed, otherUrl.DateLastUsed);
            if (DateAdded == otherUrl.DateAdded)
                Guid = otherUrl.Guid;
            VisitCount = VisitCount > otherUrl.VisitCount ? VisitCount : otherUrl.VisitCount;
        }
        public bool Equals(BookmarkUrl? other) {
            if (other is null)
                return false;
            if (Name != other.Name)
                return false;
            if (Id != other.Id)
                return false;
            if (Guid != other.Guid)
                return false;
            return Url == other.Url;
        }
        public override bool Equals(object obj) => Equals(obj as BookmarkUrl);
        public override int GetHashCode() {
            if (Url is null)
                return 0;
            return Url.GetHashCode();
        }
    }
}
