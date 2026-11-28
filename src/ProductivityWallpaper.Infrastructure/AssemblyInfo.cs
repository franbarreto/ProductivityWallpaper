using System.Runtime.InteropServices;

// Las llamadas nativas (P/Invoke) solo buscan sus DLL en System32: evita que
// una DLL falsa con el mismo nombre, puesta junto a la app, se cargue en su lugar.
[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
