using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;

namespace SaburaIIS
{
    public class ChangeTracker<T> : IDisposable
    {
        private readonly FeedIterator<T> _feed;
        public ChangeTracker(FeedIterator<T> feed)
        {
            _feed = feed;
        }

        public void Dispose()
        {
            _feed?.Dispose();
        }

        public virtual async Task<bool> HasChangeAsync()
        {
            while (_feed.HasMoreResults)
            {
                try
                {
                    var items = await _feed.ReadNextAsync();
                    return items.Count > 0;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
    }
}
