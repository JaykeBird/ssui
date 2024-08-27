﻿using Avalonia;
using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace SolidShineUi
{

    /// <summary>
    /// A control that allows you to select or enter a time.
    /// </summary>
    public class TimeSpinner : NumericSpinnerBase<TimeSpan>
    {
        // As has been well discussed, for most of .NET's existence, there hasn't really been a great "time" struct or data type present
        // while DateTime has been the go-to for working with date and time values, it suffers from being designed around representing specifically date AND time,
        // rather than representing only a date or only a time - leading to programs needing to handle disregarding the portions of a DateTime it doesn't need
        // overall, DateTime is great, but it's a tool that does a lot of things when sometimes you only need something that does only some things
        // there's a few other time-related structs that are out there, but each tends to have their own minor issues or problems
        // one of these other structs is TimeSpan, which as you can tell by its name alone, seems built to indicate a period of time, rather than a single point
        // which as you can see, is the one that I opted to use here - for a couple of reasons
        //
        // firstly, I didn't want to have a full DateTime for this class when it wouldn't be managing dates at all
        // obviously, I can disregard or strip out the date values at various input points, which was my plan, but this would also be a lot of validation/checking
        // and with how large and complex of a structure DateTime can be, I worry there'll be instances where the date doesn't end up stripped and it leads to
        // more problems down the road... while DateTime would've been an obvious choice here, I was hesitant about it all the while
        // 
        // secondly (and adding on to the previous point), TimeSpan doesn't really have a date component - it does have a "Days" property, but that doesn't
        // translate to any specific date. instead, the "Days" property is more meant to represent a period of time longer than 24 hours
        //
        // thirdly, TimeSpan operates well with DateTime and, more importantly, with .NET 6's TimeOnly struct
        // TimeSpans can be added into a DateTime, and can be extracted from a DateTime by using its "TimeOfDay" property - not the most straightforward
        // conversions, granted, but given that the two are designed to solve different problems, this is really the best that can be done
        // TimeSpan can also be directly converted to and from a TimeOnly using the FromTimeSpan and ToTimeSpan methods
        //
        // fourthly, TimeSpan has been included in .NET Framework since almost the very beginning. While TimeOnly seems like the obvious choice to use for this
        // control, the problem is that it was added in .NET 6, meaning earlier versions of .NET and all of the .NET Framework do not have access to it
        // this is insurmountable for a library that is designed to work with both .NET and also the .NET Framework - so TimeOnly is pretty much out
        // fortunately, as mentioned in the previous point, there are methods for direct conversion between TimeSpan and TimeOnly
        // 
        // finally, I know that third-party libraries exist, such as NodaTime, which include their own time-only data types (or something equivalent to that)
        // but since having SolidShineUi with no extra dependencies is a major goal of mine, that rules out that option for me as well

        /// <summary>
        /// Create a new TimeSpinner.
        /// </summary>
        public TimeSpinner()
        {
            AddPropertyChangedTrigger(ValueProperty);
            AddPropertyChangedTrigger(MinValueProperty);
            AddPropertyChangedTrigger(MaxValueProperty);
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
            }
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override TimeSpan Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        /// <summary>The backing styled property for <see cref="Value"/>. See the related property for details.</summary>
        public static readonly StyledProperty<TimeSpan> ValueProperty
            = AvaloniaProperty.Register<TimeSpinner, TimeSpan>(nameof(Value), new TimeSpan(0, 0, 0));


        /// <inheritdoc/>
        public override TimeSpan Step { get => GetValue(StepProperty); set => SetValue(StepProperty, value); }

        /// <summary>The backing styled property for <see cref="Step"/>. See the related property for details.</summary>
        public static readonly StyledProperty<TimeSpan> StepProperty
            = AvaloniaProperty.Register<TimeSpinner, TimeSpan>(nameof(Step), new TimeSpan(0, 0, 15));


        /// <summary>
        /// Get or set the maximum value allowed in this spinner (inclusive).
        /// By default for the TimeSpinner, this is a <see cref="TimeSpan"/> set to <c>00:00:00</c>, but this can be lowered to negative values (see <see cref="TimeSpan.MinValue"/>).
        /// </summary>
        public override TimeSpan MinValue { get => GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }

        /// <summary>The backing styled property for <see cref="MinValue"/>. See the related property for details.</summary>
        public static readonly StyledProperty<TimeSpan> MinValueProperty
            = AvaloniaProperty.Register<TimeSpinner, TimeSpan>(nameof(MinValue), new TimeSpan(0, 0, 0));


        /// <summary>
        /// Get or set the maximum value allowed in this spinner (inclusive).
        /// By default for the TimeSpinner, this is a <see cref="TimeSpan"/> set to <c>23:59:59</c>, but this can be raised to higher values (see <see cref="TimeSpan.MaxValue"/>).
        /// </summary>
        public override TimeSpan MaxValue { get => GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }

        /// <summary>The backing styled property for <see cref="MaxValue"/>. See the related property for details.</summary>
        public static readonly StyledProperty<TimeSpan> MaxValueProperty
            = AvaloniaProperty.Register<TimeSpinner, TimeSpan>(nameof(MaxValue), new TimeSpan(23, 59, 59));

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


        /// <summary>
        /// Get or set how the <see cref="Value"/> is displayed in the text box. This can be used to customize how values are displayed.
        /// </summary>
        /// <remarks>
        /// By default, the <c>"g"</c> format string is used, which is culture sensitive. Use <c>"c"</c> for a constant culture-insensitive format.
        /// See <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings">this Microsoft documentation</a> page
        /// for information on available valid format strings, including standard format strings included in .NET and how to create a custom format string.
        /// <para/>
        /// For custom format strings, the TimeSpinner will add on a negative <c>"-"</c> sign at the beginning of the string as needed, if the value is below
        /// <see cref="TimeSpan.Zero"/>. Standard or custom strings do not support using a 12-hour clock with "AM" and "PM" values; if a 12-hour clock is desired,
        /// set <see cref="Use12HourClock"/> to true, which will prompt the TimeSpinner to add a culture-sensitive "AM" or "PM" at the end of the string.
        /// </remarks>
        public string DisplayFormatString { get => GetValue(DisplayFormatStringProperty); set => SetValue(DisplayFormatStringProperty, value); }

        /// <summary>The backing styled property for <see cref="DisplayFormatString"/>. See the related property for details.</summary>
        public static readonly StyledProperty<string> DisplayFormatStringProperty
            = AvaloniaProperty.Register<TimeSpinner, string>(nameof(DisplayFormatString), "g");

        
        /// <summary>
        /// Get or set if a 12-hour clock display should be used when displaying the <see cref="Value"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="CultureInfo.CurrentUICulture"/> is used to determine the "AM" and "PM" string values, which will be appended to the end of the display string,
        /// regardless of the value of <see cref="DisplayFormatString"/>. If the "AM" or "PM" values are blank (as they are by default in a culture that doesn't use 
        /// 12-hour clocks), than the default English values "AM" and "PM" are used regardless of culture or language. <para/>
        /// Customizing the "AM" or "PM" string values can be done throughout your application by making a clone of the current culture's
        /// <see cref="DateTimeFormatInfo"/>, changing the <c>AMDesignator</c> and <c>PMDesignator</c> properties, and then setting that as part of the current culture.
        /// </remarks>
        public bool Use12HourClock { get => GetValue(Use12HourClockProperty); set => SetValue(Use12HourClockProperty, value); }

        /// <summary>The backing styled property for <see cref="Use12HourClock"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> Use12HourClockProperty
            = AvaloniaProperty.Register<TimeSpinner, bool>(nameof(Use12HourClock), false);

        #endregion

        #region Value as Other Formats

        /// <summary>
        /// Set the value of this TimeSpinner by using the count of a number of ticks.
        /// </summary>
        /// <param name="ticks">The value to parse to set this TimeSpinner's value.</param>
        public void SetValueFromTicks(long ticks)
        {
            Value = new TimeSpan(ticks);
        }

#if NET6_0_OR_GREATER

        /// <summary>
        /// Get the value of this TimeSpinner, as a <see cref="TimeOnly"/> value.
        /// </summary>
        /// <returns>A <see cref="TimeOnly"/> that represents the current TimeSpinner value.</returns>
        /// <remarks>
        /// If the current Value is negative (less than <see cref="TimeSpan.Zero"/>), a <see cref="TimeOnly"/> set to <c>00:00:00</c> is returned.<br />
        /// If the current Value is greater than a day (<see cref="TimeSpan.TotalDays"/> is at least 1), the Days part is removed and the remaining values
        /// (hours, minutes, seconds, and milliseconds) is returned.
        /// <para/>
        /// This differs from the built-in converter <see cref="TimeOnly.FromTimeSpan(TimeSpan)"/> as that will throw exceptions in those cases instead.
        /// </remarks>
        public TimeOnly GetValueAsTimeOnly()
        {
            if (Value <= TimeSpan.Zero)
            {
                return new TimeOnly(0, 0, 0);
            }
            else
            {
                return new TimeOnly(Value.Hours, Value.Minutes, Value.Seconds, Value.Milliseconds);
            }
        }

        /// <summary>
        /// Set the value of this TimeSpinner by using the value of a <see cref="TimeOnly"/>.
        /// </summary>
        /// <param name="value">The value to parse to set this TimeSpinner's value.</param>
        public void SetValueFromTimeOnly(TimeOnly value)
        {
            Value = value.ToTimeSpan();
        }

#endif

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

        /// <inheritdoc/>
        protected override void ValidateValue()
        {
            base.ValidateValue();

            //TimeSpan oldVal = Value;
            ////Value = Math.Round(Value, Decimals);

            //if (oldVal != Value)
            //{
            //    // redo the underlying value updates again
            //    base.ValidateValue();
            //}
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
            // format the timespan to look correct
            string sVal = Value.ToString(DisplayFormatString);

            if (Use12HourClock) // if we're going to display the 12-hour clock values (AM and PM), then we'll need to manually add that on ourselves
            {
                if (Value.Hours > 12)
                {
                    TimeSpan newVal = Value - new TimeSpan(12, 0, 0);
                    sVal = newVal.ToString(DisplayFormatString) + " " + CultureInfo.CurrentUICulture.DateTimeFormat.PMDesignator;
                }
                else if (Value.Hours > 11)
                {
                    // check for between noon (12 PM) and 1 PM
                    sVal += " " + CultureInfo.CurrentUICulture.DateTimeFormat.PMDesignator;
                }
                else if (Value.Hours == 0)
                {
                    // check for between midnight (12 AM) and 1 AM
                    TimeSpan newVal = Value + new TimeSpan(12, 0, 0);
                    sVal = newVal.ToString(DisplayFormatString) + " " + CultureInfo.CurrentUICulture.DateTimeFormat.AMDesignator;
                }
                else
                {
                    sVal += " " + CultureInfo.CurrentUICulture.DateTimeFormat.AMDesignator;
                }
            }

            if (Value < TimeSpan.Zero && !sVal.StartsWith("-")) // used for custom format strings, as they don't support displaying the negative sign for negative values
            {
                sVal = "-" + sVal;
            }

            if (txtValue == null) return; // this is not good, as it means that the template didn't apply, or the applied template's text box won't get the updated value

            if (txtValue.Text != sVal)
            {
                if (_updateBox) txtValue.Text = sVal;
            }

            base.UpdateUI();
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
            string[] dtTimeFormats = { "t", "T" };

            if (int.TryParse(txtValue.Text, out int i))
            {
                // deconstruct a TimeSpan from the int value
                string text = txtValue.Text ?? "";

                bool negativeValue = false;

                if (i < 0)
                {
                    negativeValue = true;
                    i *= -1;
                    text = text.Replace("-", "");
                }
                TimeSpan valVal;

                if (text.Length <= 4)
                {
                    int hours = i / 100;
                    int minutes = Math.Min(i % 100, 59);
                    valVal = new TimeSpan(hours, minutes, 0);
                }
                else if (text.Length <= 6)
                {
                    // hhmmss - % 100 for seconds, % 10000 for minutes
                    int hours = Math.Min(i / 10000, 23);
                    int remaining = i % 10000;
                    int minutes = Math.Min(remaining / 100, 59);
                    int seconds = Math.Min(remaining % 100, 59);
                    valVal = new TimeSpan(hours, minutes, seconds);
                }
                else
                {
                    if (i >= 235959)
                    {
                        valVal = new TimeSpan(23, 59, 59);
                    }
                    else
                    {
                        int hours = Math.Min(i / 10000, 23);
                        int remaining = i % 10000;
                        int minutes = Math.Min(remaining / 100, 59);
                        int seconds = Math.Min(remaining % 100, 59);
                        valVal = new TimeSpan(hours, minutes, seconds);
                    }
                }

                if (negativeValue)
                {
                    // this should get it to its negative form
                    Value = valVal - valVal - valVal;
                }
                else
                {
                    Value = valVal;
                }
            }
            else if (TimeSpan.TryParse(txtValue.Text, out TimeSpan ts))
            {
                Value = ts;
            }
            else if (DateTime.TryParseExact(txtValue.Text, dtTimeFormats, null, DateTimeStyles.AllowWhiteSpaces, out DateTime dt))
            {
                Value = dt.TimeOfDay;
            }
            else
            {
                // in the future, we can probably parse in some other formats (i.e. using a decimal rather than a colon)
                // however, this would be more work to parse and get done correctly, 

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
