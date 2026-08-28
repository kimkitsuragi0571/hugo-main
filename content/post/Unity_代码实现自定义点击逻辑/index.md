+++
title = "Unity 代码实现自定义点击逻辑"
date = "2026-08-28T16:30:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "UGUI", "Button", "EventTrigger", "点击事件"]
+++

> Button.onClick 事件只能监听点击；
> 之前的如 ...Down、...Up 等自定义逻辑都只能通过面板拖拽实现。

Unity 的 `Button.onClick` 只能监听完整的「按下再抬起」一次点击，无法区分 `PointerDown`、`PointerUp`、`Drag`、`PointerEnter` 等更细粒度的交互。如果想在代码里绑定这些事件，需要借助 `EventTrigger` 组件——这是 UGUI 提供的统一事件入口，既支持 Inspector 拖拽，也支持用代码动态添加 Entry。

## 一、完整脚本

`CustomBtnClick` 继承自项目内的 `UIPanelBase`（一个 UI 面板基类，提供 `GetConFromDict<T>(name)` 从字典缓存中获取子控件）。如果你的项目没有这个基类，把 `GetConFromDict<Button>("GreenButton")` 换成 `transform.Find("GreenButton").GetComponent<Button>()` 即可。

```csharp
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
```

## 二、四个关键步骤

| 步骤 | 代码 | 说明 |
|------|------|------|
| 1. 添加组件 | `gameObject.AddComponent<EventTrigger>()` | 给目标物体（这里是 GreenButton 的 GameObject）挂上 `EventTrigger` |
| 2. 创建条目 | `new EventTrigger.Entry()` | 一个 Entry 对应一种事件类型，相当于 Inspector 里的「一行」 |
| 3. 指定类型 | `entry.eventID = EventTriggerType.Drag` | 可选 `PointerClick` / `PointerDown` / `PointerUp` / `Drag` / `PointerEnter` / `PointerExit` 等 |
| 4. 绑定回调 | `entry.callback.AddListener(OnDrag)` | 把方法挂到 callback 上，事件触发时调用 |

> **一个 Entry 只能绑定一种事件类型**。如果同一个按钮要同时监听 Down 和 Up，需要创建两个 Entry，分别设置 `eventID = PointerDown` 和 `eventID = PointerUp`，再分别 `AddListener`。

## 三、回调签名：BaseEventData 还是 PointerEventData

`EventTrigger.callback` 的签名是 `UnityAction<BaseEventData>`，所以绑定的方法**必须**接受 `BaseEventData` 参数：

```csharp
void OnDrag(BaseEventData data)
```

但拖拽 / 点击相关的交互数据（位置、增量、按键）都在 `PointerEventData` 里，所以需要先强转：

```csharp
PointerEventData pointerData = data as PointerEventData;
```

不同事件使用不同的 eventData 子类：

| 事件场景 | 实际类型 |
|----------|----------|
| 拖拽 / 点击 / 按下抬起 | `PointerEventData` |
| 下拉菜单选项变化 | `BaseEventData` 派生类 |
| 输入框提交 | `InputField.SubmitEvent`（参数为字符串） |

## 四、PointerEventData 常用字段

强转拿到 `PointerEventData` 后，下面几个字段是拖拽 / 点击处理时最常用的：

```csharp
// 当前鼠标/手指的屏幕坐标
Vector2 currentPos = pointerData.position; 

// 从上次 OnDrag 到这次的位移增量（最常用，做拖拽移动时直接加到物体 position）
Vector2 delta = pointerData.delta; 

// 按下的是哪个键：Left / Right / Middle
PointerEventData.InputButton button = pointerData.button;
```

其它常用字段（备查）：

| 字段 | 说明 |
|------|------|
| `pointerCurrentRaycast` | 当前光标命中的 RaycastResult（可拿到 hit 的 GameObject） |
| `pointerPressRaycast` | 按下瞬间的 RaycastResult |
| `pressPosition` / `pressEventCamera` | 按下时的屏幕坐标 / 事件相机 |
| `dragging` | 当前是否处于拖拽中 |
| `eligibleForClick` | 是否符合触发 click 条件（没移动太多 / 没松太快） |

## 五、注意事项

1. **onClick 与 EventTrigger 不冲突**：给 Button 添加 `EventTrigger` 监听 `PointerClick` 后，Inspector 里配置的 `Button.onClick` 依然有效，二者会同时触发——它们是 Unity 内部各自维护的事件，不是同一个。

2. **必须强转再使用**：直接访问 `data.position` 会编译错误，因为 `BaseEventData` 没有这些字段。务必 `as PointerEventData` 后判空再访问。

3. **多事件分别建 Entry**：一个 Entry 一种类型，别图省事在一个 Entry 上注册多种事件。

4. **EventTrigger 性能开销**：每个 Entry 在事件分发时都会被遍历检查，按钮不多时可放心用；如果场景里有大量 UI 都用 EventTrigger，建议改用接口实现（`IBeginDragHandler` / `IDragHandler` / `IPointerDownHandler` 等），更省内存也更直接。

5. **EventTriggerType 常用枚举**：
   - `PointerEnter` / `PointerExit` / `PointerDown` / `PointerUp` / `PointerClick`
   - `Drag` / `BeginDrag` / `EndDrag` / `Drop`
   - `Scroll` / `UpdateSelected` / `Select` / `Deselect` / `Move` / `Submit` / `Cancel`
