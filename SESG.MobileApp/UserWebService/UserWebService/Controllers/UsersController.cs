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
        private UserContext CreateDbContext()
        {
            UserContext context = new UserContext("name=SESG.UserWebService.Properties.Settings.SESG_DB");
            return context;
        }

        private IOrderedQueryable<Core.User> GetUsers(UserContext dbContext)
        {
            var query = dbContext.Users;
            var orderedQuery = query.OrderBy(u => u.UserName);
            return orderedQuery;
        }

        public dynamic Get(int offset, int limit)
        {
            UserContext context = CreateDbContext();
            var query = GetUsers(context);
            var filteredQuery = query.Skip(offset).Take(limit);
            var users = filteredQuery.ToList();

            IEnumerable<dynamic> jsonUsers = users.Select(u => new 
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

            return new 
            { 
                users = jsonUsers,
                count = users.Count,
                total = query.Count(),
                offset = offset,
                limit = limit
            };
        }

        [HttpGet]
        public dynamic Count()
        {
            UserContext context = CreateDbContext();
            var query = GetUsers(context);

            return new { count = query.Count() };
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
