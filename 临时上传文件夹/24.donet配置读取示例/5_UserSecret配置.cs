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
        //1.安装Microsoft.Extensions.Configuration.UserSecrets包
        //2.项目目录打开终端,执行dotnet user-secrets init,在csproj文件中生成唯一Id
        //3.第一次在命令行写入密码dotnet user-secrets set "Player:Password" "123",自动创建secrets.json
        //注意在C盘生成目录而不是项目目录哈,所以提交Git项目时ID不会被泄露
        
        //4.挂载到配置构建器
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddJsonFile("PlayerInfo.json",true, true);
        //多个配置源时,后配置的优先级高,这里就是优先级高于普通Json,替换掉了对应内容
        //这种写法只能是secrets.json用于替换普通json,不能是随便写一个.json泛型改为对应名就可以用于替换普通Json了
        //而泛型替换的意义在于配置源分散在不同项目/类库时,用于指定读取哪个项目的 secrets.json
        //比如configBuilder.AddUserSecrets<PlayerConfig>(); 读取Game.Core 项目里的PlayerConfig
        configBuilder.AddUserSecrets<Program>();
        
        //5.执行流程
        //5.1 AddJsonFile("PlayerInfo.json")读取公开的json配置,相当于默认值
        //5.2 AddUserSecrets<Program>()总之指定系统运行时去C盘找secrets.json
        //5.3 读取C盘中secrets.json中对应内容用于替代PlayerInfo.json(不能是随便一个.json用于覆盖PlayerInfo.json)
        
        //5.4 替换PlayerInfo.json中的其他内容:dotnet user-secrets set "PlayerName" "真实玩家名字"
        
        //因为真正的内容存储在本地C盘,git上传项目时不会暴露,但换电脑就没有对应密码了
        //这只是测试环境下防止git暴露密码而已,正式环境肯定不能用的

        //接下来就是验证输出环节喵
        var configRoot = configBuilder.Build();
        var serv = new ServiceCollection();
        serv.AddOptions<PlayerConfig>()
            .Bind(configRoot);
        serv.AddScoped<PlayerService>();

        using (var prov = serv.BuildServiceProvider())
        {
            var playerService = prov.GetRequiredService<PlayerService>();
            playerService.ShowPlayerInfo();
     }
 }
 
 public class PlayerService
 {
     private readonly PlayerConfig _config;
     
     public PlayerService(IOptions<PlayerConfig> options)
     {
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
 
 