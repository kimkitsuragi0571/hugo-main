using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using PureMVC.Patterns.Mediator;
using UnityEngine;

public class pure_HidePanelCommand : SimpleCommand
{
    //依旧重写方法
    //return new时controller自动调用类中重写的Execute函数
    //controller自动把 SendNotification(pure_Notification.HIDE_PANEL, this);的通知名和参数组合为INotification 对象
    public override void Execute(INotification notification)
    {
        base.Execute(notification);
        Debug.Log("执行pure_HidePanelCommand命令,自动调用其中函数");
        //面板隐藏相关的具体逻辑
        //因为我们之前传入的this就是pure_RoleView_Mediator实例嘛,就把这里notification.Body(Object类型)向下转型
        Mediator mediator = null;
        // body 是字符串:当做面板名,自己查 Mediator
        if (notification.Body is string panelName)
        {
            // 约定:面板名 = Mediator.NAME(比如 "MainPanel" 对应 pure_MainView_Mediator.NAME)
            mediator = Facade.RetrieveMediator(panelName) as Mediator;
        }
        // body 是 Mediator:直接用
        else if (notification.Body is Mediator)
        {
            mediator = notification.Body as Mediator;
        }
        if (mediator != null && mediator.ViewComponent != null)
        {
            //直接删除场景上对象(AI建议还是改为SetActive方案)
            //HidePanel要处理多种面板,这里就不要转化为具体面板如pure_RoleView
            //所有面板都是Mono的子类,这里就相当于Object爷转化为Mono儿,而不是具体Panel孙
            GameObject.Destroy((mediator.ViewComponent as MonoBehaviour).gameObject);
            //删掉后一定要置空
            mediator.ViewComponent = null;
        }
    }
}
