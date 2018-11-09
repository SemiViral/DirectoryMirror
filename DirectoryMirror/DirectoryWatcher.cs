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

        private void pollTimerElapsed(object source, ElapsedEventArgs args) {
            DirectoryInfo parentInfo = new DirectoryInfo(MirrorParent);

            foreach (FileInfo info in parentInfo.EnumerateFiles()) {
                if (!_fileCache.Keys.Contains(info.FullName)) {
                    continue;
                }

                _fileCache[info.FullName] = DateTime.Now;
                info.CopyTo(MirrorChild);
            }
        }

        private Timer _pollTimer;
        private Dictionary<string, DateTime> _fileCache { get; }
    }
}
