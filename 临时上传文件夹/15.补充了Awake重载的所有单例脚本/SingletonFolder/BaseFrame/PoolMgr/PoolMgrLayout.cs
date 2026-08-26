using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolMgrLayout : MonoSingletonBase<PoolMgrLayout>
{
   protected override void Awake()
   {
      base.Awake();
      print("哔哔哔...对象池模块(布局优化)");
   }
   
   //是否开启自动布局功能
   public static bool isOpenLayout = true;
   //这里直接将游戏物体栈Stack<GameObject>换成了PoolData类对象(封装思想)
   private Dictionary<string,PoolData> poolDic = new Dictionary<string, PoolData>();
   //poolObj 始终指代的是最高层级根节点
   private GameObject poolObj;
   //获取游戏物体
   public GameObject GetObj(string name)
   {
      GameObject obj;
      //注意依旧poolDic[name]是一整个栈,这里poolDic[name].Count检测value栈数量
      //poolDic[name].PopData()则是弹出value栈中的一个元素
      if (poolDic.ContainsKey(name) && poolDic[name].Count > 0)
      {
         //注意这里是调用了PoolData类中的PopData方法
         obj = poolDic[name].PopData();
         //这里物体激活和设置父对象为空已经在封装类PoolData中实现,不需要写了
      }
      else
      {
         obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
         obj.name = name;
      }
      return obj;
   }
//设置游戏物体
   public void SetObj(string name, GameObject obj)
   {
      //首先要检查最高层级节点是否已经创建(方便下面创建栈root并设置为其根节点)
      if (poolObj == null && isOpenLayout)
      {
         poolObj = new GameObject("Pool");
      }
      //检查是否已经有同名物体
      if (poolDic.ContainsKey(name))
      {
         //调用PoolData中的PushData方法(poolDic[name]是一个完整的poolData对象实例)
         poolDic[name].PushData(obj);
         //同样因为封装对象中已经写入,不需要写物体失活和设置父物体了
      }
      else
      {
         //只有这里会访问PoolData的构造函数
         //如果没有指定name层的栈,就触发构造函数
         //根节点名为obj.name并将root设为总root子物体
         poolDic.Add(name,new PoolData(poolObj, obj.name));
         poolDic[name].PushData(obj);
      }
   }
//这个Clear并没有真正销毁(可能内存泄露),还需要改进
   public void ClearPool()
   {
      poolDic.Clear();
   }
   //这里选用继承了Mono的单例基类,就不使用构造函数了
}
