using MonitorBlackout.Common;

namespace MonitorBlackout.Overlay;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var monitor = ResolveMonitor(args);
        if (monitor is null)
        {
            return;
        }

        var target = monitor.Value;
        var mutexName = OverlayNaming.CreateMutexName(target.DeviceName);
        var closeEventName = OverlayNaming.CreateCloseEventName(target.DeviceName);

        var instanceMutex = new Mutex(initiallyOwned: true, mutexName, out var createdNew);
        if (!createdNew)
        {
            instanceMutex.Dispose();
            return;
        }

        var closeEvent = new EventWaitHandle(false, EventResetMode.AutoReset, closeEventName);

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new OverlayForm(target, instanceMutex, closeEvent));
    }

    private static MonitorTarget? ResolveMonitor(string[] args)
    {
        var requestedDeviceName = OverlayArguments.ParseDeviceName(args);
        if (!string.IsNullOrWhiteSpace(requestedDeviceName) &&
            MonitorResolver.TryGetMonitorByDeviceName(requestedDeviceName, out var requestedMonitor))
        {
            return requestedMonitor;
        }

        if (MonitorResolver.TryGetForegroundMonitor(out var foregroundMonitor))
        {
            return foregroundMonitor;
        }

        return null;
    }
}
