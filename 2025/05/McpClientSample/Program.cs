// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using System.Net.Http;
using System.Net.Http.Headers;

Console.WriteLine("Hello, World!");


// 今回はSSEを使うのでSseClientTransportに食わせるHttpClientにDelegateをかませてみる
var httpClient = HttpClientFactory.Create(new AuthHandler());
var clientTransport = new SseClientTransport(new SseClientTransportOptions() {
    Endpoint = new Uri("http://localhost:5028/sse"),
    Name = "SampleMcpServer"
}, httpClient);

await using var client = await McpClientFactory.CreateAsync(clientTransport);
var res = await client.ListToolsAsync();
Console.WriteLine(res.Count);

var response = await client.ListResourceTemplatesAsync();
Console.WriteLine(response.Count);


// 超雑だけどトークンをキャッシュする変数を作成

public static class State
{
    public static string TokenCache { get; set; } = string.Empty;
}


public class AuthHandler : DelegatingHandler
{
    private readonly IPublicClientApplication _msalClient;
    private readonly string _scopes;

    public AuthHandler()
    {
        // 認証で使う設定情報をロード
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();

        // 認証で利用するMSALクライアントを作成
        _msalClient = PublicClientApplicationBuilder.CreateWithApplicationOptions(new PublicClientApplicationOptions
        {
            TenantId = config["AzureAd:TenantId"],
            ClientId = config["AzureAd:ClientId"],
        }).WithDefaultRedirectUri().Build();

        _scopes = config["AzureAd:Scopes"];
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (State.TokenCache == string.Empty)
        {
            var scopes = new List<string> { _scopes };
            var req = _msalClient.AcquireTokenInteractive(scopes);
            var res = await req.ExecuteAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", res.AccessToken);
            State.TokenCache = res.AccessToken;
            Console.WriteLine(res.AccessToken);
        }
        else
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", State.TokenCache);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}