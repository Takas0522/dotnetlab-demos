using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMcpServer()
    .WithToolsFromAssembly()
    .WithHttpTransport();


builder.Services.AddAzureClients(opt => {
    var uri = builder.Configuration["AzureSearch:Uri"];
    var indexName = builder.Configuration["AzureSearch:IndexName"];
    var credential = builder.Configuration["AzureSearch:Credential"];
    opt.AddSearchClient(
        new Uri(uri),
        indexName,
        new Azure.AzureKeyCredential(credential)
    );
});

var app = builder.Build();
app.MapMcp();

app.Run();