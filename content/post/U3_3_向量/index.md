+++
title = "U3 3 向量"
date = "2026-05-03T10:20:04+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 向量模长和单位向量
  - 基本复习
    - ![image-1](https://document-image.mubu.com/document_image/32569566_cf31853f-2766-4b2f-c8ad-703cc94ef23c.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_5db9d5c2-580d-4634-a6f3-d76a8f167417.png?x-tos-process=image/resize,w_400)
      - 向量可以任意平移
    - ![image-1](https://document-image.mubu.com/document_image/32569566_b3431a17-b7fc-45e0-94ff-0c4f8c667cdf.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_e67c61db-d0fb-4e3c-ec21-15f8f0638fc6.png?x-tos-process=image/resize,w_388)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_fd3406d2-c67c-47e6-d62d-0062b356f577.png?x-tos-process=image/resize,w_298)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_53f30d36-3f98-4e75-9fd2-303a44806031.png?x-tos-process=image/resize,w_400)
  - 向量求法
    - ![image-1](https://document-image.mubu.com/document_image/32569566_63c5b41d-e972-4c40-ccdb-b0c41bbc4024.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_176ab9bc-2af7-4429-c5db-c347cb0b4ed7.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_6de0d800-ba4c-46cb-d673-b103cfd51fb8.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_3d582f45-5853-49e3-ef6a-70661266df0a.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_b28fed9a-feee-48eb-f8d7-b1a93f0a9c93.png?x-tos-process=image/resize,w_400)
- 向量四则运算
  - ![image-1](https://document-image.mubu.com/document_image/32569566_360649b3-74f6-4be6-a04a-b1f551863612.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_8ed5d30b-ba30-4db5-f7d7-22b9696fbfb0.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_fe0d14c3-b23f-4dc7-f438-f17629334f90.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_9a37c69c-678b-4d33-950e-5d91886cc388.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_d517f904-f202-4195-dc12-c4d1d7bb55f1.png?x-tos-process=image/resize,w_209)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_77a02503-a03d-49ea-e883-8db7228e9241.png?x-tos-process=image/resize,w_271)
  - 代码
    - ![image-1](https://document-image.mubu.com/document_image/32569566_5021e1d4-f6b6-4cbd-b424-8d950d4ee89a.png?x-tos-process=image/resize,w_400)
      - Translate本质也是在做向量加减法?
- 向量点乘
  - ![image-1](https://document-image.mubu.com/document_image/32569566_9212b22e-7f04-4c48-f581-77f7a79c2f07.png?x-tos-process=image/resize,w_225)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_423e3fab-d275-4248-f7f0-6a154788273f.png?x-tos-process=image/resize,w_296)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_eee2e517-91ff-4bf9-b222-7c6ba1ad23aa.png?x-tos-process=image/resize,w_287)
  - 代码
    - ![image-1](https://document-image.mubu.com/document_image/32569566_f6c967f0-c08c-4c47-c8bc-fee415fb83d6.png?x-tos-process=image/resize,w_400)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_ad8b69b1-4cc4-4e49-baa7-83a11df06254.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_cfebc5a4-9e2c-413e-9f51-77fab9ff9e09.png?x-tos-process=image/resize,w_400)
      - A点乘AB,如果结果>0,就是在前面
    - ![image-1](https://document-image.mubu.com/document_image/32569566_7efad4ca-685d-4faa-d450-4da63c9e540a.png?x-tos-process=image/resize,w_400)
      - 如果想要单位向量?
        ![如果想要单位向量?-1](https://document-image.mubu.com/document_image/32569566_bdba8437-1396-448c-a623-e2e000bc3e70.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_be266368-d4d6-4c32-e08a-9eee6eebb8cb.png?x-tos-process=image/resize,w_400)
      - 角度在0到180之间
    - ![image-1](https://document-image.mubu.com/document_image/32569566_0e690080-8182-4903-fc52-4acd433ae5bd.png?x-tos-process=image/resize,w_400)
      - 第二种方法更靠谱点
- 向量叉乘
  - ![image-1](https://document-image.mubu.com/document_image/32569566_e8b61105-a820-4e33-d8d1-defa37d16ac1.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_aba4500d-51e0-47d8-beeb-1ca8ff7a49a1.png?x-tos-process=image/resize,w_313)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_9e594a37-73f5-4823-954f-2dec94d10f15.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_36efe2ec-8024-4742-a8a9-f2f4e87da5ec.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_70ebe820-bf52-4206-eb9a-3d2f0647f632.png?x-tos-process=image/resize,w_267)
    - 叉乘几何意义
    - ![image-1](https://document-image.mubu.com/document_image/32569566_3c1097dd-608e-435f-c141-15655ae0b7de.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_9be5f141-1223-4f8c-f49d-de29f40e5994.png?x-tos-process=image/resize,w_400)
    - 用啥左手坐标系记忆?
    - ![image-1](https://document-image.mubu.com/document_image/32569566_b389f120-d74c-4031-da34-786a805f9c25.png?x-tos-process=image/resize,w_400)
- 向量插值
  - 线性插值
    - Mathf和Vector3里面都有lerp方法
    - ![image-1](https://document-image.mubu.com/document_image/32569566_fdb87f09-0638-4383-a1d4-bf798fa42d80.png?x-tos-process=image/resize,w_156)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_fb9d2243-61b5-4182-ad9e-1740cd2fb12b.png?x-tos-process=image/resize,w_147)
    - 代码
      - ![image-1](https://document-image.mubu.com/document_image/32569566_e3571c98-b23b-4173-df26-40857f89dd30.png?x-tos-process=image/resize,w_400)
        - 场景中A跟着Target,B直接和Target重合了
        - 所以可以设置个条件限制下
        - ![image-1](https://document-image.mubu.com/document_image/32569566_3196219d-7a61-428b-c2f8-bae69e4a34e4.png?x-tos-process=image/resize,w_400)
  - 球形插值
    - ![image-1](https://document-image.mubu.com/document_image/32569566_e895bae7-db40-47ec-9550-d1e7131c9b6b.png?x-tos-process=image/resize,w_214)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_ba817b25-d468-4410-d863-8d21f3b49088.png?x-tos-process=image/resize,w_400)
      - A到B的轨迹上区别
      - ![image-1](https://document-image.mubu.com/document_image/32569566_28ecd6e8-778c-4771-d781-8b29a10ff744.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_b30aea7f-854e-4f23-aacc-28e2402c3c01.png?x-tos-process=image/resize,w_400)
      - 反正处理某些特殊情况
      - 模拟太阳运动弧线?
    - ![image-1](https://document-image.mubu.com/document_image/32569566_f7e86e9a-89ea-46e0-a86f-88d0f4e15552.png?x-tos-process=image/resize,w_400)
