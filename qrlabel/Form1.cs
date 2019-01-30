using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing.Common;
using ZXing;
using ZXing.QrCode;

namespace qrlabel
{
    public partial class Form1 : Form
    {
  //      const string prefix = "http://sandbox.innovodesign.co.uk/nvlink2/";
        const string prefix = "http://find.secontrols.link/nvl2/";
        public Form1()
        {
            InitializeComponent();
            this.Text = prefix+"NNNN";
            numericUpDown_ValueChanged(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PCPrint printer = new PCPrint((int)numericUpDown1.Value, (int)numericUpDown2.Value, prefix);
            
//            PrintDialog pd = new PrintDialog();
//            PrinterSettings ps = new PrinterSettings();
//            pd.PrinterSettings = ps;

            PrintPreviewDialog printPrvDlg = new PrintPreviewDialog();
            printPrvDlg.Width = 600;
            printPrvDlg.Height = 1000;

//            DialogResult dr = pd.ShowDialog();
            printPrvDlg.Document = printer;
        
            if (printPrvDlg.ShowDialog() == DialogResult.OK)
            {
                printer.Print();
            }
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {

        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = PCPrint.makeQR(prefix+numericUpDown1.Value.ToString());
            bool invalid = (numericUpDown1.Value + numericUpDown2.Value > 4095);
           if (invalid)
            {
                label1.Text = "Only 4096 MAC addresses available\r\nFirmware changes are required and new MAC prefix";
            }
            else
            {
                label1.Text = prefix + numericUpDown1.Value.ToString();
            }
            button1.Enabled = !invalid;
        }

    }


}


public class PCPrint : System.Drawing.Printing.PrintDocument
{
    const float scale = 100 / 25.4f;
    const float hpitch = 21.0f;
    const float vpitch = 20.87f;
    public string versionString = "V2.00";
    float originX = 18.14f;
    float originY = 20.35f+vpitch;

    private int start;
    private int count;
    private string prefix;
    Bitmap logo = new Bitmap("logo.bmp");

    public PCPrint(int start, int count, string prefix) : base()
    {
        this.start = start;
        this.count = count;
        this.prefix = prefix;
    }


    public static Bitmap makeQR(string s)
    {
        QrCodeEncodingOptions options = new QrCodeEncodingOptions
        {
            DisableECI = true,
            CharacterSet = "UTF-8",
            Width = 500,
            Height = 500,
        };
        var writer = new BarcodeWriter();
        writer.Format = BarcodeFormat.QR_CODE;
        writer.Options = options;
        Bitmap result = new Bitmap(writer.Write(s.Trim()));
        
        return result;
    }

    protected override void OnPrintPage(PrintPageEventArgs e)
    {
        // Run base code
        base.OnPrintPage(e);
        draw(e.Graphics);
        e.HasMorePages = false;
    }

    private void draw(System.Drawing.Graphics canvas)
    {
        float X = originX * scale;
        float Y = originY * scale;
        int NN = 0;
        const int labelsPerRow = 9;
        Pen pen = new Pen(Color.Gray, 4);
        Font PrinterFont = new Font("Verdana", 6);
        Font MACfont = new Font("Verdana", 4f);

        //Fit as many characters as we can into the print area      
        for (int n = start; n < start + count; n++)
        {
            float rad = 19f / 2f * scale;
            canvas.DrawEllipse(new Pen(Color.Gray), X - rad, Y - rad, rad * 2, rad * 2);

            canvas.DrawLine(new Pen(Color.Blue), X - rad, Y, X - rad, Y + vpitch * scale);
            canvas.DrawLine(new Pen(Color.Blue), X + rad, Y, X + rad, Y + vpitch * scale);

            RectangleF rect = new RectangleF(X - 30, Y - 30, 60f, 60f);
            canvas.DrawImage(PCPrint.makeQR(prefix + n.ToString()), rect);

            string _text = n.ToString();
            SizeF size = canvas.MeasureString(_text, PrinterFont);
            StringFormat fmt = new StringFormat(StringFormatFlags.DirectionVertical);
            canvas.DrawString(_text, PrinterFont, Brushes.Black, X - 38, (int)(Y - size.Width / 2), fmt);

            float Y2 = Y + vpitch * scale;
            canvas.DrawEllipse(new Pen(Color.Gray), X - rad, Y2 - rad, rad * 2, rad * 2);

            canvas.DrawImage(logo, new RectangleF(X - 25, Y2 - 30, 50f, 50f / 1.62f));  //aspect ratio 1.62

            string MAC = "70-B3-D5-2F-C" + (n / 256).ToString("X") + "-" + (n & 255).ToString("X2");
            size = canvas.MeasureString(MAC, MACfont);
            canvas.DrawString(MAC, MACfont, Brushes.Black, (int)(X - size.Width / 2), Y2 + 1 * scale);

            size = canvas.MeasureString("NVLink2", PrinterFont);
            canvas.DrawString("NVLink2", PrinterFont, Brushes.Black, X - size.Width / 2, Y2 + 3 * scale);

            size = canvas.MeasureString(versionString, PrinterFont);
            canvas.DrawString(versionString, PrinterFont, Brushes.Black, X - size.Width / 2, Y2 + 5 * scale);

            X = X + hpitch * scale;
            if (++NN == labelsPerRow)
            {
                X = X - labelsPerRow * hpitch * scale;
                Y = Y + vpitch * scale * 2;
                NN = 0;
            }
        }
    }
}


