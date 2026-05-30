# x.ps1
# 用法：在解决方案根目录执行 .\x.ps1
# 功能：扫描 src 目录下的项目，为每个项目生成 THIRD-PARTY-NOTICES.txt（表格格式 + 声明）

param(
    [string]$SolutionDir = ".",
    [string[]]$TargetDirs = @("src"),
    [string[]]$ExcludePatterns = @("*tests*")
)

# 检查 nuget-license 是否可用
$null = Get-Command nuget-license -ErrorAction Stop

# 查找需要处理的项目文件
$projects = Get-ChildItem -Path $SolutionDir -Filter "*.csproj" -Recurse | Where-Object {
    $projDir = $_.DirectoryName
    $inTarget = $false
    foreach ($d in $TargetDirs) {
        if ($projDir -match "[\\/]$([regex]::Escape($d))([\\/]|$)") {
            $inTarget = $true
            break
        }
    }
    if (-not $inTarget) { return $false }
    $name = $_.Name
    foreach ($p in $ExcludePatterns) {
        if ($name -like $p) { return $false }
    }
    return $true
}

foreach ($proj in $projects) {
    $projDir = $proj.DirectoryName
    $outputFile = Join-Path $projDir "THIRD-PARTY-NOTICES.txt"

    Write-Host "Processing: $($proj.FullName)"

    # 直接生成表格文件
    & nuget-license -i $proj.FullName -t -fo $outputFile
    if ($LASTEXITCODE -ne 0 -or -not (Test-Path $outputFile)) {
        Write-Warning "nuget-license 失败，跳过: $($proj.FullName)"
        continue
    }

    # 用 UTF-8 正确读取原始文件（修复乱码的关键）
    $original = Get-Content -Path $outputFile -Raw -Encoding UTF8

    # 声明文本
    $disclaimer = @"

This file lists all direct and transitive package dependencies and their licenses.
Because this package only declares dependency version requirements and does not include their binaries, full license texts are not included.
Please refer to the linked license files for complete terms.


"@

    # 拼接后以 UTF-8 无 BOM 写回（避免后续打开时出现﻿和编码异常）
    $newContent = $disclaimer + $original
    $utf8NoBom = New-Object System.Text.UTF8Encoding $false
    [System.IO.File]::WriteAllText($outputFile, $newContent, $utf8NoBom)

    Write-Host "Generated: $outputFile"
}

Write-Host "All done."