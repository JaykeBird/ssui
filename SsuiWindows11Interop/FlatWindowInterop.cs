using Microsoft.UI.Windowing;
using System.Windows.Interop;

namespace SsuiWindows11Interop
{
    public static class FlatWindowInterop
    {

        public static void RenderWindowsCaptionButtons(ref FlatWindow w)
        {
            if (!AppWindowTitleBar.IsCustomizationSupported()) return;

            WindowInteropHelper wih = new WindowInteropHelper(w);
            IntPtr hwnd = wih.EnsureHandle();

            // Retrieve the WindowId that corresponds to hWnd.
            Microsoft.UI.WindowId wid = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(wid);

            appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            w.CaptionDisplayType = ChromeButtons.CaptionType.None;
        }

    }
}