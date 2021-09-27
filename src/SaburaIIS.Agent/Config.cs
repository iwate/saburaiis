namespace SaburaIIS.Agent
{
    public class Config : SaburaIIS.Config
    {
        public string? ScaleSetName { get; set; }
        private string? _appConfigName;
        public string AppConfigurationName
        {
            get => string.IsNullOrEmpty(_appConfigName) ? ResourceGroupName : _appConfigName;
            set => _appConfigName = value;
        }
        public string? AppConfigurationEndpoint { get; set; }
        public string GetAppConfigurationEndpoint() => AppConfigurationEndpoint ?? $"https://{AppConfigurationName}.azconfig.io";

        public int PoolingDelaySecForInit { get; set; } = 60;
        public int PoolingDelaySecForAssign { get; set; } = 15;
        public int PoolingDelaySecForUpdate { get; set; } = 1;
    }
}
