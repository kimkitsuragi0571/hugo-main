<#
批量处理PDF文件，每个PDF对应一篇文章
#>

$pdfSourcePath = "F:\pdfs"
$pdfDestPath = "d:\_StudyAPP\Hugo\dev\static\pdfs"
$postBasePath = "d:\_StudyAPP\Hugo\dev\content\post"

# 创建目标目录
if (-not (Test-Path $pdfDestPath)) {
    New-Item -ItemType Directory -Path $pdfDestPath | Out-Null
    Write-Host "创建PDF存放目录: $pdfDestPath"
}

# 获取所有PDF文件
$pdfFiles = Get-ChildItem -Path $pdfSourcePath -Filter "*.pdf" -File
$processedCount = 0

foreach ($pdfFile in $pdfFiles) {
    # 清理文件名作为目录名
    $cleanName = $pdfFile.BaseName -replace '[^\w\s\-_]', '' -replace '\s+', '_'
    $cleanName = $cleanName.Trim('_')
    
    # 文章目录
    $articleDir = Join-Path $postBasePath $cleanName
    if (-not (Test-Path $articleDir)) {
        New-Item -ItemType Directory -Path $articleDir | Out-Null
    }
    
    # 复制PDF到static目录
    $pdfDestFile = Join-Path $pdfDestPath $pdfFile.Name
    if (-not (Test-Path $pdfDestFile)) {
        Copy-Item -Path $pdfFile.FullName -Destination $pdfDestFile -Force
        Write-Host "复制PDF: $($pdfFile.Name)"
    }
    
    # 创建文章
    $articleFile = Join-Path $articleDir "index.md"
    if (-not (Test-Path $articleFile)) {
        $title = $pdfFile.BaseName
        $frontMatter = @"
+++
title = "$title"
date = "$(Get-Date -Format "yyyy-MM-ddTHH:mm:ss+08:00")"
draft = false
categories = ["PDF文档"]
tags = ["笔记"]
+++

# $title

<iframe src="/pdfs/$($pdfFile.Name)" width="100%" height="800px"></iframe>

<p style="text-align: center;">
    <a href="/pdfs/$($pdfFile.Name)" target="_blank">
        🔗 在新标签页中打开PDF
    </a>
</p>
"@
        
        # 保存为UTF-8 without BOM
        $utf8 = New-Object System.Text.UTF8Encoding $false
        [System.IO.File]::WriteAllText($articleFile, $frontMatter, $utf8)
        $processedCount++
        Write-Host "创建文章: $cleanName"
    }
}

Write-Host "`n处理完成！共处理 $processedCount 个PDF文件"
