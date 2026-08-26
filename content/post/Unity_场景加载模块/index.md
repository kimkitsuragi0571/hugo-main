+++
title = "Unity 场景加载模块"
date = "2026-08-26T09:47:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "SceneManager", "异步加载", "协程", "事件中心", "进度条"]
+++

Unity 原生 `SceneManager.LoadScene` 同步加载会卡住主线程，大场景加载时画面会冻结。异步加载 `LoadSceneAsync` 可以在后台加载，但需要协程配合 + 进度回调。场景加载模块 `ScenesMgr` 把同步/异步加载封装成统一接口，异步加载进度通过**泛型事件中心**广播给 UI，实现加载进度条。

---

## 一、场景管理器：ScenesMgr

继承 `MonoSingletonBase<ScenesMgr>`，提供同步加载和异步加载两种方式，异步加载通过协程 + 泛型事件中心广播进度。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScenesMgr : MonoSingletonBase<ScenesMgr>
{
    protected override void Awake()
    {
        base.Awake();
        print("哔哔哔...场景加载模块");
    }
    
    //1.普通的同步加载方法
    //传入加载的场景名,场景加载完毕时执行的函数
    public void LoadScene(string sceneName, UnityAction OnLoadComplete)
    {
        //场景同步加载
        SceneManager.LoadScene(sceneName);
        //加载完毕执行
        OnLoadComplete();
    }
    
    
    //2.异步加载方法
    public void LoadSceneAsync(string sceneName, UnityAction OnLoadComplete)
    {
        //如果这个ScenesMgr继承纯C#单例,也是可以用之前讲的纯C#实现协程(在Mono模块里面)
        //MonoModuleMgr.Instance.StartCor(IELoadSceneAsync(sceneName, OnLoadComplete));
        IEnumerator ie = IELoadSceneAsync(sceneName, OnLoadComplete);
        StartCoroutine(ie);
        
    }
    //定义异步加载所用的协程
    private IEnumerator IELoadSceneAsync(string sceneName, UnityAction OnLoadComplete)
    {
        //场景异步加载的句柄AsyncOperation
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        //还可以通过.progress属性获取场景加载进度
        while (!ao.isDone)
        {
            //事件中心模块(支持泛型的升级版)
            //这里就是触犯了Key = "场景加载"下的所有函数并且传入参数为ao.progress
            //不用在意传入的函数名或者说事件容器,那个是Add里面才写的
            GenericEventCenterMgr.Instance.EventTrigger("场景加载", ao.progress);
           yield return ao.progress;
           //yield return null;
        }

        yield return ao;
        OnLoadComplete();
    }

}
```

### 核心方法

| 方法 | 说明 |
|------|------|
| `LoadScene(sceneName, OnLoadComplete)` | 同步加载：`SceneManager.LoadScene` 阻塞主线程，完成后执行回调 |
| `LoadSceneAsync(sceneName, OnLoadComplete)` | 异步加载：启动协程 `IELoadSceneAsync`，后台加载不卡帧 |
| `IELoadSceneAsync` | 协程主体：`LoadSceneAsync` 获取 `AsyncOperation` 句柄，`while (!ao.isDone)` 循环中通过事件中心广播进度，加载完成后执行回调 |

### 异步加载流程

```
LoadSceneAsync("GameStore", callback)
    │
    ▼
IELoadSceneAsync 协程启动
    │
    ├── SceneManager.LoadSceneAsync → 返回 AsyncOperation ao
    │
    ├── while (!ao.isDone) 循环
    │   ├── GenericEventCenterMgr.Instance.EventTrigger("场景加载", ao.progress)
    │   │   └── 广播进度给所有订阅者（如 UI 进度条）
    │   └── yield return ao.progress  ← 等待下一帧继续
    │
    ├── 加载完成，跳出循环
    │
    └── yield return ao → OnLoadComplete() 回调
```

> 💡 如果 `ScenesMgr` 继承的是纯 C# 单例（`SingletonBase`），无法直接调用 `StartCoroutine`，可以用之前 Mono 模块的 `MonoModuleMgr.Instance.StartCor(IELoadSceneAsync(...))` 代替，脚本注释中已给出这种写法。

---

## 二、测试脚本：ScenesMgrTest

演示如何订阅进度事件 + 触发异步加载 + 更新 UI 进度条。

```csharp
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenesMgrTest : MonoBehaviour
{
    public Slider progressBar; 
    public TMP_Text progressText;  
    void Start()
    {
        //订阅事件, 当ScenesMgr触发场景加载(也就是我们使用EventTrigger的时候),触发UpdateProgress
        // GenericEventCenterMgr.Instance.EventTrigger("场景加载", ao.progress);
        GenericEventCenterMgr.Instance.AddEvent<float>("场景加载", UpdateProgress);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ScenesMgr.Instance.LoadSceneAsync("GameStore", () =>
            {
                print("场景加载完成");
            });
        }
    }

    void OnDestroy()
    {
        //销毁物体实例的时候,必须取消订阅
        GenericEventCenterMgr.Instance.RemoveEvent<float>("场景加载", UpdateProgress);
    }
    
    private void UpdateProgress(float progress)
    {
        // SceneManager.LoadSceneAsync 的进度范围是 0~0.9，这里映射到 0~1
        float displayProgress = Mathf.Clamp01(progress / 0.9f);
        
        progressBar.value = displayProgress;
        progressText.text = $"加载中... {(int)(displayProgress * 100)}%";
    }
}
```

### 关键点

| 操作 | 代码 | 说明 |
|------|------|------|
| **订阅进度** | `AddEvent<float>("场景加载", UpdateProgress)` | 泛型事件订阅，`float` 对应 `ao.progress` |
| **触发加载** | `LoadSceneAsync("GameStore", callback)` | 按空格键触发异步加载，完成后打印日志 |
| **更新 UI** | `progressBar.value` + `progressText.text` | Slider 进度 + TMP 文字百分比 |
| **取消订阅** | `RemoveEvent<float>("场景加载", UpdateProgress)` | OnDestroy 中必须移除，防止内存泄漏 |

### 进度映射说明

`SceneManager.LoadSceneAsync` 的 `ao.progress` 范围是 **0 ~ 0.9**（最后 10% 是场景激活阶段），直接显示会让进度条卡在 90%。所以需要映射：

```
displayProgress = Mathf.Clamp01(progress / 0.9f)
```

| ao.progress | displayProgress | 显示 |
|-------------|---------------|------|
| 0.0 | 0% | 加载中... 0% |
| 0.45 | 50% | 加载中... 50% |
| 0.9 | 100% | 加载中... 100% |

---

## 三、模块协作关系

场景加载模块复用了前面两个模块：

```
ScenesMgr (MonoSingletonBase)
    │
    ├── 异步加载协程
    │   └── GenericEventCenterMgr.Instance.EventTrigger<float>("场景加载", progress)
    │       │
    │       ▼
    │   GenericEventCenterMgr (泛型事件中心)
    │       │
    │       ▼
    │   ScenesMgrTest.UpdateProgress(float progress)
    │       │
    │       └── 更新 Slider + TMP_Text
    │
    └── 加载完成 → OnLoadComplete() 回调
```

- **泛型事件中心**：`EventTrigger<float>` 广播进度参数，UI 端用 `AddEvent<float>` 订阅，实现**加载逻辑与 UI 显示完全解耦**
- **MonoSingletonBase**：`ScenesMgr` 继承单例基类，自动创建 GameObject + DontDestroyOnLoad，切场景时不销毁

---

## 四、注意事项

1. **进度范围 0~0.9**：`LoadSceneAsync` 的 `ao.progress` 最大到 0.9，需要 `/ 0.9f` 映射到 0~1 才能显示 100%
2. **OnDestroy 取消订阅**：进度条脚本销毁时必须 `RemoveEvent`，否则场景切换后事件中心仍持有已销毁对象的委托
3. **同步加载会卡帧**：`LoadScene` 阻塞主线程，大场景加载时画面冻结。除小场景外，建议统一用 `LoadSceneAsync`
4. **allowSceneActivation**：如果需要手动控制场景激活时机（如进度到 100% 后按键进入），可设 `ao.allowSceneActivation = false`，但要注意此时 `progress` 最大只到 0.9 且 `isDone` 永远为 false，循环跳出条件需改写
