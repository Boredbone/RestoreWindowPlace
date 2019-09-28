using System.Runtime.InteropServices;

namespace RestoreWindowPlace.WindowsApi
{
    /// <summary>
    /// The Point structure defines the x- and y-coordinates of a point.
    /// http://www.pinvoke.net/default.aspx/Structures/POINT.html
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Point(System.Drawing.Point pt) : this(pt.X, pt.Y) { }

        public static implicit operator System.Drawing.Point(Point p)
        {
            return new System.Drawing.Point(p.X, p.Y);
        }

        public static implicit operator Point(System.Drawing.Point p)
        {
            return new Point(p.X, p.Y);
        }
    }
}
