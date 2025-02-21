using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SaburaIIS.AdminWeb.ViewModels;
using SaburaIIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaburaIIS.AdminWeb.Controllers
{
    [ApiController]
    public class PartitionsController : ControllerBase
    {
        private readonly Config _config;
        private readonly IStore _store;
        private readonly ILogger<PartitionsController> _logger;

        public PartitionsController(IOptions<Config> options, IStore store, ILogger<PartitionsController> logger)
        {
            _config = options.Value;
            _store = store;
            _logger = logger;
        }

        [HttpGet("/api/partitions")]
        public async Task<IEnumerable<string>> GetPartitionNamesAsync()
        {
            return (await _store.GetPartitionNamesAsync()).OrderBy(name => name);
        }

        [HttpGet("/api/partitions/{name}")]
        public async Task<Partition> GetPartition(string name)
        {
            var (partition, etag) = await _store.GetPartitionAsync(name);
            Response.Headers.Add("ETag", etag);
            return partition;
        }

        [HttpPost("/api/partitions")]
        public async Task<IActionResult> PostPartition([FromBody]PartitionCreationViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var partition = Defaults.CreatePartition(model.Name, _config);
            await _store.SavePartitionAsync(partition, "*");

            return Created($"/api/partitions/{partition.Name}", partition);
        }

        [HttpPut("/api/partitions/{name}")]
        public async Task<IActionResult> PutPartition([FromRoute]string name, [FromBody]Partition partition, [FromHeader(Name = "If-Match")] string etag)
        {
            if (name != partition.Name)
                return BadRequest();

            var newETag = await _store.SavePartitionAsync(partition, etag);

            Response.Headers.Add("ETag", newETag);

            return NoContent();
        }

        [HttpDelete("/api/partitions/{name}")]
        public async Task<IActionResult> DeletePartition([FromRoute] string name, [FromHeader(Name = "If-Match")] string etag)
        {
            var (partition, _) = await _store.GetPartitionAsync(name);

            if (partition == null)
                return NotFound();

            if (partition.ScaleSets.Any())
                return BadRequest(new
                {
                    message = $"{name} has any scale sets settings."
                });

            await _store.RemovePartitionAsync(partition, etag);

            return NoContent();
        }

        [HttpGet("/api/partitions/{name}/instances")]
        public async Task<IActionResult> GetInstances([FromRoute] string name, [FromQuery]string filter)
        {
            var (partition, _) = await _store.GetPartitionAsync(name);

            if (partition == null)
                return NotFound();

            var sets = await Task.WhenAll(partition.ScaleSets.Select(ss => _store.GetInstancesAsync(ss.Name, filter)).ToArray());

            return Ok(sets.SelectMany(instances => instances));
        }

        [HttpGet("/api/partitions/{partitionName}/instances/{scaleSetName}/{instanceName}")]
        public async Task<IActionResult> GetInstance([FromRoute] string partitionName, [FromRoute] string scaleSetName, [FromRoute] string instanceName)
        {
            var (partition, _) = await _store.GetPartitionAsync(partitionName);

            if (partition == null)
                return NotFound();

            if (!partition.ScaleSets.Any(ss => ss.Name == scaleSetName))
                return NotFound();

            var instance = await _store.GetInstanceAsync(scaleSetName, instanceName);

            if (instance == null)
                return NotFound();

            return Ok(instance);
        }

        [HttpGet("/api/partitions/{partitionName}/instances/{scaleSetName}/{instanceName}/snapshots")]
        public async Task<IActionResult> GetSnapshots([FromRoute] string partitionName, [FromRoute] string scaleSetName, [FromRoute] string instanceName)
        {
            var (partition, _) = await _store.GetPartitionAsync(partitionName);

            if (partition == null)
                return NotFound();

            if (!partition.ScaleSets.Any(ss => ss.Name == scaleSetName))
                return NotFound();

            var snapshots = await _store.GetSnapshotsAsync(scaleSetName, instanceName);

            return Ok(snapshots);
        }

        [HttpGet("/api/partitions/{partitionName}/instances/{scaleSetName}/{instanceName}/snapshots/{timestamp}")]
        public async Task<IActionResult> GetSnapshots([FromRoute] string partitionName, [FromRoute] string scaleSetName, [FromRoute] string instanceName, [FromRoute] string timestamp)
        {
            var (partition, _) = await _store.GetPartitionAsync(partitionName);

            if (partition == null)
                return NotFound();

            if (!partition.ScaleSets.Any(ss => ss.Name == scaleSetName))
                return NotFound();

            var snapshot = await _store.GetSnapshotAsync(scaleSetName, instanceName, timestamp);

            if (snapshot == null)
                return NotFound();

            return Ok(snapshot);
        }
    }
}
