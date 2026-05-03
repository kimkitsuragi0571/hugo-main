+++
title = "U3 8 LineRenderer"
date = "2026-05-03T10:20:04+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- LineRenderer界面
  - ![image-1](https://document-image.mubu.com/document_image/32569566_9f0f130b-ad68-4541-cb75-dd2bab0fef3e.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_196fb91b-a374-4861-ed17-4ec22b110b00.png?x-tos-process=image/resize,w_197)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_a077282a-2c46-449d-c0c8-28d407709fb0.png?x-tos-process=image/resize,w_400)
    - 创建后场景里有个紫色方块,就是线段(只是没有材质)
    - position
      - Loop首尾相连
      - size
        - 控制点的数量
        - 默认这些点都是世界坐标系下面
      - Width
        - 控制线条的宽度
        - 中间添加key就能让线条宽窄变化
      - color
        - ![image-1](https://document-image.mubu.com/document_image/32569566_b91e00f2-0d4c-4947-bdc8-397501959e60.png?x-tos-process=image/resize,w_247)
          - 上面调透明度
          - 下面调开始颜色
          - 从左到右就是线条起始
      - Corner Vertices
        - 添加线条倒角
      - EndCapVertices
        - 线条起始倒角
    - ![image-1](https://document-image.mubu.com/document_image/32569566_a471a577-107b-447b-8403-fd076edd11ac.png?x-tos-process=image/resize,w_286)
      - 阴影偏移因为线条是个3D物体,同样可以受到光照影响
    - ![image-1](https://document-image.mubu.com/document_image/32569566_17b3c479-4b58-4dae-9b8f-2c81323fe75a.png?x-tos-process=image/resize,w_400)
      - Use World Space取消勾选,上面的点就是相对于局部坐标系了
        - 就会跟着父物体移动了
      - Material
        - 材质中albdo选择贴图
        - 然后将材质拖动到Line的Material上
          - 直接拖上去是黑的,因为没有勾选Generate Lighting Data
          - 需要受到光的影响
      - ![image-1](https://document-image.mubu.com/document_image/32569566_1de15148-3dfa-4976-d4f3-020648a38870.png?x-tos-process=image/resize,w_400)
        - 开启和接收阴影
      - 探针
        - 后面讲
      - (空)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_63e91a3e-fdfa-47e1-b127-805fe2be80d0.png?x-tos-process=image/resize,w_400)
    - 新版本才有的编辑模式
      - ![image-1](https://document-image.mubu.com/document_image/32569566_e976b500-73fe-47ca-b47b-23a82596acc5.png?x-tos-process=image/resize,w_400)
        - 简化预览就是有没有那条线
        - 宽容度调低就是减少绘图点,让偏差变大
        - 细分选项就是加点
        - 输入模式
          - 基于鼠标位置这个有点乱,直接在脸上创建了
          - 基于射线这个说起来有点抽象
          - 反正创建个平面自己在上面点点就知道啥意思了
        - LayerMask
          - 就是选择输入模式中哪些层级可以反应
- 代码相关
  - ![image-1](https://document-image.mubu.com/document_image/32569566_81919bc2-c8e3-4329-b454-88790ddb134d.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_2f98a9c6-9f0c-4540-84a8-9d3a1673ecb9.png?x-tos-process=image/resize,w_400)
    - 设置点的数量结果没全都指定,默认000
    - 还可以根据索引设置
