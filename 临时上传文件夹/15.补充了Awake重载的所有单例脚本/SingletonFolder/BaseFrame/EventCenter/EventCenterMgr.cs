using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventCenterMgr : MonoSingletonBase<EventCenterMgr>
{
    protected override void Awake()
    {
        base.Awake();
        print("哔哔哔......事件中心模块启动");
    }

    //字典用于存储 事件名-UnityAction具体委托(在value中添加函数订阅)
    private Dictionary<string,UnityAction> eventDict = new Dictionary<string,UnityAction>();
    //添加订阅
    public void AddEvent(string name, UnityAction act)
    {
        if (eventDict.ContainsKey(name))
        {
            //添加到value事件订阅
            eventDict[name] += act;
        }
        else
        {
            //没有还需要创建K-V对用于存储指定委托
            eventDict.Add(name, act);
        }
    }

    public void RemoveEvent(string name, UnityAction act)
    {
        if (eventDict.ContainsKey(name))
        {
            //对应nameKey的委托去掉函数订阅
            eventDict[name] -= act;
        }
    }

    public void EventTrigger(string name)
    {
        if (eventDict.ContainsKey(name))
        {
            eventDict[name]?.Invoke();
        }
    }

    public void Clear()
    {
        //字典对象自带的清空方法
        eventDict.Clear();
    }
    
}
