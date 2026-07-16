using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton_2<T> : MonoBehaviour where T: MonoBehaviour
{
   private static T _instance;

   public static T Instance
   {
       get
       {
           if (_instance == null)
           {
               _instance = FindObjectOfType<T>();
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
}
