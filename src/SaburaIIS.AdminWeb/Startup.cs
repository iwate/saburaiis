using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SaburaIIS;
using SaburaIIS.Json;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.
            Configure<Config>(Configuration.GetSection("SaburaIIS"))
            .AddSingleton(sp => Factory.CreateStore(sp.GetRequiredService<IOptions<Config>>().Value))
            .AddSingleton(sp => Factory.CreateVault(sp.GetRequiredService<IOptions<Config>>().Value))
            .AddControllersWithViews()
            .AddJsonOptions(configure => {
                configure.JsonSerializerOptions.Converters.Add(new DateTimeOffsetConverter());
                configure.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
                configure.JsonSerializerOptions.Converters.Add(new BinaryConverter());
            });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.ApplicationServices.GetRequiredService<IStore>().InitAsync().Wait();

        app.UseStaticFiles();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/error");
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute("default", "{__tenant__=}/{controller=}/{action=}/{id?}");
            endpoints.MapFallbackToFile("/index.html");
        });
    }
}
