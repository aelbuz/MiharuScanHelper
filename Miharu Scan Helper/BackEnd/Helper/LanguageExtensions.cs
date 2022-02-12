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
                default:
                    throw new ArgumentException("Given translation language is not supported.");
            }
        }
    }
}
