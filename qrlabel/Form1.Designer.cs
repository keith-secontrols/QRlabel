using System; // Basic .NET types
using System.Drawing.Printing; // For printing support

namespace a4label
{
    // Partial class for Form1, containing designer-generated code
    partial class Form1
    {
        /// <summary>
        /// Required designer variable for managing components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Dispose method to clean up resources.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed.</param>
        protected override void Dispose(bool disposing)
        {
            // Dispose managed resources if needed
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Method that initializes all UI components and their properties.
        /// </summary>
        private void InitializeComponent()
        {
            // Initialize the container for components
            this.components = new System.ComponentModel.Container();

            // Create the print document for printing labels
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();

            // Create the Print button
            this.buttonPrint = new System.Windows.Forms.Button();

            // Numeric input for starting serial number
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();

            // Numeric input for number of labels to print
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();

            // Label for page fit/status
            this.label1 = new System.Windows.Forms.Label();

            // Label for serial number input
            this.label2 = new System.Windows.Forms.Label();

            // Label for labels-to-print input
            this.label3 = new System.Windows.Forms.Label();

            // Textbox for version input
            this.textBoxVersion = new System.Windows.Forms.TextBox();

            // Label for version textbox
            this.label5 = new System.Windows.Forms.Label();

            // Multiline textbox for layout script
            this.log = new System.Windows.Forms.TextBox();

            // Menu strip for file and help menus
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();

            // "File" menu
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            // "Open" menu item
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();

            // "Save" menu item
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            // "Print" menu item
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            // "Print Preview" menu item
            this.printPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            // "Exit" menu item
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            // "Help" menu
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            // Dialog for opening files
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();

            // Save button for layout script
            this.buttonSave = new System.Windows.Forms.Button();

            // Dialog for saving files
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();

            // Timer for delayed settings update
            this.timer1 = new System.Windows.Forms.Timer(this.components);

            // Label for displaying errors
            this.labelErrors = new System.Windows.Forms.Label();

            // Checkbox to unlock layout script editing
            this.checkBoxUnlock = new System.Windows.Forms.CheckBox();

            // Split container for dividing main UI
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();

            // Tab control for history and layout tabs
            this.tabControl1 = new System.Windows.Forms.TabControl();

            // Tab page for print history
            this.tabPage1 = new System.Windows.Forms.TabPage();

            // List box for displaying print history
            this.listBoxHistory = new System.Windows.Forms.ListBox();

            // Tab page for layout script
            this.tabPage2 = new System.Windows.Forms.TabPage();

            // Dialog for printer selection
            this.printDialog1 = new System.Windows.Forms.PrintDialog();

            // Checkbox for random serial number mode
            this.checkBoxRandomSN = new System.Windows.Forms.CheckBox();

            // Custom user control for label layout preview
            this.labelLayout1 = new a4label.LabelLayout();

            // Button for print preview
            this.buttonPreview = new System.Windows.Forms.Button();

            // Begin initialization for numeric up-down controls
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();

            // Begin initialization for menu strip
            this.menuStrip1.SuspendLayout();

            // Begin initialization for split container and its panels
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();

            // Begin initialization for tab control and its pages
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();

            // Begin form layout
            this.SuspendLayout();

            // --- Component property settings and event hookups ---

            // printDocument1: Hook up event handlers for printing
            this.printDocument1.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.printDocument1_BeginPrint);
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
            this.printDocument1.QueryPageSettings += new System.Drawing.Printing.QueryPageSettingsEventHandler(this.printDocument1_QueryPageSettings);

            // buttonPrint: Print button properties and event
            this.buttonPrint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPrint.Location = new System.Drawing.Point(295, 21);
            this.buttonPrint.Margin = new System.Windows.Forms.Padding(4);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(1187, 136);
            this.buttonPrint.TabIndex = 0;
            this.buttonPrint.Text = "Print";
            this.buttonPrint.UseVisualStyleBackColor = true;
            this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);

            // numericUpDown1: Serial number input
            this.numericUpDown1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown1.Location = new System.Drawing.Point(132, 19);
            this.numericUpDown1.Margin = new System.Windows.Forms.Padding(4);
            this.numericUpDown1.Maximum = new decimal(new int[] { 40000, 0, 0, 0 });
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(155, 28);
            this.numericUpDown1.TabIndex = 1;
            this.numericUpDown1.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);

            // numericUpDown2: Labels to print input
            this.numericUpDown2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown2.Location = new System.Drawing.Point(170, 82);
            this.numericUpDown2.Margin = new System.Windows.Forms.Padding(4);
            this.numericUpDown2.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            this.numericUpDown2.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(117, 28);
            this.numericUpDown2.TabIndex = 3;
            this.numericUpDown2.Value = new decimal(new int[] { 117, 0, 0, 0 });
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);

            // label1: Page fit/status label
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 157);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 18);
            this.label1.TabIndex = 4;
            this.label1.Text = "label1";

            // label2: Serial number label
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 37);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 18);
            this.label2.TabIndex = 5;
            this.label2.Text = "Serial Number";

            // label3: Labels to print label
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 89);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 18);
            this.label3.TabIndex = 6;
            this.label3.Text = "Labels to print";

            // textBoxVersion: Version input
            this.textBoxVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxVersion.Location = new System.Drawing.Point(199, 114);
            this.textBoxVersion.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxVersion.Name = "textBoxVersion";
            this.textBoxVersion.Size = new System.Drawing.Size(88, 28);
            this.textBoxVersion.TabIndex = 9;
            this.textBoxVersion.Text = "1.00";
            this.textBoxVersion.TextChanged += new System.EventHandler(this.TextBoxVersion_TextChanged);

            // label5: Version label
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(66, 121);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 18);
            this.label5.TabIndex = 10;
            this.label5.Text = "Version";

            // log: Multiline textbox for layout script
            this.log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.log.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.log.Location = new System.Drawing.Point(7, 68);
            this.log.Margin = new System.Windows.Forms.Padding(4);
            this.log.Multiline = true;
            this.log.Name = "log";
            this.log.ReadOnly = true;
            this.log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.log.Size = new System.Drawing.Size(735, 617);
            this.log.TabIndex = 11;
            this.log.TextChanged += new System.EventHandler(this.log_TextChanged);

            // menuStrip1: Main menu bar
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1495, 25);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";

            // openToolStripMenuItem: "File" menu
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem1,
            this.saveToolStripMenuItem,
            this.printToolStripMenuItem,
            this.printPreviewToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(37, 19);
            this.openToolStripMenuItem.Text = "File";

            // openToolStripMenuItem1: "Open" menu item
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem1.Text = "Open";
            this.openToolStripMenuItem1.Click += new System.EventHandler(this.openToolStripMenuItem_Click);

            // saveToolStripMenuItem: "Save" menu item
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);

            // printToolStripMenuItem: "Print" menu item
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.printToolStripMenuItem.Text = "Print";
            this.printToolStripMenuItem.Click += new System.EventHandler(this.PrintToolStripMenuItem_Click);

            // printPreviewToolStripMenuItem: "Print Preview" menu item
            this.printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
            this.printPreviewToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.printPreviewToolStripMenuItem.Text = "&Print Preview...";
            this.printPreviewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.printPreviewToolStripMenuItem.Click += new System.EventHandler(this.printPreviewToolStripMenuItem_Click);

            // exitToolStripMenuItem: "Exit" menu item
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);

            // helpToolStripMenuItem: "Help" menu
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 19);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.HelpToolStripMenuItem_Click);

            // openFileDialog1: File open dialog
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.RestoreDirectory = true;

            // buttonSave: Save button for layout script
            this.buttonSave.Location = new System.Drawing.Point(14, 20);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(94, 38);
            this.buttonSave.TabIndex = 13;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);

            // saveFileDialog1: File save dialog
            this.saveFileDialog1.RestoreDirectory = true;

            // timer1: Timer for delayed settings update
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);

            // labelErrors: Label for displaying errors
            this.labelErrors.AutoSize = true;
            this.labelErrors.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelErrors.ForeColor = System.Drawing.Color.Red;
            this.labelErrors.Location = new System.Drawing.Point(20, 17);
            this.labelErrors.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelErrors.Name = "labelErrors";
            this.labelErrors.Size = new System.Drawing.Size(85, 20);
            this.labelErrors.TabIndex = 17;
            this.labelErrors.Text = "labelErrors";

            // checkBoxUnlock: Checkbox to unlock layout script editing
            this.checkBoxUnlock.AutoSize = true;
            this.checkBoxUnlock.Location = new System.Drawing.Point(133, 30);
            this.checkBoxUnlock.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.checkBoxUnlock.Name = "checkBoxUnlock";
            this.checkBoxUnlock.Size = new System.Drawing.Size(74, 22);
            this.checkBoxUnlock.TabIndex = 19;
            this.checkBoxUnlock.Text = "Unlock";
            this.checkBoxUnlock.UseVisualStyleBackColor = true;
            this.checkBoxUnlock.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);

            // splitContainer1: Main split container for UI
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 199);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // Panel1: Contains tab control
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // Panel2: Contains error label and label layout preview
            this.splitContainer1.Panel2.Controls.Add(this.labelErrors);
            this.splitContainer1.Panel2.Controls.Add(this.labelLayout1);
            this.splitContainer1.Size = new System.Drawing.Size(1495, 708);
            this.splitContainer1.SplitterDistance = 754;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 20;

            // tabControl1: Tab control for history and layout
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(754, 708);
            this.tabControl1.TabIndex = 21;

            // tabPage1: History tab
            this.tabPage1.Controls.Add(this.listBoxHistory);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(746, 678);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "History";
            this.tabPage1.UseVisualStyleBackColor = true;

            // listBoxHistory: List box for print history
            this.listBoxHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxHistory.FormattingEnabled = true;
            this.listBoxHistory.ItemHeight = 17;
            this.listBoxHistory.Location = new System.Drawing.Point(3, 3);
            this.listBoxHistory.Name = "listBoxHistory";
            this.listBoxHistory.Size = new System.Drawing.Size(740, 672);
            this.listBoxHistory.TabIndex = 0;

            // tabPage2: Layout tab
            this.tabPage2.Controls.Add(this.checkBoxUnlock);
            this.tabPage2.Controls.Add(this.log);
            this.tabPage2.Controls.Add(this.buttonSave);
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(746, 678);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Layout";
            this.tabPage2.UseVisualStyleBackColor = true;

            // printDialog1: Print dialog for printer selection
            this.printDialog1.UseEXDialog = true;

            // checkBoxRandomSN: Checkbox for random serial number mode
            this.checkBoxRandomSN.AutoSize = true;
            this.checkBoxRandomSN.Location = new System.Drawing.Point(142, 53);
            this.checkBoxRandomSN.Name = "checkBoxRandomSN";
            this.checkBoxRandomSN.Size = new System.Drawing.Size(125, 22);
            this.checkBoxRandomSN.TabIndex = 21;
            this.checkBoxRandomSN.Text = "Random Serial";
            this.checkBoxRandomSN.UseVisualStyleBackColor = true;
            this.checkBoxRandomSN.CheckedChanged += new System.EventHandler(this.checkBoxRandomSN_CheckedChanged);

            // labelLayout1: Custom control for label layout preview
            this.labelLayout1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(240)))));
            this.labelLayout1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelLayout1.Location = new System.Drawing.Point(0, 0);
            this.labelLayout1.Margin = new System.Windows.Forms.Padding(4);
            this.labelLayout1.Name = "labelLayout1";
            this.labelLayout1.Size = new System.Drawing.Size(736, 708);
            this.labelLayout1.TabIndex = 18;
            this.labelLayout1.Click += new System.EventHandler(this.labelLayout1_Click);

            // buttonPreview: Button for print preview
            this.buttonPreview.Location = new System.Drawing.Point(295, 170); // Set appropriate location
            this.buttonPreview.Name = "buttonPreview";
            this.buttonPreview.Size = new System.Drawing.Size(75, 23);
            this.buttonPreview.Text = "Preview...";
            this.buttonPreview.Click += new System.EventHandler(this.buttonPreview_Click);

            // --- Form property settings ---
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1495, 920);
            this.Controls.Add(this.buttonPreview);
            this.Controls.Add(this.checkBoxRandomSN);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxVersion);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.buttonPrint);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Label Printer";
            this.Load += new System.EventHandler(this.Form1_Load);

            // End initialization for controls
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        // --- Designer fields for all controls ---
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.Button buttonPrint;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxVersion;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox log;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label labelErrors;
        private LabelLayout labelLayout1;
        private System.Windows.Forms.CheckBox checkBoxUnlock;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printPreviewToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.ListBox listBoxHistory;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxRandomSN;
        private System.Windows.Forms.Button buttonPreview;
    }
}