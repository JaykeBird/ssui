using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Data;
using Avalonia.Controls.Primitives;

namespace SolidShineUi
{
    /// <summary>
    /// An Avalonia control that has extra properties to apply theming from a <see cref="SsuiTheme"/>.
    /// For using SsuiTheme with a templated control, see ThemedTemplatedControl.
    /// </summary>
    public class ThemedControl : Control
    {

        /// <summary>
        /// Create a ThemedControl.
        /// </summary>
        public ThemedControl() { }

        #region SsuiTheme Property

        /// <summary>
        /// Get or set the theme to apply to the appearance of this control. This will bind the control's brushes to this theme.
        /// </summary>
        /// <remarks>
        /// This theme can be used to centrally apply a consistent appearance to all ThemedControls. Once a theme is set, the control's brushes are bound to the relevant
        /// values in this SsuiTheme, allowing you to update the control's appearance by changing the relevant values in the SsuiTheme. 
        /// This does not have the issue present in the WPF version where setting a theme would overwrite your locally-set brush values.
        /// Instead, setting a theme will operate similarly to applying a style to this control.
        /// <para/>
        /// This is set to null by default, so that brushes are not automatically bound to any SsuiTheme.
        /// </remarks>
        [Category("Appearance")]
        [Description("Get or set the theme to apply to the appearance of this control.")]
        public SsuiTheme? SsuiTheme { get => GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }

        /// <summary>The backing styled property for <see cref="SsuiTheme"/>. See the related property for details.</summary>
        public static readonly StyledProperty<SsuiTheme?> SsuiThemeProperty
            = AvaloniaProperty.Register<ThemedControl, SsuiTheme?>(nameof(SsuiTheme), null, inherits: true);

        /// <summary>
        /// The backing routed event object for <see cref="SsuiThemeChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent<RoutedPropertyChangedEventArgs> SsuiThemeChangedEvent =
            RoutedEvent.Register<ThemedControl, RoutedPropertyChangedEventArgs>(nameof(SsuiThemeChanged), RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when the <see cref="SsuiTheme"/> property is changed.
        /// </summary>
        public event EventHandler<RoutedPropertyChangedEventArgs>? SsuiThemeChanged
        {
            add => AddHandler(SsuiThemeChangedEvent, value);
            remove => RemoveHandler(SsuiThemeChangedEvent, value);
        }

        /// <summary>
        /// Raised when the <see cref="SsuiTheme"/> property is changed.
        /// </summary>
        /// <param name="e">the event argument object containing information about the change</param>
        protected virtual void OnSsuiThemeChanged(AvaloniaPropertyChangedEventArgs e)
        {
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);

            RaiseEvent(new RoutedPropertyChangedEventArgs(SsuiThemeChangedEvent, e));
        }

        #endregion

        #region UseLightBorder

        /// <summary>
        /// Get or set if the <see cref="SsuiTheme.LightBorderBrush"/> should be used for the border of this control when applying a SsuiTheme, rather than the <see cref="SsuiTheme.BorderBrush"/>.
        /// </summary>
        [Category("Appearance"), DefaultValue(false)]
        [Description("Get or set if the LightBorderBrush should be used for the border of this control, when applying a SsuiTheme.")]
        public bool UseLightBorder { get => GetValue(UseLightBorderProperty); set => SetValue(UseLightBorderProperty, value); }

        /// <summary>The backing styled property for <see cref="UseLightBorder"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> UseLightBorderProperty
            = AvaloniaProperty.Register<ThemedControl, bool>(nameof(UseLightBorder), false);

        private void OnUseLightBorderChange(AvaloniaPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region UseAccentTheme

        /// <summary>
        /// Get or set if an accent theme should be used rather than the standard theme for this control. The accent theme, when used in moderation,
        /// can be used to help particular controls stand out in the UI.
        /// </summary>
        /// <remarks>
        /// The value of the <see cref="SsuiTheme"/> property needs to be of an <see cref="SsuiAppTheme"/> type, or otherwise this property does nothing.
        /// </remarks>
        [Category("Appearance"), DefaultValue(false)]
        [Description("Get or set if the SsuiTheme's accent theme should be used rather than the standard theme for this control.")]
        public bool UseAccentTheme { get => GetValue(UseAccentThemeProperty); set => SetValue(UseAccentThemeProperty, value); }

        /// <summary>The backing styled property for <see cref="UseAccentTheme"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> UseAccentThemeProperty
            = AvaloniaProperty.Register<ThemedControl, bool>(nameof(UseAccentTheme), false);

        private void OnUseAccentThemeChange(AvaloniaPropertyChangedEventArgs e)
        {

        }

        #endregion

        /// <summary>
        /// Called when a avalonia property changes on the object.
        /// </summary>
        /// <param name="change">The property change details.</param>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            switch (change.Property.Name)
            {
                case nameof(SsuiTheme):
                    OnSsuiThemeChanged(change);
                    break;
                case nameof(UseLightBorder):
                    OnUseLightBorderChange(change);
                    break;
                case nameof(UseAccentTheme):
                    OnUseAccentThemeChange(change);
                    break;
            }
        }

        #region ApplySsuiTheme

        bool _skipReapply = false;

        /// <summary>
        /// Check the control's properties to make sure they match these values. If not, this will update the properties and then re-call the ApplySsuiTheme function.
        /// </summary>
        /// <param name="ssuiTheme">the inputted SsuiTheme value</param>
        /// <param name="useLightBorder">the inputted UseLightBorder value</param>
        /// <param name="useAccentTheme">the inputted UseAccentTheme value</param>
        bool CheckAndUpdateProperties(SsuiTheme? ssuiTheme, bool useLightBorder, bool useAccentTheme)
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
        public void ApplySsuiTheme(SsuiTheme? ssuiTheme)
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
        public void ApplySsuiTheme(SsuiTheme? ssuiTheme, bool useLightBorder, bool useAccentTheme)
        {
            if (!CheckAndUpdateProperties(ssuiTheme, useLightBorder, useAccentTheme)) return;

            if (ssuiTheme != null) OnApplySsuiTheme(ssuiTheme, useLightBorder, useAccentTheme);

            RaiseEvent(new RoutedEventArgs(SsuiThemeAppliedEvent));
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
        /// The base method will by default set the <see cref="TemplatedControl.BorderBrush"/> value to match the theme; if a different
        /// border brush is needed, then update it again in your override method.
        /// </remarks>
        protected virtual void OnApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {

        }

        /// <summary>
        /// The backing routed event for <see cref="SsuiThemeApplied"/>. See the related event for details.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> SsuiThemeAppliedEvent =
            RoutedEvent.Register<ThemedControl, RoutedEventArgs>(nameof(SsuiThemeApplied), RoutingStrategies.Bubble);

        /// <summary>
        /// Raised whenenver the SsuiTheme's values have been applied to this control.
        /// </summary>
        public event EventHandler<RoutedEventArgs>? SsuiThemeApplied
        {
            add => AddHandler(SsuiThemeAppliedEvent, value);
            remove => RemoveHandler(SsuiThemeAppliedEvent, value);
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
        protected BindingExpressionBase? ApplyThemeBinding(AvaloniaProperty brushProperty, AvaloniaProperty ssuiThemeProperty)
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
        protected BindingExpressionBase? ApplyThemeBinding(AvaloniaProperty brushProperty, AvaloniaProperty ssuiThemeProperty, SsuiTheme? source)
        {

            // if the theme value provided is null, then we'll just undo any binding that exists (which should undo it to the previous SsuiTheme)
            if (source == null)
            {

                // ClearBinding(this, brushProperty);
                return null;
            }
            else
            {
                return Bind(brushProperty, SsuiTheme.CreateBinding(ssuiThemeProperty, source));
            }
        }

        #endregion

    }
}
