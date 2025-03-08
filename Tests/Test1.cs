using ChromiumBookmarkManager;

namespace Tests {
    [TestClass]
    public sealed class Test1 {
        BookmarkFile? file;
        [TestMethod]
        public void LoadFile_Test() {
            bool success = BookmarkFile.LoadFile(@"C:\Users\nickgibson\AppData\Local\Microsoft\Edge\User Data\Default\Bookmarks", out file);
            Assert.IsTrue(success);
            Assert.IsNotNull(file);
        }

        [TestMethod]
        public void Count_Test() {
            LoadFile_Test();
            BookmarkFile thisFile = (BookmarkFile)file!;
            Assert.AreNotEqual(0, thisFile.FolderCount);
            Assert.AreNotEqual(0, thisFile.UrlCount);
        }
    }
}
