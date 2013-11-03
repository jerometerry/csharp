using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SESG.UserWebService.Core;
using SESG.UserWebService.Data;

namespace SESG.UserWebService.Controllers
{
    public class UsersController : ApiController
    {
        public IEnumerable<dynamic> Get(int offset, int limit)
        {
            UserContext context = new UserContext("name=SESG.UserWebService.Properties.Settings.SESG_DB");
            var query = context.Users.OrderBy(u => u.UserID).Skip(offset).Take(limit);
            return query.Select(u => new 
            { 
                id = u.UserID, 
                username = u.UserName, 
                firstName = u.FirstName, 
                lastName = u.LastName,
                company = u.Company,
                profilePicture = u.ProfilePicture == null ? string.Empty : u.ProfilePicture.Path,
                phone = u.PhoneNumber,
                cell = u.CellNumber,
                email = u.Email,
                website = u.Website
            });
        }
    }
}
