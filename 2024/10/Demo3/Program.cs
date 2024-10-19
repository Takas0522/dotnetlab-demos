using DemoLib.Model;
using DemoLib.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

#pragma warning disable SKEXP0001 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。
#pragma warning disable SKEXP0010 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。
internal class Program
{
    static async Task Main(string[] args)
    {
        var hostBuilder = DemoLib.DemoHost.ConfigureHostBuilder();
        var host = hostBuilder.Build();
        var kernel = host.Services.GetRequiredService<Kernel>();

        // まずはログインをする
        var authService = host.Services.GetRequiredService<AuthService>();
        await authService.Login();

        var settings = new OpenAIPromptExecutionSettings
        {
            ResponseFormat = typeof(EventsResult),
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var prompt = @"
            .NETラボのデモで使用するメールを取得してください。
            ";

        var result = await kernel.InvokePromptAsync(prompt, new(settings));
        Console.WriteLine(result.ToString());
    }
}
#pragma warning restore SKEXP0010 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。
#pragma warning restore SKEXP0001 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。
