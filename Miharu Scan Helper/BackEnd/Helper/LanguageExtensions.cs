using Miharu.BackEnd.Translation;
using System;

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
                    throw new ArgumentException("Given Tesseract language is not supported.");
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
                    throw new ArgumentException("Given translation language is not supported.");
            }
        }

        public static string ToTranslationTargetLanguageParameter(this TranslationTargetLanguage translationTargetLanguage)
        {
            switch (translationTargetLanguage)
            {
                case TranslationTargetLanguage.English:
                    return "en";
                case TranslationTargetLanguage.Turkish:
                    return "tr";
                default:
                    throw new ArgumentException("Given translation language is not supported.");
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
                case TranslationType.DeepL_Web: // Korean and Turkish not supported.
                    {
                        return from == TesseractSourceLanguage.Japanese && to == TranslationTargetLanguage.English;
                    }
                case TranslationType.Papago_Web: // Turkish not supported.
                    {
                        return (from == TesseractSourceLanguage.Japanese || from == TesseractSourceLanguage.Korean) && to == TranslationTargetLanguage.English;
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
