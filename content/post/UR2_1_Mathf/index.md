+++
title = "UR2 1 Mathf"
date = "2026-05-03T10:20:04+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- Math和Mathf区分
  - 一个是Unity的结构体,一个是C#的工具类
  - Math在System中,Mathf在UnityEngine中
  - 自带方法基本一样,Mathf还多了些适配游戏开发的内容
- 一次运算(放start)
  - PI
    - `print(Mathf.PI);`
  - 绝对值
    - `print(Mathf.Abs(-10));`
  - 向上取整
    - `print(Mathf.CeilToInt(1.88001f))`
  - 向下取整
    - `print(Mathf.FloorToInt(9.6f));`
  - 钳制函数
    - `print(Mathf.Clamp(15, 11, 20));`
      - Mathf.Clamp( 要限制的数, 最小值, 最大值 );
      - +
        - **要限制的数 < 最小值**  →  **返回最小值**
        - **要限制的数 > 最大值**  →  **返回最大值**
        - **在中间**  →  **返回原来的数**
  - 获取最大值
    - `print(Mathf.Max(1, 2, 3, 4, 5, 6, 7, 8));`
  - 获取最小值
    - `print(Mathf.Min(1, 2, 3, 4, 545, 6, 1123, 123));`
  - 一个数的n次幂
    - `print(Mathf.Pow(4, 2));`
      - 4的2次方
  - 四舍五入
    - `print(Mathf.RoundToInt(1.3f));`
  - 求一个数的平方根
    - `print(Mathf.Sqrt(4));`
  - 判断一个数是否是2的次方
    - `print(Mathf.IsPowerOfTwo(4));`
  - 判断正负
    - `print(Mathf.Sign(-2));`
- 重复运算(放Update)
  - Lerp插值运算
    - 调用方法
      - `result = Mathf.Lerp(start, end, t);`
    - 实际运算函数运算
      - `result = start + (end - start)*t`
        - 其中t为插值系数,取值范围是0~1
    - 用法1
      - `start = Mathf.Lerp(start, 10, Time.deltaTime);`
        - 每帧改变start的值
        - 变化速度先快后慢，位置无限接近，但不会得到end位置
    - 用法2
      - `time += Time.deltaTime;`
      - `result = Mathf.Lerp(start, 10, time);`
        - 每帧改变t的值
        - 变化速度匀速，位置每帧接近，当t=1时，得到结果
    - 详情见[有关插值的一切_哔哩哔哩_bilibili](https://www.bilibili.com/video/BV17x4y1b7rr/?spm_id_from=333.337.search-card.all.click&vd_source=84e02b1f50f8f0e11b75732187cfda96)
      - 说白了就是用来制作X-Y函数曲线
      - 我不想搞懂具体原理了,反正可以直接套用的
