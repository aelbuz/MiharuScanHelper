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
                case TesseractSourceLanguage.Afrikaans:
                    return "afr";
                case TesseractSourceLanguage.Albanian:
                    return "sqi";
                case TesseractSourceLanguage.Amharic:
                    return "amh";
                case TesseractSourceLanguage.Arabic:
                    return "ara";
                case TesseractSourceLanguage.Assamese:
                    return "asm";
                case TesseractSourceLanguage.Azerbaijani:
                    return "aze";
                case TesseractSourceLanguage.Azerbaijani_Cyrillic:
                    return "aze_cyrl";
                case TesseractSourceLanguage.Basque:
                    return "eus";
                case TesseractSourceLanguage.Belarusian:
                    return "bel";
                case TesseractSourceLanguage.Bengali:
                    return "ben";
                case TesseractSourceLanguage.Bosnian:
                    return "bos";
                case TesseractSourceLanguage.Bulgarian:
                    return "bul";
                case TesseractSourceLanguage.Burmese:
                    return "mya";
                case TesseractSourceLanguage.Catalan:
                    return "cat";
                case TesseractSourceLanguage.Cebuano:
                    return "ceb";
                case TesseractSourceLanguage.Central_Khmer:
                    return "khm";
                case TesseractSourceLanguage.Cherokee:
                    return "chr";
                case TesseractSourceLanguage.Chinese_Simplified:
                    return "chi_sim";
                case TesseractSourceLanguage.Chinese_Traditional:
                    return "chi_tra";
                case TesseractSourceLanguage.Croatian:
                    return "hrv";
                case TesseractSourceLanguage.Czech:
                    return "ces";
                case TesseractSourceLanguage.Danish:
                    return "dan";
                case TesseractSourceLanguage.Dutch:
                    return "nld";
                case TesseractSourceLanguage.Dzongkha:
                    return "dzo";
                case TesseractSourceLanguage.English:
                    return "eng";
                case TesseractSourceLanguage.English_Middle:
                    return "enm";
                case TesseractSourceLanguage.Esperanto:
                    return "epo";
                case TesseractSourceLanguage.Estonian:
                    return "est";
                case TesseractSourceLanguage.Finnish:
                    return "fin";
                case TesseractSourceLanguage.French:
                    return "fra";
                case TesseractSourceLanguage.French_Middle:
                    return "frm";
                case TesseractSourceLanguage.Galician:
                    return "glg";
                case TesseractSourceLanguage.Georgian:
                    return "kat";
                case TesseractSourceLanguage.Georgian_Old:
                    return "kat_old";
                case TesseractSourceLanguage.German:
                    return "deu";
                case TesseractSourceLanguage.German_Fraktur:
                    return "frk";
                case TesseractSourceLanguage.Greek_Ancient:
                    return "grc";
                case TesseractSourceLanguage.Greek_Modern:
                    return "ell";
                case TesseractSourceLanguage.Gujarati:
                    return "guj";
                case TesseractSourceLanguage.Haitian:
                    return "hat";
                case TesseractSourceLanguage.Hebrew:
                    return "heb";
                case TesseractSourceLanguage.Hindi:
                    return "hin";
                case TesseractSourceLanguage.Hungarian:
                    return "hun";
                case TesseractSourceLanguage.Icelandic:
                    return "isl";
                case TesseractSourceLanguage.Indonesian:
                    return "ind";
                case TesseractSourceLanguage.Inuktitut:
                    return "iku";
                case TesseractSourceLanguage.Irish:
                    return "gle";
                case TesseractSourceLanguage.Italian:
                    return "ita";
                case TesseractSourceLanguage.Italian_Old:
                    return "ita_old";
                case TesseractSourceLanguage.Japanese:
                    return isVertical ? "jpn_vert" : "jpn";
                case TesseractSourceLanguage.Javanese:
                    return "jav";
                case TesseractSourceLanguage.Kannada:
                    return "kan";
                case TesseractSourceLanguage.Kazakh:
                    return "kaz";
                case TesseractSourceLanguage.Kirghiz:
                    return "kir";
                case TesseractSourceLanguage.Korean:
                    return isVertical ? "kor_vert" : "kor";
                case TesseractSourceLanguage.Lao:
                    return "lao";
                case TesseractSourceLanguage.Latin:
                    return "lat";
                case TesseractSourceLanguage.Latvian:
                    return "lav";
                case TesseractSourceLanguage.Lithuanian:
                    return "lit";
                case TesseractSourceLanguage.Macedonian:
                    return "mkd";
                case TesseractSourceLanguage.Malay:
                    return "msa";
                case TesseractSourceLanguage.Malayalam:
                    return "mal";
                case TesseractSourceLanguage.Maltese:
                    return "mlt";
                case TesseractSourceLanguage.Marathi:
                    return "mar";
                case TesseractSourceLanguage.Nepali:
                    return "nep";
                case TesseractSourceLanguage.Norwegian:
                    return "nor";
                case TesseractSourceLanguage.Oriya:
                    return "ori";
                case TesseractSourceLanguage.Pashto:
                    return "pus";
                case TesseractSourceLanguage.Persian:
                    return "fas";
                case TesseractSourceLanguage.Polish:
                    return "pol";
                case TesseractSourceLanguage.Portuguese:
                    return "por";
                case TesseractSourceLanguage.Punjabi:
                    return "pan";
                case TesseractSourceLanguage.Romanian:
                    return "ron";
                case TesseractSourceLanguage.Russian:
                    return "rus";
                case TesseractSourceLanguage.Sanskrit:
                    return "san";
                case TesseractSourceLanguage.Serbian:
                    return "srp";
                case TesseractSourceLanguage.Serbian_Latin:
                    return "srp_latn";
                case TesseractSourceLanguage.Sinhala:
                    return "sin";
                case TesseractSourceLanguage.Slovak:
                    return "slk";
                case TesseractSourceLanguage.Slovenian:
                    return "slv";
                case TesseractSourceLanguage.Spanish_Castilian:
                    return "spa";
                case TesseractSourceLanguage.Spanish_Castilian_Old:
                    return "spa_old";
                case TesseractSourceLanguage.Swahili:
                    return "swa";
                case TesseractSourceLanguage.Swedish:
                    return "swe";
                case TesseractSourceLanguage.Syriac:
                    return "syr";
                case TesseractSourceLanguage.Tagalog:
                    return "tgl";
                case TesseractSourceLanguage.Tajik:
                    return "tgk";
                case TesseractSourceLanguage.Tamil:
                    return "tam";
                case TesseractSourceLanguage.Telugu:
                    return "tel";
                case TesseractSourceLanguage.Thai:
                    return "tha";
                case TesseractSourceLanguage.Tibetan:
                    return "bod";
                case TesseractSourceLanguage.Tigrinya:
                    return "tir";
                case TesseractSourceLanguage.Turkish:
                    return "tur";
                case TesseractSourceLanguage.Ukrainian:
                    return "ukr";
                case TesseractSourceLanguage.Urdu:
                    return "urd";
                case TesseractSourceLanguage.Uyghur:
                    return "uig";
                case TesseractSourceLanguage.Uzbek:
                    return "uzb";
                case TesseractSourceLanguage.Uzbek_Cyrillic:
                    return "uzb_cyrl";
                case TesseractSourceLanguage.Vietnamese:
                    return "vie";
                case TesseractSourceLanguage.Welsh:
                    return "cym";
                case TesseractSourceLanguage.Yiddish:
                    return "yid";
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
