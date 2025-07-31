using System; // Provides base types and fundamental classes
using System.Drawing; // For drawing graphics, colors, etc.
using System.Drawing.Printing; // For printing support
using System.Windows.Forms; // For Windows Forms UI components
using System.IO; // For file input/output operations
using System.Runtime.Remoting.Messaging; // (Unused in this file, can be removed)
using System.Collections.Generic; // For generic collections like List<T>
using System.Linq; // For LINQ operations (Skip, Take, etc.)

namespace a4label
{
    // Main form for the label printing application
    public partial class Form1 : Form
    {
        // Instance of the About dialog box
        AboutBox1 aboutBox;

        // Used for delayed settings update (timer)
        int changeTime = 0;

        // Tracks the current page number during printing/preview
        int pageN = 0;

        // Default label layout script (used if no file is loaded)
        const string defaultScript =
            "// 19mm round labels on A4 sheet\r\n" +
            "// Labels-Direct.co.uk SLR19\r\n" +
            "topleft:7.64,30.78\r\n" + // Top-left position of the label grid
            "pitch:21.0,41.74\r\n" + // Distance between labels (mm)
            "across:9\r\n" + // Number of labels horizontally
            "down:6\r\n\r\n" + // Number of labels vertically
            "[version]=V1.00\r\n\r\n" + // Version info
            "pen:BLUE,0.5\r\n" + // Pen color and thickness
            "circle:cx,y*0.25,9\r\n" + // Draw circle at calculated position
            "circle:cx,y*0.75,9\r\n" +
            "line:0,cy,x,cy\r\n" + // Draw line
            "QRcode:cx,y*0.25,14,https://myurl.com\r\n" + // Draw QR code
            "text:cx,y-4,[version]\r\n" + // Draw version text
            "text:cx,y-9,[NNNNN]\r\n"; // Draw serial number

        // Stores unique random serial numbers for random mode
        private List<decimal> randomSerials = new List<decimal>();

        // Tracks which random serial number to use next
        private int randomSerialIndex = 0;

        // Print preview dialog for showing print layout before printing
        private PrintPreviewDialog printPreviewDialog1 = new PrintPreviewDialog
        {
            AutoScrollMargin = new Size(0, 0),
            AutoScrollMinSize = new Size(0, 0),
            ClientSize = new Size(400, 300),
            Enabled = true,
            ShowIcon = false,
            UseAntiAlias = true
        };

        // Constructor: initializes the form and about box
        public Form1()
        {
            InitializeComponent(); // Sets up UI components
            aboutBox = new AboutBox1(); // Creates About dialog instance
        }

        // Handles form load event: sets up working directory, loads layout, and printer
        private void Form1_Load(object sender, EventArgs e)
        {
            // Get path to user's Documents\label_layout
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\label_layout";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path); // Create if missing

            // Set working directory and file dialog paths
            Directory.SetCurrentDirectory(path);
            openFileDialog1.InitialDirectory = path;
            saveFileDialog1.InitialDirectory = path;

            // Try to load last used layout file
            string fn = Properties.Settings.Default.filename;
            if (File.Exists(fn))
            {
                loadLog(fn); // Load file
            }
            else if (MessageBox.Show("Cannot load file " + fn + "\nUsing default layout", "Problem") == DialogResult.OK)
            {
                log.Text = defaultScript; // Use default if missing
                ReadSettings();
            }

            // Set up printer
            string printer = Properties.Settings.Default.printer;
            bool foundPrinter = false;
            foreach (string p in PrinterSettings.InstalledPrinters)
                foundPrinter |= (printer == p);

            // If printer not found, use default or show error
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
            SetTitle(); // Update window title
        }

        // Handles changes to serial number range controls
        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            // Update label layout with new range
            labelLayout1.setRange((long)numericUpDown1.Value, (long)numericUpDown2.Value);

            // Update page fit info
            label1.Text = labelLayout1.pageFit(checkBoxRandomSN.Checked);

            // Enable/disable print button based on validity
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
            setLabelErrors(); // Show any layout errors
        }

        // Handles opening a layout file
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

        // Updates the window title with current file and printer
        private void SetTitle()
        {
            this.Text = Properties.Settings.Default.filename + "  :  " + Properties.Settings.Default.printer;
        }

        // Reads settings from the layout script and updates UI
        private void ReadSettings()
        {
            labelLayout1.readSettings(log.Text);
            setLabelErrors();

            listBoxHistory.Items.Clear();
            if (!labelLayout1.randomSerialNumber)
                listBoxHistory.Items.AddRange(labelLayout1.history.ToArray());

            checkBoxRandomSN.Checked = labelLayout1.randomSerialNumber;
            checkBoxRandomSN_CheckedChanged(null, null);
            textBoxVersion.Text = labelLayout1.lastPrintedVersion;
        }

        // Displays layout errors and overlaps in the UI
        private void setLabelErrors()
        {
            labelErrors.Text = string.Join("\r\n", labelLayout1.errors);
            if ((labelLayout1.errors.Count > 0) && (labelLayout1.overlaps.Count > 0))
                labelErrors.Text += "\r\n";
            labelErrors.Text += string.Join("\r\n", labelLayout1.overlaps);
        }

        // Loads a layout script from file
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

        // Handles Save button click
        private void buttonSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        // Saves the current layout script to file
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

        // Handles Save menu item click
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

        // Saves settings if version or layout changed
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

        // Timer tick event for delayed settings update
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (changeTime > 0)
                if (--changeTime == 0)
                {
                    ReadSettings();
                    labelLayout1.Invalidate();
                }
        }

        // Handles changes to the layout script textbox
        private void log_TextChanged(object sender, EventArgs e)
        {
            if (checkBoxUnlock.Checked)
                changeTime = 30; // Start timer for delayed update
        }

        // Handles unlock checkbox for editing layout script
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            log.ReadOnly = !checkBoxUnlock.Checked;
        }

        // Handles printer selection from menu
        private void PrintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = printDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                printDocument1.PrinterSettings.PrinterName = printDialog1.PrinterSettings.PrinterName;
                printDocument1.PrinterSettings.Copies = printDialog1.PrinterSettings.Copies;
                Properties.Settings.Default.printer = printDialog1.PrinterSettings.PrinterName;
                Properties.Settings.Default.Save();
                SetTitle();
            }
        }

        // Handles Print button click
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            printDocument1.Print();
        }

        // Handles page settings query (not used here)
        private void printDocument1_QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
        {
            // No custom page settings
        }

        // Handles start of printing (records history if printing to printer)
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

            // Generate random serials if needed
            if (checkBoxRandomSN.Checked)
            {
                int totalLabels = (int)numericUpDown2.Value;
                GenerateUniqueRandomSerials(totalLabels);
            }
        }

        // Handles printing each page (and preview)
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

            // Print random or sequential serials
            if (checkBoxRandomSN.Checked)
            {
                // Get serials for this page
                var pageSerials = randomSerials.Skip(randomSerialIndex).Take(n).ToList();
                labelLayout1.drawPage(pageSerials, e.Graphics);
                randomSerialIndex += pageSerials.Count;
            }
            else
            {
                labelLayout1.drawPage(start, n, e.Graphics);
            }
        }

        // Handles version textbox change
        private void TextBoxVersion_TextChanged(object sender, EventArgs e)
        {
            labelLayout1.setVersion(textBoxVersion.Text);
        }

        // Handles Print button click (duplicate handler)
        private void ButtonPrint_Click(object sender, EventArgs e)
        {
            printDocument1.Print();
        }

        // Handles Exit menu item click
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Handles Help menu item click (shows About dialog)
        private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutBox.Show();
            aboutBox.BringToFront();
        }

        // Handles random serial number checkbox change
        private void checkBoxRandomSN_CheckedChanged(object sender, EventArgs e)
        {
            // numericUpDown1.Maximum = labelLayout1.validSerialMax;
            // numericUpDown1.Minimum = labelLayout1.validSerialMin;
            if (checkBoxRandomSN.Checked)
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

            decimal range = labelLayout1.validSerialMax - labelLayout1.validSerialMin;

            return (randomLong % range) + labelLayout1.validSerialMin;
        }

        // Handles label layout click (for random serial number mode)
        private void labelLayout1_Click(object sender, EventArgs e)
        {
            if (checkBoxRandomSN.Checked)
                nextRandom();
        }

        // Sets next random serial number in the UI
        private void nextRandom()
        {
            decimal n = makeRandom();
            numericUpDown1.Maximum = n;
            numericUpDown1.Minimum = n;
            numericUpDown1.Value = n;
        }

        // Generates a list of unique random serial numbers for printing
        private void GenerateUniqueRandomSerials(int count)
        {
            randomSerials.Clear();
            randomSerialIndex = 0;
            HashSet<decimal> used = new HashSet<decimal>();
            decimal min = labelLayout1.validSerialMin;
            decimal max = labelLayout1.validSerialMax;
            Random rng = new Random();

            int tries = 0;
            while (randomSerials.Count < count && tries < count * 10)
            {
                decimal range = max - min + 1;
                byte[] buffer = new byte[8];
                rng.NextBytes(buffer);
                decimal candidate = (decimal)(BitConverter.ToUInt64(buffer, 0) % (ulong)range) + min;
                if (!used.Contains(candidate))
                {
                    used.Add(candidate);
                    randomSerials.Add(candidate);
                }
                tries++;
            }

            if (randomSerials.Count < count)
                MessageBox.Show("Could not generate enough unique random serial numbers in the given range.");
        }

        // Handles Print Preview menu item click
        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowPrintPreview();
        }

        // Shows the print preview dialog
        private void ShowPrintPreview()
        {
            try
            {
                // Reset page counter
                pageN = 0;

                // Generate random serials if needed
                if (checkBoxRandomSN.Checked)
                {
                    int totalLabels = (int)numericUpDown2.Value;
                    GenerateUniqueRandomSerials(totalLabels);
                    randomSerialIndex = 0;  // Reset index for preview
                }

                // Configure and show preview
                printPreviewDialog1.Document = printDocument1;
                printPreviewDialog1.WindowState = FormWindowState.Maximized;
                printPreviewDialog1.PrintPreviewControl.Zoom = 1.0;  // Set initial zoom to 100%
                printPreviewDialog1.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Print preview failed: " + ex.Message, "Preview Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handles Print Preview button click (if present)
        private void buttonPreview_Click(object sender, EventArgs e)
        {
            ShowPrintPreview();
        }
    }
}