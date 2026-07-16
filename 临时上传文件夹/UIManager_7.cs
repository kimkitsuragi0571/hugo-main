using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager_7 : MonoBehaviour
{
   private static UIManager_7 _instance;

   public static UIManager_7 Instance
   {
       get
       {
          return _instance;
       }
   }

   private void Awake()
   {
       if (_instance == null)
       {
           _instance = this;
           DontDestroyOnLoad(this.gameObject);
       }
       else
       {
           Destroy(this.gameObject);
       }
   }
   
   public void DataInit()
   {
      Debug.Log("UIManager-继承了Mono的单例模式");
   }
}
