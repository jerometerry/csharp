using System;
using System.IO;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace jterry.scripting.host
{
    public class ScriptHost
    {
        ScriptEngine _engine;
        ScriptScope _scope;
        OutputRedirector _outputRedirector;

        public OutputRedirector OutputRedirector
        {
            get
            {
                return _outputRedirector;
            }
        }

        public ScriptHost()
        {
            _engine = Python.CreateEngine();
            _scope = _engine.CreateScope();
            _outputRedirector = new OutputRedirector();
            _engine.Runtime.IO.RedirectToConsole();
            Console.SetOut(TextWriter.Synchronized(_outputRedirector));
        }

        public void RegisterVariable(string name, object value)
        {
            _scope.SetVariable(name, value);
        }

        public dynamic Execute(string expression)
        {
            var source = _engine.CreateScriptSourceFromString(expression, SourceCodeKind.Statements);
            var compiled = source.Compile();

            try
            {
                return compiled.Execute(_scope);
            }
            catch (Exception ex)
            {
                _outputRedirector.Write(ex.ToString());
                return null;
            }
        }
    }
}
