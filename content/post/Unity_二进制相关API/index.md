+++
title = "Unity 二进制相关 API（BitConvert / File / FileStream / BinaryFormatter）"
date = "2026-09-01T15:55:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "二进制", "文件操作", "FileStream", "序列化", "BinaryFormatter"]
+++

游戏开发里，玩家存档、配置表、资源打包等场景都会涉及「把 C# 对象 / 普通变量写成磁盘上的二进制文件」或反过来。本文按**由浅入深**的顺序，依次演示四组 API：

1. **BitConverter / Encoding** — 变量与 `byte[]` 互转（这是一切二进制操作的基石）
2. **File 静态类** — 一次性读写整个文件（简单、常用）
3. **FileStream 文件流** — 逐块读写（控制粒度更细，适合大数据 / 不定长数据）
4. **BinaryFormatter + MemoryStream / FileStream** — 把整个 C# 对象序列化 / 反序列化

所有测试代码挂载到场景任意物体，按对应按键即可触发。

---

## 一、B1：变量与 byte[] 互转（BitConverter / Encoding）

```csharp
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
```

### 四个按键速查

| 按键 | 代码 | 说明 |
|------|------|------|
| **A**（按住） | `BitConverter.GetBytes(999)` | `int → byte[]`，int 占 4 字节，打印 4 个 byte |
| **S**（按住） | `BitConverter.ToInt32(bytes, 0)` | `byte[] → int`，第 2 个参数是**起始偏移**（从第几个字节开始解析） |
| **D**（按住） | `Encoding.UTF8.GetBytes("Hi")` | `string → byte[]`，编码格式必须选好（UTF8 / Unicode / ASCII） |
| **F**（按住） | `Encoding.UTF8.GetString(bytes, 1, bytes.Length-1)` | `byte[] → string`，支持局部解析（起始索引 + 长度） |

> 常见变体：`GetBytes(bool)` 返回 1 字节，`GetBytes(float)` 返回 4 字节，`GetBytes(long)` 返回 8 字节。`ToXXX(bytes, startIndex)` 家族与 GetBytes 一一对应。

---

## 二、B2：File 静态类 — 一次性读写整个文件

`File` 类下的 `WriteAllBytes / WriteAllLines / WriteAllText / ReadAllBytes / ReadAllLines` 都是**一步到位**：打开 → 全部写入 / 全部读取 → 自动关闭。适合小文件 / 简单场景。

```csharp
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
```

### 三个按键速查

| 按键 | 代码 | 说明 |
|------|------|------|
| **Q** | `File.Exists + File.Create` | 不存在就创建（`.Close()` 别忘了，否则下一次打开会报占用） |
| **W** | `WriteAllBytes / WriteAllLines / WriteAllText` | 三种一次性写入；每次调用都会**覆盖原内容**，三者不是追加 |
| **E** | `ReadAllBytes / ReadAllLines` | 一次性全部读取；大文件（几十 MB 以上）会瞬间占用同等内存，改用 FileStream |

> 路径统一用 `Application.persistentDataPath`（不同平台自动映射到可读写目录，如 Windows 下 `C:/Users/<用户名>/AppData/LocalLow/<公司>/<产品>`）。不要用 `Application.dataPath`（打包后只读）。

---

## 三、B3：FileStream 文件流 — 逐块读写

`File` 静态类是「一次性干完」。如果要先写一个 int 再写一段字符串、或者读一半处理一半，就要用 `FileStream`。它的核心模型是「流指针」：当前读 / 写到第几个字节，读写完自动往后移。

```csharp
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
                byte[] bytesString = Encoding.UTF8.GetBytes("Hello Unity Binary");
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
```

### 五个按键速查

| 按键 | 核心代码 | 说明 |
|------|----------|------|
| **1** | `File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite)` | 打开 / 创建文件流；`using` 出作用域自动 Dispose；`fs.Flush()` 强制把缓冲区刷进磁盘防掉电丢失 |
| **2** | `fs.Write(bytesInt, 0, bytesInt.Length)` | 固定长度写入（int 正好 4B）；Write 三参数：字节数组、数组起始偏移、写入字节数 |
| **3** | 先 `Write(len, 4B)` 再 `Write(body, len B)` | **不定长字符串的写入协议**：把字符串长度（int 4B）先写在前面，后面跟着真正的字符串字节。读取时先读长度再读本体 |
| **4** | `fs.Read(bytesInt, 0, 4)` → `ToInt32` | 固定长度读取；Read 返回值 count 是「实际读到了多少字节」，文件末尾可能不足 4B，必须判 count 再转 |
| **5** | 先 `Read(4B) → len`，再 `Read(len B) → body` | 不定长字符串读取，与按键 3 的写入协议对称，最后 `Encoding.UTF8.GetString` 还原 |

### 不定长数据的「长度前缀」协议

这是手写二进制存储时最常见的套路：

```
写入: [4字节: len=18] [18字节: "Hello Unity Binary"]
读取: 先读4字节 → 拿到 18 → 再读 18 字节 → 得到字符串
```

如果没有长度前缀，读取方根本不知道字符串在哪结束（不像 C 风格以 `\0` 结尾那样有约定）。同理存储数组、字典也是先写元素个数，再挨个写每个元素。

---

## 四、B4：BinaryFormatter 序列化 / 反序列化

手写 BitConverter + FileStream 写十几个字段很累。`BinaryFormatter` 可以**直接把标记了 `[Serializable]` 的 C# 对象整个变成字节流**，反序列化时再还原回来。内部会处理所有字段、引用、嵌套结构。

```csharp
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
```

### 四个按键速查

| 按键 | 流程 | 特点 |
|------|------|------|
| **A** | `MemoryStream` 序列化 → `ToArray()` → `WriteAllBytes` | **先攒后写**：对象先全部转成内存中的字节数组，再一次性写文件。内存峰值高（两份字节），但中间出错不会弄脏磁盘文件 |
| **S** | `bf.Serialize(fs, stu)` 直接写 `FileStream` | **边转边写**：转一点写一点，内存占用小。但中途异常会在磁盘上留下半截损坏的文件 |
| **D** | `ReadAllBytes → new MemoryStream(bytes) → bf.Deserialize` | 内存流反序列化，对应按键 A |
| **F** | `FileStream → bf.Deserialize(fs)` | 文件流反序列化，对应按键 S |

### 使用条件 & 注意

1. **类必须加 `[System.Serializable]`**：字段也得是可序列化的（`string / int / float / 嵌套 [Serializable] 类`）。`[NonSerialized]` 可以标记某个字段「不存」。
2. **MemoryStream 不用 Flush**：写入的目的地本来就是 RAM，没有「操作系统缓冲区 → 磁盘」这一步，不存在丢数据风险。
3. **ms.GetBuffer() vs ms.ToArray()**：GetBuffer 返回整个内部缓冲区（可能后面跟着没用的 0 字节），ToArray 只返回有效数据部分，一般选后者。
4. **安全性**：BinaryFormatter 的输出不是自描述 JSON，改一个字节就会反序列化异常；且存在反序列化漏洞攻击风险，**不要用 BinaryFormatter 存网络传输的数据或不可信来源的文件**（.NET 5+ 已标 Obsolete）。Unity 本地存档、单机场景放心用。

---

## 五、四种层级对照

| 层级 | API | 适用场景 | 记忆要点 |
|------|-----|----------|----------|
| 字节级 | `BitConverter.GetBytes / ToXXX` + `Encoding.UTF8.GetBytes / GetString` | 单个变量转 byte[] | int 4B / string 用 Encoding / 起始偏移 |
| 一次性文件级 | `File.WriteAllBytes / ReadAllBytes` 等 | 小文件、简单配置、代码不足 10 行的场合 | 路径用 persistentDataPath，每次写入是**覆盖** |
| 流式文件级 | `FileStream.Write / Read` + 「长度前缀」协议 | 大数据、不定长字符串 / 数组、自定义协议 | 读要判断实际 count；写不定长先写 len；用 `using` 自动释放；记得 `Flush` |
| 对象级 | `BinaryFormatter.Serialize / Deserialize` + `MemoryStream` / `FileStream` | 直接存整个对象（存档）；类型安全写代码最省事 | 类要加 `[Serializable]`；边转边写 vs 先攒后写；不要用在网络 / 不信任数据 |
