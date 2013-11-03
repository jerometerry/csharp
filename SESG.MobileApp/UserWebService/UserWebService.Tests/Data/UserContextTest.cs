using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SESG.UserWebService.Core;
using SESG.UserWebService.Data;

namespace SESG.UserWebService.Tests.Data
{
    [TestClass]
    public class UserContextTest
    {
        [TestMethod]
        public void BasicTest()
        {
            string cs = "Data Source=localhost;Initial Catalog=SESG;Integrated Security=True;MultipleActiveResultSets=True";
            UserContext context = new UserContext(cs);
            User user = context.Users.Where(r => r.UserID == 10).FirstOrDefault();
            Console.WriteLine("{0} {1}", user.UserID, user.UserName);
        }
    }
}
