using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using SESG.UserWebService.Data;

namespace SESG.UserWebService.Controllers
{
    public class UsersController : ApiController
    {
        public IEnumerable<dynamic> Get(int offset, int limit)
        {
            UserContext context = new UserContext("name=SESG.UserWebService.Properties.Settings.SESG_DB");
            var query = context.Users;
            var orderedQuery = query.OrderBy(u => u.UserName);
            var filteredQuery = orderedQuery.Skip(offset).Take(limit);
            var users = filteredQuery.ToList();

            return users.Select(u => new 
            { 
                id = u.UserID, 
                username = u.UserName, 
                firstName = u.FirstName, 
                lastName = u.LastName,
                company = u.Company,
                avatar = GetAbsoluteFileUrl(u.ProfilePicture),
                phone = u.PhoneNumber,
                cell = u.CellNumber,
                email = u.Email,
                website = u.Website
            });
        }

        private string GetAbsoluteFileUrl(Core.File file)
        {
            if (file == null)
            {
                return string.Empty;
            }

            string path = file.Path;

            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            string virtualPath = "/Content";
            if (!path.StartsWith("/"))
            {
                virtualPath += "/";
            }

            virtualPath += path;

            string authority = this.Request.RequestUri.GetLeftPart(UriPartial.Authority);
            string iisVirtualPath = VirtualPathUtility.ToAbsolute("~");
            string result = authority + iisVirtualPath + virtualPath;
            return result;
        }
    }
}
