+++
title = "P2 1 PlayerPrefs数据管理类"
date = "2026-05-03T10:20:03+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 首先创建基本的单例管理类
  - ![image-1](https://document-image.mubu.com/document_image/32569566_8ee9a8f8-96fe-43ac-e957-e7845a3cf7ba.png?x-tos-process=image/resize,w_400)
    - 饿汉单例
- 管理类中设定数据存储/读取的方法
  - 存储
    - `public void SaveData(object data, string keyName)`
      - data是数据对象,keyName则是对象的唯一key
    - 使用
      - ![image-1](https://document-image.mubu.com/document_image/32569566_6901c3cb-df8b-439c-d11d-f09840b5bb75.png?x-tos-process=image/resize,w_400)
        - ![image-1](https://document-image.mubu.com/document_image/32569566_7835d332-73ff-4581-c91e-a93662a97963.png?x-tos-process=image/resize,w_195)
  - 传入对象读取
    - `public void LoadData(object data, string keyName)`
    - 使用
      - ![image-1](https://document-image.mubu.com/document_image/32569566_8f4879c3-7106-4671-e726-835406b78edc.png?x-tos-process=image/resize,w_400)
        - 必须先new一个对象,并不方便
  - 返回数据对象读取
    - ![image-1](https://document-image.mubu.com/document_image/32569566_f08aa3fd-c4f0-4877-dbdd-0d563fbd0c97.png?x-tos-process=image/resize,w_355)
    - 使用
      - ![image-1](https://document-image.mubu.com/document_image/32569566_c6a35b9b-d172-41d6-dc30-f2ac3fa126eb.png?x-tos-process=image/resize,w_400)
