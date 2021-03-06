using Miharu.BackEnd;
using Miharu.BackEnd.Data;
using Miharu.BackEnd.Translation.Threading;
using Miharu.Properties;
using System;
using System.Collections.Generic;

namespace Miharu.Control
{
    public class PageManager
    {
        #region Events

        public event EventHandler PageChanged;
        public event EventHandler PageIndexChanged;
        public event EventHandler TextEntryRequiresTranslation;
        public event ListModificationEventHandler TextEntryAdded;
        public event ListModificationEventHandler TextEntryRemoved;
        public event ListModificationEventHandler TextEntryMoved;

        #endregion

        #region Data

        public ChapterManager ChapterManager
        {
            get; private set;
        } = null;

        public TextEntryManager TextEntryManager
        {
            get; private set;
        } = null;

        public KanjiInputManager KanjiInputManager
        {
            get; private set;
        } = null;

        private Page _currentPage = null;
        private Page CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                KanjiInputManager.ClearRads();
                PageChanged?.Invoke(this, new EventArgs());
            }
        }
        public bool IsPageLoaded => _currentPage != null;
        public bool IsPageReady => _currentPage.Ready;
        public string CurrentPagePath => CurrentPage.Path;
        public string CurrentPageName => CurrentPage.Name;

        public List<Text> CurrentPageTextEntries => CurrentPage.TextEntries;



        private int _currentPageIndex = 0;
        public int CurrentPageIndex
        {
            get => _currentPageIndex;
            set
            {
                _currentPageIndex = value;
                PageIndexChanged?.Invoke(this, new EventArgs());
            }
        }

        #endregion

        public PageManager(ChapterManager chapterManager, KanjiInputManager kanjiInputManager, TranslatorThread translatorThread)
        {
            ChapterManager = chapterManager;
            KanjiInputManager = kanjiInputManager;
            Page.UseScreenDPI = Settings.Default.UseScreenDPI;
            TextEntryManager = new TextEntryManager(this, translatorThread);
        }

        public void LoadPage(Page page, int index)
        {
            CurrentPage = page;
            CurrentPageIndex = index;
        }

        public void WaitForPage()
        {
            CurrentPage.PageWaitHandle.WaitOne();
        }

        public void NextPage()
        {
            ChangePage(CurrentPageIndex + 1);
        }

        public void PrevPage()
        {
            ChangePage(CurrentPageIndex - 1);
        }

        public void ChangePage(int index)
        {
            try
            {
                CurrentPage = ChapterManager.LoadedChapter.Pages[index];
                CurrentPageIndex = index;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                throw e;
            }
        }

        public void AddTextEntry(DPIAwareRectangle rect)
        {
            Text text;
            try
            {
                text = CurrentPage.AddTextEntry(rect);
                TextEntryAdded?.Invoke(this, new ListModificationEventArgs(-1, text, CurrentPage.TextEntries.Count - 1));
            }
            catch (Exception e)
            {
                Logger.Log(e);
                throw e;
            }
            TextEntryManager.SelectTextEntry(text, CurrentPage.TextEntries.Count - 1);
            TextEntryRequiresTranslation?.Invoke(this, new EventArgs());
            ChapterManager.IsChapterSaved = false;
        }

        public void SelectTextEntry(int index)
        {
            if (index < 0 || index >= CurrentPage.TextEntries.Count)
            {
                TextEntryManager.SelectTextEntry(null, index);
            }
            else
            {
                TextEntryManager.SelectTextEntry(CurrentPage.TextEntries[index], index);
            }
        }

        public void Unload()
        {
            CurrentPage = null;
            CurrentPageIndex = 0;
            TextEntryManager.Unload();
        }

        public void RemoveTextEntry(Text textEntry)
        {
            try
            {
                int index = CurrentPage.TextEntries.IndexOf(textEntry);
                CurrentPage.RemoveTextEntry(index);
                TextEntryRemoved?.Invoke(this, new ListModificationEventArgs(index, textEntry));
                TextEntryManager.RemovedTextEntry(textEntry);
                ChapterManager.IsChapterSaved = false;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                throw e;
            }
        }

        public void MoveTextEntry(Text textEntry, bool up)
        {
            try
            {
                int offset = up ? -1 : 1;
                int index = CurrentPage.TextEntries.IndexOf(textEntry);
                if ((up && index > 0) || (!up && index < CurrentPage.TextEntries.Count - 1))
                {
                    CurrentPage.MoveTextEntry(index, index + offset);
                    TextEntryMoved?.Invoke(this, new ListModificationEventArgs(index, textEntry, index + offset));
                    TextEntryManager.MovedTextEntry(index, textEntry, index + offset);
                    ChapterManager.IsChapterSaved = false;
                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
                throw e;
            }

        }

        public void SelectTextEntry(Text textEntry)
        {
            int index = CurrentPage.TextEntries.IndexOf(textEntry);
            TextEntryManager.SelectTextEntry(textEntry, index);
        }
    }
}
