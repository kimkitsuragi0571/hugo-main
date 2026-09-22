using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class B2_FileOperation : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!File.Exists(Application.persistentDataPath + "/MonsterData_Dragon.binary"))
            {
                print("创建二进制文件");
                FileStream fs = File.Create(Application.persistentDataPath + "/MonsterData_Dragon.binary");
                fs.Close();
            }
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            print("写入文件(先不使用文件流)");
            string path = Application.persistentDataPath + "/MonsterData_Dragon.binary";
            if (File.Exists(path))
            {
                print("直接写入指定byte数组");
                byte[] bytes = BitConverter.GetBytes(999);
                File.WriteAllBytes(path, bytes);
                print("直接写入string数组每一行");
                string[] strs = new string[] { "1", "2", "3" };
                File.WriteAllLines(path, strs);
                print("直接写入指定string");
                File.WriteAllText(path, "Hi");
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            print("读取文件(先不使用文件流)");
            string path = Application.persistentDataPath + "/MonsterData_Dragon.binary";
            print("直接读取文件到指定byte数组");
            byte[] bytes = File.ReadAllBytes(path);
            print("直接读取每一行到string数组");
            string[] strs = File.ReadAllLines(path);
        }
        //剩下还有复制替换文件啥的都懒得写了
    }
}
