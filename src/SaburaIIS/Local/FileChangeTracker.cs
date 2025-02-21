using System;
using System.IO;
using System.Threading.Tasks;

namespace SaburaIIS.Local
{
    public class FileChangeTracker : IChangeTracker
    {
        private FileSystemWatcher _watcher;
        private bool _changed = false;
        private object _lock = new object();

        public FileChangeTracker(string path)
        {
            var file = new FileInfo(path);
            _watcher = new FileSystemWatcher(file.DirectoryName);
            _watcher.Filter = file.Name;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.IncludeSubdirectories = false;
            _watcher.Changed += OnChanged;
            _watcher.EnableRaisingEvents = true;
        }
        
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            lock (_lock)
            {
                _changed = true;
            }
        }

        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;
            }
        }

        public Task<bool> HasChangeAsync()
        {
            if (_changed)
            {
                lock (_lock)
                {
                    if (!_changed)
                    {
                        return Task.FromResult(false);
                    }
                    _changed = false;
                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }
    }
}
