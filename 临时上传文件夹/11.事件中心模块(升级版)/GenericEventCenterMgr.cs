using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//之前的事件中心模块,UnityAction只能存放无参数类型的函数,这里添加泛型版(函数就可以有参数了)
//事件容器接口
//接口只是为了实现父类容器的作用,你这里直接用基类也是可以的
public interface IEventContainer { }
//1.带参数的事件容器
public class EventContainer<T> : IEventContainer
{
    public UnityAction<T> actions;

    public EventContainer(UnityAction<T> action)
    {
        actions += action;
    }
}
//2.不带参数的事件容器
//字典可以拆分为: 事件名称Key - 事件本体Value - 事件订阅的多个函数
//EventContainer对应事件容器Value,actions就是事件订阅的多个函数
public class EventContainer : IEventContainer
{
    
    public UnityAction actions;
    //只是构造函数初始化,你这里写成 actions = action;效果也是一样的
    public EventContainer(UnityAction action)
    {
        actions += action;
    }
}

public class GenericEventCenterMgr : MonoSingletonBase<GenericEventCenterMgr>
{
    //通过接口实现同时装填 泛型/非泛型 的事件容器
    private Dictionary<string, IEventContainer> eventDic = new Dictionary<string, IEventContainer>();
    //=========================泛型版本===========================
    public void AddEvent<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            //这里Value是父接口,向下转型为泛型事件容器
            (eventDic[name] as EventContainer<T>).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventContainer<T>(action));
        }
    }
    
    public void RemoveEvent<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
            (eventDic[name] as EventContainer<T>).actions -= action;
    }
    
    public void EventTrigger<T>(string name, T info)
    {
        if (eventDic.ContainsKey(name))
        {
            var container = eventDic[name] as EventContainer<T>;
            if (container != null && container.actions != null)
                container.actions.Invoke(info);
        }
    }
    //=========================非泛型版本===========================
    public void AddEvent(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventContainer).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventContainer(action));
        }
    }
    
    public void RemoveEvent(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
            (eventDic[name] as EventContainer).actions -= action;
    }
    
    public void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            var container = eventDic[name] as EventContainer;
            if (container != null && container.actions != null)
                container.actions.Invoke();
        }
    }
    
    public void Clear()
    {
        eventDic.Clear();
    }
}