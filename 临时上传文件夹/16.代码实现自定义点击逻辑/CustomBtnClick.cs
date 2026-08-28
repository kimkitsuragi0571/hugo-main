public class CustomBtnClick : UIPanelBase
{
    void Start()
    {
    //如果直接写自定义点击逻辑-->用了这个Button.onClick自动失效
      //1.获取指定Button并为其添加EventTrigger组件
      EventTrigger trig = GetConFromDict<Button>("GreenButton").gameObject.AddComponent<EventTrigger>();
      //2.创建Entry监听条目(相当于个空的表格嘛)
      EventTrigger.Entry entryDrag = new EventTrigger.Entry();
      //3.指定事件类型,点击就是PointerClick,按下就是PointerDown
      //这里就指定Drag拖拽(entryDrag这一个对象只能指定这一种类型)
      entryDrag.eventID = EventTriggerType.Drag;
      //4.绑定回调函数,当发生拖拽的时候调用OnDrag函数
      entryDrag.callback.AddListener(OnDrag);
    }

     //OnDrag要求必须有BaseEventData类型参数
   void OnDrag(BaseEventData data)
   {
      // 1. 必须先强转为 PointerEventData (拖拽/点击 用的是 PointerEventData)
      //下拉菜单选择变化 用的是 SelectionEventData,输入框内容变化 用的是 InputField.SubmitEvent子类等等
      PointerEventData pointerData = data as PointerEventData;
      if (pointerData != null)
      {
         // 2. 获取当前鼠标/手指的屏幕坐标
         Vector2 currentPos = pointerData.position; 
         // 3. 获取从上次触发 OnDrag 到现在的位移增量 (最常用！)
         Vector2 delta = pointerData.delta; 
         // 4. 获取按下的是哪个键 (左键/右键/中键)
         PointerEventData.InputButton button = pointerData.button;
      }
   }
}
   
   
  