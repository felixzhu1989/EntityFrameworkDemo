using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 比赛
    /// </summary>
    public class Game
    {
        public int Id { get; set; }
        public int Round { get; set; }//第几轮比赛
        public DateTimeOffset? StartTime { get; set; }//可空的开赛时间（值类型）

        //导航属性
        public List<GamePlayer> GamePlayers { get; set; }
        public Game()
        {
            GamePlayers=new List<GamePlayer>();
        }
    }
}
