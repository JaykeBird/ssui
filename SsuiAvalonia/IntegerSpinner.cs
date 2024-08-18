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
    public class IntegerSpinner : NumericSpinnerBase<int>
    {
        
        /// <summary>
        /// Create an IntegerSpinner.
        /// </summary>
        public IntegerSpinner()
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
                case nameof(DisplayAsHex):
                    UpdateUI();
                    DisplayAsHexChanged?.Invoke(this, change);
                    break;
            }
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override int Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        /// <summary>The backing styled property for <see cref="Value"/>. See the related property for details.</summary>
        public static readonly StyledProperty<int> ValueProperty
            = AvaloniaProperty.Register<IntegerSpinner, int>(nameof(Value), 0);


        /// <inheritdoc/>
        public override int Step { get => GetValue(StepProperty); set => SetValue(StepProperty, value); }

        /// <summary>The backing styled property for <see cref="Step"/>. See the related property for details.</summary>
        public static readonly StyledProperty<int> StepProperty
            = AvaloniaProperty.Register<IntegerSpinner, int>(nameof(Step), 1);


        /// <inheritdoc/>
        public override int MinValue { get => GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }

        /// <summary>The backing styled property for <see cref="MinValue"/>. See the related property for details.</summary>
        public static readonly StyledProperty<int> MinValueProperty
            = AvaloniaProperty.Register<IntegerSpinner, int>(nameof(MinValue), int.MinValue);


        /// <inheritdoc/>
        public override int MaxValue { get => GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }

        /// <summary>The backing styled property for <see cref="MaxValue"/>. See the related property for details.</summary>
        public static readonly StyledProperty<int> MaxValueProperty
            = AvaloniaProperty.Register<IntegerSpinner, int>(nameof(MaxValue), int.MaxValue);

        #region DisplayAsHex

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
        public bool DisplayAsHex { get => GetValue(DisplayAsHexProperty); set => SetValue(DisplayAsHexProperty, value); }

        /// <summary>The backing styled property for <see cref="DisplayAsHex"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> DisplayAsHexProperty
            = AvaloniaProperty.Register<IntegerSpinner, bool>(nameof(DisplayAsHex), false);

        /// <summary>
        /// Raised when the DisplayAsHex property is changed.
        /// </summary>
        public event EventHandler<AvaloniaPropertyChangedEventArgs>? DisplayAsHexChanged;

        #endregion

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

            string digitDisplay = "";
            if (MinimumDigitCount > 0) { digitDisplay = MinimumDigitCount.ToString("G"); }
            string sVal = Value.ToString((DisplayAsHex ? "X" : "D") + digitDisplay);

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
            else if (txtValue.Text == null)
            {
                txtValue.Text = "";
                return;
            }

            _updateBox = false;
            if (DisplayAsHex)
            {
                if (int.TryParse(txtValue.Text, System.Globalization.NumberStyles.HexNumber, null, out int newVal)) // the inputted value is a valid int, so let's make it the new value
                {
                    Value = newVal;
                }
            }
            else
            {
                if (int.TryParse(txtValue.Text, System.Globalization.NumberStyles.Integer, null, out int newVal)) // the inputted value is a valid int, so let's make it the new value
                {
                    Value = newVal;
                    // Note: the property change system won't run again if the updated Value is actually the same as the current Value (i.e. the property didn't actually change)
                }
                else if (AcceptExpressions && ArithmeticParser.IsValidString(txtValue.Text)) // okay... not a number... maybe it's a math expression?
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
