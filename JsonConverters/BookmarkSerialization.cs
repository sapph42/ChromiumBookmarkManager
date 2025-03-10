using System.Text.Json;
using JsonIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition;

namespace SapphTools.BookmarkManager.Chromium {
    internal class BookmarkSerialization {
        internal static readonly JsonSerializerOptions Options = new JsonSerializerOptions() {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new BookmarkFileConverter(), new BookmarkRootsConverter(), new BookmarkItemConverter(), new BookmarkFolderConverter(), new BookmarkUrlConverter() }
        };
    }
}
