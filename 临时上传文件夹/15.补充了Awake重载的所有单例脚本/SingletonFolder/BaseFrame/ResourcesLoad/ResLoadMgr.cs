using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResLoadMgr : MonoSingletonBase<ResLoadMgr>
{
   protected override void Awake()
   {
      base.Awake();
      print("哔哔哔...资源加载模块");
   }
   //1.同步加载的方法
   public T LoadRes<T>(string resName) where T:Object
   {
      T res = Resources.Load<T>(resName);
      //判断如果是GameObject就直接实例化(不然封装这方法纯多余,不如直接加载)
      if (res is GameObject)
      {
         return GameObject.Instantiate(res);
      }
      return res;
   }
   //2.异步加载的方法
  
   //为什么要用callback函数?-->为了让调用函数可以返回值
   //调用函数在执行了这个Start语句就直接清空调用栈,协程本身独立于调用函数
   //调用函数本身是没法return协程的返回结果(所以这里直接写void了)
   public void LoadResAsync<T>(string resName, UnityAction<T> callback) where T : Object
   {
      //依旧是纯C#单例可以使用Mono模块来开启协程
      //MonoModuleMgr.Instance.StartCoroutine(IELoadResAsync<T>(resName));
      StartCoroutine(IELoadResAsync<T>(resName, callback));
   }

   private IEnumerator IELoadResAsync<T>(string resName, UnityAction<T> callback) where T : Object
   {
      ResourceRequest req = Resources.LoadAsync<T>(resName);
      yield return req;
      if (req.asset is GameObject)
      {
         //协程内部当然也是没法直接return值得
         //为了实现返回值,只能直接将其传入callback作为参数
         //就是相当于某种闭包了
         //当然现在可以直接用async/await实现了
        callback(GameObject.Instantiate(req.asset) as T);
      }
      else
      {
         callback(req.asset as T);
      }
   }
}
