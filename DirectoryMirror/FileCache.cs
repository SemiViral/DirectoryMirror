using System;

namespace DirectoryMirror {
    public class FileCache {
        private string[] _fullNames;
        private object _cacheLock;
        private int _cacheIndexes;

        public int Length {
            get => _cacheIndexes;
            private set {
                _cacheIndexes = value;

                if (_cacheIndexes >= _fullNames.Length / 2) {
                    string[] tempArray = new string[_fullNames.Length * 2];

                    Array.Copy(_fullNames, tempArray, _fullNames.Length);

                    _fullNames = tempArray;
                }
            }
        }

        public FileCache() {
            _fullNames = new string[2];
            _cacheLock = new object();
            Length = 0;
        }

        public string this[string index] {
            get {
                lock (_cacheLock) {
                    int indexOf = Array.IndexOf(_fullNames, index);

                    return indexOf == -1 ? throw new IndexOutOfRangeException($"Index '{index}' does not exist.") : _fullNames[indexOf];
                }
            }

            set {
                lock (_cacheLock) {
                    int indexOf = Array.IndexOf(_fullNames, index);

                    if (indexOf == -1) {
                        _fullNames[Length] = value;
                        _cacheIndexes++;
                    } else {
                        _fullNames[indexOf] = value;
                    }
                }
            }
        }
    }
}
