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
