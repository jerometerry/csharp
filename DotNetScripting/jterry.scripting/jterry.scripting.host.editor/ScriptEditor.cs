using System;
using System.IO;
using System.Windows.Forms;

namespace jterry.scripting.host.editor
{
    public partial class ScriptEditor : Form
    {
        private ScriptHost _host;

        public ScriptHost Host
        {
            get { return _host; }
        }

        public ScriptEditor()
            : this(new ScriptHost())
        {
        }

        public ScriptEditor(ScriptHost host)
        {
            _host = host;
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
            _host.RegisterVariable("scriptEditor", this);
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
            var res = _host.Execute(script);
            _output.Text = _host.GetOutput();
        }

        private void ClearOutput()
        {
            _output.Clear();
            _host.ClearOutput();
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
    }
}
