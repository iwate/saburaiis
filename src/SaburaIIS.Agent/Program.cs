using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SaburaIIS.Agent;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(builder => {
        builder.AddCommandLine(args);
    })
    .UseWindowsService(options =>
    {
        options.ServiceName = "SaburaIIS";
    })
    .ConfigureServices(services =>
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        services.Configure<Config>(configuration.GetSection("SaburaIIS"));
        services.AddHttpClient();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();