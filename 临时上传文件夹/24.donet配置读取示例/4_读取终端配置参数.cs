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
         //依旧创建构造器,但是不需要AddJsonFile了
         var configBuilder = new ConfigurationBuilder();
         //这里通过AddCommandLine(也就是IConfigurationBuilder的拓展方法)实现:
         //构建器添加cmd参数->构造Root对象
         configBuilder.AddCommandLine(args);
         var configRoot = configBuilder.Build();
         
         var serv = new ServiceCollection();
         serv.AddOptions().Configure<PlayerConfig>(((PlayerConfig config) =>
         {
             //这段是通过闭包引入外界configRoot
             configRoot.GetSection("").Bind(config);
         }));
         
         serv.AddScoped<PlayerService>();
         using (var prov = serv.BuildServiceProvider())
         {
             var playerService = prov.GetRequiredService<PlayerService>();
             //终端负责在调用指令时传入参数dotnet run -- PlayerName=Hero PlayerLevel=99 
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
        
         if (_config.Items != null)
         {
             Console.WriteLine("武器: " + _config.Items.Gun);
             Console.WriteLine("载具: " + _config.Items.Car);
         }

         if (_config.Skills != null)
         {
             Console.WriteLine("技能数: " + _config.Skills.Count);
             foreach (var skill in _config.Skills)
             {
                 Console.WriteLine(" - " + skill);
             }
         }
     }
 }
 
 //3.依旧创建对应类(属性有set且和字段相同)
 public class PlayerConfig
 {

     public string? PlayerName { get; set; }
     public int PlayerLevel { get; set; } 
     public ItemInfo? Items { get; set; }    
     public List<string>? Skills { get; set; } 
 }
 
 public class ItemInfo
 {
     public string? Gun { get; set; }
     public string? Car { get; set; }
 }
 
 