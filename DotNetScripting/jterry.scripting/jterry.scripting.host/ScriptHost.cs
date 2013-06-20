using System;
using System.IO;
using System.Reflection;
using System.Text;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace jterry.scripting.host
{
    public class ScriptHost : IDisposable
    {
        ScriptEngine m_engine;
        ScriptScope m_scope;
        MemoryStream m_outputStream;
        StreamWriter m_outputStreamWriter;

        public ScriptHost()
        {
            m_engine = Python.CreateEngine();
            m_scope = m_engine.CreateScope();

            CreateOutputBuffer();
        }

        public void LoadAssembly(Assembly assembly)
        {
            m_engine.Runtime.LoadAssembly(assembly);
        }

        public string GetOutput()
        {
            return ReadFromStream(m_outputStream);
        }

        public void ClearOutput()
        {
            DisposeOutputBuffer();
            CreateOutputBuffer();
        }

        private void CreateOutputBuffer()
        {
            m_outputStream = new MemoryStream();
            m_outputStreamWriter = new StreamWriter(m_outputStream);
            m_engine.Runtime.IO.SetOutput(m_outputStream, m_outputStreamWriter);
        }

        private void DisposeOutputBuffer()
        {
            if (m_outputStreamWriter != null)
            {
                m_outputStreamWriter.Dispose();
                m_outputStreamWriter = null;
                m_outputStream = null;
            }
        }

        private static string ReadFromStream(MemoryStream ms)
        {
            int length = (int)ms.Length;
            var bytes = new Byte[length];
            ms.Seek(0, SeekOrigin.Begin);
            ms.Read(bytes, 0, (int)ms.Length);
            return Encoding.GetEncoding("utf-8").GetString(bytes, 0, (int)ms.Length);
        }

        public void RegisterVariable(string name, object value)
        {
            m_scope.SetVariable(name, value);
        }

        public dynamic Execute(string expression)
        {
            try
            {
                ScriptSource source = m_engine.CreateScriptSourceFromString(expression, SourceCodeKind.Statements);
                CompiledCode compiled = source.Compile();
                return compiled.Execute(m_scope);
            }
            catch (Exception ex)
            {
                m_outputStreamWriter.Write(ex.ToString());
                return null;
            }
        }

        public void Dispose()
        {
            DisposeOutputBuffer();
        }
    }
}

