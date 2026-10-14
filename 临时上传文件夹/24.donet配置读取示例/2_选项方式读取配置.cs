 using System;
 using System.Collections.Generic;
 using Microsoft.Extensions.Configuration;
 using Microsoft.Extensions.DependencyInjection;
 using Microsoft.Extensions.Logging;
 using Microsoft.Extensions.Options;

 internal class Program
 {
     public static void Main(string[] args)
     {
         //1.依旧创建构造器->添加文件->Build根对象
        ConfigurationBuilder configBuilder = new ConfigurationBuilder();
        configBuilder.AddJsonFile
        (
            "PlayerInfo.json",
            optional: false,
            reloadOnChange: true
        );
        IConfigurationRoot configRoot = configBuilder.Build();
        //如果你想直接Get返回强类型对象,对比下面的选项方式
        PlayerConfig player = configRoot.Get<PlayerConfig>();
        
        //4.创建DI注册容器,注册根对象
        var serv = new ServiceCollection();
        //当调用IOptions<PlayerConfig>接口时,返回PlayerConfig数据副本(通过之前的ConfigRoot生成)
        //分别绑定生成PlayerConfig的数据,用configRoot根对象实现
        serv.AddOptions<PlayerConfig>().Bind(configRoot); 
        
        //别忘了显式注册PlayerService本身(虽然你不写系统也自动帮你实现)
        //哦好吧AI告诉我系统并不会帮我自动实现,系统咋这么坏
        serv.AddScoped<PlayerService>();
        
        //5.创建DI容器
        using (var prov = serv.BuildServiceProvider())
        {
            //同样通过容器获取PlayerService对象
            var playerService = prov.GetRequiredService<PlayerService>();
            //打印信息
            playerService.ShowPlayerInfo();
        }
     }
 }
 
 //2.创建业务服务类
 public class PlayerService
 {
     private readonly PlayerConfig _config;

     //当请求PlayerService实例时,需要IOptions<PlayerConfig>参数
     public PlayerService(IOptions<PlayerConfig> options)
     {
         //prov检查
         // .Value属性才是真正的 PlayerConfig 对象
         _config = options.Value; 
     }

     public void ShowPlayerInfo()
     {
         Console.WriteLine("玩家: " + _config.PlayerName);
         Console.WriteLine("等级: " + _config.PlayerLevel);
         Console.WriteLine("武器: " + _config.Items.Gun);
         Console.WriteLine("技能数: " + _config.Skills.Count);
     }
 }
 
 //3.依旧创建对应类(属性有set且和字段相同)
 public class PlayerConfig
 {

     public string PlayerName { get; set; }
     public int PlayerLevel { get; set; } 
     public ItemInfo Items { get; set; }    
     public List<string> Skills { get; set; } 
 }
 
 public class ItemInfo
 {
     public string Gun { get; set; }
     public string Car { get; set; }
 }
 
 