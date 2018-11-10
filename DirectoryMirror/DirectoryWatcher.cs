using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

namespace DirectoryMirror {
    public class DirectoryWatcher {
        public string MirrorParent { get; set; }
        public string MirrorChild { get; set; }

        public DirectoryWatcher(int pollIntreval, string mirrorParent, string mirrorChild) {
            _fileCache = new Dictionary<string, DateTime>();

            _pollTimer = new Timer(pollIntreval);
            _pollTimer.Elapsed += pollTimerElapsed;

            MirrorParent = Path.GetFullPath(mirrorParent);
            MirrorChild = Path.GetFullPath(mirrorChild);
        }

        public void Start() {
            _pollTimer.Start();
        }

        private void pollTimerElapsed(object source, ElapsedEventArgs args) {
            DirectoryInfo dirInfo = new DirectoryInfo(MirrorParent);

            lock (_fileCacheLock) {
                foreach (FileInfo fileInfo in dirInfo.EnumerateFiles()) {
                    if (!_fileCache.Keys.Contains(fileInfo.FullName)) {
                        _fileCache.Add(fileInfo.FullName, fileInfo.LastAccessTimeUtc);
                    }

                    if (fileInfo.LastAccessTimeUtc > _fileCache[fileInfo.FullName]) {
                        fileInfo.CopyTo(MirrorChild);

                        Console.WriteLine($"Copied {fileInfo.FullName}");

                        return;
                    }
                }
            }
        }

        private Timer _pollTimer;
        private Dictionary<string, DateTime> _fileCache { get; }
        private object _fileCacheLock { get; set; }
    }
}
