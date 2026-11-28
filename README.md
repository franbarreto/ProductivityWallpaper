# Productivity Wallpaper

App de escritorio para **Windows 10/11** que convierte tu fondo de pantalla en un recordatorio de lo que querés tener presente. Elegís tus intereses, un estilo visual y cada cuánto querés que cambie, y la app aplica automáticamente un wallpaper con una frase acorde.

Funciona **sin conexión a internet, sin cuentas y sin recopilar datos**. Hecha con C#, .NET 8 y WPF.

## Características

- 92 frases en **español e inglés**, con selección por categoría y estilo, sin repeticiones seguidas.
- Intereses: disciplina, motivación, enfoque, estudio, trabajo, entrenamiento, productividad, estoicismo, fe y superación personal.
- Estilos: minimalista, oscuro, claro, naturaleza, abstracto y elegante.
- Cambio automático (cada 15 min, 30 min, 1 h, 3 h o una vez al día) o manual, con pausa y reanudación.
- Se ejecuta en segundo plano desde la **bandeja del sistema** y puede iniciarse con Windows.
- **Widgets de escritorio**: reloj (en tamaño grande sirve como "hora de fondo") y progreso del día. Se arrastran y recuerdan su posición.
- **Modo de rendimiento Bajo o Alto**, para equipos modestos.
- Interfaz minimalista estilo iOS, con animaciones.
- **Versión portable**: guarda sus datos junto a la app y no toca tu perfil de Windows.

## Descargar

Andá a la sección [**Releases**](../../releases/latest) y bajá uno de estos archivos:

| Archivo | Para qué |
|---|---|
| `ProductivityWallpaper-X.Y.Z-Autonomo.zip` | **Recomendado.** Incluye todo lo necesario, no hace falta instalar nada. |
| `ProductivityWallpaper-X.Y.Z-Liviana.zip` | Pesa mucho menos, pero necesita el [Runtime de escritorio de .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0). |
| `SHA256SUMS.txt` | Huellas para verificar que la descarga no fue alterada. |

Descomprimí la carpeta donde quieras y abrí `Iniciar.bat` (o `ProductivityWallpaper.exe`). Para desinstalarla, elegí **Salir** en el ícono de la bandeja y borrá la carpeta.

### Si Windows muestra "Windows protegió su PC"

La app **todavía no tiene firma digital**, por eso SmartScreen la advierte con cualquier programa nuevo. Hacé clic en **Más información** y después en **Ejecutar de todas formas**.

Para comprobar que el archivo es el original, comparalo con `SHA256SUMS.txt`:

```powershell
Get-FileHash .\ProductivityWallpaper-X.Y.Z-Autonomo.zip -Algorithm SHA256
```

En equipos de empresas o instituciones, una política de seguridad (por ejemplo, una regla de Microsoft Defender que bloquea ejecutables poco conocidos) puede impedir abrir el `.exe`. En ese caso usá la versión **Liviana** con `Iniciar.bat`, que abre la app a través de `dotnet`.

## Compilar desde el código

Requisitos: Windows 10/11 y el [SDK de .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0).

```powershell
git clone https://github.com/TU_USUARIO/ProductivityWallpaper.git
cd ProductivityWallpaper
dotnet run --project src/ProductivityWallpaper.App
```

Para generar las versiones portables (`portable\*.zip`):

```powershell
powershell -ExecutionPolicy Bypass -File .\Crear-Portable.ps1
```

El instalador (Inno Setup) está en `installer/`. Los pasos detallados y las pruebas manuales están en [docs/NOTAS-DE-DESARROLLO.md](docs/NOTAS-DE-DESARROLLO.md).

## Seguridad y privacidad

- No usa internet ni envía datos a ningún lado.
- No pide permisos de administrador.
- Solo cambia el fondo con las imágenes que trae en su propia carpeta `Content`.
- Instancia única, preferencias validadas al cargar y guardadas de forma atómica, y errores registrados en un archivo local (`error.log`).

## Estructura

```
src/
  ProductivityWallpaper.App/             Interfaz (WPF, MVVM), widgets y bandeja del sistema
  ProductivityWallpaper.Core/            Modelos y lógica de selección, sin dependencias de Windows
  ProductivityWallpaper.Infrastructure/  Wallpaper de Windows, preferencias, inicio con Windows
installer/                               Script de Inno Setup
```

## Licencia

[MIT](LICENSE)
