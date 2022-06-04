using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Miharu.BackEnd.Translation;
using Miharu.Control;
using Miharu.Properties;
using Ookii.Dialogs.Wpf;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Miharu.FrontEnd
{
    /// <summary>
    /// Interaction logic for PreferencesDialog.xaml
    /// </summary>
    public partial class PreferencesDialog : MetroWindow
    {
        private readonly string[] BaseColors = { "Dark", "Light" };
        private readonly string[] AccentColors = { "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna" };

        private readonly ChapterManager _chapterManager;

        private bool _originalUseScreenDPI = false;
        private bool requiresRestart = false;

        private string _tesseractPath;
        public string TesseractPath
        {
            get => _tesseractPath;
            private set
            {
                _tesseractPath = value;
                ApplyButton.IsEnabled = true;
            }
        }

        public string ThemeBaseColor
        {
            get;
            private set;
        } = null;

        public string ThemeAccentColor
        {
            get;
            private set;
        } = null;

        public PreferencesDialog(TranslationManager translationManager, ChapterManager chapterManager)
        {
            InitializeComponent();

            _chapterManager = chapterManager;

            TesseractPathTextBox.Text = Settings.Default.TesseractPath;
            ApplyButton.IsEnabled = false;

            TesseractSourceLanguageListBox.ItemsSource = Enum.GetValues(typeof(TesseractSourceLanguage));
            TesseractSourceLanguageListBox.SelectedItem = Enum.Parse(typeof(TesseractSourceLanguage), Settings.Default.TesseractSourceLanguage.ToString());
            TesseractSourceLanguageListBox.SelectionChanged += CheckChanged;
            TranslationTargetLanguageListBox.ItemsSource = Enum.GetValues(typeof(TranslationTargetLanguage));
            TranslationTargetLanguageListBox.SelectedItem = Enum.Parse(typeof(TranslationTargetLanguage), Settings.Default.TranslationTargetLanguage.ToString());
            TranslationTargetLanguageListBox.SelectionChanged += CheckChanged;

            ThemeBaseColorListBox.ItemsSource = BaseColors;
            ThemeBaseColorListBox.SelectedItem = Settings.Default.Theme;
            ThemeAccentColorListBox.ItemsSource = AccentColors;
            ThemeAccentColorListBox.SelectedItem = Settings.Default.Accent;

            _originalUseScreenDPI = Settings.Default.UseScreenDPI;
            UseScreenDPIToggleSwitch.IsOn = _originalUseScreenDPI;

            WarnTextDeletionToggleSwitch.IsOn = Settings.Default.WarnTextDeletion;

            AutoTranslateToggleSwitch.IsOn = Settings.Default.AutoTranslateEnabled;


            string disabledTypes = Settings.Default.DisabledTranslationSources;
            foreach (TranslationType t in translationManager.AvailableTranslations)
            {
                if (t.HasFlag(TranslationType.Web))
                {
                    ToggleSwitch ts = new ToggleSwitch
                    {
                        Content = t,
                        IsOn = !disabledTypes.Contains(t.ToString()),
                        IsEnabled = AutoTranslateToggleSwitch.IsOn
                    };
                    ts.Toggled += CheckChanged;
                    TranslationSourcesStackPanel.Children.Add(ts);
                }
            }

            ApplyButton.IsEnabled = false;
            AutoTranslateToggleSwitch.Toggled += OnAutoTranslateChackChange;
        }

        private void OnAutoTranslateChackChange(object sender, EventArgs e)
        {
            if (IsLoaded)
            {
                foreach (ToggleSwitch ts in TranslationSourcesStackPanel.Children)
                {
                    ts.IsEnabled = AutoTranslateToggleSwitch.IsOn;
                }
            }
        }

        private void TesseractPathButton_Click(object sender, RoutedEventArgs e)
        {
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

            bool? res = fileDialog.ShowDialog(this);
            if (res ?? false)
            {
                TesseractPath = fileDialog.FileName;
            }
        }

        private void TesseractPathTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TesseractPath = TesseractPathTextBox.Text;
        }

        public void WarnBadPath(string reason)
        {
            using (TaskDialog dialog = new TaskDialog())
            {
                dialog.WindowTitle = "Error";
                dialog.MainIcon = TaskDialogIcon.Error;
                dialog.MainInstruction = "Can't apply changes.";
                dialog.Content = reason;
                TaskDialogButton okButton = new TaskDialogButton(ButtonType.Ok);
                dialog.Buttons.Add(okButton);
                TaskDialogButton button = dialog.ShowDialog(this);
            }
        }

        private string _failReason;
        private bool CheckTesseractPath()
        {
            bool res;
            if (res = File.Exists(TesseractPath))
            {
                FileInfo fi = new FileInfo(TesseractPath);
                if (!(res = fi.Extension == ".exe"))
                {
                    _failReason = "File must be an executable (Extension .exe)";
                }
            }
            else
            {
                _failReason = "Could not find file at " + TesseractPath;
            }

            return res;
        }

        private void SavePreferences()
        {
            Settings.Default.TesseractPath = TesseractPath;

            Settings.Default.TesseractSourceLanguage = TesseractSourceLanguageListBox.SelectedItem.ToString();
            Settings.Default.TranslationTargetLanguage = TranslationTargetLanguageListBox.SelectedItem.ToString();

            Settings.Default.Theme = (string)ThemeBaseColorListBox.SelectedValue;
            Settings.Default.Accent = (string)ThemeAccentColorListBox.SelectedValue;

            Settings.Default.UseScreenDPI = UseScreenDPIToggleSwitch.IsOn;

            Settings.Default.WarnTextDeletion = WarnTextDeletionToggleSwitch.IsOn;

            Settings.Default.AutoTranslateEnabled = AutoTranslateToggleSwitch.IsOn;


            string disabledSources = "";
            foreach (ToggleSwitch ts in TranslationSourcesStackPanel.Children)
            {
                if (!ts.IsOn)
                {
                    disabledSources += ts.Content.ToString() + ";";
                }
            }
            if (disabledSources.Length > 0)
            {
                disabledSources = disabledSources.Substring(0, disabledSources.Length - 1);
            }

            Settings.Default.DisabledTranslationSources = disabledSources;
            Settings.Default.Save();

            if (_chapterManager.IsChapterLoaded && _originalUseScreenDPI != UseScreenDPIToggleSwitch.IsOn)
            {
                _originalUseScreenDPI = UseScreenDPIToggleSwitch.IsOn;
                BackEnd.Data.Page.UseScreenDPI = _originalUseScreenDPI;
                _chapterManager.ReloadPages();
                _chapterManager.PageManager.ChangePage(_chapterManager.PageManager.CurrentPageIndex);
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTesseractPath())
            {
                SavePreferences();
                ThemeManager.Current.ChangeTheme(Application.Current, (string)ThemeBaseColorListBox.SelectedValue + "." + (string)ThemeAccentColorListBox.SelectedValue);
                Close();

                if (requiresRestart)
                {
                    RequestRestart();
                }
            }
            else
            {
                WarnBadPath(_failReason);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTesseractPath())
            {
                SavePreferences();
                ThemeManager.Current.ChangeTheme(Application.Current, (string)ThemeBaseColorListBox.SelectedValue + "." + (string)ThemeAccentColorListBox.SelectedValue);
                ApplyButton.IsEnabled = false;

                if (requiresRestart)
                {
                    RequestRestart();
                }
            }
            else
            {
                WarnBadPath(_failReason);
            }
        }

        private void ThemeSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                ThemeManager.Current.ChangeTheme(this, (string)ThemeBaseColorListBox.SelectedValue + "." + (string)ThemeAccentColorListBox.SelectedValue);
                ApplyButton.IsEnabled = true;
            }
        }

        private void CheckChanged(object sender, EventArgs e)
        {
            if (IsLoaded)
            {
                ApplyButton.IsEnabled = true;
            }

            if (sender == TesseractSourceLanguageListBox || sender == TranslationTargetLanguageListBox)
            {
                requiresRestart = true;
            }
        }

        private void RequestRestart()
        {
            Dispatcher.Invoke(() =>
            {
                using (TaskDialog dialog = new TaskDialog())
                {
                    dialog.WindowTitle = "Restart Required";
                    dialog.MainIcon = TaskDialogIcon.Warning;
                    dialog.MainInstruction = "Application restart required.";
                    dialog.Content = "Restart required in order to update Tesseract OCR source language and translation settings. Do you want to restart the application right now?";
                    dialog.Buttons.Add(new TaskDialogButton(ButtonType.Yes));
                    dialog.Buttons.Add(new TaskDialogButton(ButtonType.No));
                    var responseButton = dialog.ShowDialog(this);

                    if (responseButton.ButtonType is ButtonType.Yes)
                    {
                        Process.Start(Application.ResourceAssembly.Location);
                        Application.Current.Shutdown();
                    }
                }
            });
        }
    }
}
