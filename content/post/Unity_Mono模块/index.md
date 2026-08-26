+++
title = "Unity Mono模块"
date = "2026-08-23T17:56:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "MonoBehaviour", "单例", "Update封装", "事件"]
+++

Unity 中继承 MonoBehaviour 的类才能挂载到 GameObject 上，才能使用 Unity 的生命周期函数（Update、Start 等）。但有些纯逻辑类不想继承 MonoBehaviour，却又想每帧执行逻辑。Mono 模块的核心思路就是：用一个继承 MonoBehaviour 的公共模块统一执行 Update，然后通过事件把每帧调用暴露给外部非 Mono 脚本。

## 一、三层继承结构

```
MonoSingletonBase<T>  ←  最底层：自动创建实例 + 防重复 + 不销毁
        ↑
MonoModuleBase<T>     ←  中间层：统一 Update，通过 UnityAction 事件暴露给外部
        ↑
具体模块               ←  最上层：注册每帧需要执行的函数
```

---

## 二、第一层：单例基类 MonoSingletonBase

最底层的单例基类，提供自动创建实例、DontDestroyOnLoad 防销毁、防重复实例等能力。所有需要单例的 Mono 模块都继承它。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingletonBase<T> : MonoBehaviour where T: MonoBehaviour
{
    private static T _instance;
    //依旧Instance属性
    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject mgr = new GameObject(typeof(T).Name);
                    _instance = mgr.AddComponent<T>();
                    DontDestroyOnLoad(mgr);
                }
            }
            return _instance;
        }
    }
    //依旧虚Awake方法用于重写(静态了不就没法继承吗)
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static void Clear()
    {
        _instance = null;
    }

    protected virtual void OnDestroy()
    {
        if (_instance == null)
        {
            _instance = null;
        }
    }
}
```

### 核心逻辑

| 方法 | 说明 |
|------|------|
| `Instance` getter | `FindObjectOfType<T>()` 先查找场景中是否已有实例；没有则创建新 GameObject 并挂载组件，`DontDestroyOnLoad` |
| `Awake()` | 双重保障：首次挂场景时赋值 + 不销毁；已有实例则自毁防止重复 |
| `Clear()` | 主动清空静态实例引用，下次访问会重新创建 |
| `OnDestroy()` | 对象被销毁时同步清空 `_instance` |

> ⚠️ 因为我们在子类脚本中补全了泛型 T，所以 `_instance` 这里指向的并非模块基类，而是具体的子类类型。

---

## 三、第二层：模块基类 MonoModuleBase

中间层，继承自 `MonoSingletonBase<T>`。它的唯一职责就是在自己的 `Update` 里统一触发 `UnityAction` 事件，把每帧逻辑分发出去。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoModuleBase<T> : MonoSingletonBase<T> where T : MonoBehaviour
{
   //首先声明基于UnityAction的事件
   private event UnityAction updateEvent;

   protected override void Awake()
   {
      base.Awake();
      Debug.Log("Mono模块基类启动(负责真正的统一Update)");
   }
//然后让所有的函数都通过UnityAction.Invoke统一执行
//而UnityAction.Invoke又是统一在模块基类的Update里面执行
   private void Update()
   {
      updateEvent?.Invoke();
   }
//后面写TrickMgr的时候还是要外部调用的,所以这里public
   public void AddUpdateEvent(UnityAction action)
   {
      updateEvent += action;
   }
   
   public void RemoveUpdateEvent(UnityAction action)
   {
      updateEvent -= action;
   }
   // 确保销毁时清理事件，防止内存泄漏
   protected override void OnDestroy()
   {
      updateEvent = null; 
      base.OnDestroy();
   }
}
```

### 核心逻辑

- `updateEvent`：私有 `UnityAction` 事件，存储所有需要每帧执行的函数
- `Update()`：唯一的 MonoBehaviour 生命周期函数，统一调用 `updateEvent?.Invoke()`
- `AddUpdateEvent / RemoveUpdateEvent`：对外暴露事件订阅/取消，外部（包括非继承 MonoBehaviour 的纯 C# 类）都可以注册自己的方法到每帧调用链中
- `OnDestroy()`：先把事件赋值为 null 清空订阅，再调用父类清理单例，防止内存泄漏

> 💡 核心思想：**非 Mono 脚本想每帧执行，不用自己挂 GameObject，直接把方法传入事件，模块基类通过统一的 Update 调用事件即可。**

---

## 四、第三层：具体模块 MonoModuleMgr

最上层的具体业务模块，继承自 `MonoModuleBase<MonoModuleMgr>`。在 Awake 中注册需要每帧执行的方法。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoModuleMgr : MonoModuleBase<MonoModuleMgr>
{
   protected override void Awake()
   {
      base.Awake();
      Debug.Log("Mono模块启动");
      Debug.Log("将OnRun方法注册");
      AddUpdateEvent(OnRun);
   }

   private void OnRun()
   {
      Debug.Log("每帧调用Run,这里用OnRun封装一下");
      Run();
   }

   private void Run()
   {
      Debug.Log("每帧统一调用Run");
   }


}
```

### 使用方式

```csharp
// 启动：首次访问 Instance 时自动创建 GameObject 并挂载组件
// Awake 会自动调用 base.Awake → 注册 OnRun 到 updateEvent
var mgr = MonoModuleMgr.Instance;

// 每帧自动执行：
// MonoModuleBase.Update() → updateEvent.Invoke() → OnRun() → Run()
```

---

## 五、执行流程总结

| 阶段 | 执行顺序 | 说明 |
|------|---------|------|
| **出生 Awake** | `MonoSingletonBase.Awake()` → 赋值 `_instance` + 不销毁 → 向上返回 → `MonoModuleBase.Awake()` → 向上返回 → `MonoModuleMgr.Awake()` → `AddUpdateEvent(OnRun)` 注册方法 | 先逐层向上调用 `base.Awake()`，再从顶层向下注册每帧逻辑 |
| **运行 Update** | `MonoModuleBase.Update()` → `updateEvent?.Invoke()` → 触发所有注册的方法（如 `MonoModuleMgr.OnRun()` → `Run()`） | 所有非 Mono 脚本都由这一个统一的 Update 分发 |
| **死亡 OnDestroy** | 外部调用 `Destroy(gameObject)` 或场景切换 → 子类 `OnDestroy`（如有）→ `MonoModuleBase.OnDestroy()` 清空事件 → `MonoSingletonBase.OnDestroy()` 清空 `_instance` | 逐层清理，下次访问 Instance 会重新创建 |

### 设计优势

1. **非 Mono 脚本可用**：纯 C# 逻辑类不继承 MonoBehaviour 也能每帧执行，只需 `AddUpdateEvent(MyMethod)`
2. **减少 Update 开销**：Unity 调用 1 个 Update + 事件分发，比每个脚本各自挂 MonoBehaviour 且各有 Update 性能更好
3. **生命周期统一管理**：DontDestroyOnLoad、单例、销毁清理都在基类封装完成，子类只需关注业务逻辑
4. **事件订阅式开发**：模块间解耦，无需持有模块引用即可添加/移除每帧逻辑
