using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Timers;

// initial base is https://github.com/Stopbyte/WPF-Numeric-Spinner-NumericUpDown , but has since been heavily modified

namespace SolidShineUi
{
    /// <summary>
    /// A control for selecting a number, via typing in a number, an arithmetic expression, or using the up and down buttons. Only integer values are allowed.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Handled in Unloaded event")]
    public partial class IntegerSpinner : UserControl
    {

        #region Internal Values

        bool _updateBox = true; // set if the text box's text should be changed when the value is changed
        bool _raiseChangedEvent = true; // set if the ValueChanged event should be raised

        #endregion

        /// <summary>
        /// Raised when the Value, MinValue, or MaxValue properties are changed. Used internally to trigger revalidating the value.
        /// </summary>
        public event EventHandler PropertyChanged;

        /// <summary>
        /// Raised when the Value property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ValueChanged;

        /// <summary>
        /// Raised when the Value property is validated, and changed to a valid value if needed.
        /// </summary>
        public event EventHandler? ValueValidated;
#else
        public event DependencyPropertyChangedEventHandler ValueChanged;

        /// <summary>
        /// Raised when the Value property is validated, and changed to a valid value if needed.
        /// </summary>
        public event EventHandler ValueValidated;
#endif

        /// <summary>
        /// Create an IntegerSpinner.
        /// </summary>
        public IntegerSpinner()
        {
            InitializeComponent();

            Loaded += IntegerSpinner_Loaded;
            Unloaded += IntegerSpinner_Unloaded;

            //txtValue.SetBinding(TextBox.TextProperty, new Binding("Value")
            //{
            //    ElementName = "baseSpinner",
            //    Mode = BindingMode.TwoWay,
            //    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            //});

            // set up ValidateValue to run whenever these properties are updated (Value, MinValue, MaxValue)
            DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(IntegerSpinner)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(MinValueProperty, typeof(IntegerSpinner)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(MaxValueProperty, typeof(IntegerSpinner)).AddValueChanged(this, PropertyChanged);
            //DependencyPropertyDescriptor.FromProperty(RepeatDelayProperty, typeof(IntegerSpinner)).AddValueChanged(this, PropertyChanged);

            InternalValueChanged += IntegerSpinner_InternalValueChanged;
            InternalRepeatDelayChanged += IntegerSpinner_InternalRepeatDelayChanged;
            InternalCornerRadiusChanged += IntegerSpinner_InternalCornerRadiusChanged;
            InternalShowArrowsChanged += IntegerSpinner_InternalShowArrowsChanged;
            InternalDisplayAsHexChanged += IntegerSpinner_InternalDisplayAsHexChanged;

            PropertyChanged += (x, y) => ValidateValue();

            keyDownTimer.AutoReset = false;
            advanceTimer.AutoReset = true;

            keyDownTimer.Elapsed += KeyDownTimer_Elapsed;
            advanceTimer.Elapsed += AdvanceTimer_Elapsed;

            // moved here from loaded
            //ValidateValue();
        }

        private void IntegerSpinner_Loaded(object sender, EventArgs e)
        {
            txtValue.TextChanged += txtValue_TextChanged;
        }

        #region Disposing / Unloading

        /// <summary>
        /// Reset the spinner's internal timers; this should only be needed if this control is being unloaded and then later loaded.
        /// </summary>
        protected void ResetTimers()
        {
            advanceTimer.Dispose();
            keyDownTimer.Dispose();

            advanceTimer = new Timer(50);
            keyDownTimer = new Timer(300);
        }

        private void IntegerSpinner_Unloaded(object sender, RoutedEventArgs e)
        {
            advanceTimer.Dispose();
            keyDownTimer.Dispose();
        }

        #endregion

        private void IntegerSpinner_InternalValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            int value = Value;

            if (advanceTimer == null || btnDown == null || btnUp == null)
            {

            }
            else if (!advanceTimer.Enabled)
            {
                if (value < MinValue)
                {
                    value = MinValue;
                    Value = value;
                    return;
                }
                if (value > MaxValue)
                {
                    value = MaxValue;
                    Value = value;
                    return;
                }

                if (value == MinValue)
                {
                    btnDown.IsEnabled = false;
                    btnDown.Background = DisabledBrush;
                }
                else
                {
                    btnDown.IsEnabled = true;
                    btnDown.Background = ButtonBackground;
                }

                if (value == MaxValue)
                {
                    btnUp.IsEnabled = false;
                    btnUp.Background = DisabledBrush;
                }
                else
                {
                    btnUp.IsEnabled = true;
                    btnUp.Background = ButtonBackground;
                }
            }

            UpdateUI();
            if (_raiseChangedEvent) ValueChanged?.Invoke(this, e);
        }

        #region ColorScheme

        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif


        /// <summary>
        /// A dependency property object backing the related ColorScheme property. See <see cref="ColorScheme"/> for more details.
        /// </summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(IntegerSpinner),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        private static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            ColorScheme cs = (e.NewValue as ColorScheme)!;
#else
            ColorScheme cs = e.NewValue as ColorScheme;
#endif
            if (d is IntegerSpinner s)
            {
                s.ColorSchemeChanged?.Invoke(d, e);
                s.ApplyColorScheme(cs);
            }
        }

        /// <summary>
        /// Get or set the color scheme used for this spinner. For easier color scheme management, bind this to the window or larger control you're using.
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

            BorderBrush = cs.BorderColor.ToBrush();
            DisabledBrush = cs.LightDisabledColor.ToBrush();
            BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();

            if (cs.IsHighContrast)
            {
                ButtonBackground = cs.BackgroundColor.ToBrush();
                divider.BorderBrush = cs.BorderColor.ToBrush();
                pathUp.Fill = cs.BorderColor.ToBrush();
                pathDown.Fill = cs.BorderColor.ToBrush();
            }
            else
            {
                ButtonBackground = cs.SecondaryColor.ToBrush();
                divider.BorderBrush = cs.SecondHighlightColor.ToBrush();
                pathUp.Fill = cs.ForegroundColor.ToBrush();
                pathDown.Fill = cs.ForegroundColor.ToBrush();
            }

            if (IsEnabled)
            {
                brdr.BorderBrush = BorderBrush;
                visBorder.BorderBrush = BorderBrush;
                btnUp.Background = ButtonBackground;
                btnDown.Background = ButtonBackground;
            }
            else
            {
                brdr.BorderBrush = BorderDisabledBrush;
                visBorder.BorderBrush = BorderDisabledBrush;
                btnUp.Background = DisabledBrush;
                btnDown.Background = DisabledBrush;
            }

            _updateBox = false;
            UpdateUI();
            _updateBox = true;

            ClickBrush = cs.ThirdHighlightColor.ToBrush();
            HighlightBrush = cs.HighlightColor.ToBrush();
            Foreground = cs.ForegroundColor.ToBrush();
        }

        //public void ApplyColorScheme(HighContrastOption hco)
        //{
        //    ColorScheme cs = ColorScheme.GetHighContrastScheme(hco);

        //    ButtonBackground = cs.BackgroundColor.ToBrush();
        //    brdr.BorderBrush = cs.BorderColor.ToBrush();
        //    SelectionBrush = cs.ThirdHighlightColor.ToBrush();
        //    HighlightBrush = cs.HighlightColor.ToBrush();
        //    DisabledBrush = cs.LightDisabledColor.ToBrush();
        //    BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
        //    Foreground = cs.ForegroundColor.ToBrush();
        //}

        #endregion

        #region Brushes

        /// <summary>
        /// The backing dependency property for <see cref="ButtonBackground"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty ButtonBackgroundProperty = DependencyProperty.Register(
            "ButtonBackground", typeof(Brush), typeof(IntegerSpinner),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.White)));

        /// <summary>
        /// The backing dependency property for <see cref="DisabledBrush"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty DisabledBrushProperty = DependencyProperty.Register(
            "DisabledBrush", typeof(Brush), typeof(IntegerSpinner),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// The backing dependency property for <see cref="BorderBrush"/>. See the related property for more details.
        /// </summary>
        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(IntegerSpinner),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// The backing dependency property for <see cref="HighlightBrush"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(IntegerSpinner),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// The backing dependency property for <see cref="ClickBrush"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty ClickBrushProperty = DependencyProperty.Register(
            "ClickBrush", typeof(Brush), typeof(IntegerSpinner),
            new PropertyMetadata(new SolidColorBrush(Colors.Gainsboro)));

        /// <summary>
        /// The backing dependency property for <see cref="BorderDisabledBrush"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            "BorderDisabledBrush", typeof(Brush), typeof(IntegerSpinner),
            new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

        /// <summary>
        /// Get or set the brush used for the background of the buttons of the spinner.
        /// </summary>
        [Category("Brushes")]
        public Brush ButtonBackground
        {
            get
            {
                return (Brush)GetValue(ButtonBackgroundProperty);
            }
            set
            {
                SetValue(ButtonBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used for the background of the buttons when the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush DisabledBrush
        {
            get
            {
                return (Brush)GetValue(DisabledBrushProperty);
            }
            set
            {
                SetValue(DisabledBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush of the border around the control.
        /// </summary>
        [Category("Brushes")]
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

        /// <summary>
        /// Get or set the brush used when a button is highlighted (i.e. has a mouse over it or keyboard focus).
        /// </summary>
        [Category("Brushes")]
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
        /// Get or set the brush used when a button is being clicked.
        /// </summary>
        [Category("Brushes")]
        public Brush ClickBrush
        {
            get
            {
                return (Brush)GetValue(ClickBrushProperty);
            }
            set
            {
                SetValue(ClickBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used for the border around the control, while the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush BorderDisabledBrush
        {
            get
            {
                return (Brush)GetValue(BorderDisabledBrushProperty);
            }
            set
            {
                SetValue(BorderDisabledBrushProperty, value);
            }
        }

        #endregion

        #region ValueProperty

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalValueChanged;

        /// <summary>
        /// The backing dependency property for <see cref="Value"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(int), typeof(IntegerSpinner),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Get or set the value of the spinner.
        /// </summary>
        [Category("Common")]
        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //int value = (d as IntegerSpinner).Value;
            if (d is IntegerSpinner s)
            {
                s.InternalValueChanged?.Invoke(s, e);
            }
        }

        #endregion

        #region StepProperty

        /// <summary>
        /// The backing dependency property for <see cref="Step"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
            "Step", typeof(int), typeof(IntegerSpinner),
            new PropertyMetadata(1));

        ///<summary>
        /// Get or set how much to change the value by when you press the up or down button.
        ///</summary>
        [Category("Common")]
        public int Step
        {
            get => (int)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        #endregion

        #region MinValueProperty

        /// <summary>
        /// The backing dependency property for <see cref="MinValue"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue", typeof(int), typeof(IntegerSpinner),
            new PropertyMetadata(int.MinValue));

        ///<summary>
        /// Get or set the minimum value allowed for this spinner (inclusive).
        ///</summary>
        [Category("Common")]
        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set
            {
                if (value > MaxValue) { MaxValue = value; }
                SetValue(MinValueProperty, value);
            }
        }

        #endregion

        #region MaxValueProperty

        /// <summary>
        /// The backing dependency property for <see cref="MaxValue"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue", typeof(int), typeof(IntegerSpinner),
            new PropertyMetadata(int.MaxValue));

        ///<summary>
        /// Get or set the maximum value allowed for this spinner (inclusive).
        ///</summary>
        [Category("Common")]
        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set
            {
                if (value < MinValue) { value = MinValue; }

                SetValue(MaxValueProperty, value);
            }
        }

        #endregion

        #region RepeatDelayProperty

        /// <summary>
        /// The backing dependency property for <see cref="RepeatDelay"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty RepeatDelayProperty = DependencyProperty.Register(
            "RepeatDelay", typeof(double), typeof(IntegerSpinner),
            new PropertyMetadata(300d, new PropertyChangedCallback(OnInternalRepeatDelayChanged)));

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalRepeatDelayChanged;

        /// <summary>
        /// The backing routed event for <see cref="RepeatDelayChanged"/>. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent RepeatDelayChangedEvent = EventManager.RegisterRoutedEvent(
            "RepeatDelayChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(IntegerSpinner));

        /// <summary>
        /// Raised when the RepeatDelay property is changed.
        /// </summary>
        public event RoutedEventHandler RepeatDelayChanged
        {
            add { AddHandler(RepeatDelayChangedEvent, value); }
            remove { RemoveHandler(RepeatDelayChangedEvent, value); }
        }

        private static void OnInternalRepeatDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IntegerSpinner s)
            {
                s.InternalRepeatDelayChanged?.Invoke(s, e);
            }
        }

        private void IntegerSpinner_InternalRepeatDelayChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            keyDownTimer.Interval = RepeatDelay;
            RoutedEventArgs re = new RoutedEventArgs(RepeatDelayChangedEvent);
            RaiseEvent(re);
        }

        /// <summary>
        /// Get or set the delay period before starting the repeatedly stepping up or down while the button is held, in milliseconds. Default is 300 milliseconds.
        /// </summary>
        [Category("Common")]
        public double RepeatDelay
        {
            get => (double)GetValue(RepeatDelayProperty);
            set => SetValue(RepeatDelayProperty, value);
        }

        #endregion

        #region CornerRadiusProperty

        /// <summary>
        /// The backing dependency property for <see cref="CornerRadius"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", typeof(CornerRadius), typeof(IntegerSpinner),
            new PropertyMetadata(new CornerRadius(0), new PropertyChangedCallback(OnInternalCornerRadiusChanged)));

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalCornerRadiusChanged;

        /// <summary>
        /// The backing routed event for <see cref="CornerRadiusChanged"/>. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent CornerRadiusChangedEvent = EventManager.RegisterRoutedEvent(
            "CornerRadiusChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(IntegerSpinner));

        /// <summary>
        /// Raised when the CornerRadius property is changed.
        /// </summary>
        public event RoutedEventHandler CornerRadiusChanged
        {
            add { AddHandler(CornerRadiusChangedEvent, value); }
            remove { RemoveHandler(CornerRadiusChangedEvent, value); }
        }

        private static void OnInternalCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IntegerSpinner s)
            {
                s.InternalCornerRadiusChanged?.Invoke(s, e);
            }
        }

        private void IntegerSpinner_InternalCornerRadiusChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            brdrVisualEffect.CornerRadius = new CornerRadius(CornerRadius.TopLeft + 0.5, CornerRadius.TopRight + 0.5, CornerRadius.BottomRight + 0.5, CornerRadius.BottomLeft + 0.5);
            visBorder.CornerRadius = CornerRadius;

            RoutedEventArgs re = new RoutedEventArgs(CornerRadiusChangedEvent);
            RaiseEvent(re);
        }

        /// <summary>
        /// Get or set the corner radius to use around the corners of this control. Setting the corner radius to a value other than 0 displays rounded corners.
        /// </summary>
        [Category("Appearance")]
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #endregion

        #region AcceptExpressionsProperty

        /// <summary>
        /// The backing dependency property for <see cref="AcceptExpressions"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty AcceptExpressionsProperty = DependencyProperty.Register(
            "AcceptExpressions", typeof(bool), typeof(IntegerSpinner),  new PropertyMetadata(true));

        /// <summary>
        /// Get or set if the spinner should evaluate arithmetic expressions (such as "2+5") to accept as a value.
        /// </summary>
        /// <remarks>
        /// See the <see cref="ArithmeticParser"/> class for more info about how expressions are parsed.
        /// Note that AcceptExpressions is ignored (and expressions not accepted) while <see cref="DisplayAsHex"/> is set to true.
        /// </remarks>
        [Category("Common")]
        public bool AcceptExpressions
        {
            get => (bool)GetValue(AcceptExpressionsProperty);
            set =>  SetValue(AcceptExpressionsProperty, value);
        }

        #endregion

        #region ShowArrowsProperty

        /// <summary>
        /// The backing dependency property for <see cref="ShowArrows"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty ShowArrowsProperty = DependencyProperty.Register(
            "ShowArrows", typeof(bool), typeof(IntegerSpinner),
            new PropertyMetadata(true, new PropertyChangedCallback(OnInternalShowArrowsChanged)));

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalShowArrowsChanged;

        /// <summary>
        /// The backing routed event for <see cref="ShowArrowsChanged"/>. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent ShowArrowsChangedEvent = EventManager.RegisterRoutedEvent(
            "ShowArrowsChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(IntegerSpinner));

        /// <summary>
        /// Raised when the ShowArrows property is changed.
        /// </summary>
        public event RoutedEventHandler ShowArrowsChanged
        {
            add { AddHandler(ShowArrowsChangedEvent, value); }
            remove { RemoveHandler(ShowArrowsChangedEvent, value); }
        }

        private static void OnInternalShowArrowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IntegerSpinner s)
            {
                s.InternalShowArrowsChanged?.Invoke(s, e);
            }
        }

        private void IntegerSpinner_InternalShowArrowsChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            colArrows.Width = ShowArrows ? new GridLength(20) : new GridLength(0);
            RoutedEventArgs re = new RoutedEventArgs(ShowArrowsChangedEvent);
            RaiseEvent(re);
        }

        /// <summary>
        /// Get or set whether the up and down arrow buttons are shown.
        /// </summary>
        [Category("Common")]
        public bool ShowArrows
        {
            get => (bool)GetValue(ShowArrowsProperty);
            set => SetValue(ShowArrowsProperty, value);
        }

        #endregion

        #region DisplayAsHex

        /// <summary>
        /// The backing dependency property for <see cref="DisplayAsHex"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty DisplayAsHexProperty = DependencyProperty.Register(
            "DisplayAsHex", typeof(bool), typeof(IntegerSpinner),
            new PropertyMetadata(false, new PropertyChangedCallback(OnInternalDisplayAsHexChanged)));

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalDisplayAsHexChanged;

        /// <summary>
        /// The backing routed event for <see cref="DisplayAsHexChanged"/>. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent DisplayAsHexChangedEvent = EventManager.RegisterRoutedEvent(
            "DisplayAsHexChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(IntegerSpinner));

        /// <summary>
        /// Raised when the DisplayAsHex property is changed.
        /// </summary>
        public event RoutedEventHandler DisplayAsHexChanged
        {
            add { AddHandler(DisplayAsHexChangedEvent, value); }
            remove { RemoveHandler(DisplayAsHexChangedEvent, value); }
        }

        private static void OnInternalDisplayAsHexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IntegerSpinner s)
            {
                s.InternalDisplayAsHexChanged?.Invoke(s, e);
            }
        }

        private void IntegerSpinner_InternalDisplayAsHexChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RoutedEventArgs re = new RoutedEventArgs(DisplayAsHexChangedEvent);
            RaiseEvent(re);

            UpdateUI();
            //_raiseChangedEvent = false;
            //if ((bool)e.NewValue == true)
            //{
            //    if (txtValue.Text != Value.ToString("X"))
            //    {
            //        if (_updateBox) txtValue.Text = Value.ToString("X");
            //    }
            //}
            //else
            //{
            //    if (txtValue.Text != Value.ToString())
            //    {
            //        if (_updateBox) txtValue.Text = Value.ToString();
            //    }
            //}
            //_raiseChangedEvent = true;
        }

        /// <summary>
        /// Get or set whether to show the value as a hexadecimal or decimal value. Note that while DisplayAsHex is set to true, <see cref="AcceptExpressions"/> is ignored.
        /// </summary>
        /// <remarks>
        /// Certain situations, particularly involving computer representations of data or memory, may benefit more with displaying numbers as hexadecimals rather than decimals.
        /// With hexadecimals, the letters A-F are allowed along with 0-9, and the number "15" in decimal turns into "F" in hexadecimal. Please view online resources like
        /// Wikipedia for more details.
        /// </remarks>
        [Category("Common")]
        public bool DisplayAsHex
        {
            get => (bool)GetValue(DisplayAsHexProperty);
            set => SetValue(DisplayAsHexProperty, value);
        }

        #endregion

        #region MinimumDigitCount

        /// <summary>
        /// The backing dependency property for <see cref="MinimumDigitCount"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty MinimumDigitCountProperty = DependencyProperty.Register(
            "MinimumDigitCount", typeof(int), typeof(IntegerSpinner),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Get or set the minimum number of digits to display in the spinner. A value of 0 or lower will display only the minimum needed for the number being displayed.
        /// </summary>
        /// <remarks>
        /// This modifies the number of digits being rendered via <see cref="int.ToString(string)"/>.
        /// Setting this to <c>4</c> and then setting the Value to <c>16</c> will render the text <c>0016</c> in the display. The stored value isn't affected.
        /// You should generally avoid setting this to numbers larger than 99, as larger numbers are only supported in newer versions of .NET.
        /// </remarks>
        [Category("Common")]
        public int MinimumDigitCount
        {
            get => (int)GetValue(MinimumDigitCountProperty);
            set => SetValue(MinimumDigitCountProperty, value);
        }

        #endregion

        /// <summary>
        /// Validate the value and update the UI if needed.
        /// </summary>
        void ValidateValue()
        {
            if (MinValue > MaxValue) MinValue = MaxValue;
            if (MaxValue < MinValue) MaxValue = MinValue;
            if (Value < MinValue) Value = MinValue;
            if (Value > MaxValue) Value = MaxValue;
            
            UpdateUI();
            ValueValidated?.Invoke(this, EventArgs.Empty);
        }

        void UpdateUI()
        {
            if (!IsEnabled)
            {
                brdr.BorderBrush = BorderDisabledBrush;
                visBorder.BorderBrush = BorderDisabledBrush;
                btnUp.Background = DisabledBrush;
                btnDown.Background = DisabledBrush;

                advanceTimer.Stop();
            }
            else
            {
                if (Value == MinValue)
                {
                    btnDown.IsEnabled = false;
                    btnDown.Background = DisabledBrush;
                    advanceTimer.Stop();
                }
                else
                {
                    if (advanceTimer.Enabled && !advanceStepUp)
                    {
                        btnDown.Background = ClickBrush;
                    }
                    else
                    {
                        btnDown.Background = ButtonBackground;
                    }
                    btnDown.IsEnabled = true;
                }

                if (Value == MaxValue)
                {
                    btnUp.IsEnabled = false;
                    btnUp.Background = DisabledBrush;
                    advanceTimer.Stop();
                }
                else
                {
                    if (advanceTimer.Enabled && advanceStepUp)
                    {
                        btnUp.Background = ClickBrush;
                    }
                    else
                    {
                        btnUp.Background = ButtonBackground;
                    }
                    btnUp.IsEnabled = true;
                }
            }

            string digitDisplay = "";
            if (MinimumDigitCount > 0) { digitDisplay = MinimumDigitCount.ToString("G", null); }
            string sVal = Value.ToString((DisplayAsHex ? "X" : "D") + digitDisplay, null);

            if (txtValue.Text != sVal)
            {
                if (_updateBox) txtValue.Text = sVal;
            }
        }

        #region Textbox

        private void txtValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtValue == null)
            {
                return;
            }

            _updateBox = false;
            if (DisplayAsHex)
            {
                if (int.TryParse(txtValue.Text, System.Globalization.NumberStyles.HexNumber, null, out int ival))
                {
                    Value = ival;
                }
            }
            else
            {
                if (int.TryParse(txtValue.Text, System.Globalization.NumberStyles.Integer, null, out int ival))
                {
                    Value = ival;
                }
                else if (AcceptExpressions && ArithmeticParser.IsValidString(txtValue.Text))
                {
                    try
                    {
                        Value = (int)Math.Round(ArithmeticParser.Evaluate(txtValue.Text, null), MidpointRounding.AwayFromZero);
                    }
                    catch (FormatException)
                    {

                    }
                    catch (ArgumentOutOfRangeException)
                    {

                    }
                }
            }
            _updateBox = true;
        }

        private void txtValue_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateValue();
        }

        private void txtValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                ValidateValue();
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                advanceStepUp = false;
                keyDownTimer.Start();
            }
            else if (e.Key == Key.Up)
            {
                advanceStepUp = true;
                keyDownTimer.Start();
            }
        }

        private void txtValue_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                if (advanceTimer.Enabled)
                {
                    advanceTimer.Stop();
                }
                else
                {
                    keyDownTimer.Stop();
                    if (Value >= MinValue) Value -= Step;
                    else Value = MinValue;
                    //UpdateUI();
                }
            }
            else if (e.Key == Key.Up)
            {
                if (advanceTimer.Enabled)
                {
                    advanceTimer.Stop();
                }
                else
                {
                    keyDownTimer.Stop();
                    if (Value <= MaxValue) Value += Step;
                    else Value = MaxValue;
                    //UpdateUI();
                }
            }
        }

        #endregion

        private void baseSpinner_Loaded(object sender, RoutedEventArgs e)
        {
            // doesn't work in constructor, apparently
            _raiseChangedEvent = false;
            ValidateValue();
            _raiseChangedEvent = true;
        }

        Timer keyDownTimer = new Timer(300);
        Timer advanceTimer = new Timer(50);
        bool advanceStepUp = false;

        private void btnUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            btnUp.Background = ClickBrush;
            advanceStepUp = true;
            keyDownTimer.Start();
        }

        private void btnUp_MouseUp(object sender, MouseButtonEventArgs e)
        {
            btnUp.Background = HighlightBrush;

            if (advanceTimer.Enabled)
            {
                advanceTimer.Stop();
            }
            else
            {
                keyDownTimer.Stop();
                if (Value <= MaxValue) Value += Step;
                else Value = MaxValue;
                UpdateUI();
            }
        }

        private void btnDown_MouseDown(object sender, MouseButtonEventArgs e)
        {
            btnDown.Background = ClickBrush;
            advanceStepUp = false;
            keyDownTimer.Start();
        }

        private void btnDown_MouseUp(object sender, MouseButtonEventArgs e)
        {
            btnDown.Background = HighlightBrush;

            if (advanceTimer.Enabled)
            {
                advanceTimer.Stop();
            }
            else
            {
                keyDownTimer.Stop();
                if (Value >= MinValue) Value -= Step;
                else Value = MinValue;
                UpdateUI();
            }
        }

#if NETCOREAPP
        private void KeyDownTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            advanceTimer.Start();
        }

        private void AdvanceTimer_Elapsed(object? sender, ElapsedEventArgs e)
#else
        private void KeyDownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            advanceTimer.Start();
        }

        private void AdvanceTimer_Elapsed(object sender, ElapsedEventArgs e)
#endif
        {
            try
            {
#if (NETCOREAPP || NET45_OR_GREATER)
                Dispatcher.Invoke(() =>
                {
                    if (advanceStepUp)
                    {
                        if (Value < MaxValue) Value += Step;
                        else Value = MaxValue;
                    }
                    else
                    {
                        if (Value > MinValue) Value -= Step;
                        else Value = MinValue;
                    }

                    UpdateUI();
                }, System.Windows.Threading.DispatcherPriority.Input);
#else
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input, new DoStep(DoStepAction));
#endif
            }
            catch (TaskCanceledException)
            {
                advanceTimer.Stop();
            }
        }

#if !(NETCOREAPP || NET45_OR_GREATER)
        delegate void DoStep();

        void DoStepAction()
        {
            if (advanceStepUp)
            {
                if (Value < MaxValue) Value += Step;
                else Value = MaxValue;
            }
            else
            {
                if (Value > MinValue) Value -= Step;
                else Value = MinValue;
            }

            UpdateUI();
        }
#endif

        private void btnUp_MouseEnter(object sender, MouseEventArgs e)
        {
            btnUp.Background = HighlightBrush;
        }

        private void btnUp_MouseLeave(object sender, MouseEventArgs e)
        {
            btnUp.Background = ButtonBackground;
            if (advanceStepUp) advanceTimer.Stop();
            UpdateUI();
        }

        private void btnDown_MouseEnter(object sender, MouseEventArgs e)
        {
            btnDown.Background = HighlightBrush;
        }

        private void btnDown_MouseLeave(object sender, MouseEventArgs e)
        {
            btnDown.Background = ButtonBackground;
            if (!advanceStepUp) advanceTimer.Stop();
            UpdateUI();
        }

        private void BaseSpinner_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsEnabled)
            {
                brdr.BorderBrush = BorderBrush;
                visBorder.BorderBrush = BorderBrush;
                btnUp.Background = ButtonBackground;
                btnDown.Background = ButtonBackground;
                UpdateUI();
            }
            else
            {
                brdr.BorderBrush = BorderDisabledBrush;
                visBorder.BorderBrush = BorderDisabledBrush;
                btnUp.Background = DisabledBrush;
                btnDown.Background = DisabledBrush;
            }
        }

        #region Touch/Stylus

        private void btnUp_TouchDown(object sender, TouchEventArgs e)
        {
            btnUp.Background = ClickBrush;
            advanceStepUp = true;
            keyDownTimer.Start();
        }

        private void btnUp_TouchUp(object sender, TouchEventArgs e)
        {
            btnUp.Background = ButtonBackground;

            if (advanceTimer.Enabled)
            {
                advanceTimer.Stop();
            }
            else
            {
                keyDownTimer.Stop();
                if (Value <= MaxValue) Value += Step;
                else Value = MaxValue;
                UpdateUI();
            }
        }

        private void btnUp_StylusDown(object sender, StylusDownEventArgs e)
        {
            btnUp.Background = ButtonBackground;
            advanceStepUp = true;
            keyDownTimer.Start();
        }

        private void btnUp_StylusUp(object sender, StylusEventArgs e)
        {
            btnUp.Background = ButtonBackground;

            if (advanceTimer.Enabled)
            {
                advanceTimer.Stop();
            }
            else
            {
                keyDownTimer.Stop();
                if (Value <= MaxValue) Value += Step;
                else Value = MaxValue;
                UpdateUI();
            }
        }

        private void btnDown_TouchDown(object sender, TouchEventArgs e)
        {
            btnDown.Background = ClickBrush;
            advanceStepUp = false;
            keyDownTimer.Start();
        }

        private void btnDown_TouchUp(object sender, TouchEventArgs e)
        {
            btnDown.Background = ButtonBackground;

            if (advanceTimer.Enabled)
            {
                advanceTimer.Stop();
            }
            else
            {
                keyDownTimer.Stop();
                if (Value >= MinValue) Value -= Step;
                else Value = MinValue;
                UpdateUI();
            }
        }

        private void btnDown_StylusDown(object sender, StylusDownEventArgs e)
        {
            btnDown.Background = ButtonBackground;
            advanceStepUp = false;
            keyDownTimer.Start();
        }

        private void btnDown_StylusUp(object sender, StylusEventArgs e)
        {
            btnDown.Background = ButtonBackground;

            if (advanceTimer.Enabled)
            {
                advanceTimer.Stop();
            }
            else
            {
                keyDownTimer.Stop();
                if (Value >= MinValue) Value -= Step;
                else Value = MinValue;
                UpdateUI();
            }
        }

        #endregion
    }
}
