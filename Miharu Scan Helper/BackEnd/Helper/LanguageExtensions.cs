using Miharu.BackEnd.Translation;
using System.ComponentModel;

namespace Miharu.BackEnd.Helper
{
    public static class LanguageExtensions
    {
        public static string ToTesseractTestDataName(this TesseractSourceLanguage tesseractSourceLanguage, bool isVertical = false)
        {
            switch (tesseractSourceLanguage)
            {
                case TesseractSourceLanguage.Japanese:
                    {
                        return isVertical ? "jpn_vert" : "jpn";
                    }
                case TesseractSourceLanguage.Korean:
                    {
                        return isVertical ? "kor_vert" : "kor";
                    }
                default:
                    throw new InvalidEnumArgumentException("Given Tesseract language is not supported.", (int)tesseractSourceLanguage, typeof(TesseractSourceLanguage));
            }
        }

        public static string ToTranslationSourceLanguageParameter(this TesseractSourceLanguage translationSourceLanguage)
        {
            switch (translationSourceLanguage)
            {
                case TesseractSourceLanguage.Japanese:
                    return "ja";
                case TesseractSourceLanguage.Korean:
                    return "ko";
                default:
                    throw new InvalidEnumArgumentException("Given translation language is not supported.", (int)translationSourceLanguage, typeof(TesseractSourceLanguage));
            }
        }

        public static string ToTranslationTargetLanguageParameter(this TranslationTargetLanguage translationTargetLanguage)
        {
            switch (translationTargetLanguage)
            {
                case TranslationTargetLanguage.English:
                    return "en";
                case TranslationTargetLanguage.Bulgarian:
                    return "bg";
                case TranslationTargetLanguage.Czech:
                    return "cs";
                case TranslationTargetLanguage.Danish:
                    return "da";
                case TranslationTargetLanguage.Dutch:
                    return "nl";
                case TranslationTargetLanguage.Estonian:
                    return "et";
                case TranslationTargetLanguage.Finnish:
                    return "fi";
                case TranslationTargetLanguage.French:
                    return "fr";
                case TranslationTargetLanguage.German:
                    return "de";
                case TranslationTargetLanguage.Greek:
                    return "el";
                case TranslationTargetLanguage.Hungarian:
                    return "hu";
                case TranslationTargetLanguage.Italian:
                    return "it";
                case TranslationTargetLanguage.Japanese:
                    return "ja";
                case TranslationTargetLanguage.Latvian:
                    return "lv";
                case TranslationTargetLanguage.Lithuanian:
                    return "lt";
                case TranslationTargetLanguage.Polish:
                    return "pl";
                case TranslationTargetLanguage.Portuguese:
                    return "pt";
                case TranslationTargetLanguage.Romanian:
                    return "ro";
                case TranslationTargetLanguage.Russian:
                    return "ru";
                case TranslationTargetLanguage.SimplifiedChinese:
                    return "zh-CN";
                case TranslationTargetLanguage.Slovak:
                    return "sk";
                case TranslationTargetLanguage.Slovenian:
                    return "sl";
                case TranslationTargetLanguage.Spanish:
                    return "es";
                case TranslationTargetLanguage.Swedish:
                    return "sv";
                case TranslationTargetLanguage.Turkish:
                    return "tr";
                default:
                    throw new InvalidEnumArgumentException("Given translation language is not supported.", (int)translationTargetLanguage, typeof(TranslationTargetLanguage));
            }
        }

        public static bool SupportsTranslation(this TranslationType translationType, TesseractSourceLanguage from, TranslationTargetLanguage to)
        {
            // Assumed that Google, Bing and Yandex supports all languages.

            switch (translationType)
            {
                case TranslationType.Google_Web:
                case TranslationType.Google_API:
                    {
                        return true;
                    }
                case TranslationType.Bing_Web:
                case TranslationType.Bing_API:
                    {
                        return true;
                    }
                case TranslationType.Yandex_API:
                case TranslationType.Yandex_Web:
                    {
                        return true;
                    }
                case TranslationType.DeepL_Web:
                    {
                        return from.SupportedByDeepL() && to.SupportedByDeepL();
                    }
                case TranslationType.Papago_Web:
                    {
                        return from.SupportedByPapago() && to.SupportedByPapago();
                    }
                case TranslationType.Jaded_Network: // Japanese only (SFX dictionary). Returns results in English.
                case TranslationType.Jisho: // Japanese only. Returns results in English.
                    {
                        return from == TesseractSourceLanguage.Japanese && to == TranslationTargetLanguage.English;
                    }
                case TranslationType.Web:
                case TranslationType.SFX:
                case TranslationType.Dict:
                default:
                    {
                        return false;
                    }
            }
        }
    }
}
