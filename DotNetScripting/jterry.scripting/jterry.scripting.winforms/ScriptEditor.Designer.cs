namespace jterry.scripting.winforms
{
    partial class ScriptEditor
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
            this._scriptEditor = new System.Windows.Forms.RichTextBox();
            this._miSaveScript = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._miClearScript = new System.Windows.Forms.ToolStripMenuItem();
            this._miClearOutput = new System.Windows.Forms.ToolStripMenuItem();
            this._saveScriptDlg = new System.Windows.Forms.SaveFileDialog();
            this._miLoadScript = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._miRunScript = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._output = new System.Windows.Forms.RichTextBox();
            this._btnRunScript = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._openScriptDlg = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _scriptEditor
            // 
            this._scriptEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this._scriptEditor.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._scriptEditor.Location = new System.Drawing.Point(0, 0);
            this._scriptEditor.Name = "_scriptEditor";
            this._scriptEditor.Size = new System.Drawing.Size(760, 239);
            this._scriptEditor.TabIndex = 3;
            this._scriptEditor.Text = "";
            // 
            // _miSaveScript
            // 
            this._miSaveScript.Name = "_miSaveScript";
            this._miSaveScript.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this._miSaveScript.Size = new System.Drawing.Size(185, 22);
            this._miSaveScript.Text = "Save Script";
            this._miSaveScript.Click += new System.EventHandler(this._miSaveScript_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(182, 6);
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
            // _miLoadScript
            // 
            this._miLoadScript.Name = "_miLoadScript";
            this._miLoadScript.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this._miLoadScript.Size = new System.Drawing.Size(185, 22);
            this._miLoadScript.Text = "Load Script";
            this._miLoadScript.Click += new System.EventHandler(this._miLoadScript_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(182, 6);
            // 
            // _miRunScript
            // 
            this._miRunScript.Name = "_miRunScript";
            this._miRunScript.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this._miRunScript.Size = new System.Drawing.Size(185, 22);
            this._miRunScript.Text = "Run Script";
            this._miRunScript.Click += new System.EventHandler(this._miRunScript_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 41);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._scriptEditor);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._output);
            this.splitContainer1.Size = new System.Drawing.Size(760, 480);
            this.splitContainer1.SplitterDistance = 239;
            this.splitContainer1.TabIndex = 7;
            // 
            // _output
            // 
            this._output.Dock = System.Windows.Forms.DockStyle.Fill;
            this._output.Location = new System.Drawing.Point(0, 0);
            this._output.Name = "_output";
            this._output.ReadOnly = true;
            this._output.Size = new System.Drawing.Size(760, 237);
            this._output.TabIndex = 0;
            this._output.Text = "";
            // 
            // _btnRunScript
            // 
            this._btnRunScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._btnRunScript.Location = new System.Drawing.Point(12, 527);
            this._btnRunScript.Name = "_btnRunScript";
            this._btnRunScript.Size = new System.Drawing.Size(75, 23);
            this._btnRunScript.TabIndex = 6;
            this._btnRunScript.Text = "Run";
            this._btnRunScript.UseVisualStyleBackColor = true;
            this._btnRunScript.Click += new System.EventHandler(this._btnRunScript_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._miRunScript,
            this.toolStripSeparator2,
            this._miLoadScript,
            this._miSaveScript,
            this.toolStripSeparator1,
            this._miClearScript,
            this._miClearOutput});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // _openScriptDlg
            // 
            this._openScriptDlg.Filter = "Iron Python Files|*.py|All Files|*.*";
            // 
            // ScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this._btnRunScript);
            this.Controls.Add(this.menuStrip1);
            this.Name = "ScriptEditor";
            this.Text = "Script Editor";
            this.Load += new System.EventHandler(this.ScriptEditor_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox _scriptEditor;
        private System.Windows.Forms.ToolStripMenuItem _miSaveScript;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem _miClearScript;
        private System.Windows.Forms.ToolStripMenuItem _miClearOutput;
        private System.Windows.Forms.SaveFileDialog _saveScriptDlg;
        private System.Windows.Forms.ToolStripMenuItem _miLoadScript;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem _miRunScript;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox _output;
        private System.Windows.Forms.Button _btnRunScript;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog _openScriptDlg;
    }
}