using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SolidShineUi
{
    /// <summary>
    /// Interaction logic for ChromeButtons.xaml
    /// </summary>
    public partial class ChromeButtons
    {
        /// <summary>
        /// The parent Window of the control.
        /// </summary>
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

        /// <summary>
        /// Event when the control is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void CaptionButtonsLoaded(object sender, RoutedEventArgs e)
        {
            _parent = GetTopParent();
        }

        /// <summary>
        /// Action on the button to close the window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            if (_parent == null) return;
            _parent.Close();
        }

        /// <summary>
        /// Changes the view of the window (maximized or normal).
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void RestoreButtonClick(object sender, RoutedEventArgs e)
        {
            if (_parent == null) return;
            _parent.WindowState = _parent.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        /// <summary>
        /// Minimizes the Window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            if (_parent == null) return;
            _parent.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Gets the top parent (Window).
        /// </summary>
        /// <returns>The parent Window.</returns>
        private Window GetTopParent()
        {
            return Window.GetWindow(this);
        }

        /// <summary>
        /// Gets or sets the margin button.
        /// </summary>
        /// <value>The margin button.</value>
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
        /// The dependency property for the Margin between the buttons.
        /// </summary>
        public static readonly DependencyProperty MarginButtonProperty = DependencyProperty.Register(
             "MarginButton", typeof(Thickness), typeof(ChromeButtons));

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
            MaximizeClose
        }

        /// <summary>
        /// Gets or sets the visibility of the buttons.
        /// </summary>
        /// <value>The visible buttons.</value>
        public CaptionType DisplayType
        {
            get
            {
                return (CaptionType) GetValue(TypeProperty);
            }
            set
            {
                SetValue(TypeProperty, value);
            }
        }

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
        /// The dependency property for the Margin between the buttons.
        /// </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "DisplayType", typeof(CaptionType), typeof(ChromeButtons),
            new PropertyMetadata(CaptionType.Full));

        public static readonly DependencyProperty SelectionBrushProperty = DependencyProperty.Register(
            "SelectionBrush", typeof(Brush), typeof(ChromeButtons),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(ChromeButtons),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        private void Button_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((Button) sender).Background = SelectionBrush;
        }

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((Button) sender).Background = HighlightBrush;
        }

        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((Button) sender).Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }
    }
}
