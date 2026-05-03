+++
title = "8 C#进阶 特性"
date = "2026-05-03T10:20:02+08:00"
draft = false
categories = ["C-Sharp"]
tags = ["Notes"]
+++

- 概念
  - ![image-1](https://document-image.mubu.com/document_image/08be0de9-8b6a-438b-b20b-aeb57acc1318-32569566.jpg?x-tos-process=image/resize,w_400)
    - 反射获取类元数据(类+类成员)
    - 特性添加元数据信息
      - 是否被遗弃,是否可序列化
    - 常用特性
      - [SerializeField]
        - 是否可序列化
      - [header("")]
        - 编辑器面板上显示灰色标题文字
- 自定义特性
  - 声明
    - ![image-1](https://document-image.mubu.com/document_image/32569566_8fe9a4b7-1e7e-4734-bd58-094cd87fe6b8.png?x-tos-process=image/resize,w_261)
      - 特性里面最常用的就是字段+构造函数
      - 特性就是类继承了Attribute,所以类成员都能写
  - 调用
    - ![image-1](https://document-image.mubu.com/document_image/32569566_6309c9b9-9576-4d3a-cabe-bdd093f9a0f0.png?x-tos-process=image/resize,w_261)
      - 这里引用特性实质就是在调用构造函数
      - 对字段,成员方法,属性都可以用特性
    - 如果给成员加上特性
      - ![image-1](https://document-image.mubu.com/document_image/32569566_9820d9d5-91aa-4e5e-dfce-e4ce3d8904b1.png?x-tos-process=image/resize,w_400)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_133d9fe4-e87d-4411-d8e7-8ef47626872f.png?x-tos-process=image/resize,w_400)
        - 这里用.NET内置的Required特性
        - 标注成员,就会检查只有当不是NULL且不是空字符串时,才会返回True
        - 这个(ErrorMessage = "不能为空)是框架/反射代码用
          - 总之这里特性内部没有相关逻辑
  - 用反射判断/获取类自定义特性
    - ![image-1](https://document-image.mubu.com/document_image/32569566_c8fa9b32-a5d4-4cff-fac2-03c031f2b337.png?x-tos-process=image/resize,w_400)
      - 判断
        - typeof(StuInfo)
          - 检查的反射类型名称
        - false
          - 不检查其父类
      - 获取
        - 把贴在类上的「特性标签」拿下来，变成一个真实可用的对象传给attr
        - GetCustomAttribute用于获取贴的特性
          - typ // 参数1：从哪个类上取？
          - typeof(StuInfo)  // 参数2：取哪个特性？
    - 一次性获取多个特性
      - ![image-1](https://document-image.mubu.com/document_image/32569566_e2983698-14b4-4254-f26b-d5604bb2560a.png?x-tos-process=image/resize,w_400)
  - 限制自定义特性的使用范围
    - ![image-1](https://document-image.mubu.com/document_image/32569566_4797c065-eb5c-46ca-c268-a67fba62c59c.png?x-tos-process=image/resize,w_400)
  - 系统自带特性
    - 过时标签
      - `[Obsolete("此方法已过时，请使用NewSpeak()方法！")]`
      - 提醒类已经过时,使用会报错
    - 调用者信息特性
      - ![image-1](https://document-image.mubu.com/document_image/32569566_eda8cb75-476f-425e-d034-a5c1c4907208.png?x-tos-process=image/resize,w_332)
    - 条件编译特性
      - 根据编译条件，决定方法是否被调用,相当于给方法加了开关
      - ![image-1](https://document-image.mubu.com/document_image/32569566_a8c99792-62ee-45a2-b6f2-87c24a62102d.png?x-tos-process=image/resize,w_400)
    - 外部dll函数特性
      - C# 本身无法直接调用 C++ 写的 DLL 函数，[DllImport]是桥梁
      - ![image-1](https://document-image.mubu.com/document_image/32569566_7908b446-a670-4741-d4a5-5b02a90ed836.png?x-tos-process=image/resize,w_400)
