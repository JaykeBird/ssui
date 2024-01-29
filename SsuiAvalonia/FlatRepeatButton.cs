using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace SolidShineUi
{

    /// <summary>
    /// A <see cref="FlatButton"/> with extra functionality that activates when the button is pressed.
    /// This includes repeatedly firing an Execute event while the button is pressed down, and executing commands when pressing starts and stops.
    /// </summary>
    /// <remarks>
    /// This is similar to the <see cref="Avalonia.Controls.RepeatButton"/>. One major difference is that this activates a separate Execute event
    /// over and over while this button is being pressed, rather than activating the Click event. This provides some finer control over 
    /// </remarks>
    public class FlatRepeatButton : FlatButton
    {
        /// <summary>
        /// Create a FlatRepeatButton.
        /// </summary>
        public FlatRepeatButton()
        {
            SetupTimer();
            Click += FlatRepeatButton_Click;
        }

        private void FlatRepeatButton_Click(object? sender, RoutedEventArgs e)
        {
            if (ExecuteOnFirstClick && !timerRan)
            {
                DoExecute();
            }

            timerRan = false;
        }

        #region Press Begins

        /// <summary>
        /// The backing value for the <see cref="PressBegins"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent PressBeginsEvent =
            RoutedEvent.Register<FlatRepeatButton, RoutedEventArgs>("PressBegins", RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when the user starts clicking/pressing this button.
        /// </summary>
        /// <remarks>
        /// This will execute whenever <see cref="FlatButton.IsPressed"/> is changed to <c>true</c>. This occurs when the user clicks down on
        /// the button, or presses the Space or Enter key on the button, or presses on the button via touch, pen, or other pointer.
        /// </remarks>
        public event EventHandler<RoutedEventArgs> PressBegins
        {
            add { AddHandler(PressBeginsEvent, value); }
            remove { RemoveHandler(PressBeginsEvent, value); }
        }

        /// <summary>
        /// Get or set the command to execute when this button begins being pressed.
        /// </summary>
        /// <remarks>
        /// This will execute whenever <see cref="FlatButton.IsPressed"/> is changed to <c>true</c>. This occurs when the user clicks down on
        /// the button, or presses the Space or Enter key on the button, or presses on the button via touch, pen, or other pointer.
        /// </remarks>
        public ICommand? PressBeginsCommand { get => GetValue(PressBeginsCommandProperty); set => SetValue(PressBeginsCommandProperty, value); }

        /// <summary>The backing styled property for <see cref="PressBeginsCommand"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ICommand?> PressBeginsCommandProperty
            = AvaloniaProperty.Register<FlatRepeatButton, ICommand?>(nameof(PressBeginsCommand), null);

        /// <summary>
        /// Get or set the parameter to pass with <see cref="PressBeginsCommand"/> when it is executed. Default value is <c>null</c>.
        /// </summary>
        public object? PressBeginsCommandParameter { get => GetValue(PressBeginsCommandParameterProperty); set => SetValue(PressBeginsCommandParameterProperty, value); }

        /// <summary>The backing styled property for <see cref="PressBeginsCommandParameter"/>. See the related property for details.</summary>
        public static readonly StyledProperty<object?> PressBeginsCommandParameterProperty
            = AvaloniaProperty.Register<FlatRepeatButton, object?>(nameof(PressBeginsCommandParameter), null);

        #endregion

        #region Press Ends

        /// <summary>
        /// The backing value for the <see cref="PressEnds"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent PressEndsEvent =
            RoutedEvent.Register<FlatRepeatButton, RoutedEventArgs>("PressEnds", RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when the user stops clicking/pressing this button.
        /// </summary>
        /// <remarks>
        /// This will execute whenever <see cref="FlatButton.IsPressed"/> is changed to <c>false</c>. This occurs when the user releases a
        /// mouse button while over this button, or releases the Space or Enter key, or moves the touch, pen, or other pointer away from the button. This also occurs if the user
        /// moves the mouse cursor away from the button while still holding down a mouse button (and bringing the mouse cursor back to trigger the press begins actions again).
        /// </remarks>
        public event EventHandler<RoutedEventArgs> PressEnds
        {
            add { AddHandler(PressEndsEvent, value); }
            remove { RemoveHandler(PressEndsEvent, value); }
        }

        /// <summary>
        /// Get or set the command to execute when this button is no longer being pressed.
        /// </summary>
        /// <remarks>
        /// This will execute whenever <see cref="FlatButton.IsPressed"/> is changed to <c>false</c>. This occurs when the user releases a
        /// mouse button while over this button, or releases the Space or Enter key, or moves the touch, pen, or other pointer away from the button. This also occurs if the user
        /// moves the mouse cursor away from the button while still holding down a mouse button (and bringing the mouse cursor back to trigger the press begins actions again).
        /// </remarks>
        public ICommand? PressEndsCommand { get => GetValue(PressEndsCommandProperty); set => SetValue(PressEndsCommandProperty, value); }

        /// <summary>The backing styled property for <see cref="PressEndsCommand"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ICommand?> PressEndsCommandProperty
            = AvaloniaProperty.Register<FlatRepeatButton, ICommand?>(nameof(PressEndsCommand), null);

        /// <summary>
        /// Get or set the parameter to pass with <see cref="PressEndsCommand"/> when it is executed. Default value is <c>null</c>.
        /// </summary>
        public object? PressEndsCommandParameter { get => GetValue(PressEndsCommandParameterProperty); set => SetValue(PressEndsCommandParameterProperty, value); }

        /// <summary>The backing styled property for <see cref="PressEndsCommandParameter"/>. See the related property for details.</summary>
        public static readonly StyledProperty<object?> PressEndsCommandParameterProperty
            = AvaloniaProperty.Register<FlatRepeatButton, object?>(nameof(PressEndsCommandParameter), null);

        #endregion

        #region Execute (repeatedly)

        /// <summary>
        /// The backing value for the <see cref="Execute"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent ExecuteEvent =
            RoutedEvent.Register<FlatRepeatButton, RoutedEventArgs>("Execute", RoutingStrategies.Bubble);

        /// <summary>
        /// Raised while this button is being pressed. This continues to be raised repeatedly until the button is no longer pressed.
        /// </summary>
        /// <remarks>
        /// This will begin whenever <see cref="FlatButton.IsPressed"/> is changed to <c>true</c>, and will stop when it changes back to <c>false</c>. 
        /// This includes when the user clicks and holds a mouse button over this button, pressing down on the Space or Enter key, or pressing on the butotn with touch, a pen, or other
        /// pointer.
        /// <para/>
        /// You can use <see cref="Delay"/> to set up how long the initial delay is until this command begins being executed, and <see cref="Interval"/> to set how soon each 
        /// successive execution should occur after the previous one.
        /// You can also use <see cref="ExecuteOnFirstClick"/> to disable this raising when the button is just clicked, without holding it down beyond <see cref="Delay"/>.
        /// </remarks>
        public event EventHandler<RoutedEventArgs> Execute
        {
            add { AddHandler(ExecuteEvent, value); }
            remove { RemoveHandler(ExecuteEvent, value); }
        }

        /// <summary>
        /// Get or set the command to execute while this button is being pressed. This continues to be executed repeatedly until the button is no longer pressed.
        /// </summary>
        /// <remarks>
        /// This will begin whenever <see cref="FlatButton.IsPressed"/> is changed to <c>true</c>, and will stop when it changes back to <c>false</c>. 
        /// This includes when the user clicks and holds a mouse button over this button, pressing down on the Space or Enter key, or pressing on the butotn with touch, a pen, or other
        /// pointer.
        /// <para/>
        /// You can use <see cref="Delay"/> to set up how long the initial delay is until this command begins being executed, and <see cref="Interval"/> to set how soon each 
        /// successive execution should occur after the previous one.
        /// You can also use <see cref="ExecuteOnFirstClick"/> to disable this raising when the button is just clicked, without holding it down beyond <see cref="Delay"/>.
        /// </remarks>
        public ICommand? ExecuteCommand { get => GetValue(ExecuteCommandProperty); set => SetValue(ExecuteCommandProperty, value); }

        /// <summary>The backing styled property for <see cref="ExecuteCommand"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ICommand?> ExecuteCommandProperty
            = AvaloniaProperty.Register<FlatRepeatButton, ICommand?>(nameof(ExecuteCommand), null);

        /// <summary>
        /// Get or set the parameter to pass with <see cref="ExecuteCommand"/> when it is executed. Default value is <c>null</c>.
        /// </summary>
        public object? ExecuteCommandParameter { get => GetValue(ExecuteCommandParameterProperty); set => SetValue(ExecuteCommandParameterProperty, value); }

        /// <summary>The backing styled property for <see cref="ExecuteCommandParameter"/>. See the related property for details.</summary>
        public static readonly StyledProperty<object?> ExecuteCommandParameterProperty
            = AvaloniaProperty.Register<FlatRepeatButton, object?>(nameof(ExecuteCommandParameter), null);

        #endregion

        #region Timer

        void SetupTimer()
        {
            executeTimer.Tick += ExecuteTimer_Elapsed;
            ResetTimer();
        }

        void ResetTimer()
        {
            executeTimer.Stop();
            executeTimer.Interval = new TimeSpan(0, 0, 0, 0, Delay);
            //executeTimer.AutoReset = false;
        }

        private void ExecuteTimer_Elapsed(object? sender, EventArgs e)
        {
            if (firstRun)
            {
                executeTimer.IsEnabled = false;
                executeTimer.Interval = new TimeSpan(0, 0, 0, 0, Interval);
                executeTimer.Start();

                firstRun = false;
                timerRan = true;

                executeTimer.Start();
            }

            DoExecute();
        }

        /// <summary>
        /// the timer to set how long to wait before responding to and acting upon a key press (for changing the value)
        /// </summary>
        protected DispatcherTimer executeTimer = new DispatcherTimer(DispatcherPriority.Input);

        bool firstRun = false;
        bool timerRan = false;

        /// <summary>
        /// Get or set how long of a pause there should be between <see cref="Execute"/> firing, while the button is being pressed. Measured in milliseconds.
        /// </summary>
        public int Interval { get => GetValue(IntervalProperty); set => SetValue(IntervalProperty, value); }

        /// <summary>The backing styled property for <see cref="Interval"/>. See the related property for details.</summary>
        public static readonly StyledProperty<int> IntervalProperty
            = AvaloniaProperty.Register<FlatRepeatButton, int>(nameof(Interval), 200);

        /// <summary>
        /// Get or set how long the delay should be after the button is initially pressed, before starting to raise <see cref="Execute"/>. Measured in milliseconds.
        /// </summary>
        public int Delay { get => GetValue(DelayProperty); set => SetValue(DelayProperty, value); }

        /// <summary>The backing styled property for <see cref="Delay"/>. See the related property for details.</summary>
        public static readonly StyledProperty<int> DelayProperty
            = AvaloniaProperty.Register<FlatRepeatButton, int>(nameof(Delay), 200);

        #endregion

        /// <summary>
        /// Get or set if the Execute event should be activated when the button is initially clicked, even if the <see cref="Delay"/> time hasn't been reached.
        /// </summary>
        /// <remarks>
        /// When this is <c>true</c>, this guarantees that Execute is ran at least once when button is clicked, even if the button was pressed for long enough
        /// to wait past <see cref="Delay"/> and trigger running the Execute event repeatedly. This is the default and expected behaviour with a RepeatButton,
        /// but if you need to have more fine control over when Execute runs, this can be set to false.
        /// </remarks>
        public bool ExecuteOnFirstClick { get => GetValue(ExecuteOnFirstClickProperty); set => SetValue(ExecuteOnFirstClickProperty, value); }

        /// <summary>The backing styled property for <see cref="ExecuteOnFirstClick"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> ExecuteOnFirstClickProperty
            = AvaloniaProperty.Register<FlatRepeatButton, bool>(nameof(ExecuteOnFirstClick), true);

        #region Pressed Handling / Core Logic

        /// <inheritdoc/>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            PressBegan();
            base.OnPointerPressed(e);
        }

        /// <inheritdoc/>
        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            PressEnded();
            base.OnPointerReleased(e);
        }

        void PressBegan()
        {
            if (PressBeginsCommand != null)
            {
                ActivateCommand(PressBeginsCommand, PressBeginsCommandParameter);
            }
            RoutedEventArgs re = new RoutedEventArgs(PressBeginsEvent, this);
            RaiseEvent(re);

            firstRun = true;
            executeTimer.Start();
        }

        void PressEnded()
        {
            if (PressEndsCommand != null)
            {
                ActivateCommand(PressEndsCommand, PressEndsCommandParameter);
            }
            RoutedEventArgs re = new RoutedEventArgs(PressEndsEvent, this);
            RaiseEvent(re);

            ResetTimer();
        }

        /// <summary>
        /// Raise the <see cref="Execute"/> event and activate the <see cref="ExecuteCommand"/> (if present). This occurs automatically and repeatedly while the button is being pressed.
        /// This method allows you to cause this to occur programmatically at any point, as if the button was being pressed.
        /// </summary>
        public void DoExecute()
        {
            if (ExecuteCommand != null)
            {
                ActivateCommand(ExecuteCommand, ExecuteCommandParameter);
            }
            RoutedEventArgs re = new RoutedEventArgs(ExecuteEvent, this);
            RaiseEvent(re);
        }

        /// <summary>
        /// Execute a particular command, with an optional parameter and target.
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="parameter">The parameter to pass with the command</param>
        static void ActivateCommand(ICommand command, object? parameter)
        {
            if (command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }

        #endregion
    }
}
