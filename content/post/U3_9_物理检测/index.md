+++
title = "U3 9 物理检测"
date = "2026-05-03T10:20:04+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 之前还学了一个碰撞检测
  - ![image-1](https://document-image.mubu.com/document_image/32569566_f4012899-fc9d-4364-a270-123ac637fc23.png?x-tos-process=image/resize,w_400)
- 范围检测
  - ![image-1](https://document-image.mubu.com/document_image/32569566_21c9efd5-5cec-47a3-d5e0-9f5fb42cf320.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_055c898d-2ff9-4e43-cb25-492b7e90f1b9.png?x-tos-process=image/resize,w_400)
    - 被检测的对象不一定有刚体,但必须有碰撞器
    - 原理就是创建一个瞬时的"实际不存在"的碰撞器,看谁进入了这个范围
      - 注意没有创建碰撞器实体哈
    - 想要持续检测得放Update里面
  - ![image-1](https://document-image.mubu.com/document_image/32569566_1bdf41f0-117a-49cb-c555-102f51346d99.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_dd51dedc-dc4d-4981-bcef-7220df363d58.png?x-tos-process=image/resize,w_400)
      - 所以刚好有32个层级0~31
      - 这里检测的是UI层
      - 首先NameToLayer返回的是一个int值
        - 这里UI所在层级序号5,返回的就是5
        - 但这并不是最终结果
      - 这里要传入的是1左移多少位,实现只检测UI层
        - 要传入的是1<<变成一个二进制数字
        - 这里就是1左移了5位,转化为0010 0000即32
      - ![image-1](https://document-image.mubu.com/document_image/32569566_9847c6c8-9154-457c-a903-5357fae56c84.png?x-tos-process=image/resize,w_400)
        - 两个二进制|或运算,结果为10 0001 = 33
        - 就是检测UI或者Default层
      - Unity会把0~31层进行与运算,只要结果不为0,就进行检测
    - ![image-1](https://document-image.mubu.com/document_image/32569566_95c0bb02-75a1-4b68-8157-3d0c8e88221a.png?x-tos-process=image/resize,w_400)
    - Physics里面有QueriesTrigger全局设置
    - 这个方法返回值是个数组,可以返回这个范围内所有的碰撞体
  - ![image-1](https://document-image.mubu.com/document_image/32569566_c55d26f6-da5a-4cff-c792-73adfa3d9569.png?x-tos-process=image/resize,w_400)
    - 返回值是int
    - 传入Collider[]数组来存储,而不是直接返回数组
  - ![image-1](https://document-image.mubu.com/document_image/32569566_ddd25da1-6c5c-441c-afc0-afd642f26faa.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_d691a1f6-fe37-4f12-bbf2-68b67427d993.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_a6980cef-737b-42fb-8015-851388d71d04.png?x-tos-process=image/resize,w_400)
    - 两个点就是胶囊上下两个半球体的球心位
    - ![image-1](https://document-image.mubu.com/document_image/32569566_f3f91eab-21a9-4bab-ee29-68fe5c38255d.png?x-tos-process=image/resize,w_321)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_a916773f-5010-4c7d-bd1b-8eb2fe1d1225.png?x-tos-process=image/resize,w_400)
- 射线检测
  - ![image-1](https://document-image.mubu.com/document_image/32569566_f406ba96-196e-45ad-c5c8-3c68311cc428.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_a6a36271-7acd-48df-c637-bfea3490e0dc.png?x-tos-process=image/resize,w_400)
    - 注意不是两点一线,第二个参数是方向向量
  - ![image-1](https://document-image.mubu.com/document_image/32569566_52b6eeab-46c8-4b67-c6e1-722c46a39f6d.png?x-tos-process=image/resize,w_400)
  - 碰撞检测函数
    - ![image-1](https://document-image.mubu.com/document_image/32569566_b0225119-28cb-4a96-f3b9-33c5550c8422.png?x-tos-process=image/resize,w_400)
      - 点进去有16种重载
    - ![image-1](https://document-image.mubu.com/document_image/32569566_0aa233cf-6a14-4477-c165-ca8f9adc0210.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_38c0cccd-0bee-4e2a-d2b3-4aad4df122b5.png?x-tos-process=image/resize,w_400)
      - 法线信息
        ![法线信息-1](https://document-image.mubu.com/document_image/32569566_87643a6a-7abb-4dbd-f9db-a8e988ec9948.png?x-tos-process=image/resize,w_400)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_9ee6913e-6b86-45c6-c948-72256c589858.png?x-tos-process=image/resize,w_400)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_65974a39-f484-46bf-ee4b-d300eba6e468.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_aa2f403a-abcb-4094-c930-ceecf0e2cd75.png?x-tos-process=image/resize,w_400)
    - 相交的多个物体
      - ![image-1](https://document-image.mubu.com/document_image/32569566_2e7c303f-5814-4fc9-c7ee-31b29cf83e4c.png?x-tos-process=image/resize,w_400)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_ad098fc6-217f-4103-9519-99ee37b84d1c.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_d4a7c086-1d5d-44df-f506-ef4701050355.png?x-tos-process=image/resize,w_400)
      - 不报错但是没法用
