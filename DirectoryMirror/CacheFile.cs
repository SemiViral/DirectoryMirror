using System;

namespace DirectoryMirror {
    public class CacheFile {
        public string FullName { get; }
        public DateTime LastWriteTimeUtc { get; set; }

        public CacheFile(string fullName, DateTime lastWriteTimeUtc) {
            FullName = fullName;
            LastWriteTimeUtc = lastWriteTimeUtc;
        }
    }
}
