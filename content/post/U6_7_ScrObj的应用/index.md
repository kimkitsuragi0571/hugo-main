+++
title = "U6 7 ScrObj的应用"
date = "2026-05-03T10:20:04+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 配置数据
  - ![image-1](https://document-image.mubu.com/document_image/32569566_5462fd58-e23d-4196-ddf8-34927a508dc6.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_24d44e10-0faf-4adb-cdc5-50b89f3d9436.png?x-tos-process=image/resize,w_400)
    - 配置文件用List形式记录
    - 这个序列化的部分在入门里面,我居然全忘了
  - `public RoInfo info;`
    - 直接拖动挂载即可
  - <mark style="background-color:#fff3bf;">配置文件的作用在之前的数据持久化和实践项目里面搞过,后面来看把</mark>
- 复用数据
  - 预设体存在的问题
    - 比如预设体子弹,那么每个实例化子弹都会有脚本
      - 但是这种只读不会改的脚本有一份就够了
      - 当然,对于需要更改数据的脚本,每个实例一份才是对的
  - 使用ScrObj
    - ![image-1](https://document-image.mubu.com/document_image/32569566_c2ff816c-d8af-47ce-c905-2c0d49233c69.png?x-tos-process=image/resize,w_400)
      - 三个子弹就是共用一份数据(游戏发布后也一样)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_89d08a30-c2d3-4bbd-bd61-0492b30d41b2.png?x-tos-process=image/resize,w_400)
- 数据带来的多态行为
  - ![image-1](https://document-image.mubu.com/document_image/32569566_85484ba3-d66c-4783-a6c2-c5933f142648.png?x-tos-process=image/resize,w_400)
    - 依赖倒转原则就是依赖于其抽象而不是实现
  - ![image-1](https://document-image.mubu.com/document_image/32569566_4e98161c-bd2c-4ad6-a4f9-c0a86209618d.png?x-tos-process=image/resize,w_400)
    - AudioPlayBase基类脚本中不需要写逻辑,直接抽象类都行
    - 子类继承然后实现具体逻辑就行
  - ![image-1](https://document-image.mubu.com/document_image/32569566_c337a16b-986a-41c0-a478-bc59f38db788.png?x-tos-process=image/resize,w_400)
    - 基类中不需要声明特性
  - `public AudioPlayBase audioPlay;`
    - 在需要实现功能的脚本中声明对象即可
  - ![image-1](https://document-image.mubu.com/document_image/32569566_86deeaf9-381e-489a-b8eb-439b16fe4878.png?x-tos-process=image/resize,w_400)
    - 不同的子类实现的功能不同,通过配置不同的数据文件就可以了
  - 如果是拾取功能
    - ![image-1](https://document-image.mubu.com/document_image/32569566_2bee83e8-e480-4f25-8947-a0eaf068ef60.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_97392b3d-887e-41cb-e9a6-9607dee54929.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_be76d006-5b19-4ed9-c006-e7fcf3f4265c.png?x-tos-process=image/resize,w_400)
- 单例模式化获取数据
  - ![image-1](https://document-image.mubu.com/document_image/32569566_9ce70439-099e-48e5-812f-b8fafdef8734.png?x-tos-process=image/resize,w_400)
    - 给 ScriptableObject 做一个 “单例基类”，以后拿数据就不用再手动拖引用、写加载代码了，直接一行代码就能拿到数据。
  - ![image-1](https://document-image.mubu.com/document_image/32569566_03449ace-6219-4715-b429-b9a418c862a4.png?x-tos-process=image/resize,w_400)
  - <mark style="background-color:#fff3bf;">哎呀我日死你的哥,这里也是默认你学过单例模式,之后再看吧</mark>
