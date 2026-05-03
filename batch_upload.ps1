<#
Batch upload MD files to Hugo blog
#>

$destinationBase = "d:\_StudyAPP\Hugo\dev\content\post"

$csharpPath = "E:\_EDownL\C#语法文件总结"
$unityPath = "E:\_EDownL\Unity知识点总结"

$processedCount = 0

# Process C# files
if (Test-Path $csharpPath) {
    $csharpFiles = Get-ChildItem -Path $csharpPath -Filter "*.md" -File
    Write-Host "Processing C# files: $($csharpFiles.Count) files"
    
    foreach ($file in $csharpFiles) {
        $cleanName = $file.BaseName -replace '[#\.\s]', '_'
        $cleanName = $cleanName.TrimEnd('_')
        
        $articleDir = Join-Path $destinationBase $cleanName
        if (-not (Test-Path $articleDir)) {
            New-Item -ItemType Directory -Path $articleDir | Out-Null
        }
        
        $mdDestination = Join-Path $articleDir "index.md"
        Copy-Item -Path $file.FullName -Destination $mdDestination -Force
        
        $content = Get-Content $mdDestination -Raw -Encoding UTF8
        
        $title = $file.BaseName -replace '^\d+[\.\-_]', ''
        $title = $title -replace '[\.\-_]', ' '
        $title = $title.Trim()
        
        $frontMatter = @"
+++
title = "$title"
date = "$(Get-Date -Format "yyyy-MM-ddTHH:mm:ss+08:00")"
draft = false
categories = ["C-Sharp"]
tags = ["Notes"]
+++

"@
        
        $newContent = $frontMatter + $content
        Set-Content -Path $mdDestination -Value $newContent -NoNewline -Encoding UTF8
        
        $processedCount++
        Write-Host "Created: $title"
    }
}

# Process Unity files
if (Test-Path $unityPath) {
    $unityFiles = Get-ChildItem -Path $unityPath -Filter "*.md" -File
    Write-Host "Processing Unity files: $($unityFiles.Count) files"
    
    foreach ($file in $unityFiles) {
        $cleanName = $file.BaseName -replace '[#\.\s]', '_'
        $cleanName = $cleanName.TrimEnd('_')
        
        $articleDir = Join-Path $destinationBase $cleanName
        if (-not (Test-Path $articleDir)) {
            New-Item -ItemType Directory -Path $articleDir | Out-Null
        }
        
        $mdDestination = Join-Path $articleDir "index.md"
        Copy-Item -Path $file.FullName -Destination $mdDestination -Force
        
        $content = Get-Content $mdDestination -Raw -Encoding UTF8
        
        $title = $file.BaseName -replace '^\d+[\.\-_]', ''
        $title = $title -replace '[\.\-_]', ' '
        $title = $title.Trim()
        
        $frontMatter = @"
+++
title = "$title"
date = "$(Get-Date -Format "yyyy-MM-ddTHH:mm:ss+08:00")"
draft = false
categories = ["Unity"]
tags = ["Notes"]
+++

"@
        
        $newContent = $frontMatter + $content
        Set-Content -Path $mdDestination -Value $newContent -NoNewline -Encoding UTF8
        
        $processedCount++
        Write-Host "Created: $title"
    }
}

Write-Host "Batch upload completed! Processed $processedCount files"