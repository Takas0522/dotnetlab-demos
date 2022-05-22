using System;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTesting
{
    public class ���[�U�[�ꗗ���: PageTest
    {
        private string? FrontEndpoint { get; set; }
        [SetUp]
        public void Setup()
        {
            FrontEndpoint = "http://localhost:4200";
        }

        [Test]
        public async Task users�G���h�|�C���g�A�N�Z�X�Ń��[�U�[�̈ꗗ���\������Ă��邱��()
        {
            await Page.GotoAsync($"{FrontEndpoint}/users");
            await Page.WaitForTimeoutAsync(2000);
            var loc = Page.Locator("mat-list-item");
            var rowCn = await loc.CountAsync();
            Assert.That(rowCn, Is.GreaterThan(0));

        }
    }
}