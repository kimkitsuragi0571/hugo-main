+++
+++ title = "Hugo部署使用方法" date = "2026-01-11T11:20:41+08:00" draft = false cover = "dragon.jpg" +++

+++

##### 不管新增/修改/删除文章,始终是在本地文件中直接修改,然后设置:

git add .
git commit -m "新增文章：hugo部署使用方法"
git push

##### 前两句指令可以直接合并为 git commit -am "修改文章：Hugo部署使用方法"

`git commit -am` 【不能用于新增文章 / 新增文件】，只能用于「修改 / 删除」

##### 查看更新日志:

git log

##### 为文章设置封面:

 cover: "104859212_p0.jpg"  # ✅ 新增这一行！就是你的图片文件名 ---

##### hugo -D的作用:

设置文章draft:false

当然yaml中我们已经设置,提交的时候不用写

##### 控制台创建新文章指令:

hugo new content post/PassageName/index.md 

##### 重装系统的时候:

没有任何配置文件/注册表,所以hugo文件夹压缩之后,直接可以重新用\

对路径也没有要求,只需要在主目录打开文件夹

