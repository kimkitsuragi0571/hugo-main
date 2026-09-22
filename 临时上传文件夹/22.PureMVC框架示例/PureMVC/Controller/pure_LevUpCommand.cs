using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityEngine;

public class pure_LevUpCommand : SimpleCommand
{
    public override void Execute(INotification notification)
    {
        //好吧,没啥卵用的代码,留着纯习惯
        base.Execute(notification);
        //得到数据代理,调用升级,完成后通知别人更新数据
        //前面StartUpCommand中注册了,反正RegisterProxy和RetrieveProxy这俩方法知道是存取同一个实例就行
        pure_PlayerProxy playerProxy = Facade.RetrieveProxy(pure_PlayerProxy.NAME) as pure_PlayerProxy;
        if (playerProxy != null)
        {
            playerProxy.LevUp();
            //注意发送的不是LEV_UP通知
            SendNotification(pure_Notification.UPDATE_PLAYER_INFO, playerProxy.Data);
        }
    }
}
