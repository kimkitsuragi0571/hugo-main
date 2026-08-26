+++
title = "Unity 资源加载模块(使用await实现)"
date = "2026-08-26T10:20:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "Resources", "async", "await", "Task", "TaskCompletionSource", "拓展方法"]
+++

上一篇用 callback 解决了协程无法返回值的问题。C# 的 `async/await` 模式可以让异步代码像同步代码一样线性编写：**直接 return 返回值，不用在回调和协程里面绕，还能用标准 try/catch 捕获异常。**

但 Unity 2022 版的 `ResourceRequest`（继承自 `AsyncOperation`）没有实现 `GetAwaiter()` 方法，无法直接 `await req`。需要自己手写一个拓展方法，用 `TaskCompletionSource` 把旧式异步包装成 `Task`。

---

## 一、AwaitLoadMgr + 拓展方法

```csharp
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class AwaitLoadMgr : MonoSingletonBase<AwaitLoadMgr>
{
   protected override void Awake()
   {
      base.Awake();
      print("哔哔哔...资源加载模块(await实现)启动");
   }

   //之前的版本只能用callback传入参数作为协程的返回值
   //这里直接用await/async来实现
   //好处1.可以直接return返回值 2.逻辑线性不需要在协程和回调里面绕 3.可以用标准try/catch(老协程很难捕获异常)
   
   //加载方法里面写async,返回值改为Task类型
   public async Task<T> LoadResAsync<T>(string resName) where T : Object
   {
      ResourceRequest req = Resources.LoadAsync<T>(resName);
      //使用await等待req加载完毕
      //await req;老版本会报错,AsyncOperation并没有实现GetAwaiter()方法
      //哎呀2022版太老了还不内置AsTask方法,狗史,需要自己手写个拓展类了
      await req.AsTask(); 
      if (req.asset is GameObject)
      {
         //这里可以直接返回加载的对象
         //泛型约束Object,可以返回任何继承自 UnityEngine.Object 的类型(Sprite,AudioClip啥的都可以)
         return GameObject.Instantiate(req.asset) as T;
      }
      return req.asset as T;
   }
}

//给ResourceRequest对象(也就是req)实现的拓展方法
public static class AsyncExtensions
{
   public static Task<UnityEngine.Object> AsTask(this ResourceRequest request)
   {
      // 创建一个新的 TaskCompletionSource (TCS)
      // TCS 是连接"旧式异步"和"现代 Task"的桥梁
      var tcs = new TaskCompletionSource<UnityEngine.Object>();
      // 定义一个局部函数，用于处理加载完成的事件
      void OnLoaded(AsyncOperation op)
      {
         // 当加载完成时，设置 Task 的结果
         tcs.SetResult(request.asset);
         // 记得移除监听，防止内存泄漏
         request.completed -= OnLoaded;
      }
      // 注册完成事件
      request.completed += OnLoaded;
      // 返回这个 Task 对象
      return tcs.Task;
   }
}
```

---

## 二、核心：async/await 的三大优势

> 之前的版本只能用 callback 传入参数作为协程的返回值。这里直接用 await/async 来实现：
> 1. **可以直接 return 返回值** — 不需要 callback 代为传递
> 2. **逻辑线性** — 不需要在协程和回调里面绕
> 3. **可以用标准 try/catch** — 老协程很难捕获异常

### callback vs await 对比

| 特性 | callback（旧版） | await（新版） |
|------|-----------------|--------------|
| 返回值 | `void`，通过 callback 参数传 | `Task<T>`，直接 `return` |
| 代码结构 | 回调嵌套 | 线性，await 后继续写 |
| 异常处理 | callback 内的异常难捕获 | 标准 `try/catch` |
| 调用方写法 | `LoadResAsync(name, (res) => { ... })` | `var res = await LoadResAsync(name)` |

---

## 三、核心：AsTask 拓展方法

### 3.1 为什么需要手写拓展

```csharp
await req;  // ❌ 老版本会报错,AsyncOperation并没有实现GetAwaiter()方法
await req.AsTask();  // ✅ 需要自己手写拓展类
```

`await` 的前提是对象实现了 `GetAwaiter()` 方法。C# 的 `Task` 天然支持，但 Unity 的 `AsyncOperation` / `ResourceRequest` 在 2022 版没有内置支持。所以需要用 `TaskCompletionSource` 手动包装。

### 3.2 TaskCompletionSource 桥接原理

```
ResourceRequest（旧式异步）
    │
    ├── request.completed += OnLoaded   ← 注册完成事件
    │
    ▼
TaskCompletionSource<UnityEngine.Object>（TCS）
    │
    ├── tcs.Task  ← 返回一个 Task 给 await 等待
    │
    ▼
OnLoaded 触发时：
    ├── tcs.SetResult(request.asset)    ← 设置结果，Task 完成
    └── request.completed -= OnLoaded    ← 移除监听，防内存泄漏
```

> 💡 TCS 是连接"旧式异步（事件回调）"和"现代 Task"的桥梁。旧式异步用事件通知完成，TCS 把这个通知转换成 Task 的完成状态，让 await 可以等待。

---

## 四、测试脚本：AwaitLoadMgrTest

调用方也需要配合 `async/await`，一般在 `Start` 或按钮点击事件里调用。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//调用者也需要配合await,一般在Start 或按钮点击事件里调用
public class AwaitLoadMgrTest : MonoBehaviour
{
   //声明await加载器对象
   public AwaitLoadMgr loader;
   //Start 方法也可以加 async，但返回值必须是 void 或 Task
   private async void Start()
   {
      //调用异步方法，并用 await 接收结果
      //代码会停在这里直到加载完毕
      GameObject enemy = await loader.LoadResAsync<GameObject>("Prefabs/Enemy");
      Debug.Log("加载完成！名字是：" + enemy.name);
   }
}
```

> ⚠️ **AwaitLoadMgrTest 输入检测在 Awake 中订阅 Update 事件可能存在时序问题，暂时懒得改。**
> 实际上 `AwaitLoadMgrTest` 仅在 `Start` 中 `await` 加载资源，并不涉及 `Awake` 订阅 `Update` 事件。如果你后续扩展该脚本（比如在 `Awake` 里订阅 `MonoModuleMgr.Instance.AddUpdateEvent(...)` 做 InputCheck），需要保证 `MonoModuleMgr` 已先完成 `Awake`，否则会触发空引用或订阅时序问题。当前版本未触发该路径，暂不修改。

### 调用方要点

| 要点 | 说明 |
|------|------|
| `async void Start()` | Unity 生命周期方法加 `async`，返回值必须是 `void`（不能是 `Task`，Unity 不会 await 它） |
| `await loader.LoadResAsync(...)` | 代码停在这一行直到加载完成，然后继续往下执行 |
| 结果直接赋值 | `GameObject enemy = await ...` — 不需要 callback，直接拿到返回值 |

### 执行流程

```
Start() {
    // 遇到 await，Start 暂停在这里（不阻塞主线程，控制权交还 Unity）
    GameObject enemy = await loader.LoadResAsync("Prefabs/Enemy");
    // ↓ 加载完成后继续往下执行
    Debug.Log("加载完成！名字是：" + enemy.name);
}
```

---

## 五、与 callback 版本完整对比

| 对比维度 | ResLoadMgr（callback） | AwaitLoadMgr（await） |
|---------|----------------------|----------------------|
| 异步方法签名 | `void LoadResAsync<T>(string, UnityAction<T>)` | `async Task<T> LoadResAsync<T>(string)` |
| 返回值 | void，结果在 callback 参数 | `Task<T>`，直接 return |
| 等待机制 | 协程 `yield return req` | `await req.AsTask()` |
| 结果传递 | `callback(Instantiate(req.asset) as T)` | `return Instantiate(req.asset) as T` |
| 调用方写法 | `LoadResAsync(name, (res) => { ... })` | `var res = await LoadResAsync(name)` |
| 异常处理 | callback 内异常难捕获 | `try { await ... } catch { ... }` |
| 串联加载 | 回调嵌套（回调地狱） | `var a = await LoadA(); var b = await LoadB();` 线性 |
| 继承 | `MonoSingletonBase<T>`（单例） | `MonoSingletonBase<T>`（单例） |
| 额外依赖 | 无 | 需要 `AsTask` 拓展方法 + `TaskCompletionSource` |

---

## 六、注意事项

1. **`async void` vs `async Task`**：Unity 生命周期方法（Start/Update 等）必须用 `async void`，因为 Unity 不会 await Task。普通方法建议用 `async Task` 以便调用方 await
2. **异常未捕获会崩**：`async void` 方法中未捕获的异常会直接抛到 Unity 主循环，建议在方法内 try/catch
3. **AsTask 移除监听**：`OnLoaded` 内 `request.completed -= OnLoaded` 必须执行，否则每次加载都累积事件监听导致内存泄漏
4. **AwaitLoadMgr 已改为单例**：新版继承 `MonoSingletonBase<AwaitLoadMgr>`，可通过 `AwaitLoadMgr.Instance` 直接访问，也可通过 Inspector 拖拽引用
5. **Unity 版本差异**：Unity 2023+ 已内置 `AsyncOperation` 的 await 支持，可省略 AsTask 拓展方法
