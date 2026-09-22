using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Facade;
using UnityEngine;

//Facade负责初始化框架，并统一管理 Model、View 和 Controller 三大核心模块
//1.首先继承
public class pure_Facade : Facade
{
   //父类Facade里面有 protected static IFacade instance;
   //和单例基类一个作用就对了,只是把向下转型放在了子类来写
    public static pure_Facade Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new pure_Facade();
            }
            //返回值为子类这里就要向下转型了(因为instance是IFacade类型嘛)
            return instance as pure_Facade;
        }
    }
    
    //Facade通过IController,IModel,IView三个接口管理对应的Core模块
    //初始化Controller相关的方法
    protected override void InitializeController()
    {
        base.InitializeController();
        //相当于在字典中添加映射,当SendNotification("START_UP")时,调用工厂(其中是怎么制造command的逻辑)
        //这里我们写的逻辑(或者说工厂函数)就是直接new一个pure_StartUpCommand实例(注意此时并不会执行Excute)
        RegisterCommand(pure_Notification.START_UP, () =>
        {
            //return new时controller自动调用类中的Execute函数
            return new pure_StartUpCommand();
        });
        
        RegisterCommand(pure_Notification.SHOW_PANEL, () =>
        {
            return new pure_ShowPanelCommand();
        });
        
        RegisterCommand(pure_Notification.HIDE_PANEL, () =>
        {
            return new pure_HidePanelCommand();
        });
        
        RegisterCommand(pure_Notification.LEVEL_UP, () =>
        {
            return new pure_LevUpCommand();
        });
    }
    
    //一定有一个启动函数(我们在Entry中调用的)
    public void StartUp()
    {
        //发送通知-->返回new pure_StartUpCommand()-->执行 Debug.Log("执行命令时自动调用的函数");
        SendNotification(pure_Notification.START_UP);
        
        //发送通知的时候也可以传入参数body,就是个Object类实例,收到通知直接Body取出要用的成员就行
        SendNotification(pure_Notification.SHOW_PANEL, "MainPanel");
    }
}
