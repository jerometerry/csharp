using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jterry.scripting.api
{
    public class Invoice
    {
        public virtual long Id { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual DateTime? InvoiceDate { get; set; }
        public virtual string BillingAddress { get; set; }
        public virtual string BillingCity { get; set; }
        public virtual string BillingState { get; set; }
        public virtual string BillingCountry { get; set; }
        public virtual string BillingPostalCode { get; set; }
        public virtual double Total { get; set; }

        public virtual ICollection<InvoiceLine> Lines { get; set; }
    }
}
