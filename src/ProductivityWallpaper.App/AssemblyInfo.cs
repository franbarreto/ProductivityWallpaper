using System.Runtime.InteropServices;

// Las llamadas nativas (P/Invoke) solo buscan sus DLL en System32 (ver Infrastructure).
[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
