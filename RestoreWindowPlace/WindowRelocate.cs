using RestoreWindowPlace.WindowsApi;
using System;
using System.Diagnostics;
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

        private static Size GetTaskbarOffset(ref Rect rect)
        {
            var taskbarInfo = new WindowPlacement.APPBARDATA();
            taskbarInfo.cbSize = Marshal.SizeOf(typeof(WindowPlacement.APPBARDATA));
            taskbarInfo.hWnd = IntPtr.Zero;

            WindowPlacement.SHAppBarMessage(
                WindowPlacement.AppBarMessage.ABM_GETTASKBARPOS,
                ref taskbarInfo);

            var taskbarEdge = taskbarInfo.uEdge;
            Debug.WriteLine($"taskbarEdge={taskbarEdge}");

            if (taskbarEdge == WindowPlacement.AppBarEdge.ABE_TOP
                || taskbarEdge == WindowPlacement.AppBarEdge.ABE_LEFT)
            {
                var taskbarWidth = taskbarInfo.rc.Width;
                var taskbarHeight = taskbarInfo.rc.Height;

                var monitorUnderWindow = WindowPlacement.MonitorFromRect(ref rect,
                    WindowPlacement.MonitorFromRectFlags.MONITOR_DEFAULTTONEAREST);

                //var mInfo = new WindowPlacement.MONITORINFO();
                //mInfo.cbSize = Marshal.SizeOf(typeof(WindowPlacement.MONITORINFO));
                //WindowPlacement.GetMonitorInfo(hMonitor1, ref mInfo);

                var monitorUnderTaskbar = WindowPlacement.MonitorFromRect(ref taskbarInfo.rc,
                    WindowPlacement.MonitorFromRectFlags.MONITOR_DEFAULTTONEAREST);

                if (monitorUnderWindow == monitorUnderTaskbar)
                {
                    Debug.WriteLine("monitor has taskbar!!!");

                    if (WindowPlacement.SHAppBarMessage(
                        WindowPlacement.AppBarMessage.ABM_GETAUTOHIDEBAR,
                        ref taskbarInfo) == IntPtr.Zero)
                    {
                        Debug.WriteLine("taskbar is static!!!");

                        switch (taskbarEdge)
                        {
                            case WindowPlacement.AppBarEdge.ABE_TOP:
                                return new Size(0, taskbarHeight);
                            case WindowPlacement.AppBarEdge.ABE_LEFT:
                                return new Size(taskbarWidth, 0);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("taskbar is hidden...");
                    }
                }
                else
                {
                    Debug.WriteLine("monitor does not have taskbar...");
                }
            }
            return Size.Empty;
        }

        /// <summary>
        /// Get position and size of window
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <param name="byRect"></param>
        /// <returns></returns>
        public static Rectangle GetPlace(IntPtr windowHandle, bool byRect)
        {
            var placement = new WindowPlacement.WINDOWPLACEMENT();
            placement.Length = Marshal.SizeOf(typeof(WindowPlacement.WINDOWPLACEMENT));

            WindowPlacement.GetWindowPlacement(windowHandle, ref placement);

            var minimized = placement.ShowCmd == WindowPlacement.ShowWindowCommands.ShowMinimized;
            var maximized = placement.ShowCmd == WindowPlacement.ShowWindowCommands.Maximize;

            if (byRect && !maximized && !minimized)
            {
                var rect = new Rect();
                WindowPlacement.GetWindowRect(windowHandle, ref rect);

                var taskbarOffset = GetTaskbarOffset(ref rect);

                return new Rectangle(
                    rect.Left - taskbarOffset.Width,
                    rect.Top - taskbarOffset.Height,
                    rect.Width,
                    rect.Height);
            }
            else
            {
                return new Rectangle(
                    placement.NormalPosition.Left,
                    placement.NormalPosition.Top,
                    (maximized ? -1 : 1) * placement.NormalPosition.Width,
                    placement.NormalPosition.Height);
            }
        }
    }
}
