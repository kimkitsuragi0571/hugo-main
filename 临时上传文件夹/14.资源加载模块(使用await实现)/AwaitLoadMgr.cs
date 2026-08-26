using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class AwaitLoadMgr : MonoBehaviour
{
   //之前的版本只能用callback传入参数作为协程的返回值
   //这里直接用await/async来实现
   //好处1.可以直接return返回值 2.逻辑线性不需要在协程和回调里面绕 3.可以用标准try/catch(老协程很难捕获异常)
   
   //加载方法里面写async,返回值改为Task类型
   public async Task<T> LoadResAsync<T>(string resName) where T : Object
   {
      ResourceRequest req = Resources.LoadAsync<T>(resName);
      //使用await等待req加载完毕
      //await req;老版本会报错,AsyncOperation并没有实现GetAwaiter()方法
      //哎呀2022版太老了还不内置AsTask方法,狗史,需要自己手写个拓展类了
      await req.AsTask(); 
      if (req.asset is GameObject)
      {
         //这里可以直接返回加载的对象
         //泛型约束Object,可以返回任何继承自 UnityEngine.Object 的类型(Sprite,AudioClip啥的都可以)
         return GameObject.Instantiate(req.asset) as T;
      }
      return req.asset as T;
   }
}

//给ResourceRequest对象(也就是req)实现的拓展方法
public static class AsyncExtensions
{
   public static Task<UnityEngine.Object> AsTask(this ResourceRequest request)
   {
      // 创建一个新的 TaskCompletionSource (TCS)
      // TCS 是连接“旧式异步”和“现代 Task”的桥梁
      var tcs = new TaskCompletionSource<UnityEngine.Object>();
      // 定义一个局部函数，用于处理加载完成的事件
      void OnLoaded(AsyncOperation op)
      {
         // 当加载完成时，设置 Task 的结果
         tcs.SetResult(request.asset);
         // 记得移除监听，防止内存泄漏
         request.completed -= OnLoaded;
      }
      // 注册完成事件
      request.completed += OnLoaded;
      // 返回这个 Task 对象
      return tcs.Task;
   }
}