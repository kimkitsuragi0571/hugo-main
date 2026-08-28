+++
title = "Unity UI模块实现"
date = "2026-08-28T17:45:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "UGUI", "面板管理", "UIPanelMgr", "虚方法重写"]
+++

UGUI 项目里，如果每个面板都手写「获取组件 → 绑定 onClick → 分别写方法」，代码会非常臃肿。本文实现一个完整的 UI 模块：**UIPanelBase 基类自动抓组件 + 虚方法统一订阅、UIPanelMgr 管理器负责面板加载 / 层级挂载 / 自定义事件封装、具体面板只需重写虚方法即可。**

---

## 一、四层结构

```
UIPanelBase (面板基类)
    ↳ 自动扫 Button / Image / Text / Slider / Toggle → 存入 Dict
    ↳ 自动为 Button / Toggle 订阅 OnClick / OnValueChanged 虚函数
    ↳ 提供 GetConFromDict<T>(uiName) 获取控件
    ↳ 提供 ShowMe / HideMe 显隐虚方法
           ↓
UIPanelMgr (面板管理器, MonoSingleton)
    ↳ 自动初始化 Canvas / EventSystem (先搜场景,没有就 Resources 加载)
    ↳ 分三层挂载：Top / Middle / Bottom
    ↳ ShowPanel<T>(name, layer, callback) 异步加载 + 设置父物体 + 调用 callback + 调用 ShowMe
    ↳ HidePanel(name) 调用 HideMe + 销毁 + 移除字典
    ↳ AddCustomEvent(con, triType, callback) 代码方式添加 EventTrigger 自定义逻辑
           ↓
PlayPanelTest (具体面板)
    ↳ 继承 UIPanelBase
    ↳ 只需 override OnClick(btnName) 用 switch 区分不同按钮
    ↳ override OnValueChanged(togName, isOn) 处理多选框
    ↳ override ShowMe / HideMe 执行面板专属逻辑
           ↓
UIPanelMgrTest (调用方)
    ↳ 仅负责在 Update 里按 Z 显示 / X 隐藏 面板
```

---

## 二、UIPanelBase 面板基类

核心思路：**基类 Awake 里自动扫描所有子控件（包括未激活的）并分类存入字典，遇到 Button 就闭包订阅同一个 `OnClick` 虚方法，遇到 Toggle 就订阅 `OnValueChanged`。子类只需重写这两个虚方法，不用再写 `GetComponent` + `AddListener`。**

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//UIPanel面板基类
public class UIPanelBase : MonoBehaviour
{
    //要存储多种UI组件,所有组件父类容器都是UIBehaviour
    //一个UI可能有多个组件比如Image和Button,添加了Button后,同一个UI的Image就没法添加了(因为Key无法重复嘛)
    //所以这里改为使用List
    private Dictionary<string, List<UIBehaviour>> conDict = new Dictionary<string, List<UIBehaviour>>();

    //事件监听中用于重写的虚函数
    protected virtual void OnClick(string btnName)
    {
        
    }

    protected virtual void OnValueChanged(string togName, bool isOn)
    {
        
    }
    //虚方法方便子类在Awake中处理额外逻辑
    protected virtual void Awake()
    {
        AddConToDict<Button>();
        AddConToDict<Image>();
        AddConToDict<Text>();
        AddConToDict<Slider>();
        AddConToDict<Toggle>();
    }

    
    //基类首先要提供获取所有组件然后显隐的方法
    //找到对应类型控件,然后添加到Dict里面,不需要以前一样依次声明变量然后拖拽
    private void AddConToDict<T>()where T : UIBehaviour
    {
        //未激活对象仍然获取
        T[] cons =  this.GetComponentsInChildren<T>(true);
       
      
        foreach (T con in cons)
        {
            if (con == null)
            {
                continue;
            }
            //检测是否已经有了对应的Key,没有就创建有了就添加
            //之前uiName变量是写在Foreach外部的,现在必须改到内部
            //不然uiName始终是被更新为最后一个foreach的con的名称,后面的闭包捕获就会出错
            string uiName = con.gameObject.name;
            if (conDict.ContainsKey(uiName))
            {
              
                //如果一个UI上有两个Button(UGUI不允许重复添加同类型组件,一般来说不会有这种情况)
                //但比如你不小心调用了两次Add方法或者脚本重复添加,就会导致List有重复项
                //为了避免上面的重复项,还是得判断该物体下是否已经有同类型组件
                if (!conDict[uiName].Contains(con))
                {
                    //不添加上面的约束这里就会报错
                    //有就直接指定Key(也就是指定了List.Add)添加Value
                    conDict[uiName].Add(con);
                }
            }
            else
            {
                //没有就创建新的键值对(也是要new List并添加con)
                conDict.Add(uiName, new List<UIBehaviour>() { con });
            }
            
            //新增:根据类型直接添加监听虚函数,后面重写即可
            if (con is Button)
            {
                //这里onClick限定添加无参函数不能直接传入OnClick,所以用闭包
               
                //每次循环,btn1和btn2两个不同的按钮,各自不同的onClick事件但是都注册了同一个onClick方法,只是传入的不同参数.
                //当我们点击其中任意一个按钮都会触发onClick函数,此时就需要通过传入uiName参数区分(用switch语句之类的)
                (con as Button).onClick.AddListener(() =>
                {
                    OnClick(uiName);
                });
            }
            else if (con is Toggle)
            {
                //这里订阅函数需要有个bool参数
                (con as Toggle).onValueChanged.AddListener((bool isOn) =>
                {
                    OnValueChanged(uiName, isOn);
                });
            }
        }
    }

    protected T GetConFromDict<T>(string uiName) where T : UIBehaviour
    {
        if (conDict.ContainsKey(uiName))
        {
            //如果包含某UI,则遍历其包含的List
            foreach (UIBehaviour con in conDict[uiName])
            {
                //List中某个组件如果是指定类型(UGUI不允许重复添加同类型组件,所以List中没有重复类型组件)
                //也和向下转型一样有简易写法 if (con is T target)return target;
                if (con is T)
                {
                    return con as T;
                }
            }
        }
        return null;
    }

    //显隐方法:这个我们在UIPanelMgr中调用(比如显示面板的时候调用对应面板ShowMe)
    public virtual void ShowMe(string panelName)
    {
        print("已经显示" + panelName);
    }

    public virtual void HideMe(string panelName)
    {
        print("已经隐藏" + panelName);
    }

}
```

### 重点：虚方法重写简化订阅

> **每次循环，btn1 和 btn2 两个不同的按钮，各自不同的 onClick 事件，但是都注册了同一个 `OnClick` 方法，只是传入的不同参数（`uiName`）。**
> **当我们点击其中任意一个按钮都会触发 `OnClick` 函数，此时就需要通过传入 `uiName` 参数区分（用 switch 语句之类的）。**

这是整个 UI 模块最核心的设计：

| 传统写法 | 基类自动订阅写法 |
|----------|------------------|
| 每个面板声明 `public Button btn1; public Button btn2;` 拖拽 | 声明 0 个变量，基类按名字全自动存入 conDict |
| `btn1.onClick.AddListener(OnBtn1Click); btn2.onClick.AddListener(OnBtn2Click);` | 基类闭包：`AddListener(() => OnClick(uiName));` 一行搞定所有按钮 |
| 每个按钮一个方法：`OnBtn1Click()` / `OnBtn2Click()` | 一个方法 `OnClick(btnName)` 内 switch 分支 |
| 加新按钮：声明变量 → 拖 → AddListener → 写方法（3~4 处改动） | 加新按钮：在 switch 里加一个 case（1 处改动） |

**注意闭包陷阱**：`uiName` 必须写在 `foreach` 内部。如果写在外部，所有闭包捕获的都是最后一次循环的变量值，导致所有按钮都被识别为同一个按钮名。

---

## 三、UIPanelMgr 面板管理器

```csharp
using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//各个子对象面板层级,直接用枚举来表示
public enum E_UILayer
{
   Top,
   Middle,
   Bottom
}


public class UIPanelMgr : MonoSingletonBase<UIPanelMgr>
{
   //这里存储各种面板,同样直接使用UI面板基类就可以
   public Dictionary<string, UIPanelBase> panelDict = new Dictionary<string, UIPanelBase>();
   
   //获取Canvas的Transform方便后续设置子面板,以及获取子对象的Transform
   public RectTransform canvasTrans;
   //虽然EventSystem并不需要设置子对象,但是我后面封装了方法传参,还是装模作样写一个吧
   public Transform eventSystemTrans;
   public RectTransform botTrans;
   public RectTransform topTrans;
   public RectTransform midTrans;
 
   //如果是纯C#单例就在构造函数中设置咯
   protected override void Awake()
   {
      base.Awake();
      //初始化负责防止重复 + 实例化 + 跨场景保留 + 获取Transform信息
      InitPanelRoot("Canvas", ref canvasTrans);
      InitPanelRoot("EventSystem", ref eventSystemTrans);
      
      //找到各层Transform
      botTrans = canvasTrans?.Find("Bottom") as RectTransform;
      topTrans = canvasTrans?.Find("Top") as RectTransform;
      midTrans = canvasTrans?.Find("Middle") as RectTransform;
   }
   
   //加载EventSystem和Canvas我们直接抽成一个方法了
   //传入的应该是组件名称(要么Canvas要么EventSystem)
   //以及需要用于存储的组件Transform(就是canvasTrans或者eventSystemTrans)
   //这里改为传入泛型而非直接Transform(为了可以传入RectTransform)
   private void InitPanelRoot<T>(string conName, ref T trans) where T : Transform
   {
      //为了避免场景中已经有Canvas/EventSystem,需要先检测(要么手动设置,要么之前脚本已经赋值)
      //但是Find方法找不到未激活的物体,更保险的方法是用Canvas[] allCanvases = FindObjectsOfType<Canvas>(true); 
      //但是这个API偏偏只能返回数组,必须遍历获取,我真懒得写了
      GameObject existingCon = GameObject.Find(conName);
      if (existingCon != null)
      {
         //如果有就直接用
         Debug.Log($"检测到场景中已有  {conName},直接复用");
         trans = existingCon.transform as T;
         DontDestroyOnLoad(existingCon);
      }
      else
      {
         //当场景中没有现存Canvas(我们手动设置的),且没有重复设置(如果是重复设置,这里canvasTrans肯定有值)
         if (trans == null)
         {
            Debug.Log($"检测到场景/内存中没有  {conName},终于可以Res加载了哈");
            //注意Canvas的子对象本身也是分层级的:
            //比如面板在分别在Top,Bottom,Middle等子对象下,又或者某个面板始终在另一个面板上方
            //所以我们分别设置Canvas和EventSystem作为预设体(Canvas需要设置为Scale with Screen Size)
            //这里不要异步加载了,容易导致时序灾难
            GameObject entityPanel = ResLoadMgr.Instance.LoadRes<GameObject>("Panel/" + conName);
            //哎呀怪不得场景上没有变化,忘了实例化了哈
            //GameObject EntityCon = Instantiate(prefab);好吧其实不用,我们ResLoadMgr里面就会自动实例化,这里导致重复
            //这是为了去掉实例化时的去掉(Clone)后缀,不然后面脚本就没法用了
            entityPanel.name = conName;
            //之前canvasTrans = Canvas.transform;还有DontDestroyOnLoad(Canvas);
            trans = entityPanel.transform as T;
            DontDestroyOnLoad(entityPanel);
         }
         else
         {
            //比如刚从其他场景里面切换
            Debug.Log($"内存里还有缓存的{conName},{conName}Trans依旧有效");
         }
      }
   }

   
//这里也新增传入枚举
//这里还可以传入callback回调函数预设体执行成功后自动执行的函数,默认null,调用的时候传入
   public void ShowPanel<T>(string panelName, E_UILayer uiLayer = E_UILayer.Middle, UnityAction<T> callback = null) where T : UIPanelBase
   {
      //如果重复调用ShowPanel,之前已经有对应Key
      //如果这里异步加载还没完成又有其他地方调用异步,这里Dict[panelName]就是空的,不过暂时也不纠结这个了
      //直接调用callback
      if (panelDict.ContainsKey(panelName))
      {
         //加载完毕后调用
         //注意这里字典里面都是存放的基类,所以需要转化为泛型指定类型
            callback?.Invoke(panelDict[panelName] as T);
            return;
      }

      //游戏面板体积较大,还是得用加载,传入的是面板在Resources下的路径
      //额,要是想加载AB包下的预制体,干脆就不要用ResLoadMgr
      //之前讲的,这个方法需要用callback函数来获取返回值
      ResLoadMgr.Instance.LoadResAsync<GameObject>("Panel/"+panelName, (GameObject panel) =>
      {
         print("加载面板"+ panel.gameObject.name);
         //只是加载出了面板,还需要将其设置为Canvas的子对象 + 设置其相对位置 + 设置所属层级
         
         //代表物体父对象的Transform,默认是botTrans(好吧现在改成RectTransform了)
         RectTransform panelFather = botTrans;
         switch (uiLayer)
         {
            case E_UILayer.Top:
               panelFather = topTrans;
               break;
            case E_UILayer.Middle:
               panelFather = midTrans;
               break;
            case E_UILayer.Bottom:
               panelFather = botTrans;
               break;
         }
         //检查完毕直接设置父对象,并添加到字典
         panel.transform.SetParent(panelFather);
         panelDict[panelName] = panel.GetComponent<T>();
         //设置物体相对位置
         panel.transform.localPosition = Vector3.zero;
         panel.transform.localScale = Vector3.one;
         //设置RectTransform
         //之前(panel.transform as RectTransform).offsetMax = Vector3.zero;
         //修改为了直接使用 RectTransform不然每次都得修改
         (panel.transform as RectTransform).offsetMax = Vector3.zero;
         (panel.transform as RectTransform).offsetMin = Vector3.zero;
         
         //面板加载完成后,调用callback
         callback?.Invoke(panelDict[panelName] as T);
         
         //顺带调用面板基类里面的ShowMe()
         panelDict[panelName].ShowMe(panelDict[panelName].gameObject.name);
      });
   }

   public void HidePanel(string panelName)
   {
      if (panelDict.ContainsKey(panelName))
      {
         //顺带调用面板基类里的HideMe()
         panelDict[panelName].HideMe(panelDict[panelName].gameObject.name);
         //好吧这里并不是频繁销毁,直接Destory就行
         GameObject.Destroy(panelDict[panelName].gameObject);
         panelDict.Remove(panelName);
       
      }
   }
   
   //再来个获取已经显示的面板的方法
   public T GetPanel<T>(string panelName) where T : UIPanelBase
   {
      if (panelDict.ContainsKey(panelName))
      {
         //注意不要和面板基类存储的那个Dict搞混,这里Value直接就是面板基类
         return panelDict[panelName] as T;
      }
      return null;
   }
   
   //再来个获取层级对应父对象的方法
   public Transform GetLayFather(E_UILayer uiLayer)
   {
      switch (uiLayer)
      {
         case E_UILayer.Top:
            return this.topTrans;
            
         case E_UILayer.Middle:
            return this.midTrans;
            
         case E_UILayer.Bottom:
            return this.botTrans;
            
      }

      return null;
   }
   
   
   //代码实现自定义逻辑的方法---->我们直接封装在UIMgr,其他详细解释看那个PlayPanelTest的老版
   //任意控件都可以添加自定义事件(吗?)所以这里直接填入UI基类
   //传入检测的事件类型,以及回调的函数
   public void AddCustomEvent(UIBehaviour con,EventTriggerType triType,UnityAction<BaseEventData> callback)
   {
      EventTrigger trig = con.GetComponent<EventTrigger>();
      if (trig == null)
      {
         trig = con.gameObject.AddComponent<EventTrigger>();
      }
      // 新建 EventTrigger 时 triggers 为 null,必须先初始化
      if (trig.triggers == null)
      {
         trig.triggers = new List<EventTrigger.Entry>();
      }
      EventTrigger.Entry entry = new EventTrigger.Entry();
      entry.eventID = triType;
      entry.callback.AddListener(callback);
      
      trig.triggers.Add(entry);
   }
}
```

---

## 四、PlayPanelTest 具体面板（重写虚方法 + 重写显隐）

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayPanelTest : UIPanelBase
{
   void Start()
   {
      //直接获取组件然后添加事件就可以了
      // GetConFromDict<Button>("GreenBtn").onClick.AddListener(() =>
      // {
      //    print("点击了绿色Play按钮");
      //    GetConFromDict<Slider>("Slider").value += 0.1f;
      // });
      //老版本:每个子面板里面对特定按钮添加监听
      //改进版本:在面板基类中,每次检测到Button就自动添加虚函数监听,现在只需要重写虚函数本身就可以了
      
      //
      // //如果直接写自定义点击逻辑-->用了这个Button.onClick自动失效
      // //1.获取指定Button并为其添加EventTrigger组件
      // EventTrigger trig = GetConFromDict<Button>("GreenBtn").gameObject.AddComponent<EventTrigger>();
      // //2.创建Entry监听条目(相当于个空的表格嘛)
      // EventTrigger.Entry entryDrag = new EventTrigger.Entry();
      // //3.指定事件类型,点击就是PointerClick,按下就是PointerDown
      // //这里就指定Drag拖拽(entryDrag这一个对象只能指定这一种类型)
      // entryDrag.eventID = EventTriggerType.Drag;
      // //4.绑定回调函数,当发生拖拽的时候调用OnDrag函数
      // entryDrag.callback.AddListener(OnDrag);
      //改进版:每种操作每个面板都得写一大串,直接在UIPanelMgr(注意不是面板基类)写方法
      
      UIPanelMgr.Instance.AddCustomEvent(GetConFromDict<Button>("GreenBtn"), EventTriggerType.PointerDown, (data) =>
      {
         print("正在按下按钮");
      });
   }
   
   // //OnDrag要求必须有BaseEventData类型参数
   // void OnDrag(BaseEventData data)
   // {
   //    // 1. 必须先强转为 PointerEventData (拖拽/点击 用的是 PointerEventData)
   //    //下拉菜单选择变化 用的是 SelectionEventData,输入框内容变化 用的是 InputField.SubmitEvent子类等等
   //    PointerEventData pointerData = data as PointerEventData;
   //    if (pointerData != null)
   //    {
   //       print("开始拖拽!!!");
   //       // 2. 获取当前鼠标/手指的屏幕坐标
   //       Vector2 currentPos = pointerData.position; 
   //       // 3. 获取从上次触发 OnDrag 到现在的位移增量 (最常用！)
   //       Vector2 delta = pointerData.delta; 
   //       // 4. 获取按下的是哪个键 (左键/右键/中键)
   //       PointerEventData.InputButton button = pointerData.button;
   //    }
   // }

   //重写参数不能变吧,权限也要注意
   //所有按钮的不同onClick事件都是触发同一个OnClick函数,所以这里直接分情况写逻辑
   protected override void OnClick(string btnName)
   {
      switch(btnName)
      {
         case "GreenBtn":
            print("点击了绿色Play按钮");
            GetConFromDict<Slider>("Slider").value += 0.1f;
            break;
         case "BlueBtn":
            print("点击了蓝色Play按钮");
            GetConFromDict<Slider>("Slider").value -= 0.1f;
            break;
      }
   }

   protected override void OnValueChanged(string togName, bool isOn)
   {
      //这里就是多选框啥的,开始判断
   }

   //应该在面板本体中重写基类,不要在调用者UIPanelMgrTest里面写
   //重写基类中的显隐调用逻辑
   public override void ShowMe(string panelName)
   {
      base.ShowMe(panelName);
      print("法!显示了PlayPanel面板");
   }

   public override void HideMe(string panelName)
   {
      base.HideMe(panelName);
      print("法!隐藏了PlayPanel面板");
   }
}
```

> **重写面板基类的显隐方法：应该在面板本体 `PlayPanel` 中重写基类，不要在调用者 `UIPanelMgrTest` 里面写。**
>
> 显隐逻辑是「面板自己的事」，调用方只管说「显示 / 隐藏 PlayPanel」，至于显示时要不要播放动画、发数据请求、重置控件值，全由具体面板自己决定，这才符合单一职责。

调用显隐的链路是：
`UIPanelMgr.Instance.ShowPanel<PlayPanelTest>("PlayPanel")` → 资源管理器加载完预制体 → `panelDict[panelName].ShowMe(...)` → 触发子类 override 的 ShowMe。

---

## 五、UIPanelMgrTest 调用方

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UIPanelMgrTest不是面板本体,只是个调用脚本
public class UIPanelMgrTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //这里只需要传入Panel名称即可,方法内部自动拼接Res文件夹下路径
            //callback传参以及泛型填面板预制体上的脚本
            //UnityAction要求的传入方法需要有一个T同款类型参数
            UIPanelMgr.Instance.ShowPanel<PlayPanelTest>("PlayPanel", E_UILayer.Top, (PlayPanelTest panel) =>
            {
                print(panel.name + "加载完成了喵");
            });
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            UIPanelMgr.Instance.HidePanel("PlayPanel");
        }
    }
   
}
```

这个脚本非常薄，只是用 Update 轮询输入演示用法。实际项目里可以放在主控制器里，或者通过事件中心触发 `ShowPanel` / `HidePanel`。

---

## 六、四个脚本各自职责总结

### 6.1 UIPanelBase — 面板基类

| 部分 | 代码位置 | 作用 |
|------|----------|------|
| `conDict` | `Dictionary<string, List<UIBehaviour>>` | 按 UI 名称存放该物体下所有 UIBehaviour 子组件（Button / Image 同属一个物体就都塞 List 里） |
| `virtual void Awake()` | 第 26-33 行 | 调用 5 次 `AddConToDict<T>()` 扫控件；子类重写时记得 `base.Awake()` |
| `AddConToDict<T>()` | 第 38-94 行 | 扫控件 + 存字典；**遇到 Button 闭包订阅 OnClick(uiName)，遇到 Toggle 闭包订阅 OnValueChanged(uiName, isOn)** — 这是整个 UI 模块的核心 |
| `GetConFromDict<T>(uiName)` | 第 96-112 行 | 从字典找控件并按类型返回；没找到返回 null，子类随便用 |
| `virtual void OnClick(btnName)` / `OnValueChanged(togName, isOn)` | 第 16-24 行 | 留空的虚方法，供子类 switch 重写 |
| `ShowMe / HideMe` | 第 115-123 行 | 基类仅打一条日志，具体面板 override 后可加动画、请求数据等 |

### 6.2 UIPanelMgr — 面板管理器

| 部分 | 代码位置 | 作用 |
|------|----------|------|
| 枚举 `E_UILayer` | 第 9-14 行 | Top / Middle / Bottom 三个层级，决定面板挂到 Canvas 的哪个子物体下 |
| `override void Awake()` | 第 31-42 行 | 调用两次 `InitPanelRoot` 初始化 Canvas / EventSystem，然后取各层 RectTransform |
| `InitPanelRoot<T>(...)` | 第 48-86 行 | **先 GameObject.Find 场景内是否已有同名物体（防重复）**，没有且缓存 trans == null 时，通过 ResLoadMgr 从 `Panel/xxx` 路径加载预制体，再重命名为原名（去掉 (Clone) 后缀）并 DontDestroyOnLoad |
| `ShowPanel<T>(...)` | 第 91-144 行 | 字典已存在 → 直接 callback；不存在 → `ResLoadMgr` 异步加载 → 按层级 SetParent → 存字典 → 调 callback → 调面板 `ShowMe` |
| `HidePanel(...)` | 第 146-157 行 | 调面板 `HideMe` → Destroy 面板对象 → 移除字典条目 |
| `GetPanel / GetLayFather` | 第 160-187 行 | 辅助查询：按名拿面板、按层拿父 Transform |
| `AddCustomEvent(...)` | 第 193-210 行 | 代码方式一键加 EventTrigger.Entry（含 triggers == null 时初始化），无需每个面板手写 4 步 |

### 6.3 PlayPanelTest — 具体面板

| 部分 | 代码位置 | 作用 |
|------|----------|------|
| `Start()` | 第 9-37 行 | 演示：`AddCustomEvent` 给 GreenBtn 加 `PointerDown` 事件（按下即触发，不等抬起）；大量旧写法注释用于对比 |
| `override OnClick(btnName)` | 第 59-72 行 | **switch 区分 GreenBtn / BlueBtn**，分别让 Slider 加减 0.1f；**这就是基类统一订阅后的写法——完全不用写 GetComponent 和 AddListener** |
| `override OnValueChanged(...)` | 第 74-77 行 | 占位，留给多选框逻辑 |
| `override ShowMe / HideMe` | 第 81-91 行 | **重写面板基类的显隐方法**（在面板本体中重写，而不是调用方），打印额外日志演示 |

### 6.4 UIPanelMgrTest — 调用方

| 部分 | 代码位置 | 作用 |
|------|----------|------|
| Update 中按 Z | 第 10-19 行 | 顶层挂载 PlayPanel，callback 里打印加载完成 |
| Update 中按 X | 第 21-24 行 | 隐藏并销毁 PlayPanel |

---

## 七、开发时踩过的坑 & 对应解释

### 坑 1：场景中始终有重复的 Canvas 和 Canvas (Clone)

**原因**：`ResLoadMgr.LoadRes<GameObject>()` 在基类中对于 `GameObject` 返回的是 **Instantiate 后的实例**（所以原预制体 Canvas 被实例化为 Canvas (Clone)），而如果你的场景里本来就手动放了一个 Canvas，调用 `ShowPanel` 时 `InitPanelRoot` 会先走 `GameObject.Find("Canvas")` 找到场景中已有的 Canvas，然后再调用 Resources 加载出来的（Clone）就是多余的。

**代码中怎么防的**：`InitPanelRoot` 里先用 `GameObject.Find(conName)` 搜场景中已存在的物体，只有找不到且 `trans == null` 时才走 Resources 加载。只要场景中已有 `Canvas`，就不会再实例化第二个。

> 另外 `ResLoadMgr` 内部对于 GameObject 会自动 Instantiate，所以加载出来的对象名字会带 `(Clone)`，代码里用 `entityPanel.name = conName;` 强制改回原名，避免后续依赖名字的脚本找不到。

### 坑 2：面板在场景中不显示（但 Hierarchy 中依旧存在）

**现象**：面板对象确实实例化了，层级也对，但 Scene / Game 视图都看不到。

**原因**：Canvas 预制体里**没有 Top、Bottom、Middle 三个子物体**，导致：

```csharp
botTrans = canvasTrans?.Find("Bottom") as RectTransform;  // ← 返回 null
topTrans = canvasTrans?.Find("Top") as RectTransform;     // ← 返回 null
midTrans = canvasTrans?.Find("Middle") as RectTransform;  // ← 返回 null
```

然后 `ShowPanel` 里根据层级选 `panelFather` 时拿到 null，`panel.transform.SetParent(null)` 就把面板挂到了根节点下（挂到根的 RectTransform 不会被 Canvas 渲染管线绘制，当然看不到）。

**解决**：Canvas 预制体里必须有三个名为 `Top`、`Middle`、`Bottom` 的空物体（RectTransform），作为面板的三个父节点。

### 坑 3：重写面板基类的显隐方法应该写在哪

**错误写法**：在 `UIPanelMgrTest`（调用方）里写 `panel.ShowMe(...)` 的专属逻辑。

**正确写法**：**在面板本体 PlayPanelTest 中重写基类的 ShowMe / HideMe**（见第 81-91 行）。调用方只需要 `ShowPanel("PlayPanel")`，具体面板显示时要做什么（动画 / 请求 / 重置）是面板自己的事，外部不用知道。

---

## 八、协作关系一览

```
玩家按 Z
   ↓
UIPanelMgrTest.Update
   ↓
UIPanelMgr.ShowPanel<PlayPanelTest>("PlayPanel", E_UILayer.Top, cb)
   ├─ 1. InitPanelRoot → Canvas 场景有就用，没有就 Res 加载 + DontDestroyOnLoad
   ├─ 2. ResLoadMgr.LoadResAsync("Panel/PlayPanel", panel => { ... })
   │       ↓ (异步完成后)
   │   ├─ panel.SetParent(topTrans)   // 顶层挂载
   │   ├─ panelDict["PlayPanel"] = PlayPanelTest实例
   │   ├─ localPosition / localScale / offsetMax/Min 归零
   │   ├─ callback?.Invoke(panel)     // 外部回调：打印「加载完成了喵」
   │   └─ panel.ShowMe("PlayPanel")   // 走 PlayPanelTest override
   │          ├─ base.ShowMe → 打印"已经显示PlayPanel"
   │          └─ 子类打印"法!显示了PlayPanel面板"
   │
   └─ PlayPanelTest.Awake（自动）
          ├─ AddConToDict<Button> → 扫到 GreenBtn、BlueBtn
          │     各自闭包：() => OnClick("GreenBtn") / OnClick("BlueBtn")
          └─ AddConToDict<Image / Text / Slider / Toggle>

玩家点击 GreenBtn
   ↓
OnClick("GreenBtn") (PlayPanelTest override)
   ├─ switch (btnName) case "GreenBtn"
   └─ Slider.value += 0.1f

玩家按 X
   ↓
UIPanelMgr.HidePanel("PlayPanel")
   ├─ panel.HideMe("PlayPanel")  → 子类 override 的 HideMe
   └─ Destroy(obj) + Remove("PlayPanel")

玩家在 GreenBtn 上按下（不等抬起）
   ↓
AddCustomEvent(GreenBtn, PointerDown, data => print(...))
   → 通过 EventTrigger 封装，等价于上篇 CustomBtnClick 的 4 步流程
```
