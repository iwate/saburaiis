using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaburaIIS.Agent
{
    public class EnvironmentVariablesWriter
    {
        private readonly ServerManager _manager;
        public EnvironmentVariablesWriter(ServerManager manager)
        {
            _manager = manager;
        }

        public async Task WriteAsync(IEnumerable<string> apppoolNames, Func<string, Task<IEnumerable<KeyValuePair<string, string>>>> keyValues)
        {
            var config = _manager.GetApplicationHostConfiguration();
            var applicationPoolsSection = config.GetSection("system.applicationHost/applicationPools");
            var applicationPoolsCollection = applicationPoolsSection.GetCollection();

            foreach (var apppoolName in apppoolNames)
            {
                foreach (ConfigurationElement el in applicationPoolsCollection)
                {
                    if (string.Equals(el.ElementTagName, "add", StringComparison.OrdinalIgnoreCase)
                    && (string)el.GetAttributeValue("name") == apppoolName)
                    {
                        var environmentVariablesCollection = el.GetCollection("environmentVariables");
                        environmentVariablesCollection.Clear();
                        foreach (var setting in await keyValues(apppoolName))
                        {
                            var addElement = environmentVariablesCollection.CreateElement("add");
                            addElement["name"] = setting.Key;
                            addElement["value"] = setting.Value;
                            environmentVariablesCollection.Add(addElement);
                        } 
                    }
                }
            }
        }
    }
}
