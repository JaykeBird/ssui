using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

// Source: https://github.com/Stopbyte/WPF-Numeric-Spinner-NumericUpDown

namespace SolidShineUi
{
    /// <summary>
    /// A control for selecting a number, via typing in a number, an arithmetic expression, or using the up and down buttons.
    /// </summary>
    public partial class DoubleSpinner : UserControl
    {

        #region Internal Values

        bool _updateBox = true; // set if the text box's text should be changed when the value is changed
        bool _raiseChangedEvent = true; // set if the ValueChanged event should be raised

        #endregion

        /// <summary>
        /// Raised when the Value, Decimals, MinValue, or MaxValue properties are changed. Used internally to trigger revalidating the value.
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
        /// Create a DoubleSpinner.
        /// </summary>
        public DoubleSpinner()
        {
            InitializeComponent();
            Loaded += DoubleSpinner_Loaded;

            //DoubleStringConverter dsc = new DoubleStringConverter();

            //txtValue.SetBinding(TextBox.TextProperty, new Binding("Value")
            //{
            //    ElementName = "baseSpinner",
            //    Mode = BindingMode.TwoWay,
            //    //Converter = dsc,
            //    //ConverterParameter = Decimals,
            //    Delay = 1000,
            //    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            //});

            DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(DoubleSpinner)).AddValueChanged(this, PropertyChanged);
            //DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(DoubleSpinner)).AddValueChanged(this, ValueChanged);
            DependencyPropertyDescriptor.FromProperty(DecimalsProperty, typeof(DoubleSpinner)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(MinValueProperty, typeof(DoubleSpinner)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(MaxValueProperty, typeof(DoubleSpinner)).AddValueChanged(this, PropertyChanged);

            InternalValueChanged += DoubleSpinner_InternalValueChanged;
            InternalRepeatDelayChanged += DoubleSpinner_InternalRepeatDelayChanged;
            InternalCornerRadiusChanged += DoubleSpinner_InternalCornerRadiusChanged;
            InternalShowArrowsChanged += DoubleSpinner_InternalShowArrowsChanged;

            PropertyChanged += (x, y) => ValidateValue();

            keyDownTimer.AutoReset = false;
            advanceTimer.AutoReset = true;

            keyDownTimer.Elapsed += KeyDownTimer_Elapsed;
            advanceTimer.Elapsed += AdvanceTimer_Elapsed;
        }

        private void DoubleSpinner_Loaded(object sender, EventArgs e)
        {
            txtValue.TextChanged += txtValue_TextChanged;
        }

        private void DoubleSpinner_InternalValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            double value = Math.Round(Value, Decimals);

            if (value < MinValue)
            {
                value = MinValue;
            }
            if (value > MaxValue)
            {
                value = MaxValue;
            }

            if (value != Value)
            {
                Value = value;
                return;
            }

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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(DoubleSpinner),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            ColorScheme cs = (e.NewValue as ColorScheme)!;
#else
            ColorScheme cs = e.NewValue as ColorScheme;
#endif

            if (d is DoubleSpinner s)
            {
                s.ColorSchemeChanged?.Invoke(d, e);
                s.ApplyColorScheme(cs);
            }
        }

        /// <summary>
        /// Get or set the color scheme used for this spinner. For easier color scheme management, bind this to the window or larger control you're using.
        /// </summary>
        [Category("Common")]
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

            visBorder.BorderBrush = cs.BorderColor.ToBrush();
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
        //    HighlightBrush = cs.SecondHighlightColor.ToBrush();
        //    DisabledBrush = cs.LightDisabledColor.ToBrush();
        //    BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
        //    Foreground = cs.ForegroundColor.ToBrush();
        //}
        #endregion

        #region Brushes

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ButtonBackgroundProperty = DependencyProperty.Register(
            "ButtonBackground", typeof(Brush), typeof(DoubleSpinner),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.White)));

        public static readonly DependencyProperty DisabledBrushProperty = DependencyProperty.Register(
            "DisabledBrush", typeof(Brush), typeof(DoubleSpinner),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(DoubleSpinner),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(DoubleSpinner),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty ClickBrushProperty = DependencyProperty.Register(
            "ClickBrush", typeof(Brush), typeof(DoubleSpinner),
            new PropertyMetadata(new SolidColorBrush(Colors.Gainsboro)));

        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            "BorderDisabledBrush", typeof(Brush), typeof(DoubleSpinner),
            new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(DoubleSpinner),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnValueChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        ///<summary>
        /// Get or set the value of the spinner.
        ///</summary>
        [Category("Common")]
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        //private double prev_value;

        // comes from here?
        // https://stackoverflow.com/questions/30318135/wpf-textbox-and-binding-to-double-not-able-to-type-on-it

        //public double Value
        //{
        //    get { return (double)GetValue(ValueProperty); }
        //    set
        //    {
        //        if (prev_value != value)
        //        {
        //            if (!advanceTimer.Enabled)
        //            {
        //                if (value < MinValue) value = MinValue;
        //                if (value > MaxValue) value = MaxValue;

        //                if (value == MinValue)
        //                {
        //                    btnDown.IsEnabled = false;
        //                    btnDown.Background = DisabledBrush;
        //                }
        //                else
        //                {
        //                    btnDown.IsEnabled = true;
        //                    btnDown.Background = ButtonBackground;
        //                }

        //                if (value == MaxValue)
        //                {
        //                    btnUp.IsEnabled = false;
        //                    btnUp.Background = DisabledBrush;
        //                }
        //                else
        //                {
        //                    btnUp.IsEnabled = true;
        //                    btnUp.Background = ButtonBackground;
        //                }
        //            }

        //            SetValue(ValueProperty, value);
        //            prev_value = value;
        //            ValueChanged?.Invoke(this, EventArgs.Empty);
        //        }

        //        //txtValue.Text = Math.Round(Value, Decimals).ToString();
        //    }
        //}

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //int value = (d as IntegerSpinner).Value;
            if (d is DoubleSpinner s)
            {
                s.InternalValueChanged?.Invoke(s, e);
            }
        }

        #endregion

        #region StepProperty

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
            "Step", typeof(double), typeof(DoubleSpinner), new PropertyMetadata(0.1d));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        ///<summary>
        /// Get or set how much to change the value by when you press the up or down button.
        ///</summary>
        [Category("Common")]
        public double Step
        {
            get => (double)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        #endregion

        #region DecimalsProperty

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty DecimalsProperty = DependencyProperty.Register(
            "Decimals", typeof(int), typeof(DoubleSpinner), new PropertyMetadata(2));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        ///<summary>
        /// Get or set how many decimal places to display. Values entered with a more precise decimal point will be rounded.
        ///</summary>
        [Category("Common")]
        public int Decimals
        {
            get => (int)GetValue(DecimalsProperty);
            set => SetValue(DecimalsProperty, value);
        }

        #endregion

        #region MinValueProperty

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue", typeof(double), typeof(DoubleSpinner), new PropertyMetadata(double.MinValue));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        ///<summary>
        /// Get or set the minimum value allowed for this spinner (inclusive).
        ///</summary>
        [Category("Common")]
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set
            {
                if (value > MaxValue) MaxValue = value;
                SetValue(MinValueProperty, value);
            }
        }

        #endregion

        #region MaxValueProperty

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue", typeof(double), typeof(DoubleSpinner), new PropertyMetadata(double.MaxValue));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        ///<summary>
        /// Get or set the maximum value allowed for this spinner (inclusive).
        ///</summary>
        [Category("Common")]
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set
            {
                if (value < MinValue) value = MinValue;
                SetValue(MaxValueProperty, value);
            }
        }

        #endregion

        #region RepeatDelayProperty

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty RepeatDelayProperty = DependencyProperty.Register(
            "RepeatDelay", typeof(double), typeof(DoubleSpinner),
            new PropertyMetadata(300d, new PropertyChangedCallback(OnInternalRepeatDelayChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member


        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalRepeatDelayChanged;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly RoutedEvent RepeatDelayChangedEvent = EventManager.RegisterRoutedEvent(
            "RepeatDelayChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DoubleSpinner));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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
            if (d is DoubleSpinner s)
            {
                s.InternalRepeatDelayChanged?.Invoke(s, e);
            }
        }

        private void DoubleSpinner_InternalRepeatDelayChanged(object sender, DependencyPropertyChangedEventArgs e)
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", typeof(CornerRadius), typeof(DoubleSpinner),
            new PropertyMetadata(new CornerRadius(0), new PropertyChangedCallback(OnInternalCornerRadiusChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member


        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalCornerRadiusChanged;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly RoutedEvent CornerRadiusChangedEvent = EventManager.RegisterRoutedEvent(
            "CornerRadiusChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DoubleSpinner));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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
            if (d is DoubleSpinner s)
            {
                s.InternalCornerRadiusChanged?.Invoke(s, e);
            }
        }

        private void DoubleSpinner_InternalCornerRadiusChanged(object sender, DependencyPropertyChangedEventArgs e)
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty AcceptExpressionsProperty = DependencyProperty.Register(
            "AcceptExpressions", typeof(bool), typeof(DoubleSpinner),
            new PropertyMetadata(true));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set if the spinner should evaluate arithmetic expressions (such as "2+5") to accept as a value.
        /// </summary>
        /// <remarks>
        /// See the <see cref="ArithmeticParser"/> class for more info about how expressions are parsed.
        /// </remarks>
        [Category("Common")]
        public bool AcceptExpressions
        {
            get => (bool)GetValue(AcceptExpressionsProperty);
            set => SetValue(AcceptExpressionsProperty, value);
        }

        #endregion

        #region ShowArrowsProperty

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ShowArrowsProperty = DependencyProperty.Register(
            "ShowArrows", typeof(bool), typeof(DoubleSpinner),
            new PropertyMetadata(true, new PropertyChangedCallback(OnInternalShowArrowsChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalShowArrowsChanged;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly RoutedEvent ShowArrowsChangedEvent = EventManager.RegisterRoutedEvent(
            "ShowArrowsChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DoubleSpinner));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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
            if (d is DoubleSpinner s)
            {
                s.InternalShowArrowsChanged?.Invoke(s, e);
            }
        }

        private void DoubleSpinner_InternalShowArrowsChanged(object sender, DependencyPropertyChangedEventArgs e)
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

        #region MinimumDigitCount

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty MinimumDigitCountProperty = DependencyProperty.Register(
            "MinimumDigitCount", typeof(int), typeof(DoubleSpinner),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Get or set the minimum number of digits to display in the spinner. A value of 0 or lower will display only the minimum needed for the number being displayed.
        /// </summary>
        /// <remarks>
        /// This modifies the number of digits being rendered via <see cref="double.ToString(string)"/> with the G format string.
        /// Given the limitations of format strings in relation to both integers and doubles, the results of this property may be different between the DoubleSpinner and IntegerSpinner.
        /// Setting this value will not actually round the stored value, only change what is displayed; instead, you can use the <see cref="Decimals"/> property for actual rounding.
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
        /// Validate the value and update the UI if neede.
        /// </summary>
        private void ValidateValue()
        {
            if (MinValue > MaxValue) MinValue = MaxValue;
            if (MaxValue < MinValue) MaxValue = MinValue;
            if (Value < MinValue) Value = MinValue;
            if (Value > MaxValue) Value = MaxValue;

            Value = Math.Round(Value, Decimals);

            UpdateUI();
            ValueValidated?.Invoke(this, EventArgs.Empty);
        }

        void UpdateUI()
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

            string digitDisplay = "G";
            if (MinimumDigitCount > 0) { digitDisplay = new string('0', MinimumDigitCount) + "." + new string('#', Decimals + 1); }
            string sVal = Value.ToString(digitDisplay);

            if (txtValue.Text != sVal)
            {
                if (_updateBox) txtValue.Text = sVal;
            }
        }

        private void txtValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtValue == null)
            {
                return;
            }

            _updateBox = false;
            if (double.TryParse(txtValue.Text, out _))
            {
                Value = Math.Round(double.Parse(txtValue.Text), Decimals);
            }
            else if (AcceptExpressions && ArithmeticParser.IsValidString(txtValue.Text))
            {
                try
                {
                    Value = Math.Round(ArithmeticParser.Evaluate(txtValue.Text), Decimals, MidpointRounding.AwayFromZero);
                }
                catch (FormatException)
                {

                }
                catch (ArgumentOutOfRangeException)
                {

                }
            }
            else
            {
                // this is not valid
                //currently do nothing
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

        private void baseSpinner_Loaded(object sender, RoutedEventArgs e)
        {
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
            }
            catch (TaskCanceledException)
            {
                advanceTimer.Stop();
            }
        }

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
