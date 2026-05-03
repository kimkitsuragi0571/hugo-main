+++
title = "U2 6 必备知识点补充"
date = "2026-05-03T10:20:03+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 1.场景切换和游戏退出
  - ![image-1](https://document-image.mubu.com/document_image/32569566_f5270114-1bc4-4343-f0a1-41c843c4134b.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_eb35c5e0-bccc-47b1-af34-7b4c62bffd8f.png?x-tos-process=image/resize,w_400)
    - 老版本的东西
  - 退出游戏
    - ![image-1](https://document-image.mubu.com/document_image/32569566_11c7bf9f-b36d-4290-df44-8bfb1b3c4093.png?x-tos-process=image/resize,w_400)
      - 发布游戏过后才有用
- 2.隐藏鼠标和锁定相关
  - ![image-1](https://document-image.mubu.com/document_image/32569566_bf86e7eb-831d-4cc8-d7d9-8420e1ed7236.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_3d2e200d-9b2a-4d0c-81cd-699a74000a47.png?x-tos-process=image/resize,w_400)
    - 找正方形图片才不会变形
    - 材质格式改成Cursor
- 3.随机数和Unity自带委托相关
  - ![image-1](https://document-image.mubu.com/document_image/32569566_2307ffd3-997b-4290-cceb-9dbebb30a1cd.png?x-tos-process=image/resize,w_400)
    - Unity和C#中的Random类所处命名空间不同,不是同一个
    - float重载就是会包含上下限的数字,相当于[]
    - 用C#中随机数必须指定命名空间
      - 但是你这里不能using System
      - 引入有重名类的命名空间,导致报错
      - 只能直接System.Random
    - 好吧Unity里也不咋用C#的随机数
  - Unity自带委托
    - ![image-1](https://document-image.mubu.com/document_image/32569566_e660f082-b512-4b0a-9281-731c1ab20cb2.png?x-tos-process=image/resize,w_374)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_b260f05c-0664-4fde-f3b8-820ddc95ccda.png?x-tos-process=image/resize,w_400)
- 4.模型资源导入
  - ![image-1](https://document-image.mubu.com/document_image/32569566_927b0f58-f749-42da-e048-32e3e7a358a1.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_e9fdd16e-9d72-4ba1-8bec-dbc42cc73ca5.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_b1582aaf-0367-45c9-f808-a88b3c2cbe00.png?x-tos-process=image/resize,w_400)
