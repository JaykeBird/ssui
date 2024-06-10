using SolidShineUi.Utils;
using System.Windows;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace SolidShineUi.Ribbon.Utils
{
    /// <summary>
    /// A base class used for displaying spinner controls in the <see cref="Ribbon"/>.
    /// </summary>
    public abstract class RibbonSpinnerBase : RibbonContentControl
    {
        /// <summary>
        /// Create a RibbonSpinnerBase.
        /// </summary>
        public RibbonSpinnerBase()
        {
            ColorSchemeChanged += RibbonSpinnerBase_ColorSchemeChanged;
        }

        private void RibbonSpinnerBase_ColorSchemeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ColorScheme cs)
            {
                ApplySpinnerColorScheme(cs);
            }
        }

        /// <summary>
        /// Apply a color scheme to this control. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply.</param>
        private void ApplySpinnerColorScheme(ColorScheme cs)
        {
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }

            BorderBrush = cs.BorderColor.ToBrush();
            DisabledBrush = cs.LightDisabledColor.ToBrush();
            BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();

            ButtonBackground = cs.IsHighContrast ? cs.BackgroundColor.ToBrush() : cs.SecondaryColor.ToBrush();

            ClickBrush = cs.ThirdHighlightColor.ToBrush();
            HighlightBrush = cs.HighlightColor.ToBrush();
            Foreground = cs.ForegroundColor.ToBrush();
        }

        #region Events

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
            ValueChanged?.Invoke(sender, e);
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

        #endregion

        #region Brushes

        /// <summary>The backing dependency property for <see cref="ButtonBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ButtonBackgroundProperty = DependencyProperty.Register(
            "ButtonBackground", typeof(Brush), typeof(RibbonSpinnerBase),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.White)));

        /// <summary>The backing dependency property for <see cref="DisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty DisabledBrushProperty = DependencyProperty.Register(
            "DisabledBrush", typeof(Brush), typeof(RibbonSpinnerBase),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>The backing dependency property for <see cref="BorderBrush"/>. See the related property for details.</summary>
        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(RibbonSpinnerBase),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>The backing dependency property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(RibbonSpinnerBase),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>The backing dependency property for <see cref="ClickBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ClickBrushProperty = DependencyProperty.Register(
            "ClickBrush", typeof(Brush), typeof(RibbonSpinnerBase),
            new PropertyMetadata(new SolidColorBrush(Colors.Gainsboro)));

        /// <summary>The backing dependency property for <see cref="BorderDisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            "BorderDisabledBrush", typeof(Brush), typeof(RibbonSpinnerBase),
            new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

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
            "RepeatDelay", typeof(int), typeof(RibbonSpinnerBase),
            new PropertyMetadata(300, new PropertyChangedCallback((d, e) => d.PerformAs<RibbonSpinnerBase>((s) => s.OnRepeatDelayChanged(e)))));

        /// <summary>
        /// The routed event object for the <see cref="RepeatDelayChanged"/> event. See the related event for details.
        /// </summary>
        public static readonly RoutedEvent RepeatDelayChangedEvent = EventManager.RegisterRoutedEvent(
            "RepeatDelayChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(RibbonSpinnerBase));

        /// <summary>
        /// Raised when the RepeatDelay property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<int> RepeatDelayChanged
        {
            add { AddHandler(RepeatDelayChangedEvent, value); }
            remove { RemoveHandler(RepeatDelayChangedEvent, value); }
        }

        /// <summary>
        /// Update internal values based upon a change in the <see cref="RepeatDelay"/> property.
        /// </summary>
        protected virtual void OnRepeatDelayChanged(DependencyPropertyChangedEventArgs e)
        {
            //keyDownTimer.Interval = RepeatDelay;
            RoutedPropertyChangedEventArgs<int> re = new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, (int)e.NewValue, RepeatDelayChangedEvent);
            re.Source = this;
            RaiseEvent(re);
        }

        /// <summary>
        /// Get or set the delay period before starting the repeatedly stepping up or down while the button is held, in milliseconds. Default is 300 milliseconds.
        /// </summary>
        /// <remarks>
        /// Once this time period is reached and the button continues to be held down, then the spinner will begin repeatedly stepping up or down, at the rate
        /// specified in <see cref="Interval"/>. This will continue until the button is no longer held down, or the <c>MaxValue</c> or <c>MinValue</c> is reached.
        /// </remarks>
        [Category("Common")]
        public int RepeatDelay
        {
            get => (int)GetValue(RepeatDelayProperty);
            set => SetValue(RepeatDelayProperty, value);
        }

        #endregion

        #region IntervalProperty

        /// <summary>
        /// Get or set the rate of repeatedly stepping up or down while a button is held, in milliseconds. Default is 50 milliseconds.
        /// </summary>
        /// <remarks>
        /// While holding down a button, once the <see cref="RepeatDelay"/> time period is reached, then stepping begins to occur repeatedly, at the rate
        /// specified in this property. This continues until the button is no longer held, or the <c>MaxValue</c> or <c>MinValue</c> is reached.
        /// </remarks>
        [Category("Common")]
        public int Interval { get => (int)GetValue(IntervalProperty); set => SetValue(IntervalProperty, value); }

        /// <summary>The backing dependency property for <see cref="Interval"/>. See the related property for details.</summary>
        public static DependencyProperty IntervalProperty
            = DependencyProperty.Register(nameof(Interval), typeof(int), typeof(RibbonSpinnerBase),
            new FrameworkPropertyMetadata(50, (d, e) => d.PerformAs<RibbonSpinnerBase>((o) => o.OnIntervalChanged(e))));

        /// <summary>
        /// Update internal values based upon a change in the <see cref="RepeatDelay"/> property.
        /// </summary>
        protected virtual void OnIntervalChanged(DependencyPropertyChangedEventArgs e)
        {
            //advanceTimer.Interval = Interval;
            RoutedPropertyChangedEventArgs<int> re = new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, (int)e.NewValue, IntervalChangedEvent);
            re.Source = this;
            RaiseEvent(re);
        }

        /// <summary>
        /// The routed event object for the <see cref="IntervalChanged"/> event. See the related event for details.
        /// </summary>
        public static readonly RoutedEvent IntervalChangedEvent = EventManager.RegisterRoutedEvent(
            "IntervalChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(RibbonSpinnerBase));

        /// <summary>
        /// Raised when the <see cref="Interval"/> property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<int> IntervalChanged
        {
            add { AddHandler(IntervalChangedEvent, value); }
            remove { RemoveHandler(IntervalChangedEvent, value); }
        }

        #endregion

        #region CornerRadiusProperty

        /// <summary>
        /// The dependency property object for the <see cref="CornerRadius"/> property. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", typeof(CornerRadius), typeof(RibbonSpinnerBase),
            new PropertyMetadata(new CornerRadius(0), new PropertyChangedCallback((d, e) => d.PerformAs<RibbonSpinnerBase>((s) => s.OnCornerRadiusChanged(e)))));

        /// <summary>
        /// The routed event object for the <see cref="CornerRadiusChanged"/> event. See the related event for details.
        /// </summary>
        public static readonly RoutedEvent CornerRadiusChangedEvent = EventManager.RegisterRoutedEvent(
            "CornerRadiusChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<CornerRadius>), typeof(RibbonSpinnerBase));

        /// <summary>
        /// Raised when the CornerRadius property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<CornerRadius> CornerRadiusChanged
        {
            add { AddHandler(CornerRadiusChangedEvent, value); }
            remove { RemoveHandler(CornerRadiusChangedEvent, value); }
        }

        /// <summary>
        /// Update internal values based upon a change in the <see cref="CornerRadius"/> property.
        /// </summary>
        protected virtual void OnCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            RoutedPropertyChangedEventArgs<CornerRadius> re = new RoutedPropertyChangedEventArgs<CornerRadius>((CornerRadius)e.OldValue, (CornerRadius)e.NewValue, CornerRadiusChangedEvent);
            re.Source = this;
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
            "AcceptExpressions", typeof(bool), typeof(RibbonSpinnerBase),
            new PropertyMetadata(true));

        /// <summary>
        /// Get or set if the spinner should evaluate arithmetic expressions (such as "2+5") to accept as a value.
        /// </summary>
        /// <remarks>
        /// See the <see cref="ArithmeticParser"/> class for more info about how expressions are parsed.
        /// (For spinners that don't interact with numbers, this property does not do anything.)
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
            "ShowArrows", typeof(bool), typeof(RibbonSpinnerBase),
            new PropertyMetadata(true, new PropertyChangedCallback((d, e) => d.PerformAs<RibbonSpinnerBase>((s) => s.OnShowArrowsChanged(e)))));

        /// <summary>
        /// The routed event object for the <see cref="ShowArrowsChanged"/> event. See the related event for details.
        /// </summary>
        public static readonly RoutedEvent ShowArrowsChangedEvent = EventManager.RegisterRoutedEvent(
            "ShowArrowsChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(RibbonSpinnerBase));

        /// <summary>
        /// Raised when the <see cref="ShowArrows"/> property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> ShowArrowsChanged
        {
            add { AddHandler(ShowArrowsChangedEvent, value); }
            remove { RemoveHandler(ShowArrowsChangedEvent, value); }
        }

        /// <summary>
        /// Update internal values based upon a change in the <see cref="ShowArrows"/> property.
        /// </summary>
        protected virtual void OnShowArrowsChanged(DependencyPropertyChangedEventArgs e)
        {
            RoutedPropertyChangedEventArgs<bool> re = new RoutedPropertyChangedEventArgs<bool>((bool)e.OldValue, (bool)e.NewValue, ShowArrowsChangedEvent);
            re.Source = this;
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

        #region ShowButtonDividerProperty

        ///// <summary>
        ///// Get or set if a small divider border should be shown between the buttons and the text box.
        ///// </summary>
        ///// <remarks>
        ///// In earlier versions of Solid Shine UI, there was no divider shown (although there actually should've been one).
        ///// So the default value now is <c>true</c>, as is the intended behavior, but this can be set to <c>false</c> if you want to maintain appearance 
        ///// </remarks>
        //public bool ShowButtonDivider { get => (bool)GetValue(ShowButtonDividerProperty); set => SetValue(ShowButtonDividerProperty, value); }

        ///// <summary>The backing dependency property for <see cref="ShowButtonDivider"/>. See the related property for details.</summary>
        //public static DependencyProperty ShowButtonDividerProperty
        //    = DependencyProperty.Register(nameof(ShowButtonDivider), typeof(bool), typeof(NewRibbonSpinnerBase),
        //    new FrameworkPropertyMetadata(true));

        #endregion

        #region MinimumDigitCount

        /// <summary>
        /// The dependency property object for the <see cref="MinimumDigitCount"/> property. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty MinimumDigitCountProperty = DependencyProperty.Register(
            "MinimumDigitCount", typeof(int), typeof(RibbonSpinnerBase),
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
        /// <para/>
        /// For spinners that don't interact with numbers, this property will not do anything.
        /// </remarks>
        [Category("Common")]
        public int MinimumDigitCount
        {
            get => (int)GetValue(MinimumDigitCountProperty);
            set => SetValue(MinimumDigitCountProperty, value);
        }

        #endregion

        #endregion

    }
}
