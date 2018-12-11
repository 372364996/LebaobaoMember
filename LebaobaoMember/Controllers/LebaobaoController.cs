using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using log4net;
using LebaobaoComponents.Domains;
using Newtonsoft.Json;
using LebaobaoComponents;
using LebaobaoComponents.Helpers;
using LebaobaoComponents.Repositories.Default;

namespace LebaobaoMember.Controllers
{
    public class LebaobaoController : Controller
    {
        protected static ILog logger = LogManager.GetLogger(typeof(LebaobaoController));
        public LebaobaoDbContext _db = new LebaobaoDbContext();
        private Users user = null;
        protected static string AppId = "wx74c6cc8e1fac314c";
        protected static string AppSecret = "55c18246c2900a11227f05a35e76f1a5";
        protected static string GetSessionKeyUrl = "https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type=authorization_code";

        protected Users CurrentUser
        {
            get
            {
                if (user == null && !string.IsNullOrEmpty(Request["openid"]))
                {
                    try
                    {
                        user = _db.Users.SingleOrDefault(u => u.OpenId == CryptoHelper.Base64Decode(Request["openid"]));
                    }
                    catch (Exception ex)
                    {
                        logger.Error("获取微信小程序用户登录信息失败:" + ex.Message);
                        return null;
                    }
                }

                return user;
            }
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var mgr = filterContext.HttpContext.Session["admin"];
            if (mgr == null && filterContext.HttpContext.Request.Url.ToString().ToLower().IndexOf("/home/login") == -1)
            {
                filterContext.HttpContext.Response.Redirect("/home/login?returnUrl=" + HttpUtility.UrlEncode(filterContext.HttpContext.Request.Url.ToString()));
                filterContext.HttpContext.Response.End();
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            string error = Utils.GetRandomString("0123456789", 6);
            Session["errorcode"] = error;
            var context = filterContext.HttpContext;
            string data = "";
            if (context.Request.Form != null && context.Request.Form.Count > 0)
            {
                data = JsonConvert.SerializeObject(context.Request.Form);
            }
            string msg = $@"{error}
URL:{context.Request.Url.ToString()}
REFER:{(context.Request.UrlReferrer != null ? filterContext.HttpContext.Request.UrlReferrer.ToString() : "NULL")}
USER:{(context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "NOT AUTH")}
DATA:{data}
{Utils.ExceptionToString(filterContext.Exception)}";
            logger.Error(msg);
            //发送错误日志Email
            //Utils.SendErrorLogEmail(context, error, msg);
            base.OnException(filterContext);
        }
    }
}