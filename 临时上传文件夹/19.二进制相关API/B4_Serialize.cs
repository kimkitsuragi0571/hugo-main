using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class B4_Serialize : MonoBehaviour
{
    string path;
    void Start()
    {
        path = Application.persistentDataPath + "/MonsterData_Dragon.binary";
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Student stu = new Student();
            //先攒后写:内存消耗峰值高
            print("使用MemoryStream内存流对象序列化");
            using (MemoryStream ms = new MemoryStream())
            {
                //二进制格式化器,负责将C#对象转化为字节流
                BinaryFormatter bf = new BinaryFormatter();
                //stu序列化到ms内存流
                bf.Serialize(ms, stu);
                //byte[] bytes = ms.GetBuffer();直接返回整个缓冲区,可能有无意义部分
                //ToArray只返回有效数据部分
                byte[] bytes = ms.ToArray();
                //BinaryFormatter序列化-->写入MemoryStream内存流并转为Byte对象--> File.WriteAllBytes写入路径文件
                File.WriteAllBytes(path, bytes);
                //注意MemoryStream并不需要ms.Flush,本来就写在RAM上不需要缓冲区攒够了才写
            }
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            Student stu = new Student();
            //边转边写:可能有损坏风险
            print("使用FileStream文件流对象序列化");
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                BinaryFormatter bf = new BinaryFormatter();
                //序列化直接写入文件流而非内存流
                bf.Serialize(fs, stu);
                fs.Flush();
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
           byte[] bytes = File.ReadAllBytes(path);
           print("使用MemoryStream内存流对象反序列化");
           //创建内存流对象的时候就传入bytes数组,相当于省略了ms.Write()这一步
           using (MemoryStream ms = new MemoryStream(bytes))
           {
               BinaryFormatter bf = new BinaryFormatter();
               //bf负责把ms里面的bytes数组翻译出来
               Student stu = bf.Deserialize(ms) as Student;
           }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            print("使用FileStream文件流对象反序列化");
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                BinaryFormatter bf = new BinaryFormatter();
                Student stu = bf.Deserialize(fs) as Student;
            }
        }
    }
}

[System.Serializable]
public class Student
{
    public string name;
    public int age;
}
