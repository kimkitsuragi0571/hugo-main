+++
title = "G1 1 GUI原理和控件"
date = "2026-05-03T10:20:03+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 概念
  - 注意,不要用GUI为玩家制作UI功能
  - 这个是用作编辑模式的调试工具
- 工作原理
  - `private OnGUI(){具体UI内容;}`
    - 写在继承Mono的脚本中,类似于生命周期函数
    - 每帧执行,在OnDisable之前,LateUpdate之后
- 控价绘制的共同点
  - ![image-1](https://document-image.mubu.com/document_image/32569566_622d7734-6d93-4331-f764-081fe06c2ad1.png?x-tos-process=image/resize,w_400)
    - 注意全部写在OnGUI中
    - 位置参数`Rect(x,y,w,h)`
    - 参数中都必须有位置信息和实现信息
    - 均有多种重载
  - 文本控件
    - `GUI.Label(new Rect(0,0,100,20),"Hi")`
      - 这里就是放在屏幕坐标系(按照像素点来判断位置)
      - 设定位置左上角(0,0),宽高(100,20)
        - 提前public Rect rec = new Rect...也行,就可以面板修改了
      - 第二个参数是具体内容
    - `GUI.Label(new Rect(0,0,100,20),tex)`
      - `public Texture tex;`
      - 这里显示贴图也是可以的
    - `GUI.Label(new Rect(0,0,100,20),content)`
      - `public GUIContent content;`
      - 一共有Text,Image,Tooltip三种参数
        - 可以实现同时显示文本和图片(始终是先图片后文本)
        - 三种重载,可以任选其中1/2/3个参数显示
        - Tooltip不直接显示,需要`Debug.Log(GUI.Tooltip)`
          - 就可以输出当前控件的Tooltip信息
          - 不重要
    - 自定义样式
      - `public GUIStyle gs;`
      - `GUI.Label(new Rect(0,0,100,20),"Hi",gs)`
        - 然后就可以在面板上修改字体的样式了
        - ![image-1](https://document-image.mubu.com/document_image/32569566_e1e1d1b1-d9db-436b-be11-aac73c3e96a2.png?x-tos-process=image/resize,w_362)
          - 要用的时候自己看文档就行
          - C盘Font文件夹下面就有默认字体,🌿
          - 自动换行就是字体超过设定的默认容量,是否要换行
  - 按钮控件
    - 和文本用起来一样
      - `public Rect BtnRec;`
      - `public GUIContent BtnCtt;`
      - `public GUIStyle BtnSty;`
      - `GUI.Button(BtnRec,BtnCtt,BtbSty)`
    - 本来有个默认按钮背景,加上Style就变成纯Text按钮了
      - 在面板的Normal的Background里面修改
      - 然后文本对齐即可
    - `if(GUI.Button(BtnRec,BtnCtt,BtbSty)){Debug.Log("点击")}`
      - 控件也可以用于测试点击
      - 注意是鼠标抬起的时候,才会显示
      - 而且点的中途移出按钮范围也不行,要有完整的按下抬起过程
    - `if(GUI.RepaeatButton(BtnRec,BtnCtt,BtbSty)){Debug.Log("点击")}`
      - 如果想检测长按,用这个就行
- 必备知识点补充:编辑器模式下执行脚本
  - GUI在脚本执行(即项目运行)的时候才能看见内容
  - 通过添加特性,使得项目没有运行的时候也能执行脚本
  - ![image-1](https://document-image.mubu.com/document_image/32569566_2db0e2b6-0460-43f2-c9e3-a201f4754d17.png?x-tos-process=image/resize,w_400)
    - 脚本类前面添加特性[ExecuteAlways]即可
    - 打开项目发现没有开始跑项目,生命周期函数也都在执行
