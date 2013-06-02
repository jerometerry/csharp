using System;

namespace jterry.scripting.host
{
    public class OutputEventArgs : EventArgs
    {
        public string Value
        {
            get;
            private set;
        }

        public OutputEventArgs(string value)
        {
            this.Value = value;
        }
    }
}
