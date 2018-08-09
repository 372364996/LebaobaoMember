using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LebaobaoMember.Models.PostModels
{
    public class ChargePostModel
    {
        public int UserId { get; set; }
        public decimal Money { get; set; }
        public int CanUseCount { get; set; }
        public string PayMethod { get; set; }
    }
}