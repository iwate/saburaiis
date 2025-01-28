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
    public class PackagesController : ControllerBase
    {
        private readonly Config _config;
        private readonly IStore _store;
        private readonly ILogger<PackagesController> _logger;

        public PackagesController(IOptions<Config> options, IStore store, ILogger<PackagesController> logger)
        {
            _config = options.Value;
            _store = store;
            _logger = logger;
        }

        [HttpGet("/api/packages/")]
        public async Task<PackagesSummaryViewModel> GetPackagesSummaryAsync()
        {
            var packages = (await _store.GetPackageNamesAsync()).OrderBy(name => name);

            return new PackagesSummaryViewModel
            {
                LocationRoot = _config.LocationRoot,
                Packages = packages
            };
        }

        [HttpGet("/api/packages/{name}")]
        public async Task<IActionResult> GetPackageAsync(string name)
        {
            var (package, etag) = await _store.GetPackageAsync(name);

            if (package == null)
                return NotFound();

            Response.Headers.Add("ETag", etag);
            
            return Ok(new { package.Name });
        }

        [HttpGet("/api/packages/{name}/releases")]
        public async Task<IEnumerable<string>> GetReleaseVersionsAsync(string name)
        {
            return await _store.GetReleaseVersionsAsync(name);
        }

        [HttpGet("/api/packages/{name}/releases/{version}")]
        public async Task<Release> GetReleaseAsync(string name, string version)
        {
            return await _store.GetReleaseAsync(name, version);
        }

        [HttpPost("/api/packages/")]
        public async Task<IActionResult> PostPackage([FromBody]PackageCreationViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var package = new Package
            {
                Name = model.Name,
            };

            await _store.SavePackageAsync(package, "*");

            return Created($"/api/packages/{package.Name}", package);
        }

        [HttpPost("/api/packages/{name}/releases")]
        public async Task<IActionResult> PostRelease([FromRoute]string name, [FromBody]ReleaseCreationViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (package, etag) = await _store.GetPackageAsync(name);

            if (package == null)
                return NotFound();

            if (package.Releases.Any(release => release.Version == model.Version))
                return Conflict();

            var release = new Release
            {
                Version = model.Version,
                Url = model.Url,
                ReleaseAt = DateTimeOffset.Now
            };

            package.Releases.Add(release);

            await _store.SavePackageAsync(package, etag);

            return Created($"/api/packages/{name}/releases/{release.Version}", release);
        }

        [HttpDelete("/api/packages/{name}")]
        public async Task<IActionResult> DeletePackageAsync([FromRoute]string name, [FromHeader(Name = "If-Match")] string etag)
        {
            var (package, _) = await _store.GetPackageAsync(name);

            if (package == null)
                return NotFound();

            await _store.RemovePackageAsync(package, etag);

            return NoContent();
        }

    }
}
