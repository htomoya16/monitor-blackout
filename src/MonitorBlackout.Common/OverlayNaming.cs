using System.Security.Cryptography;
using System.Text;

namespace MonitorBlackout.Common;

public static class OverlayNaming
{
    public static string CreateMutexName(string deviceName) =>
        $"Local\\MonitorBlackout.Overlay.{CreateDeviceHash(deviceName)}";

    public static string CreateCloseEventName(string deviceName) =>
        $"Local\\MonitorBlackout.Close.{CreateDeviceHash(deviceName)}";

    private static string CreateDeviceHash(string deviceName)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(deviceName));
        return Convert.ToHexString(bytes.AsSpan(0, 8));
    }
}
