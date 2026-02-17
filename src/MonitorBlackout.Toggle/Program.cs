using System.Diagnostics;
using MonitorBlackout.Common;

if (!MonitorResolver.TryGetForegroundMonitor(out var monitor))
{
    return 1;
}

var mutexName = OverlayNaming.CreateMutexName(monitor.DeviceName);

if (IsOverlayRunning(mutexName))
{
    return SignalOverlayToClose(monitor.DeviceName) ? 0 : 2;
}

return StartOverlay(monitor.DeviceName);

static bool IsOverlayRunning(string mutexName)
{
    try
    {
        using var _ = Mutex.OpenExisting(mutexName);
        return true;
    }
    catch (WaitHandleCannotBeOpenedException)
    {
        return false;
    }
}

static bool SignalOverlayToClose(string deviceName)
{
    try
    {
        using var closeSignal = EventWaitHandle.OpenExisting(OverlayNaming.CreateCloseEventName(deviceName));
        return closeSignal.Set();
    }
    catch (WaitHandleCannotBeOpenedException)
    {
        return false;
    }
}

static int StartOverlay(string deviceName)
{
    var overlayPath = ResolveOverlayPath();
    if (overlayPath is null)
    {
        return 3;
    }

    var info = new ProcessStartInfo
    {
        FileName = overlayPath,
        WorkingDirectory = Path.GetDirectoryName(overlayPath) ?? AppContext.BaseDirectory,
        UseShellExecute = false,
        Arguments = $"--device \"{deviceName}\""
    };

    Process.Start(info);
    return 0;
}

static string? ResolveOverlayPath()
{
    var publishedLayoutPath = Path.Combine(AppContext.BaseDirectory, "MonitorBlackout.Overlay.exe");
    if (File.Exists(publishedLayoutPath))
    {
        return publishedLayoutPath;
    }

    var baseDirectory = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    var tfmDirectory = Path.GetFileName(baseDirectory);
    var configurationDirectory = Directory.GetParent(baseDirectory)?.Name;

    if (string.IsNullOrWhiteSpace(tfmDirectory) || string.IsNullOrWhiteSpace(configurationDirectory))
    {
        return null;
    }

    var debugLayoutPath = Path.GetFullPath(
        Path.Combine(
            baseDirectory,
            "..",
            "..",
            "..",
            "..",
            "MonitorBlackout.Overlay",
            "bin",
            configurationDirectory,
            tfmDirectory,
            "MonitorBlackout.Overlay.exe"));

    return File.Exists(debugLayoutPath) ? debugLayoutPath : null;
}
