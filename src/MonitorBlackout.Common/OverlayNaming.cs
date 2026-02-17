using System.Security.Cryptography;
using System.Text;

namespace MonitorBlackout.Common;

public static class OverlayNaming
{
    // モニタ単位で一意な Overlay 監視用 Mutex 名を作る。
    public static string CreateMutexName(string deviceName) =>
        $"Local\\MonitorBlackout.Overlay.{CreateDeviceHash(deviceName)}";

    // モニタ単位で一意な Overlay 終了通知用 Event 名を作る。
    public static string CreateCloseEventName(string deviceName) =>
        $"Local\\MonitorBlackout.Close.{CreateDeviceHash(deviceName)}";

    // デバイス名は長くなるため、固定長ハッシュにして OS オブジェクト名を安定化する。
    private static string CreateDeviceHash(string deviceName)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(deviceName));
        return Convert.ToHexString(bytes.AsSpan(0, 8));
    }
}
