﻿using SolidShineUi.Utils;
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
    /// An updated version of the <see cref="LongSpinner"/>, built as a templated control.
    /// </summary>
    public class NewLongSpinner : NewSpinnerBase
    {
        static NewLongSpinner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NewLongSpinner), new FrameworkPropertyMetadata(typeof(NewLongSpinner)));
        }

        /// <summary>
        /// Create a NewLongSpinner.
        /// </summary>
        public NewLongSpinner()
        {
            // set up ValidateValue to run whenever these properties are updated (Value, MinValue, MaxValue)
            AddPropertyChangedTrigger(ValueProperty, typeof(NewLongSpinner));
            AddPropertyChangedTrigger(MinValueProperty, typeof(NewLongSpinner));
            AddPropertyChangedTrigger(MaxValueProperty, typeof(NewLongSpinner));

            CommandBindings.Add(new CommandBinding(StepUp, (o, e) => DoStepUp(), (o, e) => e.CanExecute = !IsAtMaxValue));
            CommandBindings.Add(new CommandBinding(StepDown, (o, e) => DoStepDown(), (o, e) => e.CanExecute = !IsAtMinValue));
        }

        #region Properties

        #region ValueProperty

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(long), typeof(NewLongSpinner),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        /// <summary>
        /// Get or set the value of the spinner.
        /// </summary>
        [Category("Common")]
        public long Value
        {
            get => (long)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NewLongSpinner s)
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
            "Step", typeof(long), typeof(NewLongSpinner), new PropertyMetadata(1));

        ///<summary>
        /// Get or set how much to change the value by when you press the up or down button.
        ///</summary>
        [Category("Common")]
        public long Step
        {
            get => (long)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        #endregion

        #region MinValueProperty

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue", typeof(long), typeof(NewLongSpinner),
            new PropertyMetadata(long.MinValue, (d, e) => d.PerformAs<NewLongSpinner>(i => i.OnMinValueChanged(e))));

        ///<summary>
        /// Get or set the minimum value allowed for this spinner (inclusive).
        ///</summary>
        [Category("Common")]
        public long MinValue
        {
            get { return (long)GetValue(MinValueProperty); }
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
            "MaxValue", typeof(long), typeof(NewLongSpinner),
            new PropertyMetadata(long.MaxValue, (d, e) => d.PerformAs<NewLongSpinner>(s => s.OnMaxValueChanged(e))));

        ///<summary>
        /// Get or set the maximum value allowed for this spinner (inclusive).
        ///</summary>
        [Category("Common")]
        public long MaxValue
        {
            get { return (long)GetValue(MaxValueProperty); }
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

        #region DisplayAsHex

        /// <summary>
        /// The backing dependency property object for <see cref="DisplayAsHex"/>. Please see the related property for details.
        /// </summary>
        public static readonly DependencyProperty DisplayAsHexProperty = DependencyProperty.Register(
            "DisplayAsHex", typeof(bool), typeof(NewLongSpinner),
            new PropertyMetadata(false, new PropertyChangedCallback((d, e) => d.PerformAs<NewLongSpinner>((s) => s.OnDisplayAsHexChanged(e)))));

        /// <summary>
        /// The backing routed event object for <see cref="DisplayAsHexChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent DisplayAsHexChangedEvent = EventManager.RegisterRoutedEvent(
            "DisplayAsHexChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(NewLongSpinner));

        /// <summary>
        /// Raised when the DisplayAsHex property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> DisplayAsHexChanged
        {
            add { AddHandler(DisplayAsHexChangedEvent, value); }
            remove { RemoveHandler(DisplayAsHexChangedEvent, value); }
        }

        private void OnDisplayAsHexChanged(DependencyPropertyChangedEventArgs e)
        {
            RoutedPropertyChangedEventArgs<bool> re = new RoutedPropertyChangedEventArgs<bool>((bool)e.OldValue, (bool)e.NewValue, DisplayAsHexChangedEvent);
            re.Source = this;
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

        /// <summary>
        /// Validate <see cref="Value"/> make sure it's between <see cref="MinValue"/> and <see cref="MaxValue"/>.
        /// </summary>
        protected override void ValidateValue()
        {
            long val = Value;
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
        private void txtValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            // raised when a new value is typed into the textbox
            // Value is updated as the text is being typed in (if it can be parsed), and full validation occurs once the text box loses focus or the user presses the Enter key

            if (txtValue == null)
            {
                return;
            }

            _updateBox = false;
            if (DisplayAsHex)
            {
                if (long.TryParse(txtValue.Text, System.Globalization.NumberStyles.HexNumber, null, out long newVal))
                {
                    Value = newVal;
                }
            }
            else
            {
                if (long.TryParse(txtValue.Text, System.Globalization.NumberStyles.Integer, null, out long newVal))
                {
                    Value = newVal;
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
