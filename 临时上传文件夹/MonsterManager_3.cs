using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager_3
{
   private static MonsterManager_3 _instance;
   private readonly static Object loc = new Object();

   public static MonsterManager_3 Instance
   {
       get
       {
           if (_instance == null)
           {
               lock (loc)
               {
                   if (_instance == null)
                   {
                       _instance = new MonsterManager_3();
                   }
               }
           }
           return _instance;
       }
   }

   private MonsterManager_3()
   {
       
   }

   public void DataInit()
   {
       Debug.Log("MonsterManager-线程安全");
   }
}
