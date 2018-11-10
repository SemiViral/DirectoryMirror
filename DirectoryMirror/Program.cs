using System.Threading;

namespace DirectoryMirror {
    public static class Program {
        private static DirectoryWatcher _watcher;
        private static ManualResetEvent _resetEvent;

        private static bool _quit;

        public static bool Quit {
            get => _quit;
            set {
                if (value) {
                    _resetEvent.Set();
                }
            }
        }

        private static void Main(string[] args) {
            _watcher = new DirectoryWatcher(100000, @"C:\Users\semiv\OneDrive\Documents\Test", @"C:\Users\semiv\Downloads\TestCopyTo");
            _resetEvent = new ManualResetEvent(false);

            _watcher.Start();

            _resetEvent.WaitOne();
        }
    }
}
