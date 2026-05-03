+++
title = "U4 4 SpriteRenderer"
date = "2026-05-03T10:20:04+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 概念
  - ![image-1](https://document-image.mubu.com/document_image/32569566_4437719a-5559-4e18-a376-28003284707b.png?x-tos-process=image/resize,w_400)
    - 2D游戏中除了UI都是通过SR渲染实现
  - ![image-1](https://document-image.mubu.com/document_image/32569566_ce529613-39a7-4d16-85af-ff7f2acd9c5d.png?x-tos-process=image/resize,w_255)
    - 1.直接拖入Sprite图片
    - 2.右键创建Sprite游戏物体,然后关联精灵图素材
    - 3.空物体添加SpriteRender就是个Sprite游戏物体
- 选项
  - Sprite (渲染的精灵图片)
  - Color (定义着色)
    - 在原图上叠加一个颜色,比如受伤的时候就闪红
  - Flip (水平或竖直翻转精灵图片)
    - 2D游戏实现左右转向
  - Draw Mode (绘制模式)
    - 当尺寸变化(改图片Scale)时的缩放方式
    - Simple (简单模式)
      - 缩放时整个图像一起缩放
    - Sliced (切片模式)
      - <mark style="background-color:#fef3c7;">需要把精灵的网格类型MeshType设置为 Full Rect</mark>
      - 九宫格切片模式下的十字区域缩放，4 个角不变化
        - ![image-1](https://document-image.mubu.com/document_image/32569566_669623ad-f339-4cc9-c60c-00dc1a72c285.png?x-tos-process=image/resize,w_106)
        - 只缩放中间十字
      - <mark style="background-color:#fde8e8;">这个模式下就不要通过Scale来缩放了,SR选了SliceMode,下面有个Size</mark>
      - 一般用于变化不大的纯色图片
        - ![image-1](https://document-image.mubu.com/document_image/32569566_fbe1e9a4-9f21-4337-e44c-bebb55f7c7da.png?x-tos-process=image/resize,w_94)
        - 这么个块实现缩放就很方便,省内存(素材里有,自己试试)
    - Tiled (平铺模式)
      - <mark style="background-color:#fef3c7;">需要把精灵的网格类型MeshType设置为 Full Rect</mark>
      - 将九宫格中间十字部分进行平铺而不是缩放
        - ![image-1](https://document-image.mubu.com/document_image/32569566_1e387662-7c83-44cd-8072-7f9180242de2.png?x-tos-process=image/resize,w_400)
      - Tile Mode (Tiled 模式下的子选项)
        - Continuous (连续平铺)
          - 当尺寸变化时，中间部分将均匀平铺,类似 Simple 模式
        - Adaptive (自适应平铺)
          - 当更改尺寸达到 Stretch Value(临界值) 时，中间才开始平铺
  - Mask Interaction (与精灵遮罩交互时的方式)
    - None: 不与场景中任何精灵遮罩交互
    - Visible inside Mask: 精灵遮罩覆盖的地方可见，而遮罩外部不可见
    - Visible Outside Mask: 精灵遮罩外部的地方可见，而遮罩覆盖处不可见
    - 额,涉及遮罩,看看就行
  - Sprite Sort Point (计算摄像机和精灵之间距离时使用的参考点)
    - Center 还是轴心点 Pivot，一般情况下不用修改
  - Material (材质)
    - 可以使用一些自定义材质来显示一些特殊效果
      - 一般情况不修改
    - 默认材质是不会受到光照影响的
      - 如果想要受光照影响，可以选择 Default-Diffuse
  - Additional Settings (高级设置)
    - Sorting Layer (排序层选择)
      - <mark style="background-color:#fef3c7;">2D项目里面默认正交摄像机,物体Z轴大小不变(还是会影响显示层级)</mark>
        - 默认都是在Default层,依照Z轴前后显示层级
    - Order in Layer (层级序列号)
      - <mark style="background-color:#fef3c7;">这里就可以修改显示层级,比如排序层为1的即使在0层前面,仍然被覆盖</mark>
- 代码
  - 创建物体然后添加SR组件
    - `GameObject obj = new GameObject();`
    - `SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();`
  - 动态加载图片
    - `sr.sprite = Resources.Load<Sprite>("dead1");`
  - 如果是加载图集
    - `Sprite[] sprs = Resources.LoadAll<Sprite>("Robot");`
      - 先获取一个数组,然后获取其中一个
    - `sr.sprite = sprs[10];`
