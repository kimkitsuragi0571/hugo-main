+++
title = "Unity 支持泛型的事件中心模块"
date = "2026-08-25T19:24:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "事件中心", "泛型", "UnityAction", "委托"]
+++

前面的事件中心模块中，`UnityAction` 只能存放无参数类型的函数。升级为泛型版本后，事件可以携带参数（比如怪物死亡时传递怪物类型、玩家攻击时传递伤害值），同时**兼容无参数版本**。

## 一、核心思路

普通事件中心用 `Dictionary<string, UnityAction>` 存储事件，Key 是事件名，Value 是无参委托。问题在于：**所有事件都只能是无参的**。

泛型版本的做法是引入一个**事件容器接口** `IEventContainer`，再派生出两个实现：
- `EventContainer<T>` — 带参数的容器，内部存 `UnityAction<T>`
- `EventContainer` — 不带参数的容器，内部存 `UnityAction`

字典统一存 `Dictionary<string, IEventContainer>`，通过向下转型（`as`）取回具体容器，这样**一个字典同时装泛型和非泛型事件**。

---

## 二、完整脚本

```csharp
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
    protected override void Awake()
    {
        base.Awake();
        print("哔哔哔......事件中心模块(支持泛型,避免装箱)启动");
    }
    
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
```

---

## 三、结构拆解

### 3.1 事件容器体系

```
IEventContainer（空接口，仅用于统一字典类型）
├── EventContainer<T>  ← 带参数：UnityAction<T> actions
└── EventContainer     ← 无参数：UnityAction actions
```

| 容器 | 内部委托 | 适用场景 |
|------|---------|---------|
| `EventContainer<T>` | `UnityAction<T>` | 事件需要传参数（如 `OnMonsterDead` 传递怪物类型） |
| `EventContainer` | `UnityAction` | 事件无参数（如 `OnPlayerShoot` 只通知"开火了"） |

> 为什么用接口？因为 `Dictionary` 的 Value 只能是单一类型。用 `IEventContainer` 作为父类型，泛型容器和非泛型容器都能装进同一个字典。

### 3.2 泛型版本方法

| 方法 | 签名 | 说明 |
|------|------|------|
| `AddEvent<T>` | `(string name, UnityAction<T> action)` | 向下转型 `as EventContainer<T>`，订阅到 `actions` |
| `RemoveEvent<T>` | `(string name, UnityAction<T> action)` | 向下转型后取消订阅 |
| `EventTrigger<T>` | `(string name, T info)` | 向下转型后 `Invoke(info)`，**传入参数** |

### 3.3 非泛型版本方法

| 方法 | 签名 | 说明 |
|------|------|------|
| `AddEvent` | `(string name, UnityAction action)` | 向下转型 `as EventContainer`，订阅到 `actions` |
| `RemoveEvent` | `(string name, UnityAction action)` | 取消订阅 |
| `EventTrigger` | `(string name)` | `Invoke()` 无参数触发 |

---

## 四、使用示例

### 泛型事件（带参数）

```csharp
// 订阅：怪物死亡事件，携带怪物类型信息
GenericEventCenterMgr.Instance.AddEvent<string>("OnMonsterDead", (monsterType) => {
    Debug.Log($"怪物死亡，类型：{monsterType}");
    // 任务系统记录、奖励系统发放、动画系统播放……
});

// 触发：怪物死亡时传递类型
GenericEventCenterMgr.Instance.EventTrigger("OnMonsterDead", "Goblin");
```

### 非泛型事件（无参数）

```csharp
// 订阅：玩家射击事件
GenericEventCenterMgr.Instance.AddEvent("OnPlayerShoot", () => {
    Debug.Log("玩家开火了");
});

// 触发
GenericEventCenterMgr.Instance.EventTrigger("OnPlayerShoot");
```

---

## 五、与普通事件中心对比

| 特性 | EventCenterMgr（普通） | GenericEventCenterMgr（泛型） |
|------|----------------------|-------------------------------|
| 字典 Value 类型 | `UnityAction` | `IEventContainer`（接口） |
| 支持参数 | ❌ 仅无参 | ✅ 泛型 + 无参 |
| 容器抽象 | 无（直接存委托） | `IEventContainer` 接口 + 两个实现类 |
| 向下转型 | 不需要 | 需要 `as EventContainer<T>` / `as EventContainer` |
| 类型安全 | 编译期确定 | 泛型方法编译期确定，运行时靠容器类型 |
| 复杂度 | 低 | 中（多了容器体系） |
| 适用场景 | 简单通知型事件 | 需要传数据的事件驱动 |

> 💡 泛型版本**完全兼容**无参数事件——非泛型方法和泛型方法共存于同一个字典，通过接口向下转型区分。
