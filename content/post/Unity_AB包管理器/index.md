+++
title = "Unity AB包管理器"
date = "2026-07-19T21:10:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "AssetBundle", "资源加载", "单例模式"]
+++

AssetBundle（AB包）是 Unity 用于资源热更新和动态加载的核心机制。本文实现一个通用的 AB 包管理器，封装主包加载、依赖加载、同步/异步资源加载和包卸载等常用逻辑。

## 一、单例基类 MonoSingletonBase

继承 `MonoBehaviour` 的泛型单例基类，提供自动创建实例、不销毁、防重复等能力。

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

---

## 二、AB包管理器 ABPackageMgr

继承 `MonoSingletonBase`，通过字典缓存已加载的 AB 包避免重复加载，并自动处理依赖关系。

```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public class ABPackageMgr : MonoSingletonBase<ABPackageMgr>
{
    //依旧重写父类方法,后面写自身逻辑
    protected override void Awake()
    {
        base.Awake();
        Debug.Log("ABPackageMgr--->加载完毕");
    }

    //0.提前准备
    //0.1声明主包和.manifest配置文件变量
    private AssetBundle mainAB = null;
    private AssetBundleManifest mainABManifest = null;
    //0.2使用Dict存储AB包避免重复加载(唯一Key嘛)
    private Dictionary<string,AssetBundle> _abDict = new Dictionary<string, AssetBundle>();
    //0.3使用属性便于获取文件路径
    private string PathUrl => Application.streamingAssetsPath + "/";
    //0.4使用属性根据平台获取主包名
    private string MainABName
    {
        get
        {
#if UNITY_IOS
         return "IOS";
#elif UNITY_ANDROID
         return "Android"; 
#elif PC
            return "PC";
#else
            return "StandaloneWindows";
#endif
        }
    }
    
    //0.5提前封装加载主包+传入ab包+ab相关依赖包的方法(注意一定是先加载依赖包哈)
    public void LoadAB(string abName)
    {
        AssetBundle docker;
        //使用原生AssetBundle时还是需要手动加载主包的
        if (mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFile(PathUrl + MainABName);
            mainABManifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        //获取项目中的依赖关系,然后遍历加载依赖包
        string[] rels = mainABManifest.GetAllDependencies(abName);
        //数组中存储的是abName所依赖包的[名字]
        foreach (string rel in rels)
        {
            if (!_abDict.ContainsKey(rel))
            {
                //这里abRel只是充当一个公交车容器的作用,每次装要放入字典的包,外面声明减少GC
                docker = AssetBundle.LoadFromFile(PathUrl + rel);
                _abDict.Add(rel, docker);
            }
        }
        //最后还有个主包没有加载
        if (!_abDict.ContainsKey(abName))
        {
            docker = AssetBundle.LoadFromFile(PathUrl + abName);
            _abDict.Add(abName, docker);
        }
    }
    
    //1.同步加载不指定类型
    public object LoadRes(string abName, string resName)
    {
        LoadAB(abName);
        return _abDict[abName].LoadAsset(resName);
    }
    //2.同步加载 指定类型
    public object LoadResType(string abName, string resName, System.Type type)
    {
        LoadAB(abName);
        return _abDict[abName].LoadAsset(resName,type);
    }
    //3.同步加载 直接泛型
    public T LoadRes<T>(string abName, string resName)  where T : Object
    {
        LoadAB(abName);
        return _abDict[abName].LoadAsset<T>(resName);
    }
    
    //这里的异步加载只有AB包加载资源时异步,AB包本身加载是在前面LoadAB中统一同步实现
    //4.异步加载不指定类型
    public void LoadResAsync(string abName, string resName,UnityAction<Object> callback)
    {
        //委托用于传入callback参数,最终我们获取到的.asset会传入这个函数执行
        //为什么不直接在外部手动传入?因为异步加载调用时机你没法把握(这也是回调的意义)
        StartCoroutine(LoadResCor(abName, resName, callback));
    }
    private IEnumerator LoadResCor(string abName, string resName, UnityAction<Object> callback)
    {
        LoadAB(abName);
        //这里就直接到 AssetBundleRequest这一步了,并没有 AssetBundleCreateRequest
        AssetBundleRequest abr = _abDict[abName].LoadAssetAsync(resName);
     
        yield return abr;
        //这里GameObject只是预制体模版,并不是场景中真正物体
        //总之不能传入预制体模版就对了
        if (abr.asset is GameObject)
        {
            callback(Instantiate(abr.asset));
        }
        else
        {
            callback(abr.asset);
        }
    }
    
    //5.异步加载结合Type指定类型
    public void LoadResAsyncType(string abName, string resName,System.Type type,UnityAction<Object> callback)
    {
        StartCoroutine(LoadResCorType(abName, resName, type, callback));
    }
    private IEnumerator LoadResCorType(string abName, string resName, System.Type type, UnityAction<Object> callback)
    {
        LoadAB(abName);
        AssetBundleRequest abr = _abDict[abName].LoadAssetAsync(resName, type);
        yield return abr;
        if (abr.asset is GameObject)
        {
            callback(Instantiate(abr.asset));
        }
        else
        {
            callback(abr.asset);
        }
    }
    
    //6.异步加载结合泛型
    //这里也是LoadAssetAsync本身就有Object限制
    public void LoadResAsync<T>(string abName, string resName,UnityAction<T> callback) where T: Object
    {
        StartCoroutine(LoadResCor<T>(abName, resName, callback));
    }

    private IEnumerator LoadResCor<T>(string abName, string resName, UnityAction<T> callback) where T: Object
    {
        LoadAB(abName);
        AssetBundleRequest abr = _abDict[abName].LoadAssetAsync<T>(resName);
        yield return abr;

        if (abr.asset is GameObject)
        {
            callback(Instantiate(abr.asset) as T);
        }
        else
        {
            callback(abr.asset as T);
        }
    }

    //7.单个包卸载
    public void UnLoad(string abName)
    {
        if (_abDict.ContainsKey(abName))
        {
            //先释放内存中包数据
            _abDict[abName].Unload(false);
            //清除字典Key
            _abDict.Remove(abName);
        }
    }
   
    //8.所有包的卸载
    public void UnLoadAll()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        _abDict.Clear();
        mainAB = null;
        mainABManifest = null;
    }

}
```

---

## 三、核心功能说明

### 1. 准备工作

| 成员 | 作用 |
|------|------|
| `mainAB` | 主包，存储平台对应的总 AB 包 |
| `mainABManifest` | 主包清单，用于查询依赖关系 |
| `_abDict` | 已加载 AB 包字典，避免重复加载 |
| `PathUrl` | 资源路径，指向 `StreamingAssets` |
| `MainABName` | 根据平台返回主包名（IOS/Android/PC） |

### 2. 加载 AB 包 LoadAB

加载顺序至关重要：**先加载主包 → 获取依赖列表 → 遍历加载依赖包 → 最后加载目标包**。

主包只加载一次，依赖包和目标包通过字典去重，已加载的不会重复加载。

### 3. 同步加载（3 种重载）

| 方法 | 签名 | 说明 |
|------|------|------|
| `LoadRes` | `(abName, resName)` | 不指定类型，返回 `object` |
| `LoadResType` | `(abName, resName, type)` | 通过 `Type` 指定类型 |
| `LoadRes<T>` | `(abName, resName)` | 泛型版本，直接返回 `T` |

### 4. 异步加载（3 种重载）

| 方法 | 签名 | 说明 |
|------|------|------|
| `LoadResAsync` | `(abName, resName, callback)` | 不指定类型 |
| `LoadResAsyncType` | `(abName, resName, type, callback)` | 结合 `Type` 指定类型 |
| `LoadResAsync<T>` | `(abName, resName, callback)` | 结合泛型 |

**关键点：**
- 异步仅针对**资源加载**，AB 包本身仍在 `LoadAB` 中同步加载
- 使用协程 + `UnityAction` 回调返回结果
- 若资源是 `GameObject`，会自动 `Instantiate` 实例化后返回（预制体模版不能直接用）

### 5. 卸载

| 方法 | 说明 |
|------|------|
| `UnLoad(abName)` | 卸载单个包，`Unload(false)` 保留已加载资源 |
| `UnLoadAll()` | 卸载所有包，清空字典并重置主包引用 |

---

## 四、使用示例

```csharp
// 同步加载
GameObject prefab = ABPackageMgr.Instance.LoadRes<GameObject>("model", "Player");
Instantiate(prefab);

// 异步加载
ABPackageMgr.Instance.LoadResAsync<GameObject>("model", "Player", (obj) =>
{
    Instantiate(obj);
});

// 卸载单个包
ABPackageMgr.Instance.UnLoad("model");

// 卸载所有包（场景切换时）
ABPackageMgr.Instance.UnLoadAll();
```

**注意事项：**
- `Unload(false)` 只释放 AB 包本身，已实例化的物体不受影响
- `Unload(true)` 会连同样板资源一起释放，场景中引用该资源的物体可能丢失材质
- `GameObject` 类型资源在异步加载时会自动实例化，其他类型直接返回
