using Miharu.BackEnd.Helper;
using OpenQA.Selenium;
using System;

namespace Miharu.BackEnd.Translation.WebCrawlers
{
    public class WCGoogleTranslator : WebCrawlerTranslator
    {
        private readonly string _URL;

        public WCGoogleTranslator(WebDriverManager webDriverManager,
                                  TesseractSourceLanguage tesseractSourceLanguage,
                                  TranslationTargetLanguage translationTargetLanguage)
            : base(webDriverManager)
        {
            _URL = string.Format("https://translate.google.com/#view=home&op=translate&sl={0}&tl={1}&text=",
                                 tesseractSourceLanguage.ToTranslationSourceLanguageParameter(),
                                 translationTargetLanguage.ToTranslationTargetLanguageParameter());
        }

        public override TranslationType Type => TranslationType.Google_Web;

        protected override By FetchBy => By.XPath("//div[@class='zkZ4Kc dHeVVb']");

        protected override string GetUri(string text)
        {
            return _URL + Uri.EscapeDataString(text);
        }

        public override string ProcessResult(IWebElement result)
        {
            string res = "";
            res += result.GetAttribute("data-text");

            return res;
        }
    }
}

