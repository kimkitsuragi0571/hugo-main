+++
title = "7 C#进阶 反射"
date = "2026-05-03T10:20:02+08:00"
draft = false
categories = ["C-Sharp"]
tags = ["Notes"]
+++

- 概念
  - 程序集
    - ![image-1](https://document-image.mubu.com/document_image/32569566_989fe169-e18d-4ad7-ae19-1491c280512f.png?x-tos-process=image/resize,w_400)
  - 元程序
    - ![image-1](https://document-image.mubu.com/document_image/32569566_5e02bfd7-b520-4981-f24a-5aa03b6c5654.png?x-tos-process=image/resize,w_400)
  - 反射
    - ![image-1](https://document-image.mubu.com/document_image/32569566_a0613803-5df8-43de-e0c6-77a4967ae03b.png?x-tos-process=image/resize,w_400)
      - 就是得到其他程序集的代码信息
  - 反射的作用
    - ![image-1](https://document-image.mubu.com/document_image/32569566_328a8321-565c-43db-cd14-aadaf8d5b66b.png?x-tos-process=image/resize,w_400)
- 语法
  - Type
    - ![image-1](https://document-image.mubu.com/document_image/682a76ad-6cb6-4944-b398-18ab7638cf96-32569566.jpg?x-tos-process=image/resize,w_400)
      - 说人话就是用来获取类的信息
    - 获取Type
      - ![image-1](https://document-image.mubu.com/document_image/32569566_f3306c63-c001-4b3c-85d1-de13d81c662f.png?x-tos-process=image/resize,w_400)
        - 发生了装箱,这里就相当于Object.GetType()
      - ![image-1](https://document-image.mubu.com/document_image/32569566_2effa88d-cd07-4cc5-da75-1171bb6f1ee9.png?x-tos-process=image/resize,w_400)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_f986a819-9979-447e-9957-291c0047e91a.png?x-tos-process=image/resize,w_400)
        - 传入的是类名Int32(不是自己取了叫个Int32的类)
        - 前面还要有个命名空间.
      - 啊,3个type指向的都是同一块内存
        - int/System.Int32 在整个程序域中只有一份类型元数据
        - 相当于int类型变量是实体,int元数据是图纸
          - 不是一个东西
    - 得到类的程序集信息
      - ![image-1](https://document-image.mubu.com/document_image/32569566_ae05085d-991e-4604-dad2-46082e1103cd.png?x-tos-process=image/resize,w_400)
        - 程序集元数据,比起类型原数据更加高级的元数据
    - type获取
      - (空)
        - Test是自定义的一个类
        - 需要引入反射命名空间
        - 类中的私有成员是得不到的
        - 这里就会把父类Object和自身的所有公共成员的元数据打印出来
      - ![image-1](https://document-image.mubu.com/document_image/32569566_8e2d62c5-341e-4339-b85b-adc4c0801746.png?x-tos-process=image/resize,w_400)
        - ![image-1](https://document-image.mubu.com/document_image/32569566_5124404c-25bc-4f95-a3ec-1bbe47bdbcaf.png?x-tos-process=image/resize,w_259)
        - ctor代表构造函数
      - ![image-1](https://document-image.mubu.com/document_image/58359607-0944-4c7e-9711-940a92724a30-32569566.jpg?x-tos-process=image/resize,w_400)
        - ![image-1](https://document-image.mubu.com/document_image/32569566_613cbe5f-5b36-46af-ddca-fda04dd2804c.png?x-tos-process=image/resize,w_400)
        - ctor按tab自动打印构造函数
      - ![image-1](https://document-image.mubu.com/document_image/32569566_5ea58342-7fc8-4b5b-a44a-9315ca9d8892.png?x-tos-process=image/resize,w_400)
        - 啊???????完全看不懂
        - 后面再来看吧,在反射的30分钟处
      - 获取类的公共成员变量
        - ![image-1](https://document-image.mubu.com/document_image/32569566_05d63372-f4aa-4a20-e8db-2facb6b7c906.png?x-tos-process=image/resize,w_400)
        - ![image-1](https://document-image.mubu.com/document_image/32569566_61a5c531-c5e4-48db-c0b9-055c1abd8829.png?x-tos-process=image/resize,w_400)
        - ![image-1](https://document-image.mubu.com/document_image/32569566_902c6ebf-dc94-4423-b3f2-5b7d5dbe7f16.png?x-tos-process=image/resize,w_400)
      - 获取类的公共成员方法
        - ![image-1](https://document-image.mubu.com/document_image/32569566_01710789-f69b-401e-b713-4481e86e3382.png?x-tos-process=image/resize,w_400)
        - ![image-1](https://document-image.mubu.com/document_image/32569566_a0c53d4f-1ba7-427e-f6d8-e889f50cf5d6.png?x-tos-process=image/resize,w_400)
- 反射两个关键类
  - Activator
    - ![image-1](https://document-image.mubu.com/document_image/32569566_7b767391-1385-485d-bd9f-4aa091ddb30d.png?x-tos-process=image/resize,w_400)
    - 上节实例对象需要先获取再执行一个构造函数,很麻烦
    - ![image-1](https://document-image.mubu.com/document_image/32569566_75c853a7-7d0b-452c-c495-4f481b2f4bc6.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_324bd606-267b-48d6-8383-dccfd5e3e8c9.png?x-tos-process=image/resize,w_381)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_3c1d9af6-f017-4ea8-cb2a-594ff2fc41ad.png?x-tos-process=image/resize,w_400)
      - 没有对应的构造函数,报错
  - Assembly
    - ![image-1](https://document-image.mubu.com/document_image/32569566_2beabf2b-5b55-4e32-d4bb-65de32f1b0be.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_2cc9dcf7-45dc-4844-e862-fbaabe1ef309.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_3cfa7373-005e-46e2-824c-ea2b69d1ac8b.png?x-tos-process=image/resize,w_400)
      - 直接复制粘贴报错,因为\是转义字符
      - 这里需要\转化,或者直接字符串前面加个@
