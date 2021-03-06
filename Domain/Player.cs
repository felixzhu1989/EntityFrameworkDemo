using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 队员
    /// </summary>
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "date")]//不需要时间，只需要日期
        public DateTime DateOfBirth { get; set; }
        public Gender_e Gender { get; set; }
        public List<Role> Roles { get; set; } 

        //导航属性
        public List<GamePlayer> GamePlayers { get; set; }
        public Player()
        {
            GamePlayers=new List<GamePlayer>();
            Roles=new List<Role>();
        }
        //一对一关系设置的外键，同时在Resume中也做相同的设置，需要在Context中设定主体
        public int ResumeId { get; set; }
        public Resume Resume { get; set; }
    }
}
