using System;

namespace SESG.UserWebService.Core
{
    public class User
    {
        public virtual long UserID { get; set; }
        public virtual string UserName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string CellNumber { get; set; }
        public virtual string Company { get; set; }
        public virtual string Website { get; set; }
        public virtual File ProfilePicture { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime? DateModified { get; set; }
    }
}
