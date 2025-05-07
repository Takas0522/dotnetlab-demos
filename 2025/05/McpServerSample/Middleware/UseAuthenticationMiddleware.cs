using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace McpServerSample.Middleware
{
    public static class AutenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            var config = builder.ApplicationServices.GetRequiredService<IConfiguration>();
            return builder.UseMiddleware<AutenticationMiddleware>(Options.Create(config));
        }
    }

    public class AutenticationMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<AutenticationMiddleware> _logger;
        private readonly IConfiguration _config;

        public AutenticationMiddleware(RequestDelegate next, ILogger<AutenticationMiddleware> logger, IOptions<IConfiguration> config)
        {
            _next = next;
            _logger = logger;
            _config = config.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            var user = await AuthenticationService.ValidateRequestAsync(request, _logger, _config);

            if (user != null)
            {
                request.HttpContext.User.AddIdentities(user.Identities);
            }
            else
            {
                context.Response.StatusCode = 401;
                await request.HttpContext.Response.Body.FlushAsync();
                request.HttpContext.Response.Body.Close();
                return;
            }
            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }

    // e.g.:https://philippbauknecht.medium.com/authentication-authorization-in-azure-functions-with-azure-active-directory-using-c-net-aad52c8de925

    public static class AuthenticationService
    {
        public static async Task<ClaimsPrincipal> ValidateRequestAsync(HttpRequest req, ILogger logger, IConfiguration config)
        {
            var accessToken = GetAccessToken(req);
            return await ValidateAccessTokenAsync(accessToken, logger, config);
        }

        private static string GetAccessToken(HttpRequest req)
        {
            var authorizationHeader = req.Headers?["Authorization"];
            var parts = authorizationHeader?.ToString().Split(null) ?? new string[0];
            if (parts.Length == 2 && parts[0].Equals("Bearer"))
                return parts[1];
            return null;
        }

        private static string aadInstance = "https://login.microsoftonline.com/{0}/v2.0";
        private static string GetAuthority(string tenant) => string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
        private static string[] GetValidIssuers(string tenant) => new string[]
        {
            $"https://login.microsoftonline.com/{tenant}/",
            $"https://login.microsoftonline.com/{tenant}/v2.0",
            $"https://login.windows.net/{tenant}/",
            $"https://login.microsoft.com/{tenant}/",
            $"https://sts.windows.net/{tenant}/"
        };

        private static async Task<ClaimsPrincipal> ValidateAccessTokenAsync(string accessToken, ILogger logger, IConfiguration configuration)
        {

            var tenantId = configuration["AzureAd:TenantId"];
            var clientId = configuration["AzureAd:ClientId"];
            var audience = configuration["AzureAd:Audience"];

            if (tenantId == null)
            {
                tenantId = "common";
            }

            var authority = GetAuthority(tenantId);
            var validIssuers = GetValidIssuers(tenantId);

            var configManager =
                new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"{authority}/.well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever());

            var config = await configManager.GetConfigurationAsync();
            var tokenValidator = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidAudiences = new[] { audience, clientId },
                ValidIssuers = tenantId == "common" ? null : validIssuers,
                ValidateIssuer = tenantId != "common",
                IssuerSigningKeys = config.SigningKeys
            };

            try
            {
                SecurityToken securityToken;
                var claimsPrincipal = tokenValidator.ValidateToken(accessToken, validationParameters, out securityToken);
                return claimsPrincipal;
            }
            catch (Exception ex)
            {
                logger.LogError("Auth Error");
            }
            return null;
        }
    }
}
