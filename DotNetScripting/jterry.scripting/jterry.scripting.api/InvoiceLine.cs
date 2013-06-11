using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jterry.scripting.api
{
    public class InvoiceLine
    {
        public virtual long Id { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual double UnitPrice { get; set; }
        public virtual double Quantity { get; set; }
        public virtual Track Track { get; set; }
    }
}
