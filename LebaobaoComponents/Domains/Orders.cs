using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LebaobaoComponents.Domains
{
   public class Orders
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public int UserId { get; set; }
        public virtual Users User { get; set; }
    }
}
