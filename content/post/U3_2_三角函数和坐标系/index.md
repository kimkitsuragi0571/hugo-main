+++
title = "U3 2 三角函数和坐标系"
date = "2026-05-03T10:20:03+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 三角函数
  - 角度弧度转化
    - ![image-1](https://document-image.mubu.com/document_image/32569566_f8498a8d-2fa1-4161-dab2-d6aa872db493.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_b8b9777b-550b-42cd-cb48-b47dac06b8df.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_3f300fbe-9650-4e92-c3ea-ebe1c25b16dd.png?x-tos-process=image/resize,w_500)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_71b4edab-735f-45bd-d3df-63a06f186a7c.png?x-tos-process=image/resize,w_400)
  - 三角函数
    - ![image-1](https://document-image.mubu.com/document_image/32569566_c4ec4cd8-bc34-4d7d-d5a3-45007859c2bd.png?x-tos-process=image/resize,w_400)
      - 肯定不能直接传入sin30度,这里将三十度转化为弧度传入参数
  - 反三角函数
    - ![image-1](https://document-image.mubu.com/document_image/32569566_98a9aca7-cd04-4bc1-a922-b4b7e08566da.png?x-tos-process=image/resize,w_400)
      - 输入1/2,得到对应弧度制
      - 第二句把弧度转化为角度,方便看懂
- 坐标系
  - 视口坐标系
    - ![image-1](https://document-image.mubu.com/document_image/32569566_30d74084-e750-4946-ac63-025b28421fe1.png?x-tos-process=image/resize,w_400)
      - 和屏幕坐标系类似,但是屏幕坐标：像素单位，随分辨率变
      - 视口坐标：0~1 比例，永远不变
  - 代码(大部分是回顾)
    - 世界坐标系
      - ![image-1](https://document-image.mubu.com/document_image/32569566_1848db22-93f9-4052-fc40-09a693c95bff.png?x-tos-process=image/resize,w_400)
    - 物体坐标系
      - ![image-1](https://document-image.mubu.com/document_image/32569566_8902fd49-f2f1-4788-ae04-499e2a2bedad.png?x-tos-process=image/resize,w_400)
    - 屏幕坐标系
      - ![image-1](https://document-image.mubu.com/document_image/32569566_850ede66-24f2-4e1d-cc51-1e9ee60f77aa.png?x-tos-process=image/resize,w_400)
    - 视口坐标系
    - 坐标系转换
      - ![image-1](https://document-image.mubu.com/document_image/32569566_eee2d8ab-1b4f-43e2-f4f5-e82fd1e90260.png?x-tos-process=image/resize,w_400)
      - 这节课新增的
        - ![image-1](https://document-image.mubu.com/document_image/32569566_d1c44c89-abbe-476f-9e6e-7f676b4e7355.png?x-tos-process=image/resize,w_400)
