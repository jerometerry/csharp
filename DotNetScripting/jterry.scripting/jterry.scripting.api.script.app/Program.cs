using System;
using System.Windows.Forms;
using jterry.scripting.host;
using jterry.scripting.host.editor;

namespace jterry.scripting.api.script.app
{
    static class Program
    {
        static ScriptHost _scriptHost;
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(Initialize());
        }

        private static Form Initialize()
        {
            var host = CreateScriptHost();
            var dlg = CreateScriptEditor();
            var factory = CreateFactory();
            RegisterScriptVariable("factory", factory);
            return dlg;
        }

        private static ScriptHost CreateScriptHost()
        {
            _scriptHost = new ScriptHost();
            return _scriptHost;
        }

        private static ScriptEditor CreateScriptEditor()
        {
            var dlg = new ScriptEditor();
            dlg.Script = LoadDefaultScript();
            dlg.ScriptHost = _scriptHost;
            return dlg;
        }

        private static IFactory CreateFactory()
        {
            IFactory factory = new Factory();
            return factory;
        }

        private static void RegisterScriptVariable(string name, object value)
        {
            _scriptHost.RegisterVariable(name, value);
        }

        private static string LoadDefaultScript()
        {
            string file = Properties.Settings.Default.DefaultScript;
            string script = System.IO.File.ReadAllText(file);
            return script;
        }
    }
}
