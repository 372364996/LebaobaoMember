using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LebaobaoMember.Models.PostModels
{
    public class SessionKey
    {
        public string openid { get; set; }
        public string session_key { get; set; }
    }

    public class WxLoginViewModel
    {
        /// <summary>
        /// 包括敏感数据在内的完整用户信息的加密数据
        /// </summary>
        public string encryptedData { get; set; }

        /// <summary>
        /// 加密算法的初始向量
        /// </summary>
        public string iv { get; set; }

        public string code { get; set; }
    }
}