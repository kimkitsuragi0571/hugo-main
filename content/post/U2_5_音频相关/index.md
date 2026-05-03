+++
title = "U2 5 音频相关"
date = "2026-05-03T10:20:03+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 音频文件导入
  - ![image-1](https://document-image.mubu.com/document_image/32569566_70b497a8-17e5-4090-8b1b-bb796877cedf.png?x-tos-process=image/resize,w_168)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_65ec69e6-6be8-41d5-f9a8-75f79cf8b4dc.png?x-tos-process=image/resize,w_150)
    - 进度条右边
      ![进度条右边-1](https://document-image.mubu.com/document_image/32569566_94b9b964-1287-4382-98a8-f6bdc8e45950.png)
      - 中间这个是点开该文件自动播放,切到其他文件就不会播放
    - ![image-1](https://document-image.mubu.com/document_image/32569566_477c9de7-34da-4a9b-efdb-d144af792438.png?x-tos-process=image/resize,w_400)
      - Ambisonbic一般就3D混响或者VR的时候才用的上
      - 3种加载类型
        - 反正根据内存来选就对了
    - ![image-1](https://document-image.mubu.com/document_image/32569566_fbe5f7c3-c357-4d5f-ee49-655216dff60d.png?x-tos-process=image/resize,w_400)
      - 只有Format里面选择Vorbis的时候才会依质量来压缩
- 音频源和音频监听
  - AudioSource
    - 音效源脚本
    - AudioClip音效切片选项
      - 关联音效文件
    - ![image-1](https://document-image.mubu.com/document_image/32569566_0b8c7d10-3af9-4687-a1e4-bc1449327e8a.png?x-tos-process=image/resize,w_208)
      - Outpute声音混响,除非声效游戏不然用不上
      - PlayOnAwake脚本依附的对象一生成就开始播放
        - 除了BGM一般是通过代码打开播放,不会自己开始
      - Prioity优先级越高,场景中音效多的时候越不容易被擦除
      - Pitch音高就是常见的整蛊音效加速hhh
    - ![image-1](https://document-image.mubu.com/document_image/32569566_cd29625f-fb2d-4bd6-a771-b0573a246d9a.png?x-tos-process=image/resize,w_400)
      - StereoPan设为0,直接根据音效自身左右声道来播放,不咋用
      - Spatial Blend还挺有用..吗?
        - 就是设为0直接2D音效,在哪里播放都一样
        - 1就是3D音效
  - AudioListener
    - 音效监听,相当于耳朵
    - 默认挂载到场景MainCamera
    - 场景中创建多个主摄像机会默认都挂载监听,就会报错
- 音频控制脚本
  - 获取脚本组件
    ![获取脚本组件-1](https://document-image.mubu.com/document_image/32569566_6441d8c3-66d7-4f55-eb43-3446abcd4a34.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_5241826a-2f08-41dc-883c-42a27d44ca42.png?x-tos-process=image/resize,w_383)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_c4523b87-837c-49cc-dd92-b0bc281a4cc8.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_ffccb0d1-4d82-4b03-ac01-2ca9788d3f9a.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_1558735f-0cc4-4102-95aa-709a4ffa0b3e.png?x-tos-process=image/resize,w_400)
    - 放在Update中不断检测属性
    - 没有直接提供检测音乐是否播放完毕的功能
  - 动态控制音效播放
    - ![image-1](https://document-image.mubu.com/document_image/32569566_5add63a1-aafd-4ae3-eef1-4ef11660595a.png?x-tos-process=image/resize,w_400)
      - 挂载了音效源,然后勾选play on Awake
    - ![image-1](https://document-image.mubu.com/document_image/32569566_43758b47-316f-46df-82f2-929453964fd3.png?x-tos-process=image/resize,w_400)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_f698292d-846f-4e69-cd17-fcc23ec84dc7.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_82628c30-8077-4c95-dd5c-b6fbe178cb84.png?x-tos-process=image/resize,w_400)
- 麦克风输入相关
  - 我是很难想象能用得到这一节
  - 跳过暂时不学了
