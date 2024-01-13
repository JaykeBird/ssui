using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SolidShineUi
{
    /// <summary>
    /// An updated version of <see cref="IntegerSpinner"/>. The functionality is present, but the UI has not yet been built.
    /// </summary>
    public class NewIntegerSpinner : NewSpinnerBase
    {

        /// <summary>
        /// Create an NewIntegerSpinner.
        /// </summary>
        public NewIntegerSpinner()
        {
            //InitializeComponent();
            //Loaded += NewIntegerSpinner_Loaded;

            // set up ValidateValue to run whenever these properties are updated (Value, MinValue, MaxValue)
            AddPropertyChangedTrigger(ValueProperty, typeof(NewIntegerSpinner));
            AddPropertyChangedTrigger(MinValueProperty, typeof(NewIntegerSpinner));
            AddPropertyChangedTrigger(MaxValueProperty, typeof(NewIntegerSpinner));
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
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(NewIntegerSpinner),
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
            if (d is NewIntegerSpinner s)
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
                //divider.BorderBrush = cs.BorderColor.ToBrush();
                //pathUp.Fill = cs.BorderColor.ToBrush();
                //pathDown.Fill = cs.BorderColor.ToBrush();
            }
            else
            {
                ButtonBackground = cs.SecondaryColor.ToBrush();
                //divider.BorderBrush = cs.SecondHighlightColor.ToBrush();
                //pathUp.Fill = cs.ForegroundColor.ToBrush();
                //pathDown.Fill = cs.ForegroundColor.ToBrush();
            }

            if (IsEnabled)
            {
                //brdr.BorderBrush = BorderBrush;
                //visBorder.BorderBrush = BorderBrush;
                //btnUp.Background = ButtonBackground;
                //btnDown.Background = ButtonBackground;
            }
            else
            {
                //brdr.BorderBrush = BorderDisabledBrush;
                //visBorder.BorderBrush = BorderDisabledBrush;
                //btnUp.Background = DisabledBrush;
                //btnDown.Background = DisabledBrush;
            }

            _updateBox = false;
            UpdateUI();
            _updateBox = true;

            ClickBrush = cs.ThirdHighlightColor.ToBrush();
            HighlightBrush = cs.HighlightColor.ToBrush();
            Foreground = cs.ForegroundColor.ToBrush();
        }

        #endregion

        #region Properties

        #region ValueProperty

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(int), typeof(NewIntegerSpinner),
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
            if (d is NewIntegerSpinner s)
            {
                s.UpdateValue(e);
            }
        }

        #endregion

        #region StepProperty

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
            "Step", typeof(int), typeof(NewIntegerSpinner), new PropertyMetadata(1));

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
            "MinValue", typeof(int), typeof(NewIntegerSpinner),
            new PropertyMetadata(int.MinValue, (d, e) => d.PerformAs<NewIntegerSpinner>(i => i.OnMinValueChanged(e))));

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

        private void OnMinValueChanged(DependencyPropertyChangedEventArgs e)
        {
            ValidateMinMax();
            RaiseMinValueChanged(this, e);
        }

        #endregion

        #region MaxValueProperty

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue", typeof(int), typeof(NewIntegerSpinner),
            new PropertyMetadata(int.MaxValue, (d, e) => d.PerformAs<NewIntegerSpinner>(s => s.OnMaxValueChanged(e))));

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

        private void OnMaxValueChanged(DependencyPropertyChangedEventArgs e)
        {
            ValidateMinMax();
            RaiseMinValueChanged(this, e);
        }

        #endregion

        #region ShowArrows/CornerRadius

        // properties defined in SpinnerBase

        /// <summary>
        /// Update this control's UI to reflect the change in <see cref="SpinnerBase.CornerRadius"/>.
        /// </summary>
        protected override void OnCornerRadiusChanged()
        {
            //brdrVisualEffect.CornerRadius = new CornerRadius(CornerRadius.TopLeft + 0.5, CornerRadius.TopRight + 0.5, CornerRadius.BottomRight + 0.5, CornerRadius.BottomLeft + 0.5);
            //visBorder.CornerRadius = CornerRadius;

            base.OnCornerRadiusChanged();
        }

        /// <summary>
        /// Update this control's UI to reflect the change in <see cref="SpinnerBase.ShowArrows"/>.
        /// </summary>
        protected override void OnShowArrowsChanged()
        {
            base.OnShowArrowsChanged();
        }

        #endregion

        #region DisplayAsHex

        /// <summary>
        /// The backing dependency property object for <see cref="DisplayAsHex"/>. Please see the related property for details.
        /// </summary>
        public static readonly DependencyProperty DisplayAsHexProperty = DependencyProperty.Register(
            "DisplayAsHex", typeof(bool), typeof(NewIntegerSpinner),
            new PropertyMetadata(false, new PropertyChangedCallback((d, e) => d.PerformAs<NewIntegerSpinner>((s) => s.OnDisplayAsHexChanged()))));

        /// <summary>
        /// The backing routed event object for <see cref="DisplayAsHexChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent DisplayAsHexChangedEvent = EventManager.RegisterRoutedEvent(
            "DisplayAsHexChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NewIntegerSpinner));

        /// <summary>
        /// Raised when the DisplayAsHex property is changed.
        /// </summary>
        public event RoutedEventHandler DisplayAsHexChanged
        {
            add { AddHandler(DisplayAsHexChangedEvent, value); }
            remove { RemoveHandler(DisplayAsHexChangedEvent, value); }
        }

        private void OnDisplayAsHexChanged()
        {
            RoutedEventArgs re = new RoutedEventArgs(DisplayAsHexChangedEvent);
            RaiseEvent(re);

            UpdateUI();
        }

        /// <summary>
        /// Get or set whether to show the value as a hexadecimal or decimal value. Note that while this is set to <c>true</c>, <c>AcceptExpressions</c> is ignored.
        /// </summary>
        /// <remarks>
        /// Certain situations, particularly involving computer representations of data or memory, may benefit more with displaying numbers as hexadecimals rather than decimals.
        /// With hexadecimals, the letters A-F are allowed along with 0-9, and the number "15" in decimal turns into "F" in hexadecimal. Please view online resources like
        /// Wikipedia for more details.
        /// <para/>
        /// <see cref="ArithmeticParser"/> is currently not built to correctly handle hexadecimal values, and so math expressions cannot be entered, and will instead be treated
        /// as an invalid value.
        /// </remarks>
        [Category("Common")]
        public bool DisplayAsHex
        {
            get => (bool)GetValue(DisplayAsHexProperty);
            set => SetValue(DisplayAsHexProperty, value);
        }

        #endregion

        #endregion

        #region Template IO
        
        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            LoadTemplateItems();
        }

        bool itemsLoaded = false;

#if NETCOREAPP
        TextBox? txtValue = null;
        Border? vb = null;
#else
        TextBox txtValue = null;
        Border vb = null;
#endif

        void LoadTemplateItems()
        {
            if (!itemsLoaded)
            {
                txtValue = (TextBox)GetTemplateChild("PART_Text");
                vb = (Border)GetTemplateChild("PART_Border");

                if (txtValue != null)
                {
                    txtValue.TextChanged += txtValue_TextChanged;
                    txtValue.LostFocus += txtValue_LostFocus;
                    txtValue.KeyDown += txtValue_KeyDown;
                    txtValue.KeyUp += txtValue_KeyUp;

                    if (vb != null)
                    {
                        itemsLoaded = true;
                    }
                }
            }
        }
        #endregion

        #region Base Functions

        /// <summary>
        /// Validate <see cref="Value"/> make sure it's between <see cref="MinValue"/> and <see cref="MaxValue"/>.
        /// </summary>
        protected override void ValidateValue()
        {
            int val = Value;
            if (val < MinValue) val = MinValue;
            if (val > MaxValue) val = MaxValue;
            if (val != Value) Value = val;

            base.ValidateValue();
        }

        /// <summary>
        /// Validate <see cref="MinValue"/> and <see cref="MaxValue"/>, to make sure they're not impossibly out of bounds of each other.
        /// </summary>
        protected override void ValidateMinMax()
        {
            if (MinValue > MaxValue) MinValue = MaxValue;
            if (MaxValue < MinValue) MaxValue = MinValue;
            base.ValidateMinMax();
        }

        /// <inheritdoc/>
        protected override void DoStepDown()
        {
            if (Value >= MinValue) Value -= Step;
            else Value = MinValue;
            base.DoStepDown();
        }

        /// <inheritdoc/>
        protected override void DoStepUp()
        {
            if (Value <= MaxValue) Value += Step;
            else Value = MaxValue;
            base.DoStepUp();
        }

        /// <inheritdoc/>
        protected override void UpdateValue(DependencyPropertyChangedEventArgs e)
        {
            //int value = Value;

            if (!advanceTimer.Enabled)
            {
                ValidateValue();

                IsAtMinValue = Value == MinValue;
                IsAtMaxValue = Value == MaxValue;
            }
            base.UpdateValue(e);
        }

        #endregion

        #region TextBox
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
            TextBoxKeyDown(e);
        }

        private void txtValue_KeyUp(object sender, KeyEventArgs e)
        {
            TextBoxKeyUp(e);
        }

        #endregion
    }
}
