using CompAndBingSearch.Extension;
using CompAndBingSearch.Models;
using CompAndBingSearch.Plugins;
using Google.Apis.CustomSearchAPI.v1.Data;
using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Web;
#pragma warning disable SKEXP0050 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。
namespace CompAndBingSearch
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var hb = ConfigureHostBuilder();
            var host = hb.Build();

            //var question = "C#のDelegateとはなんでしょうか？";
            var question = "Microsoftの株価を教えてください";
            //var question = "2024年お勧めアニメ";
            var kernel = host.Services.GetService<Kernel>();

            var func = kernel.CreateFunctionFromPrompt(@"
                あなたは下記のことを行い回答を行ってください。
                1. 正確に回答できるようであれば回答してください
                2. リアルタイムの情報が必要となる場合は回答例3の答えのようなコマンドを発行してください。
                [回答例1]
                質問: 日本で一番大きな湖はなんですか？
                答え: 琵琶湖です。
                [回答例2]
                質問: 日本の首都はどこでしょうか？
                答え: 東京です。
                [回答例3]
                質問: DevTakasの最近の活動は？
                答え: 
                {{ '{{' }} SearchPlugin.GetContentsData 'DevTakas,活動,Twitter' {{'}}'}}
                [実行するタスク]
                質問: {{ $question }}
                答え: 
            ", new OpenAIPromptExecutionSettings { MaxTokens = 200, Temperature = 0 });
            var res = await kernel.InvokeAsync(func, new KernelArguments { ["question"] = question, ["externalInformation"] = string.Empty });
            var resSt = res.GetValue<string>();
            var answer = "";
            answer = res.GetValue<string>();

            //Console.WriteLine(answer);

            // 答えれない質問の場合はBingる
            if (resSt.Contains("SearchPlugin.GetContentsData", StringComparison.OrdinalIgnoreCase))
            {
                var promptTemplateFactory = new KernelPromptTemplateFactory();
                var promptTemplate = promptTemplateFactory.Create(new PromptTemplateConfig(resSt));
                var information = await promptTemplate.RenderAsync(kernel);

                var func2 = kernel.CreateFunctionFromPrompt(@"
                提供された情報を使って質問に回答してください。回答には参考情報へのリンクを箇条書きで含めてください。
                [提供された情報]
                {{ $externalInformation }}
                質問: {{ $question }}
                答え: 
            ", new OpenAIPromptExecutionSettings { MaxTokens = 200, Temperature = 0 });

                var bingAnswer = await kernel.InvokeAsync(func2, new KernelArguments()
                {
                    ["question"] = question,
                    ["externalInformation"] = information
                });
                answer = bingAnswer.GetValue<string>();

                Console.WriteLine(answer);
            }

            // 履歴の情報含めて回答を作成して返信する
            var prompt = $@"
                <message role=""system"">
                    あなたはデジタルアシスタントです。
                    以下の参考情報も踏まえてユーザーからの質問に回答してください。
                    参考情報を利用する場合は提供元のリンクを箇条書きで掲載してください。
                    [参考情報]
                    {{{{ $externalInformation }}}}
                </message>
                {{{{ $history }}}}
            ";
            var history = new ChatHistory();
            history.AddUserMessage(question);
            var historyPrompt = history.GetChatMessageTag();

            var promptres = await kernel.InvokePromptAsync(prompt, arguments: new KernelArguments {
                ["externalInformation"] = answer,
                ["history"] = historyPrompt
            });

            Console.WriteLine(promptres);
        }

        private static IHostBuilder ConfigureHostBuilder()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json");
            configBuilder.AddEnvironmentVariables();
            var config = configBuilder.Build();

            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder => {
                    builder.AddJsonFile("appsettings.json");
                })
                .ConfigureServices(s => {
                    s.AddAzureOpenAIChatCompletion(
                        "gpt35",
                        config["AZURE_OPENAI_API_URL"],
                        config["AZURE_OPENAI_API_KEY"]
                    );
                    s.AddKernel();
                    s.AddSingleton(sp =>
                    {
                        var bingSearchKey = config["BING_SEARCH_KEY"];
                        var bingConnector = new BingConnector(bingSearchKey);
                        var plugin = new WebSearchEnginePlugin(bingConnector);
                        return KernelPluginFactory.CreateFromObject(plugin, "bing");
                    });
                    s.AddSingleton(sp => KernelPluginFactory.CreateFromType<SearchPlugin>(serviceProvider: sp));
                });
            return hostBuilder;
        }
    }
}
#pragma warning restore SKEXP0050 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。