+++
title = "93 泛型DS 精华版"
date = "2026-05-03T10:20:02+08:00"
draft = false
categories = ["C-Sharp"]
tags = ["Notes"]
+++

- List
  - 概念
    - List类就是个可变类型的泛型数组
    - 一开始指定了类型就不能变
      - 和数组一样只能是一种类型
      - <mark style="background-color:#fef3c7;">好处就是可以扩容</mark>
        - 让人想到StringBuilder
      - ArrayList是真的类型随意
  - 方法示例
    - | 方法 / 语法 | 代码示例 | 作用 |
      | --- | --- | --- |
      | 空列表初始化 | List list = new List(); | 创建一个空的泛型列表，默认初始容量为 4 |
      | 指定容量初始化 | List list = new List(10); | 创建指定初始容量的列表，减少后续扩容次数，提升性能 |
      | 集合初始化 | List list = new List { 1, 2, 3 }; | 初始化时直接添加元素，简化代码书写 |
      - 注意初始容量手动指定,不足就会自动扩容
    - | 方法 / 语法 | 代码示例 | 作用 |
      | --- | --- | --- |
      | Add(T) | list.Add(4); | 向列表末尾添加单个元素，容量不足时自动扩容 |
      | AddRange(IEnumerable) | list.AddRange(new int[] { 5, 6 }); | 批量添加集合元素到列表末尾，比多次 Add 更高效 |
      | Insert(int, T) | list.Insert(1, 9); | 在指定索引位置插入元素，后续元素自动后移 |
    - | 方法 / 语法 | 代码示例 | 作用 |
      | --- | --- | --- |
      | Remove(T) | list.Remove(2); | 移除列表中第一个匹配的指定元素，返回是否删除成功 |
      | RemoveAt(int) | list.RemoveAt(0); | 移除指定索引位置的元素，索引越界会抛异常 |
      | Clear() | list.Clear(); | 清空列表所有元素，保留当前容量（不会释放内存） |
    - | 方法 / 语法 | 代码示例 | 作用 |
      | --- | --- | --- |
      | 索引查询 | int value = list[2]; | 通过索引快速获取指定位置的元素，时间复杂度 O (1) |
      | Contains(T) | bool exists = list.Contains(3); | 判断列表是否包含指定元素，返回布尔值 |
      | IndexOf(T) | int index = list.IndexOf(3); | 获取指定元素第一次出现的索引，不存在返回 - 1 |
      - 可以通过下标实现随机访问
      - 栈和堆就只能查看栈顶/堆首元素
    - | 方法 / 语法 | 代码示例 | 作用 |
      | --- | --- | --- |
      | 索引修改 | list[1] = 8; | 通过索引直接修改指定位置的元素值 |
    - | 方法 / 语法 | 代码示例 | 作用 |
      | --- | --- | --- |
      | 索引遍历（for） | for (int i = 0; i < list.Count; i++) { Console.WriteLine(list[i]); } | 可修改元素值，适合需要索引的场景 |
      | 只读遍历（foreach） | foreach (var item in list) { Console.WriteLine(item); } | 语法简洁，只读遍历，无法修改元素 |
      | 迭代器遍历 | using (var enumerator = list.GetEnumerator()) { while (enumerator.MoveNext()) { Console.WriteLine(enumerator.Current); } } | 手动控制迭代过程，适合复杂遍历场景 |
      - ?foreach不就自动调用了遍历器吗
- Dictionary
  - 拥有泛型的哈希表,从哈希表的Obj类型变为了手动指定类型
  - 声明
    - ![image-1](https://document-image.mubu.com/document_image/32569566_995559de-7f13-4d3b-e72e-408ebae6bc53.png?x-tos-process=image/resize,w_400)
      - 指定了键和值的类型
  - 方法举例
    - | 方法 / 语法 | 代码示例 | 作用 |
      | --- | --- | --- |
      | 空字典初始化 | Dictionary dict = new Dictionary(); | 创建一个空的泛型字典，默认初始容量为 0，首次添加元素时扩容 |
      | 指定容量初始化 | Dictionary dict = new Dictionary(10); | 创建指定初始容量的字典，减少后续哈希冲突和扩容次数 |
      | 集合初始化 | Dictionary dict = new Dictionary { {1, "张三"}, {2, "李四"} }; | 初始化时直接添加键值对，简化代码书写 |
    - | 方法 / 语法 | 代码示例 | 作用 |
      | --- | --- | --- |
      | Add(TKey, TValue) | dict.Add(3, "王五"); | 向字典添加唯一键值对，键已存在时会抛出异常 |
      | 索引赋值（新增 / 修改） | dict[4] = "赵六"; // 新增 dict[1] = "张三丰"; // 修改 | 键不存在则新增，键已存在则覆盖原有值（最常用） |
      | TryAdd(TKey, TValue) | bool isSuccess = dict.TryAdd(5, "孙七"); | 尝试添加键值对，键已存在时返回 false（不会抛异常），新增成功返回 true |
      - 值可以重复,键不能重复
    - | 方法 / 语法 | 代码示例 | 作用 |
      | --- | --- | --- |
      | Remove(TKey) | bool isRemoved = dict.Remove(2); | 根据键移除对应的键值对，移除成功返回 true，键不存在返回 false |
      | Clear() | dict.Clear(); | 清空字典所有键值对，保留当前容量（不会释放内存） |
    - | 方法 / 语法 | 代码示例 | 作用 |
      | --- | --- | --- |
      | 索引查询 | string value = dict[1]; | 通过键快速获取对应值，键不存在时抛出 KeyNotFoundException 异常 |
      | ContainsKey(TKey) | bool hasKey = dict.ContainsKey(3); | 判断字典是否包含指定键，返回布尔值（最常用的查询前置判断） |
      | ContainsValue(TValue) | bool hasValue = dict.ContainsValue("李四"); | 判断字典是否包含指定值，需遍历所有元素，效率较低 |
      | TryGetValue(TKey, out TValue) | bool isFound = dict.TryGetValue(4, out string val); | 尝试通过键获取值，键不存在时返回 false 且 out 参数为默认值（不会抛异常） |
      - 这里要是把键设置为其他类型,索引就可以不用int
    - | 方法 / 语法 | 代码示例 | 作用 |
      | --- | --- | --- |
      | 遍历键值对（KeyValuePair） | foreach (KeyValuePair item in dict) { Console.WriteLine($"{item.Key}: {item.Value}"); } | 遍历所有键值对，可同时获取键和值（最常用） |
      | 遍历键集合 | foreach (int key in dict.Keys) { Console.WriteLine(key); } | 仅遍历字典的所有键 |
      | 遍历值集合 | foreach (string val in dict.Values) { Console.WriteLine(val); } | 仅遍历字典的所有值 |
      - 这里也是用count
- 线性表
  - 顺序存储
    - 线性表要求所有元素是同种数据类型
    - ArrayList仍然是线性表
      - 只是有点非泛型的、弱类型的线性表
      - 其实存的都是Object类型
    - ![image-1](https://document-image.mubu.com/document_image/32569566_1e824e00-a39a-4dae-beb6-b62e97a98a3d.png?x-tos-process=image/resize,w_400)
  - 链式存储
    - 实现个单向链表
      - ![image-1](https://document-image.mubu.com/document_image/32569566_b8cd57ee-6553-423f-b41c-518b819d5837.png?x-tos-process=image/resize,w_223)
        - 如果是C++
          - ![image-1](https://document-image.mubu.com/document_image/32569566_d8e65dcc-2eab-43c2-d316-69fe1030282e.png?x-tos-process=image/resize,w_173)
        - 毕竟有引用类型,和指针一样的
        - 下面那个是路过的构造函数哈,和链表无关
      - ![image-1](https://document-image.mubu.com/document_image/32569566_e5693936-ae41-4a60-f77a-9d2384baee1b.png?x-tos-process=image/resize,w_400)
        - 申明两个节点,然后链起来就行了
        - 也可以直接连接的时候声明新节点
          - ![image-1](https://document-image.mubu.com/document_image/32569566_ef973c4b-2fbd-4bcb-c533-22bd4f5c2def.png?x-tos-process=image/resize,w_400)
    - 单向链表类
      - ![image-1](https://document-image.mubu.com/document_image/32569566_c028de13-96c4-42f0-d805-56e48da9663f.png?x-tos-process=image/resize,w_203)
        - 最后一句是声明新加入节点为末尾节点
        - ![image-1](https://document-image.mubu.com/document_image/32569566_68cd3ecd-dfb4-4abe-a18a-809d22421f25.png?x-tos-process=image/resize,w_265)
          - 所以是node引用赋值给last引用
          - 现在last引用指向了node对应堆对象
          - 对比C++指针
            - ![image-1](https://document-image.mubu.com/document_image/32dd57f9-2ed7-4746-9b71-3020082f6cf0-32569566.jpg?x-tos-process=image/resize,w_400)
            - <mark style="background-color:#fef3c7;">总之对于栈堆区分配就没有那么严</mark>
      - 如果要添加Remove方法
        - ![image-1](https://document-image.mubu.com/document_image/32569566_c446881f-9758-49eb-bb8e-84a9e71d18e1.png?x-tos-process=image/resize,w_233)
    - 遍历
      - ![image-1](https://document-image.mubu.com/document_image/32569566_1662930a-52e8-4bae-8fa3-7fe4ed9eec74.png?x-tos-process=image/resize,w_315)
  - LinkedList
    - 申明
      - 本质是可变类型的泛型双向链表
      - ![image-1](https://document-image.mubu.com/document_image/32569566_f301df8c-ed2e-4895-e919-0956712de311.png?x-tos-process=image/resize,w_297)
        - LinkedList和LinkedListNode两种概念
    - 基本操作
      - ![image-1](https://document-image.mubu.com/document_image/73347bb6-0688-40f3-9e4d-8f36e2e34966-32569566.jpg?x-tos-process=image/resize,w_500)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_98f2dd35-f52e-4070-9195-5792b5418773.png?x-tos-process=image/resize,w_500)
        - 需要先查找到指定节点
      - ![image-1](https://document-image.mubu.com/document_image/32569566_3e2cf476-3b47-42a3-9e0f-d5ff9116ef58.png?x-tos-process=image/resize,w_500)
        - <mark style="background-color:#fef3c7;">只能是指定元素而不能指定下标</mark>
        - 毕竟链表本身就没有下标
      - ![image-1](https://document-image.mubu.com/document_image/32569566_89d402de-ca2a-4c27-e064-87d46d02e327.png?x-tos-process=image/resize,w_500)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_910ca73f-60c2-45c8-d8f1-f85e9c9de085.png?x-tos-process=image/resize,w_500)
  - 泛型栈和队列
    - 几种数据集合
      - ![image-1](https://document-image.mubu.com/document_image/32569566_b7c63e6f-69c7-4b4a-8a0d-2bb285914103.png?x-tos-process=image/resize,w_308)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_ccfad3f9-fd44-4e3f-d509-d0be1fa02d3d.png?x-tos-process=image/resize,w_311)
    - ![image-1](https://document-image.mubu.com/document_image/0692d63f-71d5-47e2-a073-b62c52906531-32569566.jpg?x-tos-process=image/resize,w_400)
