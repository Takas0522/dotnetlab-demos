using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompAndBingSearch.Extension
{
    public static class ChatHistoryExtension
    {
        public static string GetChatMessageTag(this ChatHistory messages)
        {
            return string.Join('\n', messages.Select(s => MessageToString(s)));
        }

        private static string MessageToString(ChatMessageContent content) {
            return $@"
                <message role=""{content.Role.Label.ToLowerInvariant()}"">
                    {content}
                </message>
            ";
        }
    }
}
