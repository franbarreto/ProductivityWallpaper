<#
.SYNOPSIS
  Genera las versiones portables de Productivity Wallpaper.

.DESCRIPTION
  Ejecutar desde la carpeta del proyecto (la que tiene ProductivityWallpaper.sln):
      .\Crear-Portable.ps1

  Requisitos: SDK de .NET 8 instalado (dotnet --list-sdks debe mostrar 8.x).

  Resultado, dentro de .\portable\ :
    ProductivityWallpaper-Portable-Autonomo\   carpeta + .zip (incluye el runtime; funciona en cualquier PC)
    ProductivityWallpaper-Portable-Liviana\    carpeta + .zip (necesita el Runtime de escritorio de .NET 8)

  Cada carpeta lleva portable.flag (datos junto a la app), Iniciar.bat y LEEME.txt.
#>
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

$sdks = & dotnet --list-sdks
if (-not ($sdks -match "^8\.")) {
    throw "Hace falta el SDK de .NET 8. Instalalo con: winget install Microsoft.DotNet.SDK.8"
}

$project = "src/ProductivityWallpaper.App"
$out = Join-Path $PSScriptRoot "portable"
if (Test-Path $out) { Remove-Item $out -Recurse -Force }
New-Item -ItemType Directory $out | Out-Null

function Build-Portable([string]$Name, [bool]$SelfContained) {
    $dir = Join-Path $out $Name
    $flag = if ($SelfContained) { "true" } else { "false" }
    Write-Host "== Compilando $Name ..." -ForegroundColor Cyan

    & dotnet publish $project -c Release -r win-x64 --self-contained $flag -o $dir
    if ($LASTEXITCODE -ne 0) { throw "Fallo dotnet publish ($Name)" }

    # Archivos propios de la version portable.
    New-Item -ItemType File -Path (Join-Path $dir "portable.flag") | Out-Null
    Copy-Item "portable-files\Iniciar.bat" $dir
    Copy-Item "portable-files\LEEME.txt" $dir

    # Quitar archivos de depuracion que no hacen falta para usar la app.
    Get-ChildItem $dir -Filter *.pdb -Recurse | Remove-Item -Force

    $zip = Join-Path $out "$Name.zip"
    Compress-Archive -Path (Join-Path $dir "*") -DestinationPath $zip -Force
    Write-Host ("   Listo: {0} ({1:N1} MB)" -f $zip, ((Get-Item $zip).Length / 1MB)) -ForegroundColor Green
}

Build-Portable "ProductivityWallpaper-Portable-Autonomo" $true
Build-Portable "ProductivityWallpaper-Portable-Liviana"  $false

Write-Host ""
Write-Host "Terminado. Mira la carpeta: $out" -ForegroundColor Green
Write-Host "Para probar: entra a una de las carpetas y abre Iniciar.bat"
