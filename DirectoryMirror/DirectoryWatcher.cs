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

        public string MirrorParentPath { get; set; }
        public List<string> MirrorChildrenPaths { get; set; }

        #endregion

        public DirectoryWatcher(int pollIntreval, string mirrorParent, params string[] mirrorChildren) {
            _pollTimer = new Timer(pollIntreval);
            _pollTimer.Elapsed += pollTimerElapsed;

            _fileCacheLock = new object();

            MirrorParentPath = Path.GetFullPath(mirrorParent);
            MirrorChildrenPaths = mirrorChildren.Select(Path.GetFullPath).ToList();

            Initialise();
        }

        #region METHODS

        public void Start() {
            _pollTimer.Start();
        }

        private void Tick() {
            DirectoryInfo parentDirInfo = new DirectoryInfo(MirrorParentPath);
            VerifyParentCache(parentDirInfo);

            foreach (string path in MirrorChildrenPaths) {
                DirectoryInfo childDirectory = new DirectoryInfo(path);

                IEnumerable<CacheFile> files = childDirectory.EnumerateFiles().Select(file => new CacheFile(file.FullName, file.LastWriteTimeUtc));

                for (int i = 0; i < _parentCache.Length; i++) {
                    if ()
                }
            }
        }

        private void VerifyParentCache(DirectoryInfo parentDirectory) {
            if (parentDirectory.LastWriteTimeUtc > _parentCache.LastWriteTimeUtc) {
                lock (_fileCacheLock) {
                    _parentCache.LastWriteTimeUtc = parentDirectory.LastWriteTimeUtc;

                    foreach (FileInfo file in parentDirectory.EnumerateFiles()) {
                        if (_parentCache[file.FullName] == null) {
                            _parentCache[file.FullName] = new CacheFile(file.FullName, file.LastWriteTimeUtc);
                        }

                        if (file.LastAccessTimeUtc > _parentCache[file.FullName].LastWriteTimeUtc) {
                            PropagateFileChange(file);

                            return;
                        }
                    }
                }
            }
        }

        private void PropagateFileChange(FileInfo file) {
            _parentCache[file.FullName].LastWriteTimeUtc = file.LastWriteTimeUtc;

            foreach (string path in MirrorChildrenPaths) {
                file.CopyTo(path);
            }

            Console.WriteLine($"Copied {file.FullName}");
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
            InitialiseChildCaches();
        }

        private void InitialiseParentCache() {
            DirectoryInfo parentDirInfo = new DirectoryInfo(MirrorParentPath);
            _parentCache = new FileCache(parentDirInfo.LastWriteTimeUtc);

            foreach (FileInfo file in parentDirInfo.EnumerateFiles()) {
                _parentCache[file.FullName] = new CacheFile(file.FullName, file.LastWriteTimeUtc);
                Console.WriteLine($"Read {file.FullName}");
            }
        }

        #endregion
    }
}
