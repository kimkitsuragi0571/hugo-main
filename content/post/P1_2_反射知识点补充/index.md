+++
title = "P1 2 反射知识点补充"
date = "2026-05-03T10:20:03+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 知识点回顾
  - ![image-1](https://document-image.mubu.com/document_image/32569566_a759ecf9-6667-4069-e645-d9adc9da44b4.png?x-tos-process=image/resize,w_400)
- 知识点补充
  - 注意要using system;
  - ![image-1](https://document-image.mubu.com/document_image/32569566_93ec1027-6726-4c31-879f-d47976dd54f8.png?x-tos-process=image/resize,w_400)
    - 比如判断Person父类是否可以让Student子类为自己分配空间
      - 结果是可以(子类对象可以复制给父类变量)
    - <mark style="background-color:#fde8e8;">就是通过反射判断容器能不能装对象(只有父类容器能装子类对象)</mark>
      - <mark style="background-color:#fde8e8;">大部分场景下就是判断A是不是B的父类</mark>
      - <mark style="background-color:#fde8e8;">也可以判断是不是同一个类型/接口继承/Object兼容</mark>
    - <mark style="background-color:#fef3c7;">如果可以装就创建子类实例,装入父类容器</mark>
  - ![image-1](https://document-image.mubu.com/document_image/32569566_17928af0-5b57-4dfc-900a-e14f91cbba98.png?x-tos-process=image/resize,w_400)
    - 通过这个方法就可以获取List补全的泛型是什么
    - Dic同理,键和值分别是数组0,1
