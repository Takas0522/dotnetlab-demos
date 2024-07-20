using api.Domains;
using System.Configuration;

// Featureフラグを利用する場合
using Microsoft.FeatureManagement;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using api.Filters;

var builder = WebApplication.CreateBuilder(args);

// AuthSettings
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(opt =>
    {
        builder.Configuration.Bind("AzureAd", opt);

    }, opt => { builder.Configuration.Bind("AzureAd", opt); });

// ローカルのAppSettingsでFeatureフラグを利用する場合
#if DEBUG
//builder.Services.AddFeatureManagement(builder.Configuration.GetSection("FeatureFlags"));
// 対話型でAzure.Identityの認証通してもいいんだけど面倒なのでシークレットで…
// 個人的には基本appsetting.jsonの情報をベースにローカルで開発。
// AppCofigの動作確認したいときは対話型。Azureでの利用はManaged Indentityを使うのがいいかなと思ってます。
builder.Configuration.AddAzureAppConfiguration(opt => {
    opt.Connect(
        builder.Configuration["ConnectionStrings:AppConfig"]
    ).UseFeatureFlags();
});
# endif
builder.Services.AddFeatureManagement()
    .AddFeatureFilter<RandomFilter>()
    .WithTargeting<UserTargetingContextAccessor>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAzureAppConfiguration();

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
app.UseAuthorization();

app.MapControllers();

app.UseAzureAppConfiguration();

app.Run();
