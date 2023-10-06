using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Timers;
using SolidShineUi.Utils;

// initial base is https://github.com/Stopbyte/WPF-Numeric-Spinner-NumericUpDown , but has since been heavily modified

namespace SolidShineUi
{
    /// <summary>
    /// A control for selecting a number, via typing in a number, an arithmetic expression, or using the up and down buttons. Only integer values are allowed.
    /// </summary>
    public partial class IntegerSpinner : SpinnerBase
    {
       
        /// <summary>
        /// Create an IntegerSpinner.
        /// </summary>
        public IntegerSpinner()
        {
            InitializeComponent();
            Loaded += IntegerSpinner_Loaded;

            // set up ValidateValue to run whenever these properties are updated (Value, MinValue, MaxValue)
            AddPropertyChangedTrigger(ValueProperty, typeof(IntegerSpinner));
            AddPropertyChangedTrigger(MinValueProperty, typeof(IntegerSpinner));
            AddPropertyChangedTrigger(MaxValueProperty, typeof(IntegerSpinner));
            //DependencyPropertyDescriptor.FromProperty(RepeatDelayProperty, typeof(IntegerSpinner)).AddValueChanged(this, PropertyChanged);

            InternalValueChanged += IntegerSpinner_InternalValueChanged;
            InternalDisplayAsHexChanged += IntegerSpinner_InternalDisplayAsHexChanged;

            PropertyChanged += (x, y) => ValidateValue();

            keyDownTimer.Elapsed += KeyDownTimer_Elapsed;
            advanceTimer.Elapsed += AdvanceTimer_Elapsed;
        }

        private void IntegerSpinner_Loaded(object sender, EventArgs e)
        {
            txtValue.TextChanged += txtValue_TextChanged;
        }

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
            RaiseValueChanged(this, e);
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
        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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

        #region ValueProperty

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalValueChanged;

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
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
            if (d is IntegerSpinner s)
            {
                s.InternalValueChanged?.Invoke(s, e);
            }
        }

        #endregion

        #region StepProperty

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
            "Step", typeof(int), typeof(IntegerSpinner), new PropertyMetadata(1));

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
        /// A dependency property object backing a related property. See the related property for more details.
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
        /// A dependency property object backing a related property. See the related property for more details.
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

        #region ShowArrows/CornerRadius

        // properties defined in SpinnerBase

        protected override void OnCornerRadiusChanged()
        {
            brdrVisualEffect.CornerRadius = new CornerRadius(CornerRadius.TopLeft + 0.5, CornerRadius.TopRight + 0.5, CornerRadius.BottomRight + 0.5, CornerRadius.BottomLeft + 0.5);
            visBorder.CornerRadius = CornerRadius;

            base.OnCornerRadiusChanged();
        }

        protected override void OnShowArrowsChanged()
        {
            colArrows.Width = ShowArrows ? new GridLength(20) : new GridLength(0);
            base.OnShowArrowsChanged();
        }

        #endregion

        #region DisplayAsHex

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty DisplayAsHexProperty = DependencyProperty.Register(
            "DisplayAsHex", typeof(bool), typeof(IntegerSpinner),
            new PropertyMetadata(false, new PropertyChangedCallback(OnInternalDisplayAsHexChanged)));

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalDisplayAsHexChanged;

        public static readonly RoutedEvent DisplayAsHexChangedEvent = EventManager.RegisterRoutedEvent(
            "DisplayAsHexChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(IntegerSpinner));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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

        /// <summary>
        /// Validate the value and update the UI if needed.
        /// </summary>
        private void ValidateValue()
        {
            if (MinValue > MaxValue) MinValue = MaxValue;
            if (MaxValue < MinValue) MaxValue = MinValue;
            if (Value < MinValue) Value = MinValue;
            if (Value > MaxValue) Value = MaxValue;

            UpdateUI();
            RaiseValueValidated(this);
        }

        protected override void UpdateUI()
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
            if (MinimumDigitCount > 0) { digitDisplay = MinimumDigitCount.ToString("G"); }
            string sVal = Value.ToString((DisplayAsHex ? "X" : "D") + digitDisplay);

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
                if (int.TryParse(txtValue.Text, System.Globalization.NumberStyles.HexNumber, null, out _))
                {
                    Value = int.Parse(txtValue.Text, System.Globalization.NumberStyles.HexNumber);
                }
            }
            else
            {
                if (int.TryParse(txtValue.Text, System.Globalization.NumberStyles.Integer, null, out _))
                {
                    Value = int.Parse(txtValue.Text, System.Globalization.NumberStyles.Integer);
                }
                else if (AcceptExpressions && ArithmeticParser.IsValidString(txtValue.Text))
                {
                    try
                    {
                        Value = (int)Math.Round(ArithmeticParser.Evaluate(txtValue.Text), MidpointRounding.AwayFromZero);
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
