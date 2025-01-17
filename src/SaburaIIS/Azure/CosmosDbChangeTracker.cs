using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;

namespace SaburaIIS.Azure
{

    public class CosmosDbChangeTracker<T> : IChangeTracker
    {
        private readonly FeedIterator<T> _feed;
        public CosmosDbChangeTracker(FeedIterator<T> feed)
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
