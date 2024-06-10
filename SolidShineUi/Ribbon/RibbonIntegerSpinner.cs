using SolidShineUi.Ribbon.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SolidShineUi.Ribbon
{

    /// <summary>
    /// A <see cref="IntegerSpinner"/> to display in a <see cref="RibbonGroup"/>.
    /// </summary>
    public class RibbonIntegerSpinner : RibbonSpinnerBase
    {
        private IntegerSpinner baseSpinner;

        /// <summary>
        /// Create a RibbonIntegerSpinner.
        /// </summary>
        public RibbonIntegerSpinner()
        {
            baseSpinner = new IntegerSpinner();
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

            // IntegerSpinner
            SetBinding(ValueProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(Value)) });
            SetBinding(MinValueProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(MinValue)) });
            SetBinding(MaxValueProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(MaxValue)) });
            SetBinding(StepProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(Step)) });
            SetBinding(DisplayAsHexProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(DisplayAsHex)) });

            // other
            SetBinding(SpinnerWidthProperty,
                new Binding() { Source = baseSpinner, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(Width)) });

            // set up event handling
            baseSpinner.ValueChanged += (s, e) => RaiseValueChanged(this, e);
            baseSpinner.MaxValueChanged += (s, e) => RaiseMaxValueChanged(this, e);
            baseSpinner.MinValueChanged += (s, e) => RaiseMinValueChanged(this, e);
            baseSpinner.ValueValidated += (s, e) => RaiseValueValidated(this);
            baseSpinner.DisplayAsHexChanged += (s, e) => OnDisplayAsHexChanged(e);
        }

        #region Properties

        #region ValueProperty

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(int), typeof(RibbonIntegerSpinner),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <inheritdoc/>
        [Category("Common")]
        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        #endregion

        #region StepProperty

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
            "Step", typeof(int), typeof(RibbonIntegerSpinner), new PropertyMetadata(1));

        /// <inheritdoc/>
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
            "MinValue", typeof(int), typeof(RibbonIntegerSpinner),
            new PropertyMetadata(int.MinValue));

        ///<inheritdoc/>
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
            "MaxValue", typeof(int), typeof(RibbonIntegerSpinner),
            new PropertyMetadata(int.MaxValue));

        ///<inheritdoc/>
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

        #region DisplayAsHex

        /// <summary>
        /// The backing dependency property object for <see cref="DisplayAsHex"/>. Please see the related property for details.
        /// </summary>
        public static readonly DependencyProperty DisplayAsHexProperty = DependencyProperty.Register(
            "DisplayAsHex", typeof(bool), typeof(RibbonIntegerSpinner),
            new PropertyMetadata(false));

        /// <summary>
        /// The backing routed event object for <see cref="DisplayAsHexChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent DisplayAsHexChangedEvent = EventManager.RegisterRoutedEvent(
            "DisplayAsHexChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(RibbonIntegerSpinner));

        /// <summary>
        /// Raised when the DisplayAsHex property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> DisplayAsHexChanged
        {
            add { AddHandler(DisplayAsHexChangedEvent, value); }
            remove { RemoveHandler(DisplayAsHexChangedEvent, value); }
        }

        private void OnDisplayAsHexChanged(RoutedPropertyChangedEventArgs<bool> e)
        {
            RoutedPropertyChangedEventArgs<bool> re = new RoutedPropertyChangedEventArgs<bool>(e.OldValue, e.NewValue, DisplayAsHexChangedEvent);
            re.Source = this;
            RaiseEvent(re);
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

        /// <summary>
        /// Get or set the width of the spinner with in this control.
        /// </summary>
        public double SpinnerWidth { get => (double)GetValue(SpinnerWidthProperty); set => SetValue(SpinnerWidthProperty, value); }

        /// <summary>The backing dependency property for <see cref="SpinnerWidth"/>. See the related property for details.</summary>
        public static DependencyProperty SpinnerWidthProperty
            = DependencyProperty.Register(nameof(SpinnerWidth), typeof(double), typeof(RibbonIntegerSpinner),
            new FrameworkPropertyMetadata(140.0));

        /// <summary>
        /// Get the base spinner object that is contained in this control.
        /// </summary>
        public IntegerSpinner BaseSpinner { get => (IntegerSpinner)GetValue(BaseSpinnerProperty); private set => SetValue(BaseSpinnerPropertyKey, value); }

        private static readonly DependencyPropertyKey BaseSpinnerPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(BaseSpinner), typeof(IntegerSpinner), typeof(RibbonIntegerSpinner), new FrameworkPropertyMetadata());

        /// <summary>The backing dependency property for <see cref="BaseSpinner"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BaseSpinnerProperty = BaseSpinnerPropertyKey.DependencyProperty;

    }
}
