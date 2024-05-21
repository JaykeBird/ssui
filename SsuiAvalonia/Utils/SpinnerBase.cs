using System;
using System.ComponentModel;
using System.Timers;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia;
using Avalonia.Input;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// The base class for Solid Shine UI's spinner controls (such as <see cref="IntegerSpinner"/> and <see cref="DoubleSpinner"/>).
    /// </summary>
    /// <remarks>
    /// Spinner controls for storing/editing numeric data values should inherit from <see cref="NumericSpinnerBase{T}"/>.
    /// </remarks>
    public class SpinnerBase : Control
    {

        /// <summary>
        /// Create a SpinnerBase.
        /// </summary>
        public SpinnerBase()
        {
            Loaded += SpinnerBase_Loaded;

            keyDownTimer.AutoReset = false;
            advanceTimer.AutoReset = true;

            keyDownTimer.Elapsed += (s, e) => advanceTimer.Start();
            advanceTimer.Elapsed += AdvanceTimer_Elapsed;

            PropertyChanged += (x, y) => ValidateValue();
        }

        private void SpinnerBase_Loaded(object? sender, RoutedEventArgs e)
        {
            // doesn't work in constructor, apparently
            _raiseChangedEvent = false;
            ValidateValue();
            _raiseChangedEvent = true;
        }

        private void AdvanceTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    if (advanceStepUp)
                    {
                        DoStepUp();
                    }
                    else
                    {
                        DoStepDown();
                    }

                    UpdateUI();
                }, DispatcherPriority.Input);
            }
            catch (TaskCanceledException)
            {
                advanceTimer.Stop();
            }
        }

        #region Commands

        //// these will be used for the transition from standard UserControls to full templated controls

        ///// <summary>A WPF command that when executed, will increase a spinner's value by its <c>Step</c> amount</summary>
        //public static Command StepUp { get; } = new RoutedCommand("StepUp", typeof(SpinnerBase));

        ///// <summary>A WPF command that when executed, will decrease a spinner's value by its <c>Step</c> amount</summary>
        //public static RoutedCommand StepDown { get; } = new RoutedCommand("StepDown", typeof(SpinnerBase));

        #endregion

        #region IsAtMaxValue / IsAtMinValue

        private bool _isAtMaxValue = false;
        private bool _isAtMinValue = false;

        /// <summary>
        /// Get if this spinner's <c>Value</c> is currently equal to its <c>MinValue</c>. If true, the value cannot be decreased any more.
        /// </summary>
        public bool IsAtMinValue { get => _isAtMinValue; protected private set => SetAndRaise(IsAtMinValueProperty, ref _isAtMinValue, value); }

        /// <summary>The backing direct property for <see cref="IsAtMinValue"/>. See the related property for details.</summary>
        public static readonly DirectProperty<SpinnerBase, bool> IsAtMinValueProperty
            = AvaloniaProperty.RegisterDirect<SpinnerBase, bool>(nameof(IsAtMinValue), (s) => s.IsAtMinValue, unsetValue: false);

        /// <summary>
        /// Get if this spinner's <c>Value</c> is currently equal to its <c>MaxValue</c>. If true, the value cannot be increased any more.
        /// </summary>
        public bool IsAtMaxValue { get => _isAtMaxValue; protected private set => SetAndRaise(IsAtMaxValueProperty, ref _isAtMaxValue, value); }

        /// <summary>The backing direct property for <see cref="IsAtMaxValue"/>. See the related property for details.</summary>
        public static readonly DirectProperty<SpinnerBase, bool> IsAtMaxValueProperty
            = AvaloniaProperty.RegisterDirect<SpinnerBase, bool>(nameof(IsAtMaxValue), (s) => s.IsAtMaxValue, unsetValue: false);

        #endregion

        #region Internal Values

        /// <summary>
        /// determine the text box's text should be changed when <c>Value</c> is updated
        /// </summary>
        protected bool _updateBox = true;
        /// <summary>
        /// determine if the <see cref="ValueChanged"/> event should be raised
        /// </summary>
        protected bool _raiseChangedEvent = true;

        /// <summary>
        /// the timer to set how long to wait before responding to and acting upon a key press (for changing the value)
        /// </summary>
        protected Timer keyDownTimer = new Timer(300);

        /// <summary>
        /// the timer to set the interval at which the value is changed while the user is holding down an arrow button
        /// </summary>
        protected Timer advanceTimer = new Timer(50);

        /// <summary>
        /// determine whether to increase or decrease the <c>Value</c> while the control is stepping; false for decrease, true for increase
        /// </summary>
        protected bool advanceStepUp = false;

        #endregion

        #region Events / Event-related methods

        /// <summary>
        /// Raised when the Value, MinValue, or MaxValue properties are changed. Used internally to trigger revalidating the value.
        /// </summary>
        public event EventHandler<AvaloniaPropertyChangedEventArgs>? SpinnerPropertyChanged;

        /// <summary>
        /// Raised when the Value property is changed.
        /// </summary>
        public event EventHandler<AvaloniaPropertyChangedEventArgs>? ValueChanged;

        /// <summary>
        /// Raised when the Value property is validated, and changed to a valid value if needed.
        /// </summary>
        public event EventHandler? ValueValidated;

        /// <summary>
        /// Raise the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Extra event data to provide to any event listeners.</param>
        protected void RaiseValueChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (_raiseChangedEvent) ValueChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// Raise the <see cref="ValueValidated"/> event.
        /// </summary>
        /// <param name="sender">The soruce of the event.</param>
        protected void RaiseValueValidated(object sender)
        {
            ValueValidated?.Invoke(sender, EventArgs.Empty);
        }

        /// <summary>
        /// Add a dependency property to trigger the <see cref="SpinnerPropertyChanged"/> event when its value changes.
        /// </summary>
        /// <param name="property">The dependency property to monitor changes to.</param>
        protected void AddPropertyChangedTrigger(AvaloniaProperty property)
        {
            property.Changed.AddClassHandler<SpinnerBase>((tt, e) => SpinnerPropertyChanged?.Invoke(this, e));
            //DependencyPropertyDescriptor.FromProperty(property, targetType).AddValueChanged(this, PropertyChanged);
        }
        #endregion

        #region Brushes

        /// <summary>The backing styled property for <see cref="ButtonBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> ButtonBackgroundProperty
            = AvaloniaProperty.Register<SpinnerBase, IBrush?>(nameof(ButtonBackground), new SolidColorBrush(ColorsHelper.White));

        /// <summary>The backing styled property for <see cref="DisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> DisabledBrushProperty
            = AvaloniaProperty.Register<SpinnerBase, IBrush?>(nameof(DisabledBrush), new SolidColorBrush(Colors.Gray));

        ///// <summary>The backing styled property for <see cref="BorderBrush"/>. See the related property for details.</summary>
        //public static readonly StyledProperty<IBrush?> BorderBrushProperty
        //    = AvaloniaProperty.Register<SpinnerBase, IBrush?>(nameof(BorderBrush), new SolidColorBrush(Colors.Black));

        /// <summary>The backing styled property for <see cref="BorderDisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BorderDisabledBrushProperty
            = AvaloniaProperty.Register<SpinnerBase, IBrush?>(nameof(BorderDisabledBrush), new SolidColorBrush(Colors.DarkGray));

        /// <summary>The backing styled property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightBrushProperty
            = AvaloniaProperty.Register<SpinnerBase, IBrush?>(nameof(HighlightBrush), new SolidColorBrush(Colors.LightGray));

        /// <summary>The backing styled property for <see cref="ClickBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> ClickBrushProperty
            = AvaloniaProperty.Register<SpinnerBase, IBrush?>(nameof(ClickBrush), new SolidColorBrush(Colors.Gainsboro));



        /// <summary>
        /// Get or set the brush used for the background of the buttons of the spinner.
        /// </summary>
        [Category("Brushes")]
        public IBrush? ButtonBackground
        {
            get
            {
                return GetValue(ButtonBackgroundProperty);
            }
            set
            {
                SetValue(ButtonBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used for the background of the buttons when the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public IBrush? DisabledBrush
        {
            get
            {
                return GetValue(DisabledBrushProperty);
            }
            set
            {
                SetValue(DisabledBrushProperty, value);
            }
        }

        ///// <summary>
        ///// Get or set the brush of the border around the control.
        ///// </summary>
        //[Category("Brushes")]
        //public new IBrush? BorderBrush
        //{
        //    get
        //    {
        //        return GetValue(BorderBrushProperty);
        //    }
        //    set
        //    {
        //        SetValue(BorderBrushProperty, value);
        //    }
        //}

        /// <summary>
        /// Get or set the brush used when a button is highlighted (i.e. has a mouse over it or keyboard focus).
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightBrush
        {
            get
            {
                return GetValue(HighlightBrushProperty);
            }
            set
            {
                SetValue(HighlightBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used when a button is being clicked.
        /// </summary>
        [Category("Brushes")]
        public IBrush? ClickBrush
        {
            get
            {
                return GetValue(ClickBrushProperty);
            }
            set
            {
                SetValue(ClickBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used for the border around the control, while the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public IBrush? BorderDisabledBrush
        {
            get
            {
                return GetValue(BorderDisabledBrushProperty);
            }
            set
            {
                SetValue(BorderDisabledBrushProperty, value);
            }
        }

        #endregion

        #region Properties

        #region RepeatDelayProperty

        /// <summary>
        /// Get or set the delay period before starting the repeatedly stepping up or down while the button is held, in milliseconds. Default is 300 milliseconds.
        /// </summary>
        [Category("Common")]
        public double RepeatDelay { get => GetValue(RepeatDelayProperty); set => SetValue(RepeatDelayProperty, value); }

        /// <summary>The backing styled property for <see cref="RepeatDelay"/>. See the related property for details.</summary>
        public static readonly StyledProperty<double> RepeatDelayProperty
            = AvaloniaProperty.Register<SpinnerBase, double>(nameof(RepeatDelay), 300.0);


        /// <summary>
        /// The routed event object for the <see cref="RepeatDelayChanged"/> event. See the related event for details.
        /// </summary>
        public static readonly RoutedEvent RepeatDelayChangedEvent = RoutedEvent.Register<SpinnerBase, RoutedEventArgs>(
            "RepeatDelayChanged", RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when the RepeatDelay property is changed.
        /// </summary>
        public event EventHandler<RoutedEventArgs> RepeatDelayChanged
        {
            add { AddHandler(RepeatDelayChangedEvent, value); }
            remove { RemoveHandler(RepeatDelayChangedEvent, value); }
        }

        /// <summary>
        /// Update internal values based upon a change in the <see cref="RepeatDelay"/> property.
        /// </summary>
        protected virtual void OnRepeatDelayChanged()
        {
            keyDownTimer.Interval = RepeatDelay;
            RoutedEventArgs re = new RoutedEventArgs(RepeatDelayChangedEvent);
            RaiseEvent(re);
        }

        #endregion

        #region CornerRadiusProperty

        /// <summary>
        /// The routed event object for the <see cref="CornerRadiusChanged"/> event. See the related event for details.
        /// </summary>
        public static readonly RoutedEvent CornerRadiusChangedEvent = RoutedEvent.Register<SpinnerBase, RoutedEventArgs>(
            "CornerRadiusChanged", RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when the CornerRadius property is changed.
        /// </summary>
        public event EventHandler<RoutedEventArgs> CornerRadiusChanged
        {
            add { AddHandler(CornerRadiusChangedEvent, value); }
            remove { RemoveHandler(CornerRadiusChangedEvent, value); }
        }

        /// <summary>
        /// Update internal values based upon a change in the <see cref="CornerRadius"/> property.
        /// </summary>
        protected virtual void OnCornerRadiusChanged()
        {
            RoutedEventArgs re = new RoutedEventArgs(CornerRadiusChangedEvent);
            RaiseEvent(re);
        }

        /// <summary>
        /// Get or set the corner radius to use around the corners of this control. Setting the corner radius to a value other than 0 displays rounded corners.
        /// </summary>
        [Category("Appearance")]
        public CornerRadius CornerRadius { get => GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }

        /// <summary>The backing styled property for <see cref="CornerRadius"/>. See the related property for details.</summary>
        public static readonly StyledProperty<CornerRadius> CornerRadiusProperty
            = AvaloniaProperty.Register<SpinnerBase, CornerRadius>(nameof(CornerRadius), new CornerRadius(0));

        #endregion

        #region AcceptExpressionsProperty

        /// <summary>
        /// Get or set if the spinner should evaluate arithmetic expressions (such as "2+5") to accept as a value.
        /// </summary>
        /// <remarks>
        /// See the <see cref="ArithmeticParser"/> class for more info about how expressions are parsed.
        /// </remarks>
        [Category("Common")]
        public bool AcceptExpressions { get => GetValue(AcceptExpressionsProperty); set => SetValue(AcceptExpressionsProperty, value); }

        /// <summary>The backing styled property for <see cref="AcceptExpressions"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> AcceptExpressionsProperty
            = AvaloniaProperty.Register<SpinnerBase, bool>(nameof(AcceptExpressions), true);


        #endregion

        #region ShowArrowsProperty

        /// <summary>
        /// The routed event object for the <see cref="ShowArrowsChanged"/> event. See the related event for details.
        /// </summary>
        public static readonly RoutedEvent ShowArrowsChangedEvent = RoutedEvent.Register<SpinnerBase, RoutedEventArgs>("ShowArrowsChanged", RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when the ShowArrows property is changed.
        /// </summary>
        public event EventHandler<RoutedEventArgs> ShowArrowsChanged
        {
            add { AddHandler(ShowArrowsChangedEvent, value); }
            remove { RemoveHandler(ShowArrowsChangedEvent, value); }
        }

        /// <summary>
        /// Update internal values based upon a change in the <see cref="ShowArrows"/> property.
        /// </summary>
        protected virtual void OnShowArrowsChanged()
        {
            RoutedEventArgs re = new RoutedEventArgs(ShowArrowsChangedEvent);
            RaiseEvent(re);
        }

        /// <summary>
        /// Get or set whether the up and down arrow buttons are shown.
        /// </summary>
        [Category("Common")]
        public bool ShowArrows { get => GetValue(ShowArrowsProperty); set => SetValue(ShowArrowsProperty, value); }

        /// <summary>The backing styled property for <see cref="ShowArrows"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> ShowArrowsProperty
            = AvaloniaProperty.Register<SpinnerBase, bool>(nameof(ShowArrows), true);

        #endregion

        #region MinimumDigitCount

        /// <summary>
        /// Get or set the minimum number of integral digits to display in the spinner (for spinners that display numbers). A value of 0 or lower will revert the display to the standard number display format.
        /// </summary>
        /// <remarks>
        /// This modifies the number of digits being rendered via <see cref="double.ToString(string)"/>, <see cref="int.ToString(string)"/>, and other similar methods.
        /// Setting this to <c>4</c> and then setting the Value to <c>16</c> will render the text <c>0016</c> in the display. The stored value isn't affected.
        /// You should generally avoid setting this to numbers larger than 99, as larger numbers are only supported in newer versions of .NET.
        /// <para/>
        /// For <see cref="DoubleSpinner"/>, this only modifies the integral (integer) part of the number; use the <see cref="DoubleSpinner.Decimals"/> property 
        /// for modifying how many numbers to display after the decimal point.
        /// </remarks>
        [Category("Common")]
        public int MinimumDigitCount { get => GetValue(MinimumDigitCountProperty); set => SetValue(MinimumDigitCountProperty, value); }

        /// <summary>The backing styled property for <see cref="MinimumDigitCount"/>. See the related property for details.</summary>
        public static readonly StyledProperty<int> MinimumDigitCountProperty
            = AvaloniaProperty.Register<SpinnerBase, int>(nameof(MinimumDigitCount), 0, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);


        #endregion

        #endregion

        #region Base Functions

        /// <summary>
        /// Update visual appearance, based upon internal values and the current state. This should be overridden in controls that inherit this class.
        /// </summary>
        protected virtual void UpdateUI()
        {

        }

        /// <summary>
        /// Validate the value and update the UI if needed. This should be overriden in controls that inherit this class, to perform actual validation on the value.
        /// </summary>
        protected virtual void ValidateValue()
        {
            UpdateUI();
            RaiseValueValidated(this);
        }

        /// <summary>
        /// Validate MinValue and MaxValue, to make sure they're not impossibly out of bounds of each other. This should be overriden in controls that inherit this class, to 
        /// perform actual validation on the value. This base function calls <see cref="ValidateValue"/>.
        /// </summary>
        protected virtual void ValidateMinMax()
        {
            ValidateValue();
        }

        /// <summary>
        /// Handle the process of updating value properties. Should be overridden in classes that inherit this, in order to perform validation or other functions.
        /// </summary>
        /// <param name="e">Event args from the related <see cref="ValueChanged"/> event.</param>
        protected virtual void UpdateValue(AvaloniaPropertyChangedEventArgs e)
        {
            UpdateUI();
            RaiseValueChanged(this, e);
        }

        /// <summary>
        /// Increase the spinner's value by whatever the Step value is. Should be overridden in controls that inherit this class.
        /// </summary>
        protected virtual void DoStepUp()
        {

        }

        /// <summary>
        /// Decrease the spinner's value by whatever the Step value is. Should be overridden in controls that inherit this class.
        /// </summary>
        protected virtual void DoStepDown()
        {

        }

        #endregion

        #region TextBox/Arrow Functions

        /// <summary>
        /// Handle the functionality for pressing a key in the text box of a spinner control.
        /// </summary>
        /// <param name="e">The KeyEventArgs for the related event (from the child object).</param>
        protected void TextBoxKeyDown(KeyEventArgs e)
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

        /// <summary>
        /// Handle the functionality for releasing a key in the text box of a spinner control.
        /// </summary>
        protected void TextBoxKeyUp(KeyEventArgs e)
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
                    DoStepDown();
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
                    DoStepUp();
                }
            }
        }

        /// <summary>
        /// Perform the initial action for when a button is pressed down. This doesn't actually perform any changes to <c>Value</c> yet.
        /// Use <paramref name="stepUp"/> to determine if this is stepping up (<c>true</c>) or stepping down (<c>false</c>).
        /// </summary>
        /// <param name="stepUp">Determine if stepping up (<c>true</c>) or stepping down (<c>false</c>)</param>
        protected void BeginButtonPress(bool stepUp)
        {
            advanceStepUp = stepUp;
            keyDownTimer.Start();
        }

        /// <summary>
        /// Perform the cutoff action for when a button is no longer pressed down. More specifically, this has been used for when <c>MouseLeave</c> occurs,
        /// although in the future I may look into other situations for this to run.
        /// </summary>
        /// <param name="stepUp">Determine if stepping up (<c>true</c>) or stepping down (<c>false</c>)</param>
        protected void StopButtonPress(bool stepUp)
        {
            if (advanceStepUp == stepUp) advanceTimer.Stop();
            UpdateUI();
        }

        /// <summary>
        /// Perform the actions of stepping down when pressing the down button.
        /// </summary>
        protected void DownButtonPress()
        {
            if (advanceTimer.Enabled)
            {
                advanceTimer.Stop();
            }
            else
            {
                keyDownTimer.Stop();
                DoStepDown();
                UpdateUI();
            }
        }

        /// <summary>
        /// Perform the actions of stepping up when pressing the up button.
        /// </summary>
        protected void UpButtonPress()
        {
            if (advanceTimer.Enabled)
            {
                advanceTimer.Stop();
            }
            else
            {
                keyDownTimer.Stop();
                DoStepUp();
                UpdateUI();
            }
        }

        #endregion
    }

    /// <summary>
    /// A base class for Solid Shine UI's spinner controls that interact with numbers (such as <see cref="IntegerSpinner"/> and <see cref="DoubleSpinner"/>).
    /// </summary>
    /// <typeparam name="T">The data type supported by the spinner.</typeparam>
    /// <remarks>
    /// This provides some underlying logic (and enforces the existence of certain properties) for spinners that interact with numbers.
    /// Spinner controls that don't interact with numbers should instead just inherit from <see cref="SpinnerBase"/>.
    /// </remarks>
    public abstract class NumericSpinnerBase<T> : SpinnerBase where T : IEquatable<T>, IComparable<T>
    {
        /// <summary>
        /// Get or set the value of the spinner.
        /// </summary>
        public abstract T Value { get; set; }

        /// <summary>
        /// Get or set the maximum value allowed in this spinner (inclusive).
        /// By default, this corresponds to the max value allowed by the underlying type (<typeparamref name="T"/>).
        /// </summary>
        public abstract T MinValue { get; set; }

        /// <summary>
        /// Get or set the maximum value allowed in this spinner (inclusive).
        /// By default, this corresponds to the max value allowed by the underlying type (<typeparamref name="T"/>).
        /// </summary>
        public abstract T MaxValue { get; set; }

        /// <summary>
        /// Get or set how much to change the value by when you press the up or down button, or use the Up and Down arrow keys.
        /// </summary>
        public abstract T Step { get; set; }

        /// <summary>
        /// Validate <see cref="Value"/> make sure it's between <see cref="MinValue"/> and <see cref="MaxValue"/>.
        /// </summary>
        protected override void ValidateValue()
        {
            T val = Value;
            if (val.CompareTo(MinValue) < 0) val = MinValue;
            if (val.CompareTo(MaxValue) > 0) val = MaxValue;
            if (!val.Equals(Value)) Value = val;

            base.ValidateValue();
        }

        /// <summary>
        /// Validate <see cref="MinValue"/> and <see cref="MaxValue"/>, to make sure they're not impossibly out of bounds of each other.
        /// </summary>
        protected override void ValidateMinMax()
        {
            if (MinValue.CompareTo(MaxValue) > 0) MinValue = MaxValue;
            if (MaxValue.CompareTo(MinValue) < 0) MaxValue = MinValue;

            IsAtMinValue = Value.Equals(MinValue);
            IsAtMaxValue = Value.Equals(MaxValue);

            base.ValidateMinMax();
        }

        /// <inheritdoc/>
        protected override void UpdateValue(AvaloniaPropertyChangedEventArgs e)
        {
            //int value = Value;

            if (!advanceTimer.Enabled)
            {
                ValidateValue();

                IsAtMinValue = Value.Equals(MinValue);
                IsAtMaxValue = Value.Equals(MaxValue);
            }
            base.UpdateValue(e);
        }
    }
}
