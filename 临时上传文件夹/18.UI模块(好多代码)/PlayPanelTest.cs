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
