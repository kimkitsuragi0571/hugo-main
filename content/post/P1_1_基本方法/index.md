+++
title = "P1 1 基本方法"
date = "2026-05-03T10:20:03+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 概念
  - ![image-1](https://document-image.mubu.com/document_image/32569566_74cbc02b-039c-4a94-b936-3bd0f4bc4cfd.png?x-tos-process=image/resize,w_400)
    - 注意key只能是string
- 基本方法
  - 存储方法
    - ![image-1](https://document-image.mubu.com/document_image/32569566_c07fc7e8-a87e-40fa-d945-68a97610642f.png?x-tos-process=image/resize,w_400)
      - 注意游戏结束才到硬盘,游戏崩溃数据也没了
    - ![image-1](https://document-image.mubu.com/document_image/32569566_24eddae1-9d54-495e-81b6-c3171f45df05.png?x-tos-process=image/resize,w_400)
      - char转为string,bool就通过三位运算符间接存储(总之不专业)
      - double还得砍成float才能存储
      - Int和float不同类型都用的MyAge键,不存在"重载",只会覆盖原来的值
  - 读取方法
    - ![image-1](https://document-image.mubu.com/document_image/32569566_a72d0d52-dfeb-4b02-87a8-4417ced8ad64.png?x-tos-process=image/resize,w_400)
      - 还没到硬盘的时候也可以读取
      - 找不到就返回第二个参数默认值
        - 初始化表的时候没有数据,总不能空着吧,就可以设置个默认值
  - 其余方法
    - ![image-1](https://document-image.mubu.com/document_image/32569566_692d2352-7b60-451c-d9af-efe07f7ddfad.png?x-tos-process=image/resize,w_269)
      - 主要是判断是否已经有重复key
    - ![image-1](https://document-image.mubu.com/document_image/32569566_a19dcb7e-8d2a-4bd5-c723-110558a0837b.png?x-tos-process=image/resize,w_271)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_e3b028d7-2bbf-4223-9736-cecb7103c1ae.png?x-tos-process=image/resize,w_268)
    - 简单举例使用
      - ![image-1](https://document-image.mubu.com/document_image/32569566_72de151b-416e-43cd-b333-fce5efb68044.png?x-tos-process=image/resize,w_400)
- 不同平台的存储位置
  - <mark style="background-color:#fde8e8;">Windows注册表就是一个统一的文件管理器(键值对版),用于存储几乎所有应用的配置信息</mark>
    - 早期就是每个应用的配置存在各自的.ini文件下面非常混乱
  - ![image-1](https://document-image.mubu.com/document_image/32569566_d4c62718-66cd-4f41-f1ae-b0b70054d4dc.png?x-tos-process=image/resize,w_400)
    - 可以在Build Settings->Player里面查看
    - ![image-1](https://document-image.mubu.com/document_image/32569566_24142711-97b7-42e7-a764-02c74187bff4.png?x-tos-process=image/resize,w_400)
    - <mark style="background-color:#fde8e8;">Unity下面的注册表,就有对应项目名和公司名的文件夹</mark>
      - <mark style="background-color:#fde8e8;">注意新版的位置变化了计算机</mark><mark style="background-color:#fde8e8;">\</mark><mark style="background-color:#fde8e8;">HKEY_CURRENT_USER</mark><mark style="background-color:#fde8e8;">\</mark><mark style="background-color:#fde8e8;">Software</mark><mark style="background-color:#fde8e8;">\</mark><mark style="background-color:#fde8e8;">Unity</mark><mark style="background-color:#fde8e8;">\</mark><mark style="background-color:#fde8e8;">UnityEditor</mark>
      - WebGL格式数据比较特殊,存储在浏览器缓存中,浏览器关闭就没了
    - 对应项目名字里面就有我们SeInt的键值对
      - 在文件夹里面修改,项目里打印的值也变化(废话)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_ca119617-8178-4fa0-9bcc-2431321b6eee.png?x-tos-process=image/resize,w_400)
- 保证数据唯一性
  - ![image-1](https://document-image.mubu.com/document_image/32569566_9bfe7927-c192-4d3c-8852-1a105ac6378f.png?x-tos-process=image/resize,w_400)
    - 总之就是防止key相同导致被覆盖
- 优缺点
  - ![image-1](https://document-image.mubu.com/document_image/32569566_7211729e-e72b-4959-f46a-bda7b5ad556b.png?x-tos-process=image/resize,w_273)
    - 实践小项目就是为了提升其安全和便利性
  - ![image-1](https://document-image.mubu.com/document_image/32569566_c9a2e253-3dae-4796-b822-71d51f7f3088.png?x-tos-process=image/resize,w_218)
