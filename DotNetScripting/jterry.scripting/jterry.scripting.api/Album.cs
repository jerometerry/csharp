using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jterry.scripting.api
{
    public class Album
    {
        public virtual long Id { get; set; }
        public virtual string Title { get; set; }
        public virtual Artist Artist { get; set; }
    }
}
