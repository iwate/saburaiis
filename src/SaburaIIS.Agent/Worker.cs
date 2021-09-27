using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SaburaIIS.Agent.Mappers;

namespace SaburaIIS.Agent
{
    public class Worker : BackgroundService
    {
        private readonly WorkerModel _model;

        public Worker(IOptions<Config> options, ILoggerFactory loggerFactory)
        {
            var config = options.Value;
            var store = new Store(config);
            _model = new WorkerModel(
                config,
                store,
                new Storage(config),
                new KeyVault(config),
                new AppConfiguration(config),
                new CertificateStoreFactory(),
                new Mapper(),
                new Reporter(config.ScaleSetName!, store, loggerFactory.CreateLogger<Reporter>()),
                loggerFactory.CreateLogger<WorkerModel>());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _model.ExecuteAsync(stoppingToken);
        }
    }
}
