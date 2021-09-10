using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

// Source: https://github.com/Stopbyte/WPF-Numeric-Spinner-NumericUpDown

namespace SolidShineUi
{
    /// <summary>
    /// Interaction logic for DoubleSpinner.xaml
    /// </summary>
    public partial class DoubleSpinner : UserControl
    {

        #region Internal Values

        bool _updateBox = true; // set if the text box's text should be changed when the value is changed
        bool _raiseChangedEvent = true; // set if the ValueChanged event should be raised

        #endregion

        public event EventHandler PropertyChanged;
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ValueChanged;
        public event EventHandler? ValueValidated;
#else
        public event DependencyPropertyChangedEventHandler ValueChanged;
        public event EventHandler ValueValidated;
#endif

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
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(DoubleSpinner),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

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

        [Category("Common")]
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

        protected event DependencyPropertyChangedEventHandler InternalValueChanged;

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(DoubleSpinner),
            new PropertyMetadata(0d, new PropertyChangedCallback(OnValueChanged)));

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

        public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
            "Step", typeof(double), typeof(DoubleSpinner), new PropertyMetadata(0.1d));

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

        public static readonly DependencyProperty DecimalsProperty = DependencyProperty.Register(
            "Decimals", typeof(int), typeof(DoubleSpinner), new PropertyMetadata(2));

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

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue", typeof(double), typeof(DoubleSpinner), new PropertyMetadata(double.MinValue));

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

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue", typeof(double), typeof(DoubleSpinner), new PropertyMetadata(double.MaxValue));

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

        public static readonly DependencyProperty RepeatDelayProperty = DependencyProperty.Register(
            "RepeatDelay", typeof(double), typeof(DoubleSpinner),
            new PropertyMetadata(300d, new PropertyChangedCallback(OnInternalRepeatDelayChanged)));

        protected event DependencyPropertyChangedEventHandler InternalRepeatDelayChanged;

        public static readonly RoutedEvent RepeatDelayChangedEvent = EventManager.RegisterRoutedEvent(
            "RepeatDelayChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DoubleSpinner));

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

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", typeof(CornerRadius), typeof(DoubleSpinner),
            new PropertyMetadata(new CornerRadius(0), new PropertyChangedCallback(OnInternalCornerRadiusChanged)));

        protected event DependencyPropertyChangedEventHandler InternalCornerRadiusChanged;

        public static readonly RoutedEvent CornerRadiusChangedEvent = EventManager.RegisterRoutedEvent(
            "CornerRadiusChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DoubleSpinner));

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

        [Category("Appearance")]
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #endregion

        #region AcceptExpressionsProperty

        public static readonly DependencyProperty AcceptExpressionsProperty = DependencyProperty.Register(
            "AcceptExpressions", typeof(bool), typeof(DoubleSpinner),
            new PropertyMetadata(true));

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

        public static readonly DependencyProperty ShowArrowsProperty = DependencyProperty.Register(
            "ShowArrows", typeof(bool), typeof(DoubleSpinner),
            new PropertyMetadata(true, new PropertyChangedCallback(OnInternalShowArrowsChanged)));

        protected event DependencyPropertyChangedEventHandler InternalShowArrowsChanged;

        public static readonly RoutedEvent ShowArrowsChangedEvent = EventManager.RegisterRoutedEvent(
            "ShowArrowsChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DoubleSpinner));

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

            if (txtValue.Text != Value.ToString())
            {
                if (_updateBox) txtValue.Text = Value.ToString();
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

        private void KeyDownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            advanceTimer.Start();
        }

        private void AdvanceTimer_Elapsed(object sender, ElapsedEventArgs e)
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
