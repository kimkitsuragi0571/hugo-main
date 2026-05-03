+++
title = "U1 5 MonoBehaviour"
date = "2026-05-03T10:20:03+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 重要成员
  - ![image-1](https://document-image.mubu.com/document_image/32569566_82905ece-cb77-4032-8f9a-acef2765dbd1.png?x-tos-process=image/resize,w_400)
    - 直接写gameobject也是可以的,这里为了便于理解还是写this.gameobject
    - 同一个脚本挂载的不同物体,这种的执行顺序很难去控制
      - 之前说的设置里面指定的方法似乎也没法改同一个脚本的顺序
    - 获取是否激活
      - 关系到start的执行
  - ![image-1](https://document-image.mubu.com/document_image/32569566_62b5e5e9-7dec-4477-cc97-d3f0d58b3790.png?x-tos-process=image/resize,w_400)
    - 这里要手动挂载游戏物体(算了自己试试吧,这个命名太整蛊了)
- 重要方法
  - ![image-1](https://document-image.mubu.com/document_image/32569566_e460b88a-869a-4359-bf33-e0824a842d33.png?x-tos-process=image/resize,w_400)
    - GetComPonent是个基类,所以这里需要转化为子类
    - 继承了mono就可以通过这个方法检查自己依附的GameObj上还有什么其他的脚本
    - 这里31就是不存在的脚本,返回null
    - 如果挂载了多个脚本,用这个方法是没法知道获取结果是哪个脚本的
  - ![image-1](https://document-image.mubu.com/document_image/32569566_02ab8c43-34b4-48f5-96d5-fd120b44c5e0.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_f6ca470e-1139-4f99-e17f-6616fe5f99a5.png?x-tos-process=image/resize,w_400)
    - 这个还不用转换类型,最常用的一集
  - ![image-1](https://document-image.mubu.com/document_image/32569566_21d682e6-2b01-4cc2-f4d6-6b9cc374fb3f.png?x-tos-process=image/resize,w_400)
    - new的不是 Lesson3实例，而是 List这个容器?
  - ![image-1](https://document-image.mubu.com/document_image/32569566_5cc4976f-b588-46e3-e016-e0af01093854.png?x-tos-process=image/resize,w_400)
    - 参数true就是失活了也能找到,false就是只能找到激活的
  - ![image-1](https://document-image.mubu.com/document_image/32569566_1499d602-ee7d-4cf3-dba5-908fdb9b81e7.png?x-tos-process=image/resize,w_400)
    - 父对象要是失活,子对象始终失活,没法进入生命周期
    - 孙子和爷都一起获取
