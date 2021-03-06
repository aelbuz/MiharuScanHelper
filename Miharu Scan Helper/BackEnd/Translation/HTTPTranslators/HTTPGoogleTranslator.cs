using Miharu.BackEnd.Helper;
using System;

namespace Miharu.BackEnd.Translation.HTTPTranslators
{
    public class HTTPGoogleTranslator : HTTPTranslator
    {
        private readonly string _URL;
        public override TranslationType Type => TranslationType.Google_API;

        public HTTPGoogleTranslator(TesseractSourceLanguage tesseractSourceLanguage, TranslationTargetLanguage translationTargetLanguage)
        {
            _URL = string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q=",
                                 tesseractSourceLanguage.ToTranslationSourceLanguageParameter(),
                                 translationTargetLanguage.ToTranslationTargetLanguageParameter());
        }

        protected override string GetUri(string text)
        {
            string refinedText = text.Replace(Environment.NewLine, " ");
            refinedText = refinedText.Replace("\n", " ");

            return _URL + Uri.EscapeDataString(refinedText);
        }

        protected override string ProcessResponse(string res)
        {
            int firstString = res.IndexOf("\"") + 1;
            if (res.IndexOf("null") <= firstString)
                throw new Exception("Google API translation failed");
            else
            {
                res = res.Substring(firstString);
                res = res.Substring(0, res.IndexOf("\",\""));
                if (res.Contains("\\u"))
                    res = DecodeEncodedUnicodeCharacters(res);
                res = CleanNewLines(res);
            }

            return res;
        }
    }
}
