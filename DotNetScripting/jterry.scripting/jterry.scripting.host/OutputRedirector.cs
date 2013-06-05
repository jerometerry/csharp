using System.IO;
using System.Text;

namespace jterry.scripting.host
{
    public class OutputRedirector : TextWriter
    {
        public event OutputEventHandler StringWritten;
        private StringBuilder _output = new StringBuilder();

        public string Text
        {
            get
            {
                return _output.ToString();
            }
        }

        public void Clear()
        {
            _output.Clear();
        }

        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }

        public override void Write(string value)
        {
            base.Write(value);
            OnTextWritten(value);
        }

        private void OnTextWritten(string txtWritten)
        {
            if (StringWritten != null)
                StringWritten(this, new OutputEventArgs(txtWritten));
            _output.Append(txtWritten);
        }
    }
}
