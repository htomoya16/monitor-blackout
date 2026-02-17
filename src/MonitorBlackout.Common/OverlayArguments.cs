namespace MonitorBlackout.Common;

public static class OverlayArguments
{
    // Overlay 起動引数から `--device <name>` を取り出す。
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
