+++
title = "6 C#进阶 多线程,预处理"
date = "2026-05-03T10:20:02+08:00"
draft = false
categories = ["C-Sharp"]
tags = ["Notes"]
+++

- 多线程
  - 概念
    - 线程是程序调度的最小单位,一个进程可以并发调度多个线程
    - 多线程:通过代码开启新的线程
      - 各线程之间相互独立,互不干扰
  - 使用
    - ![image-1](https://document-image.mubu.com/document_image/32569566_8e739116-3368-4468-9b7d-62939462732f.png?x-tos-process=image/resize,w_400)
    - `Thread stu = new Thread()`
      - F12点进去是个委托
        - `Thread stu = new Thread(Test)`
        - `static void Test(){ }`
          - 新增的线程就在这个函数中执行,这个函数传入线程类对象
        - `stu.start()`
          - 只是把函数装进线程类并不会触发调用,还要启动
  - 设置为后台线程
    - ![image-1](https://document-image.mubu.com/document_image/32569566_f9f4e896-f3f8-492c-c91c-d42d49c14405.png?x-tos-process=image/resize,w_400)
    - 比如刚才新开线程Test里面无限循环
      - 主线程只有个委托和启动线程
      - 主线程执行完了却永远没法关闭
        - 因为test设置为了前台线程
        - 主线程关闭test也不会关闭
    - `Test.IsBackground = true`
      - 设置为后台进程
  - 关闭一个线程
    - ![image-1](https://document-image.mubu.com/document_image/32569566_492d061f-016d-4a78-8283-ad971f5b7ac0.png?x-tos-process=image/resize,w_400)
      - 如果test中不是死循环,执行结束后回收机制就发力自动回收?
      - 如果test中不是死循环
        - 1.外部添加个bool标识
          - 比如死循环是while(isRun)
          - 外部设置isRun=false让循环停止
        - 2.线程提供方法
          - 有的版本会报错
          - `test.Abort();`
            - 强制关闭,不建议使用
  - 线程的休眠
    - ![image-1](https://document-image.mubu.com/document_image/aaaeb97a-5fd6-41f1-8442-8f459f1dd017-32569566.jpg?x-tos-process=image/resize,w_400)
      - 1s=1000ms
      - `Thread.Sleep(1000)`
        - 让当前线程休眠1000ms
      - ![image-1](https://document-image.mubu.com/document_image/32569566_79aba2a3-901c-4002-f3d7-352777cc7c32.png?x-tos-process=image/resize,w_281)
        - 写在循环里,让轮回的慢一点
  - 线程之间共享数据
    - ![image-1](https://document-image.mubu.com/document_image/32569566_9508f4be-29fc-4521-b557-60bb2b9cb561.png?x-tos-process=image/resize,w_400)
      - 说人话就是要搞互斥访问,通过加锁操作
      - 加锁了以后就阻塞了，前一个线程没执行完之前，后面的线程进不来
    - ![image-1](https://document-image.mubu.com/document_image/32569566_a6f7eeb5-075a-46ae-a276-e0a84ac1ee36.png?x-tos-process=image/resize,w_400)
      - `lock( ){ }`
        - lock锁定当前变量,只有当前lock可以使用,执行完了再释放
        - 进入当前语句前查看传入变量是否已经被其他进程锁定
        - 传入变量obj记住一定是<mark style="background-color:#fef3c7;">引用变量类型</mark>
- 预处理器指令
  - ![image-1](https://document-image.mubu.com/document_image/32569566_87ca96ba-4124-48cc-b88b-46a6ce3af74d.png?x-tos-process=image/resize,w_400)
    - 哦不就是C++里面的#include iostream吗
    - 讲课常用的#region #enderegion 也是
  - (空)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_3d13be13-f2a6-4230-92dd-7c0472b56e59.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_654f4092-045d-452a-9683-eec17cf9bda0.png?x-tos-process=image/resize,w_400)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_138406df-9087-4047-a388-7d39323a8e48.png?x-tos-process=image/resize,w_220)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_2ad3a474-d691-4cb9-e4ec-2846216e877a.png?x-tos-process=image/resize,w_326)
        - #elif 简写的else if
        - ![image-1](https://document-image.mubu.com/document_image/32569566_7aa55b04-239e-411d-f96b-f165322fbefb.png?x-tos-process=image/resize,w_400)
          - 也可以配合逻辑与或
      - 用于不同版本的兼容,编译前就决定执行哪部分代码
  - ![image-1](https://document-image.mubu.com/document_image/32569566_cef6af8a-27f9-4442-9649-7e99171352f1.png?x-tos-process=image/resize,w_218)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_c785565f-70f6-46ae-da6c-4581a03ff5d9.png?x-tos-process=image/resize,w_400)
      - 满足条件:编译之前就会报错
    - 用的很少
