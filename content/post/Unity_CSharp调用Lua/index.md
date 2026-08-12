+++
title = "Unity中C#调用Lua"
date = "2026-08-11T22:00:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "xLua", "Lua", "C#调用Lua", "委托", "接口"]
+++

本文基于 **Lua 文件重定向管理器**，演示 C# 调用 Lua 中各种成员的方法：函数、类、List、Dictionary、普通变量等。委托类型声明独立放在专门脚本中，和函数调用脚本分离。

## 一、委托类型声明 xDel_ReLoadDel

委托类型声明单独放一个脚本，直接写在**命名空间裸区域**而非类内部（因为本身就是类型声明）。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;


//委托类型声明单独放一个脚本
//直接不需要写在类内部了,裸区域声明(因为本来就是类型声明嘛)
    
//只有存储无参无返回值的委托不需要使用特性(Unity自动识别)
public delegate void CustomCall();
    
//有参有返回值  使用特性就需要点击生成代码
[CSharpCallLua]
public delegate string CustomCall2(int val);
//多返回值   out参数对应Lua多个返回值   
[CSharpCallLua]
public delegate int CustomCall3(int val, out int ret0, out string ret1);
//变长参数  params object[]对应变长参数   本身也可以有固定参数
[CSharpCallLua]
public delegate void CustomCall4(string msg, params object[] args);


public class xDel_ReLoadDel: MonoBehaviour
{
    void Awake()
    {
        print("----------------启动委托类型声明的脚本(类型声明都在裸区域而非类内)--------------");
    }

}
```

**四种委托类型说明：**

| 委托 | 签名 | 特性 | 说明 |
|------|------|------|------|
| `CustomCall` | `()` → `void` | 无 | 无参无返回值，无需 `[CSharpCallLua]` |
| `CustomCall2` | `(int val)` → `string` | `[CSharpCallLua]` | 有参有返回值 |
| `CustomCall3` | `(int val, out int, out string)` → `int` | `[CSharpCallLua]` | 多返回值，`out` 参数对应 Lua 多个返回值 |
| `CustomCall4` | `(string msg, params object[] args)` → `void` | `[CSharpCallLua]` | 变长参数，`params object[]` 对应 Lua 变长参数 |

> ⚠️ 带 `[CSharpCallLua]` 特性的委托，需要在 xLua 菜单中点击「Generate Code」生成桥接代码后才能正常使用。

---

## 二、C# 调用 Lua 函数 x3_ReLoadFunctionTest

通过重定向管理器加载 Lua 脚本返回的 table，将其中的函数通过委托接收，按键调用测试。

```csharp
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
```

**核心步骤：**
1. `LuaReloadMgr.Instance.Init()` — 初始化 Lua 环境
2. `DoString("G_Req2 = require('req2_luaRequireStu')")` — 执行 require，把返回表挂到全局
3. `Global.Get<LuaTable>("G_Req2")` — 取到 Lua 返回的 table
4. `table.Get<CustomCallX>("函数名")` — 用对应委托接收 Lua 函数
5. 按键 `Invoke()` 调用 / `OnDestroy` 释放委托与 LuaTable

**按键对应：**

| 按键 | 函数 | 说明 |
|------|------|------|
| 1 | `_stuRun` | 无参无返回值 |
| 2 | `_stuSpeak(100)` | 有参有返回值 |
| 3 | `_stuGrade(996, out int, out string)` | 多返回值 |
| 4 | `_stuParams("hello", 1, 2.5f, ...)` | 变长参数 |

---

## 三、C# 调用 Lua 数据结构 x4_ReLoadDSTest

包含 Lua 类映射到 C# 类、映射到 C# 接口、List / Dictionary / 普通变量等多种方式。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XLua;

public class x4_ReLoadDSTest : MonoBehaviour
{
   private  StuClass stu;
   private ICallLua iCallLua;
    void Start()
    {
        LuaReloadMgr.Instance.Init("reqluads");
        LuaReloadMgr.Instance.DoString("require ('req3_luaDS')");
        //C#调用Lua中的类(StuClass是一个用于接收的自定义类)
        stu = LuaReloadMgr.Instance.Global.Get<StuClass>("testStuClass");
        //内部类table用C#接口接收
         iCallLua = LuaReloadMgr.Instance.Global.Get<ICallLua>("testStuClass");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            print("开始测试调用Lua类(对象在Start中赋值)");
            print(stu.stuName);
            print(stu.stuIsDead);
            //调用Lua中成员函数--LuaFunction的调用只是方便,还是建议用前面的委托
            //需要传入实例而非StuClass类型(Lua中函数定义传入self)
            stu.stuWork.Call(stu);
            //调用内部类(LuaTable装填)
            print( stu.testTopStu.Get<string>("stuName"));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            print("开始测试调用Lua中Dict和List");
            List<int> list =  LuaReloadMgr.Instance.Global.Get<List<int>>("testList");
            //多类型的List用Object装填
            //记得小写别识别为Unity中Object了
            List<object> list1 =  LuaReloadMgr.Instance.Global.Get<List<object>>("testListType");
            
            Dictionary<string, int> dict = LuaReloadMgr.Instance.Global.Get<Dictionary<string, int>>("testDict");
            //对于多类型Key的Dict(即使这里Value都是int类型),仍然建立kv均用Object
            //这里会自动识别为Unity的Objdect导致报错,使用object小写,即System.Object的别名
            Dictionary<object, object> dict1 = LuaReloadMgr.Instance.Global.Get<Dictionary<object, object>>("testDictType");
            print(list[0]);
            print(list1[0]);
            print(dict["1"]);
            print(dict1[true]);
        }
        
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            print("开始测试调用Lua中普通变量");
            int testInt = LuaReloadMgr.Instance.Global.Get<int>("testInt");
            bool testBool = LuaReloadMgr.Instance.Global.Get<bool>("testBool");
            string testStr = LuaReloadMgr.Instance.Global.Get<string>("testStr");

            print(testInt);  
            print(testBool);  
            print(testStr);   
        }
        
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            print("开始测试调用Lua中table做为C#接口");
            print(iCallLua.stuName);              // AKI
            print(iCallLua.stuIsDead);            // False
            iCallLua.stuWork();                   // 直接调用
            print(iCallLua.testTopStu.stuName);   // JP
        }
    }

}

public class StuClass
{
    //与Lua成员同名,必须是public
    //table可以用LuaTable类型
    public string stuName;
    public bool stuIsDead;
    public LuaFunction stuWork;
    public LuaTable testTopStu;
}

[CSharpCallLua]
public interface ICallLua
{
     string stuName {get;set; }
    bool stuIsDead {get;set;}
    UnityAction stuWork {get;set;}
    //接口类型嵌套接收内部table
    ITopStu testTopStu { get; set; }
}

//接口中不能直接嵌套内部类,需要单独定义接口
[CSharpCallLua]
public interface ITopStu
{
    string stuName { get; set; }
}
```

### 按键对应功能

| 按键 | 功能 | 说明 |
|------|------|------|
| 1 | C# 类接收 Lua table | `StuClass` 接收，`LuaFunction.Call()` 调用成员函数 |
| 2 | List / Dictionary | 支持单类型 `List<int>`，多类型必须用 `List<object>` 和 `Dictionary<object, object>` |
| 3 | 普通变量 | `Get<int/bool/string>` 直接取全局变量 |
| 4 | C# 接口接收 Lua table | `ICallLua` 接口，属性自动映射，嵌套 table 用 `ITopStu` 接口 |

### 两种映射方式对比

| 方式 | C# 类映射 | C# 接口映射 |
|------|----------|------------|
| 成员函数接收 | `LuaFunction`（需手动传 self） | `UnityAction`（自动绑定 self，直接调用） |
| 嵌套 table 接收 | `LuaTable`（需手动 `.Get<T>("key")`） | 定义 `ITopStu` 接口（属性直接访问） |
| 字段可见性 | 必须 public | 必须属性 { get; set; } |
| 是否需要 `[CSharpCallLua]` | 否 | 是 |
| 推荐场景 | 快速访问，灵活 | 正式项目，强类型 |

---

## 四、注意事项

1. **`[CSharpCallLua]` 特性**：有参有返回值委托、接口映射必须加此特性，并在 xLua 菜单中执行 **Generate Code** 生成桥接代码
2. **大小写问题**：多类型集合使用 `object`（小写），而不是 `Object`（大写会被识别为 `UnityEngine.Object`）
3. **LuaFunction 调用**：类接收方式下，`stuWork.Call(stu)` 要把实例作为 self 传入；接口方式自动处理，直接 `stuWork()` 调用
4. **接口嵌套限制**：接口中不能直接嵌套类，嵌套 table 必须定义新的 `ITopStu` 接口
5. **资源释放**：`OnDestroy` 中要将委托赋值为 `null`、`LuaTable.Dispose()`、`LuaReloadMgr.Instance.Dispose()`，否则热更时旧代码无法卸载并导致内存泄漏
6. **Init 调用**：`LuaReloadMgr.Instance.Init("reqlua")` 的参数是对应 Lua 脚本所在的 AB 包名
