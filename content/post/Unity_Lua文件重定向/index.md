+++
title = "Unity中Lua文件重定向管理器"
date = "2026-07-19T21:55:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "xLua", "Lua", "AssetBundle", "重定向"]
+++

在 Unity 中使用 xLua 时，`luaEnv.DoString("require 'main'")` 默认只会到 xLua 内置的几个路径查找 Lua 文件。如果 Lua 脚本被打进 AB 包，或者放在自定义目录下，就找不到。本文通过自定义 Loader 实现 **绝对路径 → AB 包路径 → 默认路径** 的三级自动重定向。

## 一、AB包管理器（修改版）ABPackageMgr

本版在前一篇 AB 包管理器基础上做了两处修改：

1. `MainABName` 增加了 `#elif PC` 分支，并将默认返回值改为 `StandaloneWindows`
2. 顶部新增 `using Object = UnityEngine.Object;`，消除与 `System.Object` 的歧义

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

## 二、文件重定向管理器 LuaReloadMgr

核心思路：通过 `luaEnv.AddLoader` 注册自定义 Loader，让 xLua 在执行 `require` 时依次走 **绝对路径 → AB包路径 → 默认路径** 三级查找。

```csharp
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class LuaReloadMgr : MonoSingletonBase<LuaReloadMgr>
{
    protected override void Awake()
    {
        base.Awake();
        print("哔哔哔...Lua重定向模块");
    }
    private LuaEnv luaEnv;

    //总之就是避免_instance不为null,luaEnv为null,结果懒汉单例不初始化
    //Init外部直接指明abName(这里不修改其他脚本,直接给个默认值reqlua吧)
    public void Init(string abName = "reqlua")
    {
        //只有luaEnv为null时初始化
        if (luaEnv != null)
        {
            return;
        }
        else
        {
            luaEnv = new LuaEnv();
            //绝对路径找->2.AB包路径找->3.默认路径找
            luaEnv.AddLoader(CustomLoader);
            //luaEnv.AddLoader(CustomABLoader);没法指定加载的AB包
            //我们这里直接用lambda闭包来实现(外部的Init负责传入abName参数)
            luaEnv.AddLoader((ref string fileName) =>
            {
                TextAsset lua = ABPackageMgr.Instance.LoadRes<TextAsset>(
                    abName + ".ab", fileName + ".lua");
                if (lua != null) {
                    return lua.bytes;
                }
                else {
                    return null;
                }
            });
        }
    }
    
    private byte[] CustomLoader(ref string fileName)
    {
        //获取Asset下的绝对路径
        string path = Application.dataPath + "/_Scripts/_xLuaLoader/" + fileName + ".lua";
        if (File.Exists(path))
        {
            print("绝对路径重定向成功");
            return File.ReadAllBytes(path);
        }
        else
        {
            Debug.Log("绝对路径重定向失败");
        }
        
        return null;
    }
    
    //改为用闭包实现了,现在这个硬编码ab包地址的函数可以滚了
    // private byte[] CustomABLoader(ref string fileName)
    // {
    //     //传入abName和resName(我这里手贱加了个AB包后缀所以abName也要添加ab后缀)
    //     TextAsset lua = ABPackageMgr.Instance.LoadRes<TextAsset>("reqlua.ab",fileName+ ".lua");
    //     if (lua != null)
    //     {
    //         print("AB重定向成功");
    //         return lua.bytes;
    //     }
    //     else
    //     {
    //         Debug.Log("AB重定向失败");
    //     }
    //     
    //     return null;
    // }
    
    //新增Global属性用于获取_G表
    public LuaTable Global
    {
        get
        {
            if (luaEnv == null)
            {
                print("解析器未初始化!");
                return null;
            }
            return luaEnv.Global;
        }
    }


    //没用AB包管理器版本
    // private byte[] CustomABLoader(ref string fileName)
    // {
        //print("进入AB包重定向");
        //依旧三件套,要加载文本文件肯定是用textAsset
        //加载path不能是一个文件夹而是具体文件,就是说这里只能重定向到一个具体的AB包
        //string path = Application.streamingAssetsPath + "/reqlua.ab";
        //AssetBundle ab = AssetBundle.LoadFromFile(path);
        //TextAsset textAsset = ab.LoadAsset<TextAsset>(fileName + ".lua");
        
        //这里之前少了调用链逻辑,return null才能让调用默认路径
       //if (textAsset == null)
        //{
    //         print("AB重定向依旧失败");
    //         return null; 
    //     }
    //    print("AB重定向成功");
    //     return textAsset.bytes;
    // }
    
    //剩下的一些常见成员方法
    public void DoString(string str)
    {
        if (luaEnv == null)
        {
            print("解析器未初始化!");
            return;
        }
        luaEnv.DoString(str);
    }

    public void Tick()
    {
        if (luaEnv == null)
        {
            print("解析器未初始化!");
            return;
        }
        luaEnv.Tick();
    }

    public void Dispose()
    {
        if (luaEnv == null)
        {
            print("解析器未初始化!");
            return;
        }
        luaEnv.Dispose();
        luaEnv = null;
    }
}
```

---

## 三、三级重定向流程

xLua 的 `AddLoader` 支持注册多个自定义 Loader，按注册顺序依次尝试。每个 Loader 返回 `null` 表示"我找不到，交给下一个"，返回字节数组则表示找到并加载成功。

### 1. 绝对路径重定向 CustomLoader

```
fileName → Application.dataPath/_Scripts/_xLuaLoader/fileName.lua
```

- 通过 `File.Exists` 判断文件是否存在
- 存在则 `File.ReadAllBytes` 读取返回
- 不存在返回 `null`，交给下一个 Loader

### 2. AB包路径重定向（Lambda 闭包）

```
fileName → ABPackageMgr.LoadRes<TextAsset>(abName + ".ab", fileName + ".lua")
```

- 通过 `ABPackageMgr` 从 AB 包加载 `TextAsset`
- 成功则返回 `lua.bytes`
- 失败返回 `null`，交给下一个 Loader

### 3. 默认路径

如果前两个 Loader 都返回 `null`，xLua 会走内置默认路径查找。

### 关键问题：为什么用 Lambda 闭包？

`luaEnv.AddLoader` 的签名是 `LuaEnv.CustomLoader(ref string fileName)`，只能传入文件名，**无法指定从哪个 AB 包加载**。如果用普通方法，AB 包名会被硬编码：

```csharp
// 硬编码版本 - 不灵活
TextAsset lua = ABPackageMgr.Instance.LoadRes<TextAsset>("reqlua.ab", fileName + ".lua");
```

改用 **Lambda 闭包** 后，`abName` 参数由外部的 `Init(abName)` 捕获传入，调用方可以自由指定 AB 包名：

```csharp
// Init 外部传入 abName，闭包捕获该参数
luaEnv.AddLoader((ref string fileName) =>
{
    TextAsset lua = ABPackageMgr.Instance.LoadRes<TextAsset>(
        abName + ".ab", fileName + ".lua");
    return lua != null ? lua.bytes : null;
});
```

---

## 四、使用示例

```csharp
public class GameMain : MonoBehaviour
{
    void Start()
    {
        // 初始化 Lua 环境，指定 Lua 脚本所在的 AB 包名
        LuaReloadMgr.Instance.Init("reqlua");

        // 执行 Lua 脚本（会依次走：绝对路径 → AB包 → 默认路径）
        LuaReloadMgr.Instance.DoString("require 'main'");

        // 获取 Lua 全局表，与 C# 交互
        LuaTable global = LuaReloadMgr.Instance.Global;
    }

    void Update()
    {
        // 每帧调用，清理 Lua 的内存
        LuaReloadMgr.Instance.Tick();
    }

    void OnDestroy()
    {
        // 释放 Lua 环境
        LuaReloadMgr.Instance.Dispose();
    }
}
```

**注意事项：**
- `Init` 必须在使用前调用一次，内部通过 `luaEnv != null` 防止重复初始化
- `AddLoader` 注册顺序决定查找优先级，先注册的先尝试
- `return null` 是触发下一个 Loader 的关键，不要遗漏
- `Tick()` 建议每帧调用，用于触发 Lua 的 GC
- `Dispose()` 在场景切换或退出时调用，释放 LuaEnv 资源
