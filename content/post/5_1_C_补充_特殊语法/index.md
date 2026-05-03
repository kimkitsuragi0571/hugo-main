+++
title = "1 C#补充 特殊语法"
date = "2026-05-03T10:20:03+08:00"
draft = false
categories = ["C-Sharp"]
tags = ["Notes"]
+++

- var隐式类型
  - 不能作为类成员,只能写在函数中
  - var必须初始化
    - 根据初始值类型来设定自己的类型,且之后不能更改
    - var age =11;之后age="jackie"就不行
  - var属于编译时确定类型，dynamic属于运行时确定类型
- 匿名类型
  - ![image-1](https://document-image.mubu.com/document_image/32569566_ae2c118b-d269-4261-911d-7a83e4d19e67.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_6a5f72b2-6f25-466f-f7e0-9ba69031219a.png?x-tos-process=image/resize,w_400)
    - 不能有函数内容,只能有成员变量
- 初始化器
  - 设置对象初始值
    - 不是通过构造函数,而是直接对公共成员/属性初始化
      - ()构造器,{}初始化器
    - 不需要将所有成员变量都写完
    - ![image-1](https://document-image.mubu.com/document_image/32569566_14f15b65-a957-4b66-d506-b36be7814be7.png?x-tos-process=image/resize,w_400)
      - 既有构造函数,又有初始化
      - 此时就是优先执行构造函数
  - 设置集合初始值
    - ![image-1](https://document-image.mubu.com/document_image/32569566_3a1d9818-3ad2-471e-84b7-5ec147c37550.png?x-tos-process=image/resize,w_400)
- 可空类型
  - ![image-1](https://document-image.mubu.com/document_image/32569566_57600e6a-363c-4fa4-f6de-8622fb4b5f2f.png?x-tos-process=image/resize,w_400)
    - 不指定默认值就是0
    - 括号里指定100
      - 有默认值就返回默认值
      - 没指定就返回括号中的100
        - 实际并没有给value赋值
  - 引用类型也可以用可空
    - ![image-1](https://document-image.mubu.com/document_image/32569566_2b3cae15-8e14-4e09-e1b4-a234adde39f0.png?x-tos-process=image/resize,w_182)
      - 直接调用引用类型对象的成员方法,那对象不能为空
      - 下面这个用可空对象调用就不报错
        - 当对象为空的时候就不会执行方法
        - 不是说用这个就能执行空对象的方法了哈
- 内插字符串
  - 就是让字符串中可以拼接变量
  - ![image-1](https://document-image.mubu.com/document_image/32569566_33b69ea4-9f3a-4feb-ae94-0cc8f651765a.png?x-tos-process=image/resize,w_400)
