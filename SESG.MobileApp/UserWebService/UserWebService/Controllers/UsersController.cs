using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SESG.UserWebService.Data;
using SESG.UserWebService.Core;

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
