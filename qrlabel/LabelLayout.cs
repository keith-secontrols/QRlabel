﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using System.IO;
using ZXing.QrCode;

namespace a4label
{
    public partial class LabelLayout : UserControl
    {

        public PointF origin = new PointF(18.14f, 20.35f + 20.87f);
        public SizeF pitch = new SizeF(21.0f, 20.87f);
        public List<Printable> labelItems = new List<Printable>();
        Dictionary<string, string> fields = new Dictionary<string, string>();
        public List<PrintRecord> history = new List<PrintRecord>();

        private int serialNum;
        private int firstNum,lastNum;
        public int lastPrintedSerno = -1;
        public string lastPrintedVersion = "";
        public int repeatAcross = 1;
        public int repeatDown = 1;
        private int validSerialMin;
        private int validSerialMax;
        public List<string> errors = new List<string>();


        public List<string> overlaps = new List<string>();

        public LabelLayout()
        {
            InitializeComponent();
        }

        private void LabelLayout_Paint(object sender, PaintEventArgs e)
        {
            panel1.Invalidate();
        }


        public string fix(string text, int tries)
        {
            string old = text;
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
                text = text.Replace("[MAC12]", (serialNum / 256).ToString("X") + "-" + (serialNum & 255).ToString("X2"));

            foreach (string key in fields.Keys)
            {
                if (text.Contains(key))
                    text = text.Replace(key, fields[key]);
            }

            if ((old != text) && (tries > 0))
                return fix(text, tries - 1);
            return text;
        }

        internal bool canPrint()
        {
            return !((serialNum < validSerialMin) || (lastNum > validSerialMax));
        }

        internal void setRange(int start, int count)
        {
            if (serialNum != start)
            {
                serialNum = start;
                this.Invalidate();
            }
            firstNum = start;
            lastNum = serialNum + count - 1;
        }

        internal int labelsPerPage()
        {
            return repeatDown * repeatAcross;
        }

        internal string[] readSettings(string text)
        {
            errors.Clear();
            labelItems.Clear();
            fields.Clear();
            history.Clear();
            fields.Add("[QRlink]", "?");
            fields.Add("[version]", "?");
            int line = 0;
            int first = 1;
            validSerialMin = 0;
            validSerialMax = int.MaxValue;
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
                        validSerialMin = int.Parse(ln.Substring(17));
                    }
                    else if (ln.StartsWith("valid_serial_max:"))
                    {
                        validSerialMax = int.Parse(ln.Substring(17));
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
                    else if (ln.Trim() == "")
                    { }
                    else if (ln.StartsWith("\\") || ln.StartsWith("/"))
                    { }
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

            resizeLabel();
            return errors.ToArray();
        }



        internal string pageFit()
        {
            string s = "";
            if (labelsPerPage() <= 0)
            {
                return "Define labels per sheet with 'across:N' and 'down:N' ";
            }

            int pages = (lastNum - serialNum) / labelsPerPage() + 1;
            int wasted = pages * labelsPerPage() - (lastNum - serialNum + 1);

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



            //                label1.Text = "Only 4096 MAC addresses available|Firmware changes are required and new MAC prefix";



            return s;
        }

        internal void setVersion(string text)
        {
            fields["[version]"] = text;
            this.Invalidate();
        }
        internal string addHistory()
        {
            PrintRecord pr = new PrintRecord(DateTime.Now, firstNum, lastNum, fields["[version]"]);
            history.Add(pr);
            return "PRINTED:" + pr.ToString();
        }

        public float eval(string v)
        {
            v = v.Trim();

            if (v.StartsWith("-"))
                return -eval(v.Substring(1));

            int pos;
            pos = Math.Max(v.IndexOf("+"), v.IndexOf("-"));

            if (pos == -1)
            {
                pos = Math.Max(v.IndexOf("*"), v.IndexOf("/"));
            }

            if (pos == -1)
            {
                if (v == "cx")
                    return pitch.Width / 2;
                if (v == "cy")
                    return pitch.Height / 2;
                if (v == "w")
                    return pitch.Width;
                if (v == "h")
                    return pitch.Height;

                float val;
                if (!float.TryParse(v, out val))
                    throw new Exception("Bad value:" + v);
                return val;
            }
            else
            {
                string a = v.Substring(0, pos);
                string b = v.Substring(pos + 1);
                char op = v[pos];

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

        internal string QRlink()
        {
            return fields["[QRlink]"];
        }

        internal string version()
        {
            return fields["[version]"];
        }


        internal void drawOn(Graphics canvas)
        {
            System.Drawing.Drawing2D.GraphicsState transState = canvas.Save();
            foreach (Printable p in labelItems)
                p.draw(canvas);
            canvas.Restore(transState);
        }

        internal void drawPage(int start, int n, Graphics canvas)
        {

            serialNum = start;
            canvas.PageUnit = GraphicsUnit.Millimeter;
            canvas.TranslateTransform(origin.X, origin.Y);

            for (int nn = 0; nn < n; nn++)
            {
                drawOn(canvas);
                serialNum++;
                canvas.TranslateTransform(pitch.Width, 0);
                if (((nn + 1) % repeatAcross) == 0)
                    canvas.TranslateTransform(-pitch.Width * repeatAcross, pitch.Height);
            }
        }

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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Beige, e.Graphics.ClipBounds);
            float panelWidthMM = 25.4f*(float)panel1.Width/e.Graphics.DpiX;
            float scale = panelWidthMM / pitch.Width;
            e.Graphics.ScaleTransform(scale, scale);

            e.Graphics.PageUnit = GraphicsUnit.Millimeter;
     //       e.Graphics.DrawRectangle(Pens.Blue, 0, 0, pitch.Width, pitch.Height);
            drawOn(e.Graphics);
        }

        private void LabelLayout_Resize(object sender, EventArgs e)
        {
            resizeLabel();
            Invalidate();
        }
    }

    public class Printable
    {
        internal const float scale = 100 / 25.4f;
        internal float X;
        internal float Y;
        internal string type = "";
        internal string text = "";
        internal LabelLayout label;

        public Printable(LabelLayout label, string s)
        {
            this.label = label;
            type = s.Substring(0, s.IndexOf(':'));
            parse(s.Substring(s.IndexOf(':') + 1).Split(','));

        }

        public float eval(string s)
        {
            return label.eval(s);
        }

        public string fixedText()
        {
            return label.fix(text, 10);
        }


        internal virtual void parse(string[] p)
        {
            throw new NotImplementedException();
        }

        public virtual void draw(Graphics canvas)
        {
            throw new NotImplementedException();
        }
    }

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


    public class PrintRecord:Object
    {
        string date;
        public int start, end;
        public string version="";

        public PrintRecord(string ln):base()
        {

            if (ln.StartsWith("PRINTED:"))
                ln = ln.Substring(8);
            string[] s = ln.Split(' ');
            if (s.Length != 4)
                throw new Exception("Expected PRINTED:date time nn-nn comment");
            date = s[0]+" "+s[1];
            string[] num = s[2].Split('-');
            int.TryParse(num[0], out start);
            int.TryParse(num[1], out end);
            if (s.Length >= 4)
                version = ln.Substring(s[0].Length + s[1].Length + s[2].Length + 3);
            else
                version = "";
        }

        public PrintRecord(DateTime date, int start, int end, string version):base()
        {
            this.date = date.ToShortDateString() + " " + date.ToShortTimeString();
            this.start = start;
            this.end = end;
            this.version = version;
        }

        public bool overlaps(int first, int last)
        {
            return ((first <= end) && (last >= start));
        }

        public override string ToString()
        {
            return string.Format("{0} {1}-{2} {3}", date, start, end, version);
        }

    }


}
