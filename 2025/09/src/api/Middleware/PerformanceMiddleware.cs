using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System.Diagnostics;

namespace api.Middleware;

public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TelemetryClient _telemetryClient;
    private readonly ILogger<PerformanceMiddleware> _logger;

    public PerformanceMiddleware(RequestDelegate next, TelemetryClient telemetryClient, ILogger<PerformanceMiddleware> logger)
    {
        _next = next;
        _telemetryClient = telemetryClient;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var operationId = Activity.Current?.Id ?? Guid.NewGuid().ToString();

        try
        {
            // リクエスト情報をログ
            _logger.LogInformation("API Request started: {Method} {Path}", 
                context.Request.Method, context.Request.Path);

            // リクエストテレメトリの作成
            var requestTelemetry = new RequestTelemetry
            {
                Name = $"{context.Request.Method} {context.Request.Path}",
                Url = new Uri($"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}"),
                Timestamp = DateTimeOffset.UtcNow,
                Context = { Operation = { Id = operationId } }
            };

            // ユーザー情報の追加
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                requestTelemetry.Context.User.Id = context.User.Identity.Name;
                requestTelemetry.Properties["UserId"] = context.User.Identity.Name ?? "Anonymous";
            }

            await _next(context);

            stopwatch.Stop();

            // 成功時のテレメトリ
            requestTelemetry.Duration = stopwatch.Elapsed;
            requestTelemetry.ResponseCode = context.Response.StatusCode.ToString();
            requestTelemetry.Success = context.Response.StatusCode < 400;

            // カスタムメトリクス
            _telemetryClient.TrackMetric("API.ResponseTime", stopwatch.ElapsedMilliseconds, new Dictionary<string, string>
            {
                ["Endpoint"] = $"{context.Request.Method} {context.Request.Path}",
                ["StatusCode"] = context.Response.StatusCode.ToString(),
                ["UserId"] = context.User?.Identity?.Name ?? "Anonymous"
            });

            _telemetryClient.TrackRequest(requestTelemetry);

            _logger.LogInformation("API Request completed: {Method} {Path} - {StatusCode} in {Duration}ms", 
                context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // エラーテレメトリ
            var exceptionTelemetry = new ExceptionTelemetry(ex)
            {
                Context = { Operation = { Id = operationId } }
            };

            exceptionTelemetry.Properties["Endpoint"] = $"{context.Request.Method} {context.Request.Path}";
            exceptionTelemetry.Properties["UserId"] = context.User?.Identity?.Name ?? "Anonymous";
            exceptionTelemetry.Properties["Duration"] = stopwatch.ElapsedMilliseconds.ToString();

            _telemetryClient.TrackException(exceptionTelemetry);

            // カスタムメトリクス（エラー）
            _telemetryClient.TrackMetric("API.Errors", 1, new Dictionary<string, string>
            {
                ["Endpoint"] = $"{context.Request.Method} {context.Request.Path}",
                ["ExceptionType"] = ex.GetType().Name,
                ["UserId"] = context.User?.Identity?.Name ?? "Anonymous"
            });

            _logger.LogError(ex, "API Request failed: {Method} {Path} - Error: {Error}", 
                context.Request.Method, context.Request.Path, ex.Message);

            throw;
        }
    }
}
