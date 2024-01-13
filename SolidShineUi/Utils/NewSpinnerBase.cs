using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// The base class for Solid Shine UI's spinner controls (such as <see cref="IntegerSpinner"/> and <see cref="DoubleSpinner"/>).
    /// </summary>
    public class NewSpinnerBase : Control
    {

        /// <summary>
        /// Create a NewSpinnerBase.
        /// </summary>
        public NewSpinnerBase()
        {
            Loaded += NewSpinnerBase_Loaded;

            keyDownTimer.AutoReset = false;
            advanceTimer.AutoReset = true;

            keyDownTimer.Elapsed += (s, e) => advanceTimer.Start();
            advanceTimer.Elapsed += AdvanceTimer_Elapsed;

            PropertyChanged += (x, y) => ValidateValue();
        }

        private void NewSpinnerBase_Loaded(object sender, RoutedEventArgs e)
        {
            // doesn't work in constructor, apparently
            _raiseChangedEvent = false;
            ValidateValue();
            _raiseChangedEvent = true;
        }

#if NETCOREAPP
        private void AdvanceTimer_Elapsed(object? sender, ElapsedEventArgs e)
#else
        private void AdvanceTimer_Elapsed(object sender, ElapsedEventArgs e)
#endif
        {
            try
            {
                Dispatcher.Invoke(() =>
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
                }, System.Windows.Threading.DispatcherPriority.Input);
            }
            catch (TaskCanceledException)
            {
                advanceTimer.Stop();
            }
        }

        #region Commands

        // these will be used for the transition from standard UserControls to full templated controls

        /// <summary>A WPF command that when executed, will increase a spinner's value by its <c>Step</c> amount</summary>
        public static RoutedCommand StepUp { get; } = new RoutedCommand("StepUp", typeof(NewSpinnerBase));

        /// <summary>A WPF command that when executed, will decrease a spinner's value by its <c>Step</c> amount</summary>
        public static RoutedCommand StepDown { get; } = new RoutedCommand("StepDown", typeof(NewSpinnerBase));

        #endregion

        #region IsAtMaxValue / IsAtMinValue

        /// <summary>
        /// The internal dependency property key, used for setting the <see cref="IsAtMaxValue"/> property. Only accessible to <c>NewSpinnerBase</c> and controls that inherit from it.
        /// </summary>
        protected static readonly DependencyPropertyKey IsAtMaxValuePropertyKey
            = DependencyProperty.RegisterReadOnly("IsAtMaxValue", typeof(bool), typeof(NewSpinnerBase), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// The internal dependency property key, used for setting the <see cref="IsAtMinValue"/> property. Only accessible to <c>NewSpinnerBase</c> and controls that inherit from it.
        /// </summary>
        protected static readonly DependencyPropertyKey IsAtMinValuePropertyKey
            = DependencyProperty.RegisterReadOnly("IsAtMinValue", typeof(bool), typeof(NewSpinnerBase), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// A dependency property object backing the <see cref="IsAtMaxValue"/> property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty IsAtMaxValueProperty = IsAtMaxValuePropertyKey.DependencyProperty;

        /// <summary>
        /// A dependency property object backing the <see cref="IsAtMinValue"/> property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty IsAtMinValueProperty = IsAtMinValuePropertyKey.DependencyProperty;

        /// <summary>
        /// Get if this spinner's <c>Value</c> is currently equal to its <c>MaxValue</c>. If true, the value cannot be increased any more.
        /// </summary>
        public bool IsAtMaxValue { get => (bool)GetValue(IsAtMaxValueProperty); protected set => SetValue(IsAtMaxValuePropertyKey, value); }

        /// <summary>
        /// Get if this spinner's <c>Value</c> is currently equal to its <c>MinValue</c>. If true, the value cannot be decreased any more.
        /// </summary>
        public bool IsAtMinValue { get => (bool)GetValue(IsAtMinValueProperty); protected set => SetValue(IsAtMinValuePropertyKey, value); }

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
#if NETCOREAPP
        public event EventHandler? PropertyChanged;
#else
        public event EventHandler PropertyChanged;
#endif

#if NETCOREAPP
        /// <summary>
        /// Raised when the Value property is changed.
        /// </summary>
        public event DependencyPropertyChangedEventHandler? ValueChanged;
        
        /// <summary>
        /// Raised when the MinValue property is changed.
        /// </summary>
        public event DependencyPropertyChangedEventHandler? MinValueChanged;
        
        /// <summary>
        /// Raised when the MaxValue property is changed.
        /// </summary>
        public event DependencyPropertyChangedEventHandler? MaxValueChanged;

        /// <summary>
        /// Raised when the Value property is validated, and changed to a valid value if needed.
        /// </summary>
        public event EventHandler? ValueValidated;
#else
        /// <summary>
        /// Raised when the Value property is changed.
        /// </summary>
        public event DependencyPropertyChangedEventHandler ValueChanged;

        /// <summary>
        /// Raised when the MinValue property is changed.
        /// </summary>
        public event DependencyPropertyChangedEventHandler MinValueChanged;

        /// <summary>
        /// Raised when the MaxValue property is changed.
        /// </summary>
        public event DependencyPropertyChangedEventHandler MaxValueChanged;

        /// <summary>
        /// Raised when the Value property is validated, and changed to a valid value if needed.
        /// </summary>
        public event EventHandler ValueValidated;
#endif

        /// <summary>
        /// Raise the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Extra event data to provide to any event listeners.</param>
        protected void RaiseValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_raiseChangedEvent) ValueChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// Raise the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Extra event data to provide to any event listeners.</param>
        protected void RaiseMinValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MinValueChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// Raise the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Extra event data to provide to any event listeners.</param>
        protected void RaiseMaxValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MaxValueChanged?.Invoke(sender, e);
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
        /// Add a dependency property to trigger the <see cref="PropertyChanged"/> event when its value changes.
        /// </summary>
        /// <param name="property">The dependency property to monitor changes to.</param>
        /// <param name="targetType">The type which contains the dependency property.</param>
        protected void AddPropertyChangedTrigger(DependencyProperty property, Type targetType)
        {
            DependencyPropertyDescriptor.FromProperty(property, targetType).AddValueChanged(this, PropertyChanged);
        }
        #endregion

        #region Brushes

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ButtonBackgroundProperty = DependencyProperty.Register(
            "ButtonBackground", typeof(Brush), typeof(NewSpinnerBase),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.White)));

        public static readonly DependencyProperty DisabledBrushProperty = DependencyProperty.Register(
            "DisabledBrush", typeof(Brush), typeof(NewSpinnerBase),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(NewSpinnerBase),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(NewSpinnerBase),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty ClickBrushProperty = DependencyProperty.Register(
            "ClickBrush", typeof(Brush), typeof(NewSpinnerBase),
            new PropertyMetadata(new SolidColorBrush(Colors.Gainsboro)));

        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            "BorderDisabledBrush", typeof(Brush), typeof(NewSpinnerBase),
            new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the brush used for the background of the buttons of the spinner.
        /// </summary>
        [Category("Brushes")]
        public Brush ButtonBackground
        {
            get
            {
                return (Brush)GetValue(ButtonBackgroundProperty);
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
        public Brush DisabledBrush
        {
            get
            {
                return (Brush)GetValue(DisabledBrushProperty);
            }
            set
            {
                SetValue(DisabledBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush of the border around the control.
        /// </summary>
        [Category("Brushes")]
        public new Brush BorderBrush
        {
            get
            {
                return (Brush)GetValue(BorderBrushProperty);
            }
            set
            {
                SetValue(BorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used when a button is highlighted (i.e. has a mouse over it or keyboard focus).
        /// </summary>
        [Category("Brushes")]
        public Brush HighlightBrush
        {
            get
            {
                return (Brush)GetValue(HighlightBrushProperty);
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
        public Brush ClickBrush
        {
            get
            {
                return (Brush)GetValue(ClickBrushProperty);
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
        public Brush BorderDisabledBrush
        {
            get
            {
                return (Brush)GetValue(BorderDisabledBrushProperty);
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
        /// The dependency property object for the <see cref="RepeatDelay"/> property. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty RepeatDelayProperty = DependencyProperty.Register(
            "RepeatDelay", typeof(double), typeof(NewSpinnerBase),
            new PropertyMetadata(300d, new PropertyChangedCallback((d, e) => d.PerformAs<NewSpinnerBase>((s) => s.OnRepeatDelayChanged()))));

        /// <summary>
        /// The routed event object for the <see cref="RepeatDelayChanged"/> event. See the related event for details.
        /// </summary>
        public static readonly RoutedEvent RepeatDelayChangedEvent = EventManager.RegisterRoutedEvent(
            "RepeatDelayChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NewSpinnerBase));

        /// <summary>
        /// Raised when the RepeatDelay property is changed.
        /// </summary>
        public event RoutedEventHandler RepeatDelayChanged
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

        /// <summary>
        /// Get or set the delay period before starting the repeatedly stepping up or down while the button is held, in milliseconds. Default is 300 milliseconds.
        /// </summary>
        [Category("Common")]
        public double RepeatDelay
        {
            get => (double)GetValue(RepeatDelayProperty);
            set => SetValue(RepeatDelayProperty, value);
        }

        #endregion

        #region CornerRadiusProperty

        /// <summary>
        /// The dependency property object for the <see cref="CornerRadius"/> property. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", typeof(CornerRadius), typeof(NewSpinnerBase),
            new PropertyMetadata(new CornerRadius(0), new PropertyChangedCallback((d, e) => d.PerformAs<NewSpinnerBase>((s) => s.OnCornerRadiusChanged()))));

        /// <summary>
        /// The routed event object for the <see cref="CornerRadiusChanged"/> event. See the related event for details.
        /// </summary>
        public static readonly RoutedEvent CornerRadiusChangedEvent = EventManager.RegisterRoutedEvent(
            "CornerRadiusChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NewSpinnerBase));

        /// <summary>
        /// Raised when the CornerRadius property is changed.
        /// </summary>
        public event RoutedEventHandler CornerRadiusChanged
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
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #endregion

        #region AcceptExpressionsProperty

        /// <summary>
        /// The dependency property object for the <see cref="AcceptExpressions"/> property. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty AcceptExpressionsProperty = DependencyProperty.Register(
            "AcceptExpressions", typeof(bool), typeof(NewSpinnerBase),
            new PropertyMetadata(true));

        /// <summary>
        /// Get or set if the spinner should evaluate arithmetic expressions (such as "2+5") to accept as a value.
        /// </summary>
        /// <remarks>
        /// See the <see cref="ArithmeticParser"/> class for more info about how expressions are parsed.
        /// </remarks>
        [Category("Common")]
        public bool AcceptExpressions
        {
            get => (bool)GetValue(AcceptExpressionsProperty);
            set => SetValue(AcceptExpressionsProperty, value);
        }

        #endregion

        #region ShowArrowsProperty

        /// <summary>
        /// The dependency property object for the <see cref="ShowArrows"/> property. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ShowArrowsProperty = DependencyProperty.Register(
            "ShowArrows", typeof(bool), typeof(NewSpinnerBase),
            new PropertyMetadata(true, new PropertyChangedCallback((d, e) => d.PerformAs<NewSpinnerBase>((s) => s.OnShowArrowsChanged()))));

        /// <summary>
        /// The routed event object for the <see cref="ShowArrowsChanged"/> event. See the related event for details.
        /// </summary>
        public static readonly RoutedEvent ShowArrowsChangedEvent = EventManager.RegisterRoutedEvent(
            "ShowArrowsChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NewSpinnerBase));

        /// <summary>
        /// Raised when the ShowArrows property is changed.
        /// </summary>
        public event RoutedEventHandler ShowArrowsChanged
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
        public bool ShowArrows
        {
            get => (bool)GetValue(ShowArrowsProperty);
            set => SetValue(ShowArrowsProperty, value);
        }

        #endregion

        #region MinimumDigitCount

        /// <summary>
        /// The dependency property object for the <see cref="MinimumDigitCount"/> property. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty MinimumDigitCountProperty = DependencyProperty.Register(
            "MinimumDigitCount", typeof(int), typeof(NewSpinnerBase),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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
        public int MinimumDigitCount
        {
            get => (int)GetValue(MinimumDigitCountProperty);
            set => SetValue(MinimumDigitCountProperty, value);
        }

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
        /// <remarks>
        /// Overridding functions of this should additionally set the <see cref="IsAtMaxValue"/> and <see cref="IsAtMinValue"/> properties at this stage.
        /// </remarks>
        protected virtual void UpdateValue(DependencyPropertyChangedEventArgs e)
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
}
