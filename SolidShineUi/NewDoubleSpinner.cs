using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace SolidShineUi
{
    /// <summary>
    /// A control for selecting a number, via typing in a number, an arithmetic expression, or using the up and down buttons.
    /// </summary>
    public class DoubleSpinner : NumericSpinnerBase<double>
    {
        static DoubleSpinner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DoubleSpinner), new FrameworkPropertyMetadata(typeof(DoubleSpinner)));
        }

        /// <summary>
        /// Create a NewDoubleSpinner.
        /// </summary>
        public DoubleSpinner()
        {
            // set up ValidateValue to run whenever these properties are updated (Value, MinValue, MaxValue)
            AddPropertyChangedTrigger(ValueProperty, typeof(DoubleSpinner));
            AddPropertyChangedTrigger(MinValueProperty, typeof(DoubleSpinner));
            AddPropertyChangedTrigger(MaxValueProperty, typeof(DoubleSpinner));

            CommandBindings.Add(new CommandBinding(StepUp, (o, e) => DoStepUp(), (o, e) => e.CanExecute = !IsAtMaxValue));
            CommandBindings.Add(new CommandBinding(StepDown, (o, e) => DoStepDown(), (o, e) => e.CanExecute = !IsAtMinValue));
        }

        #region Properties

        #region ValueProperty

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(DoubleSpinner),
            new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        /// <inheritdoc/>
        [Category("Common")]
        public override double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DoubleSpinner s)
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
            "Step", typeof(double), typeof(DoubleSpinner), new PropertyMetadata(1.0d));

        /// <inheritdoc/>
        [Category("Common")]
        public override double Step
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
            "MinValue", typeof(double), typeof(DoubleSpinner),
            new PropertyMetadata(double.MinValue, (d, e) => d.PerformAs<DoubleSpinner>(i => i.OnMinValueChanged(e))));

        ///<inheritdoc/>
        [Category("Common")]
        public override double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
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
            "MaxValue", typeof(double), typeof(DoubleSpinner),
            new PropertyMetadata(double.MaxValue, (d, e) => d.PerformAs<DoubleSpinner>(s => s.OnMaxValueChanged(e))));

        ///<inheritdoc/>
        [Category("Common")]
        public override double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
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

        #region DecimalsProperty

        /// <summary>
        /// A dependency property object backing the <see cref="Decimals"/> property. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty DecimalsProperty = DependencyProperty.Register(
            "Decimals", typeof(int), typeof(DoubleSpinner), new PropertyMetadata(2));

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
#else
        TextBox txtValue = null;
#endif

        void LoadTemplateItems()
        {
            if (!itemsLoaded)
            {
                txtValue = (TextBox)GetTemplateChild("PART_Text");

                if (txtValue != null)
                {
                    txtValue.TextChanged += txtValue_TextChanged;
                    txtValue.LostFocus += txtValue_LostFocus;
                    txtValue.KeyDown += txtValue_KeyDown;
                    txtValue.KeyUp += txtValue_KeyUp;

                    itemsLoaded = true;
                }
            }
        }
        #endregion

        #region Base Functions

        /// <inheritdoc/>
        protected override void ValidateValue()
        {
            base.ValidateValue();

            if (Decimals > 15) Decimals = 15;
            if (Decimals < 0) Decimals = 0;

            double oldVal = Value;
            Value = Math.Round(Value, Decimals);

            if (oldVal != Value)
            {
                // redo the underlying value updates again
                base.ValidateValue();
            }
        }

        ///// <summary>
        ///// Validate <see cref="MinValue"/> and <see cref="MaxValue"/>, to make sure they're not impossibly out of bounds of each other.
        ///// </summary>
        //protected override void ValidateMinMax()
        //{
        //    if (MinValue > MaxValue) MinValue = MaxValue;
        //    if (MaxValue < MinValue) MaxValue = MinValue;
        //    base.ValidateMinMax();
        //}

        /// <inheritdoc/>
        protected override void DoStepDown()
        {
            if (Value >= MinValue) Value -= Step;
            else Value = MinValue;
        }

        /// <inheritdoc/>
        protected override void DoStepUp()
        {
            if (Value <= MaxValue) Value += Step;
            else Value = MaxValue;
        }

        /// <inheritdoc/>
        protected override void UpdateUI()
        {
            string digitDisplay = "G";
            if (MinimumDigitCount > 0) { digitDisplay = new string('0', MinimumDigitCount) + "." + new string('#', Decimals + 1); }
            string sVal = Value.ToString(digitDisplay);

            if (txtValue == null) return; // this is not good, as it means that the template didn't apply, or the applied template's text box won't get the updated value

            if (txtValue.Text != sVal)
            {
                if (_updateBox) txtValue.Text = sVal;
            }

            base.UpdateUI();
        }

        #endregion

        #region TextBox
        private void txtValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            // raised when a new value is typed into the textbox
            // Value is updated as the text is being typed in (if it can be parsed), and full validation occurs once the text box loses focus or the user presses the Enter key

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
            TextBoxKeyDown(e);
        }

        private void txtValue_KeyUp(object sender, KeyEventArgs e)
        {
            TextBoxKeyUp(e);
        }

        #endregion
    }
}
