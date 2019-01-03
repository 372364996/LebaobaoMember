using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LebaobaoComponents.Domains
{
   public class ChargeLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [StringLength(100)]
        [Index("INDEX_CHARGE_NUMBER", IsUnique = true)]
        public string Number { get; set; }
        /// <summary>
        /// 充值使用了多少现金
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 获得了多少可使用次数
        /// </summary>
        public int CanUseCount { get; set; }
        public DateTime CreateTime { get; set; }
        [StringLength(100)]
        public string PayMethod { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public OrderType OrderType { get; set; }
        public virtual Users User { get; set; }
    }
}
