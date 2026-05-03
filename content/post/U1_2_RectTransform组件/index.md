+++
title = "U1 2 RectTransform组件"
date = "2026-05-03T10:20:03+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 概念
  - 继承于Transform专门处理UI元素位置和大小
  - Transform组件只处理位置角度缩放
    - RectTransform 额外加入中心点,锚点,长宽等属性,把UI当做矩形处理
- 面板
  - `Pivot(轴心点)`
    - (0,0)到(1,1),决定中心点位置
      - 视口坐标系那种
    - 比如旋转/缩放/对齐时的中心点,也用于配合Anchors
  - `Anchors(相对父矩形锚点)`
    - 就是修改Rect的那个※准星,决定四条边相对父矩形的位置
      - Canvas 本身 **没有锚点（Anchors）** ，也不支持设置相对父矩形的锚点
    - <mark style="background-color:#fef3c7;">比如X设置为0.1~0.9,那就是相对父矩形左右边都缺少了0.1</mark>
      - 如果是Canvas->img1->img2
      - 你设置img2就是相对于img1的,而不是爷爷Canvas
    - 当锚点是一个点时
      - <mark style="background-color:#fef3c7;">准星位置就是以锚点为原点的XY坐标系显示</mark>
      - 比如用于分辨率自适应,锚点放左下角,准星位置都相对于这个坐标系
    - 当锚点是一个矩形
      - 此时准星位置变化,不是PosX/Y而是Left/Top...
      - 变成了锚点相对父矩形的那种,Left的值就是和父矩形Left边的距离
      - 比如用于背景图,当父矩形变大,该image也变大(因为四条边之间的距离是固定的)
        - 不过会导致图像大小拉伸
        - 一般还是通过锚点是点直接对齐,image大小不会变
    - 当锚点是一条线
      - 比如X方向锚点重合,Y方向锚点没重合,那就是坐标/距离混合使用
  - ![image-1](https://document-image.mubu.com/document_image/32569566_c16db708-3006-447d-9dac-a90e6a4f6d85.png)
    - `Blueprint Mode（蓝图模式）`
      - 旋转缩放图形时,对应的矩形不会变,一般不勾选
    - `Raw Edit Mode（原始编辑模式）`
      - 原本修改轴心位置,相对锚点坐标轴变化,相当于Rect的位置也变化
      - 启用后，改变轴心和锚点值不会改变矩形位置
  - ![image-1](https://document-image.mubu.com/document_image/32569566_2c28d371-2c60-430b-8824-8772c2e8f0ba.png?x-tos-process=image/resize,w_112)
    - 九宫格布局快速设置锚点
    - 按住 Shift 点击鼠标左键可以同时设置轴心点（相对自身矩形）
      - 注意轴心不一定和锚点重合,是各自的位置
    - 按住 Alt 点击鼠标左键可以同时设置位置
- 脚本
  - 获取组件(父类容器)
    - `Transform rectTransform = GetComponent<RectTransform>();`
