+++
title = "U3 1 Mathf"
date = "2026-05-03T10:20:03+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 概念
  - Math和Mathf
    - ![image-1](https://document-image.mubu.com/document_image/32569566_db8f7aec-983c-43c8-9042-f7374e9e4bf5.png?x-tos-process=image/resize,w_400)
      - 一个是Unity的结构体,一个是C#的工具类
      - Mathf还多了些适配游戏开发的内容
- 使用
  - 一次运算
    - ![image-1](https://document-image.mubu.com/document_image/32569566_05306568-cddb-4172-e31b-5999d36fd26e.png?x-tos-process=image/resize,w_228)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_836f41ca-e8e6-4f6b-933c-0b8816d08595.png?x-tos-process=image/resize,w_242)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_4edc1ca4-6469-4a9f-f398-459a7ca7bac7.png?x-tos-process=image/resize,w_142)
        - 第三个15在11到20之间,所以还是输出自身15
      - 其他语言参数顺序不同
    - ![image-1](https://document-image.mubu.com/document_image/32569566_9c987c72-0275-4fa9-fec0-573819d50358.png?x-tos-process=image/resize,w_304)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_a36889d9-1a41-46dc-cc5f-3c6a3bb584c0.png?x-tos-process=image/resize,w_296)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_e5beaaf4-ba1f-4e50-db0b-916eee293a8f.png?x-tos-process=image/resize,w_299)
  - 重复运算(放在Update里面不停计算)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_1336bccf-274b-4c37-f1b9-23c6044f1cc5.png?x-tos-process=image/resize,w_400)
      - 变量start设定个初始值0
      - Time.deltaTime作为t系数,一般值为0.02远小于1
      - 详情见[有关插值的一切_哔哩哔哩_bilibili](https://www.bilibili.com/video/BV17x4y1b7rr/?spm_id_from=333.337.search-card.all.click&vd_source=84e02b1f50f8f0e11b75732187cfda96)
        - 说白了就是用来制作X-Y函数曲线
    - ![image-1](https://document-image.mubu.com/document_image/32569566_4cd417a0-e936-40ad-8c15-c0e7d5dff22b.png?x-tos-process=image/resize,w_400)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_51f30076-a699-49a7-f9ec-248e378bb201.png?x-tos-process=image/resize,w_191)
