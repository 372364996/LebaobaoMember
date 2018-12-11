using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LebaobaoMember.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Login(string username, string password, string returnUrl)
        {
            string msg = "";
            if (username == "lebaobao" && password == "Dufei.18348728262")
            {
                Session["admin"] = username;
                msg = "登录成功";
                return Json(new { success = true, msg, returnurl = returnUrl });
            }
            msg = "用户名或密码错误";
            return Json(new { success = false, msg });
        }
    }
}