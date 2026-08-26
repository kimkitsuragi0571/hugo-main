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
