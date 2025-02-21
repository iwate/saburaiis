using Azure.Core;
using Azure.Data.AppConfiguration;
using Azure.Identity;
using SaburaIIS.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaburaIIS.Azure
{
    public class AppConfigurationVariables : IVariables
    {
        private ConfigurationClient _configClient;
        public AppConfigurationVariables(Config config)
        {
            TokenCredential credential = !string.IsNullOrEmpty(config.AADClientSecret)
                                                    ? new ClientSecretCredential(config.AADTenantId, config.AADClientId, config.AADClientSecret)
                                                    : new DefaultAzureCredential();
            _configClient = new ConfigurationClient(new Uri(config.GetAppConfigurationEndpoint()), credential);
        }

        public async Task<IEnumerable<KeyValuePair<string, string>>> GetVariables(string partitionName, string applicationPoolName)
        {
            return await _configClient.GetKeyValues(partitionName, applicationPoolName);
        }
    }
}
