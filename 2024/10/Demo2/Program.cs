using DemoLib.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text.Json;
#pragma warning disable SKEXP0010 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。
internal class Program
{
    public static async Task Main(string[] args)
    {
        var hostBuilder = DemoLib.DemoHost.ConfigureHostBuilder();
        var host = hostBuilder.Build();
        var kernel = host.Services.GetRequiredService<Kernel>();

        // Eventのサンプル構造体に対してデータの不足が発生していた場合の挙動を確認する。
        var settings = new OpenAIPromptExecutionSettings
        {
            ResponseFormat = typeof(EventsResult)
        };

        // Endが不足している
        var prompt = @"
            以降に記載される列挙情報を使用してイベントデータを作成してください。イベントはEvents配列に格納されます。
            * subject: 新しい予定
            * body: 本文
            * start: 2022-01-01T00:00:00
            * subject: 新しい予定2
            * body: 本文2
            * start: 2022-01-02T00:00:00
            作成される構造体に不足している情報がある場合は 不足項目に **Error** と表示されます。
            ";

        var result = await kernel.InvokePromptAsync(prompt, new(settings));

        if (result.ToString().Contains("Error"))
        {
            Console.WriteLine("データに不足があります。");
            return;
        }
        var eventsResult = JsonSerializer.Deserialize<EventsResult>(result.ToString());

        Console.WriteLine(eventsResult);
    }
}
#pragma warning restore SKEXP0010 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。