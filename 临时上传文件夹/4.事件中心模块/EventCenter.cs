using System.Collections.Generic;
using UnityEngine.Events;

public class EventCenter : BaseManager<EventCenter>
{
    //string是事件名称,传入函数补充字典泛型,需要用委托
    //这里是string事件加入UnityAction委托对应函数
    //这里用字典主要是方便寻找是否有对应事件以及用参数来添加Key和Value
    private Dictionary<string, UnityAction> eventDic = new Dictionary<string, UnityAction>();
    
    //传入的函数一开始没有被执行,只是被加入了事件订阅列表,所以是回调函数
    //注意这里事件列表实际关联的物体实例+函数 两部分(所以后面销毁订阅呢),除非是静态函数
    public void AddEvent(string name, UnityAction action)
    {
        // 如果字典有对应的事件name，就添加到订阅列表
        if (eventDic.ContainsKey(name))
        {
            eventDic[name] += action;
        }
        else
        {
            // 如果没有，就新增
            eventDic.Add(name, action);
        }
    }

    //物体OnDestory()中必须调用RemoveEvent()移除监听
    public void RemoveEvent(string name, UnityAction action)
    {
        // 理论上移除时必定有事件监听,不用写if也行
        //实际上eventDic[name]可能报空引用异常,传入未注册key导致报错
        if (eventDic.ContainsKey(name))
        {
            eventDic[name] -= action;
        }
    }
    
    //事件触发,外部直接EventCenter.Instance.EventTrigger("OnPlayerShoot");触发指定时事件
    public void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            // 这种会依据加入事件的前后依次触发函数
            eventDic[name].Invoke();
        }
        // 没有对应事件监听时，不需要管触发
    }
    
    //场景切换和游戏结束时,记得清空防止内存泄露
    public void Clear()
    {
        eventDic.Clear();
    }
}