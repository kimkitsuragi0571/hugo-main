+++
title = "Unity 资源加载模块(使用旧版callback实现)"
date = "2026-08-26T10:17:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "Resources", "异步加载", "协程", "callback", "委托"]
+++

Unity 中 `Resources.Load` 同步加载资源会卡帧，`Resources.LoadAsync` 异步加载需要配合协程。资源加载模块 `ResLoadMgr` 封装了同步和异步两种加载方式，自动判断 GameObject 类型并实例化。

异步加载中有一个核心问题：**协程无法直接返回值给调用方**。旧版方案通过 callback（回调函数）解决，把结果作为参数传入回调执行。

---

## 一、完整脚本

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResLoadMgr : MonoSingletonBase<ResLoadMgr>
{
   protected override void Awake()
   {
      base.Awake();
      print("哔哔哔...资源加载模块");
   }
   //1.同步加载的方法
   public T LoadRes<T>(string resName) where T:Object
   {
      T res = Resources.Load<T>(resName);
      //判断如果是GameObject就直接实例化(不然封装这方法纯多余,不如直接加载)
      if (res is GameObject)
      {
         return GameObject.Instantiate(res);
      }
      return res;
   }
   //2.异步加载的方法
  
   //为什么要用callback函数?-->为了让调用函数可以返回值
   //调用函数在执行了这个Start语句就直接清空调用栈,协程本身独立于调用函数
   //调用函数本身是没法return协程的返回结果(所以这里直接写void了)
   public void LoadResAsync<T>(string resName, UnityAction<T> callback) where T : Object
   {
      //依旧是纯C#单例可以使用Mono模块来开启协程
      //MonoModuleMgr.Instance.StartCoroutine(IELoadResAsync<T>(resName));
      StartCoroutine(IELoadResAsync<T>(resName, callback));
   }

   private IEnumerator IELoadResAsync<T>(string resName, UnityAction<T> callback) where T : Object
   {
      ResourceRequest req = Resources.LoadAsync<T>(resName);
      yield return req;
      if (req.asset is GameObject)
      {
         //协程内部当然也是没法直接return值得
         //为了实现返回值,只能直接将其传入callback作为参数
         //就是相当于某种闭包了
         //当然现在可以直接用async/await实现了
        callback(GameObject.Instantiate(req.asset) as T);
      }
      else
      {
         callback(req.asset as T);
      }
   }
}
```

---

## 二、同步加载：LoadRes

```csharp
public T LoadRes<T>(string resName) where T : Object
```

| 特性 | 说明 |
|------|------|
| 约束 | `where T : Object`（Unity 的 Object，不是 C# 的 object） |
| 返回值 | `T` — 直接 return 资源 |
| GameObject 特殊处理 | 如果加载的是 GameObject，自动 `Instantiate` 实例化后返回 |
| 非 GameObject | 直接返回资源本身（如 Texture、AudioClip 等） |

同步加载可以直接 `return`，因为调用栈没有中断：

```
LoadRes() → Resources.Load() → return res → 调用方拿到结果
```

---

## 三、异步加载：LoadResAsync（重点）

### 3.1 核心问题：协程无法返回值

> **为什么要用 callback 函数？**
>
> **为了让调用函数可以返回值。**
>
> 调用函数在执行了 `StartCoroutine` 语句后就直接清空调用栈，协程本身独立于调用函数运行。调用函数本身是没法 `return` 协程的返回结果的（所以 `LoadResAsync` 直接写 `void` 了）。

换句话说：

```
LoadResAsync() {
    StartCoroutine(IELoadResAsync(...));  // ← 执行完这行，LoadResAsync 的调用栈就结束了
    // 这里拿不到协程的结果，协程还在后台慢慢跑
}
```

`LoadResAsync` 返回类型是 `void`，不是 `T`。因为 `StartCoroutine` 启动协程后立即返回，协程的后续 `yield return` 逻辑在后续帧异步执行，调用方无法等待它完成。

### 3.2 解决方案：callback 回调

> **协程内部当然也是没法直接 return 值的。**
>
> 为了实现返回值，只能直接将其传入 callback 作为参数，就相当于某种闭包了。
>
> （当然现在可以直接用 async/await 实现了）

协程方法 `IELoadResAsync` 的返回类型是 `IEnumerator`，不是 `T`。所以加载完成后，把结果作为参数传给 callback 执行：

```csharp
// 协程内部加载完成后：
callback(Instantiate(req.asset) as T);  // 把结果传入 callback，由 callback "代为返回"
```

### 3.3 完整异步流程

```
调用方: LoadResAsync("Player", (prefab) => { /* 拿到结果 */ })
    │
    ▼
LoadResAsync 执行:
    StartCoroutine(IELoadResAsync(resName, callback))
    │  ← LoadResAsync 调用栈结束，返回 void
    │
    ▼
IELoadResAsync 协程在后续帧执行:
    ├── Resources.LoadAsync<T> → 返回 ResourceRequest
    ├── yield return req       ← 等待异步加载完成
    ├── req.asset is GameObject?
    │   ├── 是 → callback(Instantiate(req.asset) as T)
    │   └── 否 → callback(req.asset as T)
    │           ← 把结果传给 callback 执行，实现"返回值"
    ▼
调用方的 callback Lambda 被执行，拿到加载结果
```

---

## 四、使用示例

```csharp
// 同步加载
GameObject player = ResLoadMgr.Instance.LoadRes<GameObject>("Prefabs/Player");

// 异步加载（callback 方式）
ResLoadMgr.Instance.LoadResAsync<GameObject>("Prefabs/Player", (prefab) => {
    // 在这里才能拿到加载结果
    Instantiate(prefab);  // LoadResAsync 已经实例化过一次了，这里看需求
});

// 异步加载非 GameObject 资源
ResLoadMgr.Instance.LoadResAsync<AudioClip>("Audio/BGM", (clip) => {
    AudioSource.PlayClipAtPoint(clip, Vector3.zero);
});
```

> ⚠️ 注意：`LoadResAsync` 内部已经对 GameObject 做了 `Instantiate`，callback 收到的已经是实例化后的对象。如果不需要实例化，用同步方法 `LoadRes` 并判断类型。

---

## 五、方法对比

| 特性 | LoadRes（同步） | LoadResAsync（异步） |
|------|----------------|---------------------|
| 返回值 | `T`（直接 return） | `void`（无法 return） |
| 结果传递方式 | return | callback 回调参数 |
| 主线程阻塞 | ✅ 卡帧 | ❌ 不卡帧 |
| 适用场景 | 小资源、初始化加载 | 大资源、运行时动态加载 |
| 协程 | 不需要 | 需要 `IELoadResAsync` |
| 现代替代方案 | — | `async/await`（见下一篇） |

---

## 六、callback 模式的局限与演进

callback 模式解决了协程无法返回值的问题，但也有局限：

1. **回调地狱**：多个异步加载串联时，回调嵌套层级深，代码可读性差
2. **异常处理**：callback 内的异常不容易被外层 try-catch 捕获
3. **取消机制**：没有内置的取消/超时机制

C# 5.0 之后可以用 `async/await` + `TaskCompletionSource` 替代 callback，让异步代码写起来像同步代码一样直观。下一篇将介绍用 `await` 实现的资源加载模块。
