using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SaburaIIS.AdminWeb;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(o => o.UseStartup<Startup>())
    .Build()
    .Run();
