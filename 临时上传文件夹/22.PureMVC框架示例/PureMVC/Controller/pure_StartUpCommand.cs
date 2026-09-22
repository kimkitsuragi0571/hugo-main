using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityEngine;

//继承SimpleCommand
public class pure_StartUpCommand : SimpleCommand
{
   //重写其中的执行函数(命令被执行时自动调用该方法)
   public override void Execute(INotification notification)
   {
      base.Execute(notification);
      Debug.Log("执行pure_StartUpCommand命令,自动调用其中函数");
      //避免重复,没有有数据代理才注册
      if(!Facade.HasProxy(pure_PlayerProxy.NAME))
      {
         //pure_PlayerProxy 实例存进Facade字典
         //后面任何地方(Command、Mediator)都能通过 Facade.RetrieveProxy(NAME) 拿到同一个实例
         //不注册就每次 new 一个新 Proxy,数据各存各的,升级了也白升,下次拿又是初始值
         Facade.RegisterProxy(new pure_PlayerProxy()); 
      }
   }
}
