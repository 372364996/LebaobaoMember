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
        public ActionResult Index(string childname, string phone, int userid = 0, int index = 1)
        {
            var orderlist = _db.Orders.ToList();
            if (userid != 0)
            {
                orderlist = orderlist.Where(o => o.UserId == userid).ToList();
            }
            if (!string.IsNullOrEmpty(childname))
            {
                orderlist = orderlist.Where(o => o.User.ChildName.Contains(childname.Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(phone))
            {
                orderlist = orderlist.Where(o => o.User.Phone.Contains(phone.Trim())).ToList();
            }
            ViewBag.OrderCount = orderlist.Count();
            var list = orderlist.OrderByDescending(o => o.Id).ToPagedList(index, 20);
            return View(list);
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
                if (user.CanUseCount==0)
                {
                    logger.Debug($"{DateTime.Now}:可使用次数不足");
                    return Json(new { success = false });
                }
                user.LastTime = DateTime.Now;
                user.CanUseCount--;
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