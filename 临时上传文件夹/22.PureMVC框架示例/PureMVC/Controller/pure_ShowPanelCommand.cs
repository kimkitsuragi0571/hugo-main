using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityEngine;

public class pure_ShowPanelCommand : SimpleCommand
{
   //依旧重写方法
    public override void Execute(INotification notification)
    {
        base.Execute(notification);
        Debug.Log("执行pure_ShowPanelCommand命令,自动调用其中函数");
        //面板创建相关的具体逻辑
        //SendNotification(pure_Notification.SHOW_PANEL, "MainPanel");
        //这里notification应该是包含Name和Body两部分?
        string panelName = notification.Body.ToString();
        switch (panelName)
        {
            case "MainPanel":
            //显示面板相关的逻辑
          
            //使用Mediator前也一定要在Facade里面去注册(和Command,Proxy都一样)
            //直接在命令中使用Facade,代表全局唯一的Facade
            //额...每个面板的Mediator也是全局唯一的?
            if (!Facade.HasMediator(pure_MainView_Mediator.NAME))
            {
                //这里就会调用pure_MainView_Mediator构造函数
                Facade.RegisterMediator(new pure_MainView_Mediator());
            }
            //通过Facade获取Mediator的方法
            pure_MainView_Mediator mediatorMain =
                Facade.RetrieveMediator(pure_MainView_Mediator.NAME) as pure_MainView_Mediator;

            //后面写了的,显示面板时会把面板上脚本绑定到ViewComponent
            if (mediatorMain.ViewComponent == null)
            {
                //有了Mediator 下一步就是创建界面预设体(已经提前挂载好了View脚本)
                GameObject res = Resources.Load<GameObject>("Panel/MainPanel");
                GameObject mainPanel = GameObject.Instantiate(res);
                mainPanel.transform.SetParent(GameObject.Find("Canvas").transform, false);
                //获取预设体上面的View脚本然后关联到Mediator上
                //mediatorMain.ViewComponent = mainPanel.GetComponent<pure_MainView>();
                //还可以直接获取控件
               //(mediatorMain.ViewComponent as pure_MainView).btnRole
               
               //但是这里还是使用对应mediator类中的封装方法吧
               //面板View脚本(预制体中已经挂载)绑定到Mediator .ViewComponent
               mediatorMain.SetView(mainPanel.GetComponent<pure_MainView>());
               
               //实现面板后需要进行第一次更新
               //需要把数据通过参数一起传出去
               var proxyMain = Facade.RetrieveProxy(pure_PlayerProxy.NAME) as pure_PlayerProxy;
               if (proxyMain != null)
                   //通过Mediator线路,实现不需要每个面板单独写转化并调用UpdateInfo_View逻辑
                   SendNotification(pure_Notification.UPDATE_PLAYER_INFO, proxyMain.Data);
               else
                   Debug.LogError("pure_PlayerProxy 未注册,跳过第一次 UPDATE_PLAYER_INFO 刷新");
            }
            break;
            
            case "RolePanel":
                if (!Facade.HasMediator(pure_RoleView_Mediator.NAME))
                {
                    Facade.RegisterMediator(new pure_RoleView_Mediator());
                }
                pure_RoleView_Mediator mediatorRole =
                    Facade.RetrieveMediator(pure_RoleView_Mediator.NAME) as pure_RoleView_Mediator;

                if (mediatorRole.ViewComponent == null)
                {
                    GameObject res = Resources.Load<GameObject>("Panel/RolePanel");
                    GameObject rolePanel = GameObject.Instantiate(res);
                    rolePanel.transform.SetParent(GameObject.Find("Canvas").transform, false);
                    mediatorRole.SetView(rolePanel.GetComponent<pure_RoleView>());
                    //同样需要更新
                    var proxyROle = Facade.RetrieveProxy(pure_PlayerProxy.NAME) as pure_PlayerProxy;
                    if (proxyROle != null)
                        SendNotification(pure_Notification.UPDATE_PLAYER_INFO, proxyROle.Data);
                    else
                        Debug.LogError("pure_PlayerProxy 未注册,跳过第一次 UPDATE_PLAYER_INFO 刷新");
                }
                break;
        }
    }
}
