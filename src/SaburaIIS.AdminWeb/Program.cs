using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SaburaIIS;
using SaburaIIS.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .Configure<Config>(builder.Configuration.GetSection("SaburaIIS"))
    .AddSingleton(sp => Factory.CreateStore(sp.GetRequiredService<IOptions<Config>>().Value))
    .AddSingleton(sp => Factory.CreateVault(sp.GetRequiredService<IOptions<Config>>().Value))
    .AddControllersWithViews()
    .AddJsonOptions(configure => {
        configure.JsonSerializerOptions.Converters.Add(new DateTimeOffsetConverter());
        configure.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
        configure.JsonSerializerOptions.Converters.Add(new BinaryConverter());
    });

var app = builder.Build();

await app.Services.GetRequiredService<IStore>().InitAsync();

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseRouting();

app.MapControllerRoute("default", "{__tenant__=}/{controller=}/{action=}/{id?}");

app.MapFallbackToFile("/index.html");

app.Run();
