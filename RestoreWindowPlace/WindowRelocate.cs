using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace RestoreWindowPlace
{
    /// <summary>
    /// Operate window using WindowPlacement
    /// </summary>
    internal static class WindowRelocate
    {

        /// <summary>
        /// Enumerate all windows
        /// </summary>
        /// <returns></returns>
        public static List<WindowInformation> EnumerateWindows()
        {

            var windows = new List<WindowInformation>();

            WindowPlacement.EnumWindows((handle, parameter) =>
            {
                var sb = new StringBuilder(0x1024);

                if (WindowPlacement.IsWindowVisible(handle) != 0
                    && WindowPlacement.GetWindowText(handle, sb, sb.Capacity) != 0)
                {

                    var window = new WindowInformation()
                    {
                        Title = sb.ToString(),
                        Handle = handle,
                        Id = (int)WindowPlacement.GetWindowThreadProcessId(handle, IntPtr.Zero),
                    };

                    var title = sb.ToString();

                    if (windows.Count == 0
                        || windows.Last().Id != window.Id
                        || windows.Last().Title != window.Title)
                    {
                        windows.Add(window);
                    }

                }
                return 1;
            }, 0);

            if (windows.Count > 0 && windows.Last().Title == "Program Manager")
            {
                windows.RemoveAt(windows.Count - 1);
            }

            return windows;
        }

        /// <summary>
        /// Set position and size to window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="positon">Position and size. If width &lt; 0 maximized, if height &lt; 0 minimized.</param>
        public static void Relocate(this WindowInformation window, Rectangle positon)
        {

            var placement = new WindowPlacement.WINDOWPLACEMENT();

            placement.Length = Marshal.SizeOf(typeof(WindowPlacement.WINDOWPLACEMENT));

            // Get current placement
            WindowPlacement.GetWindowPlacement(window.Handle, ref placement);

            placement.ShowCmd = WindowPlacement.ShowWindowCommands.Restore;
            placement.Flags = 0;
            placement.NormalPosition.Top = positon.Top;
            placement.NormalPosition.Left = positon.Left;

            ////Restore if minimized
            //if (placement.ShowCmd == WindowPlacement.ShowWindowCommands.ShowMinimized)
            //{
            //    placement.ShowCmd = WindowPlacement.ShowWindowCommands.Normal;
            //}

            var width = positon.Width;
            var height = positon.Height;

            if (width == 0 || height == 0)
            {
                width = placement.NormalPosition.Right - placement.NormalPosition.Left;
                height = placement.NormalPosition.Bottom - placement.NormalPosition.Top;
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

            WindowPlacement.SetWindowPlacement(window.Handle, ref placement);

            // Show at top
            WindowPlacement.SetForegroundWindow(window.Handle);
        }

        /// <summary>
        /// Set position to window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public static void Relocate(this WindowInformation window, int left, int top)
        {
            window.Relocate(new Rectangle(left, top, 0, 0));
        }

        /// <summary>
        /// Set position and size to top window
        /// </summary>
        /// <param name="positon"></param>
        public static void RelocateTop(Rectangle positon)
        {
            int thisProcessId = -1;


            WindowInformation window = null;


            WindowPlacement.EnumWindows((handle, parameter) =>
            {
                var sb = new StringBuilder(0x1024);
                if (WindowPlacement.IsWindowVisible(handle) != 0
                    && WindowPlacement.GetWindowText(handle, sb, sb.Capacity) != 0)
                {
                    var id = (int)WindowPlacement.GetWindowThreadProcessId(handle, IntPtr.Zero);

                    if (thisProcessId < 0)
                    {
                        thisProcessId = id;
                    }
                    else if (id != thisProcessId)
                    {
                        window = new WindowInformation()
                        {
                            Title = sb.ToString(),
                            Handle = handle,
                            Id = id,
                        };

                        return 0;
                    }
                }
                return 1;
            }, 0);

            if (window != null)
            {
                window.Relocate(positon);
            }

        }

        /// <summary>
        /// Get position and size of window
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static Rectangle GetPlace(this WindowInformation window)
        {
            var placement = new WindowPlacement.WINDOWPLACEMENT();

            placement.Length = Marshal.SizeOf(typeof(WindowPlacement.WINDOWPLACEMENT));

            WindowPlacement.GetWindowPlacement(window.Handle, ref placement);

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
