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
   
   // 用于开启/关闭协程的方法
   //TrickMgr没有继承Mono,没法调用协程(但是仍然可以声明协程),只能传入由基类执行
   //通过ie = Cor(),startCoroutine(ie)  所以这里传入协程的句柄
   public Coroutine StartCor(IEnumerator ie)
   {
      //返回协程执行语句的句柄cor = StartCoroutine(ie)
      return StartCoroutine(ie);
   }

   //这里传入协程执行语句的句柄StopCoroutine(cor)
   public void StopCor(Coroutine cor)
   {
      if (cor != null) StopCoroutine(cor);
   }
}
