 using System;
 using System.Collections.Generic;
 using Microsoft.Extensions.Configuration;
 using Microsoft.Extensions.DependencyInjection;
 using Microsoft.Extensions.Logging;

 internal class Program
 {
     public static void Main(string[] args)
     {
        //1.配置系统构建器(用于往里面添加Json,相当于草稿)
        ConfigurationBuilder configBuilder = new ConfigurationBuilder();
        //2.添加Json配置文件源
        configBuilder.AddJsonFile
        (
            //文件名-文件不存在是否报错-文件修改后是否自动刷新
            "PlayerInfo.json",
            optional: false,
            reloadOnChange: true
        );
        //3.配置系统根对象(构建器添加好数据,就Build生成根对象,以后就从根对象里面读取)
        IConfigurationRoot configRoot = configBuilder.Build();
        
        //4.接下来只需要从根对象里面读取
        //4.1 读取普通字符串和数字(注意都是string还需要转化)
        //string name = configRoot.GetValue<string>("playerName", "Unknown");可以避免读取null
        string playerName = configRoot["playerName"]; 
        //4.2 读取嵌套对象
        //用:冒号表示层级就行了
        string gun = configRoot["Items:Gun"];
        string car = configRoot["Items:Car"];
        //4.3 读取数组列表
        //用:冒号后接索引就行了
        string skill1 = configRoot["Skills:0"];
        string skill2 = configRoot["Skills:1"];
        //4.4 读取某一层级
        //直接先读取某个非叶子节点作为层级,然后通过该层级直接获取子节点,也可以直接foreach遍历所有子节点
        //这样就不用每个都写一遍:冒号层级获取了
        IConfigurationSection itemsSection = configRoot.GetSection("Items");
        //还可以直接遍历获取所有子节点foreach (var child in itemsSection.GetChildren())
        string gunSection = itemsSection["Gun"]; 
        string carSection = itemsSection["Car"];
        
        //5.直接通过配置根对象反序列化
        PlayerConfig player = configRoot.Get<PlayerConfig>();
     }
 }

 //类名和Json文件名[不需要!]相同
 public class PlayerConfig
 {
     //属性名和Json字段名需要相同
     public string PlayerName { get; set; }
     public int PlayerLevel { get; set; }       // 自动把字符串 "13" 转成 int
     public ItemInfo Items { get; set; }        // 对应嵌套对象
     public List<string> Skills { get; set; }   // 对应数组
 }
 
 //同样的,嵌套类名也随便取
 public class ItemInfo
 {
     public string Gun { get; set; }
     public string Car { get; set; }
 }
 
 