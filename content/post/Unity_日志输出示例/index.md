+++
title = "Dotnet 日志输出示例（ILogger / AddLogging / 日志级别 / 结构化日志）"
date = "2026-11-14T10:00:00+08:00"
draft = false
slug = "dotnet-logging-example"
categories = [".NET开发"]
tags = ["笔记", "Dotnet", "日志", "ILogger", "DI"]
+++

.NET 正式项目里几乎都用 `Microsoft.Extensions.Logging` 这套日志抽象：它不绑定具体输出端（控制台、文件、第三方），通过 DI 注入 `ILogger<T>` 就能在任意类里打日志。本文用一个「玩家攻击」的例子，演示：

1. **注册日志系统** — `AddLogging` + `AddConsole`，并设定最低输出级别
2. **构造函数注入 `ILogger<T>`** — 泛型参数只是日志标签
3. **不同级别输出** — `LogDebug` / `LogInformation` / `LogWarning` / `LogError`
4. **结构化占位符** — 为什么推荐 `{Power}` 而不是字符串拼接
5. **异常日志** — `LogError(ex, ...)` 把异常对象一起带上

---

## 一、完整脚本

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal class Program
{
    public static void Main(string[] args)
    {
        var serv = new ServiceCollection();

        // 开启日志系统 + 注册日志级别和输出对象(直接在注册列表中注册对日志的要求)
        // 需要在 DI 容器创建之前开启，因为其他类创建时需要立即用到日志系统
        serv.AddLogging((ILoggingBuilder logBuilder) =>
        {
            // 告诉日志系统把记录的文字打印到 Console 窗口内
            logBuilder.AddConsole();
            // 设定打印日志的最低级别，低于 Debug 级别的不打印
            logBuilder.SetMinimumLevel(LogLevel.Debug);
        });

        // 注册服务
        serv.AddScoped<IAtk, ReAtk>();
        serv.AddScoped<Player>();

        // 构建容器并获取服务
        using (var prov = serv.BuildServiceProvider())
        {
            var player = prov.GetRequiredService<Player>();
            // 执行带日志的攻击方法
            player.AtkLogger(skillName: "超级重击", powerValue: "999");
        }
    }
}

public class Player
{
    private readonly IAtk _atk;

    // ILogger<T> 只是决定日志的标签而已，建议用当前 Player 类名便于找问题
    private readonly ILogger<Player> _logger;

    // 容器创建 Player 时，会自动注入 IAtk 和 ILogger<Player>
    public Player(IAtk atk, ILogger<Player> logger)
    {
        _atk = atk;
        _logger = logger;
    }

    // 设个带日志的 Atk 方法用于演示
    public void AtkLogger(string skillName, string powerValue)
    {
        // 实际开发中推荐用占位符形式，字符串拼接会丧失结构化字段信息
        // _logger.LogInformation("技能名:{SkillName},能量值:{Power}", skillName, powerValue);

        _logger.LogInformation("技能名" + skillName);
        _logger.LogInformation("能量值" + powerValue);

        // 测试：通过 _logger 工具打印不同级别的 Log
        try
        {
            int power = int.Parse(powerValue);

            if (power > 100)
            {
                _logger.LogWarning("能量值过高: {Power}", power);
            }

            _atk.Atk();
            _logger.LogDebug("技能执行完毕");
        }
        catch (FormatException ex)
        {
            _logger.LogError(ex, message: "能量值解析失败");
        }
    }
}

public interface IAtk
{
    void Atk();
}

public class ReAtk : IAtk
{
    public void Atk()
    {
        Console.WriteLine("重攻击");
    }
}

public class LightAtk : IAtk
{
    public void Atk()
    {
        Console.WriteLine("轻攻击");
    }
}
```

---

## 二、第一步：注册日志系统

日志是在 `ServiceCollection` 上通过 `AddLogging` 注册的：

```csharp
serv.AddLogging((ILoggingBuilder logBuilder) =>
{
    logBuilder.AddConsole();                          // 输出到控制台
    logBuilder.SetMinimumLevel(LogLevel.Debug);       // 最低输出级别
});
```

两个要点：

- **`AddConsole()`** 只是其中一个「输出端（Provider）」。换成写文件、接第三方日志平台，也只是换这里的注册方法，业务代码完全不用动。
- **`SetMinimumLevel(LogLevel.Debug)`** 决定了低于这个级别的日志会被直接丢弃。级别从低到高是 `Trace < Debug < Information < Warning < Error < Critical`，设成 `Debug` 意味着 `Trace` 不打印。

> **注意：`AddLogging` 要在其他服务注册之前调用。** 因为 DI 容器解析某个类时，如果它的构造函数需要 `ILogger<T>`，容器必须已经知道怎么提供日志对象。顺序错了会直接抛「无法解析 `ILogger<T>`」的异常。

---

## 三、注入 `ILogger<T>`

`Player` 的构造函数多了一个 `ILogger<Player>` 参数，容器会自动注入，不需要我们手动 `new`：

```csharp
public class Player
{
    private readonly IAtk _atk;
    private readonly ILogger<Player> _logger;

    public Player(IAtk atk, ILogger<Player> logger)
    {
        _atk = atk;
        _logger = logger;
    }
}
```

`ILogger<T>` 里的泛型 `T` **只是一个分类标签**：日志输出时通常会带上这个类别名，方便你在控制台/日志文件里按类过滤。所以约定俗成写「当前类名」，比如 `ILogger<Player>`。

---

## 四、日志级别

`ILogger` 上每种级别都有对应的扩展方法，底层最终都会调用 `Log(level, ...)`：

| 方法 | 级别 | 典型用途 |
|------|------|----------|
| `LogTrace` | Trace | 最细粒度，逐帧/逐循环调试 |
| `LogDebug` | Debug | 开发期调试信息 |
| `LogInformation` | Information | 正常业务流程（启动、加载完成等） |
| `LogWarning` | Warning | 可恢复的异常、参数异常但程序还能继续 |
| `LogError` | Error | 发生错误，当前操作失败 |
| `LogCritical` | Critical | 严重故障，应用可能无法继续 |

例子里的几种用法：

```csharp
_logger.LogInformation("技能名" + skillName);       // Information
_logger.LogWarning("能量值过高: {Power}", power);   // Warning
_logger.LogDebug("技能执行完毕");                   // Debug
_logger.LogError(ex, message: "能量值解析失败");    // Error + 异常对象
```

---

## 五、结构化日志（占位符 vs 字符串拼接）

这是最容易忽略、但最重要的一点。对比两种写法：

```csharp
// 不推荐：字符串拼接
_logger.LogInformation("技能名" + skillName);

// 推荐：占位符
_logger.LogInformation("技能名:{SkillName},能量值:{Power}", skillName, powerValue);
```

区别不在于「好不好看」，而在于**信息能不能被结构化**：

- 字符串拼接只是把最终一句话丢给日志系统，框架**无法知道**里面哪段是技能名、哪段是能量值；
- 占位符写法会把参数作为**独立字段**保留下来。控制台输出看起来差不多，但换成 JSON 日志文件或第三方平台时，就能按 `SkillName`、`Power` 检索和筛选。

> 占位符是 `{名字}`，不是 `string.Format` 的 `{0}`。参数按出现顺序对应，名字只是给结构化用的字段名。

---

## 六、异常日志

打异常日志不要只写 `LogError(ex.Message)`，应该把**异常对象**一起传进去：

```csharp
try
{
    int power = int.Parse(powerValue);
}
catch (FormatException ex)
{
    _logger.LogError(ex, message: "能量值解析失败");
}
```

传了 `ex` 之后，堆栈信息、内部异常、异常类型都会被记录下来；只传字符串的话，线上排查问题时基本等于没有信息。

---

## 七、小结

| 步骤 | 写法 | 说明 |
|------|------|------|
| 注册日志 | `serv.AddLogging(b => { b.AddConsole(); b.SetMinimumLevel(...); })` | 必须在其他服务注册之前 |
| 注入日志 | 构造函数参数加 `ILogger<T>` | `T` 用当前类名当标签 |
| 输出日志 | `_logger.LogInformation/...` | 按级别选择方法 |
| 带参数 | `_logger.LogInformation("值:{Power}", power)` | 用占位符保留结构化字段 |
| 异常 | `_logger.LogError(ex, "...")` | 一定要传异常对象 |

> 实际项目里，日志的「注册」通常在程序入口组装容器时统一配置一次，业务类只负责注入 `ILogger<T>` 并按需打日志——输出到控制台还是写文件，是入口配置的事，业务代码不关心。
