var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.ToDo_APP_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

//builder.AddProject<Projects.ToDo_APP_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithHttpHealthCheck("/health")
//    .WithReference(apiService)
//    .WaitFor(apiService);

builder.Build().Run();
