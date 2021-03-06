using ControlzEx.Theming;
using Miharu.BackEnd;
using Miharu.BackEnd.Helper;
using Miharu.BackEnd.Translation;
using Miharu.BackEnd.Translation.Threading;
using Miharu.Control;
using Miharu.FrontEnd;
using Miharu.FrontEnd.Page;
using Miharu.FrontEnd.TextEntry;
using Miharu.Properties;
using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Windows;

namespace Miharu
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Main
        #region Helper Methods
        private static bool ExistsInPath(string fileName)
        {
            bool res = false;

            string path = Environment.GetEnvironmentVariable("PATH");
            string[] values = path.Split(Path.PathSeparator);
            for (int i = 0; i < values.Length && !res; i++)
            {
                string fullPath = Path.Combine(values[i], fileName);
                res = File.Exists(fullPath);
            }

            return res;
        }

        private static bool CheckForTesseract()
        {
            if (!File.Exists(Settings.Default.TesseractPath))
            {
                if (ExistsInPath("tesseract.exe"))
                {
                    Settings.Default.TesseractPath = "tesseract.exe";
                    Settings.Default.Save();
                    return true;
                }

                TaskDialog dialog = new TaskDialog
                {
                    WindowTitle = "Warning Tesseract Not Found",
                    MainIcon = TaskDialogIcon.Warning,
                    MainInstruction = "Tesseract executable could not be located.",
                    Content = @"Miharu requires Tesseract to function. Would you like to locate the Tesseract exectutable manually?"
                };

                TaskDialogButton okButton = new TaskDialogButton(ButtonType.Yes);
                dialog.Buttons.Add(okButton);
                TaskDialogButton cancelButton = new TaskDialogButton(ButtonType.No);
                dialog.Buttons.Add(cancelButton);
                TaskDialogButton button = dialog.ShowDialog();
                if (button.ButtonType == ButtonType.No)
                {
                    return false;
                }

                VistaOpenFileDialog fileDialog = new VistaOpenFileDialog
                {
                    AddExtension = true,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    DefaultExt = ".exe",
                    Filter = "Tesseract (tesseract.exe)|tesseract.exe",
                    Multiselect = false,
                    Title = "Select Tesseract Executable"
                };

                bool? res = fileDialog.ShowDialog();
                if (res ?? false)
                {
                    Settings.Default.TesseractPath = fileDialog.FileName;
                    Settings.Default.Save();
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private static string CheckCrash()
        {
            string res = null;
            if (CrashHandler.LastSessionCrashed)
            {
                TaskDialog dialog = new TaskDialog
                {
                    WindowTitle = "Warning",
                    MainIcon = TaskDialogIcon.Warning,
                    MainInstruction = "It seems like a file can be recovered from last session.",
                    Content = "Would you like to attempt to recover the file?"
                };

                TaskDialogButton saveButton = new TaskDialogButton(ButtonType.Yes)
                {
                    Text = "Yes"
                };
                dialog.Buttons.Add(saveButton);

                TaskDialogButton noSaveButton = new TaskDialogButton(ButtonType.No)
                {
                    Text = "No"
                };
                dialog.Buttons.Add(noSaveButton);

                TaskDialogButton button = dialog.ShowDialog();
                string temp = CrashHandler.RecoverLastSessionFile();

                if (button.ButtonType == ButtonType.Yes)
                {
                    res = temp;
                }
            }

            return res;
        }

        #endregion

        [STAThread]
        public static void Main(string[] args)
        {
            ChapterManager chapterManager = null;
            KanjiInputManager kanjiInputManager = null;
            TranslatorThread translatorThread = null;
            MiharuMainWindow mainWindow = null;

            try
            {
                App application = new App();

                /*MainWindow mw = new MainWindow();
				application.Run(mw);*/

                //Initialize stuff
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory.ToString());
                if (CheckForTesseract() &&
                    Enum.TryParse(Settings.Default.TesseractSourceLanguage, out TesseractSourceLanguage tesseractSourceLanguage) &&
                    Enum.TryParse(Settings.Default.TranslationTargetLanguage, out TranslationTargetLanguage translationTargetLanguage))
                {
                    translatorThread = TranslatorThread.StartThread(tesseractSourceLanguage, translationTargetLanguage);

                    string startChapter = null;
                    startChapter = CheckCrash();
                    if (startChapter == null && args.Length > 0 && File.Exists(args[0]))
                    {
                        startChapter = args[0];
                    }

                    /*Logger.Log("Start Chapter: " + startChapter);
					Logger.Log("Args Length: " + args.Length);
					for (int i = 0; i < args.Length; i++)
						Logger.Log("\t" + args[i]);*/

                    kanjiInputManager = new KanjiInputManager();

                    chapterManager = new ChapterManager(kanjiInputManager, translatorThread);

                    var availableTesseractLanguages = TesseractHelper.GetAvailableTesseractLanguages();

                    mainWindow = new MiharuMainWindow(availableTesseractLanguages, chapterManager, startChapter);

                    PageControl pageControl = new PageControl(chapterManager.PageManager);
                    mainWindow.PageControlArea.Child = pageControl;

                    TextEntryView textEntryView = new TextEntryView(chapterManager.PageManager.TextEntryManager, kanjiInputManager, tesseractSourceLanguage);
                    mainWindow.TextEntryArea.Child = textEntryView;

                    application.Run(mainWindow);
                }
            }
            catch (Exception e)
            {
                CrashHandler.HandleCrash(chapterManager, e);
                FileInfo crashFileInfo = new FileInfo(Logger.CurrentCrashLog);

                TaskDialog dialog = new TaskDialog
                {
                    WindowTitle = "Fatal Error",
                    MainIcon = TaskDialogIcon.Error,
                    MainInstruction = "There was a fatal error. Details can be found in the generated crash log:",
                    Content = crashFileInfo.FullName
                };

                TaskDialogButton okButton = new TaskDialogButton("Ok")
                {
                    ButtonType = ButtonType.Ok
                };
                dialog.Buttons.Add(okButton);

                TaskDialogButton button = dialog.ShowDialog();
            }
            finally
            {
                mainWindow?.Close();
                translatorThread?.FinalizeThread();
            }
        }

        #endregion

        public App()
        {
            InitializeComponent();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            ThemeManager.Current.ChangeTheme(Current, Settings.Default.Theme + "." + Settings.Default.Accent);

            base.OnStartup(e);
        }

        /*public void ChangeSkin(string newSkin) {
			Uri skinDictUri;
			if (newSkin != null && Uri.TryCreate(newSkin,UriKind.Absolute, out skinDictUri)) {
				ResourceDictionary skinDict = (ResourceDictionary)LoadComponent(skinDictUri);
				Collection<ResourceDictionary> mergedDicts = base.Resources.MergedDictionaries;
				mergedDicts.Add(skinDict);
			}
						
		}*/
    }
}
