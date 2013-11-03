using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SESG.UserWebService.Core;
using SESG.UserWebService.Data;

namespace SESG.UserWebService.Controllers
{
    public class UsersController : ApiController
    {
        public IEnumerable<User> Get(int offset, int limit)
        {
            UserContext context = new UserContext("name=SESG.UserWebService.Properties.Settings.SESG_DB");
            return context.Users.OrderBy(u => u.UserID).Skip(offset).Take(limit);
        }
    }
}
