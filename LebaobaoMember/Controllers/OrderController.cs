using LebaobaoComponents.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace LebaobaoMember.Controllers
{
    public class OrderController : LebaobaoController
    {
        // GET: Order
        public ActionResult Index(string name, int index = 1)
        {
            var orderlist = _db.Orders.OrderByDescending(o => o.Id).ToPagedList(index, 20);
            return View(orderlist);
        }
        public ActionResult OrderAddView(int userid)
        {
            var user = _db.Users.Find(userid);
            return View(user);
        }

        #region Post请求
        /// <summary>
        /// 添加来访记录
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OrderAdd(int userid)
        {
            try
            {
                var order = new Orders
                {
                    UserId = userid,
                    CreateTime = DateTime.Now
                };
                _db.Orders.Add(order);
                var user = _db.Users.Find(userid);
                user.LastTime = DateTime.Now;
                _db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {

                logger.Debug(ex.Message);
                return Json(new { success = false });
            }

        }
        #endregion
    }
}