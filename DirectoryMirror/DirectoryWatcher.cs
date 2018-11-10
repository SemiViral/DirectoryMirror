using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

namespace DirectoryMirror {
    public class DirectoryWatcher {
        private Timer _pollTimer;
        private Dictionary<string, DateTime> _parentFileCache { get; }
        private Dictionary<string, DateTime> _childFileCache { get; }
        private object _fileCacheLock { get; }

        public string MirrorParent { get; set; }
        public string MirrorChild { get; set; }

        public DirectoryWatcher(int pollIntreval, string mirrorParent, string mirrorChild) {
            _parentFileCache = new Dictionary<string, DateTime>();
            _childFileCache = new Dictionary<string, DateTime>();
            _fileCacheLock = new object();

            _pollTimer = new Timer(pollIntreval);
            _pollTimer.Elapsed += pollTimerElapsed;

            MirrorParent = Path.GetFullPath(mirrorParent);
            MirrorChild = Path.GetFullPath(mirrorChild);
        }

        public void Start() {
            _pollTimer.Start();
        }

        private void pollTimerElapsed(object source, ElapsedEventArgs args) {
            DirectoryInfo parentDirInfo = new DirectoryInfo(MirrorParent);
            DirectoryInfo childDirInfo = new DirectoryInfo(MirrorParent);

            lock (_fileCacheLock) {
                foreach (FileInfo fileInfo in dirInfo.EnumerateFiles()) {
                    if (!_parentFileCache.Keys.Contains(fileInfo.FullName)) {
                        _parentFileCache.Add(fileInfo.FullName, fileInfo.LastAccessTimeUtc);
                    }

                    if (fileInfo.LastAccessTimeUtc > _parentFileCache[fileInfo.FullName]) {
                        fileInfo.CopyTo(MirrorChild);

                        Console.WriteLine($"Copied {fileInfo.FullName}");

                        return;
                    }
                }
            }
        }

        private void Initialise() {
            DirectoryInfo parentDirInfo = new DirectoryInfo(MirrorParent);
            DirectoryInfo childDirInfo = new DirectoryInfo(MirrorParent);

            lock (_fileCacheLock) {
                foreach (FileInfo file in parentDirInfo.EnumerateFiles()) {
                    _parentFileCache.Add(file.FullName, file.LastWriteTimeUtc);
                }

                foreach (FileInfo file in childDirInfo.EnumerateFiles()) {
                    _childFileCache.Add(file.FullName, file.LastWriteTimeUtc);
                }
            }
        }
    }
}
