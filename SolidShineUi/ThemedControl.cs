using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SolidShineUi.Utils;

namespace SolidShineUi
{
    /// <summary>
    /// A WPF control that has extra properties to apply theming from a <see cref="SolidShineUi.SsuiTheme"/>.
    /// </summary>
    public class ThemedControl : Control
    {
        static ThemedControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ThemedControl), new FrameworkPropertyMetadata(typeof(ThemedControl)));
        }

        /// <summary>
        /// Create a ThemedControl.
        /// </summary>
        public ThemedControl() { }

        #region SsuiTheme Property

        // references to follow up on this:
        // https://github.com/dotnet/wpf/blob/8223378b6fdddd3ae4e0577e90d9953b5469f076/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/FrameworkElement.cs#L3623
        // https://github.com/dotnet/wpf/blob/8223378b6fdddd3ae4e0577e90d9953b5469f076/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Documents/Block.cs#L253

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
        /// <para/>
        /// Note that setting this will likely overwrite any brush values that are set in all child controls (that have a SsuiTheme property to inherit),
        /// so if you had set a control to use a specific brush, you will need to re-update that after calling this property.
        /// </remarks>
        [Category("Appearance")]
#if NETCOREAPP
        public SsuiTheme? SsuiTheme { get => (SsuiTheme)GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }
#else
        public SsuiTheme SsuiTheme { get => (SsuiTheme)GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }
#endif

        /// <summary>The backing dependency property for <see cref="SsuiTheme"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SsuiThemeProperty
            = DependencyProperty.RegisterAttached(nameof(SsuiTheme), typeof(SsuiTheme), typeof(ThemedControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, OnSsuiThemeChanged));

        /// <summary>
        /// Get the <see cref="SsuiTheme"/> value applied to the specified framework element.
        /// </summary>
        /// <param name="tc">the element to get the <see cref="SsuiThemeProperty"/> value of</param>
        /// <returns></returns>
        [AttachedPropertyBrowsableForType(typeof(ThemedControl))]
        [AttachedPropertyBrowsableForType(typeof(ThemedContentControl))]
        public static SsuiTheme GetSsuiThemeProperty(FrameworkElement tc)
        {
            return (SsuiTheme)tc.GetValue(SsuiThemeProperty);
        }

        /// <summary>
        /// Set the <see cref="SsuiTheme"/> value for a specified framework element.
        /// </summary>
        /// <param name="tc">the element to set the <see cref="SsuiThemeProperty"/> value for</param>
        /// <param name="value">the <see cref="SsuiTheme"/> value to set</param>
        public static void SetSsuiThemeProperty(FrameworkElement tc, SsuiTheme value)
        {
            tc.SetValue(SsuiThemeProperty, value);
        }

        /// <summary>
        /// The backing routed event object for <see cref="SsuiThemeChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent SsuiThemeChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(SsuiThemeChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ThemedControl));

        /// <summary>
        /// Raised when the <see cref="SsuiTheme"/> property is changed.
        /// </summary>
        public event RoutedEventHandler SsuiThemeChanged
        {
            add { AddHandler(SsuiThemeChangedEvent, value); }
            remove { RemoveHandler(SsuiThemeChangedEvent, value); }
        }

        private static void OnSsuiThemeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ThemedControl tc)
            {
                tc.ApplyAndRaiseTheme();
            }
            else if (sender is ThemedContentControl tcc)
            {
                //// update the SsuiTheme property for the ThemedContentControl
                //tcc.SsuiTheme = (SsuiTheme)e.NewValue;
                tcc.ApplyAndRaiseTheme();
            }
            else if (sender is ThemedUserControl tuc)
            {
                tuc.ApplyAndRaiseTheme();
            }
        }

        /// <summary>
        /// Update the control's appearance by using <see cref="ApplySsuiTheme(SsuiTheme)"/> and then raise the <see cref="SsuiThemeChanged"/> event.
        /// </summary>
        internal protected void ApplyAndRaiseTheme()
        {
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);

            RoutedEventArgs re = new RoutedEventArgs(SsuiThemeChangedEvent, this);
            RaiseEvent(re);
        }

#endregion

        #region UseLightBorder

        /// <summary>
        /// Get or set if the <see cref="SsuiTheme.LightBorderBrush"/> should be used for the border of this control, rather than the <see cref="SsuiTheme.BorderBrush"/>.
        /// </summary>
        public bool UseLightBorder { get => (bool)GetValue(UseLightBorderProperty); set => SetValue(UseLightBorderProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseLightBorder"/>. See the related property for details.</summary>
        public static readonly DependencyProperty UseLightBorderProperty
            = DependencyProperty.Register(nameof(UseLightBorder), typeof(bool), typeof(ThemedControl),
            new FrameworkPropertyMetadata(false, (d, e) => d.PerformAs<ThemedControl>((o) => o.OnUseLightBorderChange(o, e))));

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
        public static readonly DependencyProperty UseAccentThemeProperty
            = DependencyProperty.Register(nameof(UseAccentTheme), typeof(bool), typeof(ThemedControl),
            new FrameworkPropertyMetadata(false, (d, e) => d.PerformAs<ThemedControl>((o) => o.OnUseAccentColorsChange(o ,e))));

        void OnUseAccentColorsChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_skipReapply) return;
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);
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
        public static readonly DependencyProperty ThemeValueExcludeProperty
            = DependencyProperty.Register(nameof(ThemeValueExclude), typeof(string), typeof(ThemedControl),
            new FrameworkPropertyMetadata(""));

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
            // TODO: if SsuiTheme is null, instead call a ClearBindings method that prompts the control to instead clear all of the bindings to the SsuiTheme

            RoutedEventArgs re = new RoutedEventArgs(SsuiThemeAppliedEvent, this);
            RaiseEvent(re);
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
        /// The base method will by default set the <see cref="Control.BorderBrush"/> value to match the theme; if a custom
        /// border brush is needed, then update it again after this 
        /// </remarks>
        protected virtual void OnApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {
            if (useAccentTheme && ssuiTheme is SsuiAppTheme sat)
            {
                ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty, sat.AccentTheme);
                // Foreground = sat.AccentTheme.Foreground; // we'll see if I need this line
            }
            else
            {
                ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty);
                // Foreground = ssuiTheme.Foreground; // we'll see if I need this line
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
        public static readonly RoutedEvent SsuiThemeAppliedEvent = EventManager.RegisterRoutedEvent(
            nameof(SsuiThemeApplied), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ThemedControl));

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
    }


    /// <summary>
    /// A WPF content control that has extra properties to apply theming from a <see cref="SolidShineUi.SsuiTheme"/>.
    /// </summary>
    public class ThemedContentControl : ContentControl
    {

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
        /// <para/>
        /// Note that setting this will likely overwrite any brush values that are set in all child controls (that have a SsuiTheme property to inherit),
        /// so if you had set a control to use a specific brush, you will need to re-update that after calling this property.
        /// </remarks>
        [Category("Appearance")]
#if NETCOREAPP
        public SsuiTheme? SsuiTheme { get => (SsuiTheme)GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }
#else
        public SsuiTheme SsuiTheme { get => (SsuiTheme)GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }
#endif

        /// <summary>The backing dependency property for <see cref="SsuiTheme"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SsuiThemeProperty = ThemedControl.SsuiThemeProperty.AddOwner(typeof(ThemedContentControl));

        /// <summary>
        /// The backing routed event object for <see cref="SsuiThemeChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent SsuiThemeChangedEvent = ThemedControl.SsuiThemeChangedEvent.AddOwner(typeof(ThemedContentControl));

        /// <summary>
        /// Raised when the <see cref="SsuiTheme"/> property is changed.
        /// </summary>
        public event RoutedEventHandler SsuiThemeChanged
        {
            add { AddHandler(SsuiThemeChangedEvent, value); }
            remove { RemoveHandler(SsuiThemeChangedEvent, value); }
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
        public static readonly DependencyProperty UseLightBorderProperty = ThemedControl.UseLightBorderProperty.AddOwner(typeof(ThemedContentControl),
            new FrameworkPropertyMetadata(false, (d, e) => d.PerformAs<ThemedContentControl>((o) => o.OnUseLightBorderChange(o, e))));

        void OnUseLightBorderChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_skipReapply) return;
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);
        }

        #endregion

        #region UseAccentColors

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
        public static readonly DependencyProperty UseAccentThemeProperty = ThemedControl.UseAccentThemeProperty.AddOwner(typeof(ThemedContentControl),
            new FrameworkPropertyMetadata(false, (d, e) => d.PerformAs<ThemedContentControl>((o) => o.OnUseAccentColorsChange(o, e))));

        void OnUseAccentColorsChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_skipReapply) return;
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);
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
        public static readonly DependencyProperty ThemeValueExcludeProperty = ThemedControl.ThemeValueExcludeProperty.AddOwner(typeof(ThemedContentControl));

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
            // TODO: if SsuiTheme is null, instead call a ClearBindings method that prompts the control to instead clear all of the bindings to the SsuiTheme

            RoutedEventArgs re = new RoutedEventArgs(SsuiThemeAppliedEvent, this);
            RaiseEvent(re);
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
        /// The base method will by default set the <see cref="Control.BorderBrush"/> value to match the theme; if a custom
        /// border brush is needed, then update it again after this is called.
        /// </remarks>
        protected virtual void OnApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {
            if (useAccentTheme && ssuiTheme is SsuiAppTheme sat)
            {
                ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty, sat.AccentTheme);
                // Foreground = sat.AccentTheme.Foreground; // we'll see if I need this line
            }
            else
            {
                ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty);
                // Foreground = ssuiTheme.Foreground; // we'll see if I need this line
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
        public static readonly RoutedEvent SsuiThemeAppliedEvent = ThemedControl.SsuiThemeAppliedEvent.AddOwner(typeof(ThemedContentControl));

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
    }

    /// <summary>
    /// A WPF user control that has extra properties to apply theming from a <see cref="SolidShineUi.SsuiTheme"/>.
    /// </summary>
    public class ThemedUserControl : UserControl
    {

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
        /// <para/>
        /// Note that setting this will likely overwrite any brush values that are set in all child controls (that have a SsuiTheme property to inherit),
        /// so if you had set a control to use a specific brush, you will need to re-update that after calling this property.
        /// </remarks>
        [Category("Appearance")]
#if NETCOREAPP
        public SsuiTheme? SsuiTheme { get => (SsuiTheme)GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }
#else
        public SsuiTheme SsuiTheme { get => (SsuiTheme)GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }
#endif

        /// <summary>The backing dependency property for <see cref="SsuiTheme"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SsuiThemeProperty = ThemedControl.SsuiThemeProperty.AddOwner(typeof(ThemedUserControl));

        /// <summary>
        /// The backing routed event object for <see cref="SsuiThemeChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent SsuiThemeChangedEvent = ThemedControl.SsuiThemeChangedEvent.AddOwner(typeof(ThemedUserControl));

        /// <summary>
        /// Raised after the <see cref="SsuiTheme"/> property has been changed and applied.
        /// </summary>
        public event RoutedEventHandler SsuiThemeChanged
        {
            add { AddHandler(SsuiThemeChangedEvent, value); }
            remove { RemoveHandler(SsuiThemeChangedEvent, value); }
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
        public static readonly DependencyProperty UseLightBorderProperty = ThemedControl.UseLightBorderProperty.AddOwner(typeof(ThemedUserControl),
            new FrameworkPropertyMetadata(false, (d, e) => d.PerformAs<ThemedUserControl>((o) => o.OnUseLightBorderChange(o, e))));

        void OnUseLightBorderChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_skipReapply) return;
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);
        }

        #endregion

        #region UseAccentColors

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
        public static readonly DependencyProperty UseAccentThemeProperty = ThemedControl.UseAccentThemeProperty.AddOwner(typeof(ThemedUserControl),
            new FrameworkPropertyMetadata(false, (d, e) => d.PerformAs<ThemedUserControl>((o) => o.OnUseAccentColorsChange(o, e))));

        void OnUseAccentColorsChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_skipReapply) return;
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);
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
        public static readonly DependencyProperty ThemeValueExcludeProperty = ThemedControl.ThemeValueExcludeProperty.AddOwner(typeof(ThemedUserControl));

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
            // TODO: if SsuiTheme is null, instead call a ClearBindings method that prompts the control to instead clear all of the bindings to the SsuiTheme

            RoutedEventArgs re = new RoutedEventArgs(SsuiThemeAppliedEvent, this);
            RaiseEvent(re);
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
        /// The base method will by default set the <see cref="Control.BorderBrush"/> value to match the theme; if a custom
        /// border brush is needed, then update it again after this is called.
        /// </remarks>
        protected virtual void OnApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {
            if (useAccentTheme && ssuiTheme is SsuiAppTheme sat)
            {
                ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty, sat.AccentTheme);
                // Foreground = sat.AccentTheme.Foreground; // we'll see if I need this line
            }
            else
            {
                ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty);
                // Foreground = ssuiTheme.Foreground; // we'll see if I need this line
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
        public static readonly RoutedEvent SsuiThemeAppliedEvent = ThemedControl.SsuiThemeAppliedEvent.AddOwner(typeof(ThemedUserControl));

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
    }
}
