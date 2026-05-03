+++
title = "U1 2 反射,预制体"
date = "2026-05-03T10:20:03+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 反射
  - 概念
    - <mark style="background-color:#fef3c7;">物体挂载脚本 = 实例化一个脚本对象，并把这个对象和 GameObject 绑定关联</mark>
      - GameObject相当于个容器,初始一定关联了Transform
  - 作用
    - 当修改面板组件值
      - ![image-1](https://document-image.mubu.com/document_image/32569566_bd1f63e8-a9b3-4117-fb26-2bc3349529cb.png?x-tos-process=image/resize,w_400)
      - 就是不停利用SetValue,反射更改值
    - 当拖动挂载脚本
      - ![image-1](https://document-image.mubu.com/document_image/32569566_f0253613-cd32-484f-cc2d-628978c15a07.png?x-tos-process=image/resize,w_400)
      - 挂载的时候得到了脚本名字(和脚本中类名相同)
        - 从而实现反射获取公共成员
    - 当创建不同场景
      - 1.一个场景拖动到另一个场景中,可以两个场景叠加显示
      - 2.场景的实质:记录场景安排的配置文件
        - 编辑器读取配置文件后创造场景
- 预制体
  - 原理同样是配置文件
  - 场景中修改预制体
    - <mark style="background-color:#fef3c7;">1.面板中点击override,保留对预制体的修改</mark>
      - Revert All就是重置所有变化
      - Apply All就是应用所有变化
    - 2.open进入预设体专用场景(那个蓝色的)
    - 3.直接再次拖进文件夹
  - 破话预制体就是Unpack...
    - 此时再拖进文件夹就是另一个预设体
- 资源包Package
  - 就是文件管理导入导出资源包
  - import和export
