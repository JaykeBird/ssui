﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SolidShineUi
{
    /// <summary>
    /// A small collection of methods to interact with native Windows libraries, used to modify window behavior or get file icons.
    /// </summary>
    public static class NativeMethods
    {

        /// <summary>
        /// Displays the window of a specified handle.
        /// </summary>
        /// <param name="hWnd">The handle of the window to show.</param>
        /// <param name="nCmdShow">A command associated with this action.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Moves a specified window to the foreground.
        /// </summary>
        /// <param name="hWnd">The handle of the window.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// The standard style for a window.
        /// </summary>
        public const int GWL_STYLE = -16;

        /// <summary>
        /// The integer value that points to the maximize button in a window.
        /// </summary>
        public const int WS_MAXIMIZEBOX = 0x10000;

        /// <summary>
        /// The integer value that points to the minimize button in a window.
        /// </summary>
        public const int WS_MINIMIZEBOX = 0x20000;

        /// <summary>
        /// Get this window's long value, which can be used for further modification.
        /// </summary>
        /// <param name="hWnd">The handle of the window.</param>
        /// <param name="nIndex">The standard style of this window.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Set this window's long value, which is used to modify features and abilities of this window.
        /// </summary>
        /// <param name="hWnd">The handle of the window.</param>
        /// <param name="nIndex">The standard style of this window.</param>
        /// <param name="dwNewLong">The long to set for this window.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// Hide/disable the maximize button, so that the window cannot be maximized.
        /// </summary>
        /// <param name="w">The window to hide the button on.</param>
        public static void HideMaximizeButton(System.Windows.Window w)
        {
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(w).Handle;
            HideMaximizeButton(hwnd);
        }

        /// <summary>
        /// Hide/disable the maximize button, so that the window cannot be maximized.
        /// </summary>
        /// <param name="hwnd">The handle to the window.</param>
        public static int HideMaximizeButton(IntPtr hwnd)
        {
            int currentStyle = GetWindowLong(hwnd, GWL_STYLE);

            return SetWindowLong(hwnd, GWL_STYLE, (currentStyle & ~WS_MAXIMIZEBOX));
        }

        /// <summary>
        /// Hide/disable the minimize button, so that the window cannot be minimized (hidden in taskbar).
        /// </summary>
        /// <param name="w">The window to hide the button on.</param>
        public static void HideMinimizeButton(System.Windows.Window w)
        {
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(w).Handle;
            HideMinimizeButton(hwnd);
        }

        /// <summary>
        /// Hide/disable the minimize button, so that the window cannot be minimized (hidden in taskbar).
        /// </summary>
        /// <param name="hwnd">The handle to the window.</param>
        public static int HideMinimizeButton(IntPtr hwnd)
        {
            int currentStyle = GetWindowLong(hwnd, GWL_STYLE);

            return SetWindowLong(hwnd, GWL_STYLE, (currentStyle & ~WS_MINIMIZEBOX));
        }

        /// <summary>
        /// Hide/disable the minimize and maximize buttons, so that the window cannot be maximized or minimized.
        /// </summary>
        /// <param name="w">The window to hide the buttons on.</param>
        public static void HideMinimizeAndMaximizeButtons(System.Windows.Window w)
        {
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(w).Handle;
            HideMinimizeAndMaximizeButtons(hwnd);
        }

        /// <summary>
        /// Hide/disable the minimize and maximize buttons, so that the window cannot be maximized or minimized.
        /// </summary>
        /// <param name="hwnd">The handle to the window.</param>
        public static int HideMinimizeAndMaximizeButtons(IntPtr hwnd)
        {
            int currentStyle = GetWindowLong(hwnd, GWL_STYLE);

            return SetWindowLong(hwnd, GWL_STYLE, (currentStyle & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX));
        }

        #region Window extension methods

        /// <summary>
        /// Calls the native Windows method to disable the Maximize action (via the right-click menu and the like). For <see cref="FlatWindow"/>, you'll want to update the CaptionDisplayType to remove the physical button.
        /// </summary>
        public static void DisableMaximizeAction(this System.Windows.Window w)
        {
            HideMaximizeButton(w);
        }

        /// <summary>
        /// Calls the native Windows method to disable the Minimize action (via the right-click menu and the like). For <see cref="FlatWindow"/>, you'll want to update the CaptionDisplayType to remove the physical button.
        /// </summary>
        public static void DisableMinimizeAction(this System.Windows.Window w)
        {
            HideMinimizeButton(w);
        }

        /// <summary>
        /// Calls the native Windows method to disable the Minimize and Maximize actions (via the right-click menu and the like). For <see cref="FlatWindow"/>, you'll want to update the CaptionDisplayType to remove the physical buttons.
        /// </summary>
        public static void DisableMinimizeAndMaximizeActions(this System.Windows.Window w)
        {
            HideMinimizeAndMaximizeButtons(w);
        }

        #endregion

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr handle;
            public IntPtr index;
            public uint attr;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string display;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string type;
        };
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0; // Large icon
        public const uint SHGFI_SMALLICON = 0x1; // Small icon

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SHGetFileInfo(string path, uint fattrs, ref SHFILEINFO sfi, uint size, uint flags);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #region File Icons
        /// <summary>
        /// Get a handle for a small (16x16) icon associated with a file.
        /// </summary>
        /// <param name="path">The file to get the icon for.</param>
        /// <returns></returns>
        public static IntPtr GetSmallIconHandle(string path)
        {
            SHFILEINFO info = new SHFILEINFO();
            SHGetFileInfo(path, 0, ref info, (uint)Marshal.SizeOf(info), SHGFI_ICON | SHGFI_SMALLICON);
            return info.handle;
        }

        /// <summary>
        /// Get a handle for a large (32x32) icon associated with a file.
        /// </summary>
        /// <param name="path">The file to get the icon for.</param>
        /// <returns></returns>
        public static IntPtr GetLargeIconHandle(string path)
        {
            SHFILEINFO info = new SHFILEINFO();
            SHGetFileInfo(path, 0, ref info, (uint)Marshal.SizeOf(info), SHGFI_ICON | SHGFI_LARGEICON);
            return info.handle;
        }

        /// <summary>
        /// Get an 16x16 image that is associated with a file.
        /// </summary>
        /// <param name="path">The file to get the icon for.</param>
        /// <exception cref="ArgumentNullException">Thrown if the icon cannot be created (such as there being no existing file in the given path)</exception>
        public static BitmapSource GetSmallIcon(string path)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(GetSmallIconHandle(path), System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        /// <summary>
        /// Get an 32x32 image that is associated with a file.
        /// </summary>
        /// <param name="path">The file to get the icon for.</param>
        /// <exception cref="ArgumentNullException">Thrown if the icon cannot be created (such as there being no existing file in the given path)</exception>
        public static BitmapSource GetLargeIcon(string path)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(GetLargeIconHandle(path), System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
        #endregion

        /// <summary>
        /// Get a handle to the display context, either for a certain window or for the entire screen. This can be used to directly draw or get visual info for that particular context.
        /// </summary>
        /// <param name="hwnd">The pointer to the item you want the display context for. Use <c>IntPtr.Zero</c> for the entire screen, or a window handle for that window.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        /// <summary>
        /// Release the handle to the display context. Do this when you're done using the display context. Must be in the same thread that GetDC was called.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="hdc"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        /// <summary>
        /// Get color info for a particular pixel in a display context.
        /// </summary>
        /// <param name="hdc">The handle of the display context.</param>
        /// <param name="nXPos">The X position of the pixel.</param>
        /// <param name="nYPos">The Y position of the pixel.</param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        /// <summary>
        /// Get the color of a particular pixel on the screen.
        /// </summary>
        /// <param name="x">The X coordinate of the pixel.</param>
        /// <param name="y">The Y coordinate of the pixel.</param>
        /// <returns></returns>
        static public Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            int _ = ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromRgb((byte)(pixel & 0x000000FF),
                         (byte)((pixel & 0x0000FF00) >> 8),
                         (byte)((pixel & 0x00FF0000) >> 16));
            return color;
        }

        /// <summary>
        /// Create a rounded rectangular region.
        /// </summary>
        /// <param name="x1">The top-left corner's X coordinate.</param>
        /// <param name="y1">The top-left corner's Y coordinate.</param>
        /// <param name="x2">The bottom-right corner's X coordinate.</param>
        /// <param name="y2">The bottom-right corner's Y coordinate.</param>
        /// <param name="cx">The width of the ellipse used for rounded corners.</param>
        /// <param name="cy">The height of the ellipse used for rounded corners.</param>
        /// <returns>The handle of the created region.</returns>
        /// <remarks>The region should be deleted once you're done using it. However, if you're using this region in conjunction with <see cref="SetWindowRgn(IntPtr, IntPtr, bool)"/>,
        /// the region doesn't need to be deleted as Windows will delete it for you when needed.</remarks>
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int cx, int cy);

        /// <summary>
        /// Set the region (location, size) of a window, using the handle of a created region.
        /// </summary>
        /// <param name="hWnd">The handle of the window.</param>
        /// <param name="hRgn">The handle of the region.</param>
        /// <param name="redraw">Set if the window's redraw function should be called right after. If the window is visible, this typically should be "true".</param>
        /// <returns>Returns 0 if successful. If not successful, a different number is returned.</returns>
        [DllImport("user32.dll")]
        static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool redraw);

        /// <summary>
        /// Send a message to a window.
        /// </summary>
        /// <param name="hWnd">The handle of the window to send the message to.</param>
        /// <param name="msgType">The type of message to send. See https://learn.microsoft.com/en-us/windows/win32/winmsg/about-messages-and-message-queues for a list of types.</param>
        /// <param name="wParam">Data, metadata, or other info to send with the message.</param>
        /// <param name="lParam">Additional data, metadata, or other info to send with the message.</param>
        /// <returns>A pointer to a result, the contents of which will depend upon the message being sent.</returns>
        /// <remarks>If the message could not be sent, then GetLastError will return 5 (Access denied).
        /// Windows using SendMessage to communicate with each other should first use RegisterWindowMessage to get a unique message ID to use.
        /// </remarks>
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msgType, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Get the system menu for a particular window (this is the menu that contains the standard window controls, like Maximize, Move, or Close).
        /// </summary>
        /// <param name="hWnd">The handle of the window to get the menu for.</param>
        /// <param name="bRevert">Set whether a brand-new system menu should be created, replacing the one currently used by the window.</param>
        /// <returns>The handle to the system menu. If a brand-new system menu was created, the handle will be null, and a new menu can instead be created.</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        /// <summary>
        /// Display a menu on the screen, and return the index of the item that was selected from the menu.
        /// </summary>
        /// <param name="hMenu">The handle of the menu to display.</param>
        /// <param name="uFlags">Flags to determine how the menu should appear and behave. See https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-trackpopupmenu#parameters</param>
        /// <param name="x">The x (horizontal) coordinate of the pixel to display the menu at (relative to the screen).</param>
        /// <param name="y">The y (vertical) coordinate of the pixel to display the menu at (relative to the screen).</param>
        /// <param name="nReserved">Not currently used, please just enter 0.</param>
        /// <param name="hWnd">The handle of the window that owns the menu, and will receive updates from the menu.</param>
        /// <param name="prcRect">Not currently used, please just enter IntPtr.Zero.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y,
           int nReserved, IntPtr hWnd, IntPtr prcRect);


        //[DllImport("user32.dll")]
        //static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
        //struct RECT { public int left, top, right, bottom; }

    }
}
