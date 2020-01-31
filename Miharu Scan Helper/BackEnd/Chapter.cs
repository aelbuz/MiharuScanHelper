﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Manga_Scan_Helper.BackEnd
{
	[JsonObject(MemberSerialization.OptOut)]
    public class Chapter
    {
		[JsonIgnore]
		private volatile bool _allPagesLoaded = false;

		public bool AllPagesReady {
			get => _allPagesLoaded;
		}
		
		[JsonIgnore]
		public EventWaitHandle ChapterWaitHandle {
			get;
			private set;
		}


		public List<Page> Pages {
			get; private set;
		}

		public int TotalPages {
			get { return Pages.Count; }
		}

		public Chapter (string folderSrc) {
			Pages = new List<Page>();

			DirectoryInfo d = new DirectoryInfo(folderSrc);

			FileInfo [] files = d.GetFiles("*.jpg", SearchOption.TopDirectoryOnly);
			if (files.Length == 0)
				files = d.GetFiles("*.jpeg", SearchOption.TopDirectoryOnly);
			if (files.Length == 0)
				files = d.GetFiles("*.png", SearchOption.TopDirectoryOnly);
			if (files.Length == 0)
				throw new Exception("No images were found in folder " + folderSrc + Environment.NewLine + Environment.NewLine + "Only jpg, jpeg or png files supported.");
			
			
			FileInfo[] sortedFiles = null;
			try {
				sortedFiles = files.OrderBy(x => {
					return Int32.Parse(x.Name.Substring(0, x.Name.IndexOf('.')));
				}).ToArray();

			}
			catch (FormatException) {
				sortedFiles = files.OrderBy(x=> x.Name).ToArray();
			}
			foreach (FileInfo file in sortedFiles) {
				Page p = new Page (file.FullName);
				Pages.Add(p);
			}
			
			ChapterWaitHandle = new ManualResetEvent(false);
			Pages[0].Load();
			Task.Run(() => LoadPagesAsync(this, 0));
		}

		public Chapter (string [] filesSrc) {
			Pages = new List<Page>();
			string [] sortedFiles = null;
			try {
				sortedFiles = filesSrc.OrderBy(x => {
					int lastSlash = x.LastIndexOf("\\") + 1;
					return Int32.Parse(x.Substring(lastSlash, x.IndexOf('.') - lastSlash));						
				}).ToArray();
			}
			catch (FormatException) {
				sortedFiles = filesSrc.OrderBy(x=> x.Substring(x.LastIndexOf("\\") + 1)).ToArray();
			}

			foreach (string file in sortedFiles) {
				Page p = new Page (file);
				Pages.Add(p);
			}

			ChapterWaitHandle = new ManualResetEvent(false);
			Pages[0].Load();
			Task.Run(() => LoadPagesAsync(this, 0));
		}

		

		[JsonConstructor]
		private Chapter (List<Page> pages) {
			Pages = pages;
			ChapterWaitHandle = new ManualResetEvent(false);
		}

		public void AddPage (int pageIndex, string src) {
			Page page = new Page(src);
			Pages.Insert(pageIndex, page);
			page.Load();
		}

		public void RemovePage (int pageIndex) {
			Pages.RemoveAt(pageIndex);
		}


		public void MovePageUp(int pageIndex)
		{
			Page aux = Pages [pageIndex];
			Pages.RemoveAt(pageIndex);
			Pages.Insert(pageIndex-1, aux);

		}

		public void MovePageDown(int pageIndex)
		{
			Page aux = Pages [pageIndex];
			Pages.RemoveAt(pageIndex);
			if (pageIndex >= TotalPages)
				Pages.Add(aux);
			else
				Pages.Insert(pageIndex+1, aux);

		}


		public void Save (string destPath, int currentPage = 0) {
			StreamWriter writer = null;
			try {
				writer = new StreamWriter(destPath, false, Encoding.UTF8);	
				writer.WriteLine(currentPage);
				writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
				/*writer.WriteLine(Path);
				writer.WriteLine(TotalPages);
				foreach (Page p in Pages)
					p.Save(writer);*/
				writer.Close();
			}
			catch (Exception e) {
				writer?.Close();
				throw e;
			}
			
		}

		public static Chapter Load (string src, out int page) {
			Chapter res = null;
			StreamReader reader = null;
			page = 0;
			try {
				reader = new StreamReader(src);
				if (reader.Peek() != '{')
					page = int.Parse(reader.ReadLine());
				res = JsonConvert.DeserializeObject<Chapter>(reader.ReadToEnd());
				res.Pages[page].Load();

				int loadPage = page;
				Task.Run(() => LoadPagesAsync(res, loadPage));
				
				reader.Close();
			}
			catch (Exception e){
				reader?.Close();
				throw e;
			}

			return res;
			
		}

		private static void LoadPagesAsync (Chapter res, int page) {
			int lower = page -1;
			int higher = page +1;
			while (lower >= 0 || higher < res.TotalPages) {
				if (lower >= 0)
					res.Pages[lower--].Load();
				if (higher < res.TotalPages)
					res.Pages[higher++].Load();
			}
			res._allPagesLoaded = true;
			res.ChapterWaitHandle.Set();
		}

		public void ExportScript (string destPath) {
			StreamWriter writer = null;
			try {
				writer = new StreamWriter(destPath, false, Encoding.UTF8);
				for (int i = 1; i <= Pages.Count; i++) {
					if (Pages [i - 1].TextEntries.Count > 0) {
						writer.WriteLine(i.ToString("D2") + Environment.NewLine);
						Pages[i-1].ExportScript (writer);
						writer.WriteLine();
					}
				}
				writer.Close();
			}
			catch (Exception e) {
				writer?.Close();
				throw e;
			}
		}

		public void ExportJPScript (string destPath) {
			StreamWriter writer = null;
			try {
				writer = new StreamWriter(destPath, false, Encoding.UTF8);
				for (int i = 1; i <= Pages.Count; i++) {
					if (Pages [i - 1].TextEntries.Count > 0) {
						writer.WriteLine(i.ToString("D2") + Environment.NewLine);
						Pages [i - 1].ExportJPScript(writer);
						writer.WriteLine();
					}
				}
				writer.Close();
			}
			catch (Exception e) {
				writer?.Close();
				throw e;
			}
		}

		public void ExportCompleteScript (string destPath) {
			StreamWriter writer = null;
			try {
				writer = new StreamWriter(destPath, false, Encoding.UTF8);
				for (int i = 1; i <= Pages.Count; i++) {
					if (Pages [i - 1].TextEntries.Count > 0) {
						writer.WriteLine(i.ToString("D2") + Environment.NewLine);
						Pages [i - 1].ExportCompleteScript(writer);
						writer.WriteLine();
					}
				}
				writer.Close();
			}
			catch (Exception e) {
				writer?.Close();
				throw e;
			}
		}

		
	}
}
