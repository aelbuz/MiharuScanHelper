using Miharu.BackEnd.Helper;
using OpenQA.Selenium;
using System;
using System.Threading.Tasks;

namespace Miharu.BackEnd.Translation.WebCrawlers
{
    public class WCDeepLTranslator : WebCrawlerTranslator
    {
        private readonly string _URL;

        public WCDeepLTranslator(WebDriverManager webDriverManager,
                                 TesseractSourceLanguage tesseractSourceLanguage,
                                 TranslationTargetLanguage translationTargetLanguage)
            : base(webDriverManager)
        {
            _URL = string.Format("https://www.deepl.com/translator#{0}/{0}/",
                                 tesseractSourceLanguage.ToTranslationSourceLanguageParameter(),
                                 translationTargetLanguage.ToTranslationTargetLanguageParameter());
        }

        public override TranslationType Type
        {
            get { return TranslationType.DeepL_Web; }
        }

        protected override By FetchBy
        {
            get { return By.XPath("//p[@class='lmt__translations_as_text__item lmt__translations_as_text__main_translation']"); }
        }

        protected override string GetUri(string text)
        {
            return _URL + Uri.EscapeDataString(text);
        }

        public override async Task<string> Translate(string text)
        {
            string res = "";
            if (text == "")
                return res;
            res = _webDriverManager.NavigateAndFetch(GetUri(text), FetchBy, ProcessResult, OverrideNavigation);

            return res;
        }

        public IWebElement OverrideNavigation(IWebDriver driver, string url)
        {
            IWebElement result = null;
            string res = "";

            driver.Navigate().GoToUrl(url);
            int i = 0;
            for (i = 0; i < 20 && res == ""; i++)
            {
                result = driver.FindElement(By.XPath("//textarea[@class='lmt__textarea lmt__target_textarea lmt__textarea_base_style']"));
                result.SendKeys(" \b");

                var results = result.FindElements(By.XPath("//div[@class='lmt__textarea_base_style']"));
                if (results.Count > 0)
                    result = results[results.Count - 1];

                var words = result.FindElements(By.XPath(".//*"));
                foreach (IWebElement w in words)
                    res += w.GetAttribute("textContent");
            }

            return result;
        }


        public override string ProcessResult(IWebElement result)
        {
            string res = "";

            var words = result.FindElements(By.XPath(".//*"));
            foreach (IWebElement w in words)
                res += w.GetAttribute("textContent");

            int index = -1;
            while ((index = res.IndexOf(" \b")) != -1)
                res = res.Substring(0, index) + res.Substring(index + 2, res.Length - (index + 2));
            return res;
        }
    }
}
