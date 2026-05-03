+++
title = "U3 4 四元数"
date = "2026-05-03T10:20:04+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 欧拉角
  - ![image-1](https://document-image.mubu.com/document_image/32569566_c4c601e4-bf04-48ea-981a-31255544d637.png?x-tos-process=image/resize,w_319)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_76e4b6be-03c5-4275-9e11-4f87744f799d.png?x-tos-process=image/resize,w_310)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_ab4c5474-50f8-4979-ed76-da13d6811e45.png?x-tos-process=image/resize,w_290)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_89232474-6961-4923-ffd2-502ee5c67306.png?x-tos-process=image/resize,w_400)
    - 面板上修改旋转也可能出现万向节死锁
      - X=90时,旋转Y仍然绕着X轴旋转
- 四元数概念
  - ![image-1](https://document-image.mubu.com/document_image/32569566_fc66a1a9-fc21-49d1-a15f-172f9d135aab.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_47d9661f-97a1-4325-8abb-269ac150daf1.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_944e3fca-311c-449b-8d2a-72cea33c1c36.png?x-tos-process=image/resize,w_400)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_a200ecb3-f3ab-46b8-d80a-93e44af71a62.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_7474d7f0-85fb-4d44-d391-291d42ca7291.png?x-tos-process=image/resize,w_229)
- 代码
  - ![image-1](https://document-image.mubu.com/document_image/32569566_79c16ae8-f868-42ce-fae7-0ded11a57034.png?x-tos-process=image/resize,w_400)
    - 说了半天用第二种简易初始化方法就行了
    - 第一种方法不需要理解原理,直接把参数带入即可,比如绕x轴60度
    - ![image-1](https://document-image.mubu.com/document_image/32569566_2576d690-1780-4bb5-bdd7-20d1d26c5ca2.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_a854d200-1e1a-4309-8667-91d488f295a7.png?x-tos-process=image/resize,w_400)
- 四元数和欧拉角转换
  - ![image-1](https://document-image.mubu.com/document_image/32569566_ebf18c7b-3e4b-4792-9891-910321d8e7bc.png?x-tos-process=image/resize,w_233)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_7d8b06f3-8a9f-4cf8-84d2-3642f93dcba0.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_9cddc432-23a9-4342-a1d5-f2319045eba8.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_31d5cc30-1c2a-4939-8ee2-17a0f391a010.png?x-tos-process=image/resize,w_220)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_3b87292d-8d27-4ddc-e694-b843d92c09a7.png?x-tos-process=image/resize,w_400)
      - 四元数始终是0~180度和-180~0的旋转
        - 解决了同一旋转的表示不唯一
    - ![image-1](https://document-image.mubu.com/document_image/32569566_7882ebef-c46d-4f68-c95d-2c6fdb182e94.png?x-tos-process=image/resize,w_400)
      - 这样转就会有万向死锁,还是绕着Z轴旋转
- 四元数常用方法
  - 单位四元数
    - 0,0,0,1
    - ![image-1](https://document-image.mubu.com/document_image/32569566_c540d46a-74f6-46c5-f821-97a1f2e72197.png?x-tos-process=image/resize,w_400)
      - 下面这个就是复制个物体处于坐标原点,旋转为0
  - 四元数
    - ![image-1](https://document-image.mubu.com/document_image/32569566_997448e9-c76c-49bc-d8ae-d8ec4a685558.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_9826ad9a-30b8-4861-b90b-bd4207cde7f8.png?x-tos-process=image/resize,w_400)
      - 一般用SLerp
  - 向量指向转四元数
    - ![image-1](https://document-image.mubu.com/document_image/32569566_b5a5ab1d-c2c1-44a8-e0ec-d54fe34403e5.png?x-tos-process=image/resize,w_400)
      - A想要改为朝向B,在这个方法中传入AB向量即可
      - ![image-1](https://document-image.mubu.com/document_image/32569566_c9e6121b-c3ca-406d-86d9-a12c7f088a2d.png?x-tos-process=image/resize,w_400)
  - 剩下很多方法大概率用不上
- 四元数的计算
  - ![image-1](https://document-image.mubu.com/document_image/32569566_f975221c-2f60-49d3-b286-0b74aa917b84.png?x-tos-process=image/resize,w_400)
    - 以后就别用欧拉角的方法来旋转了
    - ![image-1](https://document-image.mubu.com/document_image/32569566_2fde958c-eae4-43fb-9d53-a0f44f160613.png?x-tos-process=image/resize,w_400)
      - 实现朝着Y旋转90度
      - 这里Vector3.up是物体局部坐标系Y轴
  - ![image-1](https://document-image.mubu.com/document_image/32569566_a8aa2c5f-4754-4b7a-9af3-88dc53e1757d.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_da529154-c149-4004-d152-0d741740ea04.png?x-tos-process=image/resize,w_400)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_88d27d82-1c9b-4051-d5ec-adc882f61375.png?x-tos-process=image/resize,w_400)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_035ff144-c58f-4da5-e875-cbc4b82af43c.png?x-tos-process=image/resize,w_400)
    - 如果改成v*...就会报错,顺序不能改
  - ![image-1](https://document-image.mubu.com/document_image/32569566_4de796f1-1f66-4aa5-959e-b03e5bd74fd0.png?x-tos-process=image/resize,w_400)
