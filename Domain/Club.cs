using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    /// <summary>
    /// 足球队
    /// </summary>
    public class Club
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        [Column(TypeName = "date")]//不需要时间，只需要日期
        public DateTime DateOfEstablishment { get; set; }
        //如果想让History可空，则指定为string?类型，结果数据库nullable: true(Allow Nulls被勾选了)
        public string? History { get; set; }

        //导航属性，一对多的两种表现形式
        //形式1：（我是成员）一个League联赛对应多个Club俱乐部，数据库中Club表中出现LeagueId外键
        public League League { get; set; }
        //形式2：（我是容器）一个Club俱乐部对应多个Player球员，数据库中Player表中出现ClubId外键
        public List<Player> Players { get; set; }
        //先初始化List，以防以后使用时产生空引用的异常
        public Club()
        {
            Players = new List<Player>();
        }
    }
}
