using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SolidShineUi
{
    /// <summary>
    /// A control that hosts the caption buttons for a <see cref="FlatWindow"/>.
    /// These buttons are the Minimize, Maximize/Restore, and Close buttons visible at the top corner of most windows.
    /// </summary>
    public partial class ChromeButtons : UserControl
    {

#if NETCOREAPP
        private Window? _parent;
#else
        private Window _parent;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeButtons"/> class.
        /// </summary>
        public ChromeButtons()
        {
            InitializeComponent();
            Loaded += CaptionButtonsLoaded;
        }

        #region Window Interop (Button Click Handling)
        void CaptionButtonsLoaded(object sender, RoutedEventArgs e)
        {
            _parent = GetTopParent();
        }

        // Close the window
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            if (_parent == null) return;
            _parent.Close();
        }

        // Maximize or restore the window
        private void RestoreButtonClick(object sender, RoutedEventArgs e)
        {
            if (_parent == null) return;
            _parent.WindowState = _parent.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        // Minimize the window
        private void MinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            if (_parent == null) return;
            _parent.WindowState = WindowState.Minimized;
        }

        // Get the parent window
        private Window GetTopParent()
        {
            return Window.GetWindow(this);
        }
        #endregion

        #region Layout Properties
        /// <summary>
        /// Gets or sets the internal padding in each of the caption buttons.
        /// </summary>
        public Thickness ButtonPadding
        {
            get
            {
                return (Thickness)GetValue(ButtonPaddingProperty);

            }
            set
            {
                SetValue(ButtonPaddingProperty, value);
            }
        }

        /// <summary>
        /// A dependency property object backing the related property. See <see cref="ButtonPadding"/> for more details.
        /// </summary>
        public static readonly DependencyProperty ButtonPaddingProperty = DependencyProperty.Register(
             "ButtonPadding", typeof(Thickness), typeof(ChromeButtons), new FrameworkPropertyMetadata(new Thickness(9, 7, 9, 7)));

        // TODO: rename to ButtonMargin
        /// <summary>
        /// Gets or sets the margin (spacing) around each of the caption buttons.
        /// </summary>
        /// <remarks>
        /// For Solid Shine UI 2.0, this will be renamed to ButtonMargin.
        /// </remarks>
        public Thickness MarginButton
        {
            get
            {
                return (Thickness) GetValue(MarginButtonProperty);
                
            }
            set
            {
                SetValue(MarginButtonProperty, value);
            }
        }

        /// <summary>
        /// A dependency property object backing the related property. See <see cref="MarginButton"/> for more details.
        /// </summary>
        public static readonly DependencyProperty MarginButtonProperty = DependencyProperty.Register(
             "MarginButton", typeof(Thickness), typeof(ChromeButtons));
        #endregion

        #region Caption Type
        /// <summary>
        /// Determines which caption buttons will appear on a window.
        /// </summary>
        public enum CaptionType
        {
            /// <summary>
            /// Display the close, maximize/restore, and minimize buttons.
            /// </summary>
            Full,
            /// <summary>
            /// Display only the close button.
            /// </summary>
            Close,
            /// <summary>
            /// Display only the close and minimize buttons.
            /// </summary>
            MinimizeClose,
            /// <summary>
            /// Display only the close and maximize/restore buttons.
            /// </summary>
            MaximizeClose,
            /// <summary>
            /// Do not display any caption buttons.
            /// (Note that hiding caption buttons alone doesn't prevent users from being able to perform their actions via other methods.)
            /// </summary>
            None
        }

        /// <summary>
        /// Gets or sets the visibility of the caption buttons, specifically which buttons should be visible.
        /// </summary>
        /// <value>The visible buttons.</value>
        /// <remarks>
        /// Note that this does not disable these functions from being possible with a window, it only hides the buttons in the top corner.
        /// Please use <see cref="FlatWindow.DisableMaximizeAction"/> and <see cref="FlatWindow.DisableMinimizeAction"/> to disable actions in a window.
        /// </remarks>
        public CaptionType DisplayType
        {
            get
            {
                return (CaptionType) GetValue(DisplayTypeProperty);
            }
            set
            {
                SetValue(DisplayTypeProperty, value);
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty DisplayTypeProperty = DependencyProperty.Register(
            "DisplayType", typeof(CaptionType), typeof(ChromeButtons),
            new PropertyMetadata(CaptionType.Full));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        #endregion

        #region Brushes
        /// <summary>
        /// Get or set the brush used when a button is being clicked.
        /// </summary>
        /// <remarks>
        /// For Solid Shine UI 2.0, this will be renamed to ClickBrush.
        /// </remarks>
        public Brush SelectionBrush
        {
            get
            {
                return (Brush) GetValue(SelectionBrushProperty);
            }
            set
            {
                SetValue(SelectionBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used when a button is being highlighted.
        /// </summary>
        public Brush HighlightBrush
        {
            get
            {
                return (Brush) GetValue(HighlightBrushProperty);
            }
            set
            {
                SetValue(HighlightBrushProperty, value);
            }
        }

        /// <summary>
        /// A dependency property object backing the related property. See <see cref="SelectionBrush"/> for more details.
        /// </summary>
        public static readonly DependencyProperty SelectionBrushProperty = DependencyProperty.Register(
            "SelectionBrush", typeof(Brush), typeof(ChromeButtons),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// A dependency property object backing the related property. See <see cref="HighlightBrush"/> for more details.
        /// </summary>
        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(ChromeButtons),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        bool mouseEntered = false;

        private void Button_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((Button) sender).Background = SelectionBrush;
        }

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((Button) sender).Background = HighlightBrush;
            mouseEntered = true;
        }

        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((Button) sender).Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            mouseEntered = false;
        }
        private void Button_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (mouseEntered)
            {
                ((Button)sender).Background = HighlightBrush;
            }
            else
            {
                ((Button)sender).Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }
        }
        #endregion

    }
}
