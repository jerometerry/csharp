using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using jterry.scripting.api;
using jterry.scripting.host;

namespace jterry.scripting.winforms
{
    public partial class ScriptEditor : Form
    {
        ScriptHost _sdbScriptHost;

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
            CreateScriptHost();
            LoadDefaultScript();
        }

        private void CreateScriptHost()
        {
            _sdbScriptHost = ScriptHost.Instance;
            _sdbScriptHost.OutputRedirector.StringWritten += new OutputEventHandler(output_StringWritten);

            IFactory factory = new Factory();
            _sdbScriptHost.RegisterVariable("factory", factory);
            _sdbScriptHost.RegisterVariable("scriptEditor", this);
        }

        private void LoadDefaultScript()
        {
            string file = Properties.Settings.Default.DefaultScript;
            string script = System.IO.File.ReadAllText(file);
            this._scriptEditor.Text = script;
        }

        private void output_StringWritten(object sender, OutputEventArgs e)
        {
            _output.AppendText(e.Value);
        }

        private string Script
        {
            get
            {
                return this._scriptEditor.Text;
            }
        }

        private void RunScript()
        {
            ClearOutput();
            string script = this.Script;
            var res = _sdbScriptHost.Execute(script);
        }

        private void ClearOutput()
        {
            _output.Clear();
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
