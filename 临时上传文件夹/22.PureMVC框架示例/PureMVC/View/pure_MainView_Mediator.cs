using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;

//负责管理UI具体面板与UI框架间的通信
public class pure_MainView_Mediator : Mediator
{
    //查询Facade字典避免重复的时候用得上if (!Facade.HasMediator(pure_MainView_Mediator.NAME)) 
    public static new string NAME = "pure_MainView";
    //虽然啥都没写,还是有初始化NAME字段的作用
    public pure_MainView_Mediator() : base(NAME)
    {
        //界面的创建应该由触发逻辑来实现,而且这么多界面也不能重复写
    }
    
    //重写监听通知的方法
    //双分发机制:当我们SendNotification(XXX)时,Facade同时转发两种路线
    //这里UPDATE_PLAYER_INFO没有 RegisterCommand,但是Mediator路线仍然是Facade转发的
    public override string[] ListNotificationInterests()
    {
        //PureMVC的规则:监听哪些通知,就将其用String[]返回(类似通过事件名注册监听)
        //当有人SendNotification(XXX)-->Facade 遍历所有已注册的 Mediator并且查看其 ListNotificationInterests()
        //如果 XXX 在这个数组里,调它的 HandleNotification
        return new string[]
        {
            pure_Notification.UPDATE_PLAYER_INFO,
            //好吧这里其实也没有必要监听
            // pure_Notification.SHOW_PANEL,
            // pure_Notification.HIDE_PANEL
        };
    }
    
    //重写处理通知的方法
    //要是只有Command路径,这里处理就需要每个面板在Excute中单独写转化并调用UpdateInfo_View
    public override void HandleNotification(INotification notification)
    {
        //INotification对象中包含通知名+通知包含信息
        switch (notification.Name)
        {
            //额,说啥监听了 SendNotification(pure_Notification.UPDATE_PLAYER_INFO);就都会执行更新的方法?
           case pure_Notification.UPDATE_PLAYER_INFO:
               //收到通知所做的处理
               //ViewComponent就是Mediator中具体存储的面板属性
               (ViewComponent as pure_MainView)?.UpdateInfo_View(notification.Body as pure_PlayerDataObj);
                break;
           //这俩已经由command处理了,这里空着也行(不如说注释掉比较好,浪费性能)
           // case pure_Notification.SHOW_PANEL:
           //      break;
           // case pure_Notification.HIDE_PANEL:
           //      break;
        }
    }
    
    //可选: 重写注册时的方法
    //Mediator 被 注册到 Facade 时自动调用(这个注册在pure_ShowPanelCommand.cs 里,不在 Facade)
    public override void OnRegister()
    {
        base.OnRegister();
        //额外的初始化内容
    }
    
    //封装设置View上控件的方法
    public void SetView(pure_MainView view)
    {
        // mediatorMain.SetView(mainPanel.GetComponent<pure_MainView>());
        //显示面板的时候把上面的脚本(提前挂载了View脚本)绑定到ViewComponent
        ViewComponent = view;
        // 统一判空,避免 Inspector 漏拖引用直接崩
        if (view == null||view.btnRole == null)
        {
            Debug.Log("view == null||view.btnRole == null,无法执行");
            return;
        }
        
        view.btnRole.onClick.AddListener(() =>
            {
                Debug.Log("点击了btnRole,自动发送通知");
                SendNotification(pure_Notification.SHOW_PANEL, "RolePanel");
            }
        );
    }
}
