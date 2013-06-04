using System;
using System.IO;
using System.Windows.Forms;

namespace jterry.scripting.host.editor
{
    public partial class ScriptEditor : Form
    {
        public ScriptHost ScriptHost
        {
            get;
            set;
        }

        public ScriptEditor()
        {
            InitializeComponent();
        }

        private void _miRunScript_Click(object sender, EventArgs e)
        {
            RunScript();
        }

        private void _miLoadScript_Click(object sender, EventArgs e)
        {
            LoadScript();
        }

        private void _miSaveScript_Click(object sender, EventArgs e)
        {
            SaveScript();
        }

        private void _miClearScript_Click(object sender, EventArgs e)
        {
            ClearOutput();
        }

        private void _miClearOutput_Click(object sender, EventArgs e)
        {
            ClearScript();
        }

        private void _btnRunScript_Click(object sender, EventArgs e)
        {
            RunScript();
        }

        private void ScriptEditor_Load(object sender, EventArgs e)
        {
            InitializeScriptHost();
        }

        private void InitializeScriptHost()
        {
            ScriptHost.Output.StringWritten += new OutputEventHandler(output_StringWritten);
            ScriptHost.RegisterVariable("scriptEditor", this);
        }

        private void output_StringWritten(object sender, OutputEventArgs e)
        {
            _output.AppendText(e.Value);
        }

        public string Script
        {
            get
            {
                return this._scriptEditor.Text;
            }
            set
            {
                this._scriptEditor.Text = value;
            }
        }

        private void RunScript()
        {
            ClearOutput();
            string script = this.Script;
            var res = ScriptHost.Execute(script);
        }

        private void ClearOutput()
        {
            _output.Clear();
            ScriptHost.Output.Clear();
        }

        private void ClearScript()
        {
            _scriptEditor.Clear();
        }

        private void SaveScript()
        {
            if (_saveScriptDlg.ShowDialog() == DialogResult.OK)
            {
                string file = _saveScriptDlg.FileName;
                File.WriteAllText(file, _scriptEditor.Text);
            }
        }

        private void LoadScript()
        {
            if (_openScriptDlg.ShowDialog() == DialogResult.OK)
            {
                string file = _openScriptDlg.FileName;
                var lines = File.ReadAllLines(file);
                _scriptEditor.Lines = lines;
            }
        }

        private void ScriptEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            ScriptHost.Output.StringWritten -= new OutputEventHandler(output_StringWritten);
        }
    }
}
