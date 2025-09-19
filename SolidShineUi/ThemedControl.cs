using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SolidShineUi
{
    /// <summary>
    /// A WPF control that has extra properties to apply theming from a <see cref="SolidShineUi.SsuiTheme"/>.
    /// </summary>
    public abstract class ThemedControl : Control
    {
        static ThemedControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ThemedControl), new FrameworkPropertyMetadata(typeof(ThemedControl)));
        }

        //DependencyPropertyDescriptor ddd;
        //EventHandler themeChange;

        /// <summary>
        /// Create a ThemedControl.
        /// </summary>
        public ThemedControl()
        {
            // creating a method to listen to changes to the SsuiTheme property (since it's an attached property)
            //
            // according to this, this might be a memory leak issue, so I've added on a finalizer to hopefully counteract that
            // https://agsmith.wordpress.com/2008/04/07/propertydescriptor-addvaluechanged-alternative/
            //ddd = DependencyPropertyDescriptor.FromProperty(SsuiThemeProperty, typeof(ThemedControl));
            //themeChange = (s, e) => ApplyAndRaiseTheme();
            //ddd.AddValueChanged(this, themeChange);
        }

        ///// <summary>
        ///// Finalizer for ThemedControl.
        ///// </summary>
        //~ThemedControl()
        //{
        //    ddd.RemoveValueChanged(this, themeChange);
        //}

        #region SsuiTheme Property

        // references to follow up on this:
        // https://github.com/dotnet/wpf/blob/8223378b6fdddd3ae4e0577e90d9953b5469f076/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/FrameworkElement.cs#L3623
        // https://github.com/dotnet/wpf/blob/8223378b6fdddd3ae4e0577e90d9953b5469f076/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Documents/Block.cs#L253

        /// <summary>
        /// Get or set the theme to apply to the appearance of this control.
        /// </summary>
        public SsuiTheme SsuiTheme { get => (SsuiTheme)GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }

        /// <summary>The backing dependency property for <see cref="SsuiTheme"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SsuiThemeProperty
            = DependencyProperty.RegisterAttached(nameof(SsuiTheme), typeof(SsuiTheme), typeof(ThemedControl),
            new FrameworkPropertyMetadata(new SsuiTheme(), FrameworkPropertyMetadataOptions.Inherits, OnSsuiThemeChanged));

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
        }

        /// <summary>
        /// Update the control's appearance by using <see cref="ApplySsuiTheme(SsuiTheme)"/> and then raise the <see cref="SsuiThemeChanged"/> event.
        /// </summary>
        internal protected void ApplyAndRaiseTheme()
        {
            ApplySsuiTheme(SsuiTheme);

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
        public static DependencyProperty UseLightBorderProperty
            = DependencyProperty.Register(nameof(UseLightBorder), typeof(bool), typeof(ThemedControl),
            new FrameworkPropertyMetadata(false, (d, e) => d.PerformAs<ThemedControl>((o) => o.OnUseLightBorderChange(o, e))));

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
        /// This should already be set if you're putting this control in a <c>ThemedWindow</c> and inheriting from that SsuiTheme property.
        /// </remarks>
        public bool UseAccentTheme { get => (bool)GetValue(UseAccentThemeProperty); set => SetValue(UseAccentThemeProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseAccentTheme"/>. See the related property for details.</summary>
        public static DependencyProperty UseAccentThemeProperty
            = DependencyProperty.Register(nameof(UseAccentTheme), typeof(bool), typeof(ThemedControl),
            new FrameworkPropertyMetadata(false, (d, e) => d.PerformAs<ThemedControl>((o) => o.OnUseAccentColorsChange(o ,e))));

        void OnUseAccentColorsChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_skipReapply) return;
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);
        }

        #endregion

        #region ApplySsuiTheme

        bool _skipReapply = false;

        /// <summary>
        /// Check the properties to make sure they match these values. If not, update the properties and then recall the ApplySsuiTheme function.
        /// </summary>
        /// <param name="ssuiTheme">the inputted SsuiTheme value</param>
        /// <param name="useLightBorder">the inputted UseLightBorder value</param>
        /// <param name="useAccentTheme">the inputted UseAccentTheme value</param>
        /// <returns></returns>
        protected bool CheckAndUpdateProperties(SsuiTheme ssuiTheme, bool useLightBorder, bool useAccentTheme)
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
        public void ApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {
            if (!CheckAndUpdateProperties(ssuiTheme, useLightBorder, useAccentTheme)) return;

            OnApplySsuiTheme(ssuiTheme, useLightBorder, useAccentTheme);

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
                ApplyBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty, sat.AccentTheme);
                // Foreground = sat.AccentTheme.Foreground; // we'll see if I need this line
            }
            else
            {
                var bb = ApplyBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty);
                // Foreground = ssuiTheme.Foreground; // we'll see if I need this line
            }
        }

        ///// <summary>
        ///// Create and set a binding for a brush property, by binding it to a property in <see cref="SolidShineUi.SsuiTheme"/>.
        ///// </summary>
        ///// <param name="brushProperty">the property on this control to bind</param>
        ///// <param name="ssuiThemePropertyName">the name of the property in <see cref="SolidShineUi.SsuiTheme"/> to bind this control's property to</param>
        //protected void ApplyBinding(DependencyProperty brushProperty, string ssuiThemePropertyName)
        //{
        //    SetBinding(brushProperty, new Binding(ssuiThemePropertyName) { Source = SsuiTheme });
        //}

        /// <summary>
        /// Create and set a binding for a brush property, by binding it to a property in <see cref="SolidShineUi.SsuiTheme"/>.
        /// </summary>
        /// <param name="brushProperty">the property on this control to bind</param>
        /// <param name="ssuiThemeProperty">the property in <see cref="SolidShineUi.SsuiTheme"/> to bind this control's property to</param>
        /// <exception cref="ArgumentException">
        /// thrown if <paramref name="ssuiThemeProperty"/> is not a property in <see cref="SolidShineUi.SsuiTheme"/> or <see cref="SsuiAppTheme"/>
        /// </exception>
        protected BindingExpressionBase ApplyBinding(DependencyProperty brushProperty, DependencyProperty ssuiThemeProperty)
        {
            return ApplyBinding(brushProperty, ssuiThemeProperty, SsuiTheme);
        }

        /// <summary>
        /// Create and set a binding for a brush property, by binding it to a property in <see cref="SolidShineUi.SsuiTheme"/>.
        /// </summary>
        /// <param name="brushProperty">the property on this control to bind</param>
        /// <param name="ssuiThemeProperty">the name of the property in <see cref="SolidShineUi.SsuiTheme"/> to bind this control's property to</param>
        /// <param name="source">the specific SsuiTheme object to bind to (such as <see cref="SsuiAppTheme"/>'s AccentTheme or SubitemTheme)</param>
        /// <exception cref="ArgumentException">
        /// thrown if <paramref name="ssuiThemeProperty"/> is not a property in <see cref="SolidShineUi.SsuiTheme"/> or <see cref="SsuiAppTheme"/>
        /// </exception>
        protected BindingExpressionBase ApplyBinding(DependencyProperty brushProperty, DependencyProperty ssuiThemeProperty, SsuiTheme source)
        {
            if (ssuiThemeProperty.OwnerType != typeof(SsuiTheme) && ssuiThemeProperty.OwnerType != typeof(SsuiAppTheme))
            {
                throw new ArgumentException("This property is not an SsuiTheme property", nameof(ssuiThemeProperty));
            }
            return SetBinding(brushProperty, new Binding(ssuiThemeProperty.Name) { Source = source });
        }

        /// <summary>
        /// The backing routed event object for <see cref="SsuiThemeChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent SsuiThemeAppliedEvent = EventManager.RegisterRoutedEvent(
            nameof(SsuiThemeApplied), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ThemedControl));

        /// <summary>
        /// Raised after <see cref="ApplySsuiTheme(SsuiTheme, bool, bool)"/> has completed, and the theme has been applied to the control.
        /// </summary>
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
    public abstract class ThemedContentControl : ContentControl
    {

        /// <summary>
        /// Get or set the theme to apply to the appearance of this control.
        /// </summary>
        public SsuiTheme SsuiTheme { get => (SsuiTheme)GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }

        /// <summary>The backing dependency property for <see cref="SsuiTheme"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SsuiThemeProperty = ThemedControl.SsuiThemeProperty.AddOwner(typeof(ThemedContentControl));

        ///// <summary>
        ///// Get the <see cref="SsuiTheme"/> value applied to the specified framework element.
        ///// </summary>
        ///// <param name="tc">the element to get the <see cref="ThemedControl.SsuiThemeProperty"/> value of</param>
        ///// <returns></returns>
        //public static SsuiTheme GetSsuiThemeProperty(FrameworkElement tc)
        //{
        //    return (SsuiTheme)tc.GetValue(ThemedControl.SsuiThemeProperty);
        //}

        ///// <summary>
        ///// Set the <see cref="SsuiTheme"/> value for a specified framework element.
        ///// </summary>
        ///// <param name="tc"></param>
        ///// <param name="value"></param>
        //public static void SetSsuiThemeProperty(FrameworkElement tc, SsuiTheme value)
        //{
        //    tc.SetValue(ThemedControl.SsuiThemeProperty, value);
        //}

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

        /// <summary>
        /// Update the control's appearance by using <see cref="ApplySsuiTheme(SsuiTheme)"/> and then raise the <see cref="SsuiThemeChanged"/> event.
        /// </summary>
        protected internal void ApplyAndRaiseTheme()
        {
            ApplySsuiTheme(SsuiTheme);

            RoutedEventArgs re = new RoutedEventArgs(SsuiThemeChangedEvent, this);
            RaiseEvent(re);
        }

        #region UseLightBorder

        /// <summary>
        /// Get or set if the <see cref="SsuiTheme.LightBorderBrush"/> should be used for the border of this control, rather than the <see cref="SsuiTheme.BorderBrush"/>.
        /// </summary>
        public bool UseLightBorder { get => (bool)GetValue(UseLightBorderProperty); set => SetValue(UseLightBorderProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseLightBorder"/>. See the related property for details.</summary>
        public static DependencyProperty UseLightBorderProperty
            = DependencyProperty.Register(nameof(UseLightBorder), typeof(bool), typeof(ThemedContentControl),
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
        /// This should already be set if you're putting this control in a <c>ThemedWindow</c> and inheriting from that SsuiTheme property.
        /// </remarks>
        public bool UseAccentTheme { get => (bool)GetValue(UseAccentThemeProperty); set => SetValue(UseAccentThemeProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseAccentTheme"/>. See the related property for details.</summary>
        public static DependencyProperty UseAccentThemeProperty
            = DependencyProperty.Register(nameof(UseAccentTheme), typeof(bool), typeof(ThemedContentControl),
            new FrameworkPropertyMetadata(false, (d, e) => d.PerformAs<ThemedContentControl>((o) => o.OnUseAccentColorsChange(o, e))));

        void OnUseAccentColorsChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_skipReapply) return;
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);
        }

        #endregion

        #region ApplySsuiTheme

        bool _skipReapply = false;

        /// <summary>
        /// Check the properties to make sure they match these values. If not, update the properties and then recall the ApplySsuiTheme function.
        /// </summary>
        /// <param name="ssuiTheme">the inputted SsuiTheme value</param>
        /// <param name="useLightBorder">the inputted UseLightBorder value</param>
        /// <param name="useAccentTheme">the inputted UseAccentTheme value</param>
        /// <returns></returns>
        protected bool CheckAndUpdateProperties(SsuiTheme ssuiTheme, bool useLightBorder, bool useAccentTheme)
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
        public void ApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {
            if (!CheckAndUpdateProperties(ssuiTheme, useLightBorder, useAccentTheme)) return;

            OnApplySsuiTheme(ssuiTheme, useLightBorder, useAccentTheme);

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
                ApplyBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty, sat.AccentTheme);
                // Foreground = sat.AccentTheme.Foreground; // we'll see if I need this line
            }
            else
            {
                var bb = ApplyBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty);
                // Foreground = ssuiTheme.Foreground; // we'll see if I need this line
            }
        }

        ///// <summary>
        ///// Create and set a binding for a brush property, by binding it to a property in <see cref="SolidShineUi.SsuiTheme"/>.
        ///// </summary>
        ///// <param name="brushProperty">the property on this control to bind</param>
        ///// <param name="ssuiThemePropertyName">the name of the property in <see cref="SolidShineUi.SsuiTheme"/> to bind this control's property to</param>
        //protected void ApplyBinding(DependencyProperty brushProperty, string ssuiThemePropertyName)
        //{
        //    SetBinding(brushProperty, new Binding(ssuiThemePropertyName) { Source = SsuiTheme });
        //}

        /// <summary>
        /// Create and set a binding for a brush property, by binding it to a property in <see cref="SolidShineUi.SsuiTheme"/>.
        /// </summary>
        /// <param name="brushProperty">the property on this control to bind</param>
        /// <param name="ssuiThemeProperty">the property in <see cref="SolidShineUi.SsuiTheme"/> to bind this control's property to</param>
        /// <exception cref="ArgumentException">
        /// thrown if <paramref name="ssuiThemeProperty"/> is not a property in <see cref="SolidShineUi.SsuiTheme"/> or <see cref="SsuiAppTheme"/>
        /// </exception>
        protected BindingExpressionBase ApplyBinding(DependencyProperty brushProperty, DependencyProperty ssuiThemeProperty)
        {
            return ApplyBinding(brushProperty, ssuiThemeProperty, SsuiTheme);
        }

        /// <summary>
        /// Create and set a binding for a brush property, by binding it to a property in <see cref="SolidShineUi.SsuiTheme"/>.
        /// </summary>
        /// <param name="brushProperty">the property on this control to bind</param>
        /// <param name="ssuiThemeProperty">the name of the property in <see cref="SolidShineUi.SsuiTheme"/> to bind this control's property to</param>
        /// <param name="source">the specific SsuiTheme object to bind to (such as <see cref="SsuiAppTheme"/>'s AccentTheme or SubitemTheme)</param>
        /// <exception cref="ArgumentException">
        /// thrown if <paramref name="ssuiThemeProperty"/> is not a property in <see cref="SolidShineUi.SsuiTheme"/> or <see cref="SsuiAppTheme"/>
        /// </exception>
        protected BindingExpressionBase ApplyBinding(DependencyProperty brushProperty, DependencyProperty ssuiThemeProperty, SsuiTheme source)
        {
            if (ssuiThemeProperty.OwnerType != typeof(SsuiTheme) || ssuiThemeProperty.OwnerType != typeof(SsuiAppTheme))
            {
                throw new ArgumentException("This property is not an SsuiTheme property", nameof(ssuiThemeProperty));
            }
            return SetBinding(brushProperty, new Binding(ssuiThemeProperty.Name) { Source = source });
        }

        /// <summary>
        /// The backing routed event object for <see cref="SsuiThemeChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent SsuiThemeAppliedEvent = EventManager.RegisterRoutedEvent(
            nameof(SsuiThemeApplied), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ThemedContentControl));

        /// <summary>
        /// Raised after <see cref="ApplySsuiTheme(SsuiTheme, bool, bool)"/> has completed, and the theme has been applied to the control.
        /// </summary>
        public event RoutedEventHandler SsuiThemeApplied
        {
            add { AddHandler(SsuiThemeAppliedEvent, value); }
            remove { RemoveHandler(SsuiThemeAppliedEvent, value); }
        }

        #endregion
    }
}
