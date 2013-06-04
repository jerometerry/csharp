using System;
using System.IO;
using System.Web;
using jterry.scripting.host;

namespace jterry.scripting.web
{
    public partial class ScriptEditor : System.Web.UI.Page
    {
        ScriptHost _scriptHost;

        protected void Page_Load(object sender, EventArgs e)
        {
            CreateScriptHost();

            if (this.IsPostBack == false)
            {
                LoadDefaultScript();
            }
        }

        private void CreateScriptHost()
        {
            var app = HttpContext.Current.ApplicationInstance as Global;
            _scriptHost = app.ScriptHost;
        }

        private void LoadDefaultScript()
        {
            string file = Properties.Settings.Default.DefaultScript;
            string fullPath = Server.MapPath(file);
            string script = File.ReadAllText(fullPath);
            _script.Text = script;
        }

        protected void _btnRunScript_Click(object sender, EventArgs e)
        {
            RunScript();
        }

        private void RunScript()
        {
            _output.Text = null;

            try
            {
                _scriptHost.Execute(_script.Text);
                _output.Text = _scriptHost.OutputRedirector.Text;
            }
            catch (Exception ex)
            {
                _output.Text = ex.ToString();
            }
        }
    }
}