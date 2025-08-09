namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor;

public static class SnapHelper
{
    public static double Snap(double value, double snap)
    {
        if (snap == 0.0)
        {
            return value;
        }

        var c = value % snap;
        var r = c >= snap / 2.0 ? value + snap - c : value - c;
        return r;
    }

    public static Point Snap(Point point, double snapX, double snapY, bool enabled)
    {
        if (enabled)
        {
            var pointX = Snap(point.X, snapX);
            var pointY = Snap(point.Y, snapY);
            return new(pointX, pointY);
        }

        return point;
    }
}
