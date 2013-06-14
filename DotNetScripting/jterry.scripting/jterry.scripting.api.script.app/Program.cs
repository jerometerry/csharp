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
            var dlg = CreateScriptEditor(host);
            var uow = CreateUnitOfWork();
            RegisterScriptVariable(host, "unitOfWork", uow);
            return dlg;
        }

        private static ScriptHost CreateScriptHost()
        {
            _scriptHost = new ScriptHost();
            return _scriptHost;
        }

        private static TabbedScriptEditor CreateScriptEditor(ScriptHost host)
        {
            var dlg = new TabbedScriptEditor(host);
            dlg.AddScript(Properties.Settings.Default.DefaultScript);
            return dlg;
        }

        private static IUnitOfWork CreateUnitOfWork()
        {
            IUnitOfWork uow = new ChinookContext();
            return uow;
        }

        private static void RegisterScriptVariable(ScriptHost host, string name, object value)
        {
            host.RegisterVariable(name, value);
        }
    }
}
