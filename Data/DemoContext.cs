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

        //指定数据库和数据库连接字符串
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //server=.\\SQLEXPRESS;uid=sa;pwd=123456;database=Forum;
            optionsBuilder
                .UseLoggerFactory(ConsoleLoggerFactory)
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

        public static readonly ILoggerFactory ConsoleLoggerFactory =
            LoggerFactory.Create(builder =>
            {
                //对日志进行过滤，只输出数据库的执行命令，级别为information
                builder.AddFilter((category, level) =>
                        category == DbLoggerCategory.Database.Command.Name
                        && level == LogLevel.Information)
                    .AddConsole();
            });
    }
}