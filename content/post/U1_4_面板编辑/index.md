+++
title = "U1 4 面板编辑"
date = "2026-05-03T10:20:03+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- (空)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_3c829c00-e4bc-4545-cf4d-8406bf7bab40.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_6fdcf59a-45a6-45d5-b0ce-5c44b5798585.png?x-tos-process=image/resize,w_194)
      - 运行的时候通过反射获取变量名然后修改
    - 继承Mono的类没法用构造器
      - 这里序列化面板修改算是替代了初始化的作用
    - 序列化:内存对象变成可保存数据
      - 比如设置HP=100,只存放在内存中,游戏一关就没了
      - 序列化就是将其写为二进制保存进文件中
      - **序列化 = 存盘**
        - **反序列化 = 读档**
    - 为什么 [SerializeField] 可以序列化 private 变量
      - Unity 的序列化根本不遵守 C# 的访问权限
        - public 字段 → 默认序列化
        - private/protected → 不序列化
      - 序列化private的好处
        - 代码层面：安全，外部不能改
        - 编辑器层面：可见、可改、可保存
    - Unity 序列化时，会检查你有没有加 [HideInInspector]，查到了就不把这个字段放进 Inspector 绘制列表
    - 数据结构可以实例化,逻辑不能实例化
      - ![image-1](https://document-image.mubu.com/document_image/32569566_7429b7e3-df86-4d51-ac60-5760cf7db9aa.png?x-tos-process=image/resize,w_259)
      - 字典主要就是逻辑太复杂了,存起来容易出错
        - 常用解决方案是拆分为两个 List 模拟，或使用 Odin、新版 SerializedDictionary(以前用过的可序列化字典)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_685a2533-5491-41d6-dcfc-73f1c3345902.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_03023596-6119-4ace-f328-e1a63e942159.png?x-tos-process=image/resize,w_175)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_7dda8163-0fa0-4d11-c528-b2b96da9c438.png?x-tos-process=image/resize,w_253)
      - 字典不能被序列化
      - 结构体/类加上序列化就需要指定命名空间
        - 这里写了自定义类/结构体也无所谓,只要继承了Mono的类和文件名相同就可以
          - 不存在Unity找不到错乱
        - 注意是System.Serializable,Unity中并没有默认引入using System
  - 辅助特性
    - ![image-1](https://document-image.mubu.com/document_image/32569566_0f499829-d0fd-447d-b6b9-f1ac9509137f.png?x-tos-process=image/resize,w_280)
      - 面板上不至于所有属性乱成一团
    - ![image-1](https://document-image.mubu.com/document_image/32569566_11964a21-72cd-42c3-e481-d5403710e3ca.png?x-tos-process=image/resize,w_280)
      - 悬停文字
      - 行间距
    - ![image-1](https://document-image.mubu.com/document_image/32569566_97799e58-3456-4512-eb7e-98185b750bd4.png?x-tos-process=image/resize,w_284)
      - 从手动输入数值改为滑条辅助指定
      - 好吧,填入数值那个界面也能直接拖动,这里意义不大
    - ![image-1](https://document-image.mubu.com/document_image/32569566_4553ef53-ac48-4ef3-aad0-863e0ba78739.png?x-tos-process=image/resize,w_290)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_c94f13a5-e11b-455c-e86a-050faaf50c22.png?x-tos-process=image/resize,w_295)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_04f1b167-68fd-46f4-a822-39e11e092cf2.png?x-tos-process=image/resize,w_295)
      - 挺有用的
      - 后面一个参数名填方法名
        - 必须有对应同名的无参无返回值方法
        - 面板点击按钮就会执行这个方法
    - ![image-1](https://document-image.mubu.com/document_image/32569566_672e067a-6faa-4deb-f008-563a7f957fb6.png?x-tos-process=image/resize,w_295)
      - 点击脚本右边的...就能执行这个函数
      - 主要是测试用的
  - ![image-1](https://document-image.mubu.com/document_image/32569566_61cb62b4-1896-4938-d865-8a22bc3ce705.png?x-tos-process=image/resize,w_400)
    - 1.修改面板中的i就是在修改脚本中的变量i本体,不存在什么复制体两个变量
    - 2.这里把我脚本拖拽到GameObj上,脚本上修改默认值为200,但是面板上i仍然为100
      - 相当于拖拽到Obj上之后就是个独立的脚本了
    - 3.运行途中手动把i改为999,运行完毕还是会变回100
      - 运行期修改无法保存
      - 额,介绍了种比较笨的方法保留变化,感觉没用
        - 运行期点击复制脚本
