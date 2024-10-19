using DemoLib.Services;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.Plugins
{
    public class EmailPlugin
    {
        private readonly EmailService _emailService;
        public EmailPlugin(EmailService emailService)
        {
            _emailService = emailService;
        }

        [KernelFunction("get_email")]
        [Description("デモで利用するメールを取得します")]
        [return: Description("メールの内容を返却します。データが取得できなかったらNOT ACCESSを返却します。")]
        public async Task<string> GetEmail()
        {
            var res = await _emailService.GetEmailMessage();
            if (res != "")
            {
                return res;
            }
            return "NOT ACCESS";
        }
    }
}
