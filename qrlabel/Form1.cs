using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Remoting.Messaging;

namespace a4label
{
    public partial class Form1 : Form
    {
        AboutBox1 aboutBox;
        int changeTime = 0;
        int pageN = 0;
        const string defaultScript =
            "// 19mm round labels on A4 sheet\r\n" +
            "// Labels-Direct.co.uk SLR19\r\n" +
           "topleft:7.64,30.78\r\n" +
            "pitch:21.0,41.74\r\n" +
            "across:9\r\n" +
            "down:6\r\n\r\n" +
            "[version]=V1.00\r\n\r\n" +
            "pen:BLUE,0.5\r\n" +
            "circle:cx,y*0.25,9\r\n" +
            "circle:cx,y*0.75,9\r\n" +
            "line:0,cy,x,cy\r\n" +
            "QRcode:cx,y*0.25,14,https://myurl.com\r\n" +
            "text:cx,y-4,[version]\r\n" +
            "text:cx,y-9,[NNNNN]\r\n";


        public Form1()
        {
            InitializeComponent();
            aboutBox = new AboutBox1();
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
            else if (MessageBox.Show("Cannot load file " + fn + "\nUsing default layout", "Problem") == DialogResult.OK)
            {
                log.Text = defaultScript;
                ReadSettings();
            }

            string printer = Properties.Settings.Default.printer;
            bool foundPrinter = false;
            foreach (string p in PrinterSettings.InstalledPrinters)
                foundPrinter |= (printer == p);

            if (!foundPrinter)
            {
                if (PrinterSettings.InstalledPrinters.Count == 0)
                {
                    MessageBox.Show("No Printers installed");
                }
                else
                {
                    PrinterSettings settings = new PrinterSettings();
                    printer = settings.PrinterName;
                    Properties.Settings.Default.printer = printer;
                    Properties.Settings.Default.Save();

                    MessageBox.Show("Using printer" + printer);
                }
            }
            printDocument1.PrinterSettings.PrinterName = printer;
            SetTitle();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            labelLayout1.setRange((long)numericUpDown1.Value, (long)numericUpDown2.Value);
            label1.Text = labelLayout1.pageFit(checkBoxRandomSN.Checked);
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
                SetTitle();
            }
        }

        private void SetTitle()
        {
            this.Text = Properties.Settings.Default.filename + "  :  " + Properties.Settings.Default.printer;
        }

        private void ReadSettings()
        {
            labelLayout1.readSettings(log.Text);
            setLabelErrors();

            listBoxHistory.Items.Clear();
            if(!labelLayout1.randomSerialNumber)
                listBoxHistory.Items.AddRange(labelLayout1.history.ToArray());

            checkBoxRandomSN.Checked = labelLayout1.randomSerialNumber;
            checkBoxRandomSN_CheckedChanged(null, null);
            textBoxVersion.Text = labelLayout1.lastPrintedVersion;
        }


        private void setLabelErrors()
        {
            labelErrors.Text = string.Join("\r\n", labelLayout1.errors);
            if ((labelLayout1.errors.Count > 0) && (labelLayout1.overlaps.Count > 0))
                labelErrors.Text += "\r\n";
            labelErrors.Text += string.Join("\r\n", labelLayout1.overlaps);

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


        private void SaveSettings()
        {
            string old = log.Text;
            if (!log.Text.EndsWith("\r\n"))
                log.Text += "\r\n";
            if (textBoxVersion.Text != labelLayout1.version())
                log.Text += "[version]=" + labelLayout1.version();
            if (log.Text != old)
                Save();
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (changeTime > 0)
                if (--changeTime == 0)
                {
                    ReadSettings();
                    labelLayout1.Invalidate();
                }
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
                SetTitle();
            }
        }


        private void buttonPrint_Click(object sender, EventArgs e)
        {
            printDocument1.Print();
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


        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            pageN++;
            decimal start = numericUpDown1.Value;

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


        private void TextBoxVersion_TextChanged(object sender, EventArgs e)
        {
            labelLayout1.setVersion(textBoxVersion.Text);
        }


        private void ButtonPrint_Click(object sender, EventArgs e)
        {
            printDocument1.Print();
        }


        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutBox.Show();
            aboutBox.BringToFront();
        }

        private void checkBoxRandomSN_CheckedChanged(object sender, EventArgs e)
        {
//            numericUpDown1.Maximum = labelLayout1.validSerialMax;
//            numericUpDown1.Minimum = labelLayout1.validSerialMin;
            if(checkBoxRandomSN.Checked)
            {
                nextRandom();
                numericUpDown1.Enabled = false;
            }
            else
            {
                numericUpDown1.Enabled = true;
                decimal n = labelLayout1.lastPrintedSerno + 1;
                numericUpDown1.Maximum = Math.Max(labelLayout1.validSerialMax, n);
                numericUpDown1.Minimum = Math.Min(labelLayout1.validSerialMin, n);
                numericUpDown1.Value = n;
            }
        }

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

            decimal range = labelLayout1.validSerialMax - labelLayout1.validSerialMin;

            return (randomLong % range) + labelLayout1.validSerialMin;
        }

        private void labelLayout1_Click(object sender, EventArgs e)
        {
            if (checkBoxRandomSN.Checked)
             nextRandom();
        }

        private void nextRandom()
        {
            decimal n = makeRandom();
            numericUpDown1.Maximum = n;
            numericUpDown1.Minimum = n;
            numericUpDown1.Value = n;
        }
    }


}
