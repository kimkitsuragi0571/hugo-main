+++
title = "91 简单DS 精华版"
date = "2026-05-03T10:20:02+08:00"
draft = false
categories = ["C-Sharp"]
tags = ["Notes"]
+++

- ArrayList
  - 调用
    - 使用前记得引入using System.Collections;命名空间
    - 一个能装任何类型的数组
      - 对比泛型(随便可以自己指定,但还是只能装一种,指定后不能变)
    - 相当于是系统已经写好的一个类,直接用就行了
      - ![image-1](https://document-image.mubu.com/document_image/32569566_ec31a5ad-3c9f-410e-8a54-b65d6b71c09d.png?x-tos-process=image/resize,w_400)
      - 这种点击并按F1就可以直接跳转到官方文档了
  - 基本操作
    - ![image-1](https://document-image.mubu.com/document_image/32569566_03866648-d627-497f-c520-4031db46abaa.png?x-tos-process=image/resize,w_400)
      - 在遍历非泛型集合（如 ArrayList/Stack/Queue/Hashtable）时，var item 和 object item 本质上是等价的，但 var 是编译器自动推导类型，object 是显式指定类型。
      - 注意长度不是length而是count
      - Clear清空之后就是空集合,绝对不能再用索引去访问
    - ArrayList本质是Object数组,所以也有装箱拆箱
      - ![image-1](https://document-image.mubu.com/document_image/32569566_7be7f03f-96f1-42a6-ba68-1855da35023a.png?x-tos-process=image/resize,w_242)
- 栈stack
  - ![image-1](https://document-image.mubu.com/document_image/32569566_600aa447-9745-4483-ae66-8cde98929e06.png?x-tos-process=image/resize,w_400)
    - 栈中不存在删除的概念,只能是压栈/弹栈
    - 栈只能弹出栈顶
      - 想做到只改动某个特定元素只能先清空了
    - 栈只能查看栈顶元素
      - 查看某个元素是否在栈中是通过遍历底层存储实现的
    - <mark style="background-color:#fde8e8;">栈不能用for循环遍历</mark>
      - 因为for (int i=0; i<stack.size(); i++)就是随机访问
        - 按照索引遍历了
      - 但是栈本身就和这种思路冲突
      - foreach遍历就可以,很神奇罢
    - foreach或者转化为数组遍历
      - 都是从栈顶到栈底
    - 有泛型栈和普通栈两种
      - 一般还是推荐`Stack<int> stack = new Stack<int>();`
      - 旧版本需要装箱拆箱,有点没法
        - 可以存储多种类型元素
- 队列Queue
  - ![image-1](https://document-image.mubu.com/document_image/32569566_be484239-7b0b-4488-b161-ebcf6a740ef8.png?x-tos-process=image/resize,w_400)
    - 只能查看队首元素peek()而不能查看队尾元素
- 哈希表hashtable
  - ![image-1](https://document-image.mubu.com/document_image/32569566_d47b7420-e452-412c-90ba-bb5bff9741e6.png?x-tos-process=image/resize,w_400)
    - 前面是键,后面是值
      - 因为是Obj数组,所以键和值的类型都是随意的
    - 删
      - <mark style="background-color:#fef3c7;">只能通过指定键而不能指定值去删除</mark>
    - 查
      - 这次的count得到的是键值对的对数
      - <mark style="background-color:#fef3c7;">找值对应的键只能通过遍历</mark>
    - 改
      - 只能改键对应的值,不能直接改键
    - `Hashtable ht = new Hashtable();`
      - 哈希表的泛型版就是字典
