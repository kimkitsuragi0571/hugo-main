+++
title = "Unity单例模式实现总结"
date = "2026-07-16T19:30:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "设计模式", "单例模式"]
+++

单例模式（Singleton Pattern）是游戏开发中最常用的设计模式之一，确保一个类只有一个实例，并提供全局访问点。本文总结了在 Unity 开发中常见的几种单例模式实现方式。

## 一、普通 C# 单例模式

### 1. 懒汉式单例

懒汉式，即延迟加载，在第一次使用时才创建实例。

```csharp
public class AudioManager_1 
{
  private static AudioManager_1 _instance;

  public static AudioManager_1 Instance
  {
      get
      {
          if (_instance == null)
          {
              _instance = new AudioManager_1();
          }
          return _instance;
      }
  }
  
  private AudioManager_1()
  {
      
  }

  public void DataInit()
  {
      Debug.Log("AudioManager-懒汉单例");
  }
}
```

**特点：**
- 第一次访问时才创建实例，节省资源
- 构造函数私有，防止外部实例化
- 线程不安全，多线程环境下可能创建多个实例

---

### 2. 饿汉式单例

饿汉式，即提前加载，在类加载时就创建实例。

```csharp
public class DataManager_2 
{
    private static readonly  DataManager_2 _instance = new DataManager_2();

    public static DataManager_2 Instance
    {
        get
        {
            return _instance;
        }
    }

    private DataManager_2()
    {
        
    }

    public void DataInit()
    {
        Debug.Log("DataManager-饿汉单例");
    }
}
```

**特点：**
- 类加载时立即创建实例，线程安全
- 使用 `readonly` 关键字确保只能在静态构造时赋值
- 无论是否使用都会占用资源

---

### 3. 线程安全单例（双重锁定）

通过双重检查锁定（Double-Check Locking）实现线程安全的懒汉式单例。

```csharp
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
```

**特点：**
- 双重判断 + lock 锁，保证线程安全
- 第一层判断避免每次都加锁，提升性能
- 第二层判断确保只创建一个实例
- 适用于多线程环境

---

### 4. 静态内部类单例

利用 C# 静态内部类的特性实现延迟初始化。

```csharp
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
```

**特点：**
- 只有第一次访问 `Instance` 时，内部类才会被加载
- 由 CLR 保证线程安全
- 代码简洁，性能优异

---

### 5. Lazy 超级懒汉单例

使用 .NET 提供的 `Lazy<T>` 类实现延迟初始化。

```csharp
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
```

**特点：**
- 基于 `Lazy<T>` 实现，.NET 框架原生支持
- 默认线程安全，支持配置线程安全模式
- 支持自定义初始化逻辑
- 代码最简洁

---

### 6. 泛型单例基类（普通 C# 版）

将单例逻辑封装为泛型基类，方便复用。

```csharp
public class Singleton_1<T> where T : class, new()
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }

    protected Singleton_1()
    {
        
    }
}
```

**使用方式：**

```csharp
public class ItemManager_5 : Singleton_1<ItemManager_5>
{
  public void DataInit()
  {
    Debug.Log("ItemManager-继承了Singleton单例基类");
  }
}
```

**特点：**
- 所有管理器只需继承基类即可获得单例能力
- 减少重复代码，便于统一维护
- 泛型约束 `where T : class, new()` 确保 T 是引用类型且有无参构造函数

---

## 二、Unity MonoBehaviour 单例模式

在 Unity 中，很多管理器需要继承 `MonoBehaviour` 以使用协程、生命周期函数等特性，这时需要特殊的单例实现。

### 1. 基础 MonoBehaviour 单例

手动实现的 MonoBehaviour 单例，需要挂载到场景物体上。

```csharp
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
```

**特点：**
- 在 `Awake` 中初始化实例
- `DontDestroyOnLoad` 确保场景切换时不被销毁
- 重复实例自动销毁，保证唯一
- 必须手动挂载到场景物体上

---

### 2. MonoBehaviour 泛型单例基类

将 MonoBehaviour 单例封装为泛型基类，支持 `Awake` 重写。

```csharp
public class Singleton_2<T> : MonoBehaviour where T: MonoBehaviour
{
   private static T _instance;

   public static T Instance
   {
       get
       {
           if (_instance == null)
           {
               _instance = FindObjectOfType<T>();
           }
           return _instance;
       }
   }

   protected virtual void Awake()
   {
       if (_instance == null)
       {
           _instance = this as T;
           DontDestroyOnLoad(this.gameObject);
       }
       else
       {
           Destroy(this.gameObject);
       }
   }
}
```

**使用方式：**

```csharp
public class SceneManager_8 : Singleton_2<SceneManager_8>
{
    public int lev;

    protected override void Awake()
    {
      base.Awake();
      Debug.Log("继承单例基类拓展Awake逻辑");
    }

    public void DataInit()
    {
        Debug.Log("SceneManager-继承了Mono单例基类");
    }
}
```

**特点：**
- 访问 `Instance` 时会自动在场景中查找
- 提供 `virtual Awake`，子类可重写扩展逻辑
- 仍需手动挂载到场景物体

---

### 3. 自动创建的 MonoBehaviour 单例基类

访问时自动创建 GameObject 并挂载组件，无需手动预设。

```csharp
public class Singleton_3<T> : MonoBehaviour where T:MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                //手动寻找场景中脚本
                _instance = FindObjectOfType<T>();
                //找不到就创建
                if (_instance == null)
                {
                    //直接用继承的Manager来命名
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                    //脚本是固定在obj物体上,所以保留obj
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static void Clear()
    {
        _instance = null;
    }
}
```

**使用方式：**

```csharp
public class NPCManager_9 : Singleton_3<NPCManager_9>
{
    protected override void Awake()
    {
        base.Awake();
        Debug.Log("继承自动创建obj的单例基类拓展Awake逻辑");
    }

    public void DataInit()
    {
        Debug.Log("NPCManager-自动创建obj的单例模式基类");
    }
}
```

**特点：**
- 访问时自动创建，无需手动挂载
- 自动以类名命名 GameObject
- 提供 `Clear()` 方法用于手动清空实例
- 使用最方便，推荐用于游戏管理器

---

## 三、各实现方式对比

| 实现方式 | 线程安全 | 延迟加载 | 需要挂载 | 推荐场景 |
|---------|---------|---------|---------|---------|
| 懒汉式 | ❌ | ✅ | - | 单线程、简单管理器 |
| 饿汉式 | ✅ | ❌ | - | 确定会使用、初始化简单 |
| 双重锁定 | ✅ | ✅ | - | 多线程、性能要求高 |
| 静态内部类 | ✅ | ✅ | - | 大多数 C# 单例场景 |
| Lazy\<T\> | ✅ | ✅ | - | .NET 环境，代码简洁 |
| 普通泛型基类 | ❌ | ✅ | - | 多个纯 C# 管理器复用 |
| MonoBehaviour 基础版 | - | ❌ | ✅ | 单个 UI 管理器 |
| MonoBehaviour 泛型基类 | - | ⚠️ 需先查找 | ✅ | 多个 MonoBehaviour 管理器 |
| 自动创建 MonoBehaviour | - | ✅ | ❌ | 游戏核心管理器，推荐 |

---

## 四、使用建议

1. **纯数据/逻辑管理器**：优先使用 **Lazy\<T\>** 或 **静态内部类** 实现
2. **需要协程/生命周期**：使用 **自动创建的 MonoBehaviour 泛型单例基类**
3. **多个同类型管理器**：封装为 **泛型基类** 减少重复代码
4. **场景切换不销毁**：务必调用 `DontDestroyOnLoad`
5. **注意初始化顺序**：避免在 `Awake` 中访问其他单例导致的初始化问题
