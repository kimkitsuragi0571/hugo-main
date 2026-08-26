using System.Collections.Generic;
using UnityEngine;

public class PoolDataUpper 
{
    //原来只记录栈中对象(也就是没有被使用的对象)
    private Stack<GameObject> dataStack = new Stack<GameObject>();
    //现在新增List记录使用中对象(便于随机访问,记录使用的先后)
    private List<GameObject> usedList = new List<GameObject>(); 
    private GameObject rootObj;
    
    public int Count
    { //原来只是用Count属性检测栈中对象数量
        get => dataStack.Count;
    }
    public int UsedCount
    {//现在新增属性用于检测正在使用中的物体数量
        get => usedList.Count;
    }
    
    public GameObject PopData()
    {
        GameObject obj;
        //修改版的PoolMgr不再判断Count,交给PoolData管理
        if (Count > 0)
        {
            //栈中尚有余量,直接取出就行,并添加到使用列表
            obj = dataStack.Pop();
            usedList.Add(obj);
        }
        else
        {
            //栈中为空则没有能取出的备用物体,从正在使用物体中最老的开始取出
            obj = usedList[0]; 
            //只是复制到obj还不够,需要把usedList首位本体删除,然后obj添加到List末尾
            usedList.RemoveAt(0); 
            usedList.Add(obj);    
        }
        //如果是Count==0的情况,这行就是冗余的,不过也算保证了代码的统一吧
        obj.SetActive(true);
        if (PoolMgrLayout.isOpenLayout)
        {
            obj.transform.SetParent(null);
        }
        return obj;
    }
    
    public void PushData(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(rootObj.transform);
        dataStack.Push(obj);
        //新增从usedList中移除
        usedList.Remove(obj);
    }
    //构造函数新增usedObj参数,以及调用对象压入使用中容器函数
    public PoolDataUpper(GameObject root, string name, GameObject usedObj)
    {
        if (PoolMgrLayout.isOpenLayout)
        {
            rootObj = new GameObject(name);
            rootObj.transform.SetParent(root.transform);
        }
        PushUsedList(usedObj);
    }
    //GetObj取物体->没有对应栈->首次创建栈完毕直接加入使用列表
    public void PushUsedList(GameObject obj) 
    {
        usedList.Add(obj);
    }
}
