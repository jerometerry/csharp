using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jterry.scripting.api
{
    public class Track
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual Genre Genre { get; set; }
        public virtual Album Album { get; set; }
        public virtual MediaType MediaType { get; set; }
        public virtual string Composer { get; set; }
        public virtual long Milliseconds { get; set; }
        public virtual long Bytes { get; set; }
        public virtual double UnitPrice { get; set; }
    }
}
