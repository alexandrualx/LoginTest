using LoginTest;
using LoginTest.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace LoginTest.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {

        [TestMethod()]
        public void GeneratePasswordTest()
        {
            HomeController H = new HomeController();

            int PasswordLength = 5;

            string Password = H.GeneratePassword(PasswordLength);

            Assert.IsTrue(Password.Length > 0);
        }

        [TestMethod]
        public void GetRemainingTimeTest()
        {
            HomeController H = new HomeController();

            int UserID = 0;

            using (Models.DBEntities db = new Models.DBEntities())
            {
                Models.LOGIN_LOG L = db.LOGIN_LOG.OrderByDescending(x => x.LogID).FirstOrDefault();
                if (L != null)
                {
                    UserID = L.UserID;
                }
            }

            JsonResult result = H.GetRemainingTime(UserID);
            IDictionary<string, object> data = new RouteValueDictionary(result.Data);
            Assert.AreEqual("OK", data["Result"]);
        }

        [TestMethod]
        public void CheckPasswordTest()
        {
            HomeController H = new HomeController();

            int UserID = 0;
            string Password = "";

            using (Models.DBEntities db = new Models.DBEntities())
            {
                Models.LOGIN_LOG L = db.LOGIN_LOG.OrderByDescending(x => x.LogID).FirstOrDefault();
                if (L != null)
                {
                    UserID = L.UserID;
                    Password = L.Password;
                }
            }

            JsonResult result = H.CheckPassword(UserID, Password);
            IDictionary<string, object> data = new RouteValueDictionary(result.Data);
            Assert.AreEqual("OK", data["Result"]);
        }

    }
}
