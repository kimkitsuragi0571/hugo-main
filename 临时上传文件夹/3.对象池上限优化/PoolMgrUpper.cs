using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolMgrUpper : BaseManager<PoolMgrUpper>
{
   public static bool isOpenLayout = true;
   private Dictionary<string,PoolDataUpper> poolDic = new Dictionary<string, PoolDataUpper>();
   private GameObject poolObj;
   //获取物体时新增最大数量
   public GameObject GetObj(string name,int max = 50)
   {
      GameObject obj;
      //防止bug,把这段从SetObj中移动到GetObj中
      if (poolObj == null && isOpenLayout)
      {
         poolObj = new GameObject("Pool");
      }
      
      //新增判断 有栈没对象,使用中对象没有超上限
      //if (poolDic.ContainsKey(name) && poolDic[name].Count > 0) 旧版:有要取的物体对应栈+栈中有对象直接用,没有就先创建
      //新版:如果没有取出物体对应栈 或 备用栈空且使用中物体未满->先创建新物体
      //UsedList满则不创建新物体,而是直接从旧的里面取,UsedList未满则直接创建新物体
      if (!poolDic.ContainsKey(name) || (poolDic[name].Count == 0 && poolDic[name].UsedCount < max)) 
      {
         obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
         obj.name = name;
         //进一步判断,如果是没有对应栈
         if (!poolDic.ContainsKey(name))
         {
            //调用PoolDataUpper构造函数,执行之前操作并把新物体送入usedList
            poolDic.Add(name, new PoolDataUpper(poolObj, name,obj));
         }
         else
         {
            //有对应栈就不需要执行创建了,直接新物体送入usedList
            poolDic[name].PushUsedList(obj);
         }
      }
      //不需要创建新物体的逻辑都在这里
      else
      {
         obj = poolDic[name].PopData();
      }
      return obj;
   }
   //SetObj已经不需要写逻辑了
   public void SetObj(string name, GameObject obj)
   {
      //第一次获取对象
   }
   
   public void ClearPool()
   {
      //这里只是清空字典引用,导致严重内存泄露,虽然我懒得管了
      poolDic.Clear();
   }
}
