using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager_4 
{
   //静态内部类中写instance,并且只有第一次被访问时实例化仅一次
   private static class Nested
   {
      internal static readonly WeaponManager_4 _instance = new WeaponManager_4();
   }
   
   public static WeaponManager_4 Instance
   {
      get
      {
         return Nested._instance;
      }
   }

   private WeaponManager_4()
   {
      
   }

   public void DataInit()
   {
      Debug.Log("静态内部类单例");
   }
}
