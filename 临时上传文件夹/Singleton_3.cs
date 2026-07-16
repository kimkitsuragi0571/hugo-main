using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton_3<T> : MonoBehaviour where T:MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                //手动寻找场景中脚本
                _instance = FindObjectOfType<T>();
                //找不到就创建
                if (_instance == null)
                {
                    //直接用继承的Manager来命名
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                    //脚本是固定在obj物体上,所以保留obj
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static void Clear()
    {
        _instance = null;
    }
}
