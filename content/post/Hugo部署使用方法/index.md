+++
title = "Hugo部署使用方法"
date = "2026-01-11T11:20:41+08:00"
draft = false
image = "dragon.jpg"
categories = ["博客相关"]
tags = ["博客","笔记"]
+++

##### 不管新增/修改/删除文章,始终是在本地文件中直接修改,然后设置:

git add .
git commit -m "新增文章：hugo部署使用方法"
git push

##### 前两句指令可以直接合并为 git commit -am "修改文章：Hugo部署使用方法"

`git commit -am` 【不能用于新增文章 / 新增文件】，只能用于「修改 / 删除」已有文件

##### 查看更新日志:

git log

##### 为文章设置封面:

在文章开头的+++配置区中，新增一行：cover = "图片文件名.jpg"（顶格写，无空格）

##### hugo server -D的作用:

本地启动博客预览服务器，强制显示所有标记为 draft: true 的草稿文章，不会修改任何配置。
文章是否发布，由配置区的 draft: false 手动设置。

##### 控制台创建新文章指令:

hugo new content post/PassageName/index.md 

##### 重装系统的时候:

Hugo无任何配置文件/注册表依赖，把博客根文件夹压缩备份后，重装系统解压即可直接使用。
对存放路径无任何要求，只需要在解压后的博客主目录打开终端即可。

##### catgories中文件的slug:

如slug: "test",则这篇文章的最终访问地址 = https://kimkitsuragi0571.github.io/p/test/