using SaburaIIS.Models;
using System.Text.Json.Serialization;

namespace SaburaIIS.CLI
{
    public class PartitionWithETag : Partition
    {
        [JsonPropertyName("@etag")]
        public string? ETag { get; set; }
    }
}
