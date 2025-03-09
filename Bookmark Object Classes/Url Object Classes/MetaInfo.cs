using System.Text.Json.Serialization;

#nullable enable
namespace SapphTools.BookmarkManager.Chromium {
    public class MetaInfo {

        [JsonPropertyName("power_bookmark_meta")]
        internal string? PowerBookmarkMeta { get; set; } = null;
    }
}
#nullable disable