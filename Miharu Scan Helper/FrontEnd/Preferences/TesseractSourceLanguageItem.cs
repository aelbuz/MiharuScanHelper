using Miharu.BackEnd.Translation;

namespace Miharu.FrontEnd.Preferences
{
    public struct TesseractSourceLanguageItem
    {
        public TesseractSourceLanguageItem(TesseractSourceLanguage language, string label)
        {
            Language = language;
            Label = label;
        }

        public TesseractSourceLanguage Language { get; private set; }

        public string Label { get; private set; }
    }
}
