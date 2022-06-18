using Miharu.BackEnd.Translation;
using Miharu.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Miharu.BackEnd.Helper
{
    public static class TesseractHelper
    {
        //private const string TrainDataFolder = "tessdata";
        private const string TesseractTrainDataPath = @"Resources\Redist\Tesseract-OCR\tessdata";

        public static IEnumerable<TesseractSourceLanguage> GetAvailableTesseractLanguages()
        {
            var availableTesseractLanguages = new List<TesseractSourceLanguage>();

            string trainDataPath = Path.Combine(Directory.GetCurrentDirectory(), TesseractTrainDataPath);
            var trainDataFolderFiles = Directory.GetFiles(trainDataPath, $"*.{LanguageExtensions.TesseractTrainDataExtension}", SearchOption.TopDirectoryOnly);
            var trainDataFileNames = trainDataFolderFiles.Select(f => Path.GetFileNameWithoutExtension(f));

            var languages = Enum.GetValues(typeof(TesseractSourceLanguage))
                                .Cast<TesseractSourceLanguage>()
                                .Except(LanguageExtensions.TesseractMultipleTrainDataLanguages);

            foreach (var language in languages)
            {
                if (trainDataFileNames.Contains(language.ToTesseractTrainDataName()))
                {
                    availableTesseractLanguages.Add(language);
                }
            }

            foreach (var language in LanguageExtensions.TesseractMultipleTrainDataLanguages)
            {
                if (trainDataFileNames.Contains(language.ToTesseractTrainDataName()) && trainDataFileNames.Contains(language.ToTesseractTrainDataName(true)))
                {
                    availableTesseractLanguages.Add(language);
                }
            }

            return availableTesseractLanguages;
        }
    }
}
