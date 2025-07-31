using System; // Provides base types and fundamental classes
using System.Collections.Generic; // For generic collections like List<T>
using System.ComponentModel; // For component model support
using System.Drawing; // For drawing graphics, colors, etc.
using System.Data; // For data-related classes (not used here)
using System.Linq; // For LINQ operations
using System.Text; // For string manipulation
using System.Threading.Tasks; // For async tasks (not used here)
using System.Windows.Forms; // For Windows Forms UI components
using ZXing; // For barcode generation
using System.IO; // For file input/output operations
using ZXing.QrCode; // For QR code generation

namespace a4label
{
    // UserControl for label layout and printing logic
    public partial class LabelLayout : UserControl
    {
        // Origin point for the label grid (in millimeters)
        public PointF origin = new PointF(18.14f, 20.35f + 20.87f);

        // Pitch (distance between labels) in millimeters
        public SizeF pitch = new SizeF(21.0f, 20.87f);

        // List of items to print on each label (text, QR, etc.)
        public List<Printable> labelItems = new List<Printable>();

        // Dictionary of fields for variable substitution in label text
        Dictionary<string, string> fields = new Dictionary<string, string>();

        // History of printed label ranges
        public List<PrintRecord> history = new List<PrintRecord>();

        // Current serial number for printing
        private decimal serialNum;

        // First and last serial numbers in the current print range
        private decimal firstNum, lastNum;

        // Last printed serial number
        public decimal lastPrintedSerno = -1;

        // Last printed version string
        public string lastPrintedVersion = "";

        // Number of labels across the page
        public int repeatAcross = 1;

        // Number of labels down the page
        public int repeatDown = 1;

        // Minimum valid serial number
        public decimal validSerialMin = 0;

        // Maximum valid serial number
        public decimal validSerialMax = (decimal)UInt64.MaxValue;

        // Whether to use random serial numbers
        public bool randomSerialNumber = false;

        // List of error messages from layout parsing
        public List<string> errors = new List<string>();

        // List of overlapping print ranges in history
        public List<string> overlaps = new List<string>();

        // Constructor: initializes the control and its components
        public LabelLayout()
        {
            InitializeComponent();
        }

        // Property for accessing the current serial number
        public decimal SerialNum
        {
            get { return serialNum; }
            set { serialNum = value; }
        }

        // Paint event handler for the label layout (invalidates panel)
        private void LabelLayout_Paint(object sender, PaintEventArgs e)
        {
            panel1.Invalidate();
        }

        // Replaces variable fields in text with actual values, supports recursion
        public string fix(string text, int tries)
        {
            string[] hexMatch = new string[] {
                "[X]", "[XX]", "[XXX]", "[XXXX]", "[XXXXX]", "[XXXXXX]", "[XXXXXXX]", "[XXXXXXXX]", "[XXXXXXXXX]",
                "[XXXXXXXXXX]", "[XXXXXXXXXXX]", "[XXXXXXXXXXXX]", "[XXXXXXXXXXXXX]", "[XXXXXXXXXXXXXX]",
                "[XXXXXXXXXXXXXXX]", "[XXXXXXXXXXXXXXXX]"
            };
            string old = text;
            // Replace serial number placeholders
            if (text.Contains("[NNNNNN]"))
                text = text.Replace("[NNNNNN]", serialNum.ToString("000000"));
            else if (text.Contains("[NNNNN]"))
                text = text.Replace("[NNNNN]", serialNum.ToString("00000"));
            else if (text.Contains("[NNNN]"))
                text = text.Replace("[NNNN]", serialNum.ToString("0000"));
            else if (text.Contains("[NNN]"))
                text = text.Replace("[NNN]", serialNum.ToString("000"));
            else if (text.Contains("[NN]"))
                text = text.Replace("[NN]", serialNum.ToString("00"));
            else if (text.Contains("[N]"))
                text = text.Replace("[N]", serialNum.ToString());
            else if (text.Contains("[MAC12]"))
                text = text.Replace("[MAC12]", (((uint)serialNum / 256) & 0x0f).ToString("X") + "-" + ((uint)serialNum & 255).ToString("X2"));
            else foreach (string match in hexMatch)
                    if (text.Contains(match))
                        text = fixHex(text, match);

            // Replace custom fields
            foreach (string key in fields.Keys)
            {
                if (text.Contains(key))
                    text = text.Replace(key, fields[key]);
            }

            // Recursively fix text if changes were made
            if ((old != text) && (tries > 0))
                return fix(text, tries - 1);
            return text;
        }

        // Helper for replacing hexadecimal placeholders in text
        private string fixHex(string text, string match)
        {
            int len = match.Length - 2;
            string formatter = "X" + len.ToString();
            string s = ((UInt64)serialNum).ToString("X");
            s = s.Substring(s.Length - len);
            return text.Replace(match, s);
        }

        // Checks if the current serial number range is valid for printing
        internal bool canPrint()
        {
            return !((serialNum < validSerialMin) || (lastNum > validSerialMax));
        }

        // Sets the serial number range for printing
        internal void setRange(long start, long count)
        {
            if (serialNum != start)
            {
                serialNum = start;
                this.Invalidate();
            }
            firstNum = start;
            lastNum = serialNum + count - 1;
        }

        // Returns the number of labels per page
        internal int labelsPerPage()
        {
            return repeatDown * repeatAcross;
        }

        // Reads and parses the layout script, populates label items and fields
        internal string[] readSettings(string text)
        {
            errors.Clear();
            labelItems.Clear();
            fields.Clear();
            history.Clear();
            fields.Add("[QRlink]", "?");
            fields.Add("[version]", "?");
            int line = 0;
            decimal first = 1;
            validSerialMin = 0;
            validSerialMax = long.MaxValue;
            lastPrintedSerno = -1;
            repeatAcross = 1;
            repeatDown = 1;
            Pen pen = new Pen(Color.Black, 0.3f);
            Font font = new Font("Verdana", 6);

            StringReader sr = new StringReader(text);
            while (true)
            {
                line++;
                string ln = sr.ReadLine();
                if (ln == null)
                    break;
                try
                {
                    // Parse field assignments
                    if ((ln.IndexOf('[') == 0) && (ln.IndexOf("]=") > 0))
                    {
                        string name = ln.Substring(0, ln.IndexOf("]=") + 1);
                        string value = ln.Substring(ln.IndexOf("]=") + 2);
                        if (fields.ContainsKey(name))
                            fields[name] = value;
                        else
                            fields.Add(name, value);
                        if (name == "[version]")
                            lastPrintedVersion = value;
                    }
                    // Parse layout settings
                    else if (ln.StartsWith("topleft:"))
                    {
                        string[] s = ln.Substring(8).Split(',');
                        origin = new PointF(float.Parse(s[0]), float.Parse(s[1]));
                    }
                    else if (ln.StartsWith("pitch:"))
                    {
                        string[] s = ln.Substring(6).Split(',');
                        pitch = new SizeF(float.Parse(s[0]), float.Parse(s[1]));
                    }
                    else if (ln.StartsWith("across:"))
                    {
                        repeatAcross = int.Parse(ln.Substring(7));
                    }
                    else if (ln.StartsWith("down:"))
                    {
                        repeatDown = int.Parse(ln.Substring(5));
                    }
                    else if (ln.StartsWith("valid_serial_min:"))
                    {
                        validSerialMin = decimal.Parse(ln.Substring(17));
                    }
                    else if (ln.StartsWith("valid_serial_max:"))
                    {
                        validSerialMax = decimal.Parse(ln.Substring(17));
                    }
                    else if (ln.StartsWith("random_serial:"))
                    {
                        randomSerialNumber = ln.Contains("true");
                    }
                    else if (ln.StartsWith("clear:"))
                    {
                        labelItems.Clear();
                        pen = new Pen(Color.Black, 0.3f);
                        font = new Font("Verdana", 6);
                    }
                    else if (ln.StartsWith("font:"))
                    {
                        string[] s = ln.Substring(5).Split(',');
                        font = new Font(s[0], float.Parse(s[1]));
                    }
                    else if (ln.StartsWith("pen:"))
                    {
                        string[] s = ln.Substring(4).Split(',');
                        if (s.Length == 1)
                            pen = new Pen(Color.FromName(s[0]));
                        pen = new Pen(Color.FromName(s[0]), float.Parse(s[1]));
                    }
                    // Parse label items
                    else if (ln.StartsWith("text:"))
                    {
                        labelItems.Add(new HText(this, ln, font));
                    }
                    else if (ln.StartsWith("offset:"))
                    {
                        labelItems.Add(new Offset(this, ln));
                    }
                    else if (ln.StartsWith("circle:"))
                    {
                        labelItems.Add(new Circle(this, ln, pen));
                    }
                    else if (ln.StartsWith("line:"))
                    {
                        labelItems.Add(new Line(this, ln, pen));
                    }
                    else if (ln.StartsWith("QRcode:"))
                    {
                        labelItems.Add(new QRCode(this, ln));
                    }
                    else if (ln.StartsWith("graphic:"))
                    {
                        labelItems.Add(new GraphicFile(this, ln));
                    }
                    // Parse print history
                    else if (ln.StartsWith("PRINTED:"))
                    {
                        PrintRecord pr = new PrintRecord(ln);
                        history.Add(pr);
                        first = pr.start;
                        lastPrintedSerno = pr.end;
                        if (pr.version != "")
                        {
                            lastPrintedVersion = pr.version;
                            fields["[version]"] = pr.version;
                        }
                    }
                    // Ignore empty or comment lines
                    else if (ln.Trim() == "")
                    { }
                    else if (ln.StartsWith("\\") || ln.StartsWith("/"))
                    { }
                    // Unrecognized line: add error
                    else
                    {
                        errors.Add("Line " + line.ToString() + " :" + ln + " ???");
                    }
                }
                catch (Exception e)
                {
                    errors.Add("Line " + line.ToString() + " :" + ln + "\r\n     " + e.Message);
                }
            }

            resizeLabel(); // Adjust preview panel size
            return errors.ToArray();
        }

        // Returns a string describing how labels fit on the page
        internal string pageFit(bool isRandom)
        {
            string s = "";
            if (labelsPerPage() <= 0)
            {
                return "Define labels per sheet with 'across:N' and 'down:N' ";
            }

            decimal pages = (lastNum - serialNum) / labelsPerPage() + 1;
            decimal wasted = pages * labelsPerPage() - (lastNum - serialNum + 1);

            if (!isRandom)
                s += "First:" + firstNum.ToString() + " Last:" + (lastNum.ToString()) + "   ";

            if (pages == 1)
                s += "1 page";
            else
                s += pages.ToString() + " pages";

            if (wasted == 0)
                s += " exactly";
            else
                s += ", " + wasted.ToString() + " wasted";

            if ((firstNum < validSerialMin) || (lastNum > validSerialMax))
                s += "\r\n Invalid serial number, see layout";

            overlaps.Clear();
            foreach (PrintRecord pr in history)
                if (pr.overlaps(firstNum, lastNum))
                    overlaps.Add(pr.ToString());

            return s;
        }

        // Sets the version field and refreshes the control
        internal void setVersion(string text)
        {
            fields["[version]"] = text;
            this.Invalidate();
        }

        // Adds a new print record to history and returns its string
        internal string addHistory()
        {
            PrintRecord pr = new PrintRecord(DateTime.Now, firstNum, lastNum, fields["[version]"]);
            history.Add(pr);
            return "PRINTED:" + pr.ToString();
        }

        // Evaluates a string expression (supports math and field substitution)
        public float eval(string v)
        {
            v = v.Trim();
            if (v.StartsWith("-"))
                return -eval(v.Substring(1));

            int pos = -1;
            int addSubAt = -1;
            int multDivAt = -1;
            int brkts = 0;

            // Find operator position outside brackets
            for (int n = 0; n < v.Length; n++)
            {
                if (v[n] == '(')
                    brkts++;
                else if (v[n] == ')')
                    brkts--;
                else if ((brkts == 0) && ((v[n] == '+') || (v[n] == '-')))
                    addSubAt = n;
                else if ((brkts == 0) && ((v[n] == '*') || (v[n] == '/')))
                    multDivAt = n;
            }

            if (brkts != 0)
                throw new Exception("Brackets mismatch : " + v);

            if (addSubAt != -1)
                pos = addSubAt;
            else
                pos = multDivAt;

            if (pos == -1)
            {
                // No operator: evaluate as value or field
                if (v.StartsWith("(") && v.EndsWith(")"))
                    return eval(v.Substring(1, v.Length - 2));
                else if (v == "cx")
                    return pitch.Width / 2;
                else if (v == "cy")
                    return pitch.Height / 2;
                else if (v == "x")
                    return pitch.Width;
                else if (v == "y")
                    return pitch.Height;
                else if (fields.ContainsKey(v))
                {
                    return eval(fields[v]);
                }
                else
                {
                    float val;
                    if (!float.TryParse(v, out val))
                        throw new Exception("Bad value : " + v);
                    return val;
                }
            }
            else
            {
                // Operator found: evaluate both sides
                string a = v.Substring(0, pos);
                string b = v.Substring(pos + 1);
                char op = v[pos];

                if ((a.Trim() == "") || (b.Trim() == ""))
                    throw new Exception("Bad value : " + v);

                if (op == '*')
                    return eval(a) * eval(b);
                else if (op == '/')
                    return eval(a) / eval(b);
                else if (op == '+')
                    return eval(a) + eval(b);
                else if (op == '-')
                    return eval(a) - eval(b);
            }
            return 0;
        }

        // Returns the current version string
        internal string version()
        {
            return fields["[version]"];
        }

        // Generates a random serial number within valid range
        private decimal makeRandom()
        {
            decimal randomLong;
            Random random = new Random();
            do
            {
                byte[] buffer = new byte[8];
                random.NextBytes(buffer);
                randomLong = BitConverter.ToUInt64(buffer, 0);
            } while (randomLong < 1);

            decimal range = validSerialMax - validSerialMin;

            return (randomLong % range) + validSerialMin;
        }

        // Draws all label items on the provided graphics canvas
        internal void drawOn(Graphics canvas)
        {
            System.Drawing.Drawing2D.GraphicsState transState = canvas.Save();
            foreach (Printable p in labelItems)
                p.draw(canvas);
            canvas.Restore(transState);
        }

        // Draws a page of labels with sequential serial numbers
        public void drawPage(decimal start, int n, Graphics canvas)
        {
            serialNum = start;
            canvas.PageUnit = GraphicsUnit.Millimeter;
            var initialState = canvas.Save();
            canvas.TranslateTransform(origin.X, origin.Y);

            for (int nn = 0; nn < n; nn++)
            {
                var labelState = canvas.Save();

                int row = nn / repeatAcross;
                int col = nn % repeatAcross;

                canvas.TranslateTransform(col * pitch.Width, row * pitch.Height);

                drawOn(canvas);

                canvas.Restore(labelState);

                if (randomSerialNumber)
                    serialNum = makeRandom();
                else
                    serialNum++;
            }

            canvas.Restore(initialState);
        }

        // Draws a page of labels with a list of random serial numbers
        public void drawPage(List<decimal> serials, Graphics canvas)
        {
            canvas.PageUnit = GraphicsUnit.Millimeter;
            var initialState = canvas.Save();
            canvas.TranslateTransform(origin.X, origin.Y);

            int n = serials.Count;
            for (int nn = 0; nn < n; nn++)
            {
                var labelState = canvas.Save();

                int row = nn / repeatAcross;
                int col = nn % repeatAcross;

                canvas.TranslateTransform(col * pitch.Width, row * pitch.Height);

                serialNum = serials[nn];
                drawOn(canvas);

                canvas.Restore(labelState);
            }

            canvas.Restore(initialState);
        }

        // Resizes the label preview panel based on pitch and control size
        private void resizeLabel()
        {
            if ((pitch.Width == 0) || (pitch.Height == 0))
            {
                panel1.Hide();
                return;
            }
            panel1.Show();

            float aspectRatio = pitch.Width / pitch.Height;
            panel1.Height = (int)Math.Min(this.Height * 0.9f, this.Width * 0.9f / aspectRatio);
            panel1.Width = (int)(panel1.Height * aspectRatio);
            panel1.Top = (this.Height - panel1.Height) / 2;
            panel1.Left = (this.Width - panel1.Width) / 2;
        }

        // Paint event handler for the preview panel
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Beige, e.Graphics.ClipBounds);
            float panelWidthMM = 25.4f * (float)panel1.Width / e.Graphics.DpiX;
            float scale = panelWidthMM / pitch.Width;
            e.Graphics.ScaleTransform(scale, scale);

            e.Graphics.PageUnit = GraphicsUnit.Millimeter;
            drawOn(e.Graphics);
        }

        // Resize event handler for the control
        private void LabelLayout_Resize(object sender, EventArgs e)
        {
            resizeLabel();
            Invalidate();
        }
    }

    // Base class for printable items on a label
    public class Printable
    {
        internal const float scale = 100 / 25.4f; // Conversion factor for mm to pixels
        internal float X; // X position
        internal float Y; // Y position
        internal string type = ""; // Type of item (text, line, etc.)
        internal string text = ""; // Text content
        internal LabelLayout label; // Reference to parent label layout

        // Constructor: parses item type and parameters
        public Printable(LabelLayout label, string s)
        {
            this.label = label;
            type = s.Substring(0, s.IndexOf(':'));
            parse(s.Substring(s.IndexOf(':') + 1).Split(','));
        }

        // Evaluates a string expression using label's eval
        public float eval(string s)
        {
            return label.eval(s);
        }

        // Returns text with fields replaced
        public string fixedText()
        {
            return label.fix(text, 10);
        }

        // Parses parameters for the item (to be overridden)
        internal virtual void parse(string[] p)
        {
            throw new NotImplementedException();
        }

        // Draws the item on the canvas (to be overridden)
        public virtual void draw(Graphics canvas)
        {
            throw new NotImplementedException();
        }
    }

    // Printable text item
    public class HText : Printable
    {
        internal Font font = new Font("Verdana", 6);

        public HText(LabelLayout label, string s, Font f) : base(label, s)
        {
            font = f;
        }

        internal override void parse(string[] p)
        {
            X = eval(p[0]);
            Y = eval(p[1]);
            text = p[2];
        }

        public override void draw(Graphics canvas)
        {
            string txt = fixedText();
            SizeF size = canvas.MeasureString(txt, font);
            canvas.DrawString(txt, font, Brushes.Black, X - size.Width / 2, Y - size.Height / 2);
        }
    }

    // Printable line item
    public class Line : Printable
    {
        internal float X2, Y2;
        internal Pen pen = new Pen(Color.Gray, 4);

        public Line(LabelLayout label, string s, Pen pen) : base(label, s)
        {
            this.pen = new Pen(pen.Color, pen.Width);
        }

        internal override void parse(string[] p)
        {
            X = eval(p[0]);
            Y = eval(p[1]);
            X2 = eval(p[2]);
            Y2 = eval(p[3]);
            if (p.Length >= 5)
                pen = new Pen(Color.FromName(p[4]));
            if (p.Length >= 6)
                pen.Width = eval(p[5]);
        }

        public override void draw(Graphics canvas)
        {
            canvas.DrawLine(pen, X, Y, X2, Y2);
        }
    }

    // Printable circle item
    public class Circle : Printable
    {
        internal float rad;
        internal Pen pen = new Pen(Color.Gray, 4);

        public Circle(LabelLayout label, string s, Pen pen) : base(label, s)
        {
            this.pen = new Pen(pen.Color, pen.Width);
        }

        internal override void parse(string[] p)
        {
            X = eval(p[0]);
            Y = eval(p[1]);
            rad = eval(p[2]);
            if (p.Length >= 4)
                pen = new Pen(Color.FromName(p[3]));
            if (p.Length >= 5)
                pen.Width = float.Parse(p[4]);
        }

        public override void draw(Graphics canvas)
        {
            canvas.DrawEllipse(pen, X - rad, Y - rad, rad * 2, rad * 2);
        }
    }

    // Printable QR code item
    public class QRCode : Printable
    {
        internal float size;
        internal Pen pen = new Pen(Color.Gray, 4);

        public QRCode(LabelLayout label, string s) : base(label, s)
        { }

        internal override void parse(string[] p)
        {
            X = eval(p[0]);
            Y = eval(p[1]);
            size = eval(p[2]);
            text = p[3];
        }

        public override void draw(Graphics canvas)
        {
            var writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.QR_CODE;
            writer.Options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = 500,
                Height = 500,
            };
            string s = fixedText();
            if (s == "")
                s = "???";
            Bitmap bmp = new Bitmap(writer.Write(s.Trim()));
            canvas.DrawImage(bmp, X - size / 2, Y - size / 2, size, size);
        }
    }

    // Printable graphic file item
    public class GraphicFile : Printable
    {
        internal float W, H;
        Bitmap bmp;

        public GraphicFile(LabelLayout label, string s) : base(label, s)
        { }

        internal override void parse(string[] p)
        {
            X = eval(p[0]);
            Y = eval(p[1]);
            W = eval(p[2]);
            H = eval(p[3]);
            if (File.Exists(p[4]))
                bmp = new Bitmap(p[4]);
        }

        public override void draw(Graphics canvas)
        {
            if (bmp != null)
                canvas.DrawImage(bmp, X - W / 2, Y - H / 2, W, H);
            else
            {
                canvas.DrawLine(Pens.Red, X, Y, X + H, Y + W);
                canvas.DrawLine(Pens.Red, X, Y + H, X + W, Y);
                canvas.DrawRectangle(Pens.Red, X, Y, W, H);
            }
        }
    }

    // Printable offset item (translates the canvas)
    public class Offset : Printable
    {
        public Offset(LabelLayout label, string s) : base(label, s)
        {
        }

        internal override void parse(string[] p)
        {
            X = eval(p[0]);
            Y = eval(p[1]);
        }

        public override void draw(Graphics canvas)
        {
            canvas.TranslateTransform(X, Y);
        }
    }

    // Record of a printed label range
    public class PrintRecord : Object
    {
        string date; // Date/time of print
        public decimal start, end; // Serial number range
        public string version = ""; // Version string

        // Parses a print record from a string
        public PrintRecord(string ln) : base()
        {
            if (ln.StartsWith("PRINTED:"))
                ln = ln.Substring(8);
            string[] s = ln.Split(' ');
            if (s.Length != 4)
                throw new Exception("Expected PRINTED:date time nn-nn comment");
            date = s[0] + " " + s[1];
            string[] num = s[2].Split('-');
            decimal.TryParse(num[0], out start);
            decimal.TryParse(num[1], out end);
            if (s.Length >= 4)
                version = ln.Substring(s[0].Length + s[1].Length + s[2].Length + 3);
            else
                version = "";
        }

        // Creates a print record from parameters
        public PrintRecord(DateTime date, decimal start, decimal end, string version) : base()
        {
            this.date = date.ToShortDateString() + " " + date.ToShortTimeString();
            this.start = start;
            this.end = end;
            this.version = version;
        }

        // Checks if this record overlaps with a given range
        public bool overlaps(decimal first, decimal last)
        {
            return ((first <= end) && (last >= start));
        }

        // Returns a string representation of the record
        public override string ToString()
        {
            return string.Format("{0} {1}-{2} {3}", date, start, end, version);
        }
    }
}