+++
title = "U3 6 特殊文件夹"
date = "2026-05-03T10:20:04+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 常用的
  - ![image-1](https://document-image.mubu.com/document_image/32569566_2c805c8b-0e7e-4492-c8ce-626fd282684f.png?x-tos-process=image/resize,w_400)
    - 获取的只是Asset下的路径,仅编辑模式下使用
  - ![image-1](https://document-image.mubu.com/document_image/32569566_e9b9d7ff-7e45-4825-f885-f3c245c96487.png?x-tos-process=image/resize,w_400)
    - Asset下创建Resources文件夹,路径和名称一定要正确
    - 因为会全被打包,所以一般只会把需要动态加载的资源放入该文件夹
    - 一般也不会获取这个文件夹路径
  - ![image-1](https://document-image.mubu.com/document_image/32569566_ab6f3687-a8b1-4eee-f619-28aa6b857351.png?x-tos-process=image/resize,w_400)
    - 同样需要手动创建
    - 获取路径不需要拼接,直接写Application.StreamingAssets
    - 千万别用路径拼接,因为打包后这个文件夹在不同平台上路径不同
    - 对比普通Res文件夹,不会被压缩加密只能用API,可以自由操控
    - 移动平台只读,但是PC可读可写???
  - ![image-1](https://document-image.mubu.com/document_image/32569566_fbac4c14-58f6-43cb-958f-17c20fe420cb.png?x-tos-process=image/resize,w_400)
    - 没法手动创建
    - 放在固定路径,而且打包后不同平台路径也不同
    - 所有平台都可读写
- 相对不是那么重要(好吧我觉得也挺重要)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_3c31ff6c-8db3-44a9-e15c-155fc71bb023.png?x-tos-process=image/resize,w_400)
    - 同样需要手动创建
  - ![image-1](https://document-image.mubu.com/document_image/32569566_9dffd37d-43d3-4068-d73b-15de2014999b.png?x-tos-process=image/resize,w_400)
    - 同样需要手动创建
  - ![image-1](https://document-image.mubu.com/document_image/32569566_8dd4d1ee-f598-486b-dfe8-3c56df4f2801.png?x-tos-process=image/resize,w_400)
    - 手动创建(或者你导入了就有这个资源)
    - 现在做商业项目也用不上了
