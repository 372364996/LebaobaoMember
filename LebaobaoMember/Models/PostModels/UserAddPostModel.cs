using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LebaobaoMember.Models.PostModels
{
    public class UserAddPostModel
    {
        public string Name { get; set; }
        /// <summary>
        /// 孩子姓名
        /// </summary>
        public string ChildName { get; set; }
        /// <summary>
        /// 孩子性别
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// 用户地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
    }
}