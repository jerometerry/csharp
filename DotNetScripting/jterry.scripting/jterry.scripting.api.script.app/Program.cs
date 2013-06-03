using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using jterry.scripting.host;
using jterry.scripting.host.editor;
using jterry.scripting.api;

namespace jterry.scripting.api.script.app
{
    static class Program
    {
        static ScriptHost _scriptHost;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _scriptHost = new ScriptHost();
            var dlg = new ScriptEditor();
            dlg.Script = LoadDefaultScript();
            dlg.ScriptHost = _scriptHost;
            IFactory factory = new Factory();
            dlg.ScriptHost.RegisterVariable("factory", factory);

            Application.Run(dlg);
        }

        private static string LoadDefaultScript()
        {
            string file = Properties.Settings.Default.DefaultScript;
            string script = System.IO.File.ReadAllText(file);
            return script;
        }
    }
}
