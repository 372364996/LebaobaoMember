using LebaobaoComponents.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lebaobao.Controllers
{
    public class OrderController : LebaobaoController
    {
        // GET: Order
        public ActionResult GetOrderListByUserPhone(string phone,OrderType ordertype)
        {
            var list = _db.Orders.Where(u => u.User.Phone == phone&&u.OrderType== ordertype).OrderByDescending(o => o.CreateTime).ToList();

            return View(list);
        }
    }
}