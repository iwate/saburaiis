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
            _model = new WorkerModel(
                config,
                Factory.CreateStore(config),
                Factory.CreateStorage(config),
                Factory.CreateVault(config),
                Factory.CreateVariables(config),
                new CertificateStoreFactory(),
                new Mapper(),
                new ServerConfigWatcher(loggerFactory.CreateLogger<ServerConfigWatcher>()),
                loggerFactory.CreateLogger<WorkerModel>());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _model.ExecuteAsync(stoppingToken);
        }
    }
}
