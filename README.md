# ChromeBookmarkMerge
 
# ChromeBookmarkMerge

## Usage

Instantiate the class as follows:

    string file1 = "path_to_first_bookmark_file";
    string file2 = "path_to_second_bookmark_file";
    string file3 = "path_to_output_file";
    var merger = new ChromeBookmarkMerge.BookmarkMerger(file1, file2);
    merger.Merge(file3);

## Logic

Merge attempts to combine files without creating duplicates. Bookmarks are considered equivalent if they have the same value for URL. Folders are recursively merged based on the value for name.

Merge does not attempt to replicate Chromium's checksum. Therefore, use of Merge neccessitates removal of Bookmarks.bak, otherwise Chromium will prefer the backup over an un-checksummed file. 