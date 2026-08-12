using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    //继承Mono的单例基类,肯定不需要私有化构造函数啊
   private static T _instance;

   public static T Instance
   {
       get
       {
           if (_instance == null)
           {
               _instance = FindObjectOfType<T>();
               if (_instance == null)
               {
                   GameObject obj = new GameObject(typeof(T).Name);
                   _instance = obj.AddComponent<T>();
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
       Debug.Log("单例基类启动");
   }
   
   protected virtual void OnDestroy()
   {
       if (_instance == this)
       {
           _instance = null;
       }
   }
}
