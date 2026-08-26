using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoModuleBase<T> : MonoSingletonBase<T> where T : MonoBehaviour
{
   //首先声明基于UnityAction的事件
   private event UnityAction updateEvent;

   protected override void Awake()
   {
      base.Awake();
      Debug.Log("Mono模块基类启动(负责真正的统一Update)");
   }
//然后让所有的函数都通过UnityAction.Invoke统一执行
//而UnityAction.Invoke又是统一在模块基类的Update里面执行
   private void Update()
   {
      updateEvent?.Invoke();
   }
//后面写TrickMgr的时候还是要外部调用的,所以这里public
   public void AddUpdateEvent(UnityAction action)
   {
      updateEvent += action;
   }
   
   public void RemoveUpdateEvent(UnityAction action)
   {
      updateEvent -= action;
   }
   // 确保销毁时清理事件，防止内存泄漏
   protected override void OnDestroy()
   {
      updateEvent = null; 
      base.OnDestroy();
   }
}
