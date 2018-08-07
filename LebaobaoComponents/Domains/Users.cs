using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LebaobaoComponents.Domains
{
    public class Users
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
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
        /// 办卡时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 最近一次消费时间
        /// </summary>
        public DateTime LastTime { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 微信OpenId
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 用户类别
        /// </summary>
        public int UserTypeId { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public UserStatus UserStatus { get; set; }

        public virtual List<Orders> Orders { get; set; }
        public virtual UserType UserType { get; set; }
    }

    public class UserType
    {
        /// <summary>
        /// 用户类别Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 类别描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 会员价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 消费次数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public virtual List<Users> Users { get; set; }     
    }

    public enum UserStatus
    {
        Ok,
        Disabled
    }
}
