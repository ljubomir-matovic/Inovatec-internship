using Microsoft.Extensions.Hosting;
using Inovatec.OfficeManagementTool.ServiceInitializer;
using Microsoft.Extensions.DependencyInjection;
using Inovatec.OfficeManagementTool.Scheduler;
using Inovatec.OfficeManagementTool.Common;
using Microsoft.Extensions.Configuration;
using System.Reflection;

IHost host = Host.CreateDefaultBuilder(args)
.ConfigureAppConfiguration((configuration) =>
{
    var configRoot = configuration
        .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
        .AddJsonFile("appsettings.json").Build();

    configRoot.Setup();
})
.ConfigureServices(services =>
{
    services.InitializeServicesForScheduler();
    services.AddSingleton<IApplication, Application>();
}).Build();

var app = host.Services.GetRequiredService<IApplication>();

await app.Run();