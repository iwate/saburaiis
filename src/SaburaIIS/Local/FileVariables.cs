using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SaburaIIS.Local
{
    public class FileVariables : IVariables
    {
        private readonly string _dir;
        public FileVariables(Config config)
        {
            _dir = config.FileVariablesDirectoryPath;

            if (!Directory.Exists(_dir))
            {
                throw new DirectoryNotFoundException();
            }
        }

        public async Task<IEnumerable<KeyValuePair<string, string>>> GetVariables(string partitionName, string applicationPoolName)
        {
            var path = Path.Combine(_dir, partitionName, applicationPoolName + ".json");
            if (!File.Exists(path)) 
            {
                return Array.Empty<KeyValuePair<string, string>>();
            }

            var json = await File.ReadAllTextAsync(path);
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            if (data == null)
            {
                return Array.Empty<KeyValuePair<string, string>>();
            }

            return data;
        }
    }
}
