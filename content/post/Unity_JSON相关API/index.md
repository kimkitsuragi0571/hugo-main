+++
title = "Unity JSON 相关 API（JsonUtility / LitJson）"
date = "2026-09-01T15:55:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "JSON", "JsonUtility", "LitJson", "序列化", "存档"]
+++

JSON 是游戏存档、配置表、网络协议的通用格式。Unity 里常见的序列化方案有两套：**Unity 内置的 JsonUtility**（轻量、无第三方依赖）和 **LitJson**（更灵活，支持字典 / 顶层数组 / 直接从路径读取文件）。本文用一份 Lurker 玩家数据和 Weapon 武器列表，对比两者的用法、优缺点和坑点。

---

## 一、完整脚本

```csharp
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;

public class J1_JsonReadAndWrite : MonoBehaviour
{
   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            print("写入特定字符串到Json文件中(但是这样也没有{}等符号,有点蠢)");
            File.WriteAllText
            (
                Application.persistentDataPath + "/" + "PlayerData_Valira" + ".json",
                "\"PlayerName\":\"Valira\""
            );
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            print("读取Json文件字符串");
            print(File.ReadAllText(Application.persistentDataPath + "/" + "PlayerData_Valira" + ".json"));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            print("将对象JsonUtility序列化为string,然后写入文件");
            Lurker valira = new Lurker();
            string val = JsonUtility.ToJson(valira);
            File.WriteAllText(Application.persistentDataPath + "/" + "PlayerData_Valira" + ".json", val);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            print("读取Json文件中string,转化为对象");
            string val = File.ReadAllText(Application.persistentDataPath + "/" + "PlayerData_Valira" + ".json");
            Lurker valira = JsonUtility.FromJson<Lurker>(val);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            print("将对象LitJson序列化为string,然后写入文件");
            Lurker valira = new Lurker();
            string val = JsonMapper.ToJson(valira);
            File.WriteAllText(Application.persistentDataPath + "/" + "PlayerData_Valira" + ".json", val);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            print("LitJson直接读取Json文件中顶层数据");
            List<Weapon> weapons =
                JsonMapper.ToObject<List<Weapon>>(Application.persistentDataPath + "/" + "WeaponData_Valira" + ".json");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            print("LitJson反序列化Json中对象");
            string val = File.ReadAllText(Application.persistentDataPath + "/" + "PlayerData_Valira" + ".json");
            Lurker valira = JsonMapper.ToObject<Lurker>(val);
            print(valira.PlayerName);
        }
    }
}

[System.Serializable]
public class Lurker
{
    public string PlayerName = "Valira";
    public string Occupation = "Wizard";
    
    //private属性和public变量,都不会被存储
    private string hideSkill = "Rasengan";
    public string HideSkill
    {
        get
        {
            return hideSkill;
        }
        set
        {
            hideSkill = value;
        }
    }

    public Weapon[] Weapons = new[]
    {
        new Weapon("Dagger", 1),
        new Weapon("machete", 2)
    };

    //不支持字典
    public Dictionary<string, string> missionDict = new Dictionary<string, string>()
    {
        { "no.1","Kill 10 wolves" },
        { "no.2","Talk to the villager" }
    };
}
[System.Serializable]
public class Weapon
{
    public string WeaponName;
    public int ATK;

    public Weapon(string weaponName, int atk)
    {
        this.WeaponName = weaponName;
        this.ATK = atk;
    }
        
}
```

---

## 二、七个按键速查

| 按键 | API | 说明 |
|------|-----|------|
| **0** | `File.WriteAllText(path, "\"PlayerName\":\"Valira\"")` | 直接把「片段字符串」存成 .json。结果没有外层 `{}`，是**非法 JSON**，只用于演示，别在项目里这样写 |
| **1** | `File.ReadAllText(path)` | 把 JSON 文件当纯文本读出来打印；不依赖任何 JSON 库，只验证文件内容是否正确写入 |
| **2** | `JsonUtility.ToJson(valira)` + `WriteAllText` | 用 Unity **内置 JsonUtility** 将 `Lurker` 对象序列化为 JSON 字符串再写文件 |
| **3** | `File.ReadAllText` → `JsonUtility.FromJson<Lurker>(val)` | 读文件字符串 → 用 JsonUtility **反序列化回对象** |
| **4** | `JsonMapper.ToJson(valira)`（LitJson） + `WriteAllText` | 用 **LitJson**（第三方库）序列化 Lurker 并写入文件 |
| **5** | `JsonMapper.ToObject<List<Weapon>>(jsonFilePath)` | LitJson 独有的用法：**直接传一个 JSON 文件路径**就能反序列化顶层数组为 `List<T>`，不用自己 `ReadAllText` |
| **6** | `File.ReadAllText` → `JsonMapper.ToObject<Lurker>(val)` | 用 LitJson 反序列化对象后打印 `PlayerName` 验证成功 |

---

## 三、数据类的几个注意点

### 3.1 `[System.Serializable]` 属性

`Lurker` 和 `Weapon` 两个类都加了这个特性：

- **JsonUtility**：强烈依赖这个特性（不加基本就是 null / 空），且字段必须是 **public** 才能被序列化。
- **LitJson**：默认会把 public 字段 / 可读可写属性全部序列化，没有 `[Serializable]` 也能跑，但加了更稳妥，且和 Unity 其他系统兼容。

### 3.2 私有字段 & public 属性都**不会**被 JsonUtility 存储

```csharp
private string hideSkill = "Rasengan";     // 私有字段：JsonUtility 不存
public string HideSkill { get; set; }       // 属性：JsonUtility 不存
```

结果：`HideSkill` 在 JsonUtility 反序列化后为 `null`（因为 set 没被调用），如果代码里默认值又写在 backing field 里，那初始值也不会被保存。

LitJson 对属性支持好一些（如果属性是 public get + set 且两边都有），但 private 字段也不会动。

### 3.3 数组 vs 字典

- **数组 / List**：两者都支持。`Weapon[] Weapons` 会被正确序列化为 JSON 数组。
- **Dictionary**：**JsonUtility 完全不支持**。你写了 `missionDict`，JsonUtility 输出里就根本没有这个字段。
  - LitJson **支持字典**（但要求 Key 类型能直接 ToString 再 Parse，string / int 没问题）。

> 如果你用 JsonUtility 又想存字典怎么办？常见做法：把 Dictionary 转成 `List<KeyValuePairWrapper>`（一个 KV 对的数组）存，读出来再恢复回 Dictionary。

### 3.4 Weapon 类有自定义构造函数

```csharp
public Weapon(string weaponName, int atk) { ... }
```

- **JsonUtility**：反序列化时不调用构造函数，直接反射给字段赋值，所以 Weapon 即使「没有无参构造函数」也能正常反序列化回来。
- **LitJson**：对无参构造有要求，很多情况下默认构造会被调；如果类中声明了有参构造又没写无参，部分版本的 LitJson 可能抛异常，**建议手动补一个 `public Weapon() {}` 无参构造**。

---

## 四、JsonUtility vs LitJson 对照

| 维度 | JsonUtility（Unity 内置） | LitJson（第三方 dll） |
|------|---------------------------|----------------------|
| **依赖** | 零依赖，Unity 自带 | 要把 LitJson.dll 放进 Plugins |
| **性能** | 很快（内部 IL2CPP 友好） | 略慢（纯托管反射） |
| **顶层必须是对象** | ✅ **必须**（`{...}`）。直接序列化 `List<T>` → 会报错或拿到空；外面得套一层对象 | ❌ 无限制，顶层可以是 `[]` 数组 |
| **Dictionary 支持** | ❌ 不支持 | ✅ 支持 |
| **属性 / private 字段** | ❌ 都不存，只存 public 字段 | ✅ 属性可存（get + set 都有时），private 不存 |
| **直接从文件路径读** | ❌ 自己 `File.ReadAllText` | ✅ `JsonMapper.ToObject<List<T>>(filePath)` 一步到位 |
| **典型用途** | 存档（数据结构自己可控）、Unity 配置 | 顶层是数组的 JSON 配置表、字典数据、来自服务器的 JSON 数据 |

### 顶层是数组时 JsonUtility 的工作方式

假设有一个 `WeaponData_Valira.json`：

```json
[
  { "WeaponName":"Dagger","ATK":1 },
  { "WeaponName":"machete","ATK":2 }
]
```

JsonUtility 不能直接 `JsonUtility.FromJson<List<Weapon>>(File.ReadAllText(...))`——它要求顶层是对象。两种解决方法：

1. **套一个壳类**（JsonUtility 官方推荐）：

   ```csharp
   [Serializable]
   public class WeaponListWrapper
   {
       public List<Weapon> list;
   }
   ```

   然后把 JSON 改为 `{ "list": [ ... ] }`，或者把文件内容改成 `{ "list": <原文件内容> }` 再反序列化。

2. **直接用 LitJson**：就像按键 5 那样，一行 `JsonMapper.ToObject<List<Weapon>>(filePath)` 搞定。

---

## 五、常见写入流程梳理

按键 0 的「直接写片段字符串」在实际项目中不会出现，一般都是：

```
1. new Lurker() 构造对象 / 从存档读出对象改一下字段
2. ToJson 序列化为 string
   ├─ JsonUtility.ToJson(obj)
   └─ JsonMapper.ToJson(obj)     (LitJson)
3. File.WriteAllText(path, jsonString) 写文件
```

读取就是逆操作：

```
1. File.ReadAllText(path) 读 JSON 字符串
   (LitJson 顶层数组/对象可选 JsonMapper.ToObject<T>(path) 跳过这一步)
2. FromJson 反序列化回对象
   ├─ JsonUtility.FromJson<Lurker>(jsonStr)
   └─ JsonMapper.ToObject<Lurker>(jsonStr)     (LitJson)
```

> Unity 里 JSON 和二进制最大的不同：JSON 人眼可改，便于调试和做策划配置表；缺点是体积略大、解析稍慢、没有类型安全性（字段名写错就是 null）。**单机存档两者皆可，跨版本兼容 / 策划配置表一般选 JSON，纯本地大存档选 BinaryFormatter 或二进制协议。**
