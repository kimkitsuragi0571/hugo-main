+++
title = "Unity对象池管理器实现"
date = "2026-05-03T10:30:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "对象池"]
+++

以下是一个完整的Unity对象池管理器实现，支持布局优化和数量限制功能：

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolData
{
    //之前记录的是没有使用的对象
    private Stack<GameObject> dataStack= new Stack<GameObject>();
    //现在新增记录使用中的对象,为了可以从中间移除对象(随机访问),用List
    private List<GameObject> usedList = new List<GameObject>();
    private GameObject rootObj;

    public int CountElem
    {
        get => dataStack.Count;
    }

    //新增属性
    public int usedCountElem
    {
        get => usedList.Count;
    }

    public GameObject PopFunc()
    {
        //修改obj声明,新增判断
        GameObject obj;
        if (CountElem > 0)
        {
            //从没用的容器中取出
            obj = dataStack.Pop();
            //箱子要使用了,应该用正在使用的容器将其记录
            usedList.Add(obj);
        }
        else
        {
            //压入List,索引==0的肯定是使用时间最长的
            obj = usedList[0];
            //取出后还要移除
            usedList.RemoveAt(0);
            //因为还要拿出去用,所以应该将其又记录到使用中的容器里面,添加到尾部表示最新的
            usedList.Add(obj);
        }
        obj.SetActive(true);
        if (PoolMgr.isOpenLayout)
        {
            obj.transform.SetParent(null);
        }
        return obj;
    }

    public void PushFunc(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(rootObj.transform);
        dataStack.Push(obj);
    }

    //新增usdObj参数,以前是压入抽屉的时候才创建抽屉,现在获取对象时就得创建抽屉
    public PoolData(GameObject root,string name,GameObject usedObj)
    {
        if (PoolMgr.isOpenLayout)
        {
            rootObj = new GameObject(name);
            if (PoolMgr.isOpenLayout)
            {
                rootObj.transform.SetParent(root.transform);
            }
        }
        //创建抽屉时候外部肯定动态创建对象
        //我们应该将其记录到使用中的对象容器中
        PushUsedList(usedObj);
    }

    //将对象压入到使用的容器中记录
    public void PushUsedList(GameObject obj)
    {
        usedList.Add(obj);
    }
}

public class PoolMgr : BaseManager<PoolMgr>
{
    public static bool isOpenLayout = true;
    private Dictionary<string,PoolData> poolDic = new Dictionary<string,PoolData>();
    private GameObject poolObj;

    //新增最大数量属性
    public GameObject GetObj(string name,int maxNum = 50)
    {
        GameObject obj;
        //从SetObj移动到这里防止Bug
        if (poolObj == null&&isOpenLayout)
        {
            poolObj = new ("Pool");
        }

        //加入数量上限后的逻辑判断
        //没有抽屉时
        //这里直接把 有抽屉但是没对象,使用中对象没有超上限的情况融合进来
        if (!poolDic.ContainsKey(name) ||(poolDic[name].CountElem == 0 && poolDic[name].usedCountElem < maxNum))
        {
            //动态创建对象
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            obj.name = name;

            //创建抽屉,新增当没有抽屉时候才判断
            if (!poolDic.ContainsKey(name))
            {
                poolDic.Add(name,new PoolData(poolObj,name,obj));
            }
            //有抽屉的情况
            else
            {
                //实例化出来的对象,需要记录到使用中的对象容器中
                poolDic[name].PushUsedList(obj);
            }
        }
        //如果抽屉里有对象,或者使用中对象容量超上限,直接取出来用
        //else if (poolDic[name].CountElem > 0 || poolDic[name].usedCountElem >= maxNum)
        else
        {
            obj = poolDic[name].PopFunc();
        }

        //加入上限后就不需要这个了
        // if (poolDic.ContainsKey(name) && poolDic[name].CountElem > 0){
        //     obj = poolDic[name].PopFunc();
        // }
        // else{
        //     obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
        //     obj.name = name;
        // }
        return obj;
    }

    public void SetObj(string name, GameObject obj)
    {
        //这段移动到GetObj中,为了避免布局优化功能开启时,Pool根对象创建,会报空
        // if (poolObj == null&&isOpenLayout){
        //     poolObj = new ("Pool");
        // }

        //没有抽屉需要提前创建,但是现在不需要了,因为我们第一次获取对象的时候已经有抽屉了
        // if (poolDic.ContainsKey(obj.name)){
        //     poolDic[obj.name].PushFunc(obj);
        // }
        // else{
        //     poolDic.Add(obj.name, new PoolData(poolObj,obj.name));
        //     poolDic[obj.name].PushFunc(obj);
        // }
    }

    public void ClearPool()
    {
        poolDic.Clear();
    }

    public PoolMgr()
    {

    }
}
```

## 功能特点

### 对象池管理
- 使用 `Stack<GameObject>` 存储未使用的对象
- 使用 `List<GameObject>` 记录使用中的对象，支持随机访问

### 布局优化
- `isOpenLayout` 开关控制是否启用布局优化
- 启用时自动整理对象层级关系

### 数量限制
- 支持设置最大对象数量 `maxNum`
- 超过上限时自动复用最旧的对象

### 使用方法

```csharp
// 获取对象
GameObject bullet = PoolMgr.Instance.GetObj("Bullet", 50);

// 归还对象
PoolMgr.Instance.SetObj("Bullet", bullet);

// 清空对象池
PoolMgr.Instance.ClearPool();
```
