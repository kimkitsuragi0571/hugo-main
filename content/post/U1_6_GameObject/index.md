+++
title = "U1 6 GameObject"
date = "2026-05-03T10:20:03+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 成员变量
  - ![image-1](https://document-image.mubu.com/document_image/32569566_c646eb67-6f1c-4139-df39-821ecb042b23.png?x-tos-process=image/resize,w_400)
    - 名字修改之后场景中的值也变化
    - 静态可以在面板右上角修改
  - ![image-1](https://document-image.mubu.com/document_image/32569566_f93197b8-7542-4b65-f441-3920d9a6f19d.png?x-tos-process=image/resize,w_400)
- 静态方法
  - ![image-1](https://document-image.mubu.com/document_image/32569566_d5d7fcdb-17cd-4a0c-e4d8-b58aed13b9cd.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_5e6b6515-5f19-4ebe-df74-928d3da7b0b5.png?x-tos-process=image/resize,w_400)
    - 没找到就会返回null
  - ![image-1](https://document-image.mubu.com/document_image/32569566_72731b07-b635-4194-f09e-586f7bd64983.png?x-tos-process=image/resize,w_400)
    - 失活的对象找不到
    - 如果有多个对象满足条件,没法具体确定找到的是谁
    - ![image-1](https://document-image.mubu.com/document_image/32569566_5f8d84bd-0f79-4c31-a16a-866730e702ca.png)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_787cc06a-24b6-411e-abee-43694f30c33b.png?x-tos-process=image/resize,w_400)
    - 没法通过名字找到多个对象
    - 同样只能找到激活方法
  - 用的少一点的方法
    - ![image-1](https://document-image.mubu.com/document_image/32569566_da9893ac-606d-46c6-ca1c-ff0307153309.png?x-tos-process=image/resize,w_400)
      - Unity中的Object类(命名空间是UnityEngine)并非万物之父Object(命名空间System),这个假的Object同样继承自万物之父Object
  - 非常重要的方法
    - 复制
      - ![image-1](https://document-image.mubu.com/document_image/32569566_ebaaf587-a03b-4193-d61e-96e2168d63c6.png?x-tos-process=image/resize,w_159)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_3b83012d-5da6-412b-a5ce-deea68554daf.png?x-tos-process=image/resize,w_400)
    - 删除
      - ![image-1](https://document-image.mubu.com/document_image/32569566_378bb2df-ebf2-4fc0-de8e-30b319d557a1.png?x-tos-process=image/resize,w_400)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_2506a10c-a9a1-4a73-ed4a-e2edf9078ed2.png?x-tos-process=image/resize,w_400)
      - 切换场景会导致当前场景所有物体都被删除
        - ![image-1](https://document-image.mubu.com/document_image/32569566_cf0cb11a-7688-47b9-b305-7509fc2287e0.png?x-tos-process=image/resize,w_400)
- 成员方法
  - ![image-1](https://document-image.mubu.com/document_image/32569566_3f30c298-f16c-4918-93c5-d0778a0357e8.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_35a4483a-309c-44fe-fc43-14015a0d1825.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_a92b4bd3-d8ce-449e-816c-3c7a5aaf30db.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_c9231380-a467-411e-b724-d325d518874f.png?x-tos-process=image/resize,w_147)
    - enable是管理脚本的激活失活
    - 这个是管理游戏物体的激活失活
  - ![image-1](https://document-image.mubu.com/document_image/32569566_94a8b03e-9a42-4621-db2c-48f46ba1fcef.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_c592cf9e-ad0d-4ef2-ee48-ac8606f0d585.png?x-tos-process=image/resize,w_400)
