
using Miharu.BackEnd.Helper;
using Miharu.BackEnd.Translation;
using Miharu.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Xml.Linq;

namespace Miharu.BackEnd.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Text
    {
        public event TxtEventHandler TextChanged;
        public event EventHandler RotationChanged;

        [JsonIgnore]
        private static readonly string TEMP_IMG = @"./tmp.png";

        [JsonIgnore]
        private static readonly string TEMP_TXT = @"./tmp.hocr";

        [JsonIgnore]
        private bool _parseInvalidated = true;

        public readonly Guid Uuid;
        public void Invalidate()
        {
            _parseInvalidated = true;
        }

        private bool _vertical;
        public bool Vertical
        {
            get => _vertical;
            set
            {
                _vertical = value;
                TextChanged?.Invoke(this, new TxtChangedEventArgs(TextChangeType.Vertical, null, null));
            }
        }

        [JsonIgnore]
        public Bitmap Source { get; private set; }

        [JsonIgnore]
        public Bitmap OriginalSource { get; private set; }

        public DPIAwareRectangle DpiAwareRectangle { get; private set; }

        private float _rotation = 0;

        public float Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value % 360;
                if (_rotation > 180)
                {
                    _rotation -= 360;
                }

                if (_rotation < -180)
                {
                    _rotation += 360;
                }

                int width = OriginalSource.Width;
                int height = OriginalSource.Height;
                PixelFormat pixelFormat = OriginalSource.PixelFormat;

                Bitmap tempImg = new Bitmap(width, height, pixelFormat);
                Graphics g = Graphics.FromImage(tempImg);
                g.Clear(Color.White);
                g.DrawImageUnscaled(OriginalSource, 1, 1);
                g.Dispose();

                GraphicsPath path = new GraphicsPath();
                path.AddRectangle(new RectangleF(0f, 0f, width, height));
                Matrix rotMatrix = new Matrix();
                rotMatrix.Rotate(_rotation);

                RectangleF rct = path.GetBounds(rotMatrix);
                Bitmap newImg = new Bitmap(Convert.ToInt32(rct.Width), Convert.ToInt32(rct.Height), pixelFormat);
                g = Graphics.FromImage(newImg);
                g.Clear(Color.White);
                g.TranslateTransform(-rct.X, -rct.Y);
                g.RotateTransform(_rotation);
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.DrawImageUnscaled(tempImg, 0, 0);
                g.Dispose();
                tempImg.Dispose();

                Source = newImg;
                RotationChanged?.Invoke(this, new EventArgs());
            }
        }

        private string _parsedText = null;

        public string ParsedText
        {
            get
            {
                if (_parsedText == null || _parseInvalidated)
                {
                    _parsedText = ParseText();
                    _parseInvalidated = false;
                    TextChanged?.Invoke(this, new TxtChangedEventArgs(TextChangeType.Parse, null, _parsedText));
                }
                return _parsedText;
            }
            set
            {
                _parsedText = value;
                TextChanged?.Invoke(this, new TxtChangedEventArgs(TextChangeType.Parse, null, _parsedText));
            }
        }

        [JsonProperty]
        private volatile Dictionary<TranslationType, string> _translations;
        public string GetTranslation(TranslationType type)
        {
            return !_translations.TryGetValue(type, out string res) ? null : res;
        }

        public void SetTranslation(TranslationType type, string value)
        {
            Monitor.Enter(_translations);
            try
            {
                _translations[type] = value;
            }
            finally
            {
                Monitor.Exit(_translations);
            }
            TextChanged?.Invoke(this, new TxtChangedEventArgs(TextChangeType.TranslationSource, type, value));
        }

        [JsonProperty]
        public List<Note> UniqueNotes => _uniqueNotes.Values.ToList();

        [JsonIgnore]
        private readonly Dictionary<Guid, Note> _uniqueNotes;

        [JsonIgnore]
        public int NotesCount => _uniqueNotes.Count;

        [JsonIgnore]
        public IEnumerable<Note> NotesEnumerator => _uniqueNotes.Values;

        public Guid AddNote(string text)
        {
            Note n = new Note(text);
            _uniqueNotes.Add(n.Uuid, n);
            return n.Uuid;
        }

        public void RemoveNote(Guid guid)
        {
            _uniqueNotes.Remove(guid);
        }

        public void SetNote(Guid guid, string text)
        {
            _uniqueNotes[guid].Content = text;
        }

        public Note GetNote(Guid guid)
        {
            return _uniqueNotes[guid];
        }

        private string _translatedText;
        public string TranslatedText
        {
            get => _translatedText;
            set
            {
                _translatedText = value;
                TextChanged?.Invoke(this, new TxtChangedEventArgs(TextChangeType.Translation, null, value));
            }
        }

        public Text(Bitmap src, DPIAwareRectangle rect)
        {
            Uuid = Guid.NewGuid();
            if (rect.Width == 0 || rect.Height == 0)
            {
                throw new ArgumentOutOfRangeException("rect", rect, "Can't create text entry with 0 width or 0 height rectangle.");
            }

            OriginalSource = Source = src;
            DpiAwareRectangle = rect;
            TranslatedText = "";
            Vertical = src.Height >= src.Width;
            _translations = new Dictionary<TranslationType, string>();
            _uniqueNotes = new Dictionary<Guid, Note>();

        }

        //There are legacy parameters, so loading old saves still works
        [JsonConstructor]
        public Text(Guid? uuid,
                    DPIAwareRectangle dpiAwareRectangle, float? rotation, bool vertical, bool parseInvalidated,
                    string parsedText,
                    List<Note> uniqueNotes,
                    List<string> _notes,
                    Dictionary<TranslationType, string> translations,
                    Rect rectangle,
                    string googleTranslatedText, string bingTranslatedText,
                    string translatedText)
        {
            Uuid = uuid ?? Guid.NewGuid();

            if (dpiAwareRectangle != null)
            {
                DpiAwareRectangle = dpiAwareRectangle;
            }
            else
            {
                Graphics g = Graphics.FromHwnd(IntPtr.Zero);
                double dpiX = g.DpiX;
                double dpiY = g.DpiY;
                DpiAwareRectangle = new DPIAwareRectangle(rectangle, dpiX, dpiY);
            }

            _rotation = rotation ?? 0;

            Vertical = vertical;
            _parseInvalidated = parseInvalidated;
            ParsedText = parsedText;

            _uniqueNotes = new Dictionary<Guid, Note>();
            if (uniqueNotes != null)
            {
                foreach (Note n in uniqueNotes)
                {
                    _uniqueNotes.Add(n.Uuid, n);
                }
            }
            else if (_notes != null)
            {
                foreach (string text in _notes)
                {
                    Note n = new Note(text);
                    _uniqueNotes.Add(n.Uuid, n);
                }
            }

            if (translations != null)
            {
                _translations = translations;
            }
            else
            {
                _translations = new Dictionary<TranslationType, string>();
                _translations[TranslationType.Google_API] = googleTranslatedText;
                _translations[TranslationType.Bing_API] = bingTranslatedText;
            }
            TranslatedText = translatedText;
        }

        [JsonIgnore]
        private static readonly string _START_BODY = "<body>";

        [JsonIgnore]
        private static readonly string _END_BODY = "</body>";

        private string ReadHOCR(string hocrPath)
        {
            string res = "";

            using (StreamReader reader = new StreamReader(hocrPath))
            {
                string body = reader.ReadToEnd();
                reader.Close();
                int bodyStartIndex = body.IndexOf(_START_BODY);
                int bodyEndIndex = body.IndexOf(_END_BODY);
                body = body.Substring(bodyStartIndex, bodyEndIndex - bodyStartIndex + _END_BODY.Length);
                XElement XMLBody = XElement.Parse(body);
                IEnumerable<XElement> paragraphs = XMLBody.Descendants("p");
                foreach (XElement p in paragraphs)
                {
                    foreach (XElement line in p.Elements("span"))
                    {
                        foreach (XElement word in line.Elements("span"))
                        {
                            res += word.Value;
                        }
                    }

                    if (p != paragraphs.Last())
                    {
                        res += Environment.NewLine;
                    }
                }
            }

            return res;
        }

        private string ParseText()
        {
            if (Source == null ||
                !Enum.TryParse(Settings.Default.TesseractSourceLanguage.ToString(), out TesseractSourceLanguage tesseractSourceLanguage))
            {
                return "";
            }

            Source.Save(TEMP_IMG, ImageFormat.Png);

            using (Process pProcess = new Process())
            {
                pProcess.StartInfo.FileName = Settings.Default.TesseractPath;
                string lang = tesseractSourceLanguage.ToTesseractTrainDataName(Vertical);
                int psm = Vertical ? 5 : 6;
                pProcess.StartInfo.Arguments = TEMP_IMG + " tmp -l " + lang + " --psm " + psm + " hocr"; //argument
                pProcess.StartInfo.UseShellExecute = false;
                //pProcess.StartInfo.RedirectStandardOutput = true;
                pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
                pProcess.Start();
                pProcess.WaitForExit();
            }

            return ReadHOCR(TEMP_TXT);
        }

        public void Load(Bitmap src)
        {
            OriginalSource = Source = src;
            Rotation = _rotation;
        }
    }
}
