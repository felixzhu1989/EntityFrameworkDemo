using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 多对多的中间表
    /// </summary>
    public class GamePlayer
    {
        //中间表无需Id属性，但应当设置联合主键，在Context中添加

        //在Player中添加List<GamePlayer>的导航属性
        public int PlayerId { get; set; }
        //在Game中添加List<GamePlayer>的导航属性
        public int GameId { get; set; }

        //导航属性
        //设置GamePlayer对应多个Game和Player，体现Game和Player多对多的关系
        public Game Game { get; set; }
        public Player Player { get; set; }
    }
}
