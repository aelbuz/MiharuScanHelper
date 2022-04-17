using Miharu.BackEnd.Translation;
using System.Collections.Generic;

namespace Miharu.BackEnd.Helper
{
    internal static class TranslatorHelper
    {
        private static readonly HashSet<TesseractSourceLanguage> supportedDeepLSourceLanguages = new HashSet<TesseractSourceLanguage>()
        {
            TesseractSourceLanguage.Japanese
        };

        private static readonly HashSet<TranslationTargetLanguage> supportedDeepLTargetLanguages = new HashSet<TranslationTargetLanguage>()
        {
            TranslationTargetLanguage.Bulgarian,
            TranslationTargetLanguage.Czech,
            TranslationTargetLanguage.Danish,
            TranslationTargetLanguage.Dutch,
            TranslationTargetLanguage.English,
            TranslationTargetLanguage.Estonian,
            TranslationTargetLanguage.Finnish,
            TranslationTargetLanguage.French,
            TranslationTargetLanguage.German,
            TranslationTargetLanguage.Greek,
            TranslationTargetLanguage.Hungarian,
            TranslationTargetLanguage.Italian,
            TranslationTargetLanguage.Japanese,
            TranslationTargetLanguage.Latvian,
            TranslationTargetLanguage.Lithuanian,
            TranslationTargetLanguage.Polish,
            TranslationTargetLanguage.Portuguese,
            TranslationTargetLanguage.Romanian,
            TranslationTargetLanguage.Russian,
            TranslationTargetLanguage.SimplifiedChinese,
            TranslationTargetLanguage.Slovak,
            TranslationTargetLanguage.Slovenian,
            TranslationTargetLanguage.Spanish,
            TranslationTargetLanguage.Swedish
        };

        private static readonly HashSet<TesseractSourceLanguage> supportedPapagoSourceLanguages = new HashSet<TesseractSourceLanguage>()
        {
            TesseractSourceLanguage.Japanese,
            TesseractSourceLanguage.Korean
        };

        private static readonly HashSet<TranslationTargetLanguage> supportedPapagoTargetLanguages = new HashSet<TranslationTargetLanguage>()
        {
            TranslationTargetLanguage.English,
            TranslationTargetLanguage.French,
            TranslationTargetLanguage.German,
            TranslationTargetLanguage.Italian,
            TranslationTargetLanguage.Japanese,
            TranslationTargetLanguage.Portuguese,
            TranslationTargetLanguage.Russian,
            TranslationTargetLanguage.SimplifiedChinese,
            TranslationTargetLanguage.Spanish,
        };

        internal static bool SupportedByDeepL(this TesseractSourceLanguage tesseractSourceLanguage)
        {
            return supportedDeepLSourceLanguages.Contains(tesseractSourceLanguage);
        }

        internal static bool SupportedByDeepL(this TranslationTargetLanguage translationTargetLanguage)
        {
            return supportedDeepLTargetLanguages.Contains(translationTargetLanguage);
        }

        internal static bool SupportedByPapago(this TesseractSourceLanguage tesseractSourceLanguage)
        {
            return supportedPapagoSourceLanguages.Contains(tesseractSourceLanguage);
        }

        internal static bool SupportedByPapago(this TranslationTargetLanguage translationTargetLanguage)
        {
            return supportedPapagoTargetLanguages.Contains(translationTargetLanguage);
        }
    }
}
