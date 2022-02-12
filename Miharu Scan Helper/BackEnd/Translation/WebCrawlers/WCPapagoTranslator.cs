using Miharu.BackEnd.Helper;
using OpenQA.Selenium;
using System;

namespace Miharu.BackEnd.Translation.WebCrawlers
{
    public class WCPapagoTranslator : WebCrawlerTranslator
    {
        private readonly string _URL;

        public WCPapagoTranslator(WebDriverManager webDriverManager, TesseractSourceLanguage tesseractSourceLanguage) : base(webDriverManager)
        {
            _URL = "https://papago.naver.com/?sk=" + tesseractSourceLanguage.ToTranslationSourceLanguageParameter() + "&tk=en&st=";
        }

        public override TranslationType Type => TranslationType.Papago_Web;

        protected override By FetchBy => By.XPath("//div[@class='edit_box___1KtZ3 active___3VPGL']");

        protected override string GetUri(string text)
        {
            return _URL + Uri.EscapeDataString(text);
        }

        public override string ProcessResult(IWebElement result)
        {
            string res = "";
            result.FindElement(By.TagName("span"));
            res += result.Text;

            return res;
        }
    }
}
