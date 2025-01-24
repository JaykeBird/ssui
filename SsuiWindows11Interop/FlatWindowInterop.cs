using Microsoft.UI.Windowing;
using System.Windows.Interop;
//using Windows.UI.WindowManagement;

namespace SsuiWindows11Interop
{
    public static class FlatWindowInterop
    {
        // while in theory this code should work, my issue is that I'm not able to properly test it
        // as I encounter a bug when trying to run a WPF app that references the Windows App SDK:
        // https://github.com/microsoft/WindowsAppSDK/issues/3410

        /// <summary>
        /// Use Windows 11's own caption buttons for the corner of the window, rather than Solid Shine UI's caption buttons.
        /// </summary>
        /// <param name="w">The window to update.</param>
        /// <remarks>
        /// This tells Windows to render the Windows 11 caption buttons in the corner (the minimize/maximize/close buttons),
        /// which can then benefit from built-in Windows 11 features like the Snap regions under the Maximize button, as well
        /// as just fitting in better with how a standard window looks in Windows 11.
        /// This then tells the FlatWindow itself to not render its own caption buttons, since we're using the Windows 11 ones.
        /// <para/>
        /// This requires getting the window's handle, so it's best to use this in the window's SourceInitialized event handler.
        /// </remarks>
        public static void RenderWindowsCaptionButtons(this FlatWindow w)
        {
            if (!AppWindowTitleBar.IsCustomizationSupported()) return;

            WindowInteropHelper wih = new WindowInteropHelper(w);
            IntPtr hwnd = wih.EnsureHandle();

            // Retrieve the WindowId that corresponds to hWnd.
            Microsoft.UI.WindowId wid = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(wid);

            appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            w.CaptionDisplayType = CaptionType.None;
        }

    }
}