using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi
{
    // Usage of a CheckBox is good to indicate selection (such as, checking specific items from a list of checkboxes to select those items).
    // The IsChecked value is just a bool, so this can be useful for situations where you need to present an option that's stored as a bool.

    /// <summary>
    /// A control that can display content alongside a large, touch-friendly checkbox, which users are able to check or uncheck. Provides more customization than the standard WPF CheckBox.
    /// </summary>
    /// <remarks>
    /// Unlike the standard WPF CheckBox, a nullable bool (<c>bool?</c>) is not used for <see cref="IsChecked"/>; instead, <c>IsChecked</c> will return true if there is any mark,
    /// including either a full checkmark or an indeterminate mark (a square), and false only if there is no mark at all.
    /// Instead, use <see cref="IsIndeterminate"/> to check if the mark is an indeterminate mark, or use <see cref="CheckState"/> to get the current state as an enum.
    /// </remarks>
    public class CheckBox : TemplatedControl
    {

        public CheckBox()
        {

        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            switch (change.Property.Name)
            {
                case nameof(IsChecked):
                    OnCheckedChange(change);
                    break;
                case nameof(IsIndeterminate):
                    OnIndeterminateChange(change);
                    break;
                case nameof(CheckState):
                    OnCheckStateChange(change);
                    break;
                case nameof(ColorScheme):
                    OnColorSchemeChange(change);
                    break;
            }
        }

        #region CheckState

        bool _updatingValues = false;

        /// <summary>
        /// Get or set if the check box is checked. (Note: if in the Indeterminate state, it will still return true as checked.)
        /// </summary>
        public bool IsChecked { get => GetValue(IsCheckedProperty); set => SetValue(IsCheckedProperty, value); }

        /// <summary>The backing styled property for <see cref="IsChecked"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> IsCheckedProperty
            = AvaloniaProperty.Register<CheckBox, bool>(nameof(IsChecked), false);

        /// <summary>
        /// Get or set if the check box is in the Indeterminate state. 
        /// </summary>
        public bool IsIndeterminate { get => GetValue(IsIndeterminateProperty); set => SetValue(IsIndeterminateProperty, value); }

        /// <summary>The backing styled property for <see cref="IsIndeterminate"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> IsIndeterminateProperty
            = AvaloniaProperty.Register<CheckBox, bool>(nameof(IsIndeterminate), false);

        /// <summary>
        /// Get or set the state of the checkbox, via a CheckState enum. Can be set via this property or via the IsChecked and IsIndeterminate properties.
        /// </summary>
        public CheckState CheckState { get => GetValue(CheckStateProperty); set => SetValue(CheckStateProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckState"/>. See the related property for details.</summary>
        public static readonly StyledProperty<CheckState> CheckStateProperty
            = AvaloniaProperty.Register<CheckBox, CheckState>(nameof(CheckState), CheckState.Unchecked);

        /// <summary>
        /// Raised when the CheckState property is changed, either to Checked, Indeterminate, or Unchecked.
        /// </summary>
        public event EventHandler<RoutedEventArgs> CheckChanged
        {
            add { AddHandler(CheckChangedEvent, value); }
            remove { RemoveHandler(CheckChangedEvent, value); }
        }

        /// <summary>
        /// The backing routed event for <see cref="CheckChanged"/>. See the related event for more info.
        /// </summary>
        public RoutedEvent<RoutedEventArgs> CheckChangedEvent = RoutedEvent.Register<RoutedEventArgs>
            (nameof(CheckChanged), RoutingStrategies.Bubble, typeof(CheckBox));

        /// <summary>
        /// Raised when the CheckState property is changed to Checked.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Checked
        {
            add { AddHandler(CheckedEvent, value); }
            remove { RemoveHandler(CheckedEvent, value); }
        }

        /// <summary>
        /// The backing routed event for <see cref="Checked"/>. See the related event for more info.
        /// </summary>
        public RoutedEvent<RoutedEventArgs> CheckedEvent = RoutedEvent.Register<RoutedEventArgs>
            (nameof(Checked), RoutingStrategies.Bubble, typeof(CheckBox));

        /// <summary>
        /// Raised when the CheckState property is changed to Unchecked.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Unchecked
        {
            add { AddHandler(UncheckedEvent, value); }
            remove { RemoveHandler(UncheckedEvent, value); }
        }

        /// <summary>
        /// The backing routed event for <see cref="Unchecked"/>. See the related event for more info.
        /// </summary>
        public RoutedEvent<RoutedEventArgs> UncheckedEvent = RoutedEvent.Register<RoutedEventArgs>
            (nameof(Unchecked), RoutingStrategies.Bubble, typeof(CheckBox));

        /// <summary>
        /// Raised when the CheckState property is changed to Indeterminate.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Indeterminate
        {
            add { AddHandler(IndeterminateEvent, value); }
            remove { RemoveHandler(IndeterminateEvent, value); }
        }

        /// <summary>
        /// The backing routed event for <see cref="Indeterminate"/>. See the related event for more info.
        /// </summary>
        public RoutedEvent<RoutedEventArgs> IndeterminateEvent = RoutedEvent.Register<RoutedEventArgs>
            (nameof(Indeterminate), RoutingStrategies.Bubble, typeof(CheckBox));

        void OnCheckedChange(AvaloniaPropertyChangedEventArgs e)
        {
            // IsChecked: bool
            if (!_updatingValues)
            {
                _updatingValues = true;

                bool value = e.GetNewValue<bool>();
                if (value == false)
                {
                    IsIndeterminate = false;
                    CheckState = CheckState.Unchecked;
                }
                else
                {
                    if (IsIndeterminate)
                    {
                        CheckState = CheckState.Indeterminate;
                    }
                    else
                    {
                        CheckState = CheckState.Checked;
                    }
                }

                _updatingValues = false;
                FinishCheckValueChange();
            }
        }

        void OnIndeterminateChange(AvaloniaPropertyChangedEventArgs e)
        {
            // IsIndeterminate: bool
            if (!_updatingValues)
            {
                _updatingValues = true;

                bool value = e.GetNewValue<bool>();
                if (value == true)
                {
                    IsChecked = true;
                    CheckState = CheckState.Indeterminate;
                }
                else
                {
                    if (IsChecked)
                    {
                        CheckState = CheckState.Checked;
                    }
                    else
                    {
                        CheckState = CheckState.Unchecked;
                    }
                }

                _updatingValues = false;
                FinishCheckValueChange();
            }
        }

        void OnCheckStateChange(AvaloniaPropertyChangedEventArgs e)
        {
            // CheckState: CheckState
            if (!_updatingValues)
            {
                _updatingValues = true;

                CheckState value = e.GetNewValue<CheckState>();

                switch (value)
                {
                    case CheckState.Unchecked:
                        IsChecked = false;
                        IsIndeterminate = false;
                        break;
                    case CheckState.Checked:
                        IsChecked = true;
                        IsIndeterminate = false;
                        break;
                    case CheckState.Indeterminate:
                        IsChecked = true;
                        IsIndeterminate = true;
                        break;
                    default:
                        IsChecked = false;
                        IsIndeterminate = false;
                        break;
                }

                _updatingValues = false;
                FinishCheckValueChange();
            }
        }

        /// <summary>
        /// Raise needed change events, and also apply pseudo-classes.
        /// </summary>
        void FinishCheckValueChange()
        {
            RoutedEventArgs re = new RoutedEventArgs(CheckChangedEvent, this);
            RaiseEvent(re);

            switch (CheckState)
            {
                case CheckState.Unchecked:
                    PseudoClasses.Remove(":checked");
                    PseudoClasses.Remove(":indeterminate");
                    RoutedEventArgs ce1 = new RoutedEventArgs(UncheckedEvent, this);
                    RaiseEvent(ce1);
                    break;
                case CheckState.Checked:
                    PseudoClasses.Add(":checked");
                    PseudoClasses.Remove(":indeterminate");
                    RoutedEventArgs ce2 = new RoutedEventArgs(CheckedEvent, this);
                    RaiseEvent(ce2);
                    break;
                case CheckState.Indeterminate:
                    PseudoClasses.Add(":checked");
                    PseudoClasses.Add(":indeterminate");
                    RoutedEventArgs ce3 = new RoutedEventArgs(IndeterminateEvent, this);
                    RaiseEvent(ce3);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Color Scheme

        /// <summary>
        /// Get or set the color scheme used for this checkbox. For easier color scheme management, bind this to the window or larger control you're using.
        /// </summary>
        public ColorScheme ColorScheme { get => GetValue(ColorSchemeProperty); set => SetValue(ColorSchemeProperty, value); }

        /// <summary>The backing styled property for <see cref="ColorScheme"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ColorScheme> ColorSchemeProperty
            = AvaloniaProperty.Register<CheckBox, ColorScheme>(nameof(ColorScheme), new ColorScheme());

        void OnColorSchemeChange(AvaloniaPropertyChangedEventArgs e)
        {
            ApplyColorScheme(e.GetNewValue<ColorScheme>());
        }

        /// <summary>
        /// Apply a color scheme to this control. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply.</param>
        public void ApplyColorScheme(ColorScheme cs)
        {
            if (cs == null)
            {
                return;
            }
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }

            if (cs.IsHighContrast)
            {
                //Background = cs.BackgroundColor.ToBrush();
                BackgroundDisabledBrush = cs.BackgroundColor.ToBrush();
                CheckForeground = cs.ForegroundColor.ToBrush();
            }
            else
            {
                //Background = Colors.White.ToBrush();
                BackgroundDisabledBrush = cs.LightDisabledColor.ToBrush();
                CheckForeground = Colors.Black.ToBrush();
            }

            //Background = cs.SecondaryColor.ToBrush();
            BorderBrush = cs.BorderColor.ToBrush();
            //HighlightBrush = cs.SecondHighlightColor.ToBrush();
            BackgroundDisabledBrush = cs.LightDisabledColor.ToBrush();
            BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
            CheckDisabledBrush = cs.DarkDisabledColor.ToBrush();
            //SelectedBrush = cs.ThirdHighlightColor.ToBrush();
            BorderHighlightBrush = cs.HighlightColor.ToBrush();
            //BorderSelectedBrush = cs.SelectionColor.ToBrush();
            Foreground = cs.ForegroundColor.ToBrush();
        }

        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the brush used for the background of the checkbox's box. This is not set via a color scheme.
        /// </summary>
        public IBrush? CheckBackground { get => GetValue(CheckBackgroundProperty); set => SetValue(CheckBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> CheckBackgroundProperty
            = AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(CheckBackground), Colors.White.ToBrush());

        /// <summary>
        /// Get or set the brush used for the check mark in the checkbox's box.
        /// </summary>
        public IBrush? CheckForeground { get => GetValue(CheckForegroundProperty); set => SetValue(CheckForegroundProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckForeground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> CheckForegroundProperty
            = AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(CheckForeground), Colors.Black.ToBrush());

        public IBrush? HighlightBrush { get => GetValue(HighlightBrushProperty); set => SetValue(HighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightBrushProperty
            = AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(HighlightBrush), Colors.Black.ToBrush());

        public IBrush? BackgroundHighlightBrush { get => GetValue(BackgroundHighlightBrushProperty); set => SetValue(BackgroundHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BackgroundHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BackgroundHighlightBrushProperty
            = AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(BackgroundHighlightBrush), null);

        /// <summary>
        /// Get or set the brush to use for the background of the checkbox's box when it is disabled.
        /// </summary>
        public IBrush? BackgroundDisabledBrush { get => GetValue(BackgroundDisabledBrushProperty); set => SetValue(BackgroundDisabledBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BackgroundDisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BackgroundDisabledBrushProperty
            = AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(BackgroundDisabledBrush), Colors.LightGray.ToBrush());

        /// <summary>
        /// Get or set the brush used for the border of the checkbox's box when it is disabled.
        /// </summary>
        public IBrush? BorderDisabledBrush { get => GetValue(BorderDisabledBrushProperty); set => SetValue(BorderDisabledBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderDisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BorderDisabledBrushProperty
            = AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(BorderDisabledBrush), Colors.DarkGray.ToBrush());

        /// <summary>
        /// Get or set the brush used for the check mark when the control is disabled.
        /// </summary>
        public IBrush? CheckDisabledBrush { get => GetValue(CheckDisabledBrushProperty); set => SetValue(CheckDisabledBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckDisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> CheckDisabledBrushProperty
            = AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(CheckDisabledBrush), Colors.DarkGray.ToBrush());

        /// <summary>
        /// Get or set the brush used for the border of the checkbox's box, while the mouse is over the control or it has keyboard focus.
        /// </summary>
        public IBrush? BorderHighlightBrush { get => GetValue(BorderHighlightBrushProperty); set => SetValue(BorderHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BorderHighlightBrushProperty
            = AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(BorderHighlightBrush), Colors.DimGray.ToBrush());

        public IBrush? CheckBorderBrush { get => GetValue(CheckBorderBrushProperty); set => SetValue(CheckBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> CheckBorderBrushProperty
            = AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(CheckBorderBrush), Colors.Black.ToBrush());

        #endregion

        #region Border

        /// <summary>
        /// Get or set the thickness of the border of the check box.
        /// </summary>
        public Thickness CheckBorderThickness { get => GetValue(CheckBorderThicknessProperty); set => SetValue(CheckBorderThicknessProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckBorderThickness"/>. See the related property for details.</summary>
        public static readonly StyledProperty<Thickness> CheckBorderThicknessProperty
            = AvaloniaProperty.Register<CheckBox, Thickness>(nameof(CheckBorderThickness), new Thickness(1));

        /// <summary>
        /// Get or set the thickness of the border of the check box, while the check box's IsChecked property is true.
        /// </summary>
        public Thickness CheckBorderSelectionThickness { get => GetValue(CheckBorderSelectionThicknessProperty); set => SetValue(CheckBorderSelectionThicknessProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckBorderSelectionThickness"/>. See the related property for details.</summary>
        public static readonly StyledProperty<Thickness> CheckBorderSelectionThicknessProperty
            = AvaloniaProperty.Register<CheckBox, Thickness>(nameof(CheckBorderSelectionThickness), new Thickness(1));

        /// <summary>
        /// Get or set the corner radius of the check box.
        /// </summary>
        public CornerRadius CheckBorderCornerRadius { get => GetValue(CheckBorderCornerRadiusProperty); set => SetValue(CheckBorderCornerRadiusProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckBorderCornerRadius"/>. See the related property for details.</summary>
        public static readonly StyledProperty<CornerRadius> CheckBorderCornerRadiusProperty
            = AvaloniaProperty.Register<CheckBox, CornerRadius>(nameof(CheckBorderCornerRadius), new CornerRadius(0));

        #endregion


    }

    /// <summary>
    /// Represents the current state of a <see cref="CheckBox"/>.
    /// </summary>
    public enum CheckState
    {
        /// <summary>
        /// The CheckBox is unchecked. Nothing is displayed in the box.
        /// </summary>
        Unchecked = 0,
        /// <summary>
        /// The CheckBox is checked. A check mark is displayed in the box.
        /// </summary>
        Checked = 1,
        /// <summary>
        /// The CheckBox is indeterminate. A square is displayed in the box.
        /// </summary>
        Indeterminate = 2,
    }
}
