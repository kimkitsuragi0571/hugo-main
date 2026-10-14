+++
title = "依赖注入代码示例（DI / ServiceCollection / 构造函数注入）"
date = "2026-09-17T10:30:00+08:00"
draft = false
slug = "dependency-injection-example"
categories = ["Unity"]
tags = ["笔记", "Dotnet", "依赖注入", "DI", "ServiceCollection", "设计模式"]
+++

依赖注入（Dependency Injection，DI）的核心思想是：**让「使用者」不负责创建「被使用者」，而是由外部容器创建并传进来**。本文用一个「玩家攻击」的例子，演示 .NET 中 `Microsoft.Extensions.DependencyInjection` 的基本用法：接口分离、构造函数注入、DI 注册与解析，以及工厂注册。

---

## 一、完整脚本

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    public static void Main(string[] args)
    {
        //1.创建DI注册表serv
        //对比WebApi就需要先手动创建services容器对象(相当于一个List)
        var serv = new ServiceCollection();

        //2.DI注册
        //如果你还注册了轻攻击LightAtk, GetRequiredService<IAtk>()只会取最后注册的那个
        serv.AddScoped<IAtk, ReAtk>();
        //这里Player类也需要注册,之后调用player实例也需要避免new Player()
        serv.AddScoped<Player>();

        //3.创建DI容器prov
        //调用serv.BuildServiceProvider()拓展方法
        using (var prov = serv.BuildServiceProvider())
        {
            //让DI容器返回一个实现类
        //容器检查Player构造函数,发现需要IAtk参数->DI注册表中查看谁对应IATK->传入ReAtk实例
            var player = prov.GetRequiredService<Player>();
            player.PlayerAtk();
        }

        // //工厂注册,参数传入lambda表达式
        // //虽然也是ServiceProvider类对象(即DI容器),但是这里的只是当前作用域对象,之前的是根容器
        // string atkType = "Heavy";
        // serv.AddScoped<IAtk>(new Func<IServiceProvider, IAtk>((IServiceProvider prov) =>
        //     {
        //         if (atkType == "Heavy")
        //         {
        //             return new ReAtk();
        //         }
        //         else
        //         {
        //             return new LightAtk();
        //         }
        //     }
        // ));
    }
}

public interface IAtk
{
    void Atk();
}

public class ReAtk:IAtk
{
    public void Atk()
    {
        Console.WriteLine("重攻击");
    }
}

public class LightAtk:IAtk
{
    public void Atk()
    {
        Console.WriteLine("轻攻击");
    }
}

//让Player只负责 "使用攻击"，不负责 "决定用什么攻击"
public class Player
{
    //1.就是让外部的"攻击行为"确定下来后就不能改变
    //这里变量名实际上不适用reatk,接口注入的核心就是不关注具体实现
    private readonly IAtk _atk;

    //2.就是说创建Player实例时,外部必须提供具体的攻击行为
    //这里实际传入的就是具体实现类
    public Player(IAtk atk)
    {
        this._atk = atk;
    }
    //3.调用PlayerAtk就是调用外部已经确定好的攻击行为
    public void PlayerAtk()
    {
        this._atk.Atk();
    }
}
```

---

## 二、DI 的三个核心步骤

### 2.1 创建注册表 `ServiceCollection`

```csharp
var serv = new ServiceCollection();
```

`ServiceCollection` 就是一个「**接口 → 实现类**」的映射表（可以理解成一个 `List`）。注册阶段只是往里面塞映射关系，**此时还没有真正创建对象**。

### 2.2 注册

```csharp
serv.AddScoped<IAtk, ReAtk>();   // 接口 IAtk 对应实现类 ReAtk
serv.AddScoped<Player>();         // Player 本身也要注册
```

`AddScoped` 是三种生命周期之一：

| 方法 | 生命周期 | 说明 |
|------|----------|------|
| `AddTransient` | 瞬时 | 每次获取都 `new` 一个新对象 |
| `AddScoped` | 作用域 | 同一个作用域内复用同一个实例（Web 请求级别） |
| `AddSingleton` | 单例 | 整个应用生命周期内只有一个实例 |

> 如果同一个接口注册了多个实现类（比如先 `AddScoped<IAtk, ReAtk>()` 再 `AddScoped<IAtk, LightAtk>()`），`GetRequiredService<IAtk>()` **只会取最后注册的那个**。如果想获取全部实现，要用 `GetServices<IAtk>()`。

### 2.3 创建容器并解析

```csharp
using (var prov = serv.BuildServiceProvider())
{
    var player = prov.GetRequiredService<Player>();
    player.PlayerAtk();
}
```

`BuildServiceProvider()` 把注册表编译成真正的 DI 容器 `prov`。调用 `GetRequiredService<Player>()` 时，容器会：

1. 检查 `Player` 的构造函数 → 发现需要 `IAtk` 参数。
2. 去注册表里查 `IAtk` 对应哪个实现类 → 找到 `ReAtk`。
3. 自动 `new ReAtk()` 并传入 `Player` 构造函数。
4. 返回组装好的 `Player` 实例。

整个过程中 **没有一行 `new Player(new ReAtk())`**，`Player` 完全不知道也不关心攻击行为是哪个实现类。

---

## 三、构造函数注入的设计要点

`Player` 类的写法是 DI 的标准范式：

```csharp
public class Player
{
    private readonly IAtk _atk;          // 依赖接口，不依赖具体实现

    public Player(IAtk atk)              // 构造函数声明所需依赖
    {
        this._atk = atk;
    }

    public void PlayerAtk()
    {
        this._atk.Atk();                 // 只负责使用，不负责创建
    }
}
```

**三个关键点**：

1. **依赖接口 `IAtk`，而不是具体类 `ReAtk`** —— 这是「面向接口编程」，换攻击行为只需要改注册那一行，`Player` 代码一行不动。
2. **依赖通过构造函数传入** —— 创建 `Player` 时外部必须提供攻击行为，否则编译都过不了。比「在构造函数里 `new ReAtk()`」耦合度低得多。
3. **字段用 `readonly`** —— 依赖一旦注入就不允许在内部被替换，保证对象状态稳定。

> 一句话总结：**让 Player 只负责「使用攻击」，不负责「决定用什么攻击」**。后者交给 DI 容器在注册阶段决定。

---

## 四、工厂注册（Factory Registration）

如果「用哪个实现类」需要**运行时根据条件判断**（而不是写死 `ReAtk`），就用工厂注册：

```csharp
string atkType = "Heavy";
serv.AddScoped<IAtk>(new Func<IServiceProvider, IAtk>((IServiceProvider prov) =>
    {
        if (atkType == "Heavy")
        {
            return new ReAtk();
        }
        else
        {
            return new LightAtk();
        }
    }
));
```

工厂注册接受一个 `Func<IServiceProvider, T>` 的 lambda：

- 参数 `prov` 是**当前作用域**的 DI 容器（不是根容器），可以在工厂里继续 `prov.GetService<XXX>()` 获取其他依赖。
- 返回值就是最终注入的实例。
- 适合「根据配置 / 环境变量动态选择实现类」的场景。

> 注意：示例里 `atkType` 是闭包捕获的外部变量。如果 `atkType` 在注册之后被修改，由于 `AddScoped` 的工厂是延迟执行的（第一次解析 `IAtk` 时才调用），实际生效的是**解析时的值**而不是注册时的值。

---

## 五、为什么要用 DI

| 维度 | 不用 DI（手动 new） | 用 DI |
|------|---------------------|-------|
| 耦合度 | `Player` 直接 `new ReAtk()`，改攻击行为要改 Player | 只改注册那一行，Player 不动 |
| 可测试性 | 单元测试无法替换攻击行为（写死了） | 可以注入 mock 实现 |
| 生命周期 | 自己管理对象创建/销毁 | 容器统一管理（Scoped/Singleton/Transient） |
| 依赖关系 | 对象互相 new，关系网复杂 | 依赖关系集中在注册表里，一目了然 |

DI 的本质是「**把对象的创建权从业务代码里剥离，集中交给容器**」。业务类只声明「我需要什么接口」，容器负责「找谁来实现」。
