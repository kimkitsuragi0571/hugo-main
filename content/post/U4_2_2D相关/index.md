+++
title = "U4 2 2D相关"
date = "2026-05-03T10:20:04+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 平台设置
  - ![image-1](https://document-image.mubu.com/document_image/32569566_3801880e-f38e-4bcf-d8cb-553d92caa676.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_c9d01b52-5a7a-46a1-b7c8-b435776b0cb6.png?x-tos-process=image/resize,w_400)
  - Max Size: 设置导入的纹理的最大尺寸
    - 最大也就2048,别超过了
  - Resize Algorithm: 当纹理尺寸大于指定的 Max Size 时使用的缩小算法
    - Mitchell: 默认米切尔算法来调整大小，该算法是常用的尺寸缩小算法
    - Bilinear: 使用双线性插值来调整大小。如果细节很重要的图片，可以使用它，它比米切尔算法保留的细节更多
  - Format: 纹理格式
    - 各平台支持的格式有所不同,如果选择 Automatic,会根据平台使用默认设置
    - ![image-1](https://document-image.mubu.com/document_image/32569566_3f4c2194-31f6-4710-9ef0-338c2551f915.png?x-tos-process=image/resize,w_400)
      - 切换不同平台,然后点击Override,可以选择不同Format
    - 都支持的格式
      - ![image-1](https://document-image.mubu.com/document_image/32569566_17a0cfc6-7559-4418-af74-c9676c052bd4.png?x-tos-process=image/resize,w_500)
    - 移动端/网页端特有格式
      - 没截到,懒得搞了反正也不会看
    - 选择注意事项
      - ![image-1](https://document-image.mubu.com/document_image/32569566_24334e11-a1fd-4b9f-cbe3-2bb6160294ae.png?x-tos-process=image/resize,w_400)
      - 安卓的注意事项(nm一张图片截不全的)
        - ![image-1](https://document-image.mubu.com/document_image/32569566_1da24f94-3d93-4df0-c0c3-260d4b444dbe.png?x-tos-process=image/resize,w_400)
  - Compression:纹理的压缩类型
    - None：不压缩纹理
    - Low Quality：以低质量格式压缩纹理
    - Normal Quality：以标准格式压缩纹理
    - High Quality：以高质量格式压缩纹理
  - Use Crunch Compression
    - 一种压缩时间长但解压时间短的有损压缩格式
    - Compressed Quality 就是压缩质量,质量越高时间越长
  - Split Alpha Channel分离Alpha通道
    - 把一张图分成两张纹理,分别包含 RGB 数据/ Alpha 数据,渲染时合并
    - 节约内存,但是只有图片有透明通道且支持ETC通道才能有这个选项
  - ### Override ETC2 fallback
    - 不支持ETC2压缩的设备上使用的格式
  - 图片窗口
    - ![image-1](https://document-image.mubu.com/document_image/32569566_d18bc9dc-451a-4414-f453-cf5a7f8ab063.png?x-tos-process=image/resize,w_73)
    - 上面的RGB就是以红绿蓝通道显示
    - 下面的RGBA8就是图片格式
