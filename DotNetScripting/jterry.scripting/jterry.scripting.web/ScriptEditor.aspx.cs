using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using jterry.scripting.host;
using jterry.scripting.api;

namespace jterry.scripting.web
{
    public partial class ScriptEditor : System.Web.UI.Page
    {
        ScriptHost _sdbScriptHost;

        protected void Page_Load(object sender, EventArgs e)
        {
            _sdbScriptHost = new ScriptHost();
            IFactory factory = new Factory();
            _sdbScriptHost.RegisterVariable("factory", factory);
        }

        protected void _btnRunScript_Click(object sender, EventArgs e)
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