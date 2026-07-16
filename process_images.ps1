<#
批量处理PNG图片文件，每个PNG对应一篇文章
使用简单的ASCII文件名
#>

$pngSourcePath = "F:\pdfs"
$pngDestPath = "d:\_StudyAPP\Hugo\dev\static\images\posts"
$postBasePath = "d:\_StudyAPP\Hugo\dev\content\post"

# 创建目标目录
if (-not (Test-Path $pngDestPath)) {
    New-Item -ItemType Directory -Path $pngDestPath | Out-Null
    Write-Host "Created images dir: $pngDestPath"
}

# Get all PNG files
$pngFiles = Get-ChildItem -Path $pngSourcePath -Filter "*.png" -File
$processedCount = 0

foreach ($pngFile in $pngFiles) {
    # Clean filename for directory
    $cleanName = $pngFile.BaseName -replace '[^\w\s\-_]', '' -replace '\s+', '_'
    $cleanName = $cleanName.Trim('_')
    
    # Article directory
    $articleDir = Join-Path $postBasePath $cleanName
    if (-not (Test-Path $articleDir)) {
        New-Item -ItemType Directory -Path $articleDir | Out-Null
    }
    
    # Copy PNG
    $pngDestFile = Join-Path $pngDestPath $pngFile.Name
    if (-not (Test-Path $pngDestFile)) {
        Copy-Item -Path $pngFile.FullName -Destination $pngDestFile -Force
        Write-Host "Copied image: $($pngFile.Name)"
    }
    
    # Create article - use Write tool directly
    $articleFile = Join-Path $articleDir "index.md"
    if (-not (Test-Path $articleFile)) {
        $title = $pngFile.BaseName
        
        # Simple article content
        $content = @"
+++
title = `"$title`"
date = `"$(Get-Date -Format `"yyyy-MM-ddTHH:mm:ss+08:00)`"
draft = false
categories = [`"图片分享`"]
tags = [`"笔记`"]
+++

# $title

![$title](/images/posts/$($pngFile.Name))
"@
        
        # Save using .NET method with UTF8 without BOM
        $utf8NoBom = New-Object System.Text.UTF8Encoding $false
        [System.IO.File]::WriteAllText($articleFile, $content, $utf8NoBom)
        $processedCount++
        Write-Host "Created article: $cleanName"
    }
}

Write-Host "`nDone! Processed $processedCount PNG files"
