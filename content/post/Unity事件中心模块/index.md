+++
title = "Unity事件中心模块"
date = "2026-07-19T18:55:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "事件中心", "观察者模式"]
+++

## 一、事件中心概念

比如怪物死亡就有玩家奖励，播放动画，任务记录等多种事件，此时就得在怪物死亡 Dead() 方法中写这些逻辑。

需要先获取 Player 物体然后获取上面的 Player.cs 脚本组件，调用其中的死亡方法，很麻烦，而且脚本间关联性太强，不符合七大设计原则。

所以我们可以设置**事件中心**：怪物死亡就传递给事件中心，事件中心传递给各种事件让他们执行；事件自己也得告诉事件中心：XX 触发时，就告诉我执行 XX 逻辑。

---

## 二、基础版事件中心实现

### 1. 事件中心 EventCenter

使用 `Dictionary<string, UnityAction>` 存储事件名称与对应的委托回调。

```csharp
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
```

### 2. 事件订阅

```csharp
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int HP = 100;

    //物体添加到订阅列表
    void Awake()
    {
        //看似只是存储了一个函数名,实际委托间接创建了一个类实例引用和方法本身的引用
        //总之事件的订阅列表实际上其实关联了实例方法和实例两部分
        EventCenter.Instance.AddEvent("OnPlayerShoot", OnBeShot);
    }

    void OnBeShot()
    {
        //这里定义类字段HP,并且访问HP时
        //默认相当于this.HP -= 10,优先访问
        HP -= 10;
        if (HP <= 0)
        {
            Debug.Log("敌人死亡！");
            // 死亡时销毁物体，触发 OnDestroy
            Destroy(gameObject);
        }
    }

    //物体销毁时移除订阅
    void OnDestroy()
    {
        //即使方法内部只是Debug.Log仍然需要销毁订阅(因为委托还是关联了实例)
        //如果是静态方法就不需要销毁订阅了
        EventCenter.Instance.RemoveEvent("OnPlayerShoot", OnBeShot);
    }
}
```

### 3. 事件触发

```csharp
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    void Update()
    {
       
        if (Input.GetMouseButtonDown(0))
        {
            EventCenter.Instance.EventTrigger("OnPlayerShoot");
        }
    }
}
```

也可以用 `EventCenter.Instance.EventTrigger("PlayerRun", this)`，从原来的只是广播"出事辣"变为广播"我出事辣"。

`UnityAction` 改为 `UnityAction<object>`，并更新增删触发方法。触发和监听脚本都要修改，具体懒得搞了。

---

## 三、执行流程

1. **实例化敌人**：`Instantiate(enemyObj)` → 触发 `EnemyScript.Awake()` → 从而将函数添加到事件列表
2. **用户按下按键**：调用 `EventTrigger("OnPlayerShoot")` → EventCenter 根据 Key 执行列表
   - 如果有多个敌人，一个 Key 对应多个 Value 全部执行
3. **敌人销毁**：触发 `OnDestroy()` → 移除该实例相关订阅
   - 多个物体多个 Value，只会移除那一个而不是全都移除

---

## 四、优化：泛型版本解决装箱拆箱

### 泛型委托的意义

比如 Monster 死了你咋知道是 BOSS 还是 Goblin 死了？

这里把委托 `UnityAction` 都改成泛型委托 `UnityAction<object>`：
- 传入的是万物之父 `object`，这样你传入什么类型都可以
- 装箱拆箱肯定还是有性能开销
- 装箱拆箱 + 泛型，还有这种用法的哦嚯嚯嚯

### EventCenter 优化为泛型版本

使用 `Dictionary<Type, Dictionary<string, Delegate>>` 双层字典结构，实现零装箱高性能版本。

```csharp
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
```

**优化原理：**
- 第一层 Key 是参数类型 `Type`，不同类型的事件完全隔离
- 第二层 Key 是事件名称 `string`，存储具体的 `UnityAction<T>` 委托
- 存储时用 `Delegate` 基类，调用时强转回 `UnityAction<T>`
- 全程没有 `object` 装箱，零性能损耗
