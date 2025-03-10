# SapphTools.BookmarkManager.Chromium

## BookmarkFile

### Definition

Namespace: SapphTools.BookmarkManager.Chromium  
Assembly: SapphTools.BookmarkManager.Chromium.dll

Provides method for merging two Chromium Bookmark files (version 1, JSON).

    public class BookmarkFile
	
### Examples

The following example demonstrates the methods of the BookmarkFile class.

    using SapphTools.BookmarkManager.Chromium;
	
    class Test {
        public static void Main() {
            string LiveBookmarks = "C:\Users\You\AppData\Local\Google\Chrome\User Data\Default\Bookmarks";
            var file1 = new BookmarkFile(LiveBookmarks);
            var file2 = new BookmarkFile("C:\Users\You\Documents\Backup\Bookmarks");
	    if (file1.Merge(file2, out var file3) {
                foreach (Process chromeproc in Process.GetProcessesByName("chrome")) {
                    chromeproc.Kill();
                    chromeproc.WaitForExit();
                }
                file3.WriteFile(LiveBookmarks);
	    }
        }
    }

## BookmarkFile Constructors

### Definition

Namespace: SapphTools.BookmarkManager.Chromium  
Assembly: SapphTools.BookmarkManager.Chromium.dll

Initializes a new instance of the BookmarkFile class.

#### Constructor

`BookmarkFile` does not have a public constructor. Use `Deserialize` to initialize a new instance.

## BookmarkFile.Deserialize

###

Namespace: SapphTools.BookmarkManager.Chromium
Assembly: SapphTools.BookmarkManager.Chromium.dll

Deserializes a JSON string from a Chromium Bookmarks file.

#### Deserialize(string json, out BookmarkFile? file) 

    public bool Deserialize(string json, out BookmarkFile? file) 

##### Parameters

`json` string

A JSON string, obtained from a Chromium Bookmarks file.

`file` BookmarkFile?

When this method returns `true`, contains the bookmark data. Otherwise, contains null.

##### Returns

bool

Indicates if deserialization succeeded.

## BookmarkFile.Merge 

### Definition

Namespace: SapphTools.BookmarkManager.Chromium  
Assembly: SapphTools.BookmarkManager.Chromium.dll

Merges this instance of BookmarkFile with another.

#### Merge(BookmarkFile otherFile)

    public void Merge(BookmarkFile otherFile);

##### Parameters

`otherFile` BookmarkFile

The other Chromium JSON Version 1 bookmark file that will be merged with this object.

#### Serialize() 

    public string Serialize() 

##### Returns

string

The serialized JSON string representing the current object state.