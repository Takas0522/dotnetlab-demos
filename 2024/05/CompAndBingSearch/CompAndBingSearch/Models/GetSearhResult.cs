using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompAndBingSearch.Models
{
    [MessagePackObject]
    public class GetSearhResult
    {
        [Key("name")]
        public string Name { get; set; } = "";

        [Key("url")]
        public string Url { get; set; } = "";

        [Key("snippet")]
        public string Snippet { get; set; } = "";
    }
}
