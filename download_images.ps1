<#
批量下载MD文件中的图片到对应文章目录
#>

$postDir = "d:\_StudyAPP\Hugo\dev\content\post"
$downloadedCount = 0
$failedCount = 0

# 获取所有MD文件
$mdFiles = Get-ChildItem -Path $postDir -Recurse -Filter "index.md" -File

foreach ($mdFile in $mdFiles) {
    $articleDir = $mdFile.Directory.FullName
    $content = Get-Content $mdFile.FullName -Raw -Encoding UTF8
    
    # 匹配图片链接格式: ![alt](url)
    $regex = '\!\[.*?\]\((https?://[^)]+)\)'
    $matches = [regex]::Matches($content, $regex)
    
    if ($matches.Count -eq 0) {
        continue
    }
    
    Write-Host "Processing: $($mdFile.Directory.Name)"
    $imageCounter = 0
    $contentUpdated = $content
    
    foreach ($match in $matches) {
        $imageUrl = $match.Groups[1].Value
        
        # 获取文件扩展名
        $extension = [System.IO.Path]::GetExtension($imageUrl)
        if (-not $extension) {
            $extension = ".png"
        }
        
        # 生成文件名
        $fileName = "image_$imageCounter$extension"
        $fileName = $fileName -replace '[^\w\.-]', '_'
        $localPath = Join-Path $articleDir $fileName
        
        try {
            # 创建Web客户端并设置超时
            $webClient = New-Object System.Net.WebClient
            $webClient.Timeout = 30000
            $webClient.DownloadFile($imageUrl, $localPath)
            $webClient.Dispose()
            
            # 更新MD文件中的链接
            $relativePath = "./$fileName"
            $contentUpdated = $contentUpdated.Replace($imageUrl, $relativePath)
            
            $downloadedCount++
            $imageCounter++
            Write-Host "  Downloaded: $fileName"
        } catch {
            $failedCount++
            Write-Host "  Failed: $imageUrl"
        }
    }
    
    # 只有当内容有更新时才写回
    if ($contentUpdated -ne $content) {
        Set-Content -Path $mdFile.FullName -Value $contentUpdated -NoNewline -Encoding UTF8
    }
}

Write-Host "`nDownload completed! Success: $downloadedCount, Failed: $failedCount"