
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;


namespace Miharu2.BackEnd.Data {
	[JsonObject(MemberSerialization.OptOut)]
	public class Page
    {
		[JsonIgnoreAttribute]
		private volatile bool _ready = false;
		[JsonIgnoreAttribute]
		public bool Ready {
			get => _ready;
			private set => _ready = value;
		}
		[JsonIgnoreAttribute]
		public EventWaitHandle PageWaitHandle {
			get;
			private set;
		}


		public event EventHandler PageChanged;

		[JsonIgnore]
		public string Name {
			get {
				int lastSlash = Path.LastIndexOf('\\') + 1;
				return Path.Substring(lastSlash);
			}
		}

		public string Path {
			get; private set;
		}
		public void MakePathRelative (Uri relativeTo) {
			Uri pageUri = new Uri(Path);
			Path = Uri.UnescapeDataString(relativeTo.MakeRelativeUri(pageUri).ToString().Replace("/", "\\"));
		}
		public void MakePathAbsolute (string absoluteFrom) {
			string res = System.IO.Path.Combine(absoluteFrom, Path);
			if (File.Exists(res))
				Path = res;
			if (!File.Exists(Path))
				throw new IOException("Couldn't find file: " + Path);
		}
		

		[JsonIgnoreAttribute]
		public Image Source {
			get; private set;
		}

		[JsonIgnore]
		public static bool UseScreenDPI {
			get; set;
		}


		[JsonIgnoreAttribute]
		private double ScreenDPIX {
			get; set;
		}
		[JsonIgnoreAttribute]
		private double ScreenDPIY {
			get; set;
		}


		public List<Text> TextEntries {
			get; private set;
		}

		

		public Page (string src) {
			Path = src;
			TextEntries = new List<Text>();
			PageWaitHandle = new ManualResetEvent(false);
			ScreenDPIX = ScreenDPIY = Eto.Forms.Screen.PrimaryScreen.RealDPI;
		}

		[JsonConstructor]
		public Page (string path, List<Text> textEntries) {
			Path = path;
			TextEntries = textEntries;
			PageWaitHandle = new ManualResetEvent(false);
			ScreenDPIX = ScreenDPIY = Eto.Forms.Screen.PrimaryScreen.RealDPI;
		}

		

		public Text AddTextEntry (DPIAwareRectangle rect) {
			
			Text txt = new Text(CropImage(rect), rect);
			TextEntries.Add(txt);
			PageChanged?.Invoke(this, new EventArgs());
			return txt;
		}

		public void MoveTextEntry (int index, int newIndex) {
			Text tmp = TextEntries [index];
			TextEntries.RemoveAt(index);
			TextEntries.Insert(newIndex, tmp);
			PageChanged?.Invoke(this, new EventArgs());
		}

		public void RemoveTextEntry (int index) {
			TextEntries.RemoveAt(index);
			PageChanged?.Invoke(this, new EventArgs());
		}


		//TODO this has probably changed with ImageSharp
		private Image CropImage (DPIAwareRectangle DPIrect) {
			if (DPIrect.Width == 0 || DPIrect.Height == 0)
				return null;
			
			// From Microsoft's Win2D docs:
			// [...]bitmap DPI defaults to 96 regardless of the current display configuration
			double xdpiRatio = DPIrect.DpiX / (UseScreenDPI ? ScreenDPIX : 96);
			double ydpiRatio = DPIrect.DpiY / (UseScreenDPI ? ScreenDPIY : 96);
			Rectangle rect = new Rectangle((int)(DPIrect.X*xdpiRatio + 0.5), (int)(DPIrect.Y*ydpiRatio + 0.5), (int)(DPIrect.Width*xdpiRatio + 0.5), (int)(DPIrect.Height*ydpiRatio + 0.5));
			
			//Rectangle rect = new Rectangle((int)(DPIrect.X), (int)(DPIrect.Y), (int)(DPIrect.Width), (int)(DPIrect.Height));
			
			Image cropped = Source.Clone(x => x.Crop(rect));

			return cropped;
		}

		

		public void Load () {
			Source = Image.Load(Path);
			foreach (Text t in TextEntries)
				t.Load(CropImage(t.DpiAwareRectangle));
			Ready = true;
			PageWaitHandle.Set();
		}

		public void Reload () {
			foreach (Text t in TextEntries)
				t.Load(CropImage(t.DpiAwareRectangle));
		}

		public void ExportScript (StreamWriter writer) {
			foreach (Text t in TextEntries)
				writer.WriteLine(t.TranslatedText + Environment.NewLine);
		}

		public void ExportCustomScript (StreamWriter writer, ExportData ed) {
			foreach (Text t in TextEntries) {
				if (ed.HasFlag(ExportData.Japanese))
					writer.WriteLine (t.ParsedText + Environment.NewLine);
				if (ed.HasFlag(ExportData.Notes)) {
					foreach (Note n in t.NotesEnumerator)
						writer.WriteLine("Note: " + n.Content);
					writer.WriteLine();
				}
				if (ed.HasFlag(ExportData.Translation))
					writer.WriteLine(t.TranslatedText + Environment.NewLine);
			}
		}

		public override string ToString() {
			return Name;
		}


	}
}
