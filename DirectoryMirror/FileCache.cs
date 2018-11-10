using System;

namespace DirectoryMirror {
    public class FileCache {
        private CacheFile[] files;
        private object _cacheLock;
        private int _cacheIndexes;

        public int Length {
            get => _cacheIndexes;
            private set {
                _cacheIndexes = value;

                if (_cacheIndexes >= files.Length / 2) {
                    CacheFile[] tempArray = new CacheFile[files.Length * 2];

                    Array.Copy(files, tempArray, files.Length);

                    files = tempArray;
                }
            }
        }

        public DateTime LastWriteTimeUtc { get; set; }

        public FileCache(DateTime lastWriteTimeUtc) {
            files = new CacheFile[2];
            _cacheLock = new object();
            Length = 0;

            LastWriteTimeUtc = lastWriteTimeUtc;
        }

        public CacheFile this[CacheFile index] {
            get {
                lock (_cacheLock) {
                    int indexOf = Array.IndexOf(files, index);

                    return indexOf == -1 ? null : files[indexOf];
                }
            }

            set {
                lock (_cacheLock) {
                    int indexOf = Array.IndexOf(files, index);

                    if (indexOf == -1) {
                        files[Length] = value;
                        Length++;
                    } else {
                        files[indexOf] = value;
                    }
                }
            }
        }

        public CacheFile this[string index] {
            get {
                lock (_cacheLock) {
                    int indexOf = IndexOf(files, index);

                    return indexOf == -1 ? null : files[indexOf];
                }
            }

            set {
                lock (_cacheLock) {
                    int indexOf = IndexOf(files, index);

                    if (indexOf == -1) {
                        files[Length] = value;
                        Length++;
                    } else {
                        files[indexOf] = value;
                    }
                }
            }
        }

        private int IndexOf(CacheFile[] files, string index) {
            for (int i = 0; i < files.Length; i++) {
                if (files[i].FullName.Equals(index)) {
                    return i;
                }
            }

            return -1;
        }
    }
}
