using SaburaIIS.Azure;
using SaburaIIS.Local;
using SaburaIIS.Remote;
using System.Net.Http;

namespace SaburaIIS
{
    public class Factory
    {
        public static HttpClient _httpClient = new HttpClient();
        public static IStorage CreateStorage(Config config)
        {
            if (!string.IsNullOrEmpty(config.FileStorageDirectoryPath))
            {
                return new Storage([
                        new FileStorage(config),
                        new PublicHttpStorage(_httpClient)
                    ]);
            }

            return new Storage([
                        new AzBlobStorage(config),
                        new PublicHttpStorage(_httpClient)
                    ]);
        }

        public static IStore CreateStore(Config config)
        {
            if (!string.IsNullOrEmpty(config.FileStoreDirectoryPath))
            {
                return new FileStore(config);
            }

            return new CosmosDbStore(config);
        }

        public static IVariables CreateVariables(Config config)
        {
            if (!string.IsNullOrEmpty(config.FileVariablesDirectoryPath))
            {
                return new FileVariables(config);
            }

            return new AppConfigurationVariables(config);
        }

        public static IVault CreateVault(Config config)
        {
            if (config.UseMachineVault)
            {
                return new MachineVault(config);
            }

            return new AzKeyVault(config);
        }
    }
}
