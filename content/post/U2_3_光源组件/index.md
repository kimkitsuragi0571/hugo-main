+++
title = "U2 3 光源组件"
date = "2026-05-03T10:20:03+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- (空)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_93b9f486-1ea3-4725-ee10-74356b3aa459.png?x-tos-process=image/resize,w_400)
- Light组件
  - 光源类型
    - 就是Hireakey右键可以创建几种指定的光源类型
    - Area Light(baked only)
      - 只有烘焙光源有效
      - 场景中实时光源就是可以动态变化,但是消耗性能
        - 所以用到烘焙光源到一张图上
        - 类似鬼泣4
  - ![image-1](https://document-image.mubu.com/document_image/32569566_c8b13e1f-f609-4549-9787-b9d7f812d2b1.png)
    - 依次控制类型,范围,SpotAngle控制角度
  - ![image-1](https://document-image.mubu.com/document_image/32569566_7b2e9b9c-ec5f-46f0-c63d-95606ce790d8.png?x-tos-process=image/resize,w_400)
    - MOde就是选择动态还是烘焙
  - ![image-1](https://document-image.mubu.com/document_image/32569566_84b8b039-cc6a-4df9-89a3-a35af8fdc9c3.png?x-tos-process=image/resize,w_221)
    - ShadowType
      - 就是游戏里面设置阴影质量而已
    - Cookie
      - 资源包里的Cookie拖动到面板上,实现不同的照射效果
      - 比如探照灯和白炽灯的区别
    - DrawHalo
      - 光晕效果,灯泡周围有佛光
    - Flare
      - 耀斑
        - 就是看太阳的时候会有的那种炫光效果
      - 使用需要添加Flare Layer脚本,贴图拖动进light组件里面
      - 现在只能导入别人做好的
        - 可以素材库里右键创建lens flare
    - Culling Mask
      - 剔除遮罩层,决定哪些层的对象收到该光源影响
      - 同样指定层级
  - 不重要的
    - ![image-1](https://document-image.mubu.com/document_image/32569566_392a6a2a-ab9c-4d2a-c601-f666bedc7335.png?x-tos-process=image/resize,w_400)
      - 制定反射光每次反射的强度变化
      - 默认是1就不会变化
    - ![image-1](https://document-image.mubu.com/document_image/32569566_f4a87a3b-fd40-40d5-fddf-9c9219fe4148.png?x-tos-process=image/resize,w_400)
      - 下面三个选项都不需要搞懂,阴影计算的时候才有用
    - CookieSize
      - 就是刚才的Cookie设置大小而已
    - ![image-1](https://document-image.mubu.com/document_image/32569566_fd8b085c-03b5-4efa-c2c5-05523bdfe439.png?x-tos-process=image/resize,w_400)
- 啊?光照相关代码只是提一嘴,似乎是很少用
- 相关面板
  - ![image-1](https://document-image.mubu.com/document_image/32569566_60c37bd8-693a-4409-dae4-b6189a82e31a.png?x-tos-process=image/resize,w_400)
    - 选择天空盒这里材质也对应选择天空盒
      - 如果要自己做天空盒材质,就是Material->shader里选天空盒
    - Source
      - 场景中光源混合呈现出整体的光照效果
      - Gradient
        - 就是可以单独设置,天空一种光照,地面另一种光照
      - Skybox
        - 就是设定天空地面光照为一个整体
    - Ambient Mode
      - 这个要下面的光照烘焙啥的,这里暂时不管
    - Environment Reflection
      - 环境反射探针的时候才会用,这里不管
  - ![image-1](https://document-image.mubu.com/document_image/32569566_27758bdf-dd3c-4ace-ab6b-a8cba631326e.png?x-tos-process=image/resize,w_400)
    - 其实也没啥重点,了解即可
    - Fog
      - 修改start和end,还有更改雾的效果
      - Mode 就是三种雾的算法而已
    - HaloTexture
      - 光晕效果更改(甚至能变成方的)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_3ee76871-2616-480f-e38a-e13321262dd2.png?x-tos-process=image/resize,w_400)
        - 还能改
