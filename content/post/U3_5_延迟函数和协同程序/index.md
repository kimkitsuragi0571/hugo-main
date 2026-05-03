+++
title = "U3 5 延迟函数和协同程序"
date = "2026-05-03T10:20:04+08:00"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

- 延迟函数
  - 概念
    - ![image-1](https://document-image.mubu.com/document_image/32569566_88747603-63fa-456d-d7f6-ddd1b3674b57.png?x-tos-process=image/resize,w_400)
  - 使用
    - ![image-1](https://document-image.mubu.com/document_image/32569566_eaa62a87-c27b-4f8d-825b-8f7b25b2f9ab.png?x-tos-process=image/resize,w_400)
      - 另一个脚本的函数是没法执行的
    - ![image-1](https://document-image.mubu.com/document_image/32569566_02b02feb-a9f2-450b-e8a3-aeb8c1a5b162.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_f77e15ca-a7de-4ee6-9355-bcc1b3922607.png?x-tos-process=image/resize,w_400)
  - 影响
    - ![image-1](https://document-image.mubu.com/document_image/32569566_26112146-8c03-4cff-8744-d7d9cc48ecbf.png?x-tos-process=image/resize,w_400)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_65c073b2-10f3-4e61-d003-530fc6ca61e1.png?x-tos-process=image/resize,w_400)
- 协同程序
  - 多线程
    - ![image-1](https://document-image.mubu.com/document_image/32569566_8e152c80-4abd-4470-8745-537a3f086277.png?x-tos-process=image/resize,w_400)
      - ![image-1](https://document-image.mubu.com/document_image/32569566_883e6e70-1523-4569-fa1b-a0e1b238fb81.png?x-tos-process=image/resize,w_400)
        - 这里线程里面让this对象移动,直接报错
          - 连访问场景中对象,打印下都会报错
          - 哦好吧,甚至调用Unity命名空间中方法也报错,看下面的例子
        - 不释放就会一直执行,所以记得关闭线程
      - 线程很多时候作为一个独立管道,来处理某些计算逻辑
        - 就是找外包,算完了再传回来提高效率
        - 比如网络收发消息,A*算法
      - ![image-1](https://document-image.mubu.com/document_image/32569566_12227976-7c8d-4e08-906a-1dc154252515.png?x-tos-process=image/resize,w_400)
        - ![image-1](https://document-image.mubu.com/document_image/32569566_80345623-c41e-46d6-e3c5-27ba11c592a6.png?x-tos-process=image/resize,w_400)
          - 这里Random.Range仍然属于Unity空间中方法,直接报错
        - ![image-1](https://document-image.mubu.com/document_image/32569566_2ea2e099-8c5e-4f37-89a3-4862c5dba0fb.png?x-tos-process=image/resize,w_400)
          - 这里用C#里面的Random对象就可以了
  - 协同程序
    - ![image-1](https://document-image.mubu.com/document_image/32569566_893c0928-4cb3-42b2-c805-1ead86c943d5.png?x-tos-process=image/resize,w_375)
      - 总之记住不是真的多线程就对了
    - ![image-1](https://document-image.mubu.com/document_image/32569566_cc8f9fae-6ec9-43b4-d2ce-6989565f65f1.png?x-tos-process=image/resize,w_400)
      - 就是函数执行到一半挂起,等一会儿再执行下一个阶段
        - 把一个函数分时分步执行
      - 和计组里面的中断一样
  - 协程的使用
    - ![image-1](https://document-image.mubu.com/document_image/32569566_8c880c86-1f43-4014-b3c0-be82bf733696.png?x-tos-process=image/resize,w_400)
      - 返回子类也是可以的
    - ![image-1](https://document-image.mubu.com/document_image/32569566_59a2b5d7-5733-491b-8cfe-99baccc4c399.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_a504e7d6-21bf-495e-b79e-24455a7af5e4.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_125298cb-6d04-4745-8f12-e960cb1858e0.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_c26621f0-da67-4139-b4ac-bd8cd657b207.png?x-tos-process=image/resize,w_400)
      - 写几个yield return就相当于将代码分为几块
    - ![image-1](https://document-image.mubu.com/document_image/32569566_ac11f97e-68c8-4117-d1f7-d29ce7dfa634.png?x-tos-process=image/resize,w_400)
      - 还可以通过字符串关闭,但是不建议
    - ![image-1](https://document-image.mubu.com/document_image/32569566_f6e88cb3-ad9a-4d55-beed-2b14fe93eea8.png?x-tos-process=image/resize,w_400)
      - 额,这种写法是?
    - ![image-1](https://document-image.mubu.com/document_image/32569566_21c1bfa2-0da3-4752-c313-88cded928219.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_c0bf4df3-c7ad-4c3b-a31b-855e2f21b7c2.png?x-tos-process=image/resize,w_400)
    - ![image-1](https://document-image.mubu.com/document_image/32569566_822c09de-6096-411f-c01b-bf8405fc09d5.png?x-tos-process=image/resize,w_400)
