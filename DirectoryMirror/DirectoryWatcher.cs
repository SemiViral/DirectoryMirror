using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace DirectoryMirror {
    public class DirectoryWatcher {
        public string MirrorParent { get; set; }
        public string MirrorChild { get; set; }

        public DirectoryWatcher(int pollIntreval, string directoryToWatch) {
            Files = new List<File>();
            MirrorParent = directoryToWatch;
            pollTimer = new Timer(pollIntreval);
            pollTimer.Elapsed += pollTimerElapsed;
        }

        private void pollTimerElapsed(object source, ElapsedEventArgs args) {

        }

        private Timer pollTimer;
        private List<File> Files { get; }
    }

    public class File {
        public File(string path) {
            FilePath = Path.GetFullPath(path);
            LastChanged = DateTime.Now;
        }

        public string FilePath { get; }

        public DateTime LastChanged { get; set; }
    }
}
