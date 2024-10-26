using DemoLib.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text.Json;
#pragma warning disable SKEXP0010 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。
internal class Program
{
    private static async Task Main(string[] args)
    {
        var hostBuilder = DemoLib.DemoHost.ConfigureHostBuilder();
        var host = hostBuilder.Build();
        var kernel = host.Services.GetRequiredService<Kernel>();

        // Eventのサンプル構造体を作成できるか試してみる。
        var settings = new OpenAIPromptExecutionSettings
        {
            ResponseFormat = typeof(EventsResult)
        };

        var prompt = @"
            下記のコンテンツから予定情報を取得してください
            イベントの構成情報はタイトルのSubject、本文のBody、開始日時のStart、終了日時のEndを含む必要があります。
            [メールの内容]
            2024年10月26日の14:15から14:45で .NETラボの発表の予定をおさえておいてください。
            ";

        var result = await kernel.InvokePromptAsync(prompt, new(settings));

        var eventsResult = JsonSerializer.Deserialize<EventsResult>(result.ToString());

        eventsResult.Events.ForEach(e =>
        {
            Console.WriteLine("--------------------Event--------------------");
            Console.WriteLine($"Subject: {e.Subject}");
            Console.WriteLine($"Body: {e.Body}");
            Console.WriteLine($"Start: {e.Start}");
            Console.WriteLine($"End: {e.End}");
        });

    }
}
#pragma warning restore SKEXP0010 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。