using System.Runtime.InteropServices;

namespace MonitorBlackout.Common;

// Monitor 判定に必要な Win32 API 宣言をまとめる。
internal static class NativeMethods
{
    // EnumDisplayMonitors のコールバック型。
    internal delegate bool MonitorEnumProc(
        IntPtr hMonitor,
        IntPtr hdcMonitor,
        IntPtr lprcMonitor,
        IntPtr dwData);

    // 現在フォアグラウンド（アクティブ）なウィンドウを取得する。
    [DllImport("user32.dll")]
    internal static extern IntPtr GetForegroundWindow();

    // ウィンドウハンドルが属するモニタハンドルを取得する。
    [DllImport("user32.dll")]
    internal static extern IntPtr MonitorFromWindow(IntPtr hWnd, uint dwFlags);

    // モニタ情報（座標、デバイス名）を取得する。
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

    // 全モニタを列挙する。
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool EnumDisplayMonitors(
        IntPtr hdc,
        IntPtr lprcClip,
        MonitorEnumProc lpfnEnum,
        IntPtr dwData);

    // Win32 の MONITORINFOEX 構造体マッピング。
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct MONITORINFOEX
    {
        internal uint cbSize;
        internal RECT rcMonitor;
        internal RECT rcWork;
        internal uint dwFlags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        internal string szDevice;

        internal static MONITORINFOEX Create() =>
            new()
            {
                cbSize = (uint)Marshal.SizeOf<MONITORINFOEX>(),
                szDevice = string.Empty
            };
    }

    // Win32 の RECT 構造体マッピング。
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
    }
}
