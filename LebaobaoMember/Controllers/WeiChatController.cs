using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using LebaobaoComponents.Domains;
using LebaobaoComponents.Helpers;
using LebaobaoMember.Models.PostModels;
using Newtonsoft.Json;

namespace LebaobaoMember.Controllers
{
    public class WeiChatController : LebaobaoController
    {
        private const string AppId = "wxb4a5e406f48740a2";
        private const string Token = "lebaobao";
        private const string EncryptKey = "Gx99JnWavwV1bA4mLeL2foDJrZsD5cLb5vR3s4O5XOc";
        [HttpGet]
        public ContentResult Message(string signature, string timestamp, string nonce, string echostr)
        {

            if (!String.IsNullOrEmpty(signature))
            {
                string mysig = CryptoHelper.SHA1(Token, timestamp, nonce);

                if (signature.Equals(mysig, StringComparison.OrdinalIgnoreCase))
                {
                    return new ContentResult() { Content = echostr };
                }
            }

            return new ContentResult() { Content = String.Format("signature validate failed") };
        }
        [HttpPost]
        public ContentResult Message(string signature, string timestamp, string nonce, string encrypt_type, string msg_signature)
        {

            //获取消息内容
            string msgContent = "";
            using (StreamReader sr = new StreamReader(Request.InputStream, Encoding.UTF8))
            {
                msgContent = sr.ReadToEnd();
            }


            if (String.IsNullOrEmpty(encrypt_type) || encrypt_type.Equals("raw"))
            {
                string mysig = CryptoHelper.SHA1(Token, timestamp, nonce);
                if (String.IsNullOrEmpty(signature) || !signature.Equals(mysig, StringComparison.OrdinalIgnoreCase))
                {

                    return new ContentResult() { Content = "" };
                }
            }
            else
            {
                //解密消息
                string sMsg = "";  //解析之后的明文
                int ret_code = 0;
                WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(Token, EncryptKey, AppId);
                ret_code = wxcpt.DecryptMsg(msg_signature, timestamp, nonce, msgContent, ref sMsg);
                if (ret_code != 0)
                {


                    return new ContentResult() { Content = "" };
                }

                msgContent = sMsg;
            }


            XmlDocument doc = new XmlDocument();
            doc.LoadXml(msgContent);


            if (doc.DocumentElement.SelectSingleNode("MsgType").InnerText == "text")       //根据回复关键字查询课程
            {
                #region Text

                string content = doc.DocumentElement.SelectSingleNode("Content").InnerText;
                var user = _db.Users.SingleOrDefault(u => u.Phone == content.Trim()&&u.UserStatus==UserStatus.Ok);
                string msg = "";
                if (user != null)
                {
                    msg = $@"<xml>
<ToUserName><![CDATA[{doc.DocumentElement.SelectSingleNode("FromUserName").InnerText}]]></ToUserName>
<FromUserName><![CDATA[{doc.DocumentElement.SelectSingleNode("ToUserName").InnerText}]]></FromUserName>
<CreateTime>{doc.DocumentElement.SelectSingleNode("CreateTime").InnerText}</CreateTime>
<MsgType><![CDATA[text]]></MsgType>
<Content><![CDATA[孩子姓名：{user.ChildName}
联系方式：{user.Phone}
剩余次数：{user.CanUseCount}
<a href='https://www.hdlebaobao.cn/Order/GetOrderListByUserPhone?phone={user.Phone}'>点击查看推拿记录</a>]]></Content>
</xml>";
                }
                else
                {
                    msg = $@"<xml>
<ToUserName><![CDATA[{doc.DocumentElement.SelectSingleNode("FromUserName").InnerText}]]></ToUserName>
<FromUserName><![CDATA[{doc.DocumentElement.SelectSingleNode("ToUserName").InnerText}]]></FromUserName>
<CreateTime>{doc.DocumentElement.SelectSingleNode("CreateTime").InnerText}</CreateTime>
<MsgType><![CDATA[text]]></MsgType>
<Content><![CDATA[查询有误，请重新输入]]></Content>
</xml>";
                }


                return new ContentResult() { Content = msg, ContentEncoding = Encoding.UTF8, ContentType = "text/xml" };
                #endregion
            }



            return new ContentResult() { Content = "", ContentEncoding = Encoding.UTF8, ContentType = "text/xml" };

        }
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
            return Json(new { success = true, errMsg = "save user:ok", user.OpenId, user.Id, user.Name, user.Phone, user.Address, user.ChildName, user.CanUseCount, user.Sex }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindPhone(string openid, string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return Json(new { success = false, msg = "手机号不可为空！~" }, JsonRequestBehavior.AllowGet);
            }
            var user = _db.Users.SingleOrDefault(u => u.Phone == phone && u.UserStatus == UserStatus.Ok);
            if (user != null)
            {
                return Json(new { success = false, msg = "该手机号已绑定其他用户，请联系客服" }, JsonRequestBehavior.AllowGet);
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