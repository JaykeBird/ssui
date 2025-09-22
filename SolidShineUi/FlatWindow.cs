using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;
//using Microsoft.Windows.Shell;
using System.ComponentModel;
using System;
using System.Windows.Input;
using System.Windows.Interop;
using SolidShineUi.Utils;

namespace SolidShineUi
{
    /// <summary>
    /// A WPF window that has a custom, flat style and additional functionality.
    /// </summary>
    [Localizability(LocalizationCategory.None)]
    public class FlatWindow : ThemedWindow
    {
        static FlatWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlatWindow), new FrameworkPropertyMetadata(typeof(FlatWindow)));
        }

        /// <summary>
        /// Create a new FlatWindow.
        /// </summary>
        public FlatWindow()
        {
            //InternalCornerRadiusChanged += flatWindow_InternalCornerRadiusChanged;
            CommandBindings.Add(new CommandBinding(FlatWindowCommands.CloseWindow, OnCloseWindow, (_, e) => e.CanExecute = true));
            CommandBindings.Add(new CommandBinding(FlatWindowCommands.Minimize, OnMinimizeWindow, (_, e) => e.CanExecute = WindowState != WindowState.Minimized));
            CommandBindings.Add(new CommandBinding(FlatWindowCommands.Maximize, OnMaximizeWindow, (_, e) => e.CanExecute = WindowState != WindowState.Maximized));
            CommandBindings.Add(new CommandBinding(FlatWindowCommands.Restore, OnRestoreWindow, (_, e) => e.CanExecute = WindowState != WindowState.Normal));
            CommandBindings.Add(new CommandBinding(FlatWindowCommands.DisplaySystemMenu, OnShowSystemMenu, (_, e) => e.CanExecute = WindowState != WindowState.Minimized));

            DependencyPropertyDescriptor.FromProperty(ResizeModeProperty, typeof(FlatWindow)).AddValueChanged(this, ResizeModeChanged);
        }

        #region Native Interop
        /// <summary>
        /// Raises the Window.SourceInitialized event.
        /// </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            //HwndSource hwnd = (HwndSource)PresentationSource.FromVisual(this);
            //hwnd.AddHook(WndProc);
            base.OnSourceInitialized(e);
        }

        //private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        //{
        //    Window window = (Window)HwndSource.FromHwnd(hwnd).RootVisual;

        //    // https://learn.microsoft.com/en-us/windows/apps/desktop/modernize/apply-snap-layout-menu#how-do-i-fix-it
        //    if (msg == 0x0084) // NCHITTEST (asking what this point is in relation to the window)
        //    {

        //        Debug.Print("ASKING NCHITTEST");
        //        // lParam stores the coordinates the message is asking about, so let's get the coordinates out
        //        // https://learn.microsoft.com/en-us/windows/win32/inputdev/wm-nchittest
        //        // https://stackoverflow.com/questions/49288552/how-do-i-read-the-win32-wm-move-lparam-x-y-coordinates-in-c
        //        uint lparam32 = (uint)lParam.ToInt64(); // We want the bottom unsigned 32-bits
        //        Point p = window.PointFromScreen(new Point(lparam32 & 0xffff, (lparam32 >> 16) & 0xffff));

        //        if (window is FlatWindow fw)
        //        {
        //            if (fw.TestIfPointIsMaximizeButton(p))
        //            {
        //                handled = true;
        //                return new IntPtr(9);
        //            }
        //        }
        //    }

        //    return IntPtr.Zero;
        //}

        //private bool TestIfPointIsMaximizeButton(Point p)
        //{
        //    if (p.Y > CaptionHeight + 3) return false;

        //    double borderTop = BorderThickness.Top;
        //    double borderRight = BorderThickness.Right;
        //    // next, let's get the width of a button
        //    int captionFontSize = 16; // the caption font size is hardset in the theme, in the future maybe I'll open it to be changed
        //    double btnWidth = captionFontSize + CaptionButtonPadding.Left + CaptionButtonPadding.Right;
        //    double btnHeight = captionFontSize + CaptionButtonPadding.Top + CaptionButtonPadding.Bottom;

        //    double maxButtonRightBound = ActualWidth - borderRight - btnWidth; // this is the right edge of the maximize/restore button
        //    double maxButtonLeftBound = maxButtonRightBound - btnWidth; // this is the left edge of the maximize/restore button
        //    double maxButtonTopBound = borderTop;
        //    double maxButtonBottomBound = borderTop + btnHeight;

        //    if (p.X > maxButtonLeftBound && p.X < maxButtonRightBound && p.Y > maxButtonTopBound && p.Y < maxButtonBottomBound)
        //    {
        //        Debug.Print("MAX BUTTON");
        //        return true; // MAXIMIZE BUTTON
        //    }
        //    else
        //    {
        //        return false; // let the main function handle this
        //    }
        //}
        #endregion

        #region Color Scheme
        ///// <summary>
        ///// Get if the window's color scheme is a high contrast theme (and thus, the window is displaying high-contrast colors).
        ///// </summary>
        //public bool HighContrastMode { get; private set; } = false;

        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

        /// <summary>
        /// The backing dependency property object for <see cref="ColorScheme"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(FlatWindow),
            new FrameworkPropertyMetadata(new ColorScheme(), OnColorSchemeChanged));

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ColorScheme cs)
            {
                if (d is FlatWindow w)
                {
                    w.ApplyColorScheme(cs);
                    w.ColorSchemeChanged?.Invoke(d, e);
                }
            }
        }

        /// <summary>
        /// Get or set the color scheme to apply to the window. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        /// <summary>
        /// Apply a color scheme to this control. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply.</param>
        public void ApplyColorScheme(ColorScheme cs)
        {
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }

            // instead, let's move everything on to the SsuiTheme
            SsuiTheme = new SsuiAppTheme(cs);

            //if (cs.IsHighContrast)
            //{
            //    HighContrastMode = true;
            //    InactiveTextBrush = cs.WindowTitleBarTextColor.ToBrush();
            //}
            //else
            //{
            //    InactiveTextBrush = ColorsHelper.CreateFromHex("#505050").ToBrush();
            //}

            //Background = cs.WindowTitleBarColor.ToBrush();
            //InactiveBackground = cs.WindowInactiveColor.ToBrush();
            //CaptionButtonsBrush = cs.WindowTitleBarTextColor.ToBrush();
            //CaptionTextBrush = cs.WindowTitleBarTextColor.ToBrush();
            //BorderBrush = cs.BorderColor.ToBrush();
            //HighlightBrush = cs.HighlightColor.ToBrush();
            //SelectionBrush = cs.SelectionColor.ToBrush();
            //Foreground = cs.ForegroundColor.ToBrush();
            //ContentBackground = cs.BackgroundColor.ToBrush();
        }

        /// <inheritdoc/>
        protected override void OnApplySsuiTheme(SsuiAppTheme ssuiTheme)
        {
            base.OnApplySsuiTheme(ssuiTheme);

            //if (ssuiTheme.IsHighContrast)
            //{
            //    HighContrastMode = true;
            //    InactiveTextBrush = cs.WindowTitleBarTextColor.ToBrush();
            //}
            //else
            //{
            //    InactiveTextBrush = ColorsHelper.CreateFromHex("#505050").ToBrush();
            //}

            CaptionBarBackground = ssuiTheme.WindowTitleBackground;
            CaptionBarForeground = ssuiTheme.WindowTitleForeground;
            InactiveBackground = ssuiTheme.WindowInactiveBackground;
            InactiveForeground = ssuiTheme.WindowInactiveForeground;
            BorderBrush = ssuiTheme.BorderBrush;
            CaptionButtonsForeground = ssuiTheme.WindowCaptionsForeground;
            CaptionButtonsHighlightBrush = ssuiTheme.WindowCaptionsHighlight;
            CaptionButtonsClickBrush = ssuiTheme.WindowCaptionsClickBrush;
            Foreground = ssuiTheme.Foreground;
            Background = ssuiTheme.WindowBackground;
            ContentBackground = ssuiTheme.WindowBackground;
        }

        #endregion

        #region Caption Bar Appearance

        #region Caption Buttons

        /// <summary>
        /// Gets or sets the visibility of the caption buttons (close, maximize, minimize). 
        /// The actions may still be available via other methods even if the buttons are hidden.
        /// </summary>
        /// <remarks>
        /// Use the <c>DisableMinimizeAction</c> and <c>DisableMaximizeAction</c> methods to further prevent the user from being 
        /// able to minimize or maximize the window. The <c>ResizeMode</c> property can also be used to prevent certain actions.
        /// </remarks>
        [Category("Appearance")]
        public CaptionType CaptionDisplayType
        {
            get => (CaptionType)GetValue(CaptionDisplayTypeProperty);
            set => SetValue(CaptionDisplayTypeProperty, value);
        }

        /// <summary>
        /// The backing dependency property object for <see cref="CaptionDisplayType"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty CaptionDisplayTypeProperty = DependencyProperty.Register(
            "CaptionDisplayType", typeof(CaptionType), typeof(FlatWindow),
            new PropertyMetadata(CaptionType.Full));

        /// <summary>
        /// Gets or sets the amount of padding to use with each of the caption buttons. A higher padding will make the buttons larger.
        /// </summary>
        /// <remarks>The default value is (12,7,12,7).</remarks>
        [Category("Appearance")]
        public Thickness CaptionButtonPadding
        {
            get => (Thickness)GetValue(CaptionButtonPaddingProperty);
            set => SetValue(CaptionButtonPaddingProperty, value);
        }

        /// <summary>
        /// The backing dependency property object for <see cref="CaptionButtonPadding"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty CaptionButtonPaddingProperty = DependencyProperty.Register(
            "CaptionButtonPadding", typeof(Thickness), typeof(FlatWindow),
            new PropertyMetadata(new Thickness(12, 7, 12, 7)));

#if NETCOREAPP
        private void ResizeModeChanged(object? sender, EventArgs e)
#else
        private void ResizeModeChanged(object sender, EventArgs e)
#endif
        {
            switch (ResizeMode)
            {
                case ResizeMode.NoResize:
                    CaptionDisplayType = CaptionType.Close;
                    break;
                case ResizeMode.CanMinimize:
                    CaptionDisplayType = CaptionType.MinimizeClose;
                    break;
                default:
                    CaptionDisplayType = CaptionType.Full;
                    break;
            }
        }

        #endregion

        #region TopRight/TopLeft elements

        /// <summary>
        /// The backing dependency property object for <see cref="TopRightElement"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty TopRightElementProperty = DependencyProperty.Register(
            nameof(TopRightElement), typeof(UIElement), typeof(FlatWindow),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the UI element to place in the top-right of the window, to the left of the caption buttons.
        /// </summary>
        [Category("Appearance")]
        public UIElement TopRightElement
        {
            get => (UIElement)GetValue(TopRightElementProperty);
            set => SetValue(TopRightElementProperty, value);
        }

        /// <summary>
        /// The backing dependency property object for <see cref="TopLeftElement"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty TopLeftElementProperty = DependencyProperty.Register(
            nameof(TopLeftElement), typeof(UIElement), typeof(FlatWindow),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the UI element to place in the top-left corner of the window.
        /// </summary>
        [Category("Appearance")]
        public UIElement TopLeftElement
        {
            get => (UIElement)GetValue(TopLeftElementProperty);
            set => SetValue(TopLeftElementProperty, value);
        }

        /// <summary>
        /// The backing dependency property object for <see cref="ExcludeTopLeftElementFromChrome"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ExcludeTopLeftElementFromChromeProperty = DependencyProperty.Register(
            nameof(ExcludeTopLeftElementFromChrome), typeof(bool), typeof(FlatWindow),
            new PropertyMetadata(true));

        /// <summary>
        /// Get or set if the top-left element should be considered part of the window chrome or not. Set to <c>true</c> to exclude it from the window chrome.
        /// </summary>
        /// <remarks>
        /// This changes the IsHitTestVisibleInChrome property for the top-left element. If set to true, the element can be interacted with.
        /// If set to false, the element cannot be interacted with, and clicking, dragging, etc. acts as if you're clicking on the window's title bar.
        /// </remarks>
        public bool ExcludeTopLeftElementFromChrome
        {
            get => (bool)GetValue(ExcludeTopLeftElementFromChromeProperty);
            set => SetValue(ExcludeTopLeftElementFromChromeProperty, value);
        }

        /// <summary>
        /// The backing dependency property object for <see cref="ExcludeTopRightElementFromChrome"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ExcludeTopRightElementFromChromeProperty = DependencyProperty.Register(
            nameof(ExcludeTopRightElementFromChrome), typeof(bool), typeof(FlatWindow),
            new PropertyMetadata(true));

        /// <summary>
        /// Get or set if the top-right element should be considered part of the window chrome or not. Set to <c>true</c> to exclude it from the window chrome.
        /// </summary>
        /// <remarks>
        /// This changes the IsHitTestVisibleInChrome property for the top-right element. If set to true, the element can be interacted with.
        /// If set to false, the element cannot be interacted with, and clicking, dragging, etc. acts as if you're clicking on the window's title bar.
        /// </remarks>
        public bool ExcludeTopRightElementFromChrome
        {
            get => (bool)GetValue(ExcludeTopRightElementFromChromeProperty);
            set => SetValue(ExcludeTopRightElementFromChromeProperty, value);
        }

        #endregion

        #region Other Caption Options

        /// <summary>
        /// The backing dependency property object for <see cref="ShowTitle"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register(
            "ShowTitle", typeof(bool), typeof(FlatWindow),
            new PropertyMetadata(true));

        /// <summary>
        /// Get or set if the <see cref="Window.Title"/> should be displayed at the top of the window.
        /// </summary>
        [Category("Appearance")]
        public bool ShowTitle
        {
            get => (bool)GetValue(ShowTitleProperty);
            set => SetValue(ShowTitleProperty, value);
        }

        /// <summary>
        /// The backing dependency property object for <see cref="ShowIcon"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register(
            "ShowIcon", typeof(bool), typeof(FlatWindow),
            new PropertyMetadata(true));

        /// <summary>
        /// Get or set if the <see cref="Window.Icon"/> should be displayed at the top of the window.
        /// </summary>
        [Category("Appearance")]
        public bool ShowIcon
        {
            get => (bool)GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }

        /// <summary>
        /// Get or set the height of the caption (title bar) area of the window. Default is 29.
        /// </summary>
        public int CaptionHeight { get => (int)GetValue(CaptionHeightProperty); set => SetValue(CaptionHeightProperty, value); }

        /// <summary>The backing dependency property for <see cref="CaptionHeight"/>. See the related property for details.</summary>
        public static DependencyProperty CaptionHeightProperty
            = DependencyProperty.Register(nameof(CaptionHeight), typeof(int), typeof(FlatWindow),
            new FrameworkPropertyMetadata(29, (d, e) => d.PerformAs<FlatWindow, int>(e.NewValue, (o, n) => o.ResizeCaptionHeight(n))));

        void ResizeCaptionHeight(int height)
        {
            // note to self: if updating this method, make sure this matches the WindowChrome settings in the Generic.xaml file.
            WindowChrome wc = new WindowChrome
            {
                CaptionHeight = height,
                CornerRadius = new CornerRadius(0),
                ResizeBorderThickness = new Thickness(4),
                GlassFrameThickness = new Thickness(0, 0, 0, 1)
            };

            WindowChrome.SetWindowChrome(this, wc);
        }

        #endregion

        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the brush used when one of the caption buttons (minimize, restore, close) is being clicked.
        /// </summary>
        [Category("Brushes")]
        public Brush CaptionButtonsClickBrush
        {
            get => (Brush)GetValue(CaptionButtonsClickBrushProperty);
            set => SetValue(CaptionButtonsClickBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used when one of the caption buttons (minimize, restore, close) has focus or has the mouse over it.
        /// </summary>
        [Category("Brushes")]
        public Brush CaptionButtonsHighlightBrush
        {
            get => (Brush)GetValue(CaptionButtonsHighlightBrushProperty);
            set => SetValue(CaptionButtonsHighlightBrushProperty, value);
        }

        /// <summary>
        /// Get or set the background brush to use for the caption area (title bar).
        /// </summary>
        /// <remarks>
        /// Use <c>ContentBackground</c> for the content area of the window, or set this to <c>null</c> and use <c>Background</c> to
        /// set the background for the entire window.
        /// </remarks>
        [Category("Brushes")]
        public Brush CaptionBarBackground
        {
            get => (Brush)GetValue(CaptionBarBackgroundProperty);
            set => SetValue(CaptionBarBackgroundProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the icons of the caption buttons (minimize, restore, close).
        /// </summary>
        [Category("Brushes")]
        public Brush CaptionButtonsForeground
        {
            get => (Brush)GetValue(CaptionButtonsForegroundProperty);
            set => SetValue(CaptionButtonsForegroundProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the text in the caption area (title bar). This has no effect if <c>ShowTitle</c> is false.
        /// </summary>
        [Category("Brushes")]
        public Brush CaptionBarForeground
        {
            get => (Brush)GetValue(CaptionBarForegroundProperty);
            set => SetValue(CaptionBarForegroundProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the text in the caption area (title bar) and caption buttons when the window isn't focused/active.
        /// </summary>
        [Category("Brushes")]
        public Brush InactiveForeground
        {
            get => (Brush)GetValue(InactiveForegroundProperty);
            set => SetValue(InactiveForegroundProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the caption area (title bar) when the window isn't focused/active.
        /// </summary>
        [Category("Brushes")]
        public Brush InactiveBackground
        {
            get => (Brush)GetValue(InactiveBackgroundProperty);
            set => SetValue(InactiveBackgroundProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of the content area of the window (where most content and controls are displayed).
        /// </summary>
        /// <remarks>
        /// Use <c>CaptionBarBackground</c> for the title bar of the window, or set this to <c>null</c> and use <c>Background</c> to
        /// set the background for the entire window.
        /// </remarks>
        [Category("Brushes")]
        public Brush ContentBackground
        {
            get => (Brush)GetValue(ContentBackgroundProperty);
            set => SetValue(ContentBackgroundProperty, value);
        }

        /// <summary>
        /// The backing dependency property object for <see cref="CaptionButtonsClickBrush"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty CaptionButtonsClickBrushProperty = DependencyProperty.Register(
            nameof(CaptionButtonsClickBrush), typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// The backing dependency property object for <see cref="CaptionButtonsHighlightBrush"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty CaptionButtonsHighlightBrushProperty = DependencyProperty.Register(
            nameof(CaptionButtonsHighlightBrush), typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// The backing dependency property object for <see cref="CaptionBarBackground"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty CaptionBarBackgroundProperty = DependencyProperty.Register(
            nameof(CaptionBarBackground), typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.White)));

        /// <summary>
        /// The backing dependency property object for <see cref="CaptionButtonsForeground"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty CaptionButtonsForegroundProperty = DependencyProperty.Register(
            nameof(CaptionButtonsForeground), typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.Black)));

        /// <summary>
        /// The backing dependency property object for <see cref="CaptionBarForeground"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty CaptionBarForegroundProperty = DependencyProperty.Register(
            nameof(CaptionBarForeground), typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.Black)));

        /// <summary>
        /// The backing dependency property object for <see cref="InactiveForeground"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty InactiveForegroundProperty = DependencyProperty.Register(
            nameof(InactiveForeground), typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));

        /// <summary>
        /// The backing dependency property object for <see cref="InactiveBackground"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty InactiveBackgroundProperty = DependencyProperty.Register(
            nameof(InactiveBackground), typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));

        /// <summary>
        /// The backing dependency property object for <see cref="ContentBackground"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ContentBackgroundProperty = DependencyProperty.Register(
            nameof(ContentBackground), typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));

        #endregion

        #region Command Handling

        private void OnCloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void OnMinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OnMaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        private void OnRestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void OnShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
        {
            Point origPoint;
            if (ShowIcon)
            {
                if (FlowDirection == FlowDirection.RightToLeft)
                {
                    origPoint = new Point(ActualWidth - 6, 26);
                }
                else
                {
                    origPoint = new Point(6, 26);
                }
            }
            else
            {
                if (FlowDirection == FlowDirection.RightToLeft)
                {
                    origPoint = new Point(ActualWidth - 8, 26);
                }
                else
                {
                    origPoint = new Point(8, 26);
                }
            }

            Point p = PointToScreen(origPoint);
            DisplaySystemMenu(p);
        }

        #endregion

        #region Native Methods interop
        /// <summary>
        /// Calls the native Windows method to disable the Maximize action (via the right-click menu and the like). For <see cref="FlatWindow"/>, 
        /// you'll want to update the CaptionDisplayType to remove the physical button.
        /// </summary>
        public void DisableMaximizeAction()
        {
            NativeMethods.DisableMaximizeAction(this);
            CaptionDisplayType = CaptionDisplayType.RemoveMaximizeButton();
        }

        /// <summary>
        /// Calls the native Windows method to disable the Minimize action (via the right-click menu and the like). For <see cref="FlatWindow"/>, 
        /// you'll want to update the CaptionDisplayType to remove the physical button.
        /// </summary>
        public void DisableMinimizeAction()
        {
            NativeMethods.DisableMinimizeAction(this);
            CaptionDisplayType = CaptionDisplayType.RemoveMinimizeButton();
        }

        /// <summary>
        /// Calls the native Windows method to disable the Minimize and Maximize actions (via the right-click menu and the like). For <see cref="FlatWindow"/>, 
        /// you'll want to update the CaptionDisplayType to remove the physical buttons.
        /// </summary>
        public void DisableMaximizeAndMinimizeActions()
        {
            NativeMethods.DisableMinimizeAndMaximizeActions(this);
            CaptionDisplayType = CaptionDisplayType.RemoveMaximizeButton();
            CaptionDisplayType = CaptionDisplayType.RemoveMinimizeButton();
        }

        /// <summary>
        /// Display the system menu for this window (i.e. the menu containing values like Move, Size, Minimize, and Close), at a specified point on the screen.
        /// </summary>
        /// <param name="p">The coordinates of the pixel at which to display this menu (relative to the screen).</param>
        public void DisplaySystemMenu(Point p)
        {
            // from https://stackoverflow.com/a/21830822/2987285
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            IntPtr hmnu = NativeMethods.GetSystemMenu(hwnd, false);
            int cmd = NativeMethods.TrackPopupMenu(hmnu, 0x100, (int)p.X, (int)p.Y, 0, hwnd, IntPtr.Zero);
            if (cmd > 0) NativeMethods.SendMessage(hwnd, 0x112, (IntPtr)cmd, IntPtr.Zero);
        }
        #endregion
    }
}