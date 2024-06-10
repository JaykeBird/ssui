using SolidShineUi.Ribbon.Utils;
using System.Windows.Data;
using System.Windows;
using System.Collections.ObjectModel;

namespace SolidShineUi.Ribbon
{
    /// <summary>
    /// A <see cref="StringSpinner"/> to display in a <see cref="RibbonGroup"/>.
    /// </summary>
    public class RibbonStringSpinner : RibbonSpinnerBase
    {
        private StringSpinner baseSpinner;

        /// <summary>
        /// Create a RibbonStringSpinner.
        /// </summary>
        public RibbonStringSpinner()
        {
            baseSpinner = new StringSpinner();
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

        /// <summary>
        /// Get the string value that is currently selected in this control.
        /// </summary>
        /// <remarks>
        /// Use <see cref="SelectedIndex"/> to select a different string programmatically, like so: <c>mySpinner.SelectedIndex = mySpinner.Items.IndexOf("my string");</c>
        /// </remarks>
        public string Value
        {
            get => baseSpinner.Value;
        }

        /// <summary>
        /// Get the list of string values to select from in this StringSpinner control. Add items to this list via <c>Items.Add("string")</c>.
        /// </summary>
        public ObservableCollection<string> Items
        {
            get => baseSpinner.Items;
        }

        /// <summary>
        /// Get the highest index value that is allowed to be set via <see cref="SelectedIndex"/> in this control. This will be equal to <c>Items.Count - 1</c>.
        /// </summary>
        public int MaxValue
        {
            get => baseSpinner.MaxValue;
        }

        /// <summary>
        /// Get or set the index of the selected string in this control. This index must be between 0 and <see cref="MaxValue"/>, inclusive.
        /// </summary>
        public int SelectedIndex
        {
            get => baseSpinner.SelectedIndex;
            set => baseSpinner.SelectedIndex = value;
        }

        #endregion

        /// <summary>
        /// Get or set the width of the spinner with in this control.
        /// </summary>
        public double SpinnerWidth { get => (double)GetValue(SpinnerWidthProperty); set => SetValue(SpinnerWidthProperty, value); }

        /// <summary>The backing dependency property for <see cref="SpinnerWidth"/>. See the related property for details.</summary>
        public static DependencyProperty SpinnerWidthProperty
            = DependencyProperty.Register(nameof(SpinnerWidth), typeof(double), typeof(RibbonStringSpinner),
            new FrameworkPropertyMetadata(140));

        /// <summary>
        /// Get the base spinner object that is contained in this control.
        /// </summary>
        public StringSpinner BaseSpinner { get => (StringSpinner)GetValue(BaseSpinnerProperty); private set => SetValue(BaseSpinnerPropertyKey, value); }

        private static readonly DependencyPropertyKey BaseSpinnerPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(BaseSpinner), typeof(StringSpinner), typeof(RibbonStringSpinner), new FrameworkPropertyMetadata());

        /// <summary>The backing dependency property for <see cref="BaseSpinner"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BaseSpinnerProperty = BaseSpinnerPropertyKey.DependencyProperty;

    }
}
