using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using jterry.scripting.host;
using jterry.scripting.api;
using System.IO;

namespace jterry.scripting.web
{
    public partial class ScriptEditor : System.Web.UI.Page
    {
        ScriptHost _sdbScriptHost;

        protected void Page_Load(object sender, EventArgs e)
        {
            CreateScriptHost();
            LoadDefaultScript();
        }

        private void CreateScriptHost()
        {
            _sdbScriptHost = new ScriptHost();
            IFactory factory = new Factory();
            _sdbScriptHost.RegisterVariable("factory", factory);
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
                _sdbScriptHost.Execute(_script.Text);
                _output.Text = _sdbScriptHost.OutputRedirector.Text;
            }
            catch (Exception ex)
            {
                _output.Text = ex.ToString();
            }
        }
    }
}