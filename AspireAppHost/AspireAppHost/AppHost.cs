
var builder = DistributedApplication.CreateBuilder(args);

// Add RabbitMQ with a data volume
var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin();

// Add ModuleA and ModuleB as projects and reference RabbitMQ
builder.AddProject("modulea", "../ModuleA/ModuleA.csproj")
       .WithReference(rabbitmq);

builder.AddProject("moduleb", "../ModuleB/ModuleB.csproj")
       .WithReference(rabbitmq);

builder.Build().Run();
