using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoginTest.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            Models.Home model = new Models.Home();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(Models.Home model, string UserID = "")
        {
            if (UserID != "")
            {
                if (IsNumeric(UserID) == true)
                {
                    model.UserID = UserID;

                    int PasswordLength = 5;

                    string Password = GeneratePassword(PasswordLength);

                    if (Password != "")
                    {
                        using (Models.DBEntities db = new Models.DBEntities())
                        {
                            Models.LOGIN_LOG L = new Models.LOGIN_LOG();

                            L.UserID = Convert.ToInt32(UserID);
                            L.Password = Password.Trim();
                            L.DateTime = DateTime.Now;

                            db.LOGIN_LOG.Add(L);

                            if (db.SaveChanges() != 0)
                            {
                                model.Password = Password;
                                return View(model);
                            }
                            else
                            {
                                model.Message = "Password save error";
                                return View(model);
                            }

                        }
                    }
                    else
                    {
                        model.Message = "Password generation error";
                        return View(model);
                    }

                }
                else
                {
                    model.Message = "Incorrect User ID";
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        public string GeneratePassword(int PasswordLength)
        {
            string Password = "";

            Random r = new Random();

            for (int i = 1; i <= PasswordLength; i++)
            {
                if ((i % 2) == 1)
                {
                    int RandomValue = (int)r.Next(0, 9);
                    Password += RandomValue.ToString();
                }
                else
                {
                    char RandomValue = (char)r.Next('A', 'Z');
                    Password += RandomValue.ToString();
                }
            }

            return Password;
        }

        [HttpPost]
        public JsonResult GetRemainingTime(int UserID)
        {
            try
            {
                int RemainingTime = 0;

                using (Models.DBEntities db = new Models.DBEntities())
                {
                    DateTime CurrentDateTime = DateTime.Now;

                    DateTime? DBDateTime = db.LOGIN_LOG.Where(x => x.UserID == UserID).OrderByDescending(x => x.LogID).Select(x => x.DateTime).FirstOrDefault();

                    if (DBDateTime != null)
                    {
                        int ElapsedTime = Convert.ToInt32(DateTime.Now.Subtract(Convert.ToDateTime(DBDateTime)).TotalSeconds);
                        if (ElapsedTime >= 0)
                        {
                            RemainingTime = Convert.ToInt32(30 - ElapsedTime);
                        }
                    }

                }

                return Json(new { Result = "OK", RemainingTime = RemainingTime });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CheckPassword(int UserID, string Password)
        {
            try
            {
                using (Models.DBEntities db = new Models.DBEntities())
                {
                    Models.LOGIN_LOG L = db.LOGIN_LOG.Where(x => x.UserID == UserID).OrderByDescending(x => x.LogID).FirstOrDefault();

                    if (L != null)
                    {
                        if (L.Password.Trim() == Password.Trim())
                        {
                            DateTime CurrentDateTime = DateTime.Now;

                            DateTime? DBDateTime = db.LOGIN_LOG.Where(x => x.UserID == UserID).OrderByDescending(x => x.LogID).Select(x => x.DateTime).FirstOrDefault();

                            if (DBDateTime != null)
                            {
                                int ElapsedTime = Convert.ToInt32(DateTime.Now.Subtract(Convert.ToDateTime(DBDateTime)).TotalSeconds);
                                if (ElapsedTime >= 0 && ElapsedTime <= 30)
                                {
                                    return Json(new { Result = "OK", ValidPassword = true });
                                }
                                else
                                {
                                    return Json(new { Result = "OK", ValidPassword = false });
                                }
                            }

                        }
                        else
                        {
                            return Json(new { Result = "OK", ValidPassword = false });
                        }
                    }

                }
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        private bool IsNumeric(string value)
        {
            return value.All(char.IsNumber);
        }

    }
}