using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace jterry.scripting.host.editor
{
    public partial class TabbedScriptEditor : Form
    {
        List<Script> _scripts = new List<Script>();
        ScriptHost _host;

        public ScriptHost Host
        {
            get { return _host; }
        }

        private Script SelectedScript
        {
            get
            {
                return _scripts.Where(s => this._scriptsTabControl.SelectedTab == s.TabPage).FirstOrDefault();
            }
        }

        public TabbedScriptEditor()
            : this(new ScriptHost())
        {
        }

        public TabbedScriptEditor(ScriptHost host)
        {
            _host = host;
            InitializeComponent();
        }

        private void _btnRunScript_Click(object sender, EventArgs e)
        {
            RunScript();
        }

        private void RunScript()
        {
            var script = this.SelectedScript;
            script.Run();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void _miClearOutput_Click(object sender, EventArgs e)
        {
            ClearOutput();
        }

        private void ClearOutput()
        {
            var script = this.SelectedScript;
            script.ClearOutput();
        }

        private void _miClearScript_Click(object sender, EventArgs e)
        {
            ClearScript();
        }

        private void ClearScript()
        {
            var script = this.SelectedScript;
            script.ClearScript();
        }

        private void _miSaveScript_Click(object sender, EventArgs e)
        {
            var script = this.SelectedScript;
            string path = script.Path;

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                script.SaveScript(path);
            }
            else
            {
                _saveScriptDlg.FileName = Path.GetFileName(path);

                if (_saveScriptDlg.ShowDialog() == DialogResult.OK)
                {
                    string file = _saveScriptDlg.FileName;
                    script.SaveScript(file);
                }
            }
        }

        private void _miLoadScript_Click(object sender, EventArgs e)
        {
            if (_openScriptDlg.ShowDialog() == DialogResult.OK)
            {
                string file = _openScriptDlg.FileName;
                var script = this.AddScript(file);
            }
        }

        private void _miRunScript_Click(object sender, EventArgs e)
        {
            RunScript();
        }

        private void _miNewScript_Click(object sender, EventArgs e)
        {
            var script = AddScript();
        }

        public Script AddScript(string file)
        {
            var script = AddScript();
            script.LoadScript(file);
            return script;
        }

        public Script AddScript()
        {
            var script = new Script(this._scriptsTabControl, _host);
            _scripts.Add(script);
            return script;
        }

        private void _miCloseScript_Click(object sender, EventArgs e)
        {
            var script = this.SelectedScript;
            this._scriptsTabControl.Controls.Remove(script.TabPage);
            this._scripts.Remove(script);
        }
    }
}
