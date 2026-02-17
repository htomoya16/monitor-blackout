using System.Drawing;

namespace MonitorBlackout.Common;

// 対象モニタを識別するための最小情報（デバイス名と境界座標）。
public readonly record struct MonitorTarget(string DeviceName, Rectangle Bounds);
