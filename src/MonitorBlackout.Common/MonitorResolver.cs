using System.Drawing;

namespace MonitorBlackout.Common;

public static class MonitorResolver
{
    // モニタ取得失敗時に、最も近いモニタを返す Win32 フラグ。
    private const uint MonitorDefaultToNearest = 0x00000002;

    // 現在アクティブなウィンドウから対象モニタを取得する。
    public static bool TryGetForegroundMonitor(out MonitorTarget monitor)
    {
        var foregroundWindow = NativeMethods.GetForegroundWindow();
        if (foregroundWindow == IntPtr.Zero)
        {
            monitor = default;
            return false;
        }

        return TryGetMonitorFromWindow(foregroundWindow, out monitor);
    }

    // デバイス名（例: \\.\DISPLAY2）から該当モニタを列挙して探す。
    public static bool TryGetMonitorByDeviceName(string deviceName, out MonitorTarget monitor)
    {
        monitor = default;
        MonitorTarget? match = null;

        NativeMethods.EnumDisplayMonitors(
            IntPtr.Zero,
            IntPtr.Zero,
            (hMonitor, _, _, _) =>
            {
                if (TryReadMonitorInfo(hMonitor, out var candidate) &&
                    string.Equals(candidate.DeviceName, deviceName, StringComparison.OrdinalIgnoreCase))
                {
                    match = candidate;
                    return false;
                }

                return true;
            },
            IntPtr.Zero);

        if (match is null)
        {
            return false;
        }

        monitor = match.Value;
        return true;
    }

    // ウィンドウハンドルから所属モニタを取得する。
    private static bool TryGetMonitorFromWindow(IntPtr windowHandle, out MonitorTarget monitor)
    {
        var monitorHandle = NativeMethods.MonitorFromWindow(windowHandle, MonitorDefaultToNearest);
        if (monitorHandle == IntPtr.Zero)
        {
            monitor = default;
            return false;
        }

        return TryReadMonitorInfo(monitorHandle, out monitor);
    }

    // モニタハンドルからデバイス名と境界座標を読み取る。
    private static bool TryReadMonitorInfo(IntPtr monitorHandle, out MonitorTarget monitor)
    {
        var info = NativeMethods.MONITORINFOEX.Create();
        if (!NativeMethods.GetMonitorInfo(monitorHandle, ref info))
        {
            monitor = default;
            return false;
        }

        monitor = new MonitorTarget(
            info.szDevice,
            Rectangle.FromLTRB(
                info.rcMonitor.Left,
                info.rcMonitor.Top,
                info.rcMonitor.Right,
                info.rcMonitor.Bottom));

        return true;
    }
}
