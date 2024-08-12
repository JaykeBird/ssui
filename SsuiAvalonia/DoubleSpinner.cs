using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using SolidShineUi.Utils;
using System;

namespace SolidShineUi
{
    /// <summary>
    /// A control for selecting a number, via typing in a number, an arithmetic expression, or using the up and down buttons. Only integer values are allowed.
    /// </summary>
    public class DoubleSpinner : NumericSpinnerBase<double>
    {

        /// <summary>
        /// Create an DoubleSpinner.
        /// </summary>
        public DoubleSpinner()
        {
            AddPropertyChangedTrigger(ValueProperty);
            AddPropertyChangedTrigger(MinValueProperty);
            AddPropertyChangedTrigger(MaxValueProperty);
            //AddPropertyChangedTrigger(StepProperty);
        }

        #region Property Change Listener

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            switch (change.Property.Name)
            {
                case nameof(Value):
                    UpdateValue(change);
                    break;
                case nameof(MaxValue):
                    ValidateMinMax();
                    RaiseMaxValueChanged(this, change);
                    break;
                case nameof(MinValue):
                    ValidateMinMax();
                    RaiseMinValueChanged(this, change);
                    break;
                case nameof(Decimals):
                    if (Decimals < 0) Decimals = 0;
                    if (Decimals > 15) Decimals = 15;
                    break;
            }
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override double Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        /// <summary>The backing styled property for <see cref="Value"/>. See the related property for details.</summary>
        public static readonly StyledProperty<double> ValueProperty
            = AvaloniaProperty.Register<DoubleSpinner, double>(nameof(Value), 0.0d);


        /// <inheritdoc/>
        public override double Step { get => GetValue(StepProperty); set => SetValue(StepProperty, value); }

        /// <summary>The backing styled property for <see cref="Step"/>. See the related property for details.</summary>
        public static readonly StyledProperty<double> StepProperty
            = AvaloniaProperty.Register<DoubleSpinner, double>(nameof(Step), 1.0d);


        /// <inheritdoc/>
        public override double MinValue { get => GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }

        /// <summary>The backing styled property for <see cref="MinValue"/>. See the related property for details.</summary>
        public static readonly StyledProperty<double> MinValueProperty
            = AvaloniaProperty.Register<DoubleSpinner, double>(nameof(MinValue), double.MinValue);


        /// <inheritdoc/>
        public override double MaxValue { get => GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }

        /// <summary>The backing styled property for <see cref="MaxValue"/>. See the related property for details.</summary>
        public static readonly StyledProperty<double> MaxValueProperty
            = AvaloniaProperty.Register<DoubleSpinner, double>(nameof(MaxValue), double.MaxValue);

        ///<summary>
        /// Get or set how many decimal places to display. Values entered with a more precise decimal value will be rounded.
        ///</summary>
        ///<remarks>
        /// This must be a value between 0 (which means round up to an integer number) and 15, inclusive. The default value is 15.
        /// Trailing zeros are not displayed if a value doesn't have the full decimal precision; instead, this property is more
        /// meant to provide a built-in rounding feature as needed.
        ///</remarks>
        public byte Decimals { get => GetValue(DecimalsProperty); set => SetValue(DecimalsProperty, value); }

        /// <summary>The backing styled property for <see cref="Decimals"/>. See the related property for details.</summary>
        public static readonly StyledProperty<byte> DecimalsProperty
            = AvaloniaProperty.Register<DoubleSpinner, byte>(nameof(Decimals), 15);


        #endregion

        #region Template IO

        TextBox? txtValue;
        FlatRepeatButton? btnStepUp;
        FlatRepeatButton? btnStepDown;

        bool itemsLoaded = false;

        /// <inheritdoc/>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            LoadTemplateItems(e);
        }


        void LoadTemplateItems(TemplateAppliedEventArgs e)
        {
            if (!itemsLoaded)
            {
                txtValue = (TextBox?)e.NameScope.Find("PART_Text");

                if (txtValue != null)
                {
                    txtValue.TextChanged += txtValue_TextChanged;
                    txtValue.LostFocus += txtValue_LostFocus;
                    txtValue.KeyDown += txtValue_KeyDown;
                    txtValue.KeyUp += txtValue_KeyUp;

                    itemsLoaded = true;
                }

                // Avalonia doesn't include a command system out of the box (usually I think most people rely upon Reactive), so for now, I'll need to call these events this way
                btnStepUp = (FlatRepeatButton?)e.NameScope.Find("PART_btnStepUp");
                btnStepDown = (FlatRepeatButton?)e.NameScope.Find("PART_btnStepDown");

                if (btnStepUp != null) { btnStepUp.Execute += (s, e) => DoStepUp(); }
                if (btnStepDown != null) { btnStepDown.Execute += (s, e) => DoStepDown(); }
            }
        }

        #endregion

        #region Base Functions

        ///// <summary>
        ///// Validate <see cref="Value"/> make sure it's between <see cref="MinValue"/> and <see cref="MaxValue"/>.
        ///// </summary>
        //protected override void ValidateValue()
        //{
        //    int val = Value;
        //    if (val < MinValue) val = MinValue;
        //    if (val > MaxValue) val = MaxValue;
        //    if (val != Value) Value = val;

        //    base.ValidateValue();
        //}

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
            base.UpdateUI();

            string digitDisplay = "G";
            if (MinimumDigitCount > 0) { digitDisplay = new string('0', MinimumDigitCount) + "." + new string('#', Decimals + 1); }
            string sVal = Value.ToString(digitDisplay);

            if (txtValue == null) return; // this is not good, as it means that the template didn't apply, or the applied template's text box won't get the updated value

            if (txtValue.Text != sVal)
            {
                if (_updateBox) txtValue.Text = sVal;
            }
        }

        #endregion

        #region TextBox
        private void txtValue_TextChanged(object? sender, TextChangedEventArgs e)
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
            else if (AcceptExpressions && ArithmeticParser.IsValidString(txtValue.Text ?? ""))
            {
                try
                {
                    Value = Math.Round(ArithmeticParser.Evaluate(txtValue.Text ?? ""), Decimals, MidpointRounding.AwayFromZero);
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

        private void txtValue_LostFocus(object? sender, RoutedEventArgs e)
        {
            ValidateValue();
        }

        private void txtValue_KeyDown(object? sender, KeyEventArgs e)
        {
            TextBoxKeyDown(e);
        }

        private void txtValue_KeyUp(object? sender, KeyEventArgs e)
        {
            TextBoxKeyUp(e);
        }

        #endregion
    }
}
