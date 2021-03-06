using MahApps.Metro.Controls;
using Miharu.BackEnd.Data;
using Miharu.Control;
using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Miharu.FrontEnd.Helper
{
	/// <summary>
	/// Interaction logic for ExportDialogue.xaml
	/// </summary>
	public partial class ExportDialogue : MetroWindow
	{
		private ChapterManager _chapterManager;

		public ExportDialogue(ChapterManager chapterManager)
		{
			InitializeComponent();
			_chapterManager = chapterManager;
			int lastSlashIndex = -1;;
			if (_chapterManager.CurrentSaveFile == null || ((lastSlashIndex = _chapterManager.CurrentSaveFile.LastIndexOf("\\")) == -1))
				ExportPathTextBox.Text = null;
			else
				ExportPathTextBox.Text = _chapterManager.CurrentSaveFile.Substring(0, lastSlashIndex) + "\\script.txt";
			IndexPagesLabel.Content = IndexPagesSwitch.OffContent;
		}

		

		private void ExportPathButton_Click(object sender, RoutedEventArgs e)
		{
			VistaSaveFileDialog fileDialog = new VistaSaveFileDialog();
			fileDialog.AddExtension = true;
			fileDialog.DefaultExt = ".txt";
			fileDialog.Filter = "Script files (*.txt)|*.txt";
			fileDialog.OverwritePrompt = true;
			fileDialog.Title = "Export Complete Script";
			bool? res = fileDialog.ShowDialog(this);
			if (res ?? false) {
				ExportPathTextBox.Text = fileDialog.FileName;
			}
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			FileInfo fi = new FileInfo(ExportPathTextBox.Text);
			if (!fi.Directory.Exists) {
				using (TaskDialog dialog = new TaskDialog()) {
					dialog.WindowTitle = "Error";
					dialog.MainIcon = TaskDialogIcon.Error;
					dialog.MainInstruction = "Could not find specified directory.";
					TaskDialogButton okButton = new TaskDialogButton(ButtonType.Ok);
					dialog.Buttons.Add(okButton);
					TaskDialogButton button = dialog.ShowDialog(this);
				}
			}
			else {
				try {
					Mouse.SetCursor(Cursors.Wait);
					ExportData ed = ExportData.None;
					if (JapaneseTextSwitch.IsOn)
						ed = ed | ExportData.Japanese;
					if (NotesSwitch.IsOn)
						ed = ed | ExportData.Notes;
					if (TranslationSwitch.IsOn)
						ed = ed | ExportData.Translation;
					if (IndexPagesSwitch.IsOn)
						ed = ed | ExportData.Filename;

					_chapterManager.ExportCustomScript(ExportPathTextBox.Text, ed);
					Mouse.SetCursor(Cursors.Arrow);
				}
				catch (Exception ex) {
					Mouse.SetCursor(Cursors.Arrow);

					using (TaskDialog dialog = new TaskDialog()) {
						dialog.WindowTitle = "Error";
						dialog.MainIcon = TaskDialogIcon.Error;
						dialog.MainInstruction = "There was an error exporting the script.";
						dialog.Content = ex.Message;
						TaskDialogButton okButton = new TaskDialogButton(ButtonType.Ok);
						dialog.Buttons.Add(okButton);
						TaskDialogButton button = dialog.ShowDialog(this);
					}
					return;
				}
				Close();
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}


		private void FileNameSwitch_Checked(object sender, RoutedEventArgs e)
		{
			if (IndexPagesSwitch.IsOn)
				IndexPagesLabel.Content = IndexPagesSwitch.OnContent;
			else
				IndexPagesLabel.Content = IndexPagesSwitch.OffContent;
		}

	}
}
