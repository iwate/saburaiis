namespace SaburaIIS.Agent
{
    public class Config : SaburaIIS.Config
    {
        public string? ScaleSetName { get; set; }

        public int PoolingDelaySecForInit { get; set; } = 60;
        public int PoolingDelaySecForAssign { get; set; } = 15;
        public int PoolingDelaySecForUpdate { get; set; } = 1;
    }
}
