+++
title = "Dotnet 配置读取示例（Json / Options / 命令行 / UserSecret）"
date = "2026-09-17T10:00:00+08:00"
draft = false
slug = "dotnet-config-reading-example"
categories = ["Unity"]
tags = ["笔记", "Dotnet", "配置", "IConfiguration", "Options", "UserSecrets"]
+++

.NET 里读取配置的方式很多，本文用同一个 `PlayerInfo.json` 作为数据源，依次演示 **5 种**读取方式：

1. **基本配置读取** — `ConfigurationBuilder` + `AddJsonFile`，通过 `configRoot[key]` 或 `Get<T>()` 直接取
2. **选项方式读取配置** — `AddOptions<TOptions>().Bind(configRoot)`，配合 `IOptions<TOptions>` 注入
3. **选项方式工厂注册** — 链式 `Bind().Configure()` 或在 `Configure` 闭包里手动 `Bind`，可叠加多步修改
4. **读取终端配置参数** — `AddCommandLine(args)`，从命令行参数读取配置
5. **UserSecret 配置** — `AddUserSecrets<T>()`，本地 C 盘的 secrets.json 覆盖公开 json

这 5 种方式的代码都基于同一份 `PlayerConfig` 数据类结构，只是配置源和注册写法不同。

---

## 一、公共数据源 PlayerInfo.json

> **注意：Json 文件必须复制到输出目录，程序实际的工作目录是 `bin/Debug/netX.X` 或 `bin/Release/netX.X` 这样的输出文件夹，而不是你的源代码文件夹。** 所以需要在 VS / Rider 里把 `PlayerInfo.json` 的「复制到输出目录」设为「如果较新则复制」，否则运行时会找不到文件。

```json
{
  "playerName": "Batman",
  "playerLevel": "13",
  "Items":
  {
    "Gun": "BatGun",
    "Car": "BatCar"
  },
  "Skills":
  [
    "Kick",
    "Punch"
  ]
}
```

这份 json 包含了 4 种常见结构：普通字符串、嵌套对象、数组、以及需要「层级读取」的子节点。后面的所有示例都围绕它展开。

---

## 二、基本配置读取

这是最底层的写法：`ConfigurationBuilder` 作为草稿，往里 `AddJsonFile`，再 `Build()` 生成 `IConfigurationRoot`，之后所有读取都从根对象里取。

```csharp
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal class Program
{
    public static void Main(string[] args)
    {
        //1.配置系统构建器(用于往里面添加Json,相当于草稿)
        ConfigurationBuilder configBuilder = new ConfigurationBuilder();
        //2.添加Json配置文件源
        configBuilder.AddJsonFile
        (
            //文件名-文件不存在是否报错-文件修改后是否自动刷新
            "PlayerInfo.json",
            optional: false,
            reloadOnChange: true
        );
        //3.配置系统根对象(构建器添加好数据,就Build生成根对象,以后就从根对象里面读取)
        IConfigurationRoot configRoot = configBuilder.Build();

        //4.接下来只需要从根对象里面读取
        //4.1 读取普通字符串和数字(注意都是string还需要转化)
        //string name = configRoot.GetValue<string>("playerName", "Unknown");可以避免读取null
        string playerName = configRoot["playerName"];
        //4.2 读取嵌套对象
        //用:冒号表示层级就行了
        string gun = configRoot["Items:Gun"];
        string car = configRoot["Items:Car"];
        //4.3 读取数组列表
        //用:冒号后接索引就行了
        string skill1 = configRoot["Skills:0"];
        string skill2 = configRoot["Skills:1"];
        //4.4 读取某一层级
        //直接先读取某个非叶子节点作为层级,然后通过该层级直接获取子节点,也可以直接foreach遍历所有子节点
        //这样就不用每个都写一遍:冒号层级获取了
        IConfigurationSection itemsSection = configRoot.GetSection("Items");
        //还可以直接遍历获取所有子节点foreach (var child in itemsSection.GetChildren())
        string gunSection = itemsSection["Gun"];
        string carSection = itemsSection["Car"];

        //5.直接通过配置根对象反序列化
        PlayerConfig player = configRoot.Get<PlayerConfig>();
    }
}

//类名和Json文件名[不需要!]相同
public class PlayerConfig
{
    //属性名和Json字段名需要相同
    public string PlayerName { get; set; }
    public int PlayerLevel { get; set; }       // 自动把字符串 "13" 转成 int
    public ItemInfo Items { get; set; }        // 对应嵌套对象
    public List<string> Skills { get; set; }   // 对应数组
}

//同样的,嵌套类名也随便取
public class ItemInfo
{
    public string Gun { get; set; }
    public string Car { get; set; }
}
```

**关键点**：

| 写法 | 作用 |
|------|------|
| `configRoot["playerName"]` | 直接按 key 取，返回字符串 |
| `configRoot["Items:Gun"]` | 用 `:` 冒号表示嵌套层级 |
| `configRoot["Skills:0"]` | 数组用 `:索引` |
| `configRoot.GetSection("Items")` | 取某个子节点作为 `IConfigurationSection`，之后再在它上面取子节点 |
| `configRoot.Get<PlayerConfig>()` | 把整个根对象反序列化成强类型对象（属性名要和 json 字段名一致） |

> `AddJsonFile` 的三个参数：`路径 / optional（文件不存在是否报错）/ reloadOnChange（文件修改后是否自动刷新）`。生产环境一般 `optional: false`（缺文件直接报错比静默 null 好排查）。

---

## 三、选项方式读取配置

基本读取是「直接从 configRoot 取」，选项方式则是「把配置绑定到一个强类型对象，再通过 `IOptions<T>` 注入到业务类」。这需要配合 DI 容器使用。

```csharp
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal class Program
{
    public static void Main(string[] args)
    {
        //1.依旧创建构造器->添加文件->Build根对象
        ConfigurationBuilder configBuilder = new ConfigurationBuilder();
        configBuilder.AddJsonFile
        (
            "PlayerInfo.json",
            optional: false,
            reloadOnChange: true
        );
        IConfigurationRoot configRoot = configBuilder.Build();
        //如果你想直接Get返回强类型对象,对比下面的选项方式
        PlayerConfig player = configRoot.Get<PlayerConfig>();

        //4.创建DI注册容器,注册根对象
        var serv = new ServiceCollection();
        //当调用IOptions<PlayerConfig>接口时,返回PlayerConfig数据副本(通过之前的ConfigRoot生成)
        //分别绑定生成PlayerConfig的数据,用configRoot根对象实现
        serv.AddOptions<PlayerConfig>().Bind(configRoot);

        //别忘了显式注册PlayerService本身(虽然你不写系统也自动帮你实现)
        //哦好吧AI告诉我系统并不会帮我自动实现,系统咋这么坏
        serv.AddScoped<PlayerService>();

        //5.创建DI容器
        using (var prov = serv.BuildServiceProvider())
        {
            //同样通过容器获取PlayerService对象
            var playerService = prov.GetRequiredService<PlayerService>();
            //打印信息
            playerService.ShowPlayerInfo();
        }
    }
}

//2.创建业务服务类
public class PlayerService
{
    private readonly PlayerConfig _config;

    //当请求PlayerService实例时,需要IOptions<PlayerConfig>参数
    public PlayerService(IOptions<PlayerConfig> options)
    {
        //prov检查
        // .Value属性才是真正的 PlayerConfig 对象
        _config = options.Value;
    }

    public void ShowPlayerInfo()
    {
        Console.WriteLine("玩家: " + _config.PlayerName);
        Console.WriteLine("等级: " + _config.PlayerLevel);
        Console.WriteLine("武器: " + _config.Items.Gun);
        Console.WriteLine("技能数: " + _config.Skills.Count);
    }
}

//3.依旧创建对应类(属性有set且和字段相同)
public class PlayerConfig
{

    public string PlayerName { get; set; }
    public int PlayerLevel { get; set; }
    public ItemInfo Items { get; set; }
    public List<string> Skills { get; set; }
}

public class ItemInfo
{
    public string Gun { get; set; }
    public string Car { get; set; }
}
```

**关键调用链**：

```
AddOptions<PlayerConfig>().Bind(configRoot)
    ↓ 注册 IOptions<PlayerConfig>
    ↓
PlayerService 构造函数需要 IOptions<PlayerConfig>
    ↓
DI 容器自动注入 options.Value (即 PlayerConfig 实例)
```

> `IOptions<T>.Value` 才是真正的配置对象。另外 `PlayerService` 本身也必须显式 `AddScoped<PlayerService>()` 注册，否则 `GetRequiredService<PlayerService>()` 会抛异常。

---

## 四、选项方式工厂注册

如果绑定后还想**再叠加几步修改**（比如改名字、改等级），可以用工厂注册的写法：链式 `Bind().Configure()` 或者在 `Configure` 闭包里手动 `Bind` 再改。

```csharp
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal class Program
{
    public static void Main(string[] args)
    {
        ConfigurationBuilder configBuilder = new ConfigurationBuilder();
        configBuilder.AddJsonFile
        (
            "PlayerInfo.json",
            optional: false,
            reloadOnChange: true
        );
        //4.创建另一个构建器,Add并build另一个Root
        ConfigurationBuilder serverBuilder = new ConfigurationBuilder();
        serverBuilder.AddJsonFile
        (
            "ServerInfo.json",
            optional: false,
            reloadOnChange: true
        );
        IConfigurationRoot configRoot = configBuilder.Build();
        IConfigurationRoot serverRoot = serverBuilder.Build();

        var serv = new ServiceCollection();
        //哦下面有工厂注册的写法这里我就注释掉了
        //serv.AddOptions<PlayerConfig>().Bind(configRoot);
        serv.AddOptions<ServerConfig>().Bind(serverRoot);

        //6.选项方法的工厂注册写法(提前链式调用Bind,指定构建器)
        //链式写法:泛型和参数都只能是同一配置类PlayerConfig,按照顺序执行
        serv.AddOptions<PlayerConfig>()
            .Bind(configRoot)//这里就是依次链式调用Bind->第一步->第二步
            .Configure((PlayerConfig config) =>
            {
                Console.WriteLine("第一步");
                config.PlayerName = "Ken";
            })
            .Configure((PlayerConfig config) =>
            {
                Console.WriteLine("第二步");
                config.PlayerLevel = 99;
            });
        //7.工厂注册的另一种写法(不通过提前链式调用实现)
        serv.AddOptions<ServerConfig>()
            .Configure(config =>
            {
                //通过闭包引用外部serverRoot根对象,然后Bind
                serverRoot.Bind(config);
                //然后就可以修改了
                config.Port = 9999;
            });

        //8.业务类只需要注册同一个就行
        serv.AddScoped<PlayerService>();

        using (var prov = serv.BuildServiceProvider())
        {
            var playerService = prov.GetRequiredService<PlayerService>();
            playerService.ShowPlayerInfo();
        }
    }
}

public class PlayerService
{
    private readonly PlayerConfig _config;
    //2.业务类里面新增额外注入即可,不用专门新增业务类
    private readonly ServerConfig _serverConfig;
    public PlayerService(IOptions<PlayerConfig> options, IOptions<ServerConfig> serverOptions)
    {
        _config = options.Value;
        _serverConfig = serverOptions.Value;
    }

    public void ShowPlayerInfo()
    {
        Console.WriteLine("玩家: " + _config.PlayerName);
        Console.WriteLine("等级: " + _config.PlayerLevel);
        //3.这里就可以直接使用另一个配置类的数据了
        Console.WriteLine("服务器IP: " + _serverConfig.Ip);
        Console.WriteLine("服务器端口: " + _serverConfig.Port);
    }
}

 //1.这里新增额外配置类
public class ServerConfig
{
    public string Ip { get; set; }
    public int Port { get; set; }
}

public class PlayerConfig
{

    public string PlayerName { get; set; }
    public int PlayerLevel { get; set; }
    public ItemInfo Items { get; set; }
    public List<string> Skills { get; set; }
}

public class ItemInfo
{
    public string Gun { get; set; }
    public string Car { get; set; }
}
```

**两种工厂注册写法**：

| 写法 | 特点 |
|------|------|
| `AddOptions<T>().Bind(root).Configure(...)` | 链式，先 Bind 再叠加多个 Configure，按顺序执行 |
| `AddOptions<T>().Configure(config => { root.Bind(config); ... })` | 只调一个 Configure，在闭包里先 Bind 再改 |

业务类的构造函数可以同时注入 `IOptions<PlayerConfig>` 和 `IOptions<ServerConfig>`，DI 会分别解析。

---

## 五、读取终端配置参数

有时候配置不想写在 json 里，而是启动时通过命令行参数传入（比如 `dotnet run -- PlayerName=Hero PlayerLevel=99`），就用 `AddCommandLine(args)`。

```csharp
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal class Program
{
    public static void Main(string[] args)
    {
        //依旧创建构造器,但是不需要AddJsonFile了
        var configBuilder = new ConfigurationBuilder();
        //这里通过AddCommandLine(也就是IConfigurationBuilder的拓展方法)实现:
        //构建器添加cmd参数->构造Root对象
        configBuilder.AddCommandLine(args);
        var configRoot = configBuilder.Build();

        var serv = new ServiceCollection();
        serv.AddOptions().Configure<PlayerConfig>(((PlayerConfig config) =>
        {
            //这段是通过闭包引入外界configRoot
            configRoot.Bind(config);
        }));

        serv.AddScoped<PlayerService>();
        using (var prov = serv.BuildServiceProvider())
        {
            var playerService = prov.GetRequiredService<PlayerService>();
            //终端负责在调用指令时传入参数dotnet run -- PlayerName=Hero PlayerLevel=99
            playerService.ShowPlayerInfo();
        }
    }
}

//2.创建业务服务类
public class PlayerService
{
    private readonly PlayerConfig _config;

    //当请求PlayerService实例时,需要IOptions<PlayerConfig>参数
    public PlayerService(IOptions<PlayerConfig> options)
    {
        //prov检查
        // .Value属性才是真正的 PlayerConfig 对象
        _config = options.Value;
    }

    public void ShowPlayerInfo()
    {
        Console.WriteLine("玩家: " + _config.PlayerName);
        Console.WriteLine("等级: " + _config.PlayerLevel);

        if (_config.Items != null)
        {
            Console.WriteLine("武器: " + _config.Items.Gun);
            Console.WriteLine("载具: " + _config.Items.Car);
        }

        if (_config.Skills != null)
        {
            Console.WriteLine("技能数: " + _config.Skills.Count);
            foreach (var skill in _config.Skills)
            {
                Console.WriteLine(" - " + skill);
            }
        }
    }
}

//3.依旧创建对应类(属性有set且和字段相同)
public class PlayerConfig
{

    public string? PlayerName { get; set; }
    public int PlayerLevel { get; set; }
    public ItemInfo? Items { get; set; }
    public List<string>? Skills { get; set; }
}

public class ItemInfo
{
    public string? Gun { get; set; }
    public string? Car { get; set; }
}
```

**运行方式**：

```
dotnet run -- PlayerName=Hero PlayerLevel=99
```

- `--` 后面的参数会被 `AddCommandLine` 解析成 key-value 对。
- `configRoot.Bind(config)` 把命令行参数绑定到 `PlayerConfig` 上。
- 注意这里没有 `AddJsonFile`，配置完全来自命令行。

---

## 六、UserSecret 配置

正式项目里密码、密钥这类敏感信息不能写进公开的 `PlayerInfo.json`（否则提交 Git 就泄露了）。.NET 提供了 `UserSecrets` 机制：把敏感配置存在**本地 C 盘**的 `secrets.json` 里，运行时用它覆盖公开 json 中对应的字段。

```csharp
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal class Program
{
    public static void Main(string[] args)
    {
        //1.安装Microsoft.Extensions.Configuration.UserSecrets包
        //2.项目目录打开终端,执行dotnet user-secrets init,在csproj文件中生成唯一Id
        //3.第一次在命令行写入密码dotnet user-secrets set "Player:Password" "123",自动创建secrets.json
        //注意在C盘生成目录而不是项目目录哈,所以提交Git项目时ID不会被泄露

        //4.挂载到配置构建器
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddJsonFile("PlayerInfo.json",true, true);
        //多个配置源时,后配置的优先级高,这里就是优先级高于普通Json,替换掉了对应内容
        //这种写法只能是secrets.json用于替换普通json,不能是随便写一个.json泛型改为对应名就可以用于替换普通Json了
        //而泛型替换的意义在于配置源分散在不同项目/类库时,用于指定读取哪个项目的 secrets.json
        //比如configBuilder.AddUserSecrets<PlayerConfig>(); 读取Game.Core 项目里的PlayerConfig
        configBuilder.AddUserSecrets<Program>();

        //5.执行流程
        //5.1 AddJsonFile("PlayerInfo.json")读取公开的json配置,相当于默认值
        //5.2 AddUserSecrets<Program>()总之指定系统运行时去C盘找secrets.json
        //5.3 读取C盘中secrets.json中对应内容用于替代PlayerInfo.json(不能是随便一个.json用于覆盖PlayerInfo.json)

        //5.4 替换PlayerInfo.json中的其他内容:dotnet user-secrets set "PlayerName" "真实玩家名字"

        //因为真正的内容存储在本地C盘,git上传项目时不会暴露,但换电脑就没有对应密码了
        //这只是测试环境下防止git暴露密码而已,正式环境肯定不能用的

        //接下来就是验证输出环节喵
        var configRoot = configBuilder.Build();
        var serv = new ServiceCollection();
        serv.AddOptions<PlayerConfig>()
            .Bind(configRoot);
        serv.AddScoped<PlayerService>();

        using (var prov = serv.BuildServiceProvider())
        {
            var playerService = prov.GetRequiredService<PlayerService>();
            playerService.ShowPlayerInfo();
        }
    }
}

public class PlayerService
{
    private readonly PlayerConfig _config;

    public PlayerService(IOptions<PlayerConfig> options)
    {
        _config = options.Value;
    }

    public void ShowPlayerInfo()
    {
        Console.WriteLine("玩家: " + _config.PlayerName);
        Console.WriteLine("等级: " + _config.PlayerLevel);

        if (_config.Items != null)
        {
            Console.WriteLine("武器: " + _config.Items.Gun);
            Console.WriteLine("载具: " + _config.Items.Car);
        }

        if (_config.Skills != null)
        {
            Console.WriteLine("技能数: " + _config.Skills.Count);
            foreach (var skill in _config.Skills)
            {
                Console.WriteLine(" - " + skill);
            }
        }
    }
}

public class PlayerConfig
{

    public string? PlayerName { get; set; }
    public int PlayerLevel { get; set; }
    public ItemInfo? Items { get; set; }
    public List<string>? Skills { get; set; }
}

public class ItemInfo
{
    public string? Gun { get; set; }
    public string? Car { get; set; }
}
```

**使用步骤**：

1. 安装 `Microsoft.Extensions.Configuration.UserSecrets` 包。
2. 项目目录执行 `dotnet user-secrets init`，会在 `.csproj` 里生成唯一 `UserSecretsId`。
3. 执行 `dotnet user-secrets set "PlayerName" "真实玩家名字"`，自动在 C 盘创建 `secrets.json`。
4. 代码里 `configBuilder.AddJsonFile("PlayerInfo.json").AddUserSecrets<Program>()`。

**核心规则**：多个配置源时，**后添加的优先级更高**。`AddUserSecrets` 在 `AddJsonFile` 之后，所以 secrets.json 里的字段会覆盖 json 里的同名字段。因为 secrets.json 存在本地 C 盘（不在项目目录），提交 Git 时不会泄露。

---

## 七、五种方式对比

| 方式 | 配置源 | 是否需要 DI | 适用场景 |
|------|--------|-------------|----------|
| 基本配置读取 | json 文件 | 否 | 简单小工具，直接 `configRoot[key]` 取 |
| 选项方式读取 | json 文件 | 是 | 正式项目，强类型 + `IOptions<T>` 注入 |
| 选项方式工厂注册 | 多个 json | 是 | 需要叠加多步修改 / 多配置类 |
| 终端配置参数 | 命令行 args | 是 | 启动时动态传参（如临时覆盖端口） |
| UserSecret | 本地 C 盘 secrets.json | 是 | 开发环境防止敏感信息提交 Git |

> 实际项目中最常用的组合是：**选项方式读取 + UserSecret**。公开配置放 json，敏感字段（密码、连接串）放 secrets.json，运行时自动覆盖。
