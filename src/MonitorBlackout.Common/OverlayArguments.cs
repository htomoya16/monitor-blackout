namespace MonitorBlackout.Common;

public static class OverlayArguments
{
    public static string? ParseDeviceName(string[] args)
    {
        for (var i = 0; i < args.Length - 1; i++)
        {
            if (string.Equals(args[i], "--device", StringComparison.OrdinalIgnoreCase))
            {
                return string.IsNullOrWhiteSpace(args[i + 1]) ? null : args[i + 1];
            }
        }

        return null;
    }
}
