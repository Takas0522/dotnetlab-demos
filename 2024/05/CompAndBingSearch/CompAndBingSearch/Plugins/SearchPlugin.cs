using CompAndBingSearch.Models;
using MessagePack;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace CompAndBingSearch.Plugins
{
    [Description("検索関係のプラグインのサンプルです")]
    public class SearchPlugin
    {
        //private readonly Kernel _kernel;
        //public SearchPlugin(Kernel kernel)
        //{
        //    _kernel = kernel;
        //}

        [KernelFunction]
        [Description("検索結果をベースにURLとタイトルと要約をセットにして返却します")]
        [return: Description("URLとタイトルと要約のセット")]
        public async Task<string> GetContentsDataAsync(
            Kernel kernel,
            [Description("検索を実施してURLとタイトルの要約のセットを返却する")] string query
        )
        {
            var promptTemplateFactory = new KernelPromptTemplateFactory();
            var promptTemplate = promptTemplateFactory.Create(new PromptTemplateConfig(@"{{ bing.GetSearchResults query='"+ query + "' count='2' }}"));
            var information = await promptTemplate.RenderAsync(kernel);

            var unescapeText = Regex.Unescape(information);
            var encodeText = HttpUtility.HtmlDecode(unescapeText);
            // HTMLエンコードされているのでデコードしてデータとして取り扱えるようにする
            var byteData = MessagePackSerializer.ConvertFromJson(encodeText);
            //bing.GetSearchResultsの結果は name/url/snippetで構成される
            var results = MessagePackSerializer.Deserialize<IEnumerable<GetSearhResult>>(byteData);

            // bing.searchでもう少し詳しくDescrptionを収拾する
            var returnText = "";
            foreach ( var result in results.Select((item, index) => new { item, index }) )
            {
                var url = result.item.Url;
                var searchPromptTemplate = promptTemplateFactory.Create(new PromptTemplateConfig(@"{{ bing.search '" + url +"' }}"));
                var descrption = await searchPromptTemplate.RenderAsync(kernel);
                var unescapeDescription = Regex.Unescape(descrption);
                var encodeDescription = HttpUtility.HtmlDecode(unescapeDescription);

                var resText = $"- [{result.item.Name}]({result.item.Url}): {encodeDescription}";
                //var resText = $@"[情報{result.index}]
                //- タイトル: {result.item.Name}
                //- 概要: {encodeDescription}";
                returnText += $"{resText}\n";
            }
            return returnText;
        }
    }
}
