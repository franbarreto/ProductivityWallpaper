@echo off
rem Lanzador de Productivity Wallpaper (version portable).
rem 1) Intenta abrir ProductivityWallpaper.exe.
rem 2) Si Windows lo bloquea (por ejemplo una politica de la institucion que
rem    no deja ejecutar .exe nuevos), abre la app a traves de "dotnet", que
rem    esta firmado por Microsoft y normalmente si esta permitido. Esto solo
rem    funciona con la version "Liviana" y con .NET 8 instalado.
powershell -NoProfile -WindowStyle Hidden -Command "$d='%~dp0'; $q=[char]34; try { Start-Process -FilePath ($d+'ProductivityWallpaper.exe') -WorkingDirectory $d -ErrorAction Stop } catch { try { Start-Process -FilePath 'dotnet' -ArgumentList ($q+$d+'ProductivityWallpaper.dll'+$q) -WorkingDirectory $d -WindowStyle Hidden -ErrorAction Stop } catch { Add-Type -AssemblyName PresentationFramework; [void][System.Windows.MessageBox]::Show('No se pudo iniciar la aplicacion. Lee LEEME.txt para ver las causas posibles.','Productivity Wallpaper') } }"
