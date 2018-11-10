using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

namespace DirectoryMirror {
    public class DirectoryWatcher {
        #region MEMBERS

        private Timer _pollTimer;
        private object _fileCacheLock { get; }
        private FileCache _parentCache { get; set; }
        private List<FileCache> _childCaches { get; set; }

        public string MirrorParentPath { get; set; }
        public List<string> MirrorChildrenPaths { get; set; }

        #endregion

        public DirectoryWatcher(int pollIntreval, string mirrorParent, params string[] mirrorChildren) {
            _pollTimer = new Timer(pollIntreval);
            _pollTimer.Elapsed += pollTimerElapsed;

            _fileCacheLock = new object();

            MirrorParentPath = Path.GetFullPath(mirrorParent);
            MirrorChildrenPaths = mirrorChildren.Select(Path.GetFullPath).ToList();
        }

        #region METHODS

        public void Start() {
            _pollTimer.Start();
        }

        private void Tick() {
            DirectoryInfo parentDirInfo = new DirectoryInfo(MirrorParentPath);

            if (parentDirInfo.LastWriteTimeUtc > _parentCache.LastWriteTimeUtc) {
                lock (_fileCacheLock) {
                    _parentCache.LastWriteTimeUtc = parentDirInfo.LastWriteTimeUtc;

                    foreach (FileInfo fileInfo in parentDirInfo.EnumerateFiles()) {
                        if (!(_parentCache[fileInfo.FullName] == null)) {
                            _parentCache[fileInfo.FullName] = new CacheFile(fileInfo.FullName, fileInfo.LastWriteTimeUtc);
                        }

                        if (fileInfo.LastAccessTimeUtc > _parentCache[fileInfo.FullName].LastWriteTimeUtc) {
                            _parentCache[fileInfo.FullName].LastWriteTimeUtc = fileInfo.LastWriteTimeUtc;

                            fileInfo.CopyTo(MirrorChildrenPaths);

                            Console.WriteLine($"Copied {fileInfo.FullName}");

                            return;
                        }
                    }
                }
            }
        }

        #endregion

        #region EVENT

        private void pollTimerElapsed(object source, ElapsedEventArgs args) {
            Tick();
        }

        #endregion

        #region INIT

        private void Initialise() {
            InitialiseParentCache();
        }

        private void InitialiseParentCache() {
            DirectoryInfo parentDirInfo = new DirectoryInfo(MirrorParentPath);
            _parentCache = new FileCache(parentDirInfo.LastWriteTimeUtc);

            foreach (FileInfo file in parentDirInfo.EnumerateFiles()) {
                _parentCache[file.FullName] = new CacheFile(file.FullName, file.LastWriteTimeUtc);
            }
        }

        private void InitialiseChildCaches() {

        }

        #endregion
    }
}
