// Tener UseWPF y UseWindowsForms activados a la vez en el mismo proyecto
// (necesario para el ícono de la bandeja del sistema, NotifyIcon) hace que
// el SDK agregue automáticamente "usings" implícitos para los dos mundos:
// System.Windows(.Controls) (WPF) y System.Windows.Forms + System.Drawing
// (Windows Forms). Varios tipos existen en ambos mundos con el mismo
// nombre (Application, UserControl, Color, ColorConverter, Brushes...),
// así que quedan ambiguos en cualquier archivo que no los califique
// explícitamente.
//
// La solución es declarar acá, una sola vez, a cuál de los dos nos
// referimos cuando escribimos estos nombres sin calificar: siempre a la
// versión de WPF, porque toda la UI de esta app es WPF. El código que sí
// necesita la versión de Windows Forms (como TrayIconService, para
// NotifyIcon/ToolStripMenuItem) usa su propio alias local
// "WinForms = System.Windows.Forms" para esos tipos puntuales, y
// System.Drawing.Icon se referencia siempre completamente calificado —
// ninguno de los dos choca con los alias de acá.
//
// Si en el futuro aparece un nuevo error CS0104 por otro tipo compartido
// entre System.Drawing/Forms y WPF (Point, Size, Rectangle, Font, Image...),
// se agrega acá la misma forma de alias apuntando a la versión de WPF.
global using Application = System.Windows.Application;
global using UserControl = System.Windows.Controls.UserControl;
global using Color = System.Windows.Media.Color;
global using ColorConverter = System.Windows.Media.ColorConverter;
global using Brushes = System.Windows.Media.Brushes;
global using MessageBox = System.Windows.MessageBox;
global using Brush = System.Windows.Media.Brush;
