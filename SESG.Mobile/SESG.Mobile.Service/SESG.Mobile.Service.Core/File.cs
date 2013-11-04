using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SESG.Mobile.Service.Core
{
    public class File
    {
        public virtual long FileID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Path { get; set; }
        public virtual string Type { get; set; }
        public virtual User CreatedBy { get; set; }
        public virtual string Source { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime? DateModified { get; set; }
    }
}
