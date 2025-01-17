using Azure.Data.AppConfiguration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaburaIIS.Extensions
{
    internal static class ConfigurationClientExtensions
    {
        public static async Task<IEnumerable<KeyValuePair<string, string>>> GetKeyValues(this ConfigurationClient client, string partitionName, string apppoolName)
        {
            var result = new List<KeyValuePair<string, string>>();
            var pages = client.GetConfigurationSettingsAsync(new SettingSelector { LabelFilter = $"{partitionName}/{apppoolName}" });
            await foreach (var page in pages.AsPages())
            {
                result.AddRange(page.Values.Select(setting => new KeyValuePair<string, string>(setting.Key, setting.Value)));
            }
            return result;
        }
    }
}
