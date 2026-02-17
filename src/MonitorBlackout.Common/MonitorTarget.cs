using System.Drawing;

namespace MonitorBlackout.Common;

public readonly record struct MonitorTarget(string DeviceName, Rectangle Bounds);
