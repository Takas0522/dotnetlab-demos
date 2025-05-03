using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using McpServerSample.Models;
using Microsoft.Extensions.Azure;
using ModelContextProtocol.Server;
using System.Collections;
using System.ComponentModel;

namespace McpServerSample.Tools
{
    [McpServerToolType]
    public class Wikiman
    {

        private readonly SearchClient _client;

        public Wikiman(
            SearchClient client
        ) {
            _client = client;
        }

        [McpServerTool, Description("""
            業務に関わる情報の検索を行い結果を返却します。
            回答はここで検索された内容を参考にする必要があります。
            結果にはスコアが設定されており、スコアが高いほど検索で欲しい情報に近いものとなります。
            """)]
        public async Task<IEnumerable<ToolSearchResult>> SearchAsync([Description("検索する文字列")]string searchWord)
        {
            var res = await _client.SearchAsync<SearchDocument>(searchWord);
            var searchResults = res.Value.GetResults();
            var returnResult = searchResults.Select(s => {
                var score = s.Score ?? 0;
                var contents = s.Document.Where(w => w.Key == "content")?.First();
                var contentsDocument = contents == null ? "" : contents.Value.ToString();
                return new ToolSearchResult() {
                    Score = score,
                    Document = new Document() {
                        Content = contentsDocument
                    }
                };
            });
            return returnResult;
        }

    }
}
