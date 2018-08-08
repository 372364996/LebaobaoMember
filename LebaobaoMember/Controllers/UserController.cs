using LebaobaoComponents.Domains;
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
        public ActionResult UserLevel(int index=1)
        {
            var userTypeList = _db.UserTypes.OrderByDescending(t=>t.Id).ToPagedList(index, 20);
            return View(userTypeList);
        }
        public ActionResult UserList(int index = 1)
        {
            var userList = _db.Users.Where(u => u.UserStatus == UserStatus.Ok);
            ViewBag.UserCount = userList.Count();
            var model = userList.OrderByDescending(u => u.Id).ToPagedList(index, 10);
            return View(model);
        }
        public ActionResult UserDel(string name, int index = 1)
        {
            var userList = _db.Users.Where(u => u.UserStatus == UserStatus.Disabled);
            ViewBag.UserCount = userList.Count();
            var model = userList.OrderByDescending(u => u.Id).ToPagedList(index, 10);
            return View(model);
        }

        public ActionResult UserAdd()
        {
            return View();
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
            try
            {
                var user = new Users
                {
                    Name = useraddpostmodel.Name,
                    ChildName = useraddpostmodel.ChildName,
                    Address = useraddpostmodel.Address,
                    CreateTime = DateTime.Now,
                    LastTime = DateTime.Now,
                    Phone = useraddpostmodel.Phone,
                    Sex = useraddpostmodel.Sex,
                    UserStatus = UserStatus.Ok,
                    UserTypeId = 1
                };
                _db.Users.Add(user);
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
        /// 禁用用户
        /// </summary>
        /// <param name="id">用户ID</param>
        ///  <param name="operation">true 恢复用户，false 禁用用户</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UserDelOrReturn(int[] id, bool operation)
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
        #endregion
    }
}