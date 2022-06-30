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
        public string History { get; set; }

        //导航属性
        //形式1：一个League对应多个Club，数据库中Club表中出现LeagueId外键
        public League League { get; set; }
        //形式2：一个Club对应多个Player，数据库中Player表中出现ClubId外键
        public List<Player> Players { get; set; }
        //先初始化，以防以后使用时产生空引用的异常
        public Club()
        {
            Players = new List<Player>();
        }
    }
}
