// See https://aka.ms/new-console-template for more information
using Data;
using Domain;
using System.Linq;

//using表示方法体走完以后，context会被dispose掉
using var context = new DemoContext();

//添加一条数据
//AddOneData(context);

//添加多条数据（不同类型）
//AddMoreData(context);

var leagues = context.Leagues.ToList();//查询表中所有的数据
var leagues1 = context.Leagues
    .Where(x=>x.Country=="Italy")
    .ToList();//带筛选条件的查询，只有在ToList时才真正执行查询
//如果使用foreach循环，那么数据库连接会一直保持链接，直至循环结束
foreach (var league in context.Leagues)
{
    Console.WriteLine(league.Name);
}

//Linq的方式(用的少)
var leagues2 = (from lg in context.Leagues select lg).ToList();
var leagues3 = (from lg in context.Leagues
    where lg.Country == "Italy"
    select lg).ToList();








//添加一条数据
void AddOneData(DemoContext demoContext)
{
    var seriesA = new League
    {
        Country = "Italy",
        Name = "Series A"
    };
    //将seriesA对象添加到context中的Leagues对应的DbSet，相当于被context变化追踪了，状态为新添加
    //这时还未和数据库交互
    demoContext.Leagues.Add(seriesA);
    //SaveChanges检查context中所有的变化追踪的状态，在同一个事务中提交所有变更
    //如果有一个失败，则会执行回滚
    var count = context.SaveChanges();//返回受影响的行数
    Console.WriteLine(count);
}
//添加多条数据（不同类型）
void AddMoreData(DemoContext context1)
{
    var seriesB = new League
    {
        Country = "Italy",
        Name = "Series B"
    };
    var seriesC = new League
    {
        Country = "Italy",
        Name = "Series C"
    };
    //从数据库中查找出seriesA
    var seriesA = context1.Leagues.Single(x => x.Name == "Series A");
    var milan = new Club
    {
        Name = "AC Milan",
        City = "Milan",
        DateOfEstablishment = new DateTime(1899, 12, 6),
        League = seriesA,
        History = "AC米兰足球俱乐部（A.C. Milan），前身为1899年12月16日成立的米兰板球和足球俱乐部，创始人为阿尔弗雷德·爱德华兹。此后历经了百年风风雨雨，AC米兰发展成为今天世界上最伟大的球队之一。"
    };
    context1.AddRange(seriesB, seriesC, milan);

    //添加相同类型时写法一，分别写
    //context.Leagues.AddRange(seriesB, seriesC);
    //添加相同类型时写法二，使用List<>集合
    //context.Leagues.AddRange(new List<League>{seriesB,seriesC});

    var count = context.SaveChanges();
    Console.WriteLine(count);
}