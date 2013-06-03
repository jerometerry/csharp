using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using jterry.scripting.host;
using jterry.scripting.host.editor;
using jterry.scripting.api;

namespace jterry.scripting.winforms
{
    public partial class Form1 : Form
    {
        ScriptHost _scriptHost;

        public Form1()
        {
            InitializeComponent();
            _scriptHost = new ScriptHost();
        }

        private void _miScriptEditor_Click(object sender, EventArgs e)
        {
            ShowScriptEditor();
        }

        private void _btnScriptEditor_Click(object sender, EventArgs e)
        {
            ShowScriptEditor();
        }

        private void ShowScriptEditor()
        {
            var dlg = new ScriptEditor();
            dlg.Script = LoadDefaultScript();
            dlg.ScriptHost = _scriptHost;
            IFactory factory = new Factory();
            dlg.ScriptHost.RegisterVariable("factory", factory);
            dlg.Show();
        }

        private string LoadDefaultScript()
        {
            string file = Properties.Settings.Default.DefaultScript;
            string script = System.IO.File.ReadAllText(file);
            return script;
        }
    }
}
