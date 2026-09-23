using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.UI.Windowing;
// using Windows.UI.WindowManagement;
// using Microsoft.UI.Xaml;
//using Windows.UI.WindowManagement;

namespace SolidShineUi;

public static class FlatWindowInterop
{
    // if you have an unpackaged WPF app that references the Windows App SDK, make sure to disable MRT Core
    // https://github.com/microsoft/WindowsAppSDK/issues/3410

    // TODO: add option to FlatWindow to hide maximized window buffer when this is set up

    /// <summary>
    /// Use Windows 11's customizable title bar functionality to use Windows 11's
    /// own caption buttons for the corner of the window, rather than Solid Shine UI's caption buttons.
    /// </summary>
    /// <param name="w">The window to update.</param>
    /// <remarks>
    /// This tells Windows to render the Windows 11 caption buttons in the corner (the minimize/maximize/close buttons),
    /// which can then benefit from built-in Windows 11 features like the Snap regions under the Maximize button, as well
    /// as just fitting in better with how a standard window looks in Windows 11.
    /// This then tells the FlatWindow itself to not render its own caption buttons, since we're using the Windows 11 ones.
    /// <para/>
    /// This requires getting the window's handle, so it's best to call this in the window's SourceInitialized event handler.
    /// <para/>
    /// This will also set Windows 11's caption buttons to be the same color as the FlatWindow's
    /// <c>TitleBarBackground</c> and various caption button brushes, if they are solid color brushes; otherwise, they will
    /// follow the Windows default color for your system theme.
    /// If you have a UI element in <see cref="FlatWindow.TopRightElement"/>, this will automatically move the UI element
    /// to not be underneath the caption buttons.
    /// <para/>
    /// In its current state, this may have some visual bugs in RTL languages/systems.
    /// </remarks>
    public static void UseWindows11CaptionButtons(this FlatWindow w)
    {
        if (!AppWindowTitleBar.IsCustomizationSupported()) return;

        WindowInteropHelper wih = new WindowInteropHelper(w);
        IntPtr hwnd = wih.EnsureHandle();

        //HwndSource hwndSource = (HwndSource)PresentationSource.FromVisual(w);
        //hwndSource.AddHook((IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) =>
        //        FixResizeHitZones(w, 6, msg, wParam, lParam, ref handled));

        // Retrieve the WindowId that corresponds to hWnd.
        Microsoft.UI.WindowId wid = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
        AppWindow appWindow = AppWindow.GetFromWindowId(wid);

        appWindow.TitleBar.ExtendsContentIntoTitleBar = true;

        // try to get the caption buttons to be the same color as the rest of the window
        if (w.TitleBarBackground is SolidColorBrush scb)
        {
            appWindow.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(scb.Color.A, scb.Color.R, scb.Color.G, scb.Color.B);
        }

        if (w.CaptionButtonsHighlightBrush is SolidColorBrush scb2)
        {
            appWindow.TitleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(scb2.Color.A, scb2.Color.R, scb2.Color.G, scb2.Color.B);
        }

        if (w.CaptionButtonsClickBrush is SolidColorBrush scb3)
        {
            appWindow.TitleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(scb3.Color.A, scb3.Color.R, scb3.Color.G, scb3.Color.B);
        }

        if (w.CaptionButtonsForeground is SolidColorBrush scb4)
        {
            appWindow.TitleBar.ButtonForegroundColor = Windows.UI.Color.FromArgb(scb4.Color.A, scb4.Color.R, scb4.Color.G, scb4.Color.B);
            appWindow.TitleBar.ButtonHoverForegroundColor = Windows.UI.Color.FromArgb(scb4.Color.A, scb4.Color.R, scb4.Color.G, scb4.Color.B);
            appWindow.TitleBar.ButtonPressedForegroundColor = Windows.UI.Color.FromArgb(scb4.Color.A, scb4.Color.R, scb4.Color.G, scb4.Color.B);
            appWindow.TitleBar.ButtonInactiveForegroundColor = Windows.UI.Color.FromArgb(scb4.Color.A, scb4.Color.R, scb4.Color.G, scb4.Color.B);
        }

        if (w.InactiveBackground is SolidColorBrush scb5)
        {
            appWindow.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(scb5.Color.A, scb5.Color.R, scb5.Color.G, scb5.Color.B);
        }

        // get DPI handling
        DpiScale dpi = VisualTreeHelper.GetDpi(w);
        w.DpiChanged += (s, e) => { UpdateTitleBarElements(e.NewDpi, w); };
        w.SizeChanged += (s, e) => { UpdateTitleBarElements(VisualTreeHelper.GetDpi(w), w); };

        UpdateTitleBarElements(dpi, w, hwnd);

        // and now let's hide SSUI's own caption buttons
        w.CaptionDisplayType = CaptionType.None;
    }

    static void UpdateTitleBarElements(DpiScale dpi, FlatWindow w, IntPtr? windowHandle = null)
    {
        IntPtr hwnd;
        if (windowHandle.HasValue)
        {
            hwnd = windowHandle.Value;
        }
        else
        {
            WindowInteropHelper wih = new WindowInteropHelper(w);
            hwnd = wih.EnsureHandle();
        }

        // Retrieve the WindowId that corresponds to hWnd.
        Microsoft.UI.WindowId wid = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
        AppWindow appWindow = AppWindow.GetFromWindowId(wid);

        //var nonClientInputSrc = InputNonClientPointerSource.GetForWindowId(wid); // this doesn't seem to work

        Rect titleBarRect = new Rect(appWindow.TitleBar.LeftInset, 0, 
            w.Width - appWindow.TitleBar.LeftInset - appWindow.TitleBar.RightInset, // subtract the left and right inset from the total
            w.CaptionHeight);
        List<Rect> nonClientRects = new List<Rect>();

        // handle TopLeftElement and TopRightElement            
        // get the top-left element's bounds
        if (w.TopLeftElement is FrameworkElement fel && w.ExcludeTopLeftElementFromChrome)
        {
            // if it's an RTL language, the caption buttons may be on the left, and we may need to look at LeftInset instead
            // I'll have to test and see, this may not work quite as right
            Point topLeft = fel.TranslatePoint(new Point(0, 0), w);

            var transparentRect = new Rect(topLeft, new Size(fel.Width, fel.Height));
            nonClientRects.Add(transparentRect);
        }

        // handling to make sure the top-right element isn't hidden behind the caption buttons
        if (w.TopRightElement is FrameworkElement fer)
        {
            fer.Margin = new Thickness(fer.Margin.Left, fer.Margin.Top, appWindow.TitleBar.RightInset / dpi.DpiScaleX, fer.Margin.Bottom);

            // and also get the bounds
            if (w.ExcludeTopRightElementFromChrome)
            {
                Point topLeft = fer.TranslatePoint(new Point(0, 0), w);

                var transparentRect = new Rect(topLeft, new Size(fer.Width, fer.Height));
                nonClientRects.Add(transparentRect);
            }
        }

        if (w.ShowIcon)
        {
            int iconSize = 16;
            int titleBarHeight = w.CaptionHeight;
            double iconTop = (titleBarHeight / 2d) - (iconSize / 2d);
            Thickness border = w.BorderThickness;

            // this doesn't seem to get the area quite perfect, but it's close enough
            nonClientRects.Add(new Rect(6 + border.Left, iconTop + border.Top, iconSize / dpi.DpiScaleX, iconSize / dpi.DpiScaleX));
        }

        if (nonClientRects.Count > 0)
        {
            appWindow.TitleBar.SetDragRectangles(titleBarRect.SubtractCutouts(nonClientRects).Select(r => r.ToRectInt32()).ToArray());
        }
    }

    // I'm currently dealing with an issue where when this is turned on, the resize hit zones end up being outside the window itself
    // rather than lining up closer to the window's borders
    // the solution below seems to be using the Windows message pump to listen to the hit test message and tell Windows explicitly
    // what the different hit zones are... but this also ends up requiring me to also define where other aspects of the window
    // are too, like the title bar and other stuff. this feels a bit of a lot to try implementing right now, even if the existing
    // issue is very annoying. I'll have to revisit this later on, when I have more time to investigate this

    //private static IntPtr FixResizeHitZones(Window window, int resizeBorder, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    //{
    //    const int WM_NCHITTEST = 0x0084;

    //    if (msg == WM_NCHITTEST)
    //    {
    //        int x = (short)(lParam.ToInt32() & 0xFFFF);
    //        int y = (short)(lParam.ToInt32() >> 16);

    //        Point p = window.PointFromScreen(new Point(x, y));

    //        bool left = p.X <= resizeBorder;
    //        bool right = p.X >= window.ActualWidth - resizeBorder;
    //        bool top = p.Y <= resizeBorder;
    //        bool bottom = p.Y >= window.ActualHeight - resizeBorder;

    //        handled = true;

    //        if (left && top) return (IntPtr)13;        // HTTOPLEFT
    //        if (right && top) return (IntPtr)14;       // HTTOPRIGHT
    //        if (left && bottom) return (IntPtr)16;     // HTBOTTOMLEFT
    //        if (right && bottom) return (IntPtr)17;    // HTBOTTOMRIGHT
    //        if (left) return (IntPtr)10;               // HTLEFT
    //        if (right) return (IntPtr)11;              // HTRIGHT
    //        if (top) return (IntPtr)12;                // HTTOP
    //        if (bottom) return (IntPtr)15;             // HTBOTTOM

    //        // need to implement other items, like HTCAPTION
    //        // I should see what WPF's WindowChrome does; it might already do something with this

    //        return IntPtr.Zero;
    //    }

    //    return IntPtr.Zero;
    //}
}