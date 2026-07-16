<#
批量处理PNG图片文件，每个PNG对应一篇文章
#>

$pngSourcePath = "F:\pdfs"
$pngDestPath = "d:\_StudyAPP\Hugo\dev\static\images\posts"
$postBasePath = "d:\_StudyAPP\Hugo\dev\content\post"

# 创建目标目录
if (-not (Test-Path $pngDestPath)) {
    New-Item -ItemType Directory -Path $pngDestPath | Out-Null
    Write-Host "创建图片存放目录: $pngDestPath"
}

# 获取所有PNG文件
$pngFiles = Get-ChildItem -Path $pngSourcePath -Filter "*.png" -File
$processedCount = 0

foreach ($pngFile in $pngFiles) {
    # 清理文件名作为目录名
    $cleanName = $pngFile.BaseName -replace '[^\w\s\-_]', '' -replace '\s+', '_'
    $cleanName = $cleanName.Trim('_')
    
    # 文章目录
    $articleDir = Join-Path $postBasePath $cleanName
    if (-not (Test-Path $articleDir)) {
        New-Item -ItemType Directory -Path $articleDir | Out-Null
    }
    
    # 复制PNG到static目录
    $pngDestFile = Join-Path $pngDestPath $pngFile.Name
    if (-not (Test-Path $pngDestFile)) {
        Copy-Item -Path $pngFile.FullName -Destination $pngDestFile -Force
        Write-Host "复制图片: $($pngFile.Name)"
    }
    
    # 创建文章
    $articleFile = Join-Path $articleDir "index.md"
    if (-not (Test-Path $articleFile)) {
        $title = $pngFile.BaseName
        $frontMatter = @"
+++
title = "$title"
date = "$(Get-Date -Format "yyyy-MM-ddTHH:mm:ss+08:00")"
draft = false
categories = ["图片分享"]
tags = ["笔记"]
+++

# $title

![$title](/images/posts/$($pngFile.Name))
"@
        
        # 保存为UTF-8 without BOM
        $utf8 = New-Object System.Text.UTF8Encoding $false
        [System.IO.File]::WriteAllText($articleFile, $frontMatter, $utf8)
        $processedCount++
        Write-Host "创建文章: $cleanName"
    }
}

Write-Host "`n处理完成！共处理 $processedCount 个PNG文件"
