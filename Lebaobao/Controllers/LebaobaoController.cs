using LebaobaoComponents.Domains;
using LebaobaoComponents.Helpers;
using LebaobaoComponents.Repositories.Default;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lebaobao.Controllers
{
    public class LebaobaoController : Controller
    {
        protected static ILog logger = LogManager.GetLogger(typeof(LebaobaoController));
        public LebaobaoDbContext _db = new LebaobaoDbContext();
        private Users user = null;

    
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