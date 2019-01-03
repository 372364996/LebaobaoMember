using LebaobaoComponents.Domains;
using LebaobaoComponents.Helpers;
using LebaobaoMember.Models.PostModels;
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
        public ActionResult UserLevel(int index = 1)
        {
            var userTypeList = _db.UserTypes.OrderByDescending(t => t.Id).ToPagedList(index, 20);
            return View(userTypeList);
        }
        public ActionResult UserList(string childname, string phone, int index = 1)
        {
            var userList = _db.Users.Where(u => u.UserStatus == UserStatus.Ok);
            if (!string.IsNullOrEmpty(childname))
            {
                userList = userList.Where(u => u.ChildName.Contains(childname.Trim()));
            }
            if (!string.IsNullOrEmpty(phone))
            {
                userList = userList.Where(u => u.Phone.Contains(phone.Trim()));
            }
            ViewBag.UserCount = userList.Count();
            var model = userList.OrderByDescending(u => u.LastTime).ToPagedList(index, 10);
            return View(model);
        }
        public ActionResult ChargeList(string childname, string phone, OrderType? ordertype, int userid = 0, int index = 1)
        {
            var chargeLogs = _db.ChargeLogs.ToList();
            if (userid != 0)
            {
                chargeLogs = chargeLogs.Where(u => u.UserId == userid).ToList();
            }
            if (!string.IsNullOrEmpty(childname))
            {
                chargeLogs = chargeLogs.Where(u => u.User.ChildName.Contains(childname.Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(phone))
            {
                chargeLogs = chargeLogs.Where(u => u.User.Phone.Contains(phone.Trim())).ToList();
            }
            if (ordertype != null)
            {
                chargeLogs = chargeLogs.Where(u => u.OrderType == ordertype).ToList();
            }
            ViewBag.ChargeCount = chargeLogs.Count();
            var model = chargeLogs.OrderByDescending(u => u.Id).ToPagedList(index, 10);
            return View(model);
        }
        public ActionResult UserStop(string name, int index = 1)
        {
            var userList = _db.Users.Where(u => u.UserStatus == UserStatus.Disabled);
            ViewBag.UserCount = userList.Count();
            var model = userList.OrderByDescending(u => u.Id).ToPagedList(index, 10);
            return View(model);
        }
        public ActionResult UserDel(string name, int index = 1)
        {
            var userList = _db.Users.Where(u => u.UserStatus == UserStatus.Delete);
            ViewBag.UserCount = userList.Count();
            var model = userList.OrderByDescending(u => u.Id).ToPagedList(index, 10);
            return View(model);
        }

        public ActionResult UserAdd(int userid = 0)
        {
            if (userid == 0)
            {
                return View();
            }
            else
            {
                var user = _db.Users.Find(userid);
                return View(user);
            }
        }
        public ActionResult Charge(int userid)
        {
            var user = _db.Users.Find(userid);
            return View(user);
        }

        #region Post请求
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="useraddpostmodel">请求参数实体</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UserAdd(UserAddPostModel useraddpostmodel)
        {
            Users user = new Users();
            try
            {
                if (useraddpostmodel.UserId == 0)
                {
                    user.Name = useraddpostmodel.Name;
                    user.ChildName = useraddpostmodel.ChildName;
                    user.Address = useraddpostmodel.Address;
                    user.CreateTime = DateTime.Now;
                    user.LastTime = DateTime.Now;
                    user.Phone = useraddpostmodel.Phone;
                    user.Sex = useraddpostmodel.Sex;
                    user.UserStatus = UserStatus.Ok;
                    user.UserTypeId = 1;
                    _db.Users.Add(user);
                }
                else
                {
                    user = _db.Users.Find(useraddpostmodel.UserId);
                    user.Name = useraddpostmodel.Name;
                    user.ChildName = useraddpostmodel.ChildName;
                    user.Address = useraddpostmodel.Address;
                    user.Phone = useraddpostmodel.Phone;
                    user.Sex = useraddpostmodel.Sex;
                }

                _db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return Json(new { success = false });
            }

        }
        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="chargepostmodel">请求参数实体</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Charge(ChargePostModel chargepostmodel)
        {
            try
            {
                var charge = new ChargeLog()
                {
                    Number = Utils.GetOrderNumber(),
                    Money = chargepostmodel.Money,
                    UserId = chargepostmodel.UserId,
                    CanUseCount = chargepostmodel.CanUseCount,
                    PayMethod = chargepostmodel.PayMethod,
                    OrderType = chargepostmodel.OrderType,
                    CreateTime = DateTime.Now,
                };
                _db.ChargeLogs.Add(charge);
                var user = _db.Users.Find(chargepostmodel.UserId);
                if (chargepostmodel.OrderType == OrderType.TuiNa)
                {
                    user.CanUseCount += chargepostmodel.CanUseCount;
                }
                if (chargepostmodel.OrderType == OrderType.BaoJian)
                {
                    user.BaoJianCount += chargepostmodel.CanUseCount;
                }
                _db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return Json(new { success = false, msg = ex.Message });
            }

        }
        /// <summary>
        /// 禁用用户
        /// </summary>
        /// <param name="id">用户ID</param>
        ///  <param name="operation">true 恢复用户，false 禁用用户</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UserStopOrReturn(int[] id, bool operation)
        {
            try
            {
                foreach (var item in id)
                {
                    var user = _db.Users.Find(item);
                    user.UserStatus = operation ? UserStatus.Ok : UserStatus.Disabled;
                }

                _db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return Json(new { success = false });
            }


        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">用户ID</param>
        ///  <param name="operation">true 恢复用户，false 删除用户</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UserDelOrReturn(int[] id, bool operation)
        {
            try
            {
                foreach (var item in id)
                {
                    var user = _db.Users.Find(item);
                    user.UserStatus = operation ? UserStatus.Ok : UserStatus.Delete;
                }

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