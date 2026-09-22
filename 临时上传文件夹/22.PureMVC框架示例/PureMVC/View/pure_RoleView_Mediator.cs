using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;

public class pure_RoleView_Mediator : Mediator
{
    public static new string NAME = "pure_RoleView";
    public pure_RoleView_Mediator() : base(NAME)
    {
        //这时面板还没有实例化,没有view对象能拿来赋值,只能默认null了
        
        //实际父类构造函数有(string mediatorName, object viewComponent = null)
        //这里没有给viewComponent赋值,放在SetView(pure_RoleView view)里面
        
    }
    
    //重写监听通知的方法
    public override string[] ListNotificationInterests()
    {
        return new string[]
        {
            pure_Notification.UPDATE_PLAYER_INFO,
            pure_Notification.SHOW_PANEL,
            pure_Notification.HIDE_PANEL
        };
    }
    
    //重写处理通知的方法
    public override void HandleNotification(INotification notification)
    {
        switch (notification.Name)
        {
            case pure_Notification.UPDATE_PLAYER_INFO:
                //这里按LevUp就会报错,好像是这个时候ViewComponent已经为空,所以需要判空
                //UPDATE_PLAYER_INFO 触发时 RolePanel 已关闭,Mediator 仍会接收通知,有点浪费
                if (ViewComponent != null)
                {
                    (ViewComponent as pure_RoleView)?.UpdateInfo_View(notification.Body as pure_PlayerDataObj);
                }
                break;
            case pure_Notification.SHOW_PANEL:
                break;
            case pure_Notification.HIDE_PANEL:
                break;
        }
    }
    
    //封装设置View上控件的方法
    public void SetView(pure_RoleView view)
    {
        //哦哦是在这里赋值的,这tm太容易看漏了
        ViewComponent = view;
        // 统一判空,避免 Inspector 漏拖引用直接崩
        if (view == null || view.btnClose == null || view.btnLevUp == null)
        {
            Debug.Log("view == null||view.btnClose == null,无法执行");
            return;
        }
        //监听关闭按钮
        view.btnClose.onClick.AddListener(() =>
            {
                Debug.Log("点击了btnClose,自动发送HIDE_PANEL通知");
                //这里this就是pure_RoleView_Mediator实例,因为HidePanelCommand会访问ViewComponent字段
                //注意ViewComponent初始=null,上面才 ViewComponent = view;赋值
                SendNotification(pure_Notification.HIDE_PANEL, this);
            }
        );
        //监听升级按钮
        view.btnLevUp.onClick.AddListener(() =>
            {
                Debug.Log("点击了btnLevUp,自动发送通知");
                //这里其实没用到this
                SendNotification(pure_Notification.LEVEL_UP, this);
            }
        );
    }
    
    //可选: 重写注册时的方法
    public override void OnRegister()
    {
        base.OnRegister();
        //额外的初始化内容
    }
}
