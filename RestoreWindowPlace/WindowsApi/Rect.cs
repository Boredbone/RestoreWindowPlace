using System.Runtime.InteropServices;

namespace RestoreWindowPlace.WindowsApi
{
    /// <summary>
    /// The Rect structure defines the coordinates of the upper-left and lower-right corners of a rectangle.
    /// http://www.pinvoke.net/default.aspx/Structures/RECT.html
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public Rect(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Rect(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

        public int X
        {
            readonly get { return Left; }
            set { Right -= (Left - value); Left = value; }
        }

        public int Y
        {
            readonly get { return Top; }
            set { Bottom -= (Top - value); Top = value; }
        }

        public int Height
        {
            readonly get { return Bottom - Top; }
            set { Bottom = value + Top; }
        }

        public int Width
        {
            readonly get { return Right - Left; }
            set { Right = value + Left; }
        }

        public System.Drawing.Point Location
        {
            readonly get { return new System.Drawing.Point(Left, Top); }
            set { X = value.X; Y = value.Y; }
        }

        public System.Drawing.Size Size
        {
            readonly get { return new System.Drawing.Size(Width, Height); }
            set { Width = value.Width; Height = value.Height; }
        }

        public static implicit operator System.Drawing.Rectangle(Rect r)
        {
            return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
        }

        public static implicit operator Rect(System.Drawing.Rectangle r)
        {
            return new Rect(r);
        }

        public static bool operator ==(Rect r1, Rect r2)
        {
            return r1.Equals(r2);
        }

        public static bool operator !=(Rect r1, Rect r2)
        {
            return !r1.Equals(r2);
        }

        public readonly bool Equals(Rect r)
        {
            return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
        }

        public override readonly bool Equals(object obj)
        {
            if (obj is Rect rect)
                return Equals(rect);
            else if (obj is System.Drawing.Rectangle rectangle)
                return Equals(new Rect(rectangle));
            return false;
        }

        public override readonly int GetHashCode()
        {
            return ((System.Drawing.Rectangle)this).GetHashCode();
        }

        public override readonly string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture,
                $"[Left={Left},Top={Top},Right={Right},Bottom={Bottom}]");
        }
    }
}
