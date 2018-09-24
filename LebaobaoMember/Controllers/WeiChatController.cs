using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using LebaobaoComponents.Domains;
using LebaobaoComponents.Helpers;
using LebaobaoMember.Models.PostModels;
using Newtonsoft.Json;

namespace LebaobaoMember.Controllers
{
    public class WeiChatController : LebaobaoController
    {
        public JsonResult Login(WxLoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.code))
            {
                return Json(new { success = false, errMsg = "获取code失败！" }, JsonRequestBehavior.AllowGet);
            }

            //string sessionKey;
            //if (!SessionKeys.TryGetValue(model.session_id, out sessionKey))
            //{
            //    return Json(new { success = false, errMsg = "在安全字典中获取session_key失败" });
            //}
            var url = string.Format(GetSessionKeyUrl, AppId, AppSecret, model.code);
            logger.Debug(url);
            var jsonStr = Encoding.UTF8.GetString(new WebClient().DownloadData(url));
            logger.Debug(jsonStr);
            var session = JsonConvert.DeserializeObject<SessionKey>(jsonStr);
            if (string.IsNullOrEmpty(session.session_key))
            {
                return Json(new { success = true, errMsg = "session_key参数:null" }, JsonRequestBehavior.AllowGet);
            }
            Users user = null;
            try
            {
                 user = GetUser(AppId, session.session_key, model.encryptedData, model.iv);
            }
            catch (Exception ex)
            {
                logger.Error("save user:error," + ex.Message);
                return Json(new { success = false, errMsg = "save user:error," + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, errMsg = "save user:ok", user.OpenId,user.Id,user.Name,user.Phone,user.Address,user.ChildName,user.CanUseCount,user.Sex }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindPhone(string openid, string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return Json(new { success = false, msg = "手机号不可为空！~" }, JsonRequestBehavior.AllowGet);
            }
            var user= _db.Users.SingleOrDefault(u => u.Phone == phone && u.UserStatus == UserStatus.Ok);
            if (user!=null)
            {
                return Json(new { success = false, msg = "该手机号已绑定其他用户，请联系客服" },JsonRequestBehavior.AllowGet);
            }
            user = _db.Users.SingleOrDefault(u => u.OpenId == openid && u.UserStatus == UserStatus.Ok);
            user.Phone = phone;
            _db.SaveChanges();
            return Json(new { success = true, msg = "绑定成功" }, JsonRequestBehavior.AllowGet);
        }

        // GET: WeiChat
        public ActionResult Info(int userid)
        {
            var user = _db.Users.Find(userid);

            return Json(
                new
                {
                    user.Id,
                    user.Name,
                    user.ChildName,
                    user.CanUseCount,
                    user.Address,
                    user.CreateTime,
                    user.LastTime,
                    user.Phone
                }, JsonRequestBehavior.AllowGet);
        }
        #region 微信小程序
        /// <summary>
        /// 获取/绑定用户
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="sessionKey"></param>
        /// <param name="encryptedDataStr"></param>
        /// <param name="iv"></param>
        /// <returns>unionID</returns>
        public Users GetUser(string appid, string sessionKey, string encryptedDataStr, string iv)
        {
            var encryptedData = WXBizDataCrypt.DecryptData(sessionKey, encryptedDataStr, iv);
            logger.Debug($"用户完整信息：{encryptedData}");
            UserInfoFull userinfoFull = JsonConvert.DeserializeObject<UserInfoFull>(encryptedData);
            if (userinfoFull.watermark.appid != appid)
            {
                throw new Exception("userinfofull.wartemark.appid 不等于 appid!");
            }
            Users user = null;
            if (string.IsNullOrEmpty(userinfoFull.openId))
            {
                throw new Exception("openId is null or empty!");
            }
            user = _db.Users.SingleOrDefault(u => u.OpenId == userinfoFull.openId);

            if (user == null)
            {
                Users u = new Users();
                u.Name = userinfoFull.nickName;
                u.Address = userinfoFull.country + userinfoFull.province + userinfoFull.city;
                u.CreateTime = DateTime.Now;
                u.LastTime = DateTime.Now;
                u.Sex = userinfoFull.gender;
                u.UserStatus = UserStatus.Ok;
                u.OpenId = userinfoFull.openId;
                u.UserTypeId = 1;
                _db.Users.Add(u);
                _db.SaveChanges();
                return u;
            }
            return user;
        }

        class UserInfoFull
        {
            public string openId { get; set; }
            public string nickName { get; set; }
            public int gender { get; set; }
            public string language { get; set; }
            public string city { get; set; }
            public string province { get; set; }
            public string country { get; set; }
            public string avatarUrl { get; set; }
            public string unionId { get; set; }
            public Watermark watermark { get; set; }
        }

        class Watermark
        {
            public int timestamp { get; set; }
            public string appid { get; set; }
        }
        #endregion
    }
}