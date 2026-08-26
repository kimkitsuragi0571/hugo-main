using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingletonBase<T> : MonoBehaviour where T: MonoBehaviour
{
    private static T _instance;
    //依旧Instance属性
    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject mgr = new GameObject(typeof(T).Name);
                    _instance = mgr.AddComponent<T>();
                    DontDestroyOnLoad(mgr);
                }
            }
            return _instance;
        }
    }
    //依旧虚Awake方法用于重写(静态了不就没法继承吗)
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

    protected virtual void OnDestroy()
    {
        if (_instance == null)
        {
            _instance = null;
        }
    }
}
