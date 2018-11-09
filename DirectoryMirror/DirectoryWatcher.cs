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
            new DirectoryInfo(MirrorParent)
                .EnumerateFiles()
                .ForEach(CheckCopyFile);
        }

        private void CheckCopyFile(FileInfo info) {
            if (!_fileCache.Keys.Contains(info.FullName)) {
                return;
            }

            _fileCache[info.FullName] = DateTime.Now;
            info.CopyTo(MirrorChild);

            Console.WriteLine($"Copied {info.FullName}");
        }

        private Timer _pollTimer;
        private Dictionary<string, DateTime> _fileCache { get; }
    }
}
