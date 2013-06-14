using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace jterry.scripting.host.editor
{
    public class Script
    {
        private ScriptHost _host;

        public RichTextBox ScriptEditor { get; set; }
        public RichTextBox OutputControl { get; set; }
        public SplitContainer Splitter { get; set; }
        public TabPage TabPage { get; set; }
        public TabControl TabControl { get; set; }

        private string SavedText { get; set; }
        public string Path { get; set; }

        public Script(TabControl tabControl, ScriptHost host)
        {
            _host = host;

            var tabPage = new TabPage();
            tabPage.Text = "New Script";
            tabControl.Controls.Add(tabPage);
            tabControl.SelectedTab = tabPage;

            var splitContainer = new SplitContainer();
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Name = "splitContainer1";
            splitContainer.Orientation = Orientation.Horizontal;
            splitContainer.SplitterDistance = 500;
            splitContainer.TabIndex = 4;

            tabPage.Controls.Add(splitContainer);
            tabPage.Location = new System.Drawing.Point(4, 22);
            tabPage.Name = "tabPage1";
            tabPage.Padding = new System.Windows.Forms.Padding(5);
            tabPage.Size = new System.Drawing.Size(670, 527);
            tabPage.TabIndex = 0;
            tabPage.UseVisualStyleBackColor = true;

            var scriptEditor = new RichTextBox();
            scriptEditor.Dock = DockStyle.Fill;
            scriptEditor.Font = new Font("Courier New", 12.0F, 
                FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            scriptEditor.Location = new Point(0, 0);
            scriptEditor.Name = "_scriptEditor";
            scriptEditor.Size = new Size(660, 257);
            scriptEditor.TabIndex = 3;
            scriptEditor.Text = "";
            scriptEditor.KeyPress += new KeyPressEventHandler(this._scriptEditor_KeyPress);
            scriptEditor.KeyUp += new KeyEventHandler(this._scriptEditor_KeyUp);

            var output = new RichTextBox();
            output.Dock = DockStyle.Fill;
            output.Name = "_output";
            output.ReadOnly = true;
            output.TabIndex = 0;
            output.Text = "";
            output.Font = new Font("Courier New", 12.0F,
                FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

            splitContainer.Panel1.Controls.Add(scriptEditor);
            splitContainer.Panel2.Controls.Add(output);
            
            this.TabPage = tabPage;
            this.TabControl = tabControl;
            this.ScriptEditor = scriptEditor;
            this.OutputControl = output;
        }

        private void _scriptEditor_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void _scriptEditor_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    ClearOutput();
                    break;
                default:
                    break;
            }

            CheckForModifications();
        }

        private void CheckForModifications()
        {
            string name = this.ScriptName;
            bool changed = TextChanged;
            if (changed)
                name += "*";
            this.TabPage.Text = name;
        }

        private bool TextChanged
        {
            get
            {
                int savedCount = 0;
                int editedCount = 0;

                string saved = this.SavedText;
                string edited = this.Text;

                if (saved != null)
                    savedCount = saved.Length;
                if (edited != null)
                    editedCount = edited.Length;

                if (savedCount != editedCount)
                    return true;

                bool changed = string.Compare(saved, edited) != 0;
                return changed;
            }
        }

        public string Text
        {
            get
            {
                return ScriptEditor.Text;
            }
            set
            {
                ScriptEditor.Text = value;
            }
        }

        public string Output
        {
            get
            {
                return OutputControl.Text;
            }
            set
            {
                OutputControl.Text = value;
            }
        }

        public string ScriptName
        {
            get
            {
                if (string.IsNullOrEmpty(this.Path))
                    return "New Script*";
                else
                    return System.IO.Path.GetFileName(this.Path);
            }
        }

        public string Run()
        {
            _host.ClearOutput();
            var res = _host.Execute(this.Text);
            this.Output = _host.GetOutput();
            return this.Output;
        }

        public void ClearOutput()
        {
            this.OutputControl.Clear();
        }

        public void ClearScript()
        {
            this.ScriptEditor.Clear();
            this.Path = null;
            this.ClearOutput();
            this.SavedText = null;
            this.TabPage.Text = this.ScriptName;
        }

        public void LoadScript(string file)
        {
            var text = File.ReadAllText(file);
            this.Path = file;
            this.Text = text;
            this.TabPage.Text = System.IO.Path.GetFileName(file);
            this.SavedText = text;
        }

        public void SaveScript(string file)
        {
            File.WriteAllText(file, this.Text);
            this.Path = file;
            this.SavedText = this.Text;
            this.TabPage.Text = System.IO.Path.GetFileName(file);
        }
    }
}
