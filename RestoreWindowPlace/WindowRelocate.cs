using RestoreWindowPlace.WindowsApi;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace RestoreWindowPlace
{
    /// <summary>
    /// Operate window using WindowPlacement
    /// </summary>
    internal static class WindowRelocate
    {
        /// <summary>
        /// Set position and size to window
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <param name="position">
        /// Position and size. If width &lt; 0 maximized, if height &lt; 0 minimized. If height and width are 0 ignore size
        /// </param>
        public static void Relocate(IntPtr windowHandle, Rectangle position)
        {
            var placement = new WindowPlacement.WINDOWPLACEMENT();

            placement.Length = Marshal.SizeOf(typeof(WindowPlacement.WINDOWPLACEMENT));

            // Get current placement
            WindowPlacement.GetWindowPlacement(windowHandle, ref placement);

            var normalWidth = placement.NormalPosition.Width;
            var normalHeight = placement.NormalPosition.Height;

            placement.ShowCmd = WindowPlacement.ShowWindowCommands.Restore;
            placement.Flags = 0;
            placement.NormalPosition.Top = position.Top;
            placement.NormalPosition.Left = position.Left;

            ////Restore if minimized
            //if (placement.ShowCmd == WindowPlacement.ShowWindowCommands.ShowMinimized)
            //{
            //    placement.ShowCmd = WindowPlacement.ShowWindowCommands.Normal;
            //}

            var width = position.Width;
            var height = position.Height;

            if (width == 0 || height == 0)
            {
                // ignoring size
                width = normalWidth;
                height = normalHeight;
            }
            else if (width < 0 && height < 0)
            {
                width *= -1;
                height *= -1;
            }
            else if (width < 0)
            {
                width *= -1;
                placement.ShowCmd = WindowPlacement.ShowWindowCommands.Maximize;
            }
            else if (height < 0)
            {
                height *= -1;
                placement.ShowCmd = WindowPlacement.ShowWindowCommands.ShowMinimized;
            }

            placement.NormalPosition.Bottom = placement.NormalPosition.Top + height;
            placement.NormalPosition.Right = placement.NormalPosition.Left + width;

            WindowPlacement.SetWindowPlacement(windowHandle, ref placement);

            // Show at top
            WindowPlacement.SetForegroundWindow(windowHandle);
        }

        /// <summary>
        /// Set position to window
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public static void Relocate(IntPtr windowHandle, int left, int top)
            => Relocate(windowHandle, new Rectangle(left, top, 0, 0));


        /// <summary>
        /// Get position and size of window
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <returns></returns>
        public static Rectangle GetPlace(IntPtr windowHandle)
        {
            var placement = new WindowPlacement.WINDOWPLACEMENT();

            placement.Length = Marshal.SizeOf(typeof(WindowPlacement.WINDOWPLACEMENT));

            WindowPlacement.GetWindowPlacement(windowHandle, ref placement);

            if (placement.ShowCmd != WindowPlacement.ShowWindowCommands.Maximize)
            {
                // Use GetWindowRect Api when Window is not maximized;
                // This Api works correctly when the window is snapped
                var rect = new Rect();
                WindowPlacement.GetWindowRect(windowHandle, ref rect);

                return rect;

            }
            else
            {
                var position = placement.NormalPosition;

                var minimized = placement.ShowCmd == WindowPlacement.ShowWindowCommands.ShowMinimized;
                var maximized = placement.ShowCmd == WindowPlacement.ShowWindowCommands.Maximize;

                return new Rectangle(
                    position.Left,
                    position.Top,
                    (maximized ? -1 : 1) * (position.Right - position.Left),
                    (position.Bottom - position.Top));
            }
        }
    }
}
