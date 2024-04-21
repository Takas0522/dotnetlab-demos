using api.Domains;
using System.Configuration;

// Featureフラグを利用する場合
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

// Featureフラグを利用する場合
builder.Services.AddFeatureManagement(builder.Configuration.GetSection("FeatureFlags"));

// Add services to the container.
builder.Services.AddScoped<IMyDataDomain, MyDataDomain>();
builder.Services.AddScoped<INewMyDataDomain, NewMyDataDomain>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#if DEBUG
builder.Services.AddCors(opt => {
    opt.AddPolicy(name: "Def",policy => {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
#endif

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors("Def");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
