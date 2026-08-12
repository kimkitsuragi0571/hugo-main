+++
title = "Unity MVP框架实现"
date = "2026-07-17T22:19:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "设计模式", "MVP"]
+++

MVP（Model-View-Presenter）是 MVC 的演变版本，核心区别在于：View 不再直接接收 Model 数据，所有数据流转都由 Presenter 集中处理，使 View 更加被动、纯粹。本文在上一篇 MVC 框架基础上，展示如何改造为 MVP 结构。

## 一、Model层：PlayerModel

> **PlayerModel脚本**
> 直接复用即可

PlayerModel 与 MVC 中完全一致，负责数据存储与事件通知，无需任何改动。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//主要存储数据,不需要继承Mono
public class PlayerModel 
{
    //数据类具有唯一性,这里直接做成单例
    private static PlayerModel instance;

    public static PlayerModel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerModel();
                Instance.Init();
            }   
            return instance;
        }
    }
    
    //数据内容
    private string playerName;
    private int lev;
    private int atk;
    //事件用于Model通知外部数据更新,而非直接获取外部面板
    private event UnityAction<PlayerModel> updateEvent; 
    
    //每个变量都制作public属性方便外部访问
    public string PlayerName => playerName;
    public int Level => lev;
    public int Atk => atk;

    //初始化(和直接略过的一样使用PlayerPrefs)
    public void Init()
    {
        //查看项目注册表Key数值,默认为JOJO
        playerName = PlayerPrefs.GetString("PlayerName","JOJO");
        lev = PlayerPrefs.GetInt("PlayerLev", 1);
        atk = PlayerPrefs.GetInt("PlayerAtk", 20);
    }
    
    //更新
    public void LevUp()
    {
        //升级 改变内容
        lev += 1;
        atk += lev;
        //改变后自动调用保存方法
        SaveData();
        //当 Model 层的数据发生变化时，自动通知所有需要刷新界面的 UI 组件
        //UI只需要注册事件,不需要让这个Model层获取组件
        UpdateInfo();
    }
    
    //保存方法
    private void SaveData()
    {
        PlayerPrefs.SetInt("PlayerLev", lev);
        PlayerPrefs.SetInt("PlayerAtk", atk);
    }
    
    //增减事件监听的方法,外部传入有PlayerModel类型作为参数的方法
    //想要传入方法作为参数,肯定只能用委托啊笨
    public void AddEvent(UnityAction<PlayerModel> action)
    {
        updateEvent += action;
    }
    
    public void RemoveEvent(UnityAction<PlayerModel> action)
    {
        updateEvent -= action;
    }

    //通知外部更新数据的方法
    private void UpdateInfo()
    {
        if (updateEvent != null)
        {
            //PlayerModel的唯一实例Instance作为参数传入已经加入事件订阅的方法中
            //这些方法内部写明提取Instance的哪些属性就可以了
            updateEvent?.Invoke(this);
        }
    }
}
```

---

## 二、MainPanel：主面板的 View 与 Presenter

### 1. mvp_MainView

> **View脚本**
> 只需要修改原来脚本中的方法

View 不再提供 `UpdateInfo(PlayerModel)` 方法接收 Model 对象，数据更新逻辑全部交给 Presenter。View 只保留控件声明，变得完全被动。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mvp_MainView : MonoBehaviour
{
  //1.获取控件(挂载后关联控件)
  public Button btnRole;
  public Button btnSill;
  
  public Text txtName;
  public Text txtLev;
  public Text txtPower;

  //不要用public void UpdateInfo(PlayerModel player)直接传入Model参数
  //当然这个方法也可以不写,全部丢到主持人Presenter里面执行
  //我们这里直接就放到MainPresenter.UpdateInfo里面执行了
  // public void UpdateInfo(string name,int lev,int atk)
  // {
  //   txtName.text = name;
  //   txtLev.text = lev.ToString();
  //   txtPower.text = atk.ToString();
  // }
}
```

### 2. MainPresenter

> **Presenter脚本**
> 对原来的Controller脚本修改即可

Presenter 替代了原来的 Controller，核心改动在于：数据不再通过 View 的方法传递，而是由 Presenter 直接操作 View 的控件进行更新。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPresenter : MonoBehaviour
{
    private mvp_MainView mainView;
    
    private static MainPresenter instance;
    
    public static MainPresenter Instance
    {
        get
        {
            return instance;
        }
    }

    public static void ShowView()
    {
        if (instance == null)
        {
            GameObject res = Resources.Load<GameObject>("UI/MainPanel");
            GameObject obj = Instantiate(res);
            obj.transform.SetParent(GameObject.Find("Canvas").transform, false);

            instance = obj.GetComponent< MainPresenter>();
        }
        instance.gameObject.SetActive(true);
    }

    public static void HideView()
    {
        if (instance != null)
        {
            instance.gameObject.SetActive(false);
        }
    }
    
    private void Start()
    {
        mainView = this.GetComponent<mvp_MainView>();
        //mainView.UpdateInfo(PlayerModel.Instance);
        //这里变成直接调用主持人中的UpdateInfo方法更新view
        UpdateInfo(PlayerModel.Instance);
        mainView.btnRole.onClick.AddListener(ClickRoleBtn);
        PlayerModel.Instance.AddEvent(UpdateInfo);
    }
    
    private void ClickRoleBtn()
    {
        //RoleController.ShowView();
    }
    
    //初始化Start()自动调用主持人UpdateInfo
    //因为注册了事件,同样是每次数据更新时自动调用UpdateInfo
    private void UpdateInfo(PlayerModel player)
    {
        if (mainView != null)
        {
            //mainView.UpdateInfo(player);
            //之前是数据从Model传入View,现在全部由Presenter传递
            //View中的那个方法也不用写了
            //好吧AI建议还是通过调用View层的接口,不然不符合MVP
            //mainView.UpdateInfo(player.PlayerName, player.Level, player.Atk);说是正确方法
            mainView.txtName.text = player.PlayerName;
            mainView.txtLev.text = player.Level.ToString();
            mainView.txtPower.text = player.Atk.ToString();
        }
    }

    private void OnDestroy()
    {
        PlayerModel.Instance.RemoveEvent(UpdateInfo);
    }
}
```

---

## 三、RolePanel：角色面板的 View 与 Presenter

### 1. mvp_RoleView

> **View脚本**
> 只需要修改原来脚本中的方法

同样地，RoleView 移除了 `UpdateInfo` 方法，只保留控件声明。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//针对RolePanel面板的方法(需要挂载到物体上,所以还是要继承Mono的)
public class mvp_RoleView : MonoBehaviour
{
    public Button btnLevelUp;
    public Button btnClose;
    public Text txtLev;

    // public void UpdateInfo(int lev)
    // {
    //     txtLev.text = lev.ToString();
    // }
}
```

### 2. RolePresenter

> **Presenter脚本**
> 对原来的Controller脚本修改即可

RolePresenter 直接操作 RoleView 的控件更新等级显示，按钮事件也由 Presenter 统一绑定。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RolePresenter : MonoBehaviour
{
    private mvp_RoleView roleView;
    
    private static RolePresenter instance;
    
    public static RolePresenter Instance
    {
        get
        {
            return instance;
        }
    }

    public static void ShowView()
    {
        if (instance == null)
        {
            GameObject res = Resources.Load<GameObject>("UI/RolePanel");
            GameObject obj = Instantiate(res);
            obj.transform.SetParent(GameObject.Find("Canvas").transform, false);

            instance = obj.GetComponent< RolePresenter>();
        }
        instance.gameObject.SetActive(true);
    }

    public static void HideView()
    {
        if (instance != null)
        {
            instance.gameObject.SetActive(false);
        }
    }
    
    private void Start()
    {
        roleView = this.GetComponent<mvp_RoleView>();
        UpdateInfo(PlayerModel.Instance);
        if (roleView.btnClose != null) 
        {
            roleView.btnClose.onClick.AddListener(ClickCloseBtn);
        }     
        PlayerModel.Instance.AddEvent(UpdateInfo);
    }
    
    private void ClickCloseBtn()
    {
        HideView();
    }

    
    private void UpdateInfo(PlayerModel player)
    {
        if (roleView != null)
        {
            roleView.txtLev.text = player.Level.ToString();
        }
    }

    private void OnDestroy()
    {
        PlayerModel.Instance.RemoveEvent(UpdateInfo);
    }
}
```

---

## 四、MVP 与 MVC 对比总结

| 对比项 | MVC | MVP |
|-------|-----|-----|
| 中间层名称 | Controller | Presenter |
| View 与 Model | View 可直接接收 Model | View 完全不接触 Model |
| 数据更新方式 | Controller 调用 View 方法传 Model | Presenter 直接操作 View 控件 |
| View 职责 | 声明控件 + 提供更新方法 | 仅声明控件，完全被动 |
| 耦合度 | View 与 Model 有一定耦合 | View 与 Model 完全解耦 |

**核心改动点：**
1. **Model 层**：直接复用，无需修改
2. **View 层**：移除 `UpdateInfo(PlayerModel)` 方法，不再接收 Model 对象
3. **Presenter 层**：由 Controller 改名而来，数据更新时直接操作 View 控件，而非调用 View 方法

MVP 的优势在于 View 变得更纯粹、更易测试，所有逻辑集中在 Presenter，便于统一管理和维护。
