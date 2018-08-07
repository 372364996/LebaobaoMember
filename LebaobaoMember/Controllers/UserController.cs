using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace LebaobaoMember.Controllers
{
    public class UserController : LebaobaoController
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserList(int index=1)
        {
            var data = _db.Users.OrderBy(u=>u.Id).ToPagedList(index, 2);
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