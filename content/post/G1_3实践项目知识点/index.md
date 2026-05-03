+++
title = "G1 3实践项目知识点"
date = "2026-05-03T10:20:03+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 九宫格布局
  - ![image-1](https://document-image.mubu.com/document_image/32569566_79fcb201-8160-4d79-e0d1-fe70b2a463ad.png?x-tos-process=image/resize,w_400)
    - 都是基于左上角(0,0)来表示坐标
    - 相对屏幕位置:比如以右下角点作为参考点,设定控件位置(-1,-1),那它实际位置就是(W-1,H-1),从而实现不同分辨率下自适应位置
    - 分辨率自适应公式看图
  - ![image-1](https://document-image.mubu.com/document_image/32569566_d7fe7c73-81e4-4a83-e17e-6ea738e0f26a.png?x-tos-process=image/resize,w_400)
    - 除了屏幕,控件也可以设置九宫格,左上角(0,0,0)
      - 也就是公式里的中心点偏移位置
    - 比如以右下角作为参考点,则位置变成原来的(-w,-h)
  - 前两个设置参考点,最后一个就是偏移量(不是正好在某个点上,就有偏移)
- 控件位置信息
  - 相对屏幕位置+中心点偏移量+偏移位置
    - 相对屏幕位置:控件基准点 相对于 屏幕的位置
    - 中心点偏移量:控件左上角 相对于 控件基准点 的偏移
  - ![image-1](https://document-image.mubu.com/document_image/32569566_e4de4488-53cd-4119-b07f-0a82ab4c8caa.png?x-tos-process=image/resize,w_163)
    - 对齐方式设置个枚举
  - ![image-1](https://document-image.mubu.com/document_image/32569566_fa4693d3-eed3-42f9-d565-4e703ef991d2.png?x-tos-process=image/resize,w_400)
    - 数据类,不需要继承mono
    - 传入"相对参考点局部坐标"+相对偏移的参考点,返回"相对整个屏幕九宫格(0,0)的世界坐标系"
  - ![image-1](https://document-image.mubu.com/document_image/32569566_5b6ac235-53a2-4621-cfcb-c1ffc24f9c92.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_8e01bbbf-6d84-4220-9f59-a0cef70a4c9f.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_dccf36ca-4f18-40a7-c8f9-3ecf150287f8.png?x-tos-process=image/resize,w_207)
    - 大小没有偏移,直接返回
- 控件父类
  - ![image-1](https://document-image.mubu.com/document_image/32569566_d6ded99f-69ac-4776-e7f1-a68963a75251.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_7d2a858f-7aa8-4a14-caa7-9fe1f3179dbb.png?x-tos-process=image/resize,w_400)
    - 父类写管理逻辑,子类中写绘制逻辑
    - GUI绘制顺序(按钮的层级)不能控制,也没法非运行的时候调试
      - 下一个脚本就解决这个
    - 写成抽象类,当子类继承父类的时候重写抽象方法
- 控件父类的问题修改
  - ![image-1](https://document-image.mubu.com/document_image/32569566_e9736c51-5370-4760-adbd-10f487419d37.png?x-tos-process=image/resize,w_400)
    - 通过场景中顺序实现层级依次排列
    - 加特性实现非运行时候执行脚本
    - OnGUI循环执行,每次循环都要获取脚本
      - 编辑状态下要不断变化,start只执行一次,所以这里还只能这样
      - 加个if优化,当游戏没有运行时才重复执行
        - 好吧似乎会导致没法隐藏面板,最后还是只能隐藏if
    - 因为我们具体控件实现都放在子类中,所以这里获取子类的组件
- 自定义文本
  - ![image-1](https://document-image.mubu.com/document_image/32569566_82a612b2-f383-4cf0-cebd-0cf30a867f5c.png?x-tos-process=image/resize,w_400)
    - 继承抽象父类,实现其中逻辑
    - 场景中创建Root然后挂载父类Root脚本
    - 子对象创建Label,挂载该脚本
    - Root和Label各自做成预设体,父子关系挂载即可
- 自定义按钮
  - ![image-1](https://document-image.mubu.com/document_image/32569566_4d164ddd-015a-4f46-cf30-a2524a401368.png?x-tos-process=image/resize,w_373)
    - 创建事件或者委托都可以(这里事件是基于Unity自带委托)
    - 外部+=添加函数就是了,点击就会Invoke执行
- 自定义多选框
  - ![image-1](https://document-image.mubu.com/document_image/32569566_6cd48150-e754-4432-dd87-119b33fe9b5c.png?x-tos-process=image/resize,w_392)
    - 只有改动的时候才提示执行函数
- 自定义单选框
  - ![image-1](https://document-image.mubu.com/document_image/32569566_fea29a9e-d155-4fdf-86bc-dd5117ab2d50.png?x-tos-process=image/resize,w_230)
    - 作为管理其他Toggle的存在,不需要继承抽象父类
    - lambda表达式的逻辑只是存到了事件变量中,当满足变化的时候才会执行
    - Toggle放在分组里面,就可以设定为单选框只能选其中一个
- 自定义输入框
  - ![image-1](https://document-image.mubu.com/document_image/32569566_924a3c14-4116-4714-b1d5-38766ea44ade.png?x-tos-process=image/resize,w_400)
- 自定义拖动条
  - ![image-1](https://document-image.mubu.com/document_image/32569566_5b8c7cd1-bba9-44cc-c38d-9da03a74f215.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_cd80561a-8f81-418b-e2e3-12b9b90a9180.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_79ec0200-b713-4e5c-9bfe-860c1a61040d.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_71b3ef31-2c15-4a57-9d60-b60be527027d.png?x-tos-process=image/resize,w_400)
- 自定义图片绘制
  - ![image-1](https://document-image.mubu.com/document_image/32569566_893e7ef6-3bfa-4aef-a5ab-64ef4cccfbdb.png?x-tos-process=image/resize,w_400)
