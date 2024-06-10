using SolidShineUi.Ribbon.Utils;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows;

namespace SolidShineUi.Ribbon
{
    /// <summary>
    /// A <see cref="DoubleSpinner"/> to display in a <see cref="RibbonGroup"/>.
    /// </summary>
    public class RibbonDoubleSpinner : RibbonSpinnerBase
    {
        private DoubleSpinner baseSpinner;

        /// <summary>
        /// Create a RibbonDoubleSpinner.
        /// </summary>
        public RibbonDoubleSpinner()
        {
            baseSpinner = new DoubleSpinner();
            Content = baseSpinner;
            BaseSpinner = baseSpinner;

            SetupBinding();
        }

        void SetupBinding()
        {
            // SpinnerBase
            SetBinding(RepeatDelayProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(RepeatDelay)) });
            SetBinding(IntervalProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(Interval)) });
            SetBinding(CornerRadiusProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(CornerRadius)) });
            SetBinding(AcceptExpressionsProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(AcceptExpressions)) });
            SetBinding(ShowArrowsProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(ShowArrows)) });
            SetBinding(MinimumDigitCountProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(MinimumDigitCount)) });

            SetBinding(ButtonBackgroundProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(ButtonBackground)) });
            SetBinding(DisabledBrushProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(DisabledBrush)) });
            SetBinding(BorderBrushProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(BorderBrush)) });
            SetBinding(HighlightBrushProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(HighlightBrush)) });
            SetBinding(ClickBrushProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(ClickBrush)) });
            SetBinding(BorderDisabledBrushProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(BorderDisabledBrush)) });

            // DoubleSpinner
            SetBinding(ValueProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(Value)) });
            SetBinding(MinValueProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(MinValue)) });
            SetBinding(MaxValueProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(MaxValue)) });
            SetBinding(StepProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(Step)) });

            // other
            SetBinding(SpinnerWidthProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(Width)) });

            // set up event handling
            baseSpinner.ValueChanged += (s, e) => RaiseValueChanged(this, e);
            baseSpinner.MaxValueChanged += (s, e) => RaiseMaxValueChanged(this, e);
            baseSpinner.MinValueChanged += (s, e) => RaiseMinValueChanged(this, e);
            baseSpinner.ValueValidated += (s, e) => RaiseValueValidated(this);
        }

        #region Properties

        #region ValueProperty

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(RibbonDoubleSpinner),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <inheritdoc/>
        [Category("Common")]
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        #endregion

        #region StepProperty

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
            "Step", typeof(double), typeof(RibbonDoubleSpinner), new PropertyMetadata(1.0d));

        /// <inheritdoc/>
        [Category("Common")]
        public double Step
        {
            get => (double)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        #endregion

        #region MinValueProperty

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue", typeof(double), typeof(RibbonDoubleSpinner),
            new PropertyMetadata(double.MinValue));

        ///<inheritdoc/>
        [Category("Common")]
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
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
            "MaxValue", typeof(double), typeof(RibbonDoubleSpinner),
            new PropertyMetadata(double.MaxValue));

        ///<inheritdoc/>
        [Category("Common")]
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set
            {
                if (value < MinValue) { value = MinValue; }

                SetValue(MaxValueProperty, value);
            }
        }

        #endregion

        #region DecimalsProperty

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty DecimalsProperty = DependencyProperty.Register(
            "Decimals", typeof(int), typeof(RibbonDoubleSpinner),
            new FrameworkPropertyMetadata(2));

        ///<summary>
        /// Get or set how many decimal places to display. Values entered with a more precise decimal value will be rounded.
        ///</summary>
        ///<remarks>
        /// This must be a value between 0 (which means round up to an integer number) and 15, inclusive. The default value is 2.
        ///</remarks>
        [Category("Common")]
        public int Decimals
        {
            get => (int)GetValue(DecimalsProperty);
            set => SetValue(DecimalsProperty, value);
        }

        #endregion

        #endregion

        /// <summary>
        /// Get or set the width of the spinner with in this control.
        /// </summary>
        public double SpinnerWidth { get => (double)GetValue(SpinnerWidthProperty); set => SetValue(SpinnerWidthProperty, value); }

        /// <summary>The backing dependency property for <see cref="SpinnerWidth"/>. See the related property for details.</summary>
        public static DependencyProperty SpinnerWidthProperty
            = DependencyProperty.Register(nameof(SpinnerWidth), typeof(double), typeof(RibbonDoubleSpinner),
            new FrameworkPropertyMetadata(140.0));

        /// <summary>
        /// Get the base spinner object that is contained in this control.
        /// </summary>
        public DoubleSpinner BaseSpinner { get => (DoubleSpinner)GetValue(BaseSpinnerProperty); private set => SetValue(BaseSpinnerPropertyKey, value); }

        private static readonly DependencyPropertyKey BaseSpinnerPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(BaseSpinner), typeof(DoubleSpinner), typeof(RibbonDoubleSpinner), new FrameworkPropertyMetadata());

        /// <summary>The backing dependency property for <see cref="BaseSpinner"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BaseSpinnerProperty = BaseSpinnerPropertyKey.DependencyProperty;

    }
}
