using ModelContextProtocol.Server;
using System.ComponentModel;

namespace McpServerSample.Tools
{
    [McpServerToolType]
    public class SampleTool
    {
        [McpServerTool, Description("サンプルの文字列を返却します")]
        public string GetSampleName() => "Sampleの文字列です。Hello World！！";

    }
}
