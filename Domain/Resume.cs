using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 队员的简历，和队员是一对一的关系
    /// </summary>
    public class Resume
    {
        public int Id { get; set; }
        public string Description { get; set; }

        //导航属性
        //一对一关系设置的外键，同时在Player中也做相同的设置
        public int PlayerId { get; set; }
        public Player Player { get; set; }
    }
}
