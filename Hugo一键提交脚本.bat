@echo off
chcp 65001
title Hugo博客一键提交脚本
cls
echo =====================================
echo      Hugo博客 一键提交/上传脚本
echo      无需手动输命令，自动完成所有操作
echo =====================================
echo.
echo 正在执行：git add . （暂存所有修改/新增/删除的文件）
git add .
echo ✔️ 暂存完成！
echo.

set /p commit_msg=请输入本次提交的备注（例如：修改文章：Hugo部署使用方法）：
echo.
echo 正在执行：git commit -m "%commit_msg%" （本地提交）
git commit -m "%commit_msg%"
echo ✔️ 本地提交完成！
echo.

echo 正在执行：git push （推送到GitHub，自动部署博客）
git push
echo.
echo =====================================
echo ✅ 全部操作完成！博客已自动更新！
echo ✅ 打开博客按 Ctrl+F5 强制刷新即可看到最新内容
echo =====================================
pause