using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class x3_ReLoadFunctionTest : MonoBehaviour
{
    //注意我们这里为了规范还是把委托单独写一个C#文件
    //提前声明所有委托类型对应实例
    private CustomCall  _stuRun;
    private CustomCall2 _stuSpeak;
    private CustomCall3 _stuGrade;
    private CustomCall4 _stuParams;
    //缓存req2_LuaRequireStu返回的表
    private LuaTable _req2Table;
    void Start()
    { 
        print("--------------------现在开始调用Lua文件中的函数(类型声明在单独脚本)---------------------------");
        LuaReloadMgr.Instance.Init();
        //DoString执行require语句获取返回表到G_Req2上
        LuaReloadMgr.Instance.DoString("G_Req2 = require('req2_luaRequireStu')");
        //取出req2的返回表(详解我放后面的图片了)
        _req2Table = LuaReloadMgr.Instance.Global.Get<LuaTable>("G_Req2");
        if (_req2Table == null)
        {
            Debug.LogError("_req2Table表是空的,加载失败");
            return;
        }
        //总之还是C#委托作为Lua函数的容器
        _stuRun    = _req2Table.Get<CustomCall>("stuRun");
        _stuSpeak  = _req2Table.Get<CustomCall2>("stuSpeak");
        _stuGrade  = _req2Table.Get<CustomCall3>("stuGrade");
        _stuParams = _req2Table.Get<CustomCall4>("stuParams");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            print("开始调用无参无返回值函数");
            _stuRun?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            print("开始调用有参有返回值函数");
            string ret = _stuSpeak?.Invoke(100);
            print("返回值为:"+ret);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            print("开始调用多返回值函数");
            //注意后面两个不是变长参数哈,是返回值
            int gradeCode = _stuGrade.Invoke(996, out int score, out string level);
           print("返回值依次是"+gradeCode);
           print(score);
           print(level);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            print("开始调用变长函数");
            _stuParams?.Invoke("hello", 1, 2.5f, "world", true);
        }
    }
    //这个好像在xLua工具中自带,不释放会导致没法卸载热更新旧代码+内存泄露
    void OnDestroy()
    {
        _stuRun    = null;
        _stuSpeak  = null;
        _stuGrade  = null;
        _stuParams = null;

        if (_req2Table != null)
        {
            _req2Table.Dispose();
            _req2Table = null;
        }

        print("🧹 Lua 资源已释放");
    }
    
}
