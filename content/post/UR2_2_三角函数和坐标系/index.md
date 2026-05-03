+++
title = "UR2 2 三角函数和坐标系"
date = "2026-05-03T10:20:04+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 三角函数
  - 角度和弧度
    - 角度：1°
    - 弧度：1 radian
    - 圆一周的角度：360°
    - 圆一周的弧度：2π radian
  - 角度弧度转化
    - +
      - π rad = 180°
        - <mark style="background-color:#fde8e8;">总之1弧度=180度/PI</mark>
        - <mark style="background-color:#fde8e8;">1度=PI/180 弧度</mark>
      - 1 rad = (180/π)° → 1 rad ≈ 180 / 3.14 ≈ 57.3°
      - 1° = (π/180) rad → 1° ≈ 3.14 / 180 ≈ 0.01745 rad
    - +
      - 弧度 × 57.3 = 对应角度
      - 角度 × 0.01745 = 对应弧度
  - 相关成员属性
    - `Mathf.PI`
      - 圆周率
    - `Mathf.PositiveInfinity`
      - 正无穷大
    - `Mathf.NegativeInfinity`
      - 负无穷大
    - `Mathf.Deg2Rad`
      - 角度转弧度系数
      - 都必须用浮点数接收
    - `Mathf.Rad2Deg`
      - 弧度转角度系数
    - `Mathf.Epsilon`
      - 极小值
        - 一个极小的数（约 1.4e-45）
      - `if (Mathf.Abs(a - b) < Mathf.Epsilon){}`
        - 用于判断两个浮点数 “是否差不多相等”
        - 避免因精度误差导致 == 判断失败
  - 相关成员方法
    - `Mathf.Asin(value)`
      - 反正弦函数
      - 传入正弦值比如0.5 输出对应角度(弧度制表示)如PI/6
    - `Mathf.Acos(value)`
      - 反余弦函数
    - `Mathf.Atan(value)`
      - 反正切函数
    - `Mathf.Approximately(a, b)`
      - 判断两个浮点数是否近似相等
      - 因为 float 有精度误差,不能直接用 a == b
  - 角度弧度转化
    - ![image-1](https://document-image.mubu.com/document_image/32569566_e5cbfb42-6d3c-4e37-84d6-117281af1846.png?x-tos-process=image/resize,w_253)
      - 弧度必须是浮点数哈,前面也写过
    - ![image-1](https://document-image.mubu.com/document_image/32569566_77c45d10-6b54-4014-d677-25073e3729e2.png?x-tos-process=image/resize,w_253)
  - 三角函数
    - ![image-1](https://document-image.mubu.com/document_image/32569566_ca92f9ac-6c77-4aa7-b1e9-836a9d528041.png?x-tos-process=image/resize,w_311)
      - 这里传入sin()的参数需要转化为弧度
    - ![image-1](https://document-image.mubu.com/document_image/32569566_5f55c08c-a09b-4e9e-a3c8-9594c23b2ce7.png?x-tos-process=image/resize,w_308)
      - 前面写的,传入正弦值输出对应弧度
      - 然后弧度转化为角度
- 坐标系
  - 视口坐标系
    - 和屏幕坐标系类似,但是屏幕坐标：像素单位，随分辨率变
    - 视口坐标：0~1 比例，永远不变
      - 左下角(0,0),右上角(1,1)
  - 各种坐标系
    - 世界坐标系
      - ![image-1](https://document-image.mubu.com/document_image/32569566_2355ade5-6642-4249-de42-0654215ddaa8.png?x-tos-process=image/resize,w_186)
    - 局部坐标系
      - ![image-1](https://document-image.mubu.com/document_image/32569566_96126974-ccdd-422a-9d25-272111d2fe0c.png?x-tos-process=image/resize,w_189)
    - 屏幕坐标系
      - ![image-1](https://document-image.mubu.com/document_image/32569566_14a99021-927f-4ea5-8c5c-828c3d505960.png?x-tos-process=image/resize,w_109)
    - 视口坐标系
    - 坐标系转化
      - ![image-1](https://document-image.mubu.com/document_image/32569566_e7736d6b-653a-4b78-df14-6d44d1f96285.png?x-tos-process=image/resize,w_284)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_60deae43-2fbc-42f1-9da4-dcde80a0352a.png?x-tos-process=image/resize,w_258)
