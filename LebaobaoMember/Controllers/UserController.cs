using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LebaobaoMember.Controllers
{
    public class UserController : LebaobaoController
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserList()
        {
          var data=  _db.Users.ToList();
            return View(data);
        }
        public ActionResult UserDel()
        {
            return View();
        }

        public ActionResult UserAdd()
        {
            return View();
        }
        public ActionResult UserLevel()
        {
            return View();
        }
    }
}