# 个人博客

基于 [Hugo](https://gohugo.io/) + [hugo-theme-stack](https://github.com/CaiJimmy/hugo-theme-stack) 的静态博客。

## 环境要求

本项目需要 Hugo **extended** 版。

- Windows 一键安装：

  ```powershell
  winget install Hugo.Hugo.Extended
  ```

- 或从 [Hugo Releases](https://github.com/gohugoio/hugo/releases) 下载 `hugo.exe`,放到项目根目录,然后双击 `启动脚本.bat`。

## 本地预览

双击 `启动脚本.bat`,或在命令行执行：

```bash
hugo server -D
```

默认预览地址:http://localhost:1313/

## 提交部署

双击 `Hugo一键提交脚本.bat`,输入提交备注后会自动执行 `git add .` → `git commit` → `git push`。

推送到 `main` 分支后,由 GitHub Actions(`.github/workflows/hugo_deploy.yaml`)自动构建并部署到 GitHub Pages。
