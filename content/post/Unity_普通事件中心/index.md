+++
title = "Unity 普通事件中心(仅支持无参函数订阅)"
date = "2026-08-25T19:24:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "事件中心", "UnityAction", "委托", "解耦"]
+++

事件中心模块解决的核心问题是**脚本间强耦合**：比如怪物死亡时要触发玩家奖励、播放动画、任务记录等多种逻辑，传统做法是在怪物 `Dead()` 方法中获取 Player 物体再调用其方法，脚本间关联性太强，不符合七大设计原则。

事件中心的思想是：**怪物死亡时通知事件中心，事件中心再通知所有订阅了这个事件的脚本执行各自逻辑。** 订阅者也要提前告诉事件中心："XX 事件触发时，请执行我的 XX 方法。"

本篇是基础版事件中心，仅支持无参函数订阅（`UnityAction`）。

---

## 一、事件中心：EventCenterMgr

继承 `MonoSingletonBase`，用 `Dictionary<string, UnityAction>` 存储事件名到委托的映射。

```csharp
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
```

### 核心方法

| 方法 | 作用 |
|------|------|
| `AddEvent(name, act)` | Key 存在则 `+=` 订阅；不存在则 `Add` 创建新 K-V 对 |
| `RemoveEvent(name, act)` | Key 存在则 `-=` 取消订阅（移除单个函数，不影响其他订阅者） |
| `EventTrigger(name)` | Key 存在则 `?.Invoke()` 触发该事件下所有订阅的函数 |
| `Clear()` | 清空整个字典 |

> ⚠️ 一个 Key 可以对应多个 Value（多个函数订阅同一事件），触发时全部执行。

---

## 二、事件订阅者：MonsterEvent

怪物脚本，在 `Awake` 中订阅 `OnPlayerAtk` 事件，`OnDestroy` 中取消订阅。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEvent : MonoBehaviour
{
   public int HP = 100;

   void Awake()
   {
      EventCenterMgr.Instance.AddEvent("OnPlayerAtk",PlayerAtk);
   }

   void PlayerAtk()
   {
      this.HP -= 10;
      if (this.HP <= 0)
      {
         print("Player打史了Monster");
         Destroy(this.gameObject);
      }
   }

   //哦哦就是普通的物体销毁时调用
   //不仅要销毁物体,还需要移除订阅引用,防止内存泄漏
   void OnDestroy()
   {
      EventCenterMgr.Instance.RemoveEvent("OnPlayerAtk",PlayerAtk);
   }
}
```

### 关键点

- **Awake 订阅**：场景中每个怪物实例都在 Awake 注册自己的 `PlayerAtk` 方法到同一个 Key
- **OnDestroy 取消订阅**：物体销毁时必须移除订阅，否则事件中心还持有已销毁对象的委托引用，触发时会导致空引用异常或内存泄漏
- **多怪物场景**：多个怪物订阅同一 Key，触发时**全部执行**，但 `RemoveEvent` 只移除当前实例的方法，不影响其他怪物

---

## 三、事件触发者：PlayerOfTriggerEvent

玩家脚本，按空格键触发 `OnPlayerAtk` 事件。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOfTriggerEvent : MonoBehaviour
{
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("开始触发事件中心PlayerAtk方法");
            //这里触发的是整个字典的Key,里面可能对应多个事件
            EventCenterMgr.Instance.EventTrigger("OnPlayerAtk");
            //使用这个事件中心后:
            //之前你必须引入扣血的怪物,现在只要触发Event,场景中订阅了Event的小怪自动扣血(Player不需要知道是谁)
            //MonsterEvent 只关心"自己怎么掉血、怎么死",PlayerOfTriggerEvent 只关心"什么时候触发攻击信号"
        }
    }
}
```

### 解耦效果

| 传统做法 | 事件中心做法 |
|---------|-------------|
| Player 获取所有 Monster 引用，逐个调用 `TakeDamage()` | Player 只调用 `EventTrigger("OnPlayerAtk")` |
| 新增怪物类型要改 Player 代码 | 新怪物只需自己订阅事件，Player 不用改 |
| Player 依赖 Monster 类 | Player 和 Monster 互不引用，只依赖 EventCenterMgr |

---

## 四、执行流程

```
1. 场景加载
   MonsterEvent.Awake()  →  AddEvent("OnPlayerAtk", PlayerAtk)
   （多个怪物各自订阅同一 Key，事件中心字典中该 Key 对应多个函数）

2. 玩家按空格
   PlayerOfTriggerEvent.Update()  →  EventTrigger("OnPlayerAtk")
   →  eventDict["OnPlayerAtk"]?.Invoke()
   →  所有订阅的 MonsterEvent.PlayerAtk() 全部执行
   →  每个怪物各自扣血 / 判断死亡 / 销毁

3. 怪物销毁
   MonsterEvent.OnDestroy()  →  RemoveEvent("OnPlayerAtk", PlayerAtk)
   （只移除当前实例的方法，其他怪物的订阅不受影响）
```

---

## 五、注意事项

1. **OnDestroy 必须取消订阅**：不取消会导致事件中心持有已销毁对象的委托，触发时报错或内存泄漏
2. **仅支持无参函数**：`UnityAction` 无参数，无法传递伤害值、怪物类型等信息。需要传参请用泛型版本
3. **Key 命名规范**：建议用 `On + 触发主体 + 动作` 的格式（如 `OnPlayerAtk`、`OnMonsterDead`），避免重名冲突
4. **多次订阅问题**：如果在 Awake 以外的地方也调用 `AddEvent`，同一方法可能被订阅多次，触发时执行多次
