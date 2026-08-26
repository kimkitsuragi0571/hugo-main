using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//首先是一个简单的不继承Mono的统一Update实现
//注意这里继承的是纯C#单例基类哈
public class TrickMgr : SingletonBase<TrickMgr>
{
    public TrickMgr()
    {
        //这里为了继承Singleton基类,只能public构造函数了
        Debug.Log("哔哔哔...纯C#单例实现帧更新与协程");
    }
    //声明协程执行语句(而不是协程声明) 的句柄cor(不是ie)
    private Coroutine _cor;

    //开启帧更新: 方法订阅 + 协程订阅
    //不是继承单例而是调用其中的方法哈
    //将Speak方法加入事件订阅,调用协程(每次调用方法都会订阅,多次执行)
    public void StartUp()
    {
        MonoModuleMgr.Instance.AddUpdateEvent(Speak);
        //传入声明的协程,返回协程执行句柄
        _cor = MonoModuleMgr.Instance.StartCor(Cor(1));
    }
    
    //停止帧更新:取消方法订阅+停止协程运行
    public void StopUp()
    {
        MonoModuleMgr.Instance.RemoveUpdateEvent(Speak);
        if (_cor != null)
        {
            //停止协程,然后移除句柄引用
            MonoModuleMgr.Instance.StopCor(_cor);
            _cor = null;
        }
    }

    private void Speak()
    {
        Debug.Log("传入帧更新的方法");
    }

    //这里协程订阅只是因为不继承Mono用不了协程而已hhh
    private IEnumerator Cor(int val)
    {
        while (val < 5)
        {
            yield return new WaitForSeconds(1f);
            val++;
            Debug.Log("传入Mono基类执行的协程");
        }
    }
}
