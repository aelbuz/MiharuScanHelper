using Miharu.BackEnd.Helper;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Miharu.BackEnd.Translation.HTTPTranslators
{
    public class HTTPBingTranslator : HTTPTranslator
    {
        private readonly string _URL;

        public HTTPBingTranslator(TesseractSourceLanguage tesseractSourceLanguage, TranslationTargetLanguage translationTargetLanguage)
        {
            _URL = string.Format("https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&from={0}&to={1}",
                                 tesseractSourceLanguage.ToTranslationSourceLanguageParameter(),
                                 translationTargetLanguage.ToTranslationTargetLanguageParameter());
        }

        public override TranslationType Type => TranslationType.Bing_API;

        protected override string GetUri(string text) => _URL;

        protected override string ProcessResponse(string response)
        {
            string result = response;
            string find = "\"text\":";
            if (result.Contains(find))
            {
                result = result.Substring(result.IndexOf(find) + find.Length);
                result = result.Substring(result.IndexOf("\"") + 1);
                result = result.Substring(0, result.IndexOf("\",\""));
                if (result.Contains("\\u"))
                    result = DecodeEncodedUnicodeCharacters(result);
                result = CleanNewLines(result);
            }
            else
            {
                throw new Exception("Bad response format");
            }

            return result;
        }

        public override async Task<string> Translate(string text)
        {
            string result = "";
            object[] body = new object[] { new { Text = text } };
            string requestBody = JsonConvert.SerializeObject(body);

            using (HttpClient client = new HttpClient())
            using (HttpRequestMessage request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(GetUri(text));
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", _A);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    result = await response.Content.ReadAsStringAsync();
                    result = ProcessResponse(result);
                }
                else
                {
                    throw new Exception("HTTP bad response (" + response.StatusCode.ToString() + ")");
                }
            }

            return result;
        }
    }
}
