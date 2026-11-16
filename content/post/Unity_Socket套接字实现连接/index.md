+++
title = "Socket套接字实现连接中客户端和服务器的写法"
date = "2026-11-16T09:00:00+08:00"
draft = false
slug = "socket-client-server-connection"
categories = ["Unity"]
tags = ["笔记", "Unity", "Socket", "网络编程", "TCP"]
+++

用最基础的 `System.Net.Sockets.Socket`（TCP）演示一次完整连接：服务器端负责「等连接、收发消息」，客户端负责「主动连接、收发消息」。代码放在一个 Unity `MonoBehaviour` 里方便测试，但 Socket 部分与 Unity 无关，纯 .NET。

先记住一个核心区别：

- **服务器**需要**两个** Socket：一个负责监听/迎宾，一个负责实际通信；
- **客户端**只需要**一个** Socket。

---

## 一、完整脚本

```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class SocketConnectTest : MonoBehaviour
{
    //哦好吧,先按下E再按下R也是错误的
    //因为当你按下E,Accept会导致系统阻塞,没法按下R(输入系统也阻塞)
    //实际一般用异步或者协程等触发,但是这里暂时懒得写了
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.E))
    //     {
    //         SocketServer();
    //     }
    //     else if (Input.GetKeyDown(KeyCode.R))
    //     {
    //         SocketClient();
    //     }
    // }

    //服务器的代码
    void SocketServer()
    {
        //1.创建socket套接字
        Socket socketTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //2.Socket和IP端口对象绑定
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        try
        {
            socketTcp.Bind(ipEndPoint);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }
        //3.用Listen方法监听
        //服务端程序监听是否有客户端访问8080端口(不阻塞只是打个招呼)
        //最多支持100人连接
        socketTcp.Listen(100);
        //4.知道到这一句就开始阻塞程序,直到有客户端连接
        //如果直接Accept而没有Listen,操作系统不知道服务器等待8080端口被访问,直接拒绝

        //socketConnect是客户端和服务器的连接通道
        //socketTcp只是负责调用.Accept(),让客户端来连接(然后返回个socketConnect)
        Socket socketConnect = socketTcp.Accept();

        //5.收发数据
        //Send
        //类似返回确认,当有客户端连接8080端口时给它发送
        //这里只有第一个连上8080端口的客户端能收到,实际一般配合while(true)里面装Accept和Send...
        socketConnect.Send(Encoding.UTF8.GetBytes("这是服务器端发送的消息"));
        //Receive
        //只是个接收数据的数组
        byte[] result = new byte[1024];
        int len = socketConnect.Receive(result);
        print("获取对方(客户端)的IP端口地址" + socketConnect.RemoteEndPoint.ToString());
        //这里你用UTF-8转化就已经默认了是纯文本,不会是对象或者Protobuf啥的
        print("文本转化" + Encoding.UTF8.GetString(result, 0, len));

        //6.Shutdown释放连接(禁用读写)
        //Both就是禁止收发,SocketShutdown.Send就是只禁止了Send
        socketConnect.Shutdown(SocketShutdown.Both);

        //7.关闭Socket套接字
        //注意关闭的是socketConnect这个通道,而不是socketTcp这个前台
        socketConnect.Close();
    }

    //客户端的代码
    void SocketClient()
    {
        //1.依旧先创建Socket
        Socket socketTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //2.连接指定服务器的IP地址
        //连接到这个IP端口,就可以找到服务器正在监听的Socket
        //只在本机测试,所以CLI和SERVER都只需要填写本地回环地址的8080端口
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        try
        {
            socketTcp.Connect(ipEndPoint);
        }
        //SocketException专门装“网络连接失败原因”的异常类
        //常见的还有IOException,SerializationException等(都是Exception的子类)
        catch (SocketException ex)
        {

            if (ex.ErrorCode == 10061)
            {
                //10061固定错误编号,代表目标计算机拒绝连接
                print("服务器服务器拒绝连接");
            }
            else
            {
                //这里字符串拼接自动调用.ToString(),其实不写也行
                print("连接失败" + ex.ErrorCode.ToString());
            }

            return;
        }

        //3.收发消息
        //客户端则只需要一个Socket对象,因为服务器端口需要总机和分机(一个负责迎宾,一个负责交流)
        byte[] result = new byte[1024];
        int len = socketTcp.Receive(result);
        print("收到服务器发来消息" + Encoding.UTF8.GetString(result, 0, len));
        socketTcp.Send(Encoding.UTF8.GetBytes("客户端发送给服务器的消息"));
        //4.关闭连接
        socketTcp.Shutdown(SocketShutdown.Both);
        socketTcp.Close();
    }
}
```

---

## 二、服务器端：一个「总机」，若干「分机」

服务器端的步骤是固定的 **7 步**：

1. `new Socket(...)` —— 创建套接字
2. `Bind(ipEndPoint)` —— 绑定 IP + 端口
3. `Listen(100)` —— 开始监听（不阻塞）
4. `Accept()` —— **阻塞**，直到有客户端连进来
5. `Send` / `Receive` —— 收发数据
6. `Shutdown(SocketShutdown.Both)` —— 禁用收发
7. `Close()` —— 关闭

其中最容易搞混的是第 4 步：这里出现了**两个** Socket。

<p style="color:#e11d48;font-weight:600">//socketConnect是客户端和服务器的连接通道(分机)</p>

<p style="color:#e11d48;font-weight:600">//socketTcp只是负责调用.Accept(),让客户端来连接然后返回个socketConnect(总机)</p>

可以这样理解：

- `socketTcp` 是**总机**：它只做「接电话」这件事（`Bind` / `Listen` / `Accept`），自己不负责聊天；
- `socketConnect` 是**分机**：`Accept()` 返回它之后，真正的 `Send` / `Receive` 都在它身上发生。

所以 **`Close()` 关的是 `socketConnect`（分机），不是 `socketTcp`（总机）** —— 总机要留着继续接下一个电话。

> `Listen` 只是「告诉操作系统我在这个端口等着」，不阻塞；真正卡住程序的是 `Accept()`。如果没 `Listen` 就 `Accept`，操作系统不知道你要等这个端口，会直接拒绝。

---

## 三、客户端：一个 Socket 就够

客户端的步骤更简单：

1. `new Socket(...)` —— 创建套接字
2. `Connect(ipEndPoint)` —— 主动连接服务器
3. `Receive` / `Send` —— 收发数据
4. `Shutdown` + `Close` —— 关闭

客户端不需要「监听」，因为它不等人来找它，而是自己主动找服务器：

<p style="color:#e11d48;font-weight:600">//客户端则只需要一个Socket对象,因为服务器端口需要总机和分机(一个负责迎宾,一个负责交流)</p>

服务器要同时处理「有人在门口等」和「和已经在聊的人说话」两件事，所以需要总机 + 分机；客户端只做「连上去 → 说话 → 挂断」，一个 Socket 从头用到尾就够了。

---

## 四、发送和接收的顺序要对上

这套代码是「服务器先 `Send`、客户端先 `Receive`」，两边必须**顺序对应**：

| 服务器 | 客户端 |
|--------|--------|
| `socketConnect.Send(...)` | `socketTcp.Receive(...)` |
| `socketConnect.Receive(...)` | `socketTcp.Send(...)` |

如果两边都先 `Send`，就会互相等对方的数据，看起来像「卡死」。实际项目里通常约定好协议（谁先发、发多长、要不要带长度头），避免这种互相等待。

> `Receive` 的返回值 `len` 是**实际收到的字节数**，一定要用它来截取 `Encoding.UTF8.GetString(result, 0, len)`，不能直接 `GetString(result)`，否则数组后面那些 0 字节也会被转成字符。

---

## 五、`Accept()` 会阻塞主线程

脚本里注释掉的那段 `Update` 其实说明了一个坑：

```csharp
if (Input.GetKeyDown(KeyCode.E)) SocketServer();   // 里面有 Accept()
else if (Input.GetKeyDown(KeyCode.R)) SocketClient();
```

按下 E 之后，`Accept()` 会**阻塞当前线程**，而 Unity 的输入和主线程是同一个线程——所以这时候再按 R 也没反应，输入也被一起卡住了。

实际项目里应该用：

- **异步**：`AcceptAsync` / `ConnectAsync` / `ReceiveAsync`；
- **协程 + 线程**：把阻塞操作丢到子线程，主线程只负责取结果；
- 或者用 `TcpListener` / `TcpClient` 这类封装好的类。

---

## 六、常见错误码

`SocketException.ErrorCode` 里比较常见的是：

| 错误码 | 含义 | 常见原因 |
|--------|------|----------|
| `10061` | 目标计算机拒绝连接 | 服务器没启动，或没 `Listen` 这个端口 |
| `10048` | 地址已被占用 | 端口被别的程序（或上一次没关的进程）占着 |
| `10060` | 连接超时 | IP/端口不通，或被防火墙拦了 |

代码里对 `10061` 做了单独提示，其余错误码直接打印出来，方便排查。

---

## 七、小结

| 角色 | Socket 数量 | 关键流程 |
|------|-------------|----------|
| 服务器 | **2 个**（`socketTcp` 总机 + `socketConnect` 分机） | `Bind` → `Listen` → `Accept` → `Send`/`Receive` → `Shutdown` → `Close` |
| 客户端 | **1 个** | `Connect` → `Receive`/`Send` → `Shutdown` → `Close` |

> 一句话记忆：**总机负责接，分机负责聊；客户端自己找上门，一个就够。**
