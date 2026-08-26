using System.Collections.Generic;
using UnityEngine;

//PoolData封装,是把栈+每个栈的根对象封装为一块
public class PoolData 
{
    //用于存储每个栈中的对象
    private Stack<GameObject> dataStack = new Stack<GameObject>();
    //每个栈的根对象
    private GameObject rootObj;
    //属性用于检查每个栈中是否还有对象
    public int Count
    {
        get
        {
            return dataStack.Count;
        }
    }
    //弹出数据的方法:弹出栈,激活对象,取消父子关系
    public GameObject PopData()
    {
        GameObject obj = dataStack.Pop();
        obj.SetActive(true);
        //如果开启布局功能
        if (PoolMgrLayout.isOpenLayout)
        {
            //每个栈对应仅一个Root根物体,取出的时候自然设置物体父物体为空
            obj.transform.SetParent(null);
        }
        return obj;
    }
    //压入数据的方法:失活物体,设置父类,压入栈中
    public void PushData(GameObject obj)
    {
        obj.SetActive(false);
        //压入栈同样将这个栈对应的根节点设置为该物体父节点,然后压入栈
        obj.transform.SetParent(rootObj.transform);
        dataStack.Push(obj);
    }
    //构造函数:这个构造函数就是专门给每个栈创建对应的根节点的
    //前面的rootObj除了设置父子关系,就是用来在这里接收创建的根节点
    public PoolData(GameObject root, string name)
    {
        if (PoolMgrLayout.isOpenLayout)
        {
            //比如Bullet物体,这里就创建个名叫Bullet的根节点,并且设置为总根节点的子物体
            rootObj = new GameObject(name);
            rootObj.transform.SetParent(root.transform);
        }
    }
}
