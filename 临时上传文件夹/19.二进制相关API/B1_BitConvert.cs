using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class B1_BitConvert : MonoBehaviour
{
   void Start()
   {
     
   }

   void Update()
   {
      if (Input.GetKey(KeyCode.A))
      {
         print("普通变量转化为byte数组");
         byte[] bytes = BitConverter.GetBytes(999);
         foreach (byte b in bytes)
         {
            print(b);
         }
      }
      else if (Input.GetKey(KeyCode.S))
      {
         print("byte数组转化为普通变量");
         byte[] bytes = BitConverter.GetBytes(999);
         int max = BitConverter.ToInt32(bytes, 0);
         print(max);
      }
      else if (Input.GetKey(KeyCode.D))
      {
         print("string类型转化为byte数组");
         byte[] bytes = Encoding.UTF8.GetBytes("Hi");
      }
      else if (Input.GetKey(KeyCode.F))
      {
         print("byte数组转化为string类型");
         byte[] bytes = Encoding.UTF8.GetBytes("Hi");
         string str = Encoding.UTF8.GetString(bytes);
         print(str);
         print("byte数组转化为string类型(局部转化)");
         string strLocal = Encoding.UTF8.GetString(bytes,1,bytes.Length-1);
         print(strLocal);
      }
   }
}
