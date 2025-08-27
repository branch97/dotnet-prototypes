var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject("modulea", "../ModuleA/ModuleA.csproj");

builder.AddProject("moduleb", "../ModuleB/ModuleB.csproj");

await builder.Build().RunAsync();
