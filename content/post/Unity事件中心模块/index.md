+++
title = "Unity事件中心模块"
date = "2026-05-03T10:35:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "事件中心", "观察者模式"]
+++

Unity事件中心模块基于观察者模式实现，支持参数传递的事件机制。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventCenter : BaseManager<EventCenter>
{
    //Key事件名称,value监听这个事件对应的委托函数们
    //private Dictionary<string,UnityAction> eventDic = new Dictionary<string, UnityAction>();
    private Dictionary<string,UnityAction<object>> eventDic = new Dictionary<string, UnityAction<object>>();

    //添加事件监听
    //public void AddEventListener(string name, UnityAction action)
    public void AddEventListener(string name, UnityAction<object> action)
    {
        //如果有对应的事件监听
        if (eventDic.ContainsKey(name))
        {
            eventDic[name] += action;
        }
        else
        {
            eventDic.Add(name, action);
        }
    }

    //一定要记得销毁对应监听,否则物体销毁的时候还是有监听,导致内存泄露
    //public void RemoveEventListener(string name, UnityAction action)
    public void RemoveEventListener(string name, UnityAction<object> action)
    {
        //移除时必定有事件监听,都可以不用写if的
        if (eventDic.ContainsKey(name))
        {
            eventDic[name] -= action;
        }
    }

    //事件触发
    //public void EventTrigger(string name)
    public void EventTrigger(string name,object info)
    {
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name]();
            //这种会依据加入事件的前后依次触发函数
            //eventDic[name].Invoke();
            eventDic[name].Invoke(info);
        }
        //没有对应事件监听时,不需要管触发
    }

    //清空事件中心(场景切换时)
    public void Clear()
    {
        eventDic.Clear();
    }
}
```

## 功能特点

### 观察者模式
- 使用 Dictionary 存储事件名称和对应的事件委托
- 支持同一事件绑定多个监听函数
- 事件触发时按绑定顺序依次执行

### 参数传递
- 使用 `UnityAction<object>` 支持任意类型的参数传递
- 可以传递复杂数据类型作为事件信息

### 内存管理
- **重要**：在对象销毁时必须移除事件监听
- 否则会导致内存泄漏和空引用异常
- 场景切换时调用 `Clear()` 清空所有事件

### 使用方法

```csharp
// 1. 定义事件
public class GameEventNames
{
    public const string PLAYER_DEAD = "PlayerDead";
    public const string SCORE_UPDATE = "ScoreUpdate";
    public const string LEVEL_COMPLETE = "LevelComplete";
}

// 2. 监听事件
void Start()
{
    EventCenter.Instance.AddEventListener(GameEventNames.PLAYER_DEAD, OnPlayerDead);
    EventCenter.Instance.AddEventListener(GameEventNames.SCORE_UPDATE, OnScoreUpdate);
}

private void OnPlayerDead(object info)
{
    Debug.Log("玩家死亡：" + info);
}

private void OnScoreUpdate(object info)
{
    int score = (int)info;
    Debug.Log("分数更新：" + score);
}

// 3. 触发事件
public void PlayerTakeDamage(int damage)
{
    EventCenter.Instance.EventTrigger(GameEventNames.PLAYER_DEAD, damage);
}

public void UpdateScore(int score)
{
    EventCenter.Instance.EventTrigger(GameEventNames.SCORE_UPDATE, score);
}

// 4. 移除监听（重要！）
void OnDestroy()
{
    EventCenter.Instance.RemoveEventListener(GameEventNames.PLAYER_DEAD, OnPlayerDead);
    EventCenter.Instance.RemoveEventListener(GameEventNames.SCORE_UPDATE, OnScoreUpdate);
}

// 5. 场景切换时清空
void OnLevelWasLoaded(int level)
{
    EventCenter.Instance.Clear();
}
```

## 注意事项

1. **必须移除监听**：在 `OnDestroy` 或 `OnDisable` 中移除事件监听
2. **及时清空**：场景切换时调用 `Clear()` 防止内存泄漏
3. **类型安全**：使用 `object` 参数需要注意类型转换
4. **线程安全**：Unity事件系统在主线程执行，无需担心线程问题
