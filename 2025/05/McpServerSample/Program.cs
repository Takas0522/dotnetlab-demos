using Azure.Storage.Blobs;
using McpServerSample.Middleware;
using McpServerSample.Resources;
using Microsoft.Extensions.Azure;
using ModelContextProtocol.Protocol.Types;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAzureClients(opt => {
    var uri = builder.Configuration["AzureSearch:Uri"];
    var indexName = builder.Configuration["AzureSearch:IndexName"];
    var credential = builder.Configuration["AzureSearch:Credential"];
    opt.AddSearchClient(
        new Uri(uri),
        indexName,
        new Azure.AzureKeyCredential(credential)
    );

    var cnst = builder.Configuration.GetConnectionString("BlobCst");
    opt.AddBlobServiceClient(cnst);
});

builder.Services.AddSingleton<BlobResourcesHandler>();

builder.Services.AddMcpServer()
    .WithToolsFromAssembly()
    .WithHttpTransport()
    .WithListResourceTemplatesHandler(async (context, ct) =>
    {
        var handler = context.Services.GetService<BlobResourcesHandler>();
        return await handler.GetListResourceTemplatesAsync();
    })
    .WithListResourcesHandler(async (context, ct) =>
    {
        var handler = context.Services.GetService<BlobResourcesHandler>();
        return await handler.GetListResourceAsync();
    })
    .WithReadResourceHandler(async (context, ct) => {
        var handler = context.Services.GetService<BlobResourcesHandler>();
        return await handler.GetReadResourceAsync(context.Params.Uri);
    });

var app = builder.Build();
app.MapMcp();

app.UseAuthMiddleware();

app.Run();