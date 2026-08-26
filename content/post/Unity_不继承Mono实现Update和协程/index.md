+++
title = "Unity 不继承Mono实现Update和协程"
date = "2026-08-23T18:44:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "MonoBehaviour", "单例", "Update", "协程", "Coroutine"]
+++

前面 Mono 模块中，`MonoModuleBase` 统一了 Update，但是具体模块仍然要继承 MonoBehaviour 才能使用它。更进一步：**纯 C# 脚本，不继承 MonoBehaviour，一样可以实现每帧执行和协程**——只需把要执行的方法和要跑的协程「委托」给 Mono 模块的统一入口即可。

这样业务逻辑类完全不用关心生命周期挂载，和 Unity 解耦，更适合单元测试。

## 一、四层结构

```
 SingletonBase<T>        ←  纯 C# 单例基类（不继承 Mono，用 new T()）
           ↓
  纯 C# 业务类（TrickMgr） ←  不继承 Mono，调用 MonoModuleMgr 实现 Update + 协程
           ↑
 MonoModuleMgr           ←  继承 MonoModuleBase<MonoModuleMgr>，
                             给非 Mono 脚本提供统一入口（AddUpdateEvent/StartCor/StopCor）
           ↑
 MonoModuleBase<T>       ←  统一 Update + 协程开启
           ↑
 MonoSingletonBase<T>    ←  自动创建单例 + 不销毁
```

> 关键思想：**「我自己不用挂 GameObject，找个已经挂了的人替我跑。」**

---

## 二、第一层：纯 C# 单例基类 SingletonBase

和 `MonoSingletonBase` 不同，这个单例**不继承 MonoBehaviour**，约束是 `T : class, new()`，直接 `new T()` 创建实例，完全脱离 Unity 场景。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//泛型约束:T必须是类,且有无参构造函数
public class SingletonBase<T> where T : class, new()
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            //static属性,没有this实例,所以直接new T()
            //对比继承Mono的是_instance = this as T;
            if(_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }
    protected SingletonBase()
    {
        //依旧私有化构造函数
    }
}
```

### 与 MonoSingletonBase 对比

| 特性 | SingletonBase | MonoSingletonBase |
|------|---------------|-------------------|
| 继承链 | 纯 C#，无继承 | `MonoBehaviour` |
| 实例化方式 | `new T()` | 场景查找 / `AddComponent<T>()` |
| 泛型约束 | `class, new()` | `MonoBehaviour` |
| DontDestroyOnLoad | 不需要（纯托管对象） | 需要（场景对象） |
| 构造函数 | `protected` | 不能显式定义构造函数 |
| 适合场景 | 纯业务逻辑类 | 需要挂场景 / 需要生命周期的类 |

---

## 三、Mono 两层：MonoSingletonBase + MonoModuleBase（增强版）

与前一篇 Mono 模块相比，`MonoModuleBase` 增加了 `StartCor` 和 `StopCor` 两个方法，把协程也「外包」给非 Mono 脚本用。

### 3.1 MonoSingletonBase（与上一篇相同）

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

### 3.2 MonoModuleBase（新增协程方法）

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
   
   // 用于开启/关闭协程的方法
   //TrickMgr没有继承Mono,没法调用协程(但是仍然可以声明协程),只能传入由基类执行
   //通过ie = Cor(),startCoroutine(ie)  所以这里传入协程的句柄
   public Coroutine StartCor(IEnumerator ie)
   {
      //返回协程执行语句的句柄cor = StartCoroutine(ie)
      return StartCoroutine(ie);
   }

   //这里传入协程执行语句的句柄StopCoroutine(cor)
   public void StopCor(Coroutine cor)
   {
      if (cor != null) StopCoroutine(cor);
   }
}
```

**新增的两个方法：**

| 方法 | 作用 | 非 Mono 脚本为什么需要它 |
|------|------|------------------------|
| `StartCor(IEnumerator ie)` | 调用基类的 `StartCoroutine(ie)`，返回协程句柄 | 非 Mono 类不能直接调用 `StartCoroutine`（它是 MonoBehaviour 的成员方法），但可以写 `IEnumerator` 方法声明协程 |
| `StopCor(Coroutine cor)` | 调用 `StopCoroutine(cor)` 停止指定协程 | 同上，非 Mono 类拿不到执行上下文，只能通过 Mono 类代劳 |

> 💡 非 Mono 类**仍然可以声明 `IEnumerator` 协程函数**（这只是 C# 的迭代器方法），只是不能自己 `StartCoroutine` 而已。

---

## 四、第四层：不继承 Mono 的业务类 TrickMgr

继承纯 C# 单例基类 `SingletonBase<TrickMgr>`，没有挂到任何 GameObject 上。通过调用 `MonoModuleMgr.Instance` 的封装方法实现每帧执行 + 协程。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//首先是一个简单的不继承Mono的统一Update实现
////注意这里继承的是纯C#单例基类哈
public class TrickMgr : SingletonBase<TrickMgr>
{
    public TrickMgr()
    {
        //这里为了继承Singleton基类,只能public构造函数了
        Debug.Log("哔哔哔...纯C#单例实现帧更新与协程");
    }
    //声明协程执行语句(而不是协程声明) 的句柄cor(不是ie)
    private Coroutine _cor;

    //开启帧更新: 方法订阅 + 协程订阅
    //不是继承单例而是调用其中的方法哈
    //将Speak方法加入事件订阅,调用协程(每次调用方法都会订阅,多次执行)
    public void StartUp()
    {
        MonoModuleMgr.Instance.AddUpdateEvent(Speak);
        //传入声明的协程,返回协程执行句柄
        _cor = MonoModuleMgr.Instance.StartCor(Cor(1));
    }
    
    //停止帧更新:取消方法订阅+停止协程运行
    public void StopUp()
    {
        MonoModuleMgr.Instance.RemoveUpdateEvent(Speak);
        if (_cor != null)
        {
            //停止协程,然后移除句柄引用
            MonoModuleMgr.Instance.StopCor(_cor);
            _cor = null;
        }
    }

    private void Speak()
    {
        Debug.Log("传入帧更新的方法");
    }

    //这里协程订阅只是因为不继承Mono用不了协程而已hhh
    private IEnumerator Cor(int val)
    {
        while (val < 5)
        {
            yield return new WaitForSeconds(1f);
            val++;
            Debug.Log("传入Mono基类执行的协程");
        }
    }
}
```

### 使用示例

```csharp
// 启动：同时开启每帧调用和协程
TrickMgr.Instance.StartUp();
// 1. Speak 每帧执行
// 2. Cor(1) 协程开始运行，每秒打印一条，共4条

// 停止：取消订阅 + 结束协程
TrickMgr.Instance.StopUp();
```

### StartUp / StopUp 对应关系

| 操作 | StartUp | StopUp |
|------|---------|--------|
| **Update 部分** | `AddUpdateEvent(Speak)` → Speak 加入每帧事件 | `RemoveUpdateEvent(Speak)` → 取消订阅 |
| **协程部分** | `StartCor(Cor(1))` → 返回句柄保存到 `_cor` | `StopCor(_cor)` → 用句柄停止协程，`_cor = null` 清空引用 |

---

## 五、执行流向图

```
                    纯 C#，不继承 Mono
 TrickMgr (SingletonBase<TrickMgr>)
    │
    │  StartUp() 时调用
    │  ├── AddUpdateEvent(Speak)   ──→  注册到 updateEvent 事件
    │  └── StartCor(Cor(1))        ──→  返回 Coroutine 句柄 _cor
    ▼
 MonoModuleMgr (MonoModuleBase<MonoModuleMgr>)
    │
    │  Update() 每帧自动触发
    │  └── updateEvent?.Invoke()   ──→  所有注册的方法（含 TrickMgr.Speak）
    │
    │  协程也在同一个 Mono 上下文中执行
    ▼
 TrickMgr.Cor(1) —— 每秒输出一条，共执行4秒后自动结束
```

---

## 六、设计优势与注意事项

### 优势

1. **完全解耦 Unity 场景**：业务类 TrickMgr 不用挂 GameObject，不用关心创建/销毁/切场景生命周期
2. **单元测试友好**：纯 C# 类可以在 NUnit 里直接 `new TrickMgr()` 测试，无需启动 Unity Editor
3. **统一入口**：所有 Update 和所有协程都走 MonoModuleMgr 这一个 MonoBehaviour，性能和调试都更方便
4. **按需开启/停止**：`StartUp()` 和 `StopUp()` 显式控制生命周期，不像 MonoBehaviour 只有禁用/销毁两种状态

### 注意事项

1. **协程句柄必须保存**：`StartCor` 返回的 `Coroutine` 要存到 `_cor`，后续停止时才能用句柄精准停止，否则只能粗暴 `StopAllCoroutines` 影响其他模块
2. **多次调用 StartUp 会重复订阅**：Speak 方法会被加多次，导致一帧执行多次 Speak。需要加 `if (_cor == null)` 等防重入判断
3. **销毁顺序**：MonoModuleMgr 被销毁时，TrickMgr 的 `_cor` 句柄会失效，重新调用 StartUp 时 MonoModuleMgr.Instance 会自动重建，但 `_cor` 还持有旧句柄需要清空
