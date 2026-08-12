using System;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// 泛型事件中心 - 零装箱高性能版本
/// 继承 BaseManager 实现单例模式
/// </summary>
public class EventCenter_T : BaseManager<EventCenter_T>
{
    private Dictionary<Type, Dictionary<string, Delegate>> eventDic = new Dictionary<Type, Dictionary<string, Delegate>>();

    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        Type typeKey = typeof(T);
        if (!eventDic.ContainsKey(typeKey))
        {
            eventDic[typeKey] = new Dictionary<string, Delegate>();
        }

        var typeDic = eventDic[typeKey];
        
        if (typeDic.ContainsKey(name))
        {
            typeDic[name] = (UnityAction<T>)typeDic[name] + action;
        }
        else
        {
            typeDic.Add(name, action);
        }
    }
    
    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        Type typeKey = typeof(T);

        if (eventDic.TryGetValue(typeKey, out var typeDic))
        {
            if (typeDic.TryGetValue(name, out var del))
            {
                typeDic[name] = (UnityAction<T>)del - action;
            }
        }
    }
    
    public void EventTrigger<T>(string name, T data)
    {
        Type typeKey = typeof(T);

        if (eventDic.TryGetValue(typeKey, out var typeDic))
        {
            if (typeDic.TryGetValue(name, out var del))
            {
                ((UnityAction<T>)del)?.Invoke(data);
            }
        }
    }
    
    public void Clear()
    {
        foreach (var typeDic in eventDic.Values)
        {
            typeDic.Clear();
        }
        eventDic.Clear();
    }
}