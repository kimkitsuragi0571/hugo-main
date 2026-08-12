using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolMgr : BaseSingleton<PoolMgr>
{
   //注意这里值是Stack<GameObject>
   private Dictionary<string,Stack<GameObject>> poolDic = new Dictionary<string,Stack<GameObject>>();
   //取出物体的方法
   public GameObject GetObj(string name)
   {
      GameObject obj;
      if (poolDic.ContainsKey(name) && poolDic[name].Count > 0)
      {
         //字典只是更改引用,并不会真正修改游戏物体内存位置
         obj = poolDic[name].Pop();
         obj.SetActive(true);
      }
      else
      {
         obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
         obj.name = name;
      }
      return obj;
   }
   //放入东西的方法
   public void SetObj(string name, GameObject obj)
   {
      obj.SetActive(false);
      if (poolDic.ContainsKey(name))
      {
         //这里是在 poolDic[name]对应栈中取出对象
         poolDic[name].Push(obj);
      }
      else
      {
         //需要添加的是新栈,不仅仅是游戏物体
         poolDic.Add(name, new Stack<GameObject>());
         //这里是在 poolDic[name]对应栈中压入对象
         poolDic[name].Push(obj);
      }
   }

   public void Clear()
   {
      poolDic.Clear();
   }

   
}
