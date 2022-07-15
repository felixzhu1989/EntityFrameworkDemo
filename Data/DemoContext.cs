using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public class DemoContext:DbContext
    {
        //DbContext包含了所有逻辑，包括跟数据库交互，变化追踪等
        //将三个类暴露成DbSet属性，
        public DbSet<League> Leagues { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Role> Roles { get; set; }
        //最好是所有表格都设置DbSet属性，这里还会让表名成为属性名，方便以后操作
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<Game> Games { get; set; }
        //关联表GamePlayer可以不设置DbSet，默认以GamePlayer作为表名

        public DemoContext()
        {
            //设置context对所有操作不进行变化追踪
            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        //使用日志往控制台输出EFCore生成的SQL语句
        public static readonly ILoggerFactory ConsoleLoggerFactory =
            LoggerFactory.Create(builder =>
            {
                //对日志进行过滤，只输出数据库的执行命令，级别为information
                builder.AddFilter((category, level) =>
                        category == DbLoggerCategory.Database.Command.Name
                        && level == LogLevel.Information)
                    .AddConsole();
            });
        //指定数据库和数据库连接字符串
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //示例数据库连接字符串server=.\\SQLEXPRESS;uid=sa;pwd=123456;database=Forum;
            //.UseLoggerFactory(ConsoleLoggerFactory)是让控制台输出SQL语句的配置
            //.EnableSensitiveDataLogging()是让SQL语句显示参数的配置
            optionsBuilder
                .UseLoggerFactory(ConsoleLoggerFactory)
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"server=PDMSERVER\SQLEXPRESS;database=DemoDb;uid=sa;pwd=Epdm2018;TrustServerCertificate=true");

        }

        //设置Model的属性或Model的其他特性进行设置
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //设置联合主键
            modelBuilder.Entity<GamePlayer>().HasKey(x=>new {x.PlayerId,x.GameId});
            //设置一对一关系的主体是Player（子表Resume表有一个主表的PlayerId外键）
            modelBuilder.Entity<Resume>().HasOne(x => x.Player)
                .WithOne(x => x.Resume)
                .HasForeignKey<Resume>(x => x.PlayerId);
        }

       
    }
}