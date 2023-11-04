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
    public class SpinnerBase : UserControl
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

        private void SpinnerBase_Loaded(object sender, RoutedEventArgs e)
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
                        StepUp();
                    }
                    else
                    {
                        StepDown();
                    }

                    UpdateUI();
                }, System.Windows.Threading.DispatcherPriority.Input);
            }
            catch (TaskCanceledException)
            {
                advanceTimer.Stop();
            }
        }

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

        /// <summary>
        /// Raised when the Value property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ValueChanged;

        /// <summary>
        /// Raised when the Value property is validated, and changed to a valid value if needed.
        /// </summary>
        public event EventHandler? ValueValidated;
#else
        public event DependencyPropertyChangedEventHandler ValueChanged;

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
            "ButtonBackground", typeof(Brush), typeof(SpinnerBase),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.White)));

        public static readonly DependencyProperty DisabledBrushProperty = DependencyProperty.Register(
            "DisabledBrush", typeof(Brush), typeof(SpinnerBase),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(SpinnerBase),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(SpinnerBase),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty ClickBrushProperty = DependencyProperty.Register(
            "ClickBrush", typeof(Brush), typeof(SpinnerBase),
            new PropertyMetadata(new SolidColorBrush(Colors.Gainsboro)));

        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            "BorderDisabledBrush", typeof(Brush), typeof(SpinnerBase),
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty RepeatDelayProperty = DependencyProperty.Register(
            "RepeatDelay", typeof(double), typeof(SpinnerBase),
            new PropertyMetadata(300d, new PropertyChangedCallback(OnInternalRepeatDelayChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly RoutedEvent RepeatDelayChangedEvent = EventManager.RegisterRoutedEvent(
            "RepeatDelayChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SpinnerBase));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Raised when the RepeatDelay property is changed.
        /// </summary>
        public event RoutedEventHandler RepeatDelayChanged
        {
            add { AddHandler(RepeatDelayChangedEvent, value); }
            remove { RemoveHandler(RepeatDelayChangedEvent, value); }
        }

        private static void OnInternalRepeatDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpinnerBase s)
            {
                s.OnRepeatDelayChanged();
            }
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", typeof(CornerRadius), typeof(SpinnerBase),
            new PropertyMetadata(new CornerRadius(0), new PropertyChangedCallback((d, e) => d.PerformAs<SpinnerBase>((s) => s.OnCornerRadiusChanged()))));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly RoutedEvent CornerRadiusChangedEvent = EventManager.RegisterRoutedEvent(
            "CornerRadiusChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SpinnerBase));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty AcceptExpressionsProperty = DependencyProperty.Register(
            "AcceptExpressions", typeof(bool), typeof(SpinnerBase),
            new PropertyMetadata(true));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ShowArrowsProperty = DependencyProperty.Register(
            "ShowArrows", typeof(bool), typeof(SpinnerBase),
            new PropertyMetadata(true, new PropertyChangedCallback((d, e) => d.PerformAs<SpinnerBase>((s) => s.OnShowArrowsChanged()))));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly RoutedEvent ShowArrowsChangedEvent = EventManager.RegisterRoutedEvent(
            "ShowArrowsChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SpinnerBase));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty MinimumDigitCountProperty = DependencyProperty.Register(
            "MinimumDigitCount", typeof(int), typeof(SpinnerBase),
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
        /// Update internal values based upon visual or other changes. In SpinnerBase, this currently does nothing.
        /// </summary>
        protected virtual void UpdateUI()
        {

        }

        /// <summary>
        /// Validate the value and update the UI if needed.
        /// </summary>
        protected virtual void ValidateValue()
        {
            UpdateUI();
            RaiseValueValidated(this);
        }
        
        /// <summary>
        /// Handle the process of updating value properties. Should be overridden in child classes in order to perform validation or other functions.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void UpdateValue(DependencyPropertyChangedEventArgs e)
        {
            UpdateUI();
            RaiseValueChanged(this, e);
        }

        /// <summary>
        /// Increase the spinner's value by whatever the Step value is. Should be overridden in child classes.
        /// </summary>
        protected virtual void StepUp()
        {

        }

        /// <summary>
        /// Decrease the spinner's value by whatever the Step value is. Should be overridden in child classes.
        /// </summary>
        protected virtual void StepDown()
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
                    StepDown();
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
                    StepUp();
                }
            }
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
                StepDown();
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
                StepUp();
                UpdateUI();
            }
        }

        #endregion
    }
}
