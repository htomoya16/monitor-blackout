using System.Diagnostics;
using MonitorBlackout.Common;

// 1) 現在アクティブなウィンドウから、トグル対象モニタを特定する。
if (!MonitorResolver.TryGetForegroundMonitor(out var monitor))
{
    return 1;
}

// 2) モニタごとの Mutex 名を作り、Overlay の起動状態判定に使う。
var mutexName = OverlayNaming.CreateMutexName(monitor.DeviceName);

// 3) 起動中なら終了シグナルを送って OFF、未起動なら Overlay を起動して ON。
if (IsOverlayRunning(mutexName))
{
    return SignalOverlayToClose(monitor.DeviceName) ? 0 : 2;
}

return StartOverlay(monitor.DeviceName);

// このモニタ用 Overlay がすでに動作中かを確認する。
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

// 名前付き Event で、このモニタ用 Overlay に終了要求を送る。
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

// 対象モニタのデバイス名を渡して Overlay を起動する。
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

// publish 配置と debug 配置の両方で Overlay 実行ファイルの場所を解決する。
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
