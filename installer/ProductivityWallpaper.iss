; Script de Inno Setup para generar el instalador de Productivity Wallpaper.
;
; Qué hace:
;  - Empaqueta todo lo que haya en la carpeta "publish\" (el resultado de
;    `dotnet publish`, self-contained) dentro de un instalador .exe.
;  - Crea accesos directos en el Menú Inicio y, opcionalmente, en el
;    Escritorio.
;  - Deja un desinstalador real (Panel de control > Programas).
;  - No necesita que la persona que lo instala tenga .NET: el publish ya
;    es self-contained.
;
; Cómo generarlo (en Windows):
;  1. Instalá Inno Setup (gratis): https://jrsoftware.org/isdl.php
;  2. Generá el publish self-contained como siempre:
;       dotnet publish src/ProductivityWallpaper.App -c Release -r win-x64 ^
;         --self-contained true -p:PublishSingleFile=true -o publish
;  3. Abrí este archivo (ProductivityWallpaper.iss) con Inno Setup Compiler
;     y presioná "Compile" (o F9).
;  4. El instalador queda en installer\output\ProductivityWallpaperSetup.exe
;
; Este .iss asume que se compila desde la carpeta "installer\" del proyecto,
; con "publish\" como hermana de "src\" (la ubicación por defecto del
; comando de arriba).

#define MyAppName "Productivity Wallpaper"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Productivity Wallpaper"
#define MyAppExeName "ProductivityWallpaper.exe"

[Setup]
AppId={{B6F2B6C4-6C2E-4C9C-9B5D-9B6C2A6B9A11}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
; Instala para el usuario actual por defecto: no requiere ser administrador.
PrivilegesRequired=lowest
OutputDir=output
OutputBaseFilename=ProductivityWallpaperSetup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
; El icono del instalador y del desinstalador en "Agregar o quitar programas".
SetupIconFile=..\src\ProductivityWallpaper.App\Assets\app.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"
Name: "startupicon"; Description: "Iniciar Productivity Wallpaper automáticamente con Windows"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Todo el contenido del publish self-contained, carpetas incluidas
; (imágenes del catálogo, DLLs nativas, el .exe, etc.)
Source: "..\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Desinstalar {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
; Acceso directo en la carpeta de inicio de Windows: es el mismo mecanismo
; que el checkbox "Iniciar con Windows" de la app usa vía el registro, pero
; ofrecerlo también acá le da a la persona la opción de activarlo desde el
; instalador sin tener que abrir la app primero.
Name: "{userstartup}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: startupicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
; El icono de la bandeja del sistema deja de mostrarse al desinstalar porque
; el proceso se cierra; esto limpia además el archivo de preferencias local
; para que una reinstalación arranque desde cero si la persona lo prefiere.
; (Comentado por defecto: lo normal es conservar las preferencias.)
; Type: filesandordirs; Name: "{localappdata}\ProductivityWallpaper"
