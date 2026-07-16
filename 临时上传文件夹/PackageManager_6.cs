using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageManager_6 
{
   private static readonly Lazy<PackageManager_6> _instance = new Lazy<PackageManager_6>(() =>
   {
      return new PackageManager_6();
   });

   public static PackageManager_6 Instance
   {
       get
       {
           return _instance.Value;
       }
   }
   
   private PackageManager_6()
   {
       
   }

   public void DataInit()
   {
       Debug.Log("PackageManager-Lazy超级懒汉");
   }
}
