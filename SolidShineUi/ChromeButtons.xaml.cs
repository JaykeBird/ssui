﻿using System.Windows;
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
        /// Gets or sets the internal padding in each of the caption buttons. Default value is (9, 7, 9, 7).
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
        /// The backing dependency property object for a related property. See that property for more details.
        /// </summary>
        public static readonly DependencyProperty ButtonPaddingProperty = DependencyProperty.Register(
             "ButtonPadding", typeof(Thickness), typeof(ChromeButtons), new FrameworkPropertyMetadata(new Thickness(9, 7, 9, 7)));

        /// <summary>
        /// Gets or sets the margin (spacing) around each of the caption buttons.
        /// </summary>
        public Thickness ButtonMargin
        {
            get
            {
                return (Thickness) GetValue(ButtonMarginProperty);
            }
            set
            {
                SetValue(ButtonMarginProperty, value);
            }
        }

        /// <summary>
        /// The backing dependency property object for a related property. See that property for more details.
        /// </summary>
        public static readonly DependencyProperty ButtonMarginProperty = DependencyProperty.Register(
             "ButtonMargin", typeof(Thickness), typeof(ChromeButtons));

        #endregion

        #region Caption Type

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


        /// <summary>
        /// The backing dependency property object for a related property. See that property for more details.
        /// </summary>
        public static readonly DependencyProperty DisplayTypeProperty = DependencyProperty.Register(
            "DisplayType", typeof(CaptionType), typeof(ChromeButtons),
            new PropertyMetadata(CaptionType.Full));
        
        #endregion

        #region Brushes
        /// <summary>
        /// Get or set the brush used when a button is being clicked.
        /// </summary>
        public Brush ClickBrush
        {
            get
            {
                return (Brush) GetValue(ClickBrushProperty);
            }
            set
            {
                SetValue(ClickBrushProperty, value);
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
        /// The backing dependency property object for a related property. See that property for more details.
        /// </summary>
        public static readonly DependencyProperty ClickBrushProperty = DependencyProperty.Register(
            "ClickBrush", typeof(Brush), typeof(ChromeButtons),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// The backing dependency property object for a related property. See that property for more details.
        /// </summary>
        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(ChromeButtons),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region Mouse Event Handling (update brushes)
        bool mouseEntered = false;

        private void Button_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((Button) sender).Background = ClickBrush;
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
