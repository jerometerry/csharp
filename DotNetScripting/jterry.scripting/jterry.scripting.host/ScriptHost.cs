using System;
using System.IO;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace jterry.scripting.host
{
    public class ScriptHost
    {
        ScriptEngine m_engine;
        ScriptScope m_scope;
        OutputRedirector m_outputRedirector;

        public OutputRedirector OutputRedirector
        {
            get
            {
                return m_outputRedirector;
            }
        }

        private static ScriptHost _instance;

        public static ScriptHost Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ScriptHost();
                return _instance;
            }
        }

        private ScriptHost()
        {
            m_engine = Python.CreateEngine();
            m_scope = m_engine.CreateScope();
            m_outputRedirector = new OutputRedirector();
            m_engine.Runtime.IO.RedirectToConsole();
            Console.SetOut(TextWriter.Synchronized(m_outputRedirector));
        }

        public void RegisterVariable(string name, object value)
        {
            m_scope.SetVariable(name, value);
        }

        public dynamic Execute(string expression)
        {
            var source = m_engine.CreateScriptSourceFromString(expression, SourceCodeKind.Statements);
            var compiled = source.Compile();

            // Executes in the scope of Python

            try
            {
                return compiled.Execute(m_scope);
            }
            catch (Exception ex)
            {
                m_outputRedirector.Write(ex.ToString());
                return null;
            }
        }
    }
}
