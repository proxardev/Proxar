<#
.SYNOPSIS
    清理并重新生成 DocFX 文档。
.DESCRIPTION
    删除 _site 和 api 目录，然后执行 docfx metadata 和 docfx build。
    适用于包含 docfx.json 的目录。
#>

# 设置错误处理：遇到错误立即停止
$ErrorActionPreference = "Stop"

Write-Host "Cleaning old generated files..." -ForegroundColor Cyan

# 删除 _site 目录（如果存在）
if (Test-Path "_site") {
    Remove-Item -Recurse -Force "_site"
    Write-Host "Removed _site"
}

# 删除 api 目录（如果存在）
if (Test-Path "api") {
    Remove-Item -Recurse -Force "api"
    Write-Host "Removed api"
}

Write-Host "`nRunning docfx metadata..." -ForegroundColor Cyan
docfx metadata
if ($LASTEXITCODE -ne 0) {
    Write-Error "docfx metadata failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "`nRunning docfx build..." -ForegroundColor Cyan
docfx build
if ($LASTEXITCODE -ne 0) {
    Write-Error "docfx build failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "`nDocumentation build completed successfully." -ForegroundColor Green