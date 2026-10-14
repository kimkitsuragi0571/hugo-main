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
        ConfigurationBuilder configBuilder = new ConfigurationBuilder();
        configBuilder.AddJsonFile
        (
            "PlayerInfo.json",
            optional: false,
            reloadOnChange: true
        );
        //4.创建另一个构建器,Add并build另一个Root
        ConfigurationBuilder serverBuilder = new ConfigurationBuilder();
        serverBuilder.AddJsonFile
        (
            "ServerInfo.json",
            optional: false,
            reloadOnChange: true
        );
        IConfigurationRoot configRoot = configBuilder.Build();
        IConfigurationRoot serverRoot = serverBuilder.Build();
        
        var serv = new ServiceCollection();
        //哦下面有工厂注册的写法这里我就注释掉了
        //serv.AddOptions<PlayerConfig>().Bind(configRoot); 
        serv.AddOptions<ServerConfig>().Bind(serverRoot);
        
        //6.选项方法的工厂注册写法(提前链式调用Bind,指定构建器)
        //链式写法:泛型和参数都只能是同一配置类PlayerConfig,按照顺序执行
        serv.AddOptions<PlayerConfig>()
            .Bind(configRoot)//这里就是依次链式调用Bind->第一步->第二步
            .Configure((PlayerConfig config) =>
            {
                Console.WriteLine("第一步");
                config.PlayerName = "Ken";
            })
            .Configure((PlayerConfig config) =>
            {
                Console.WriteLine("第二步");
                config.PlayerLevel = 99;
            });
        //7.工厂注册的另一种写法(不通过提前链式调用实现)
        serv.AddOptions<ServerConfig>()
            .Configure(config => 
            {
                //通过闭包引用外部serverRoot根对象,然后Bind
                serverRoot.Bind(config); 
                //然后就可以修改了
                config.Port = 9999; 
            });

        //8.业务类只需要注册同一个就行
        serv.AddScoped<PlayerService>();
        
        using (var prov = serv.BuildServiceProvider())
        {
            var playerService = prov.GetRequiredService<PlayerService>();
            playerService.ShowPlayerInfo();
        }
     }
 }
 
 public class PlayerService
 {
     private readonly PlayerConfig _config;
     //2.业务类里面新增额外注入即可,不用专门新增业务类
    private readonly ServerConfig _serverConfig;
     public PlayerService(IOptions<PlayerConfig> options, IOptions<ServerConfig> serverOptions)
     {
        _config = options.Value;
        _serverConfig = serverOptions.Value; 
     }

     public void ShowPlayerInfo()
     {
        Console.WriteLine("玩家: " + _config.PlayerName);
        Console.WriteLine("等级: " + _config.PlayerLevel);
         //3.这里就可以直接使用另一个配置类的数据了
        Console.WriteLine("服务器IP: " + _serverConfig.Ip);
        Console.WriteLine("服务器端口: " + _serverConfig.Port);
     }
 }

  //1.这里新增额外配置类
 public class ServerConfig
{
    public string Ip { get; set; }
    public int Port { get; set; }
}

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
 
 