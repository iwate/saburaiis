namespace SaburaIIS
{
    public class Config
    {
        public string SubscriptionId { get; set; }
        public string ResourceGroupName { get; set; }
        public string CosmosDbName { get; set; }
        public string CosmosDbEndpoint { get; set; }
        public string AADTenantId { get; set; }
        public string AADClientId { get; set; }
        public string AADClientSecret { get; set; }

        public string DatabaseName { get; set; } = "saburaiis";
        public string ContainerNamePrefix { get; set; } = "";
        public string LocationRoot { get; set; } = @"%SystemDrive%\inetpub\sites";
        public string AppLocationDefault { get; set; } = @"%SystemDrive%\inetpub\wwwroot";
        public string LogLocationDefault { get; set; } = @"%SystemDrive%\inetpub\logs\LogFiles";

        public string GetCosmosDbEndpoint() => CosmosDbEndpoint ?? $"https://{CosmosDbName}.documents.azure.com/";
    }
}
