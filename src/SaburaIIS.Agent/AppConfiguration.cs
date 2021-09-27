using Azure.Core;
using Azure.Data.AppConfiguration;
using Azure.Identity;
using SaburaIIS.Agent.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaburaIIS.Agent
{
    public class AppConfiguration
    {
        private ConfigurationClient _configClient;
        public AppConfiguration(Config config)
        {
            TokenCredential credential = !string.IsNullOrEmpty(config.AADClientSecret)
                                        ? new ClientSecretCredential(config.AADTenantId, config.AADClientId, config.AADClientSecret)
                                        : new DefaultAzureCredential();
            _configClient = new ConfigurationClient(new Uri(config.GetAppConfigurationEndpoint()), credential);
        }

        public async  Task<IEnumerable<KeyValuePair<string, string>>> GetConfiguration(string partitionName, string applicationPoolName)
            => await _configClient.GetKeyValues(partitionName, applicationPoolName);
    }
}
