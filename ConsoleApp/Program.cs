// See https://aka.ms/new-console-template for more information
using Data;
using Domain;
using Microsoft.EntityFrameworkCore;

//using表示方法体走完以后，context会被dispose掉
using var context = new DemoContext();

//1.添加一条数据
//AddOneData();

//2.添加多条数据（不同类型）
//AddMoreData();

//3.查询汇总
//QueryData();

//4.删除数据
//DeleteData();

//5.修改数据
//场景一，修改被追踪的数据
//UpdateTrackingData();
//场景二，修改离线的数据
//UpdateNoTrackingData();

//一对多关系数据：League-(一对多)->Club-(一对多)->Player

//6.添加关系数据
//场景一，添加俱乐部和球员，指定联赛
//AddClubAndPlayer();
//场景二，查询俱乐部添加球员
//AddPlayerInClub();
//场景三，查询俱乐部添加球员，模拟离线数据
//AddPlayerUseAttach();
//场景四，预先知道球员的Id，给球员添加简历
//AddResumeByPlayerId();

//7.加载关系数据
//场景一，预加载所有属性，Include()，ThenInclude()
//QueryUseInclude();
//场景二，选出某些字段，Select()
//QueryUseSelect();
//场景三，显式加载部分属性，Collection(),Reference()
//QueryUseCollectionOrReference();
//场景四，使用关联数据的属性作为查询条件
//QueryConditionOnChildsProperty();
//场景五，懒加载，Lazy Loading
//在EFCore中默认是关闭的，但是可以手动开启，容易出问题，不建议使用

QueryFelix();


//多对多关系数据：Player<-(一对多)-GamePlayer(中间表)-(一对多)->Game(其中Player与Game形成多对多)

//8.加载多对多关系数据
//QueryMultiToMulti();

//9.修改关联数据
//场景一、修改被追踪的关联数据
//UpdateTrackingChildData();
//场景二、修改离线的关联数据
//UpdateNoTrackingChildData();

//10.如何添加多对多关系（中间表）
//场景一、知道双方Id的情况
//AddMultiToMultiByTwoId();
//场景二、查询其中一个给定另一个的Id
//AddMultiToMultiByOneId();

//11.如何删除多对多关系（中间表）
//DeleteMultiToMultiByTwoId();

//12.如何修改多对多关系（中间表）
//UpdateMultiToMulti();




//---------------方法合集-------------------
//1.添加一条数据
void AddOneData()
{
    var seriesA = new League
    {
        Country = "Italy",
        Name = "Series A"
    };
    //将seriesA对象添加到context中的Leagues对应的DbSet，相当于被context变化追踪了，状态为新添加
    //这时还未和数据库交互
    context.Leagues.Add(seriesA);
    //SaveChanges检查context中所有的变化追踪的状态，在同一个事务中提交所有变更
    //执行添加，如果有一个失败，则会执行回滚，也可调用异步方法SaveChangesAsync
    var count = context.SaveChanges();//返回受影响的行数
    Console.WriteLine(count);
}

//2.添加多条数据（不同类型）
void AddMoreData()
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
    var seriesA = context.Leagues.Single(x => x.Name == "Series A");
    var milan = new Club
    {
        Name = "AC Milan",
        City = "Milan",
        DateOfEstablishment = new DateTime(1899, 12, 6),
        League = seriesA,
        History = "AC米兰足球俱乐部（A.C. Milan），前身为1899年12月16日成立的米兰板球和足球俱乐部，创始人为阿尔弗雷德·爱德华兹。此后历经了百年风风雨雨，AC米兰发展成为今天世界上最伟大的球队之一。"
    };
    context.AddRange(seriesB, seriesC, milan);

    //添加相同类型时写法一，分别写
    //context.Leagues.AddRange(seriesB, seriesC);
    //添加相同类型时写法二，使用List<>集合
    //context.Leagues.AddRange(new List<League>{seriesB,seriesC});

    var count = context.SaveChanges();
    Console.WriteLine(count);
}

//3.查询汇总
void QueryData(DemoContext context)
{
    //Linq方法的方式(推荐使用)
    var leagues = context.Leagues.ToList(); //查询表中所有的数据

    //精确查询：x.Country=="Italy"
    //模糊查询：x.Country.Contains("a")
    var leagues1 = context.Leagues
        .Where(x => x.Country.Contains("a"))
        .ToList();
    //模糊查询：EF.Functions.Like(x.Country, "%a%")，原SQL语句Country like "%a%"
    var leagues2 = context.Leagues
        .Where(x => EF.Functions.Like(x.Country, "%a%"))
        .ToList();

    var first = context.Leagues
        .FirstOrDefault(x => EF.Functions.Like(x.Country, "%a%"));
    Console.WriteLine(first?.Country);

    var first2 = context.Leagues
        .SingleOrDefault(x => x.Id == 2);
    var one = context.Leagues
        .Find(2);
    //使用Last时必须进行排序
    var last = context.Leagues
        .OrderBy(x => x.Id)
        .LastOrDefault(x => x.Name.Contains("a"));

    /*  带筛选条件的查询，只有在ToList时才真正执行查询
        ToList()返回集合,ToListAsync()这些方法都有Async异步方法版本
        First(),FirstOrDefault()返回单个数据
        Single(),SingleOrDefault(),Last(),LastOrDefault()
        Count(),LongCount(),Min(),Max(),Average(),Sum()
        Find()直接针对主键进行查询,它是DbSet的方法
    */

    //如果使用foreach循环，那么数据库连接会一直保持链接，直至循环结束
    foreach (var league in context.Leagues)
    {
        Console.WriteLine(league.Name);
    }

    //Linq表达式的方式(用的少)
    var leaguesExp1 = (from lg in context.Leagues select lg).ToList();
    var leaguesExp2 = (from lg in context.Leagues
                       where lg.Country == "Italy"
                       select lg).ToList();
}

//4.删除数据
void DeleteData()
{
    //Delete只能删除被context追踪的数据，只有被查询出来才能被追踪
    //因此删除之前必须查找
    var milan = context.Clubs.Single(x => x.Name == "AC Milan");
    //删除一条数据
    context.Clubs.Remove(milan);
    context.Remove(milan);
    //删除多条数据
    context.Clubs.RemoveRange(milan, milan);
    context.RemoveRange(milan, milan);
    //执行删除
    var count = context.SaveChanges();
    Console.WriteLine(count);
}

//5.修改数据
//场景一，修改被追踪的数据
void UpdateTrackingData()
{
    //修改前被追踪（真实业务场景很少）
    //修改单条数据，最终修改的SQL语句中只有Name属性，因为Name属性的修改被追踪了
    var league = context.Leagues.First();
    league.Name += "~~";
    //修改多条数据，此处Skip(1)是跳过前面1条数据，Take(2)是取2条数据
    //Skip和Take通常用于翻页功能
    //var leagues = context.Leagues.Skip(1).Take(2).ToList();
    //foreach (var league in leagues)
    //{
    //    league.Name += "~~";
    //}
    var count = context.SaveChanges();
    Console.WriteLine(count);
}
//场景二，修改离线的数据
void UpdateNoTrackingData()
{
    //方式二，前端修改后，以json的方式传递过来,数据未被追踪
    //模拟前端修改，AsNoTracking()方法是人为不进行变化追踪，可以在context中设置所有操作不追踪
    var league = context.Leagues.AsNoTracking().First();
    league.Name += "++";
    //Update()方法让context对离线数据league进行变化追踪，还有UpdateRange()方法
    //Update()方法对离线数据进行追踪时，除主键外的所有属性都会被添加到SQL语句中执行修改
    //但是对单个属性执行修改也是能做到，具体可查询微软官方文档
    context.Leagues.Update(league);
    //执行修改
    var count = context.SaveChanges();
    Console.WriteLine(count);
}

//6.添加关系数据
//场景一，添加俱乐部和球员，指定联赛，使用Add
void AddClubAndPlayer()
{
    //添加一个Club俱乐部，应该时添加到一个现有的League联赛中
    //先查询出一个确定的联赛
    var seriesA = context.Leagues.SingleOrDefault(x => x.Name.Contains("Series A"));
    //新建俱乐部对象，然后执行Add()方法，通知context追踪离线数据juventus
    //方式一，（我是成员）给导航属性赋值，引用父对象（联赛）
    //在Club中有一个导航属性League，因此将查询出来的seriesA对象赋值给导航属性
    //方式二，（我是容器）给导航集合属性List中，新增子对象（球员）
    //这里往数据库Club和Player两表中分别都新增了数据
    var juventus = new Club()
    {
        //无需设置外键
        League = seriesA,
        Name = "Juventus",
        City = "Torino",
        DateOfEstablishment = new DateTime(1897, 11, 1),
        History =
            "这一切都始于翁贝托国王大道的长凳，这是都灵市中心最著名的街道之一。 一群朋友聚集在这张长凳上。 他们都对足球有着共同的热情，这是一种发源自英格兰的特殊运动。 他们有一个有趣的想法，要创建一个体育俱乐部。",
        Players = new List<Player>()
        {
            new Player()
            {
                Name = "C. Ronaldo",
                DateOfBirth = new DateTime(1985, 2, 5)
                //这里并未指定Resume，数据库表中ResumeId为0
            }
        }
    };
    context.Clubs.Add(juventus);
    int count = context.SaveChanges();
    Console.WriteLine(count);
}
//场景二，查询俱乐部添加球员，使用Add
void AddPlayerInClub()
{
    //查询俱乐部对象，然后对俱乐部中的Players导航集合属性List中添加新建球员对象
    //此时juventus的属性Players发生改变会被context追踪，并且无需设置外键
    var juventus = context.Clubs.SingleOrDefault(x => x.Name == "Juventus");
    juventus.Players.Add(new Player()
    {
        Name = "felix",
        DateOfBirth = new DateTime(1989, 12, 15),
        Gender = Gender_e.Male,
        Resume = new Resume(){Description = "测试Role" },
        Roles = new List<Role>() { new Role() { Name = Role_e.Admin},new Role(){Name = Role_e.Viewer}}
    });
    int count = context.SaveChanges();
    Console.WriteLine(count);
}
//场景三，查询俱乐部添加球员，模拟离线数据，使用Attach
void AddPlayerUseAttach()
{
    var juventus = context.Clubs.SingleOrDefault(x => x.Name == "Juventus");
    juventus.Players.Add(new Player()
    {
        Name = "Miralem Pjanic",
        DateOfBirth = new DateTime(1999, 12, 18)
    });
    //模拟离线数据
    {
        //新建的context对象，juventus对newContext来说就是离线数据，没有跟踪状态
        using var newContext = new DemoContext();
        //使用Attach执行添加过程，让不变的数据不更新，无需指定外键
        //在SQL语句上只有Players表格更新了，而Clubs表并无变化
        //INSERT INTO [Players] ([ClubId], [DateOfBirth], [Name], [ResumeId])
        newContext.Clubs.Attach(juventus);
        var count = newContext.SaveChanges();
        Console.WriteLine(count);
    }
}
//场景四，预先知道球员的Id，给球员添加简历
void AddResumeByPlayerId()
{
    //注意这样操作导致Players表中C. Ronaldo的ResumeId依然为0，不知道是不是设计缺陷
    //个人觉得Players表中不需要ResumeId字段
    var resume = new Resume()
    {
        PlayerId = 1, //C. Ronaldo
        Description = "克里斯蒂亚诺·罗纳尔多（Cristiano Ronaldo、C罗）1985年2月5日出生于葡萄牙马德拉岛丰沙尔，葡萄牙职业足球运动员，司职边锋、中锋，效力于意甲尤文图斯足球俱乐部。"
    };
    context.Resumes.Add(resume);
    var count = context.SaveChanges();
    Console.WriteLine(count);
}


//7.加载关系数据
//场景一，预加载所有属性，Include()，ThenInclude()
void QueryUseInclude()
{
    //一次性将所有关联数据全部预加载进来
    //将Club俱乐部关联的League联赛查询出来，使用Include()
    //如果想继续查询Club俱乐部的Player球员，则应继续使用Include()
    //如果还想继续查询到Player球员的Resume简历，则应该在紧挨Player后使用ThenInclude()，使用缩进体现它们之间的级联关系
    //如果还想继续查询到Player球员关联的GamePlayers比赛信息，则应该使用Include()返回到Player再ThenInclude()
    //如果直接再继续查询GamePlayers比赛信息中的Game比赛，则继续ThenInclude()
    //Include()在SQL语句为Left Join
    var clubs = context.Clubs
        .Where(x => x.City.Contains("o"))
        .Include(x => x.League)
        .Include(x => x.Players)
            .ThenInclude(y => y.Resume)
        .Include(x => x.Players)
            .ThenInclude(y => y.GamePlayers)
            .ThenInclude(z => z.Game)
        .ToList();
    Console.WriteLine(clubs.First().Name);
}
//场景二，选出某些字段，Select()
void QueryUseSelect()
{
    //使用new构造了匿名类，info就是匿名类的集合，这个匿名类是无法被context变化追踪的
    var info = context.Clubs
        .Where(x => x.City.Contains("o"))
        .Select(x => new
        {
            x.Id,
            LeagueName = x.League.Name,
            x.Name,
            Players = x.Players
                .Where(p =>
                    p.DateOfBirth > new DateTime(1990, 1, 1))
        }).ToList();
    //但是匿名类中的Players被变化追踪，如果Players发生变化，使用SaveChanges后依然能够让变更生效
    //只要是context能够识别的对象都会被追踪
    foreach (var data in info)
    {
        foreach (var player in data.Players)
        {
            player.Name += "~";
        }
    }

    //下一行设置断点，然后打开调试-快速监视，输入如下代码，在结果视图中，可以查看变化追踪的信息
    //context.ChangeTracker.Entries(),results
    //这样就无需真的执行更改，只需要观察变化追踪即可
    context.SaveChanges();
}
//场景三，显式加载部分属性，Collection(),Reference()
void QueryUseCollectionOrReference()
{
    //弱点只能针对单个数Club据进行加载，如果是针对List<Club>则无法使用
    //先查询俱乐部，没有包含关联数据
    var info = context.Clubs.First();
    //针对导航集合属性，使用Collection()
    context.Entry(info)
        .Collection(x => x.Players)
        .Query()
        .Where(x => x.DateOfBirth > new DateTime(1990, 1, 1))
        .Load();
    //针对导航单个属性，使用Reference()
    context.Entry(info)
        .Reference(x => x.League)
        .Load();
}
//场景四，使用关联数据的属性作为查询条件
void QueryConditionOnChildsProperty()
{
    var data = context.Clubs
        .Where(x => x.League.Name.Contains("e"))
        .ToList();
}


void QueryFelix()
{
    var player = context.Players
        .Where(x => x.Name.Contains("felix"))
        .Include(x => x.Roles)
        .FirstOrDefault();
    Console.WriteLine(player.Roles[0].Name);
}


//8.加载多对多关系数据
void QueryMultiToMulti()
{
    //将之前的查询进行组合，先查询到Player，然后Include关联GamePlayer
    //在GamePlayer后用ThenInclude关联Game
    var player = context.Players
        .Include(x => x.GamePlayers)
            .ThenInclude(y => y.Game)
        .FirstOrDefault();
    //如果想直接查询GamePlayer
    //使用Set<GamePlayer>()，让未在context中设置为DbSet的属性实现被追踪
    //使用Set<GamePlayer>()，可以正常使用Where，Find，Fist等Linq方法
    var gamePlayers = context.Set<GamePlayer>()
        .Where(x => x.Player.Id > 1)
        .ToList();
}

//9.修改关联数据
//场景一、修改被追踪的关联数据
void UpdateTrackingChildData()
{
    var club = context.Clubs
        .Include(x => x.League).First();
    club.League.Name += "@";
    var count = context.SaveChanges();
    Console.WriteLine(count);
}
//场景二、修改离线的关联数据
void UpdateNoTrackingChildData()
{
    var game = context.Games
        .Include(x => x.GamePlayers)
            .ThenInclude(y => y.Player)
        .FirstOrDefault();
    var firstPlayer = game.GamePlayers[0].Player;
    firstPlayer.Name += "$";
    //模拟离线数据
    {
        using var newContext = new DemoContext();
        //Update()会将所有关联的对象都生成SQL语句进行更新（不推荐），浪费资源
        //newContext.Players.Update(firstPlayer);
        //使用Entry()方法，可以手动指定State，让EFCore只修改firstPlayer而不修改关联数据
        newContext.Entry(firstPlayer).State = EntityState.Modified;
        newContext.SaveChanges();
    }
}

//10.如何添加多对多关系（中间表）
//场景一、知道双方Id的情况
void AddMultiToMultiByTwoId()
{
    var gamePlayer = new GamePlayer
    {
        GameId = 1,
        PlayerId = 3
    };
    context.Add(gamePlayer);
    context.SaveChanges();
}
//场景二、查询其中一个给定另一个的Id
void AddMultiToMultiByOneId()
{
    var game = context.Games.FirstOrDefault();
    game.GamePlayers.Add(new GamePlayer()
    {
        PlayerId =3 
    });
    context.SaveChanges();
}

//11.如何删除多对多关系（中间表）
void DeleteMultiToMultiByTwoId()
{
    //知道双方Id的情况
    //更好的方法是从数据库查出gamePlayer而不是直接new
    var gamePlayer = new GamePlayer
    {
        GameId = 1,
        PlayerId = 3
    };
    context.Remove(gamePlayer);
    context.SaveChanges();
}

//12.如何修改多对多关系（中间表）
void UpdateMultiToMulti()
{
    //因为是联合主键，因此无法修改其中一个
    //应当首先删除，然后再添加
    DeleteMultiToMultiByTwoId();
    AddMultiToMultiByTwoId();
}

//13.如何设置一对一的关系
//场景一，数据追踪
void AddOneToOneTracking()
{
    var player = context.Players.First();
    player.Resume = new Resume()
    {
        Description = "1234"
    };
    context.SaveChanges();
}
//场景二，数据离线
void AddOneToOneNoTracking()
{
    var player = context.Players
        .AsNoTracking()
        .OrderBy(x => x.Id)
        .Last();
    player.Resume = new Resume() {Description = "4321"};
    //模拟离线数据
    {
        using var newContext = new DemoContext();
        newContext.Attach(player);
        newContext.SaveChanges();
    }
}
//场景三，添加已有数据失败
void AddOneToOneNoTrackingAgain()
{
    var player = context.Players
        .AsNoTracking()
        .OrderBy(x => x.Id)
        .Last();
    //如果继续往同一个player上添加Resume则会报错PlayerId唯一，因为他们是一对一的关系
    player.Resume = new Resume() { Description = "987654321" };
    //模拟离线数据
    {
        using var newContext = new DemoContext();
        newContext.Attach(player);
        newContext.SaveChanges();
    }
}
//场景四，添加已有数据成功（先删除后添加）
void AddOneToOneTrackingAgain()
{
    var player = context.Players
        .Include(x => x.Resume)
        .OrderBy(x => x.Id)
        .Last();
    //使用Include后，如果继续往同一个player上添加Resume时，
    //执行的SQL语句是先删除Delete原来的resume然后再Insert新的resume
    player.Resume = new Resume() {Description = "456789"};
    context.SaveChanges();
}

//14.在EFCore中执行原生的SQL语句
