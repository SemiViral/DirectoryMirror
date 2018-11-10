using System;

namespace DirectoryMirror {
    public class FileCache {
        private string[] _fullNames;
        private object _cacheLock;

        private long CacheIndexes { get{ set; }

        public FileCache() {
            _fullNames = new string[2];
            _cacheLock = new object();
            _cacheIndexes = 0;
        }

        public string this[string index] {
            get {
                lock (_cacheLock) {
                    int indexOf = Array.IndexOf(_fullNames, index);

                    return indexOf == -1 ? throw new IndexOutOfRangeException($"Index '{index}' does not exist."); : _fullNames[indexOf];
                }
            }

            set {
                lock (_cacheLock) {
                    int indexOf = Array.IndexOf(_fullNames, index);

                    if (indexOf == -1) {
                        _fullNames[CacheIndexes] = value;
                        _cacheIndexes++;
                    } else {
                        _fullNames[indexOf] = value;
                    }
                }
            }
        }
    }
}
