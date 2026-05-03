+++
title = "9 C#进阶 迭代器"
date = "2026-05-03T10:20:02+08:00"
draft = false
categories = ["C-Sharp"]
tags = ["Notes"]
+++

- 含义
  - ![image-1](https://document-image.mubu.com/document_image/32569566_4ba9e6f1-3ecf-4505-8836-421d64edebc7.png?x-tos-process=image/resize,w_400)
    - 迭代器（IEnumerator）:协程的本质就是迭代器
    - 又叫做光标
    - foreach等价于迭代器,或者说就是迭代器的简化写法
    - 用foreach实现遍历的类,需要先实现迭代器
- 实现
  - ![image-1](https://document-image.mubu.com/document_image/32569566_65105422-b347-4c76-c068-ff1dd8183618.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_1a2b6352-aa8a-4852-c627-00782e4bcc35.png?x-tos-process=image/resize,w_275)
    - // IEnumerable → 让这个类能被 foreach
    - // IEnumerator  → 让这个类自己当迭代器
    - ![image-1](https://document-image.mubu.com/document_image/32569566_22032fd9-4cdf-44b3-fe15-1a1e525e0ffe.png?x-tos-process=image/resize,w_301)
    - **IEnumerable** ：我 **能被遍历** （证书）
    - **GetEnumerator()**：给我** 拿迭代器**（方法）
    - **IEnumerator** ： **真正挨个取元素** （工具）
      - `public IEnumerator GetEnumerator()就是规定返回值类型IEnumerator`
  - ![image-1](https://document-image.mubu.com/document_image/32569566_903e9c5c-8c26-4865-af57-bd64b7826517.png?x-tos-process=image/resize,w_240)
- 标准迭代器的实现方法
  - `yield return`
  - ![image-1](https://document-image.mubu.com/document_image/32569566_ad7fa687-fd7d-47b8-9b63-430101b6e5fb.png?x-tos-process=image/resize,w_400)
  - ![image-1](https://document-image.mubu.com/document_image/32569566_5be8aabe-f97d-49f0-ed3a-34ec9f89c5cb.png?x-tos-process=image/resize,w_400)
    - 实际还是生成了IEnumerator的三个必须函数
    - ![image-1](https://document-image.mubu.com/document_image/32569566_8f0ee343-6810-4d13-ec61-511ba98bb461.png?x-tos-process=image/resize,w_310)
      - 这种写法也行
  - 用该语法糖实现泛型迭代器
    - ![image-1](https://document-image.mubu.com/document_image/32569566_afcaa94f-239d-495e-a469-e5741e8a64fc.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_f3a30870-4234-456d-fe52-fd2a592efc71.png?x-tos-process=image/resize,w_400)
