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
        /// <param name="positon"></param>
        public static void Relocate(this WindowInformation window, Rectangle positon)
        {

            var placement = new WindowPlacement.WINDOWPLACEMENT();

            placement.Length = Marshal.SizeOf(typeof(WindowPlacement.WINDOWPLACEMENT));



            WindowPlacement.GetWindowPlacement(window.Handle, ref placement);

            int width = placement.NormalPosition.Right - placement.NormalPosition.Left;
            int height = placement.NormalPosition.Bottom - placement.NormalPosition.Top;

            placement.ShowCmd = WindowPlacement.ShowWindowCommands.Restore;
            placement.NormalPosition.Top = positon.Top;
            placement.NormalPosition.Left = positon.Left;

            if (positon.Top >= positon.Bottom || positon.Right <= positon.Left)
            {
                placement.NormalPosition.Bottom = placement.NormalPosition.Top + height;
                placement.NormalPosition.Right = placement.NormalPosition.Left + width;
            }
            else
            {
                placement.NormalPosition.Bottom = positon.Bottom;
                placement.NormalPosition.Right = positon.Right;
            }

            placement.Flags = 0;

            //最小化されていたら元に戻す
            placement.ShowCmd =
                (placement.ShowCmd == WindowPlacement.ShowWindowCommands.ShowMinimized)
                ? WindowPlacement.ShowWindowCommands.Normal
                : placement.ShowCmd;


            WindowPlacement.SetWindowPlacement(window.Handle, ref placement);

            //最前面に表示
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
            window.Relocate(new Rectangle(left, top, -1, -1));
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

            return new Rectangle(
                position.Left,
                position.Top,
                position.Right - position.Left,
                position.Bottom - position.Top);
        }
    }
}
