using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.IO;

namespace a4label
{
    public partial class Form1 : Form
    {
        int changeTime = 0;
        int pageN = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            PrintPreviewDialog printPrvDlg = new PrintPreviewDialog();
            printPrvDlg.Width = 600;
            printPrvDlg.Height = 1000;
            printPrvDlg.Document = printDocument1;

            if (printPrvDlg.ShowDialog() == DialogResult.OK)
            {
           //     printDocument1.Print();
            }
        }





        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            pageN++;
            int start = (int)numericUpDown1.Value;

            int labelsPerPage = labelLayout1.repeatAcross * labelLayout1.repeatDown;

            int n = (int)numericUpDown2.Value;

            start = start + (pageN - 1) * labelsPerPage;
            n = n - (pageN - 1) * labelsPerPage;

            e.HasMorePages = false;

            if (n > labelsPerPage)
            {
                n = labelsPerPage;
                e.HasMorePages = true;
            }

            labelLayout1.drawPage(start, n, e.Graphics);
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            labelLayout1.setRange((int)numericUpDown1.Value, (int)numericUpDown2.Value);
            label1.Text = labelLayout1.pageFit();
            if (labelLayout1.canPrint())
            {
                buttonPrint.Enabled = true;
                buttonPrint.Text = "Print";
                buttonPrint.ForeColor = Color.Black;
            }
            else
            {
                buttonPrint.Enabled = false;
                buttonPrint.Text = "Invalid serial number\r\nSee layout script";
                buttonPrint.ForeColor = Color.Red;

            }
            setLabelErrors();
        }



        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.filename = openFileDialog1.FileName;
                Properties.Settings.Default.Save();
                loadLog(openFileDialog1.FileName);
            }
        }



        private void ReadSettings()
        {
            labelLayout1.readSettings(log.Text);
            setLabelErrors();
        }

        private void setLabelErrors()
        {
            labelErrors.Text = string.Join("\r\n", labelLayout1.errors);
            if ((labelLayout1.errors.Count > 0) && (labelLayout1.overlaps.Count > 0))
                labelErrors.Text += "\r\n";
            labelErrors.Text += string.Join("\r\n", labelLayout1.overlaps);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\label_layout";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            Directory.SetCurrentDirectory(path);
            openFileDialog1.InitialDirectory = path;
            saveFileDialog1.InitialDirectory = path;

            string fn = Properties.Settings.Default.filename;
            if (File.Exists(fn))
            {
                loadLog(fn);
            }
            else if (MessageBox.Show("Cannot load file" + fn + "\nUsing default layout", "Problem") == DialogResult.OK)
            {
                ReadSettings();
            }

            string printer = Properties.Settings.Default.printer;
            bool foundPrinter = false;
            foreach(string p in PrinterSettings.InstalledPrinters)
                foundPrinter |= (printer == p);

            if (!foundPrinter)
            {
                if (PrinterSettings.InstalledPrinters.Count == 0)
                {
                    MessageBox.Show("No Printers installed");
                }
                else {
                    PrinterSettings settings = new PrinterSettings();
                    printer = settings.PrinterName;
                    Properties.Settings.Default.printer = printer;
                    Properties.Settings.Default.Save();

                    MessageBox.Show("Using printer" + printer);
                }
            }
            printDocument1.PrinterSettings.PrinterName = printer;
            this.Text = fn + "  :  " + printer;



        }

        private void loadLog(string filename)
        {
            try
            {
                StreamReader sr = new StreamReader(filename);
                log.Text = sr.ReadToEnd();
                sr.Close();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, filename);
            }

            ReadSettings();
            numericUpDown1.Value = labelLayout1.lastPrintedSerno + 1;
            textBoxVersion.Text = labelLayout1.lastPrintedVersion;

        }

       

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void Save()
        {
            try
            {
                StreamWriter sw = new StreamWriter(Properties.Settings.Default.filename);
                sw.Write(log.Text);
                sw.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Properties.Settings.Default.filename);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = Properties.Settings.Default.filename;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.filename = saveFileDialog1.FileName;
                Properties.Settings.Default.Save();
                Save();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ReadSettings();
            labelLayout1.Invalidate();
        }

        private void SaveSettings()
        {
            string old = log.Text;
            if (!log.Text.EndsWith("\r\n"))
                log.Text += "\r\n";
            //            if (textBoxQR.Text != labelLayout1.QRlink())
            //                log.Text += "[QRlink]=" + labelLayout1.QRlink();
            if (textBoxVersion.Text != labelLayout1.version())
                log.Text += "[version]=" + labelLayout1.version();
            if (log.Text != old)
                Save();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (changeTime > 0)
                if (--changeTime == 0)
                    button3_Click(null, null);
        }

        private void log_TextChanged(object sender, EventArgs e)
        {
            if (checkBoxUnlock.Checked)
                changeTime = 30;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            log.ReadOnly = !checkBoxUnlock.Checked;
        }

        private void PrintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = printDialog1.ShowDialog();
            if (dr== DialogResult.OK)
            {
                printDocument1.PrinterSettings.PrinterName = printDialog1.PrinterSettings.PrinterName;
                printDocument1.PrinterSettings.Copies = printDialog1.PrinterSettings.Copies;
                Properties.Settings.Default.printer = printDialog1.PrinterSettings.PrinterName;
                Properties.Settings.Default.Save();

            }
        }

        private void TextBoxVersion_TextChanged(object sender, EventArgs e)
        {
            labelLayout1.setVersion(textBoxVersion.Text);
        }
        private void printDocument1_QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
        {

        }

        private void printDocument1_BeginPrint(object sender, PrintEventArgs e)
        {
            pageN = 0;
            if (e.PrintAction == PrintAction.PrintToPrinter)
            {
                if (!log.Text.EndsWith("\r\n"))
                    log.Text += "\r\n";
                log.Text += labelLayout1.addHistory() + "\r\n";
                Save();

            }
        }
        private void Button1_Click_1(object sender, EventArgs e)
        {

        }
    }


}
