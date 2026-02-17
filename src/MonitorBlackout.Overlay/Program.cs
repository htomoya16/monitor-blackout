using MonitorBlackout.Common;

namespace MonitorBlackout.Overlay;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        // 1) 引数または現在のアクティブ状態から対象モニタを解決する。
        var monitor = ResolveMonitor(args);
        if (monitor is null)
        {
            return;
        }

        var target = monitor.Value;
        var mutexName = OverlayNaming.CreateMutexName(target.DeviceName);
        var closeEventName = OverlayNaming.CreateCloseEventName(target.DeviceName);

        // 2) 同一モニタで Overlay が重複起動しないよう Mutex を確保する。
        var instanceMutex = new Mutex(initiallyOwned: true, mutexName, out var createdNew);
        if (!createdNew)
        {
            instanceMutex.Dispose();
            return;
        }

        // 3) Toggle 側からの OFF 指示を受けるための Event を作る。
        var closeEvent = new EventWaitHandle(false, EventResetMode.AutoReset, closeEventName);

        // 4) 黒幕フォームを起動して表示維持する。
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new OverlayForm(target, instanceMutex, closeEvent));
    }

    private static MonitorTarget? ResolveMonitor(string[] args)
    {
        // 引数でデバイス名が渡された場合はそれを優先する。
        var requestedDeviceName = OverlayArguments.ParseDeviceName(args);
        if (!string.IsNullOrWhiteSpace(requestedDeviceName) &&
            MonitorResolver.TryGetMonitorByDeviceName(requestedDeviceName, out var requestedMonitor))
        {
            return requestedMonitor;
        }

        // 引数解決できない場合は、現在アクティブなウィンドウのモニタを使う。
        if (MonitorResolver.TryGetForegroundMonitor(out var foregroundMonitor))
        {
            return foregroundMonitor;
        }

        return null;
    }
}
