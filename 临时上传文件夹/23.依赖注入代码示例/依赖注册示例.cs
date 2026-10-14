 using System;
 using Microsoft.Extensions.DependencyInjection;

 internal class Program
 {
     public static void Main(string[] args)
     {
         //1.创建DI注册表serv
         //对比WebApi就需要先手动创建services容器对象(相当于一个List)
         var serv = new ServiceCollection();
         
         //2.DI注册
         //如果你还注册了轻攻击LightAtk, GetRequiredService<IAtk>()只会取最后注册的那个
         serv.AddScoped<IAtk, ReAtk>();
         //这里Player类也需要注册,之后调用player实例也需要避免new Player()
         serv.AddScoped<Player>();

         //3.创建DI容器prov
         //调用serv.BuildServiceProvider()拓展方法
         using (var prov = serv.BuildServiceProvider())
         { 
             //让DI容器返回一个实现类
         //容器检查Player构造函数,发现需要IAtk参数->DI注册表中查看谁对应IATK->传入ReAtk实例
             var player = prov.GetRequiredService<Player>();
             player.PlayerAtk(); 
         }
         
         // //工厂注册,参数传入lambda表达式
         // //虽然也是ServiceProvider类对象(即DI容器),但是这里的只是当前作用域对象,之前的是根容器
         // string atkType = "Heavy";
         // serv.AddScoped<IAtk>(new Func<IServiceProvider, IAtk>((IServiceProvider prov) =>
         //     {
         //         if (atkType == "Heavy")
         //         {
         //             return new ReAtk();
         //         }
         //         else
         //         {
         //             return new LightAtk();
         //         }
         //     }
         // ));
     }
 }

 public interface IAtk
 {
     void Atk();
 }
 
 public class ReAtk:IAtk
 {
     public void Atk()
     {
         Console.WriteLine("重攻击");
     }
 }

 public class LightAtk:IAtk
 {
     public void Atk()
     {
         Console.WriteLine("轻攻击");
     }
 }
 
 //让Player只负责 “使用攻击”，不负责 “决定用什么攻击”
 public class Player
 {
     //1.就是让外部的"攻击行为"确定下来后就不能改变
     //这里变量名实际上不适用reatk,接口注入的核心就是不关注具体实现
     private readonly IAtk _atk;
     
     //2.就是说创建Player实例时,外部必须提供具体的攻击行为
     //这里实际传入的就是具体实现类
     public Player(IAtk atk)
     {
         this._atk = atk;
     }
     //3.调用PlayerAtk就是调用外部已经确定好的攻击行为
     public void PlayerAtk()
     {
         this._atk.Atk();
     }
 }
 