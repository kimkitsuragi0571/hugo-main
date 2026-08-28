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
