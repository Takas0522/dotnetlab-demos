using DemoLib.Model;
using DemoLib.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

#pragma warning disable SKEXP0001 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。
internal class Program
{
    static async Task Main(string[] args)
    {
        var hostBuilder = DemoLib.DemoHost.ConfigureHostBuilder();
        var host = hostBuilder.Build();
        var kernel = host.Services.GetRequiredService<Kernel>();
        var chatCom = kernel.GetRequiredService<IChatCompletionService>();

        // まずはログインをする
        var authService = host.Services.GetRequiredService<AuthService>();
        await authService.Login();

        var settings = new OpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        // 普通のチャットサービスみたいな感じで構成してみる
        var history = new ChatHistory();
        history.AddSystemMessage(@"
            あなたはチャットアシスタントです。
            デモがしたいという依頼があったらメールを取得してください。
            予定に関するやりとりが発生した場合は予定情報を提示してその予定を登録するかどうかユーザーに確認してください。
            確認の結果登録してよい場合は、メールの文章を構造体に変換し予定の登録を行います。
        ");

        var init = await chatCom.GetChatMessageContentsAsync(history, executionSettings: settings, kernel: kernel);
        Console.Write("Assistant >");
        Console.WriteLine(init.First().ToString());

        string userInput = "";

        do
        {
            Console.Write("User >");
            userInput = Console.ReadLine();

            history.AddUserMessage(userInput);

            var res = await chatCom.GetChatMessageContentsAsync(history, executionSettings: settings, kernel: kernel);
            Console.Write("Assistant >");
            Console.WriteLine(res.First().ToString());
        } while (userInput != "");
    }
}
#pragma warning restore SKEXP0001 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。
