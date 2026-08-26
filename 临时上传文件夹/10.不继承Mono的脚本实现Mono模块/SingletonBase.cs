using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//泛型约束:T必须是类,且有无参构造函数
public class SingletonBase<T> where T : class, new()
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            //static属性,没有this实例,所以直接new T()
            //对比继承Mono的是_instance = this as T;
            if(_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }
    protected SingletonBase()
    {
        //依旧私有化构造函数
    }
}