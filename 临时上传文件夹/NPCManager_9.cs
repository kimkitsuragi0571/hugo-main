using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager_9 : Singleton_3<NPCManager_9>
{
    protected override void Awake()
    {
        base.Awake();
        Debug.Log("继承自动创建obj的单例基类拓展Awake逻辑");
    }

    public void DataInit()
    {
        Debug.Log("NPCManager-自动创建obj的单例模式基类");
    }

}
