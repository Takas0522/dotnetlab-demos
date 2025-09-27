using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System.Net;
using System.Text.Json;

namespace api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TelemetryClient _telemetryClient;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, TelemetryClient telemetryClient, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _telemetryClient = telemetryClient;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var operationId = System.Diagnostics.Activity.Current?.Id ?? Guid.NewGuid().ToString();

        // Application Insightsにエラーを送信
        var exceptionTelemetry = new ExceptionTelemetry(exception)
        {
            Context = { Operation = { Id = operationId } }
        };

        // コンテキスト情報を追加
        exceptionTelemetry.Properties["RequestPath"] = context.Request.Path;
        exceptionTelemetry.Properties["RequestMethod"] = context.Request.Method;
        exceptionTelemetry.Properties["UserId"] = context.User?.Identity?.Name ?? "Anonymous";
        exceptionTelemetry.Properties["UserAgent"] = context.Request.Headers.UserAgent.ToString();
        exceptionTelemetry.Properties["RemoteIpAddress"] = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        _telemetryClient.TrackException(exceptionTelemetry);

        // ログ出力
        _logger.LogError(exception, "Unhandled exception occurred. OperationId: {OperationId}", operationId);

        // レスポンスの設定
        context.Response.ContentType = "application/json";
        
        var response = exception switch
        {
            ArgumentException => new { 
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "無効なパラメータが指定されました。",
                Details = exception.Message,
                OperationId = operationId
            },
            UnauthorizedAccessException => new { 
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Message = "認証が必要です。",
                Details = "",
                OperationId = operationId
            },
            KeyNotFoundException => new { 
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = "指定されたリソースが見つかりません。",
                Details = "",
                OperationId = operationId
            },
            _ => new { 
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "内部サーバーエラーが発生しました。",
                Details = "",
                OperationId = operationId
            }
        };

        context.Response.StatusCode = response.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
