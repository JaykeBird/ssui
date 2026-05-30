using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using SolidShineUi.Utils;

namespace SolidShineUi.Ribbon
{
    /// <summary>
    /// A checkbox control to display in a <see cref="RibbonGroup"/>.
    /// </summary>
    [ContentProperty(nameof(Title))]
    public class RibbonCheckBox : ThemedControl, ISsuiButton, IRibbonItem
    {
        static RibbonCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonCheckBox), new FrameworkPropertyMetadata(typeof(RibbonCheckBox)));
        }

        /// <summary>
        /// Create a new CheckBox control.
        /// </summary>
        public RibbonCheckBox()
        {
            CommandBindings.Add(new CommandBinding(CheckBoxClickCommand, OnCheckBoxClick));

            //SetValue(BackgroundProperty, ColorsHelper.CreateFromHex("01FFFFFF").ToBrush());
            //SetValue(BorderBrushProperty, ColorsHelper.Black.ToBrush());

            // KeyboardNavigation.SetIsTabStop(this, true);

            MouseDown += UserControl_MouseDown;
            MouseUp += UserControl_MouseUp;
            TouchDown += UserControl_TouchDown;
            TouchUp += UserControl_TouchUp;
            StylusDown += UserControl_StylusDown;
            StylusUp += UserControl_StylusUp;

            KeyDown += UserControl_KeyDown;
            KeyUp += UserControl_KeyUp;
        }
        #region CheckBoxClick

        /// <summary>
        /// The command that activates when the box of the checkbox itself has been clicked.
        /// </summary>
        public static readonly RoutedCommand CheckBoxClickCommand = new RoutedCommand();

        /// <summary>
        /// The backing value for the <see cref="CheckBoxClick"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent CheckBoxClickEvent = CheckBox.CheckBoxClickEvent.AddOwner(typeof(RibbonCheckBox));

        /// <summary>
        /// An event that raises only when the checkbox itself is clicked.
        /// </summary>
        /// <remarks>
        /// When combined with <see cref="OnlyAllowCheckBoxClick"/>, this can limit the checkbox to only being checkable when the box itself is clicked,
        /// not just anywhere within the control. This could be useful if the <c>Content</c> can also interact with the mouse.
        /// </remarks>
        public event RoutedEventHandler CheckBoxClick
        {
            add { AddHandler(CheckBoxClickEvent, value); }
            remove { RemoveHandler(CheckBoxClickEvent, value); }
        }

        bool checkBoxClick = false;

        private void OnCheckBoxClick(object sender, ExecutedRoutedEventArgs e)
        {
            checkBoxClick = true;
            RoutedEventArgs re = new RoutedEventArgs(CheckBoxClickEvent);
            RaiseEvent(re);
            DoClick();
            checkBoxClick = false;
        }

        /// <summary>
        /// Gets or sets whether clicking should only occur when the checkbox's box is clicked, and not the rest of the control.
        /// </summary>
        [Category("Common")]
        public bool OnlyAllowCheckBoxClick { get => (bool)GetValue(OnlyAllowCheckBoxClickProperty); set => SetValue(OnlyAllowCheckBoxClickProperty, value); }

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty OnlyAllowCheckBoxClickProperty
            = CheckBox.OnlyAllowCheckBoxClickProperty.AddOwner(typeof(RibbonCheckBox));

        #endregion

        #region CheckState

        #region Routed Events

        /// <summary>
        /// The backing routed event object for <see cref="CheckChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent CheckChangedEvent = CheckBox.CheckChangedEvent.AddOwner(typeof(RibbonCheckBox));

        /// <summary>
        /// Raised when the CheckState property is changed, either to Checked, Indeterminate, or Unchecked.
        /// </summary>
        public event RoutedEventHandler CheckChanged
        {
            add { AddHandler(CheckChangedEvent, value); }
            remove { RemoveHandler(CheckChangedEvent, value); }
        }

        /// <summary>
        /// The backing routed event object for <see cref="Checked"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent CheckedEvent = CheckBox.CheckedEvent.AddOwner(typeof(RibbonCheckBox));

        /// <summary>
        /// Raised when the CheckState property is changed to Checked.
        /// </summary>
        public event RoutedEventHandler Checked
        {
            add { AddHandler(CheckedEvent, value); }
            remove { RemoveHandler(CheckedEvent, value); }
        }

        /// <summary>
        /// The backing routed event object for <see cref="Unchecked"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent UncheckedEvent = CheckBox.UncheckedEvent.AddOwner(typeof(RibbonCheckBox));

        /// <summary>
        /// Raised when the CheckState property is changed to Unchecked.
        /// </summary>
        public event RoutedEventHandler Unchecked
        {
            add { AddHandler(UncheckedEvent, value); }
            remove { RemoveHandler(UncheckedEvent, value); }
        }

        /// <summary>
        /// The backing routed event object for <see cref="Indeterminate"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent IndeterminateEvent = CheckBox.IndeterminateEvent.AddOwner(typeof(RibbonCheckBox));

        /// <summary>
        /// Raised when the CheckState property is changed to Indeterminate.
        /// </summary>
        public event RoutedEventHandler Indeterminate
        {
            add { AddHandler(IndeterminateEvent, value); }
            remove { RemoveHandler(IndeterminateEvent, value); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The backing value for the <see cref="IsChecked"/> dependency property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty = CheckBox.IsCheckedProperty.AddOwner(typeof(RibbonCheckBox),
            new PropertyMetadata(new PropertyChangedCallback((d, e) => d.PerformAs<RibbonCheckBox>((c) => c.CheckedChanged()))));

        /// <summary>
        /// The backing value for the <see cref="IsIndeterminate"/> dependency property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty IsIndeterminateProperty = CheckBox.IsIndeterminateProperty.AddOwner(typeof(RibbonCheckBox),
            new PropertyMetadata(false, new PropertyChangedCallback((d, e) => d.PerformAs<RibbonCheckBox>((c) => c.IndeterminateChanged()))));

        /// <summary>
        /// Get or set if the check box is checked (or indeterminate).
        /// </summary>
        /// <remarks>
        /// While <see cref="IsIndeterminate"/> is true, this will also be true. Check <see cref="IsIndeterminate"/> to differentiate between the checked or 
        /// indeterminate states, or use <see cref="CheckState"/> to get the state as an enum.
        /// </remarks>
        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        /// <summary>
        /// Get or set if the check box is in the Indeterminate state.
        /// </summary>
        /// <remarks>
        /// By default, when the user clicks on the checkbox, it'll only toggle between being checked or unchecked, and not set to the indeterminate state.
        /// This can be enabled by setting <see cref="TriStateClick"/> to <c>true</c>.
        /// <para/>
        /// While this property is true, <see cref="IsChecked"/> will also be true. Check this property to differentiate between the checked or 
        /// indeterminate states, or use <see cref="CheckState"/> to get the state as an enum.
        /// </remarks>
        public bool IsIndeterminate
        {
            get => (bool)GetValue(IsIndeterminateProperty);
            set => SetValue(IsIndeterminateProperty, value);
        }

        /// <summary>
        /// Get or set the state of the checkbox, via a CheckState enum. This describes the exact state of the checkbox.
        /// </summary>
        public CheckState CheckState { get => (CheckState)GetValue(CheckStateProperty); set => SetValue(CheckStateProperty, value); }

        /// <summary>The backing dependency property for <see cref="CheckState"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CheckStateProperty = CheckBox.CheckStateProperty.AddOwner(typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata(CheckState.Unchecked, (d, e) => d.PerformAs<RibbonCheckBox>((o) => o.OnCheckStateChanged(e))));

        private void OnCheckStateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is CheckState newVal) { }
            else
            {
                newVal = CheckState.Unchecked;
            }

            if (updateBoolValues)
            {
                switch (newVal)
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
            }
        }

        #endregion

        bool raiseChangedEvent = true;
        bool updateBoolValues = true;

        private void CheckedChanged()
        {
            if (!IsChecked)
            {
                raiseChangedEvent = false;
                updateBoolValues = false;
                IsIndeterminate = false;
                CheckState = CheckState.Unchecked;
                raiseChangedEvent = true;
                updateBoolValues = true;
            }
            else
            {
                if (IsIndeterminate)
                {
                    updateBoolValues = false;
                    CheckState = CheckState.Indeterminate;
                    updateBoolValues = true;
                }
                else
                {
                    updateBoolValues = false;
                    CheckState = CheckState.Checked;
                    updateBoolValues = true;
                }
            }

            if (raiseChangedEvent)
            {
                RoutedEventArgs t = new RoutedEventArgs(CheckChangedEvent);
                RaiseEvent(t);
                IsSelectedChanged?.Invoke(this, new ItemSelectionChangedEventArgs(CheckChangedEvent, false, IsChecked, SelectionChangeTrigger.CodeUnknown));
                RaiseCheckedEvent();
            }
        }

        private void IndeterminateChanged()
        {

            if (IsIndeterminate)
            {
                raiseChangedEvent = false;
                updateBoolValues = false;
                IsChecked = true;
                CheckState = CheckState.Indeterminate;
                raiseChangedEvent = true;
                updateBoolValues = true;
            }
            else
            {
                if (IsChecked)
                {
                    updateBoolValues = false;
                    CheckState = CheckState.Checked;
                    updateBoolValues = true;
                }
                else
                {
                    updateBoolValues = false;
                    CheckState = CheckState.Unchecked;
                    updateBoolValues = true;
                }
            }

            if (raiseChangedEvent)
            {
                RoutedEventArgs t = new RoutedEventArgs(CheckChangedEvent);
                RaiseEvent(t);
                IsSelectedChanged?.Invoke(this, new ItemSelectionChangedEventArgs(CheckChangedEvent, false, IsChecked, SelectionChangeTrigger.CodeUnknown));
                RaiseCheckedEvent();
            }
        }

        void RaiseCheckedEvent()
        {
            RoutedEventArgs t;

            switch (CheckState)
            {
                case CheckState.Unchecked:
                    t = new RoutedEventArgs(UncheckedEvent);
                    break;
                case CheckState.Checked:
                    t = new RoutedEventArgs(CheckedEvent);
                    break;
                case CheckState.Indeterminate:
                    t = new RoutedEventArgs(IndeterminateEvent);
                    break;
                default:
                    return;
            }

            RaiseEvent(t);
        }

        #endregion

        #region Color Scheme / SsuiTheme

        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif


        /// <summary>
        /// A dependency property object backing the related ColorScheme property. See <see cref="ColorScheme"/> for more details.
        /// </summary>
        public static readonly DependencyProperty ColorSchemeProperty = CheckBox.ColorSchemeProperty.AddOwner(typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata(new ColorScheme(), OnColorSchemeChanged));

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            ColorScheme cs = (e.NewValue as ColorScheme)!;
#else
            ColorScheme cs = e.NewValue as ColorScheme;
#endif

            if (d is RibbonCheckBox c)
            {
                c.ColorSchemeChanged?.Invoke(d, e);
                c.ApplyColorScheme(cs);
            }
        }

        /// <summary>
        /// Get or set the color scheme used for this checkbox. For easier color scheme management, bind this to the window or larger control you're using.
        /// </summary>
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
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
                BackgroundDisabledBrush = cs.BackgroundColor.ToBrush();
                CheckForeground = ColorsHelper.Black.ToBrush();
                CheckHighlightBrush = ColorsHelper.Black.ToBrush();
                HighlightBrush = cs.HighlightColor.ToBrush();
                BorderHighlightBrush = cs.BorderColor.ToBrush();
            }
            else
            {
                BackgroundDisabledBrush = cs.LightDisabledColor.ToBrush();
                CheckForeground = Colors.Black.ToBrush();
                CheckHighlightBrush = ColorsHelper.DarkerGray.ToBrush();
                HighlightBrush = ColorsHelper.WhiteLightHighlight.ToBrush();
                BorderHighlightBrush = cs.HighlightColor.ToBrush();
            }

            BorderBrush = cs.BorderColor.ToBrush();
            BackgroundDisabledBrush = cs.LightDisabledColor.ToBrush();
            BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
            CheckDisabledBrush = cs.DarkDisabledColor.ToBrush();
            Foreground = cs.ForegroundColor.ToBrush();
            BorderSelectedBrush = cs.BorderColor.ToBrush();
        }

        /// <inheritdoc/>
        protected override void OnApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {
            base.OnApplySsuiTheme(ssuiTheme, useLightBorder, useAccentTheme);

            if (useAccentTheme && ssuiTheme is SsuiAppTheme ssuiAppTheme)
            {
                ApplyTheme(ssuiAppTheme.AccentTheme);
            }
            else
            {
                ApplyTheme(ssuiTheme);
            }

            void ApplyTheme(SsuiTheme theme)
            {
                ApplyThemeBinding(ForegroundProperty, SsuiTheme.ForegroundProperty, theme);
                // Border brush already applied in base
                ApplyThemeBinding(CheckForegroundProperty, SsuiTheme.CheckBrushProperty, theme);
                ApplyThemeBinding(CheckHighlightBrushProperty, SsuiTheme.CheckHighlightBrushProperty, theme);
                ApplyThemeBinding(BorderHighlightBrushProperty, SsuiTheme.HighlightBorderBrushProperty, theme);
                ApplyThemeBinding(BorderSelectedBrushProperty, SsuiTheme.SelectedBorderBrushProperty, theme);
                ApplyThemeBinding(HighlightBrushProperty, SsuiTheme.CheckBackgroundHighlightBrushProperty, theme);

                ApplyThemeBinding(BackgroundDisabledBrushProperty, SsuiTheme.DisabledBackgroundProperty, theme);
                ApplyThemeBinding(BorderDisabledBrushProperty, SsuiTheme.DisabledBorderBrushProperty, theme);
                ApplyThemeBinding(CheckDisabledBrushProperty, SsuiTheme.DisabledForegroundProperty, theme);

                ApplyThemeBinding(CornerRadiusProperty, SsuiTheme.CornerRadiusProperty, theme);
            }
        }

        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the brush used for the background of the checkbox's box. This value is not modified by the <see cref="ColorScheme"/> 
        /// or <see cref="ThemedContentControl.SsuiTheme"/> property.
        /// </summary>
        public Brush CheckBackground { get => (Brush)GetValue(CheckBackgroundProperty); set => SetValue(CheckBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="CheckBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CheckBackgroundProperty = CheckBox.CheckBackgroundProperty.AddOwner(typeof(RibbonCheckBox));

        /// <summary>
        /// Get or set the brush used for the check mark in the checkbox's box.
        /// </summary>
        public Brush CheckForeground { get => (Brush)GetValue(CheckForegroundProperty); set => SetValue(CheckForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="CheckForeground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CheckForegroundProperty = CheckBox.CheckForegroundProperty.AddOwner(typeof(RibbonCheckBox));

        /// <summary>
        /// Get or set the brush used for the check mark in the checkbox's box, while the mouse is over the control or it has keyboard focus. 
        /// </summary>
        public Brush CheckHighlightBrush { get => (Brush)GetValue(CheckHighlightBrushProperty); set => SetValue(CheckHighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="CheckHighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CheckHighlightBrushProperty = CheckBox.CheckHighlightBrushProperty.AddOwner(typeof(RibbonCheckBox));


        /// <summary>
        /// Get or set the brush used for the background of the checkbox's box, while the mouse is over the control or it has keyboard focus.
        /// </summary>
        public Brush HighlightBrush { get => (Brush)GetValue(HighlightBrushProperty); set => SetValue(HighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HighlightBrushProperty = CheckBox.HighlightBrushProperty.AddOwner(typeof(RibbonCheckBox));

        /// <summary>
        /// Get or set the brush to use for the background of the checkbox's box when it is disabled.
        /// </summary>
        public Brush BackgroundDisabledBrush { get => (Brush)GetValue(BackgroundDisabledBrushProperty); set => SetValue(BackgroundDisabledBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="BackgroundDisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BackgroundDisabledBrushProperty = CheckBox.BackgroundDisabledBrushProperty.AddOwner(typeof(RibbonCheckBox));

        /// <summary>
        /// Get or set the brush used for the border of the checkbox's box when it is disabled.
        /// </summary>
        public Brush BorderDisabledBrush { get => (Brush)GetValue(BorderDisabledBrushProperty); set => SetValue(BorderDisabledBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="BorderDisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderDisabledBrushProperty = CheckBox.BorderDisabledBrushProperty.AddOwner(typeof(RibbonCheckBox));

        /// <summary>
        /// Get or set the brush used for the check mark when the control is disabled.
        /// </summary>
        public Brush CheckDisabledBrush { get => (Brush)GetValue(CheckDisabledBrushProperty); set => SetValue(CheckDisabledBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="CheckDisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CheckDisabledBrushProperty = CheckBox.CheckDisabledBrushProperty.AddOwner(typeof(RibbonCheckBox));

        /// <summary>
        /// Get or set the brush used for the border of the checkbox's box, while the mouse is over the control or it has keyboard focus.
        /// </summary>
        public Brush BorderHighlightBrush { get => (Brush)GetValue(BorderHighlightBrushProperty); set => SetValue(BorderHighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="BorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderHighlightBrushProperty = CheckBox.BorderHighlightBrushProperty.AddOwner(typeof(RibbonCheckBox));

        /// <summary>
        /// Get or set the brush used for the border of the checkbox's box, while <see cref="IsChecked"/> is set to true.
        /// </summary>
        public Brush BorderSelectedBrush { get => (Brush)GetValue(BorderSelectedBrushProperty); set => SetValue(BorderSelectedBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="BorderSelectedBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderSelectedBrushProperty = CheckBox.BorderSelectedBrushProperty.AddOwner(typeof(RibbonCheckBox));

        #endregion

        #region Border

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty BorderSelectionThicknessProperty = CheckBox.BorderSelectionThicknessProperty.AddOwner(typeof(RibbonCheckBox),
            new PropertyMetadata(defaultValue: 1));

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = CheckBox.CornerRadiusProperty.AddOwner(typeof(RibbonCheckBox));

        /// <summary>
        /// Get or set the thickness of the border of the check box, while the check box's IsChecked property is true.
        /// </summary>
        [Category("Appearance")]
        public Thickness BorderSelectionThickness
        {
            get => (Thickness)GetValue(BorderSelectionThicknessProperty);
            set => SetValue(BorderSelectionThicknessProperty, value);
        }

        /// <summary>
        /// Get or set the corner radius of the check box.
        /// </summary>
        [Category("Appearance")]
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #endregion

        #region Click Handling

        #region Routed Events

        /// <summary>
        /// The backing value for the <see cref="Click"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent ClickEvent = CheckBox.ClickEvent.AddOwner(typeof(RibbonCheckBox));

        /// <summary>
        /// Raised when the check box is clicked.
        /// </summary>
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        /// <summary>
        /// The backing value for the <see cref="RightClick"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent RightClickEvent = CheckBox.RightClickEvent.AddOwner(typeof(RibbonCheckBox));

        /// <summary>
        /// Raised when the check box is right-clicked.
        /// </summary>
        public event RoutedEventHandler RightClick
        {
            add { AddHandler(RightClickEvent, value); }
            remove { RemoveHandler(RightClickEvent, value); }
        }

        #endregion

        #region Variables/Properties

        bool initiatingClick = false;
        /// <summary>
        /// Gets or sets whether the Click event should be raised when the checkbox is pressed, rather than when it is released.
        /// </summary>
        [Category("Common")]
        public bool ClickOnPress { get; set; } = false;

        /// <summary>
        /// Gets or sets whether the checkbox should cycle through three states (rather than two) when clicked. 
        /// The third state is the "Indeterminate" state, which can be checked via <see cref="IsIndeterminate"/> or <see cref="CheckState"/>.
        /// </summary>
        [Category("Common")]
        public bool TriStateClick { get => (bool)GetValue(TriStateClickProperty); set => SetValue(TriStateClickProperty, value); }

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty TriStateClickProperty = CheckBox.TriStateClickProperty.AddOwner(typeof(RibbonCheckBox));

        #endregion

        /// <summary>
        /// Sets up the button to be clicked. This must be run before PerformClick.
        /// </summary>
        /// <param name="rightClick">Determine whether this should be treated as a right click (which usually invokes a context menu).</param>
        void PerformPress(bool rightClick = false)
        {
            initiatingClick = true;

            if (ClickOnPress)
            {
                PerformClick(rightClick);
            }
        }

        /// <summary>
        /// If the button is prepared by PerformPress, perform the Click actions, including raising the Click event.
        /// </summary>
        /// <param name="rightClick">Determine whether this should be treated as a right click (which usually invokes a context menu).</param>
        void PerformClick(bool rightClick = false)
        {
            if (initiatingClick)
            {
                if (rightClick)
                {
                    RoutedEventArgs rre = new RoutedEventArgs(RightClickEvent);
                    RaiseEvent(rre);
                    initiatingClick = false;
                    return;
                }

                if (OnlyAllowCheckBoxClick && !checkBoxClick)
                {
                    // exit out
                    initiatingClick = false;
                    return;
                }

                if (TriStateClick)
                {
                    if (IsChecked && IsIndeterminate)
                    {
                        IsChecked = false;
                    }
                    else if (IsChecked && !IsIndeterminate)
                    {
                        IsIndeterminate = true;
                    }
                    else
                    {
                        IsChecked = true;
                        IsIndeterminate = false;
                    }
                }
                else
                {
                    IsChecked = !IsChecked;
                }

                RoutedEventArgs re = new RoutedEventArgs(ClickEvent);
                RaiseEvent(re);
                initiatingClick = false;
            }
        }

        /// <summary>
        /// Perform a click programmatically. The checkbox responds the same way as if it was clicked by the user.
        /// </summary>
        public void DoClick()
        {
            PerformPress();
            PerformClick();
        }

        #region Event Handlers

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PerformPress(e.ChangedButton == MouseButton.Right);
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            PerformClick(e.ChangedButton == MouseButton.Right);
            e.Handled = true;
        }

#if NETCOREAPP
        private void UserControl_TouchDown(object? sender, TouchEventArgs e)
        {
            PerformPress();
        }

        private void UserControl_TouchUp(object? sender, TouchEventArgs e)
        {
            PerformClick();
        }

#else
        private void UserControl_TouchDown(object sender, TouchEventArgs e)
        {
            PerformPress();
        }

        private void UserControl_TouchUp(object sender, TouchEventArgs e)
        {
            PerformClick();
        }
#endif

        private void UserControl_StylusDown(object sender, StylusDownEventArgs e)
        {
            PerformPress();
        }

        private void UserControl_StylusUp(object sender, StylusEventArgs e)
        {
            PerformClick();
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                PerformPress();
            }
        }

        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                // special handling for OnlyAllowCheckBoxClick, to allow this to be clicked via the keyboard
                if (OnlyAllowCheckBoxClick) checkBoxClick = true;
                PerformClick();
                if (OnlyAllowCheckBoxClick) checkBoxClick = false;
            }
            else if (e.Key == Key.Apps)
            {
                PerformClick(true);
            }
        }

        #endregion

        #endregion

        #region IRibbonItem implementations

        /// <summary>
        /// Get or set the size to use for this control. 
        /// For <see cref="RibbonCheckBox"/>, the only valid values are <c>Small</c> and <c>IconOnly</c>; all other values will be treated as <c>Small</c>.
        /// </summary>
        /// <remarks>
        /// When the parent group is compacted, it'll request all controls within the group to use its <see cref="CompactSize"/> instead of its <see cref="StandardSize"/>.
        /// </remarks>
        public RibbonElementSize StandardSize { get => (RibbonElementSize)GetValue(StandardSizeProperty); set => SetValue(StandardSizeProperty, value); }

        /// <summary>The backing dependency property for <see cref="StandardSize"/>. See the related property for details.</summary>
        public static DependencyProperty StandardSizeProperty
            = DependencyProperty.Register("StandardSize", typeof(RibbonElementSize), typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata(RibbonElementSize.Large));


        /// <summary>
        /// Get or set the size to use for this control, when the parent group is being compacted.
        /// For <see cref="RibbonCheckBox"/>, the only valid values are <c>Small</c> and <c>IconOnly</c>; all other values will be treated as <c>Small</c>.
        /// </summary>
        /// <remarks>
        /// When the parent group is compacted, it'll request all controls within the group to use its <see cref="CompactSize"/> instead of its <see cref="StandardSize"/>.
        /// For important and commonly used controls in a group, the <c>CompactSize</c> may still be same type as the <c>StandardSize</c>, but for less important or 
        /// more infrequently used controls, it's recommended to go down a size value for <c>CompactSize</c>.
        /// </remarks>
        public RibbonElementSize CompactSize { get => (RibbonElementSize)GetValue(CompactSizeProperty); set => SetValue(CompactSizeProperty, value); }

        /// <summary>The backing dependency property for <see cref="CompactSize"/>. See the related property for details.</summary>
        public static DependencyProperty CompactSizeProperty
            = DependencyProperty.Register("CompactSize", typeof(RibbonElementSize), typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata(RibbonElementSize.Small));
        
        /// <inheritdoc/>
        public string AccessKey { get => (string)GetValue(AccessKeyProperty); set => SetValue(AccessKeyProperty, value); }

        /// <summary>The backing dependency property for <see cref="AccessKey"/>. See the related property for details.</summary>
        public static DependencyProperty AccessKeyProperty
            = DependencyProperty.Register("AccessKey", typeof(string), typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata("C"));

        /// <inheritdoc/>
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        /// <summary>The backing dependency property for <see cref="Title"/>. See the related property for details.</summary>
        public static DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(string), typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata("Check"));

        /// <inheritdoc/>
        public ImageSource LargeIcon { get => (ImageSource)GetValue(LargeIconProperty); set => SetValue(LargeIconProperty, value); }

        /// <summary>The backing dependency property for <see cref="LargeIcon"/>. See the related property for details.</summary>
        public static DependencyProperty LargeIconProperty
            = DependencyProperty.Register("LargeIcon", typeof(ImageSource), typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata(null));

        /// <inheritdoc/>
        public ImageSource SmallIcon { get => (ImageSource)GetValue(SmallIconProperty); set => SetValue(SmallIconProperty, value); }

        /// <summary>The backing dependency property for <see cref="SmallIcon"/>. See the related property for details.</summary>
        public static DependencyProperty SmallIconProperty
            = DependencyProperty.Register("SmallIcon", typeof(ImageSource), typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata(null));

        /// <inheritdoc/>
        public bool IsCompacted { get => (bool)GetValue(IsCompactedProperty); set => SetValue(IsCompactedProperty, value); }

        /// <summary>The backing dependency property for <see cref="IsCompacted"/>. See the related property for details.</summary>
        public static DependencyProperty IsCompactedProperty
            = DependencyProperty.Register("IsCompacted", typeof(bool), typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata(false));

        /// <inheritdoc/>
        public int CompactOrder { get => (int)GetValue(CompactOrderProperty); set => SetValue(CompactOrderProperty, value); }

        /// <summary>The backing dependency property for <see cref="CompactOrder"/>. See the related property for details.</summary>
        public static DependencyProperty CompactOrderProperty
            = DependencyProperty.Register("CompactOrder", typeof(int), typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata(0));

        #endregion

        #region ISsuiButton implementations

        /// <summary>
        /// Raised when <see cref="CheckState"/> is changed. Using <see cref="CheckChanged"/> rather than this event is recommended instead.
        /// </summary>
#if NETCOREAPP
        public event ItemSelectionChangedEventHandler? IsSelectedChanged;
#else
        public event ItemSelectionChangedEventHandler IsSelectedChanged;
#endif

        Brush ISsuiButton.DisabledBrush { get => BackgroundDisabledBrush; set => BackgroundDisabledBrush = value; }
        Brush ISsuiButton.HighlightForeground { get => CheckHighlightBrush; set => CheckHighlightBrush = value; }
        Brush ISsuiButton.SelectedForeground { get => CheckForeground; set => CheckForeground = value; }
        bool ISsuiButton.TransparentBack { get; set; } = true;
        bool ISsuiButton.HighlightOnKeyboardFocus { get; set; } = true;

#if NETCOREAPP
        object? ISsuiButton.Content
#else
        object ISsuiButton.Content
#endif
        {
            get { return Title; }
            set
            {
                if (value == null)
                {
                    Title = "";
                }
                if (value is string s)
                {
                    Title = s;
                }
                else if (value is IFormattable f)
                {
                    Title = f.ToString((this as ISsuiButton).ContentStringFormat, CultureInfo.CurrentCulture);
                }
                else
                {
                    Title = value?.ToString() ?? "";
                }
            }
        }

        string ISsuiButton.ContentStringFormat { get; set; } = "g";

#if NETCOREAPP
        DataTemplate? ISsuiButton.ContentTemplate { get; set; } = null; // matching WPF default behavior
#else
        DataTemplate ISsuiButton.ContentTemplate { get; set; } = null;
#endif
        bool IClickSelectableControl.IsSelected { get => IsChecked; set => IsChecked = value; }
        bool IClickSelectableControl.SelectOnClick { get; set; } = true;
        Brush IClickSelectableControl.ClickBrush { get => CheckHighlightBrush; set => CheckHighlightBrush = value; }
        Brush IClickSelectableControl.SelectedBrush { get => BorderSelectedBrush; set => BorderSelectedBrush = value; }

#if NETCOREAPP
        void IClickSelectableControl.SetIsSelectedWithSource(bool value, SelectionChangeTrigger trigger, object? triggerSource)
        {
            IsChecked = value;
        }
#else
        void IClickSelectableControl.SetIsSelectedWithSource(bool value, SelectionChangeTrigger trigger, object triggerSource)
        {
            IsChecked = value;
        }
#endif

#endregion

    }
}
