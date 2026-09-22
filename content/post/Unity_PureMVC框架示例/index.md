+++
title = "Unity PureMVC 框架示例"
date = "2026-09-17T09:30:00+08:00"
draft = false
slug = "puremvc-framework-example"
categories = ["Unity"]
tags = ["笔记", "PureMVC", "MVC", "框架", "设计模式", "通知", "解耦"]
+++

PureMVC 是对经典 MVC 的进一步抽象：**Model / View / Controller 三大核心全部由 Facade 门面统一管理，三者之间通过 Notification 通知通信、互不直接引用**。本文基于 PureMVC 官方库（`PureMVC.Interfaces` / `PureMVC.Patterns.*`）实现一个完整的「主面板 → 角色面板 → 升级 → 关闭」流程，包含 11 个脚本。

---

## 一、PureMVC 与普通 MVC/MVP 的区别

| 维度 | 普通 MVC | MVP | **PureMVC** |
|------|----------|-----|-------------|
| 通信方式 | View 直接调 Model | View ↔ Presenter ↔ Model | 全部走 Notification，三者互不引用 |
| 中间层 | Controller | Presenter | **Facade** + 三大核心（Model/View/Controller） |
| View 与 Model 关系 | 强耦合 | 通过 Presenter | 完全解耦 |
| 多界面协作 | 各自直接通信 | 各 Presenter 互相引用 | 都通过 Notification，发通知方不知道谁收 |
| 单元测试 | 难，View 依赖 Model | 中等，Presenter 可测 | 易，三大核心都是纯 C# |

PureMVC 的核心思想：**「发通知的人不知道谁会处理，处理通知的人也不知道是谁发的。」** 这让面板之间、面板与数据之间彻底解耦，加新面板 / 加新数据源对老代码零侵入。

---

## 二、文件结构

```
PureMVC/
├── pure_Notification.cs        # 通知名字符串常量
├── pure_Facade.cs              # 门面：初始化 + 注册 Command
├── pure_MainEntry.cs           # MonoBehaviour 入口
├── Controller/
│   ├── pure_StartUpCommand.cs  # 启动：注册 Proxy
│   ├── pure_ShowPanelCommand.cs# 显示面板：注册 Mediator + 加载预制体
│   ├── pure_HidePanelCommand.cs# 隐藏面板：销毁 GameObject
│   └── pure_LevUpCommand.cs    # 升级：调 Proxy.LevUp 后通知更新
├── Model/
│   ├── pure_PlayerDataObj.cs   # 玩家数据结构
│   └── pure_PlayerProxy.cs     # 数据代理：PlayerPrefs 读写
└── View/
    ├── pure_MainView.cs            # 主面板 MonoBehaviour
    ├── pure_MainView_Mediator.cs   # 主面板中介者
    ├── pure_RoleView.cs            # 角色面板 MonoBehaviour
    └── pure_RoleView_Mediator.cs   # 角色面板中介者
```

### 各文件职责一句话总结

| 文件 | 角色 | 职责 |
|------|------|------|
| `pure_Notification` | 常量 | 声明 5 个通知名（START_UP / SHOW_PANEL / HIDE_PANEL / UPDATE_PLAYER_INFO / LEVEL_UP） |
| `pure_Facade` | 门面 | 单例；初始化 Controller；`RegisterCommand` 把通知名映射到 Command 工厂；对外暴露 `StartUp()` |
| `pure_MainEntry` | 入口 | 场景挂载的 MonoBehaviour，`Start` 调 `Facade.StartUp()`；Update 中按 Q/W 触发显隐 |
| `pure_StartUpCommand` | 启动命令 | 注册 `pure_PlayerProxy`（数据代理） |
| `pure_ShowPanelCommand` | 显示命令 | 按通知 Body 中的面板名 switch；注册对应 Mediator；`Resources.Load` 加载预制体；`SetView` 绑定 ViewComponent；触发首次 UPDATE_PLAYER_INFO |
| `pure_HidePanelCommand` | 隐藏命令 | 从 Body 取 Mediator 或面板名 → 销毁 GameObject → `ViewComponent = null` |
| `pure_LevUpCommand` | 升级命令 | 取 Proxy → `LevUp()` → 通知 UPDATE_PLAYER_INFO |
| `pure_PlayerDataObj` | 数据结构 | `playerName / playerLev / playerAtk` 三字段 |
| `pure_PlayerProxy` | 数据代理 | 构造时从 PlayerPrefs 读；`LevUp` 加 1 级 + 加攻击；`SaveData` 回写 PlayerPrefs |
| `pure_MainView` | 主面板 | 持有 4 个 Button + 3 个 Text；`UpdateInfo_View` 把数据刷到 Text |
| `pure_MainView_Mediator` | 主面板中介者 | 监听 UPDATE_PLAYER_INFO；`SetView` 绑定 btnRole → 发 SHOW_PANEL("RolePanel") |
| `pure_RoleView` | 角色面板 | btnClose + btnLevUp + txtAtk；`UpdateInfo_View` |
| `pure_RoleView_Mediator` | 角色中介者 | 监听 UPDATE/SHOW/HIDE；`SetView` 绑定 btnClose → 发 HIDE_PANEL(this)；btnLevUp → 发 LEVEL_UP |

---

## 三、通知常量 pure_Notification

PureMVC 里所有跨模块通信都靠「通知名」字符串。集中声明避免拼写错误。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//通知名类:主要用来声明各个通知的名字
public class pure_Notification
{
    //显隐面板通知
    public const string SHOW_PANEL = "show_panel";
    public const string HIDE_PANEL = "hide_panel";
    //代表玩家信息更新的通知
    public const string UPDATE_PLAYER_INFO = "updatePlayerInfo";
    //启动通知
    public const string START_UP = "startUp";
    //升级通知
    public const string LEVEL_UP = "levUp";
}
```

> 命名规范：常量全大写 + 下划线分隔；值用小驼峰，避免和类名冲突。

---

## 四、Model 层：数据结构 + 数据代理

### 4.1 pure_PlayerDataObj — 纯数据结构

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//存储玩家数据结构
public class pure_PlayerDataObj
{
   public string playerName;
   public int playerLev;
   public int playerAtk;
}
```

### 4.2 pure_PlayerProxy — 数据代理

`Proxy` 是 PureMVC 的 Model 核心。它的 `Data` 属性是 `object`，所以要用一个自定义数据结构 `pure_PlayerDataObj` 包一层再赋值给 `Data`。

```csharp
using System.Collections;
using System.Collections.Generic;
using PureMVC.Patterns.Proxy;
using UnityEngine;

//继承Proxy父类后,还必须实现对应的构造函数
public class pure_PlayerProxy : Proxy
{
    public new const string NAME = "pure_PlayerProxy";
    
    //构造函数:初始化代理类的时候,统一添加到Proxy成员便于管理(相当于字典嘛,名字和数据)
    
    //参数分别为:代理名称,代理相关数据(这里是指定参数传入父类构造),这里用简便一点的写法(data默认null,本来就可以不用写)
    //public pure_PlayerProxy(string proxyName, object data = null) : base(proxyName, data)
    
    //这个base构造:只有一个pure_PlayerProxy实例,调用base只是初始化父类的字段,然后初始化子类独有字段
    public pure_PlayerProxy() : base(pure_PlayerProxy.NAME)
    {
        pure_PlayerDataObj playerData = new pure_PlayerDataObj();
        //初始化(后面的只是默认值)
        playerData.playerName = PlayerPrefs.GetString("PlayerName","JOJO");
        playerData.playerLev = PlayerPrefs.GetInt("PlayerLevel", 2);
        playerData.playerAtk = PlayerPrefs.GetInt("PlayerAtk", 99);
        
        //Data是Proxy内部的一个属性(是单独的Object,肯定没法直接存储多个字段,所以需要playerData对象)
        //就是把playerData实例中的字段赋值给自身pure_PlayerProxy对象的Data字段
        Data = playerData;
    }
    
    public void LevUp()
    {
        //需要先向下转型
        pure_PlayerDataObj data = Data as pure_PlayerDataObj;
        data.playerLev += 1;
        data.playerAtk += data.playerLev;
        SaveData();
    }

    private void SaveData()
    {
        pure_PlayerDataObj data = Data as pure_PlayerDataObj;
        PlayerPrefs.SetString("PlayerName", data.playerName);
        PlayerPrefs.SetInt("PlayerLevel", data.playerLev);
        PlayerPrefs.SetInt("PlayerAtk", data.playerAtk);
    }
}
```

**关键点**：
- `NAME` 是注册到 Facade 字典里的 key，必须 `new const` 隐藏父类的同名字段。
- `Data` 是 `object`，所以每次用都要 `as pure_PlayerDataObj` 向下转型。
- Proxy **完全不知道 View 的存在**——它只是改自己的数据 + 存盘，谁通知它升级它不管，升完级也不主动通知 View（由 Command 来发通知）。

---

## 五、View 层：View 脚本 + Mediator 中介者

PureMVC 的 View 分两层：**View 脚本**（MonoBehaviour，挂在预制体上，负责具体 UI 控件引用 + 显示数据）和 **Mediator**（纯 C#，注册到 Facade，监听通知 + 处理业务 + 转发到 View 脚本）。

### 5.1 pure_MainView — 主面板

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pure_MainView : MonoBehaviour
{
    public Button btnRole;
    public Button btnSkill;
  
    public Text txtName;
    public Text txtLevel;
    public Text txtAtk;

   //如果是根据MVC的思路来做,这里就可以传入更新方法
   //按照MVP的话只需要放到Presenter里面
    public void UpdateInfo_View(pure_PlayerDataObj playerData)
    {
        txtName.text = playerData.playerName;
        txtAtk.text = playerData.playerAtk.ToString();
        txtLevel.text = playerData.playerLev.ToString();
    }
}
```

### 5.2 pure_MainView_Mediator — 主面板中介者

```csharp
using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;

//负责管理UI具体面板与UI框架间的通信
public class pure_MainView_Mediator : Mediator
{
    //查询Facade字典避免重复的时候用得上if (!Facade.HasMediator(pure_MainView_Mediator.NAME)) 
    public static new string NAME = "pure_MainView";
    //虽然啥都没写,还是有初始化NAME字段的作用
    public pure_MainView_Mediator() : base(NAME)
    {
        //界面的创建应该由触发逻辑来实现,而且这么多界面也不能重复写
    }
    
    //重写监听通知的方法
    //双分发机制:当我们SendNotification(XXX)时,Facade同时转发两种路线
    //这里UPDATE_PLAYER_INFO没有 RegisterCommand,但是Mediator路线仍然是Facade转发的
    public override string[] ListNotificationInterests()
    {
        //PureMVC的规则:监听哪些通知,就将其用String[]返回(类似通过事件名注册监听)
        //当有人SendNotification(XXX)-->Facade 遍历所有已注册的 Mediator并且查看其 ListNotificationInterests()
        //如果 XXX 在这个数组里,调它的 HandleNotification
        return new string[]
        {
            pure_Notification.UPDATE_PLAYER_INFO,
            //好吧这里其实也没有必要监听
            // pure_Notification.SHOW_PANEL,
            // pure_Notification.HIDE_PANEL
        };
    }
    
    //重写处理通知的方法
    //要是只有Command路径,这里处理就需要每个面板在Excute中单独写转化并调用UpdateInfo_View
    public override void HandleNotification(INotification notification)
    {
        //INotification对象中包含通知名+通知包含信息
        switch (notification.Name)
        {
            //额,说啥监听了 SendNotification(pure_Notification.UPDATE_PLAYER_INFO);就都会执行更新的方法?
           case pure_Notification.UPDATE_PLAYER_INFO:
               //收到通知所做的处理
               //ViewComponent就是Mediator中具体存储的面板属性
               (ViewComponent as pure_MainView)?.UpdateInfo_View(notification.Body as pure_PlayerDataObj);
                break;
           //这俩已经由command处理了,这里空着也行(不如说注释掉比较好,浪费性能)
           // case pure_Notification.SHOW_PANEL:
           //      break;
           // case pure_Notification.HIDE_PANEL:
           //      break;
        }
    }
    
    //可选: 重写注册时的方法
    //Mediator 被 注册到 Facade 时自动调用(这个注册在pure_ShowPanelCommand.cs 里,不在 Facade)
    public override void OnRegister()
    {
        base.OnRegister();
        //额外的初始化内容
    }
    
    //封装设置View上控件的方法
    public void SetView(pure_MainView view)
    {
        // mediatorMain.SetView(mainPanel.GetComponent<pure_MainView>());
        //显示面板的时候把上面的脚本(提前挂载了View脚本)绑定到ViewComponent
        ViewComponent = view;
        // 统一判空,避免 Inspector 漏拖引用直接崩
        if (view == null||view.btnRole == null)
        {
            Debug.Log("view == null||view.btnRole == null,无法执行");
            return;
        }
        
        view.btnRole.onClick.AddListener(() =>
            {
                Debug.Log("点击了btnRole,自动发送通知");
                SendNotification(pure_Notification.SHOW_PANEL, "RolePanel");
            }
        );
    }
}
```

### 5.3 pure_RoleView — 角色面板

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pure_RoleView : MonoBehaviour
{
    public Button btnClose;
    public Button btnLevUp;
    
    public Text txtAtk;
    
    public void UpdateInfo_View(pure_PlayerDataObj playerData)
    {
        if (playerData == null || txtAtk == null) return;
        txtAtk.text = playerData.playerAtk.ToString();
    }
}
```

### 5.4 pure_RoleView_Mediator — 角色面板中介者

```csharp
using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;

public class pure_RoleView_Mediator : Mediator
{
    public static new string NAME = "pure_RoleView";
    public pure_RoleView_Mediator() : base(NAME)
    {
        //这时面板还没有实例化,没有view对象能拿来赋值,只能默认null了
        
        //实际父类构造函数有(string mediatorName, object viewComponent = null)
        //这里没有给viewComponent赋值,放在SetView(pure_RoleView view)里面
        
    }
    
    //重写监听通知的方法
    public override string[] ListNotificationInterests()
    {
        return new string[]
        {
            pure_Notification.UPDATE_PLAYER_INFO,
            pure_Notification.SHOW_PANEL,
            pure_Notification.HIDE_PANEL
        };
    }
    
    //重写处理通知的方法
    public override void HandleNotification(INotification notification)
    {
        switch (notification.Name)
        {
            case pure_Notification.UPDATE_PLAYER_INFO:
                //这里按LevUp就会报错,好像是这个时候ViewComponent已经为空,所以需要判空
                //UPDATE_PLAYER_INFO 触发时 RolePanel 已关闭,Mediator 仍会接收通知,有点浪费
                if (ViewComponent != null)
                {
                    (ViewComponent as pure_RoleView)?.UpdateInfo_View(notification.Body as pure_PlayerDataObj);
                }
                break;
            case pure_Notification.SHOW_PANEL:
                break;
            case pure_Notification.HIDE_PANEL:
                break;
        }
    }
    
    //封装设置View上控件的方法
    public void SetView(pure_RoleView view)
    {
        //哦哦是在这里赋值的,这tm太容易看漏了
        ViewComponent = view;
        // 统一判空,避免 Inspector 漏拖引用直接崩
        if (view == null || view.btnClose == null || view.btnLevUp == null)
        {
            Debug.Log("view == null||view.btnClose == null,无法执行");
            return;
        }
        //监听关闭按钮
        view.btnClose.onClick.AddListener(() =>
            {
                Debug.Log("点击了btnClose,自动发送HIDE_PANEL通知");
                //这里this就是pure_RoleView_Mediator实例,因为HidePanelCommand会访问ViewComponent字段
                //注意ViewComponent初始=null,上面才 ViewComponent = view;赋值
                SendNotification(pure_Notification.HIDE_PANEL, this);
            }
        );
        //监听升级按钮
        view.btnLevUp.onClick.AddListener(() =>
            {
                Debug.Log("点击了btnLevUp,自动发送通知");
                //这里其实没用到this
                SendNotification(pure_Notification.LEVEL_UP, this);
            }
        );
    }
    
    //可选: 重写注册时的方法
    public override void OnRegister()
    {
        base.OnRegister();
        //额外的初始化内容
    }
}
```

### Mediator 关键设计

| 方法 | 作用 | PureMVC 规则 |
|------|------|--------------|
| `ListNotificationInterests()` | 返回该 Mediator 关心哪些通知 | Facade 在 `SendNotification` 时遍历所有 Mediator，看通知名是否在数组里 |
| `HandleNotification(INotification)` | 收到关心通知后的处理 | `notification.Name` 是通知名，`notification.Body` 是 `object` 携带的数据 |
| `OnRegister()` | 注册到 Facade 时调用一次 | 适合做 Mediator 自身的初始化（**面板 GameObject 还没创建**，不能在这里碰 View） |
| `SetView(view)` | 把 GameObject 上的 View 脚本绑定到 `ViewComponent` | **自定义方法**（非 PureMVC 标准），但很常用——显示面板的 Command 加载完预制体后调它 |

> Mediator **持有 ViewComponent**（`object` 类型，可强转为具体 View 脚本），所以 Mediator 是 View 与 PureMVC 框架之间的桥。View 脚本不懂框架，只懂 UI；Mediator 懂框架 + 懂 View 的接口。

---

## 六、Controller 层：4 个 Command

Command 是 PureMVC 处理业务逻辑的地方。**`Execute(INotification)` 在 `new` 出来时由 Controller 自动调用**，无需手动执行。

### 6.1 pure_StartUpCommand — 启动命令

```csharp
using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityEngine;

//继承SimpleCommand
public class pure_StartUpCommand : SimpleCommand
{
   //重写其中的执行函数(命令被执行时自动调用该方法)
   public override void Execute(INotification notification)
   {
      base.Execute(notification);
      Debug.Log("执行pure_StartUpCommand命令,自动调用其中函数");
      //避免重复,没有有数据代理才注册
      if(!Facade.HasProxy(pure_PlayerProxy.NAME))
      {
         //pure_PlayerProxy 实例存进Facade字典
         //后面任何地方(Command、Mediator)都能通过 Facade.RetrieveProxy(NAME) 拿到同一个实例
         //不注册就每次 new 一个新 Proxy,数据各存各的,升级了也白升,下次拿又是初始值
         Facade.RegisterProxy(new pure_PlayerProxy()); 
      }
   }
}
```

### 6.2 pure_ShowPanelCommand — 显示面板命令

这是最复杂的一个 Command：根据通知 Body 中的面板名 switch，注册对应 Mediator，加载预制体，绑定 ViewComponent，并触发首次数据刷新。

```csharp
using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityEngine;

public class pure_ShowPanelCommand : SimpleCommand
{
   //依旧重写方法
    public override void Execute(INotification notification)
    {
        base.Execute(notification);
        Debug.Log("执行pure_ShowPanelCommand命令,自动调用其中函数");
        //面板创建相关的具体逻辑
        //SendNotification(pure_Notification.SHOW_PANEL, "MainPanel");
        //这里notification应该是包含Name和Body两部分?
        string panelName = notification.Body.ToString();
        switch (panelName)
        {
            case "MainPanel":
            //显示面板相关的逻辑
          
            //使用Mediator前也一定要在Facade里面去注册(和Command,Proxy都一样)
            //直接在命令中使用Facade,代表全局唯一的Facade
            //额...每个面板的Mediator也是全局唯一的?
            if (!Facade.HasMediator(pure_MainView_Mediator.NAME))
            {
                //这里就会调用pure_MainView_Mediator构造函数
                Facade.RegisterMediator(new pure_MainView_Mediator());
            }
            //通过Facade获取Mediator的方法
            pure_MainView_Mediator mediatorMain =
                Facade.RetrieveMediator(pure_MainView_Mediator.NAME) as pure_MainView_Mediator;

            //后面写了的,显示面板时会把面板上脚本绑定到ViewComponent
            if (mediatorMain.ViewComponent == null)
            {
                //有了Mediator 下一步就是创建界面预设体(已经提前挂载好了View脚本)
                GameObject res = Resources.Load<GameObject>("Panel/MainPanel");
                GameObject mainPanel = GameObject.Instantiate(res);
                mainPanel.transform.SetParent(GameObject.Find("Canvas").transform, false);
                //获取预设体上面的View脚本然后关联到Mediator上
                //mediatorMain.ViewComponent = mainPanel.GetComponent<pure_MainView>();
                //还可以直接获取控件
               //(mediatorMain.ViewComponent as pure_MainView).btnRole
                
               //但是这里还是使用对应mediator类中的封装方法吧
               //面板View脚本(预制体中已经挂载)绑定到Mediator .ViewComponent
               mediatorMain.SetView(mainPanel.GetComponent<pure_MainView>());
                
               //实现面板后需要进行第一次更新
               //需要把数据通过参数一起传出去
               var proxyMain = Facade.RetrieveProxy(pure_PlayerProxy.NAME) as pure_PlayerProxy;
               if (proxyMain != null)
                   //通过Mediator线路,实现不需要每个面板单独写转化并调用UpdateInfo_View逻辑
                   SendNotification(pure_Notification.UPDATE_PLAYER_INFO, proxyMain.Data);
               else
                   Debug.LogError("pure_PlayerProxy 未注册,跳过第一次 UPDATE_PLAYER_INFO 刷新");
            }
            break;
            
            case "RolePanel":
                if (!Facade.HasMediator(pure_RoleView_Mediator.NAME))
                {
                    Facade.RegisterMediator(new pure_RoleView_Mediator());
                }
                pure_RoleView_Mediator mediatorRole =
                    Facade.RetrieveMediator(pure_RoleView_Mediator.NAME) as pure_RoleView_Mediator;

                if (mediatorRole.ViewComponent == null)
                {
                    GameObject res = Resources.Load<GameObject>("Panel/RolePanel");
                    GameObject rolePanel = GameObject.Instantiate(res);
                    rolePanel.transform.SetParent(GameObject.Find("Canvas").transform, false);
                    mediatorRole.SetView(rolePanel.GetComponent<pure_RoleView>());
                    //同样需要更新
                    var proxyROle = Facade.RetrieveProxy(pure_PlayerProxy.NAME) as pure_PlayerProxy;
                    if (proxyROle != null)
                        SendNotification(pure_Notification.UPDATE_PLAYER_INFO, proxyROle.Data);
                    else
                        Debug.LogError("pure_PlayerProxy 未注册,跳过第一次 UPDATE_PLAYER_INFO 刷新");
                }
                break;
        }
    }
}
```

### 6.3 pure_HidePanelCommand — 隐藏面板命令

```csharp
using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using PureMVC.Patterns.Mediator;
using UnityEngine;

public class pure_HidePanelCommand : SimpleCommand
{
    //依旧重写方法
    //return new时controller自动调用类中重写的Execute函数
    //controller自动把 SendNotification(pure_Notification.HIDE_PANEL, this);的通知名和参数组合为INotification 对象
    public override void Execute(INotification notification)
    {
        base.Execute(notification);
        Debug.Log("执行pure_HidePanelCommand命令,自动调用其中函数");
        //面板隐藏相关的具体逻辑
        //因为我们之前传入的this就是pure_RoleView_Mediator实例嘛,就把这里notification.Body(Object类型)向下转型
        Mediator mediator = null;
        // body 是字符串:当做面板名,自己查 Mediator
        if (notification.Body is string panelName)
        {
            // 约定:面板名 = Mediator.NAME(比如 "MainPanel" 对应 pure_MainView_Mediator.NAME)
            mediator = Facade.RetrieveMediator(panelName) as Mediator;
        }
        // body 是 Mediator:直接用
        else if (notification.Body is Mediator)
        {
            mediator = notification.Body as Mediator;
        }
        if (mediator != null && mediator.ViewComponent != null)
        {
            //直接删除场景上对象(AI建议还是改为SetActive方案)
            //HidePanel要处理多种面板,这里就不要转化为具体面板如pure_RoleView
            //所有面板都是Mono的子类,这里就相当于Object爷转化为Mono儿,而不是具体Panel孙
            GameObject.Destroy((mediator.ViewComponent as MonoBehaviour).gameObject);
            //删掉后一定要置空
            mediator.ViewComponent = null;
        }
    }
}
```

### 6.4 pure_LevUpCommand — 升级命令

```csharp
using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityEngine;

public class pure_LevUpCommand : SimpleCommand
{
    public override void Execute(INotification notification)
    {
        //好吧,没啥卵用的代码,留着纯习惯
        base.Execute(notification);
        //得到数据代理,调用升级,完成后通知别人更新数据
        //前面StartUpCommand中注册了,反正RegisterProxy和RetrieveProxy这俩方法知道是存取同一个实例就行
        pure_PlayerProxy playerProxy = Facade.RetrieveProxy(pure_PlayerProxy.NAME) as pure_PlayerProxy;
        if (playerProxy != null)
        {
            playerProxy.LevUp();
            //注意发送的不是LEV_UP通知
            SendNotification(pure_Notification.UPDATE_PLAYER_INFO, playerProxy.Data);
        }
    }
}
```

### Command 设计要点

| 命令 | 触发者 | 作用 | 关键调用 |
|------|--------|------|----------|
| StartUpCommand | `Facade.StartUp()` 发 START_UP | 注册 PlayerProxy | `Facade.RegisterProxy(new pure_PlayerProxy())` |
| ShowPanelCommand | `Facade.StartUp()` / MainView.btnRole 发 SHOW_PANEL + 面板名 | 注册 Mediator + 加载预制体 + 绑定 ViewComponent + 首次刷新 | `Facade.RegisterMediator` + `Resources.Load` + `mediator.SetView` + `SendNotification(UPDATE_PLAYER_INFO)` |
| HidePanelCommand | RoleView.btnClose 发 HIDE_PANEL + this | 销毁 GameObject + 置空 ViewComponent | `GameObject.Destroy` + `mediator.ViewComponent = null` |
| LevUpCommand | RoleView.btnLevUp 发 LEVEL_UP | 取 Proxy 调 LevUp 后通知更新 | `Facade.RetrieveProxy` + `playerProxy.LevUp()` + `SendNotification(UPDATE_PLAYER_INFO)` |

> **Command 是一次性的**：每次 `SendNotification` 触发对应 Command 时，工厂 `() => new xxxCommand()` 都会 `new` 一个新实例，执行完 `Execute` 后丢弃。所以不要在 Command 里存状态字段，状态都放 Proxy 里。

---

## 七、Facade 门面 + MainEntry 入口

### 7.1 pure_Facade — 门面/外观

Facade 是 PureMVC 的「总管」：单例 + 初始化三大核心 + 注册命令映射 + 对外暴露 `StartUp()`。

```csharp
using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Facade;
using UnityEngine;

//Facade负责初始化框架，并统一管理 Model、View 和 Controller 三大核心模块
//1.首先继承
public class pure_Facade : Facade
{
   //父类Facade里面有 protected static IFacade instance;
   //和单例基类一个作用就对了,只是把向下转型放在了子类来写
    public static pure_Facade Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new pure_Facade();
            }
            //返回值为子类这里就要向下转型了(因为instance是IFacade类型嘛)
            return instance as pure_Facade;
        }
    }
    
    //Facade通过IController,IModel,IView三个接口管理对应的Core模块
    //初始化Controller相关的方法
    protected override void InitializeController()
    {
        base.InitializeController();
        //相当于在字典中添加映射,当SendNotification("START_UP")时,调用工厂(其中是怎么制造command的逻辑)
        //这里我们写的逻辑(或者说工厂函数)就是直接new一个pure_StartUpCommand实例(注意此时并不会执行Excute)
        RegisterCommand(pure_Notification.START_UP, () =>
        {
            //return new时controller自动调用类中的Execute函数
            return new pure_StartUpCommand();
        });
        
        RegisterCommand(pure_Notification.SHOW_PANEL, () =>
        {
            return new pure_ShowPanelCommand();
        });
        
        RegisterCommand(pure_Notification.HIDE_PANEL, () =>
        {
            return new pure_HidePanelCommand();
        });
        
        RegisterCommand(pure_Notification.LEVEL_UP, () =>
        {
            return new pure_LevUpCommand();
        });
    }
    
    //一定有一个启动函数(我们在Entry中调用的)
    public void StartUp()
    {
        //发送通知-->返回new pure_StartUpCommand()-->执行 Debug.Log("执行命令时自动调用的函数");
        SendNotification(pure_Notification.START_UP);
        
        //发送通知的时候也可以传入参数body,就是个Object类实例,收到通知直接Body取出要用的成员就行
        SendNotification(pure_Notification.SHOW_PANEL, "MainPanel");
    }
}
```

**关键设计**：
- `InitializeController()` 是 PureMVC 父类的虚方法，**Facade 第一次被 `Instance` 访问时自动调用一次**。所有 `RegisterCommand` 都写在这里。
- `RegisterCommand(通知名, 工厂)` 把「通知名 → Command 工厂」存进 Controller 的字典。`SendNotification(通知名)` 时 Controller 查字典、调工厂 `new` 出 Command、调 `Execute`、丢弃。
- **没注册的通知名不会触发任何 Command**——但仍然会走 Mediator 路径（双分发机制）。

### 7.2 pure_MainEntry — 场景入口

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pure_MainEntry : MonoBehaviour
{
    void Start()
    {
        pure_Facade.Instance.StartUp();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            pure_Facade.Instance.SendNotification
            (
                pure_Notification.SHOW_PANEL,
                "MainPanel"
            );      
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            pure_Facade.Instance.SendNotification
            (
                pure_Notification.HIDE_PANEL, 
                //经过我们修改,这里直接传入"MainView"也可以了
                pure_Facade.Instance.RetrieveMediator(pure_MainView_Mediator.NAME)
            );   
        }
    }
}
```

> 入口脚本只做两件事：启动 Facade + 把外部输入（按键）转成通知。所有业务逻辑都在 Command / Mediator / Proxy 里。

---

## 八、通知的双分发机制

`SendNotification(通知名, body)` 时，Facade 内部**同时**做两件事：

```
SendNotification("SHOW_PANEL", "MainPanel")
        │
        ├─→ Controller 路径：查 RegisterCommand 字典
        │     找到 "SHOW_PANEL" → new pure_ShowPanelCommand() → Execute(notification)
        │
        └─→ Mediator 路径：遍历所有已注册的 Mediator
              对每个 Mediator 调 ListNotificationInterests()
              如果返回的 string[] 包含 "SHOW_PANEL"，调它的 HandleNotification(notification)
```

**示例**：
- `SHOW_PANEL` 注册了 Command（`pure_ShowPanelCommand`），同时 `pure_RoleView_Mediator` 的 `ListNotificationInterests` 也包含它——所以两边都会触发。
- `UPDATE_PLAYER_INFO` **没注册 Command**，只有 Mediator 监听——只走 Mediator 路径。
- `LEVEL_UP` 注册了 Command（`pure_LevUpCommand`），没 Mediator 监听——只走 Command 路径。

> 这是 PureMVC 解耦的核心：**发通知的人不需要知道是 Command 处理还是 Mediator 处理，甚至两者都处理**。Command 负责业务逻辑（创建面板 / 调 Proxy），Mediator 负责界面刷新（更新 Text）。两者通过通知名约定协作，但互不引用。

---

## 九、完整调用链路

### 9.1 启动流程

```
场景启动
  ↓
pure_MainEntry.Start()
  ↓
pure_Facade.Instance.StartUp()
  ├─ 1. SendNotification(START_UP)
  │     ↓ Controller 路径
  │     pure_StartUpCommand.Execute()
  │       └─ Facade.RegisterProxy(new pure_PlayerProxy())
  │            （Proxy 构造时从 PlayerPrefs 读 JOJO/2/99）
  │
  └─ 2. SendNotification(SHOW_PANEL, "MainPanel")
        ↓ Controller 路径
        pure_ShowPanelCommand.Execute()
          ├─ Facade.RegisterMediator(new pure_MainView_Mediator())
          ├─ Resources.Load("Panel/MainPanel") → Instantiate
          ├─ mediatorMain.SetView(view)  // 绑定 ViewComponent + btnRole.onClick
          └─ SendNotification(UPDATE_PLAYER_INFO, proxyMain.Data)
                ↓ Mediator 路径（无 Command 注册）
                pure_MainView_Mediator.HandleNotification()
                  └─ (ViewComponent as pure_MainView).UpdateInfo_View(data)
                       └─ txtName.text = "JOJO"; txtAtk.text = "99"; ...
```

### 9.2 点击「角色」按钮 → 显示角色面板

```
玩家点击 btnRole
  ↓ (pure_MainView_Mediator.SetView 中绑定的闭包)
SendNotification(SHOW_PANEL, "RolePanel")
  ↓ Controller 路径
pure_ShowPanelCommand.Execute()
  ├─ Facade.RegisterMediator(new pure_RoleView_Mediator())
  ├─ Resources.Load("Panel/RolePanel") → Instantiate
  ├─ mediatorRole.SetView(view)  // 绑定 ViewComponent + btnClose/btnLevUp.onClick
  └─ SendNotification(UPDATE_PLAYER_INFO, proxy.Data)
        ↓ Mediator 路径
        ├─ pure_MainView_Mediator.HandleNotification()  // 主面板也刷新（其实没变化）
        └─ pure_RoleView_Mediator.HandleNotification()
              └─ (ViewComponent as pure_RoleView).UpdateInfo_View(data)
                   └─ txtAtk.text = "99"
```

### 9.3 点击「升级」按钮

```
玩家点击 btnLevUp
  ↓ (pure_RoleView_Mediator.SetView 中绑定的闭包)
SendNotification(LEVEL_UP, this)
  ↓ Controller 路径
pure_LevUpCommand.Execute()
  ├─ playerProxy = Facade.RetrieveProxy(NAME) as pure_PlayerProxy
  ├─ playerProxy.LevUp()
  │     ├─ data.playerLev += 1   // 2 → 3
  │     ├─ data.playerAtk += data.playerLev  // 99 + 3 = 102
  │     └─ SaveData()  // 回写 PlayerPrefs
  └─ SendNotification(UPDATE_PLAYER_INFO, playerProxy.Data)
        ↓ Mediator 路径
        ├─ pure_MainView_Mediator.HandleNotification()
        │     └─ MainView.UpdateInfo_View()  // txtLevel="3" txtAtk="102"
        └─ pure_RoleView_Mediator.HandleNotification()
              └─ RoleView.UpdateInfo_View()  // txtAtk="102"
```

### 9.4 点击「关闭」按钮

```
玩家点击 btnClose
  ↓ (pure_RoleView_Mediator.SetView 中绑定的闭包)
SendNotification(HIDE_PANEL, this)  // this = pure_RoleView_Mediator 实例
  ↓ Controller 路径
pure_HidePanelCommand.Execute()
  ├─ notification.Body is Mediator → mediator = Body as Mediator
  ├─ GameObject.Destroy((mediator.ViewComponent as MonoBehaviour).gameObject)
  └─ mediator.ViewComponent = null
```

### 9.5 按 W 键隐藏 MainPanel

```
pure_MainEntry.Update() 检测到 W 键
  ↓
pure_Facade.Instance.SendNotification(HIDE_PANEL, RetrieveMediator(NAME))
  // 这里 Body 是 Mediator 实例（也可以传 "MainView" 字符串，HidePanelCommand 两种都支持）
  ↓ Controller 路径
pure_HidePanelCommand.Execute()
  └─ 销毁 MainPanel GameObject
```

---

## 十、几个关键设计点

### 10.1 为什么 Mediator 要在 ShowPanelCommand 里注册，而不是在 Facade 里

- Facade 的 `InitializeController()` 在第一次访问 `Instance` 时调用一次，适合注册**全局唯一**的东西（Command、启动时的 Proxy）。
- 面板 Mediator 是**按需注册**的：玩家没打开 RolePanel 之前，`pure_RoleView_Mediator` 不应该占着 Facade 字典，否则它会收到无用的 UPDATE_PLAYER_INFO 通知白白浪费性能。
- `ShowPanelCommand` 里 `if (!Facade.HasMediator(NAME))` 保证只注册一次，重复显示同一面板不会重复注册。

### 10.2 为什么 Mediator 持有 ViewComponent 而不是直接继承 View 脚本

- Mediator 是**纯 C# 类**（继承 `Mediator`，不继承 MonoBehaviour），不能挂在 GameObject 上。
- View 脚本是 MonoBehaviour，挂在预制体上，负责 Inspector 拖拽的控件引用。
- 二者通过 `ViewComponent`（`object` 类型）关联：Command 加载预制体 → `GetComponent<pure_MainView>()` → `mediator.SetView(view)` → Mediator 内部 `ViewComponent = view`。
- 这样 Mediator 可以**脱离 Unity 场景做单元测试**（ViewComponent 传 mock 对象）。

### 10.3 为什么 HidePanelCommand 支持两种 Body 类型

- 字符串面板名：调用方不用知道 Mediator（`pure_MainEntry` 按 W 时就这样调）。
- Mediator 实例：调用方就是 Mediator 自己（`pure_RoleView_Mediator.btnClose` 闭包里 `SendNotification(HIDE_PANEL, this)`）。
- Command 里用 `is` 模式匹配两种都支持，灵活。

### 10.4 关键风险点

- **`pure_RoleView_Mediator` 监听了 SHOW_PANEL 和 HIDE_PANEL 但 `HandleNotification` 里 case 空着**：会浪费一次 switch 分发。注释里也提到了，纯演示用，生产代码可以删掉这两个监听。
- **HidePanelCommand 销毁 GameObject 后没 `RemoveMediator`**：Mediator 还在 Facade 字典里，下次再 Show 同一面板时 `HasMediator` 返回 true 走不进 `RegisterMediator` 分支，但 `ViewComponent == null` 会进 `SetView` 分支，所以能复用。如果想彻底清理，加 `Facade.RemoveMediator(NAME)`。
- **`pure_PlayerProxy.LevUp` 不是线程安全**：PureMVC 默认在主线程使用没问题，但如果接到服务器推送的多线程通知要小心。

---

## 十一、和上篇普通 MVC / MVP 的对比

| 维度 | 普通 MVC（前篇） | MVP（前篇） | **PureMVC（本篇）** |
|------|------------------|-------------|----------------------|
| 中间层 | Controller 类 | Presenter 类 | Facade + Command + Mediator + Proxy |
| 通信 | View 直接调 Model 方法 | View 调 Presenter，Presenter 调 Model | 全部 `SendNotification` |
| View 与 Model | 直接引用 | 通过 Presenter | 完全互不可见 |
| 加新面板 | 改 Controller + 加 View | 加 Presenter + View | 加 View + Mediator + 在 ShowPanelCommand 加一个 case |
| 跨面板协作 | 各 Controller 互相引用 | 各 Presenter 互相引用 | 通过通知协作，互不引用 |
| 学习成本 | 低 | 中 | 高（要懂通知双分发、Command 一次性、Mediator 生命周期） |
| 适合规模 | 小项目 | 中项目 | 大项目 / 多面板协作 / 需要解耦的场景 |

PureMVC 的「重」换来的是**加面板 / 改业务流程对老代码零侵入**：你加一个 SkillPanel，只需要写 `pure_SkillView` + `pure_SkillView_Mediator` + 在 `pure_ShowPanelCommand` 加 `case "SkillPanel"`，MainPanel 和 RolePanel 一行代码都不用改。这是普通 MVC/MVP 做不到的。
