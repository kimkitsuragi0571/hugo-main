@echo off
title Hugo博客启动服务
cd /d "%~dp0"
start http://localhost:1313/
hugo server -D