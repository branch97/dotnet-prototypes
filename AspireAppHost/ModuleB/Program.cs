using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ModuleB;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging();

builder.Services.AddHostedService<ModuleService>();

await builder.Build().RunAsync();

