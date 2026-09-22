using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class B3_FileStream : MonoBehaviour
{   
    string path;
    void Start()
    {
        path = Application.persistentDataPath + "/MonsterData_Dragon.binary";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //创建文件不一样,但是一般就填个path即可
            print("打开文件,并获取其文件流");
            using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (fs.CanRead)
                {
                    print(fs.Length);
                }
                //每次记得把缓存写入,防止文件丢失
                print("强制将内存缓冲区中的数据立即写入硬盘");
                fs.Flush();
                fs.Dispose();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            print("通过字节流写入文件(固定长度的字节数组)");
            using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                byte[] bytesInt = BitConverter.GetBytes(998);
                //部分写入(固定长度的变量)
                fs.Write(bytesInt, 0, bytesInt.Length);
                //全部写入,只能用File调用
                //File.WriteAllBytes(path, String);
                //File.WriteAllText(path, "String");
                //File.WriteAllBytes(path, bytesString);
            }
        
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            print("通过字节流写入文件(不定长度的字符串)");
            //写入字符串长度主要是方便读取时使用
            using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                byte[] bytesString = Encoding.UTF8.GetBytes("FuckChineseGoverment");
                int len = bytesString.Length;
                //注意先把长度写入
                fs.Write(BitConverter.GetBytes(len), 0,4);
                //写入本体
                fs.Write(bytesString, 0, len);
                fs.Flush();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            print("通过字节流读取文件(固定长度的字节数组)");
            using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                //count只是个检测条件,真正存储数据的是bytesInt
                byte[] bytesInt = new byte[4];
                //读取fs的前4位(不一定够4B),放入bytesInt的前4位,返回count实际返回的字节数
                int count = fs.Read(bytesInt, 0, 4);
                if (count == 4)
                {
                    //只有刚好4B,才能转化为Int类型
                    int value = BitConverter.ToInt32(bytesInt, 0);
                }
               
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            print("通过字节流读取文件(不定长度的字符串)");
            using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                //先和定长一样的读取长度
                byte[] bytesLen = new byte[4];
                int count = fs.Read(bytesLen, 0, 4);
                int len = 0;
                if (count == 4)
                {
                   len = BitConverter.ToInt32(bytesLen, 0);
                }

                //然后根据长度创建字节数组
                byte[] bytesString = new byte[len];
                //最后读取
                fs.Read(bytesString, 0, len);
                //还原为string
                string result = Encoding.UTF8.GetString(bytesString);
            }
        }
    }
}
