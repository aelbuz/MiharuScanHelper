using Miharu.BackEnd.Data;
using Miharu.BackEnd.Translation;
using Miharu.Control;
using Miharu.FrontEnd.Input;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Miharu.FrontEnd.TextEntry
{
    /// <summary>
    /// Interaction logic for TextEntryView.xaml
    /// </summary>
    public partial class TextEntryView : UserControl
    {
        private TextEntryManager _textEntryManager;
        private PageManager _pageManager;
        private KanjiInputManager _kanjiInputManager;
        private KanjiByRadInputControl _kanjiByRadInputControl;
        private readonly TesseractSourceLanguage tesseractSourceLanguage;

        private void ConfigureButtons()
        {
            if (!_pageManager.IsPageLoaded)
            {
                PrevEntryButton.IsEnabled = false;
                NextEntryButton.IsEnabled = false;
            }
            else
            {
                PrevEntryButton.IsEnabled = _textEntryManager.CurrentTextIndex > 0;
                NextEntryButton.IsEnabled = _textEntryManager.CurrentTextIndex + 1 < _pageManager.CurrentPageTextEntries.Count;
            }
        }

        public TextEntryView(TextEntryManager textEntryManager, KanjiInputManager kanjiInputManager, TesseractSourceLanguage tesseractSourceLanguage)
        {
            InitializeComponent();

            this.tesseractSourceLanguage = tesseractSourceLanguage;

            _textEntryManager = textEntryManager;
            _textEntryManager.TextChanged += OnTextEntryChanged;
            _textEntryManager.TextIndexChanged += OnTextEntryIndexChanged;
            _pageManager = _textEntryManager.PageManager;
            _pageManager.PageChanged += OnPageChanged;
            _pageManager.TextEntryMoved += OnTextEntryMoved;
            _pageManager.TextEntryRemoved += OnTextEntryRemoved;
            _pageManager.TextEntryAdded += OnTextEntryAdded;
            _kanjiInputManager = kanjiInputManager;
            _kanjiByRadInputControl = new KanjiByRadInputControl(_kanjiInputManager);

            ConfigureButtons();
        }

        private void OnTextEntryAdded(object sender, ListModificationEventArgs e)
        {
            TextEntriesStackPanel.Children.Insert(e.EventNewIndex, new TextEntryListView((Text)e.EventObject, _pageManager));
            ConfigureButtons();
        }

        private void OnTextEntryRemoved(object sender, ListModificationEventArgs e)
        {
            TextEntriesStackPanel.Children.RemoveAt(e.EventOldIndex);
            ConfigureButtons();
        }

        private void OnTextEntryMoved(object sender, ListModificationEventArgs e)
        {
            var tmp2 = TextEntriesStackPanel.Children[e.EventOldIndex];
            TextEntriesStackPanel.Children.RemoveAt(e.EventOldIndex);
            TextEntriesStackPanel.Children.Insert(e.EventNewIndex, tmp2);
            ConfigureButtons();
        }

        private void OnPageChanged(object sender, EventArgs e)
        {
            if (_pageManager.IsPageLoaded)
            {
                if (!_pageManager.IsPageReady)
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _pageManager.WaitForPage();
                    Mouse.SetCursor(Cursors.Arrow);
                }

                TextEntriesStackPanel.Children.Clear();
                for (int i = 0; i < _pageManager.CurrentPageTextEntries.Count; i++)
                {
                    TextEntriesStackPanel.Children.Add(
                        new TextEntryListView(
                            _pageManager.CurrentPageTextEntries[i],
                            _pageManager));
                }
                if (_previousIndex >= 0 && _previousIndex < TextEntriesStackPanel.Children.Count)
                    ((TextEntryListView)TextEntriesStackPanel.Children[_previousIndex]).Selected = false;
                if (_textEntryManager.CurrentTextIndex >= 0 && _textEntryManager.CurrentTextIndex < TextEntriesStackPanel.Children.Count)
                    ((TextEntryListView)TextEntriesStackPanel.Children[_textEntryManager.CurrentTextIndex]).Selected = true;
                _previousIndex = _textEntryManager.CurrentTextIndex;
            }
            else
                TextEntriesStackPanel.Children.Clear();

            TextEntriesStackPanel.InvalidateVisual();
            ConfigureButtons();
        }

        private void OnTextEntryChanged(object sender, EventArgs e)
        {
            if (TextEntryArea.Content != null && TextEntryArea.Content is TextEntryControl tec)
                tec.RemoveHandlers();

            if (_textEntryManager.IsTextSelected)
                TextEntryArea.Content = new TextEntryControl(_textEntryManager, _kanjiInputManager, _kanjiByRadInputControl, tesseractSourceLanguage);
            else
                TextEntryArea.Content = null;
            ConfigureButtons();
        }

        private int _previousIndex = -1;

        private void OnTextEntryIndexChanged(object sender, EventArgs e)
        {
            if (_previousIndex >= 0 && _previousIndex < TextEntriesStackPanel.Children.Count)
                ((TextEntryListView)TextEntriesStackPanel.Children[_previousIndex]).Selected = false;
            if (_textEntryManager.CurrentTextIndex >= 0 && _textEntryManager.CurrentTextIndex < TextEntriesStackPanel.Children.Count)
                ((TextEntryListView)TextEntriesStackPanel.Children[_textEntryManager.CurrentTextIndex]).Selected = true;
            _previousIndex = _textEntryManager.CurrentTextIndex;
            ConfigureButtons();
        }

        private void PrevEntryButton_Click(object sender, EventArgs e)
        {
            _pageManager.SelectTextEntry(_textEntryManager.CurrentTextIndex - 1);
        }

        private void NextEntryButton_Click(object sender, EventArgs e)
        {
            _pageManager.SelectTextEntry(_textEntryManager.CurrentTextIndex + 1);
        }
    }
}
