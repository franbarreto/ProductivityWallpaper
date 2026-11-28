# Productivity Wallpaper

Fase 0 (arquitectura), Fase 1 (configuración inicial), Fase 2 (catálogo y
selección de contenido), Fase 3 (cambio real del fondo de Windows), Fase 4
(automatización: temporizador, cambio automático, pausar/reanudar), Fase 6
(inicio automático con Windows), Fase 7 (pulido: vista previa sin recortes,
imágenes en mayor calidad, atribución de autor, corrección de bugs al
cambiar de intereses/estilo) y Fase 8 (instalador de distribución) ya están
implementadas. También se agregó una pasada de pulido visual (animaciones,
sombras, caché de imágenes, indicador de pasos en la configuración inicial)
y comportamiento en segundo plano estilo Wallpaper Engine (ícono en la
bandeja del sistema).

### Versión portable, seguridad y animaciones (Fase 16)

**Versión portable.** Ejecutá `.\Crear-Portable.ps1` en la carpeta del
proyecto (necesita el SDK de .NET 8). Genera en `portable\` dos carpetas con
su `.zip`: *Autónoma* (incluye el runtime, funciona en cualquier Windows de
64 bits) y *Liviana* (pesa poco, necesita el Runtime de escritorio de .NET 8).
Ambas llevan `portable.flag` (las preferencias y el log se guardan en la
subcarpeta `Data`, sin tocar el perfil de Windows), `Iniciar.bat` y `LEEME.txt`.
`Iniciar.bat` prueba abrir el `.exe` y, si Windows lo bloquea, abre la app con
`dotnet ProductivityWallpaper.dll` (solo versión Liviana).

Límite importante: si una política de la PC (como la regla ASR de Defender que
bloquea ejecutables poco conocidos) impide el `.exe`, no hay nada en el código
que lo evite. Para distribuir el `.exe` a clientes conviene firmarlo con un
certificado de firma de código.

**Seguridad.** Instancia única (abrir la app de nuevo trae la ventana
existente al frente); preferencias validadas al cargar (tamaño máximo,
valores desconocidos, posiciones absurdas) y guardadas de forma atómica; si no
se puede escribir en disco la app sigue funcionando; rutas de imagen del
catálogo restringidas a la carpeta `Content` y solo `.jpg/.jpeg/.png/.bmp`;
las llamadas nativas solo cargan DLL de System32; los errores inesperados van a
`error.log` (con límite de tamaño) y no muestran la pila de llamadas en
pantalla; el inicio con Windows registra la ruta entre comillas.

**Animaciones.** Los widgets aparecen con desvanecimiento y deslizamiento, la
barra de progreso del día se llena suave al actualizarse y la ventana
principal reaparece con fundido al volver de la bandeja. Todas se apagan en
modo de rendimiento Bajo.

**Cómo probarlo.** Abrí la app dos veces seguidas (la segunda debe traer la
primera al frente y no abrir otra). Activá un widget y verificá que aparezca
con animación; pasá a rendimiento Bajo y reactivá el widget: debe aparecer
sin animación. En la carpeta portable, confirmá que aparece `Data\preferences.json`
después de cambiar algo en Configuración.

### Modo de rendimiento, widgets de escritorio y hora en pantalla

En Configuración hay dos secciones nuevas:

- **Rendimiento (Bajo / Alto).** Alto es el comportamiento completo de
  siempre. Bajo apaga las sombras (en WPF son lo más caro de dibujar) y
  las animaciones de entrada, decodifica las imágenes a 640 px en vez de
  1280 px, usa un caché de imágenes más corto (4 en vez de 16) y hace que
  los widgets se actualicen una vez por minuto, sin segundos. El cambio
  se aplica en el momento, sin reiniciar la app.
- **Widgets de escritorio.** Ventanas flotantes sin bordes que se pueden
  arrastrar por el escritorio (la app recuerda dónde las dejaste), no
  aparecen en la barra de tareas ni en Alt+Tab y no le roban el foco a lo
  que estés haciendo:
  - *Reloj:* hora (con segundos opcionales, 24 h o AM/PM) y fecha
    opcional. Con tamaño **Grande** funciona como "hora de fondo".
  - *Progreso del día:* porcentaje de las 24 h que ya pasó, barra y
    tiempo restante.
  - Tamaño Chico / Mediano / Grande y fondo translúcido activable (sin
    fondo, el texto va directo sobre el wallpaper).

Cómo está pensado para gastar poco: hay un único temporizador para todos
los widgets, alineado al cambio de minuto (solo corre cada segundo si el
reloj muestra segundos y el rendimiento es Alto) y se detiene del todo si
no hay ningún widget activo.

Límites conocidos: los widgets son ventanas normales de la app, no están
incrustados detrás de los íconos del escritorio, así que otras ventanas
pueden taparlos. La hora no se "pinta" dentro de la imagen del
wallpaper (eso obligaría a reescribir el fondo de Windows cada minuto).

### Rediseño visual estilo iOS minimalista

Toda la app pasó de un tema oscuro a una paleta clara inspirada en
"Ajustes" de iOS: fondo gris clarísimo (`#F2F2F7`), tarjetas blancas con
sombra suave, azul de sistema (`#0A84FF`) como acento, tipografía
"Segoe UI Variable" (equivalente más cercano a SF Pro en Windows, con
fallback automático a Segoe UI en Windows 10) y esquinas más redondeadas
en botones y tarjetas. También hay un interruptor nuevo tipo iOS
("píldora" con bolita que se desliza, verde cuando está activado) que
reemplaza a los checkboxes cuadrados en los lugares donde antes había un
simple on/off (inicio con Windows). Como todas las pantallas ya tomaban
los colores de `Resources/Styles.xaml` en vez de tener colores sueltos
escritos a mano, el cambio de paleta se aplicó de forma pareja a toda la
app sin tener que tocar cada pantalla por separado.

### Pantalla de configuración (editar preferencias después del onboarding)

En la pantalla principal hay un botón "Configuración" que abre una
pantalla única con todo agrupado en secciones, al estilo de "Ajustes" de
iOS, en vez del wizard de 3 pasos que había antes: Idioma (selector de
dos opciones), Intereses (chips de selección múltiple), Estilo y
Frecuencia (listas de selección única), Rendimiento, Widgets de
escritorio e Inicio con Windows (el interruptor nuevo). Los cambios se guardan a medida que se tocan; el
botón "Listo" en la esquina recalcula y aplica el wallpaper con las
preferencias actualizadas y vuelve a la vista previa. Cambiar el idioma
ahí mismo re-traduce al toque toda la pantalla de configuración, sin
tener que salir y volver a entrar.

### Catálogo ampliado

El catálogo de frases pasó de 52 a 92 (46 pares ES/EN), agregando más
variedad en las categorías que estaban menos cubiertas (Trabajo,
Estudio, Entrenamiento, Fe) y nuevas imágenes para que el contenido no
se repita tan seguido. Se sumaron también dos citas reales más con
atribución (Benjamin Franklin y Marie Curie), siguiendo el mismo
criterio honesto de la Fase 7: solo se atribuye cuando la cita es
auténtica y verificable, y las frases originales del proyecto siguen
sin autor.

### Pulido visual más reciente

- Indicador de pasos ("● ● ○ ○ ○") en las 5 pantallas de configuración
  inicial, para que siempre quede claro en qué paso estás y cuántos faltan.
- Fondo de la ventana con un degradé sutil en vez de color plano.
- Pequeño detalle visual (línea de acento) en la pantalla de bienvenida.

### Pulido reciente (Fase 7)

- La tarjeta de vista previa ya no recorta la imagen: antes, al ser
  panorámica (16:9) dentro de una tarjeta más angosta y alta, se perdían los
  costados (y con ellos parte de la frase). Ahora se ve siempre completa,
  con el color del estilo como marco si no coincide el aspecto.
- Las imágenes del catálogo se regeneraron a 1920×1080 con texto
  anti-aliasado (renderizado al doble de resolución y reducido), para que
  las letras no se vean pixeladas.
- Si una frase es una cita real de alguien conocido (por ejemplo, varios
  filósofos estoicos), debajo aparece su nombre en letra chica e itálica,
  bien sutil. Las frases originales del proyecto no llevan autor.
- Se corrigió un bug real: tildar categorías o estilos en el panel de
  prueba de la pantalla principal podía comportarse de forma errática
  (reconstruía toda la pantalla en medio del propio click). Ahora solo se
  actualiza el contenido mostrado, sin reconstruir nada.

## Requisitos

- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

## Cómo ejecutar (modo desarrollo)

Desde la carpeta raíz del proyecto:

```
dotnet restore
dotnet run --project src/ProductivityWallpaper.App
```

O abriendo `ProductivityWallpaper.sln` en Visual Studio 2022 y presionando F5.

## Cómo generar el ejecutable (.exe) real

Esto arma un `.exe` autocontenido (no necesita que la otra persona tenga
.NET instalado) en una sola carpeta. Corré esto en Windows, desde la raíz
del proyecto:

```
dotnet publish src/ProductivityWallpaper.App -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
```

El `.exe` queda en `publish\ProductivityWallpaper.exe`. Al ejecutarlo
desde esa carpeta, cambia de verdad el fondo de escritorio de Windows.

(Esto no se puede generar desde este entorno de desarrollo porque corre en
Linux sin el SDK de .NET instalado y sin acceso de red a los servidores de
Microsoft — WPF además solo compila en Windows.)

(El flag `UseWindowsForms` que se agregó para el ícono de la bandeja del
sistema es una referencia de framework incluida con el SDK de .NET, no un
paquete NuGet — no cambia nada en el comando de publicación de arriba ni
requiere descargar algo extra.)

## Cómo generar el instalador (Fase 8)

Si en vez de pasar la carpeta `publish\` preferís un instalador de verdad
(con accesos directos, desinstalador y la opción de "iniciar con Windows"
marcable desde el propio instalador), usá el script de Inno Setup incluido
en `installer\ProductivityWallpaper.iss`:

1. Instalá [Inno Setup](https://jrsoftware.org/isdl.php) (gratis, un .exe
   chico).
2. Generá el publish como en el paso anterior (`dotnet publish ...`), para
   que exista la carpeta `publish\` con el `.exe` adentro.
3. Abrí `installer\ProductivityWallpaper.iss` con Inno Setup Compiler y
   presioná **Compile** (o F9).
4. El instalador queda en `installer\output\ProductivityWallpaperSetup.exe`
   — ese es el archivo que le compartís a cualquiera para instalar la app,
   sin que necesite .NET ni nada más.

El instalador no pide permisos de administrador (se instala para el
usuario actual), deja accesos directos en el Menú Inicio y, si se tilda,
en el Escritorio, y un checkbox opcional para iniciar la app automáticamente
con Windows (el mismo mecanismo que el checkbox que ya tiene la app).

## Prueba para verificar que funciona

Al ejecutar, debería abrirse una ventana titulada "Productivity Wallpaper"
que te lleva por la configuración inicial y termina mostrando un wallpaper
real aplicado como fondo de tu escritorio, con un ícono nuevo en la bandeja
del sistema (junto al reloj de Windows).

### Probar la automatización (Fase 4)

- Elegí una frecuencia distinta de "Manual" durante la configuración (por
  ejemplo, cada pocos minutos para probar rápido).
- En la pantalla principal debería verse una cuenta regresiva ("Próximo
  cambio en mm:ss"). Al llegar a cero, el wallpaper cambia solo.
- El botón "Pausar" detiene la cuenta regresiva; "Reanudar" la continúa
  desde donde quedó (no la reinicia).
- Si elegís "Manual", no aparecen cuenta regresiva ni botón de pausa — en
  su lugar se muestra un mensaje indicando que el cambio es manual.
- "Cambiar ahora" fuerza un cambio inmediato y reinicia la cuenta regresiva.

### Probar la pantalla de configuración

- En la pantalla principal, botón "Configuración" (debajo de la tarjeta
  de inicio automático con Windows). Te lleva a una sola pantalla con
  todo agrupado: Idioma, Intereses, Estilo, Frecuencia y el interruptor
  de inicio con Windows.
- Tocar cualquier opción la aplica al instante sobre el borrador de
  preferencias (no hace falta "guardar" cada sección por separado).
- Cambiar el idioma ahí mismo re-traduce toda la pantalla sin salir de
  ella.
- El botón "Listo" en la esquina superior derecha aplica un wallpaper
  nuevo acorde a las preferencias actualizadas, reinicia la cuenta
  regresiva de rotación automática si corresponde, y vuelve a la vista
  previa.

### Probar el inicio automático con Windows (Fase 6)

- En la pantalla principal hay un checkbox "Iniciar automáticamente con
  Windows". Al activarlo, se agrega una entrada en el registro de Windows
  (`HKCU\Software\Microsoft\Windows\CurrentVersion\Run`) que apunta al
  `.exe` publicado; al desactivarlo, se quita.
- **Importante:** esto solo funciona ejecutando el `.exe` publicado
  (`publish\ProductivityWallpaper.exe`), no con `dotnet run` — en modo
  desarrollo el checkbox se revierte solo porque no hay un .exe real al
  que apuntar.
- Para confirmarlo a mano: `Win + R` → `shell:startup` no aplica acá (esa
  carpeta es para accesos directos); en su lugar se puede verificar la
  clave del registro con el Editor del Registro (`regedit`), navegando a
  esa ruta y buscando el valor `ProductivityWallpaper`, o reiniciando
  Windows y viendo que la app se abre sola.

### Probar el comportamiento en segundo plano (estilo Wallpaper Engine)

- Al cerrar la ventana con la "X", la app **no se cierra**: se oculta y
  sigue corriendo (y cambiando el wallpaper) desde el ícono de la bandeja.
  La primera vez que esto pasa se muestra un globo de aviso explicándolo.
- Minimizar la ventana también la oculta a la bandeja.
- Haciendo doble clic en el ícono de la bandeja, o clic derecho → "Mostrar",
  se vuelve a abrir la ventana.
- El menú del ícono de la bandeja (clic derecho) tiene: Mostrar, Cambiar
  ahora, Pausar/Reanudar (si corresponde) y Salir.
- Solo "Salir" desde ese menú cierra la aplicación de verdad.

## Estructura del proyecto

- `src/ProductivityWallpaper.App` — WPF, presentación (Views, ViewModels), catálogo de imágenes en `Content/Wallpapers`
- `src/ProductivityWallpaper.Core` — modelos, interfaces y lógica de selección, sin dependencias de Windows
- `src/ProductivityWallpaper.Infrastructure` — implementaciones concretas: catálogo JSON embebido, persistencia de preferencias, cambio real del wallpaper de Windows (P/Invoke)
