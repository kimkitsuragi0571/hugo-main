+++
title = "U4 5 Sprite杂项"
date = "2026-05-03T10:20:04+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- SpriteCreator
  - 类似Unity自带几何体,还没有美术图的时候拿这个作为替代资源
  - Asset里面右键创建Sprites就行,有很多形状可以选
    - 形状不同其实就是SpriteEditor里面设置不同边数,能自定义
  - 呃呃,一共就这两句话,没了
- SpriteMask
  - 精灵遮罩,用于只是显示图片的一部分
  - 右键创建SpriteMask就行,或者添加对应组件
  - 只是个框,然后SR里面可以选择MaskInteraciton,在框内外显示
    - 设置圆形头像那种
  - 创建
    - Sprite (遮罩图片)
    - Alpha Cutoff (透明区域分界点)
      - 透明的部分如果透明度<设置值,就不会显示了
      - (总之就是透明->不透明区域的过渡)
    - Custom Range (自定义遮罩范围)
      - 比如场景上有多个遮罩
      - 索引>=Back且<Front的层级才会被遮罩影响
      - +
        - **Sorting Layer（排序层）** ：相当于 **不同的楼层** ，决定谁在 “上层楼” 谁在 “下层楼”
        - **Order in Layer（层内序号）** ：相当于 **同一楼层里的房间号** ，数字越大，越靠上
      - 举例
        - **Front（前面）** ：只遮罩 New Layer 层、Order ≥ 1 的精灵
        - **Back（后面）** ：只遮罩 Default 层、Order ≤ 0 的精灵
- SortingGroup
  - 对多个精灵图分组排序
  - 场景里添加三个游戏对象,均添加SortingGroup组件
    - 然后添加Sprite作为子对象
    - 发现子对象图片仍然受到父对象设定层级的影响
    - 比如设置个UI层的父物体,UI都挂载在上面,从而实现整体都始终置顶
    - 额,这个自己试试就明白了
  - ![image-1](https://document-image.mubu.com/document_image/32569566_9f3121f9-e28a-42f0-928c-1b6d7764e14b.png?x-tos-process=image/resize,w_400)
    - 如果父对象做成预设体,有多个预设体的时候就记得改父物体的OrderInLayer
