using System.Windows;
//using Microsoft.Windows.Shell;
using System.Windows.Media;
using static SolidShineUi.ChromeButtons;
//using System.Collections.Generic;
using System.Windows.Shell;

namespace SolidShineUi
{
    /// <summary>
    /// A WPF window that has a custom, flat style and additional functionality.
    /// </summary>
    public class FlatWindow : Window
    {
        static FlatWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlatWindow), new FrameworkPropertyMetadata(typeof(FlatWindow)));
        }

        #region Color Scheme
        /// <summary>
        /// Get if the window's color scheme is a high contrast theme (and thus, the window is displaying high-contrast colors).
        /// </summary>
        public bool HighContrastMode { get; private set; } = false;

        // TODO: add different color for inactive window caption (especially for High Contrast Mode)

        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(FlatWindow),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            if (d is FlatWindow w)
            {
                w.ApplyColorScheme((e.NewValue as ColorScheme)!);
            }
#else
            (d as FlatWindow).ApplyColorScheme(e.NewValue as ColorScheme);
#endif
        }

        /// <summary>
        /// Get or set the color scheme to apply to the window.
        /// </summary>
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        public void ApplyColorScheme(ColorScheme cs)
        {
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }

            if (cs.IsHighContrast)
            {
                HighContrastMode = true;
                InactiveTextBrush = cs.WindowTitleBarTextColor.ToBrush();
            }
            else
            {
                InactiveTextBrush = ColorsHelper.CreateFromHex("#505050").ToBrush();
            }

            Background = cs.WindowTitleBarColor.ToBrush();
            InactiveBackground = cs.WindowInactiveColor.ToBrush();
            CaptionButtonsBrush = cs.WindowTitleBarTextColor.ToBrush();
            CaptionTextBrush = cs.WindowTitleBarTextColor.ToBrush();
            BorderBrush = cs.BorderColor.ToBrush();
            HighlightBrush = cs.HighlightColor.ToBrush();
            SelectionBrush = cs.SelectionColor.ToBrush();
            Foreground = cs.ForegroundColor.ToBrush();
            ContentBackground = cs.BackgroundColor.ToBrush();
        }

        public void ApplyColorScheme(HighContrastOption hco)
        {
            ColorScheme cs = ColorScheme.GetHighContrastScheme(hco);

            ApplyColorScheme(cs);
        }
        #endregion

        #region Layout/Appearance Properties

        int captionHeight = 29;

        public static readonly DependencyProperty CaptionDisplayTypeProperty = DependencyProperty.Register(
            "CaptionDisplayType", typeof(CaptionType), typeof(FlatWindow),
            new PropertyMetadata(CaptionType.Full));

        /// <summary>
        /// Gets or sets the visibility of the caption buttons (close, maximize, minimize). The actions may still be available via other methods even if the buttons are hidden.
        /// </summary>
        /// <remarks>Use the <c>DisableMinimizeAction</c> and <c>DisableMaximizeAction</c> methods to further prevent the user from being able to minimize or maximize the window. The standard <c>ResizeMode</c> property works as well.</remarks>
        public CaptionType CaptionDisplayType
        {
            get
            {
                return (CaptionType)GetValue(CaptionDisplayTypeProperty);
            }
            set
            {
                SetValue(CaptionDisplayTypeProperty, value);
            }
        }

        /// <summary>
        /// Get or set the height of the caption (title bar) area of the window. Default is 29.
        /// </summary>
        public int CaptionHeight
        {
            get
            {
                return captionHeight;
            }
            set
            {
                captionHeight = value;
                ResizeCaptionHeight(value);
            }
        }

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

        public static readonly DependencyProperty TopRightElementProperty = DependencyProperty.Register(
            "TopRightElement", typeof(UIElement), typeof(FlatWindow),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the UI element to place in the top-right of the window, to the left of the caption buttons.
        /// </summary>
        public UIElement TopRightElement
        {
            get
            {
                return (UIElement)GetValue(TopRightElementProperty);
            }
            set
            {
                SetValue(TopRightElementProperty, value);
            }
        }

        public static readonly DependencyProperty TopLeftElementProperty = DependencyProperty.Register(
            "TopLeftElement", typeof(UIElement), typeof(FlatWindow),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the UI element to place in the top-left corner of the window.
        /// </summary>
        public UIElement TopLeftElement
        {
            get
            {
                return (UIElement)GetValue(TopLeftElementProperty);
            }
            set
            {
                SetValue(TopLeftElementProperty, value);
            }
        }

        public static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register(
            "ShowTitle", typeof(bool), typeof(FlatWindow),
            new PropertyMetadata(true));

        public bool ShowTitle
        {
            get
            {
                return (bool)GetValue(ShowTitleProperty);
            }
            set
            {
                SetValue(ShowTitleProperty, value);
            }
        }

        public static readonly DependencyProperty ExcludeTopLeftElementFromChromeProperty = DependencyProperty.Register(
            "ExcludeTopLeftElementFromChrome", typeof(bool), typeof(FlatWindow),
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
            get
            {
                return (bool)GetValue(ExcludeTopLeftElementFromChromeProperty);
            }
            set
            {
                SetValue(ExcludeTopLeftElementFromChromeProperty, value);
            }
        }

        public static readonly DependencyProperty ExcludeTopRightElementFromChromeProperty = DependencyProperty.Register(
            "ExcludeTopRightElementFromChrome", typeof(bool), typeof(FlatWindow),
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
            get
            {
                return (bool)GetValue(ExcludeTopRightElementFromChromeProperty);
            }
            set
            {
                SetValue(ExcludeTopRightElementFromChromeProperty, value);
            }
        }

        public static readonly new DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
            "BorderThickness", typeof(Thickness), typeof(FlatWindow),
            new PropertyMetadata(new Thickness(1)));

        public new Thickness BorderThickness
        {
            get
            {
                return (Thickness)GetValue(BorderThicknessProperty);
            }
            set
            {
                SetValue(BorderThicknessProperty, value);
            }
        }

        #endregion

        #region Brushes

        public Brush SelectionBrush
        {
            get
            {
                return (Brush)GetValue(SelectionBrushProperty);
            }
            set
            {
                SetValue(SelectionBrushProperty, value);
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                return (Brush)GetValue(HighlightBrushProperty);
            }
            set
            {
                SetValue(HighlightBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the background brush to use for the caption area (title bar). Use <c>ContentBackground</c> for the content area of the window.
        /// </summary>
        public new Brush Background
        {
            get
            {
                return (Brush)GetValue(BackgroundProperty);
            }
            set
            {
                SetValue(BackgroundProperty, value);
            }
        }

        public Brush CaptionButtonsBrush
        {
            get
            {
                return (Brush)GetValue(CaptionButtonsBrushProperty);
            }
            set
            {
                SetValue(CaptionButtonsBrushProperty, value);
            }
        }

        public Brush CaptionTextBrush
        {
            get
            {
                return (Brush)GetValue(CaptionTextBrushProperty);
            }
            set
            {
                SetValue(CaptionTextBrushProperty, value);
            }
        }

        public new Brush BorderBrush
        {
            get
            {
                return (Brush)GetValue(BorderBrushProperty);
            }
            set
            {
                SetValue(BorderBrushProperty, value);
            }
        }

        public Brush InactiveTextBrush
        {
            get
            {
                return (Brush)GetValue(InactiveTextBrushProperty);
            }
            set
            {
                SetValue(InactiveTextBrushProperty, value);
            }
        }

        public Brush InactiveBackground
        {
            get
            {
                return (Brush)GetValue(InactiveBackgroundProperty);
            }
            set
            {
                SetValue(InactiveBackgroundProperty, value);
            }
        }

        public Brush ContentBackground
        {
            get
            {
                return (Brush)GetValue(ContentBackgroundProperty);
            }
            set
            {
                SetValue(ContentBackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty SelectionBrushProperty = DependencyProperty.Register(
            "SelectionBrush", typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly new DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.White)));

        public static readonly DependencyProperty CaptionButtonsBrushProperty = DependencyProperty.Register(
            "CaptionButtonsBrush", typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.Black)));

        public static readonly DependencyProperty CaptionTextBrushProperty = DependencyProperty.Register(
            "CaptionTextBrush", typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.Black)));

        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.Black)));

        public static readonly DependencyProperty InactiveTextBrushProperty = DependencyProperty.Register(
            "InactiveTextBrush", typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));

        public static readonly DependencyProperty InactiveBackgroundProperty = DependencyProperty.Register(
            "InactiveBackground", typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));

        public static readonly DependencyProperty ContentBackgroundProperty = DependencyProperty.Register(
            "ContentBackground", typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));

        #endregion

        #region Added Methods (using NativeWindowMethods)

        /// <summary>
        /// Calls the native Windows method to disable the Maximize action (via the right-click menu and the like). Note that you'll want to update the CaptionDisplayType to remove the physical button.
        /// </summary>
        public void DisableMaximizeAction()
        {
            NativeMethods.HideMaximizeButton(this);
        }

        /// <summary>
        /// Calls the native Windows method to disable the Minimize action (via the right-click menu and the like). Note that you'll want to update the CaptionDisplayType to remove the physical button.
        /// </summary>
        public void DisableMinimizeAction()
        {
            NativeMethods.HideMinimizeButton(this);
        }

        /// <summary>
        /// Calls the native Windows method to disable the Minimize and Maximize actions (via the right-click menu and the like). Note that you'll want to update the CaptionDisplayType to remove the physical buttons.
        /// </summary>
        public void DisableMinimizeAndMaximizeActions()
        {
            NativeMethods.HideMinimizeAndMaximizeButtons(this);
        }

        #endregion

    }
}