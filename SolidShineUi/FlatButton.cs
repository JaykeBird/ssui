using SolidShineUi.Utils;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace SolidShineUi
{
    /// <summary>
    /// A button with a custom, flat style and additional funcionality. Use <c>SelectOnClick</c> to have the button act like a toggle button.
    /// </summary>
    [DefaultEvent("Click")]
    [Localizability(LocalizationCategory.Button)]
    public class FlatButton : ButtonBase, IClickSelectableControl
    {
        static FlatButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlatButton), new FrameworkPropertyMetadata(typeof(FlatButton)));
        }

        /// <summary>
        /// Create a new FlatButton.
        /// </summary>
        public FlatButton()
        {
            MouseDown += UserControl_MouseDown;
            MouseUp += UserControl_MouseUp;
            MouseLeave += UserControl_MouseLeave;

            PreviewMouseDown += UserControl_PreviewMouseDown;
            PreviewMouseUp += UserControl_PreviewMouseUp;

            LostFocus += UserControl_LostFocus;
            LostKeyboardFocus += UserControl_LostKeyboardFocus;

            KeyDown += UserControl_KeyDown;
            KeyUp += UserControl_KeyUp;

            //KeyboardNavigation.SetIsTabStop(this, true);
        }

        #region Brushes

        /// <summary>
        /// Get or set the brush used for the background of the control while the mouse is clicking it.
        /// </summary>
        [Category("Brushes")]
        public Brush ClickBrush
        {
            get => (Brush)GetValue(ClickBrushProperty);
            set => SetValue(ClickBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of this button while it is selected
        /// (i.e. the <c>IsSelected</c> property is true).
        /// </summary>
        [Category("Brushes")]
        public Brush SelectedBrush
        {
            get => (Brush)GetValue(SelectedBrushProperty);
            set => SetValue(SelectedBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of the control while the mouse is over it, or it has keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public Brush HighlightBrush
        {
            get => (Brush)GetValue(HighlightBrushProperty);
            set => SetValue(HighlightBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the foreground while the control has the mouse over it, or it has keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public Brush ForegroundHighlightBrush
        {
            get => (Brush)GetValue(BorderHighlightBrushProperty);
            set => SetValue(BorderHighlightBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of the control when the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush DisabledBrush
        {
            get => (Brush)GetValue(DisabledBrushProperty);
            set => SetValue(DisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border of the control when the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush BorderDisabledBrush
        {
            get => (Brush)GetValue(BorderDisabledBrushProperty);
            set => SetValue(BorderDisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border while the control has the mouse over it, or it has keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public Brush BorderHighlightBrush
        {
            get => (Brush)GetValue(BorderHighlightBrushProperty);
            set => SetValue(BorderHighlightBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border while the control is selected
        /// (i.e. the <c>IsSelected</c> property is true).
        /// </summary>
        [Category("Brushes")]
        public Brush BorderSelectedBrush
        {
            get => (Brush)GetValue(BorderSelectedBrushProperty);
            set => SetValue(BorderSelectedBrushProperty, value);
        }

        /// <summary>The backing dependency property for <see cref="ClickBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ClickBrushProperty = DependencyProperty.Register(
            nameof(ClickBrush), typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.Gainsboro.ToBrush()));

        /// <summary>The backing dependency property for <see cref="SelectedBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
            nameof(SelectedBrush), typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.WhiteSmoke.ToBrush()));

        /// <summary>The backing dependency property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            nameof(HighlightBrush), typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.LightGray.ToBrush()));

        /// <summary>The backing dependency property for <see cref="ForegroundHighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ForegroundHighlightBrushProperty = DependencyProperty.Register(
            nameof(ForegroundHighlightBrush), typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>The backing dependency property for <see cref="DisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty DisabledBrushProperty = DependencyProperty.Register(
            nameof(DisabledBrush), typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.Gray.ToBrush()));

        /// <summary>The backing dependency property for <see cref="BorderDisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            nameof(BorderDisabledBrush), typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.DarkGray.ToBrush()));

        /// <summary>The backing dependency property for <see cref="BorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderHighlightBrushProperty = DependencyProperty.Register(
            nameof(BorderHighlightBrush), typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>The backing dependency property for <see cref="BorderSelectedBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderSelectedBrushProperty = DependencyProperty.Register(
            nameof(BorderSelectedBrush), typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.DimGray.ToBrush()));

        #endregion

        #region TransparentBack

        /// <summary>
        /// The backing dependency property for <see cref="TransparentBack"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty TransparentBackProperty
            = DependencyProperty.Register(nameof(TransparentBack), typeof(bool), typeof(FlatButton), new PropertyMetadata(false));

        /// <summary>
        /// Get or set whether the button should have a transparent background when the button is not focused or selected.
        /// </summary>
        [Category("Common")]
        [Description("Get or set whether the button should have a transparent background when the button is not focused or selected.")]
        public bool TransparentBack
        {
            get => (bool)GetValue(TransparentBackProperty);
            set => SetValue(TransparentBackProperty, value);
        }

        #endregion

        #region SsuiTheme

        /// <summary>
        /// Get or set the theme to apply to the appearance of this control. This will bind the control's brushes to this theme.
        /// </summary>
        /// <remarks>
        /// This theme can be used to centrally apply a consistent appearance to all ThemedControls. Once a theme is set, the control's brushes are bound to the relevant
        /// values in this SsuiTheme, allowing you to update the control's appearance by changing the relevant values in the SsuiTheme. 
        /// This has the downside of overwriting any preset brush values set via XAML; to circumvent this, listen to the <see cref="SsuiThemeChanged"/> event and 
        /// then reapply the brushes in that event handler, or include that brush property's name in this control's <see cref="ThemeValueExclude"/>.
        /// <para/>
        /// This is set to null by default, so that brushes are not automatically bound to any SsuiTheme (allowing you to set the brushes via XAML).
        /// </remarks>
        [Category("Appearance")]
#if NETCOREAPP
        public SsuiTheme? SsuiTheme { get => (SsuiTheme)GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }
#else
        public SsuiTheme SsuiTheme { get => (SsuiTheme)GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }
#endif

        /// <summary>The backing dependency property for <see cref="SsuiTheme"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SsuiThemeProperty = ThemedControl.SsuiThemeProperty.AddOwner(typeof(FlatButton),
            new FrameworkPropertyMetadata(OnSsuiThemeChanged));

        /// <summary>
        /// The backing routed event object for <see cref="SsuiThemeChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent SsuiThemeChangedEvent = ThemedControl.SsuiThemeChangedEvent.AddOwner(typeof(FlatButton));

        /// <summary>
        /// Raised after the <see cref="SsuiTheme"/> property has been changed and applied.
        /// </summary>
        public event RoutedEventHandler SsuiThemeChanged
        {
            add { AddHandler(SsuiThemeChangedEvent, value); }
            remove { RemoveHandler(SsuiThemeChangedEvent, value); }
        }

        private static void OnSsuiThemeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is FlatButton tc)
            {
                tc.ApplyAndRaiseTheme();
            }
        }

        /// <summary>
        /// Update the control's appearance by using <see cref="ApplySsuiTheme(SsuiTheme)"/> and then raise the <see cref="SsuiThemeChanged"/> event.
        /// </summary>
        protected internal void ApplyAndRaiseTheme()
        {
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);

            RoutedEventArgs re = new RoutedEventArgs(SsuiThemeChangedEvent, this);
            RaiseEvent(re);
        }

        #region UseLightBorder

        /// <summary>
        /// Get or set if the <see cref="SsuiTheme.LightBorderBrush"/> should be used for the border of this control, rather than the <see cref="SsuiTheme.BorderBrush"/>.
        /// </summary>
        public bool UseLightBorder { get => (bool)GetValue(UseLightBorderProperty); set => SetValue(UseLightBorderProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseLightBorder"/>. See the related property for details.</summary>
        public static readonly DependencyProperty UseLightBorderProperty = ThemedControl.UseLightBorderProperty.AddOwner(typeof(FlatButton),
            new FrameworkPropertyMetadata(false, (d, e) => d.PerformAs<FlatButton>((o) => o.OnUseLightBorderChange(o, e))));

        void OnUseLightBorderChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_skipReapply) return;
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);
        }

        #endregion

        #region UseAccentTheme

        /// <summary>
        /// Get or set if an accent theme should be used rather than the standard theme for this control. The accent theme, when used in moderation,
        /// can be used to help particular controls stand out in the UI.
        /// </summary>
        /// <remarks>
        /// The value of the <see cref="SsuiTheme"/> property needs to be of an <see cref="SsuiAppTheme"/> type, or otherwise this property does nothing.
        /// This should already be taken care of if you're putting this control in a <see cref="ThemedWindow"/> and inheriting from that SsuiTheme property.
        /// </remarks>
        public bool UseAccentTheme { get => (bool)GetValue(UseAccentThemeProperty); set => SetValue(UseAccentThemeProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseAccentTheme"/>. See the related property for details.</summary>
        public static readonly DependencyProperty UseAccentThemeProperty = ThemedControl.UseAccentThemeProperty.AddOwner(typeof(FlatButton),
            new FrameworkPropertyMetadata(false, (d, e) => d.PerformAs<FlatButton>((o) => o.OnUseAccentThemeChange(o, e))));

        void OnUseAccentThemeChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (SsuiTheme != null)
            {
                if (_skipReapply) return;
                ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);
            }
            else
            {
                if (!runApply) return;
                ApplyColorScheme(ColorScheme, UseAccentTheme);
            }
        }

        #endregion

        #region ThemeValueExclude

        /// <summary>
        /// Get or set the properties to not apply the theme values to when applying a SsuiTheme to this control.
        /// <para/>
        /// Use a comma-separated list for multiple properties; for each property, the property's value will not be changed when the SsuiTheme is applied.
        /// </summary>
        /// <remarks>
        /// This can be used when you have certain brushes or values set for a particular control and applying a SsuiTheme ends up resetting that value.
        /// If only one property's value needs to be kept unchanged, then just set this to the name of that property. If multiple properties need this,
        /// then provide this as a comma-separated list of all the properties' names in a single string (e.g., <c>"Background,Foreground,BorderBrush"</c>).
        /// When the control is applying a SsuiTheme (such as set via a property directly, or inheriting from a parent), the control will skip over setting
        /// properties that match one of the names on this list.
        /// <para/>
        /// Note that this functionality is designed for comparing dependency properties' names (<see cref="DependencyProperty.Name"/>) 
        /// to the list provided here. This will also likely not have any effect on any property that isn't a dependency property.
        /// </remarks>
        public string ThemeValueExclude { get => (string)GetValue(ThemeValueExcludeProperty); set => SetValue(ThemeValueExcludeProperty, value); }

        /// <summary>The backing dependency property for <see cref="ThemeValueExclude"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ThemeValueExcludeProperty = ThemedControl.ThemeValueExcludeProperty.AddOwner(typeof(FlatButton));

        #endregion

        #region ApplySsuiTheme

        bool _skipReapply = false;

        /// <summary>
        /// Check the control's properties to make sure they match these values. If not, this will update the properties and then re-call the ApplySsuiTheme function.
        /// </summary>
        /// <param name="ssuiTheme">the inputted SsuiTheme value</param>
        /// <param name="useLightBorder">the inputted UseLightBorder value</param>
        /// <param name="useAccentTheme">the inputted UseAccentTheme value</param>
#if NETCOREAPP
        bool CheckAndUpdateProperties(SsuiTheme? ssuiTheme, bool useLightBorder, bool useAccentTheme)
#else
        bool CheckAndUpdateProperties(SsuiTheme ssuiTheme, bool useLightBorder, bool useAccentTheme)
#endif
        {
            _skipReapply = true;

            if (UseLightBorder != useLightBorder) UseLightBorder = useLightBorder;
            if (UseAccentTheme != useAccentTheme) UseAccentTheme = useAccentTheme;

            _skipReapply = false;

            if (ssuiTheme != SsuiTheme)
            {
                SsuiTheme = ssuiTheme;
                return false; // ApplySsuiTheme will be called again once the property is changed
            }

            return true;
        }

        /// <summary>
        /// Apply a <see cref="SsuiTheme"/> to this control. This applies a consistent appearance to this control and child controls.
        /// </summary>
        /// <param name="ssuiTheme">the theme value to apply</param>
        public void ApplySsuiTheme(SsuiTheme ssuiTheme)
        {
            ApplySsuiTheme(ssuiTheme, UseLightBorder, UseAccentTheme);
        }

        /// <summary>
        /// Apply a <see cref="SsuiTheme"/> to this control. This applies a consistent appearance to this control and child controls.
        /// </summary>
        /// <param name="ssuiTheme">the theme value to apply</param>
        /// <param name="useLightBorder">whether a light border color should be used in place of the standard one</param>
        /// <param name="useAccentTheme">
        /// whether the accent theme should be used in place of the standard one; for this to apply, <paramref name="ssuiTheme"/>
        /// should be of type <see cref="SsuiAppTheme"/>
        /// </param>
#if NETCOREAPP
        public void ApplySsuiTheme(SsuiTheme? ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
#else
        public void ApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
#endif
        {
            if (!CheckAndUpdateProperties(ssuiTheme, useLightBorder, useAccentTheme)) return;

            if (ssuiTheme != null) OnApplySsuiTheme(ssuiTheme, useLightBorder, useAccentTheme);

            RoutedEventArgs re = new RoutedEventArgs(SsuiThemeAppliedEvent, this);
            RaiseEvent(re);
        }

        /// <summary>
        /// Clear the existing SsuiTheme applied to this control and remove all bindings. This will reset the control's appearance.
        /// </summary>
        public void ClearSsuiTheme()
        {
            SsuiTheme = null;

            // maybe I should update OnApplySsuiTheme to show that it can support null values, but for now, I'm doing this lol
#if NETCOREAPP
            OnApplySsuiTheme(null!);
#else
            OnApplySsuiTheme(null);
#endif
        }

        /// <summary>
        /// Raised when the SsuiTheme value is changed, or when <see cref="ApplySsuiTheme(SsuiTheme, bool, bool)"/> is called.
        /// Inheriting controls should override this to update their brushes to reflect this new theme.
        /// </summary>
        /// <param name="ssuiTheme">the theme value to apply</param>
        /// <param name="useLightBorder">whether a light border color should be used in place of the standard one</param>
        /// <param name="useAccentTheme">
        /// whether the accent theme should be used in place of the standard one; for this to apply, <paramref name="ssuiTheme"/>
        /// should be of type <see cref="SsuiAppTheme"/>
        /// </param>
        /// <remarks>
        /// The base method will by default set the <see cref="Control.BorderBrush"/> value to match the theme; if a different
        /// border brush is needed, then update it again in your override method.
        /// </remarks>
        protected virtual void OnApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {
            if (useAccentTheme && ssuiTheme is SsuiAppTheme sat)
            {
                ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty, sat.AccentTheme);
                ApplyTheme(sat.AccentTheme);
            }
            else
            {
                ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty);
                ApplyTheme(ssuiTheme);
            }

            void ApplyTheme(SsuiTheme theme)
            {
                ApplyThemeBinding(BackgroundProperty, SsuiTheme.ButtonBackgroundProperty, theme);
                ApplyThemeBinding(HighlightBrushProperty, SsuiTheme.HighlightBrushProperty, theme);
                ApplyThemeBinding(DisabledBrushProperty, SsuiTheme.DisabledBackgroundProperty, theme);
                ApplyThemeBinding(BorderDisabledBrushProperty, SsuiTheme.DisabledBorderBrushProperty, theme);
                ApplyThemeBinding(SelectedBrushProperty, SsuiTheme.SelectedBackgroundBrushProperty, theme);
                ApplyThemeBinding(BorderHighlightBrushProperty, SsuiTheme.HighlightBorderBrushProperty, theme);
                ApplyThemeBinding(BorderSelectedBrushProperty, SsuiTheme.SelectedBorderBrushProperty, theme);
                ApplyThemeBinding(ForegroundProperty, SsuiTheme.ForegroundProperty, theme);
                ApplyThemeBinding(ForegroundHighlightBrushProperty, SsuiTheme.HighlightForegroundProperty, theme);
                ApplyThemeBinding(ClickBrushProperty, SsuiTheme.ClickBrushProperty, theme);

                ApplyThemeBinding(CornerRadiusProperty, SsuiTheme.CornerRadiusProperty, theme);
            }
        }

        /// <summary>
        /// Create and set a binding for a brush property, by binding it to a property in <see cref="SolidShineUi.SsuiTheme"/>.
        /// </summary>
        /// <param name="brushProperty">the property on this control to bind</param>
        /// <param name="ssuiThemeProperty">the property in <see cref="SolidShineUi.SsuiTheme"/> to bind this control's property to</param>
        /// <exception cref="ArgumentException">
        /// thrown if <paramref name="ssuiThemeProperty"/> is not a property in <see cref="SolidShineUi.SsuiTheme"/> or a class that inherits from SsuiTheme
        /// </exception>
        /// <remarks>
        /// If <see cref="SsuiTheme"/> is <c>null</c>, then instead the binding is cleared, and <c>null</c> is returned.
        /// </remarks>
#if NETCOREAPP
        protected BindingExpressionBase? ApplyThemeBinding(DependencyProperty brushProperty, DependencyProperty ssuiThemeProperty)
#else
        protected BindingExpressionBase ApplyThemeBinding(DependencyProperty brushProperty, DependencyProperty ssuiThemeProperty)
#endif
        {
            return ApplyThemeBinding(brushProperty, ssuiThemeProperty, SsuiTheme);
        }

        /// <summary>
        /// Create and set a binding for a brush property, by binding it to a property in <see cref="SolidShineUi.SsuiTheme"/>.
        /// </summary>
        /// <param name="brushProperty">the property on this control to bind</param>
        /// <param name="ssuiThemeProperty">the name of the property in <see cref="SolidShineUi.SsuiTheme"/> to bind this control's property to</param>
        /// <param name="source">the specific SsuiTheme object to bind to (such as <see cref="SsuiAppTheme"/>'s AccentTheme or SubitemTheme)</param>
        /// <exception cref="ArgumentException">
        /// thrown if <paramref name="ssuiThemeProperty"/> is not a property in <see cref="SolidShineUi.SsuiTheme"/> or a class that inherits from SsuiTheme
        /// </exception>
        /// <remarks>
        /// If <paramref name="source"/> is <c>null</c>, then instead the binding is cleared, and <c>null</c> is returned.
        /// </remarks>
#if NETCOREAPP
        protected BindingExpressionBase? ApplyThemeBinding(DependencyProperty brushProperty, DependencyProperty ssuiThemeProperty, SsuiTheme? source)
#else
        protected BindingExpressionBase ApplyThemeBinding(DependencyProperty brushProperty, DependencyProperty ssuiThemeProperty, SsuiTheme source)
#endif
        {
            // if ThemeValueExclude includes this property's name (as part of a comma-separated list), then we'll not do anything here
            if (!string.IsNullOrEmpty(ThemeValueExclude))
            {
                if (ThemeValueExclude.Split(',').Contains(brushProperty.Name)) return null;
            }

            // if the theme value provided is null, then we'll just undo any binding that exists (which should undo it to the previous SsuiTheme)
            if (source == null)
            {
                BindingOperations.ClearBinding(this, brushProperty);
                return null;
            }
            else
            {
                return SetBinding(brushProperty, SsuiTheme.CreateBinding(ssuiThemeProperty, source));
            }
        }

        /// <summary>
        /// The backing routed event object for <see cref="SsuiThemeChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent SsuiThemeAppliedEvent = ThemedControl.SsuiThemeAppliedEvent.AddOwner(typeof(FlatButton));
        // EventManager.RegisterRoutedEvent(nameof(SsuiThemeApplied), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ThemedContentControl));

        /// <summary>
        /// Raised after <see cref="ApplySsuiTheme(SsuiTheme, bool, bool)"/> has completed, and the theme has been applied to the control.
        /// </summary>
        /// <remarks>
        /// If creating your own control that inherits from this class, use this event or override the <see cref="OnApplySsuiTheme(SsuiTheme, bool, bool)"/>
        /// method to apply any updated brushes from the <see cref="SsuiTheme"/>.
        /// <para/>
        /// This event precedes the <see cref="SsuiThemeChanged"/> event.
        /// </remarks>
        public event RoutedEventHandler SsuiThemeApplied
        {
            add { AddHandler(SsuiThemeAppliedEvent, value); }
            remove { RemoveHandler(SsuiThemeAppliedEvent, value); }
        }

        #endregion

        #region Style Generation

        /// <summary>
        /// Creates a <see cref="Style"/> for a FlatButton, where the visual properties are bound to a SsuiTheme <paramref name="theme"/>.
        /// </summary>
        /// <param name="theme">the SsuiTheme object to bind to</param>
        /// <remarks>
        /// This is currently unused, but may be experimented with for using in future versions of Solid Shine UI
        /// </remarks>
        protected internal static Style GenerateStyle(SsuiTheme theme)
        {
            Style os = new Style(typeof(FlatButton));

            // looking at the actual WPF code for Setters, a Setter's Value can be bound by setting its Value to a BindingBase object
            // https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Setter.cs

            os.Setters.Add(new Setter(BackgroundProperty, SsuiTheme.CreateBinding(SsuiTheme.ButtonBackgroundProperty, theme)));
            os.Setters.Add(new Setter(HighlightBrushProperty, SsuiTheme.CreateBinding(SsuiTheme.HighlightBrushProperty, theme)));
            os.Setters.Add(new Setter(DisabledBrushProperty, SsuiTheme.CreateBinding(SsuiTheme.DisabledBackgroundProperty, theme)));
            os.Setters.Add(new Setter(BorderDisabledBrushProperty, SsuiTheme.CreateBinding(SsuiTheme.DisabledBorderBrushProperty, theme)));
            os.Setters.Add(new Setter(SelectedBrushProperty, SsuiTheme.CreateBinding(SsuiTheme.SelectedBackgroundBrushProperty, theme)));
            os.Setters.Add(new Setter(BorderHighlightBrushProperty, SsuiTheme.CreateBinding(SsuiTheme.HighlightBorderBrushProperty, theme)));
            os.Setters.Add(new Setter(BorderSelectedBrushProperty, SsuiTheme.CreateBinding(SsuiTheme.SelectedBorderBrushProperty, theme)));
            os.Setters.Add(new Setter(ForegroundProperty, SsuiTheme.CreateBinding(SsuiTheme.ForegroundProperty, theme)));
            os.Setters.Add(new Setter(ForegroundHighlightBrushProperty, SsuiTheme.CreateBinding(SsuiTheme.HighlightForegroundProperty, theme)));
            os.Setters.Add(new Setter(ClickBrushProperty, SsuiTheme.CreateBinding(SsuiTheme.ClickBrushProperty, theme)));

            os.Setters.Add(new Setter(CornerRadiusProperty, SsuiTheme.CreateBinding(SsuiTheme.CornerRadiusProperty, theme)));

            return os;
        }

        #endregion

        #endregion

        #region ColorScheme/UseAccentColors

        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif


        /// <summary>The backing dependency property for <see cref="ColorScheme"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(FlatButton),
            new FrameworkPropertyMetadata(new ColorScheme(), OnColorSchemeChanged));


        //bool use_transp = false;
        //bool use_accent = false;

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        private static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ColorScheme cs)
            {
                if (d is FlatButton f)
                {
                    f.ApplyColorScheme(cs);
                    f.ColorSchemeChanged?.Invoke(d, e);
                }
            }
        }

        /// <summary>
        /// Get or set the color scheme to apply to this button. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        [Category("Appearance")]
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        bool runApply = true;

        /// <summary>
        /// The backing dependency property for <see cref="UseAccentColors"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty UseAccentColorsProperty
            = DependencyProperty.Register("UseAccentColors", typeof(bool), typeof(FlatButton),
            new PropertyMetadata(false,
                new PropertyChangedCallback((d, e) => d.PerformAs<FlatButton, bool>(e.NewValue, (f, v) => { f.ApplyColorScheme(f.ColorScheme, v); }))));

        /// <summary>
        /// Get or set if the button should use the accent colors of the color scheme, rather than the standard colors.
        /// <para/>
        /// This method will be removed in a future version. Please use <see cref="UseAccentTheme"/> instead.
        /// </summary>
        [Category("Appearance")]
#if NET5_0_OR_GREATER
        [Obsolete("This method will be removed in a future version. Please use UseAccentTheme instead.", DiagnosticId = "SSUI001")]
#else
        [Obsolete("This method will be removed in a future version. Please use UseAccentTheme instead.")]
#endif
        public bool UseAccentColors
        {
            get => (bool)GetValue(UseAccentColorsProperty);
            set => SetValue(UseAccentColorsProperty, value);
        }

        /// <summary>
        /// Apply a color scheme to this control, and set some other optional appearance settings. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply</param>
        public void ApplyColorScheme(ColorScheme cs)
        {
#pragma warning disable SSUI001 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
            ApplyColorScheme(cs, UseAccentColors || UseAccentTheme);
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore SSUI001 // Type or member is obsolete
        }

        /// <summary>
        /// Apply a color scheme to this control, and set some other optional appearance settings. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply</param>
        /// <param name="useAccentColors">
        /// Set if accent colors should be used for this button, rather than the main color scheme colors.
        /// This can also be achieved with the <c>UseAccentColors</c> property.
        /// </param>
        public void ApplyColorScheme(ColorScheme cs, bool useAccentColors = false)
        {
            if (runApply == false)
            {
                return;
            }

            if (cs == null)
            {
                return;
            }

            if (cs != ColorScheme)
            {
                runApply = false;
#pragma warning disable SSUI001 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
                UseAccentColors = UseAccentColors || useAccentColors;
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore SSUI001 // Type or member is obsolete
                runApply = true;
                ColorScheme = cs;
                return;
            }

            if (useAccentColors != UseAccentTheme)
            {
                runApply = false;
                UseAccentTheme = useAccentColors;
                runApply = true;
            }

            runApply = false;

            if (cs.IsHighContrast)
            {
                Background = cs.BackgroundColor.ToBrush();
                HighlightBrush = cs.HighlightColor.ToBrush();
                SelectedBrush = cs.HighlightColor.ToBrush();
                BorderHighlightBrush = cs.BorderColor.ToBrush();
                BorderSelectedBrush = cs.BorderColor.ToBrush();
                BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                DisabledBrush = cs.BackgroundColor.ToBrush();
                Foreground = cs.ForegroundColor.ToBrush();
                ClickBrush = cs.ThirdHighlightColor.ToBrush();

                //if (transparentBack || TransparentBack)
                //{
                //    BorderBrush = Color.FromArgb(1, 0, 0, 0).ToBrush();
                //    TransparentBack = true;
                //}
                //else
                //{
                //    BorderBrush = cs.BorderColor.ToBrush();
                //}
            }
            else
            {
#pragma warning disable SSUI001 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
                if (UseAccentColors || useAccentColors || UseAccentTheme)
                {
                    Background = cs.AccentSecondaryColor.ToBrush();
                    BorderBrush = cs.AccentBorderColor.ToBrush();
                    HighlightBrush = cs.AccentSecondHighlightColor.ToBrush();
                    DisabledBrush = cs.LightDisabledColor.ToBrush();
                    BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                    SelectedBrush = cs.AccentThirdHighlightColor.ToBrush();
                    BorderHighlightBrush = cs.AccentHighlightColor.ToBrush();
                    BorderSelectedBrush = cs.AccentSelectionColor.ToBrush();
                    Foreground = cs.ForegroundColor.ToBrush();
                    ClickBrush = cs.AccentThirdHighlightColor.ToBrush();

                    UseAccentColors = true;
                }
                else
                {
                    Background = cs.SecondaryColor.ToBrush();
                    BorderBrush = cs.BorderColor.ToBrush();
                    HighlightBrush = cs.SecondHighlightColor.ToBrush();
                    DisabledBrush = cs.LightDisabledColor.ToBrush();
                    BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                    SelectedBrush = cs.ThirdHighlightColor.ToBrush();
                    BorderHighlightBrush = cs.HighlightColor.ToBrush();
                    BorderSelectedBrush = cs.SelectionColor.ToBrush();
                    Foreground = cs.ForegroundColor.ToBrush();
                    ClickBrush = cs.ThirdHighlightColor.ToBrush();
                }
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore SSUI001 // Type or member is obsolete
            }

            runApply = true;
        }

        #endregion

        #region Border

        /// <summary>The backing dependency property for <see cref="BorderSelectionThickness"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderSelectionThicknessProperty = DependencyProperty.Register(
            nameof(BorderSelectionThickness), typeof(Thickness), typeof(FlatButton),
            new PropertyMetadata(new Thickness(2)));

        /// <summary>The backing dependency property for <see cref="CornerRadius"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius), typeof(CornerRadius), typeof(FlatButton),
            new PropertyMetadata(new CornerRadius(0)));

        /// <summary>
        /// Get or set the thickness of the border around the button, while the button is in a selected (<c>IsSelected</c>) state.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the thickness of the border around the button, while the button is in a selected state.")]
        public Thickness BorderSelectionThickness
        {
            get => (Thickness)GetValue(BorderSelectionThicknessProperty);
            set => SetValue(BorderSelectionThicknessProperty, value);
        }

        /// <summary>
        /// Get or set the corner radius (or radii) to use for the button and its border. Can be used to create a rounded button.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the corner radius (or radii) to use for the button and its border. Can be used to create a rounded button.")]
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #endregion

        #region HighlightOnKeyboardFocus

        /// <summary>
        /// Get or set if the button should be highlighted (using the <see cref="HighlightBrush"/> and <see cref="BorderHighlightBrush"/>)
        /// when it has keyboard focus. If <c>false</c>, only the keyboard focus outline appears, and highlighting only occurs on mouse/stylus over.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set if the button should be highlighted when it has keyboard focus.")]
        public bool HighlightOnKeyboardFocus { get => (bool)GetValue(HighlightOnKeyboardFocusProperty); set => SetValue(HighlightOnKeyboardFocusProperty, value); }

        /// <summary>The backing dependency property for <see cref="HighlightOnKeyboardFocus"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HighlightOnKeyboardFocusProperty
            = DependencyProperty.Register(nameof(HighlightOnKeyboardFocus), typeof(bool), typeof(FlatButton),
            new FrameworkPropertyMetadata(true));

        #endregion

        #region Click / Selection Handling

        #region Routed Events

        /// <summary>
        /// The backing value for the <see cref="RightClick"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent RightClickEvent = EventManager.RegisterRoutedEvent(
            nameof(RightClick), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FlatButton));

        /// <summary>
        /// Raised when the user right-clicks on the button, via a mouse click or via the keyboard.
        /// </summary>
        public event RoutedEventHandler RightClick
        {
            add { AddHandler(RightClickEvent, value); }
            remove { RemoveHandler(RightClickEvent, value); }
        }

        #endregion

        #region IsMouseDown

        // from https://stackoverflow.com/questions/10667545/why-ismouseover-is-recognized-and-mousedown-isnt-wpf-style-trigger

        /// <summary>
        /// The internal dependency property for <see cref="IsMouseDown"/>. See that property for more details.
        /// </summary>
        protected static readonly DependencyPropertyKey IsMouseDownPropertyKey = DependencyProperty.RegisterAttachedReadOnly(nameof(IsMouseDown),
            typeof(bool), typeof(FlatButton), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get if there is a mouse button currently being pressed, while the mouse cursor is over this control.
        /// </summary>
        public static readonly DependencyProperty IsMouseDownProperty = IsMouseDownPropertyKey.DependencyProperty;

        /// <summary>
        /// Set the IsMouseDown property for a FlatButton.
        /// </summary>
        /// <param name="obj">The FlatButton to apply the property change to.</param>
        /// <param name="value">The new value to set for the property.</param>
        protected static void SetIsMouseDown(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMouseDownPropertyKey, value);
        }

        /// <summary>
        /// Get the IsMouseDown property for a FlatButton.
        /// </summary>
        /// <param name="obj">The Flatbutton to get the property value from.</param>
        public static bool GetIsMouseDown(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMouseDownProperty);
        }

        /// <summary>
        /// Get if a mouse button is currently being pressed while the cursor is over this FlatButton.
        /// </summary>
        [ReadOnly(true)]
        public bool IsMouseDown
        {
            get => (bool)GetValue(IsMouseDownProperty);
            protected set => SetValue(IsMouseDownPropertyKey, value);
        }

        #endregion

        #region Variables/Properties
        
        bool initiatingClick = false;

        #region IsSelected / IsSelectedChanged

        bool _runSelChangeEvent = true;

        /// <summary>
        /// The backing dependency property for <see cref="IsSelected"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            nameof(IsSelected), typeof(bool), typeof(FlatButton), new PropertyMetadata(false, OnIsSelectedChanged));

        /// <summary>
        /// Perform an action when a property of an object has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool se)
            {
                bool old = (e.OldValue is bool oval) ? oval : false;

                if (d is FlatButton f)
                {
                    if (f._runSelChangeEvent)
                    {
                        ItemSelectionChangedEventArgs re = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, old, se, IsSelectedProperty, SelectionChangeTrigger.CodeUnknown, null);
                        f.RaiseEvent(re);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets whether this button is selected. This property (combined with <see cref="SelectOnClick"/>) allows the button to function like a ToggleButton.
        /// </summary>
        /// <remarks>
        /// A selected button will have a slightly different visual appearance to differentiate it as being selected. This will include, by default, the border being a bit thicker.
        /// This can be changed via the <see cref="BorderSelectionThickness"/> property. You can also directly edit the brushes used via the <see cref="SelectedBrush"/> and
        /// <see cref="BorderSelectedBrush"/> properties.
        /// <para />
        /// To listen to changes to this property, use <see cref="IsSelectedChanged"/>, rather than listening to the <c>Click</c> event, as other actions could change this 
        /// property rather than just clicking it.
        /// </remarks>
        [Category("Common")]
        [Description("Gets or sets whether this button is selected.")]
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// The backing value for the <see cref="IsSelectedChanged"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent IsSelectedChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(IsSelectedChanged), RoutingStrategy.Bubble, typeof(ItemSelectionChangedEventHandler), typeof(FlatButton));

        /// <summary>
        /// Raised when the user clicks on the main button (not the menu button), via a mouse click or via the keyboard.
        /// </summary>
        public event ItemSelectionChangedEventHandler IsSelectedChanged
        {
            add { AddHandler(IsSelectedChangedEvent, value); }
            remove { RemoveHandler(IsSelectedChangedEvent, value); }
        }

        /// <summary>
        /// Set the <see cref="IsSelected"/> value of this control, while also defining how the selection was changed.
        /// </summary>
        /// <param name="value">The value to set <see cref="IsSelected"/> to.</param>
        /// <param name="trigger">The source or method used to trigger the change in selection.</param>
        /// <param name="triggerSource">The object that triggered the change.</param>
#if NETCOREAPP
        public void SetIsSelectedWithSource(bool value, SelectionChangeTrigger trigger, object? triggerSource = null)
#else
        public void SetIsSelectedWithSource(bool value, SelectionChangeTrigger trigger, object triggerSource = null)
#endif
        {
            bool old = IsSelected;

            _runSelChangeEvent = false;
            IsSelected = value;
            _runSelChangeEvent = true;

            ItemSelectionChangedEventArgs re = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, old, value, IsSelectedProperty, trigger, triggerSource);
            RaiseEvent(re);
        }
        #endregion

        /// <summary>
        /// The backing dependency property object for the <see cref="SelectOnClick"/> property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty SelectOnClickProperty = DependencyProperty.Register(
            nameof(SelectOnClick), typeof(bool), typeof(FlatButton), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether the button should change its IsSelected property when a click is performed. With this enabled, this allows the button to take on the functionality of a ToggleButton.
        /// </summary>
        /// <remarks>
        /// While SelectOnClick is true, the button will toggle between <see cref="IsSelected"/> being true and false (similar to a ToggleButton).<para/>
        /// The button's Click event will still be raised while this property is set to <c>true</c>, but the event occurs after the
        /// IsSelected property has already changed. Do not use Click event to check when the button's IsSelected property is changed, but instead the IsSelectedChanged event,
        /// in case of situations where IsSelected is changed via methods other than clicking, such as programmatically or via WPF binding.
        /// </remarks>
        [Category("Common")]
        [Description("Gets or sets whether the button should change its IsSelected property when a click is performed. With this enabled, this allows the button to take on the functionality of a ToggleButton.")]
        public bool SelectOnClick
        {
            get => (bool)GetValue(SelectOnClickProperty);
            set => SetValue(SelectOnClickProperty, value);
        }

        #endregion

        #region Base Click Functions

        // Sets up the button to be clicked. This must be run before PerformClick.
        void PressRightClick()
        {
            initiatingClick = true;
        }

        // If the button is prepared by PerformPress, perform the Click actions, including raising the Click event.
        void PerformRightClick()
        {
            if (initiatingClick)
            {
                RoutedEventArgs rre = new RoutedEventArgs(RightClickEvent);
                RaiseEvent(rre);
            }
        }

        /// <summary>
        /// Perform a click programmatically. The button responds the same way as if it was clicked by the user.
        /// </summary>
        public void DoClick()
        {
            OnClick();
        }

        /// <summary>
        /// Defines the actions the button performs when it is clicked.
        /// </summary>
        protected override void OnClick()
        {
            if (SelectOnClick)
            {
                SetIsSelectedWithSource(!IsSelected, SelectionChangeTrigger.ControlClick, this);
            }

            base.OnClick();
        }

        #endregion

        #region Event handlers

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (e.ChangedButton == MouseButton.Right)
            //{
            //    PressRightClick();
            //}
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //if (e.ChangedButton == MouseButton.Right)
            //{
            //    PerformRightClick();
            //}
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (ClickMode == ClickMode.Press && e.Key == Key.Apps)
            {
                PressRightClick();
                PerformRightClick();
            }
        }

        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (ClickMode != ClickMode.Press && e.Key == Key.Apps)
            {
                PressRightClick();
                PerformRightClick();
            }
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                PressRightClick();
            }
            SetIsMouseDown(this, true);
        }

        private void UserControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                PerformRightClick();
            }
            SetIsMouseDown(this, false);
        }


        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            initiatingClick = false;
        }

        private void UserControl_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            initiatingClick = false;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            SetIsMouseDown(this, false);
            initiatingClick = false;
        }


        #endregion

        #endregion

    }
}
