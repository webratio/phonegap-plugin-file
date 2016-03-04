using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPCordovaClassLib.Cordova.Commands
{
    /// <summary>
    /// Description of the root of a file system.
    /// </summary>
    public class FileSystemRoot
    {
        private readonly string topDirectoryName;
        private File.FileEntry entry = null; // lazy

        public FileSystemRoot(string name, string topDirectoryName = null, int filesystemType = -1)
        {
            Contract.Requires(name != null);
            this.Name = name;
            this.Type = filesystemType;
            this.topDirectoryName = topDirectoryName;
        }

        public string Name { get; set; }

        public int Type { get; set; }

        public File.FileEntry Entry
        {
            get
            {
                return RetrieveOrCreateEntry();
            }
        }

        public string Path
        {
            get
            {
                if (topDirectoryName != null)
                {
                    return "/" + topDirectoryName + "/";
                }
                return "/";
            }
        }

        private File.FileEntry RetrieveOrCreateEntry()
        {
            lock (this)
            {
                if (this.entry == null)
                {
                    // Ensure that the root directory (if required) exists
                    if (topDirectoryName != null)
                    {
                        using (IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            if (!isoStorage.FileExists(topDirectoryName))
                            {
                                isoStorage.CreateDirectory(topDirectoryName);
                            }
                        }
                    }

                    this.entry = new File.FileEntry(this.Path, this);
                }
                return this.entry;
            }
        }

        public bool ContainsPath(string path)
        {
            return path.StartsWith(this.Path);
        }
    }
}
