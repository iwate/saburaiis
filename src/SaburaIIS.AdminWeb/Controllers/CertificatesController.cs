using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SaburaIIS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaburaIIS.AdminWeb.Controllers
{
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly IVault _keyVault;
        private readonly ILogger<CertificatesController> _logger;

        public CertificatesController(IVault keyVault, ILogger<CertificatesController> logger)
        {
            _keyVault = keyVault;
            _logger = logger;
        }

        [HttpGet("/api/certificates")]
        public async Task<IEnumerable<Certificate>> Get()
        {
            return (await _keyVault.GetCertificatesAsync()).OrderByDescending(o => o.ExpiresOn);
        }
    }
}
