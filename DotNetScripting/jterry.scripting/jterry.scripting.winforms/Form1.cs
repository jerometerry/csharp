using System;
using System.Windows.Forms;
using jterry.scripting.api;
using jterry.scripting.host;
using jterry.scripting.host.editor;

namespace jterry.scripting.winforms
{
    public partial class Form1 : Form
    {
        ScriptHost _scriptHost;

        public Form1()
        {
            InitializeComponent();
            _scriptHost = new ScriptHost();
            IUnitOfWork uow = new ChinookContext();
            _scriptHost.RegisterVariable("unitOfWork", uow);
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
            var dlg = new TabbedScriptEditor(_scriptHost);
            dlg.AddScript(Properties.Settings.Default.DefaultScript);
            dlg.Show();
        }

        private string LoadReadme()
        {
            string file = Properties.Settings.Default.Readme;
            string readme = System.IO.File.ReadAllText(file);
            return readme;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this._readme.Rtf = LoadReadme();
        }
    }
}
