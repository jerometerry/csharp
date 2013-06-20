namespace jterry.scripting.host.editor
{
    partial class TabbedScriptEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TabbedScriptEditor));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._miNewScript = new System.Windows.Forms.ToolStripMenuItem();
            this._miLoadScript = new System.Windows.Forms.ToolStripMenuItem();
            this._miSaveScript = new System.Windows.Forms.ToolStripMenuItem();
            this._miCloseScript = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._miRunScript = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._miClearScript = new System.Windows.Forms.ToolStripMenuItem();
            this._miClearOutput = new System.Windows.Forms.ToolStripMenuItem();
            this._saveScriptDlg = new System.Windows.Forms.SaveFileDialog();
            this._openScriptDlg = new System.Windows.Forms.OpenFileDialog();
            this._scriptsTabControl = new System.Windows.Forms.TabControl();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this._toolStrip = new System.Windows.Forms.ToolStrip();
            this._tsbRunScript = new System.Windows.Forms.ToolStripButton();
            this._tsbNewScript = new System.Windows.Forms.ToolStripButton();
            this._tsbLoadScript = new System.Windows.Forms.ToolStripButton();
            this._tsbSaveScript = new System.Windows.Forms.ToolStripButton();
            this._tsbCloseScript = new System.Windows.Forms.ToolStripButton();
            this._btnClearScript = new System.Windows.Forms.ToolStripButton();
            this._tsbClearOutput = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this._toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1008, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._miNewScript,
            this._miLoadScript,
            this._miSaveScript,
            this._miCloseScript,
            this.toolStripSeparator1,
            this._miRunScript,
            this.toolStripSeparator2,
            this._miClearScript,
            this._miClearOutput});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // _miNewScript
            // 
            this._miNewScript.Name = "_miNewScript";
            this._miNewScript.Size = new System.Drawing.Size(185, 22);
            this._miNewScript.Text = "New Script";
            this._miNewScript.Click += new System.EventHandler(this._miNewScript_Click);
            // 
            // _miLoadScript
            // 
            this._miLoadScript.Name = "_miLoadScript";
            this._miLoadScript.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this._miLoadScript.Size = new System.Drawing.Size(185, 22);
            this._miLoadScript.Text = "Load Script";
            this._miLoadScript.Click += new System.EventHandler(this._miLoadScript_Click);
            // 
            // _miSaveScript
            // 
            this._miSaveScript.Name = "_miSaveScript";
            this._miSaveScript.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this._miSaveScript.Size = new System.Drawing.Size(185, 22);
            this._miSaveScript.Text = "Save Script";
            this._miSaveScript.Click += new System.EventHandler(this._miSaveScript_Click);
            // 
            // _miCloseScript
            // 
            this._miCloseScript.Name = "_miCloseScript";
            this._miCloseScript.Size = new System.Drawing.Size(185, 22);
            this._miCloseScript.Text = "Close Script";
            this._miCloseScript.Click += new System.EventHandler(this._miCloseScript_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(182, 6);
            // 
            // _miRunScript
            // 
            this._miRunScript.Name = "_miRunScript";
            this._miRunScript.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this._miRunScript.Size = new System.Drawing.Size(185, 22);
            this._miRunScript.Text = "Run Script";
            this._miRunScript.Click += new System.EventHandler(this._miRunScript_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(182, 6);
            // 
            // _miClearScript
            // 
            this._miClearScript.Name = "_miClearScript";
            this._miClearScript.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this._miClearScript.Size = new System.Drawing.Size(185, 22);
            this._miClearScript.Text = "Clear Script";
            this._miClearScript.Click += new System.EventHandler(this._miClearScript_Click);
            // 
            // _miClearOutput
            // 
            this._miClearOutput.Name = "_miClearOutput";
            this._miClearOutput.Size = new System.Drawing.Size(185, 22);
            this._miClearOutput.Text = "Clear Output (ESC)";
            this._miClearOutput.Click += new System.EventHandler(this._miClearOutput_Click);
            // 
            // _saveScriptDlg
            // 
            this._saveScriptDlg.Filter = "Iron Python Files|*.py|All Files|*.*";
            // 
            // _openScriptDlg
            // 
            this._openScriptDlg.Filter = "Iron Python Files|*.py|All Files|*.*";
            // 
            // _scriptsTabControl
            // 
            this._scriptsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._scriptsTabControl.Location = new System.Drawing.Point(0, 0);
            this._scriptsTabControl.Name = "_scriptsTabControl";
            this._scriptsTabControl.SelectedIndex = 0;
            this._scriptsTabControl.Size = new System.Drawing.Size(1008, 681);
            this._scriptsTabControl.TabIndex = 6;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this._scriptsTabControl);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1008, 681);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 24);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1008, 706);
            this.toolStripContainer1.TabIndex = 7;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this._toolStrip);
            // 
            // _toolStrip
            // 
            this._toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsbRunScript,
            this._tsbNewScript,
            this._tsbLoadScript,
            this._tsbSaveScript,
            this._tsbCloseScript,
            this._btnClearScript,
            this._tsbClearOutput});
            this._toolStrip.Location = new System.Drawing.Point(3, 0);
            this._toolStrip.Name = "_toolStrip";
            this._toolStrip.Size = new System.Drawing.Size(341, 25);
            this._toolStrip.TabIndex = 0;
            this._toolStrip.Text = "Run";
            // 
            // _tsbRunScript
            // 
            this._tsbRunScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._tsbRunScript.Image = ((System.Drawing.Image)(resources.GetObject("_tsbRunScript.Image")));
            this._tsbRunScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._tsbRunScript.Name = "_tsbRunScript";
            this._tsbRunScript.Size = new System.Drawing.Size(32, 22);
            this._tsbRunScript.Text = "Run";
            this._tsbRunScript.ToolTipText = "Run Script";
            this._tsbRunScript.Click += new System.EventHandler(this._btnRunScript_Click);
            // 
            // _tsbNewScript
            // 
            this._tsbNewScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._tsbNewScript.Image = ((System.Drawing.Image)(resources.GetObject("_tsbNewScript.Image")));
            this._tsbNewScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._tsbNewScript.Name = "_tsbNewScript";
            this._tsbNewScript.Size = new System.Drawing.Size(35, 22);
            this._tsbNewScript.Text = "New";
            this._tsbNewScript.Click += new System.EventHandler(this._miNewScript_Click);
            // 
            // _tsbLoadScript
            // 
            this._tsbLoadScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._tsbLoadScript.Image = ((System.Drawing.Image)(resources.GetObject("_tsbLoadScript.Image")));
            this._tsbLoadScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._tsbLoadScript.Name = "_tsbLoadScript";
            this._tsbLoadScript.Size = new System.Drawing.Size(37, 22);
            this._tsbLoadScript.Text = "Load";
            this._tsbLoadScript.Click += new System.EventHandler(this._miLoadScript_Click);
            // 
            // _tsbSaveScript
            // 
            this._tsbSaveScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._tsbSaveScript.Image = ((System.Drawing.Image)(resources.GetObject("_tsbSaveScript.Image")));
            this._tsbSaveScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._tsbSaveScript.Name = "_tsbSaveScript";
            this._tsbSaveScript.Size = new System.Drawing.Size(35, 22);
            this._tsbSaveScript.Text = "Save";
            this._tsbSaveScript.ToolTipText = "Save Script";
            this._tsbSaveScript.Click += new System.EventHandler(this._miSaveScript_Click);
            // 
            // _tsbCloseScript
            // 
            this._tsbCloseScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._tsbCloseScript.Image = ((System.Drawing.Image)(resources.GetObject("_tsbCloseScript.Image")));
            this._tsbCloseScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._tsbCloseScript.Name = "_tsbCloseScript";
            this._tsbCloseScript.Size = new System.Drawing.Size(40, 22);
            this._tsbCloseScript.Text = "Close";
            this._tsbCloseScript.ToolTipText = "Close Script";
            this._tsbCloseScript.Click += new System.EventHandler(this._miCloseScript_Click);
            // 
            // _btnClearScript
            // 
            this._btnClearScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnClearScript.Image = ((System.Drawing.Image)(resources.GetObject("_btnClearScript.Image")));
            this._btnClearScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._btnClearScript.Name = "_btnClearScript";
            this._btnClearScript.Size = new System.Drawing.Size(71, 22);
            this._btnClearScript.Text = "Clear Script";
            this._btnClearScript.Click += new System.EventHandler(this._miClearScript_Click);
            // 
            // _tsbClearOutput
            // 
            this._tsbClearOutput.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._tsbClearOutput.Image = ((System.Drawing.Image)(resources.GetObject("_tsbClearOutput.Image")));
            this._tsbClearOutput.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._tsbClearOutput.Name = "_tsbClearOutput";
            this._tsbClearOutput.Size = new System.Drawing.Size(79, 22);
            this._tsbClearOutput.Text = "Clear Output";
            this._tsbClearOutput.Click += new System.EventHandler(this._miClearOutput_Click);
            // 
            // TabbedScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "TabbedScriptEditor";
            this.Text = "Script Editor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this._toolStrip.ResumeLayout(false);
            this._toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _miClearScript;
        private System.Windows.Forms.ToolStripMenuItem _miClearOutput;
        private System.Windows.Forms.ToolStripMenuItem _miSaveScript;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.SaveFileDialog _saveScriptDlg;
        private System.Windows.Forms.ToolStripMenuItem _miLoadScript;
        private System.Windows.Forms.OpenFileDialog _openScriptDlg;
        private System.Windows.Forms.ToolStripMenuItem _miRunScript;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.TabControl _scriptsTabControl;
        private System.Windows.Forms.ToolStripMenuItem _miNewScript;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip _toolStrip;
        private System.Windows.Forms.ToolStripButton _tsbRunScript;
        private System.Windows.Forms.ToolStripButton _tsbNewScript;
        private System.Windows.Forms.ToolStripButton _tsbLoadScript;
        private System.Windows.Forms.ToolStripButton _tsbSaveScript;
        private System.Windows.Forms.ToolStripMenuItem _miCloseScript;
        private System.Windows.Forms.ToolStripButton _tsbCloseScript;
        private System.Windows.Forms.ToolStripButton _tsbClearOutput;
        private System.Windows.Forms.ToolStripButton _btnClearScript;
    }
}

