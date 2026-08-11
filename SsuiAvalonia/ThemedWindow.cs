using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;

namespace SolidShineUi
{
    /// <summary>
    /// An Avalonia window that supports theming by Solid Shine UI's <see cref="SsuiAppTheme"/>. Set the <see cref="SsuiTheme"/> property to apply this theme
    /// to all child Solid Shine UI controls.
    /// </summary>
    public class ThemedWindow : Window
    {

        /// <summary>
        /// Create a ThemedWindow.
        /// </summary>
        public ThemedWindow() { }



        /// <summary>
        /// Called when an <see cref="AvaloniaProperty"/> changes on this control.
        /// </summary>
        /// <param name="change">The property change details</param>
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

        /// <inheritdoc/>
        protected override void OnUnloaded(RoutedEventArgs e)
        {
            base.OnUnloaded(e);
            foreach (var item in _themeBindings)
            {
                item.Value.Dispose();
            }
            _themeBindings.Clear();
        }

        #region SsuiTheme Property

        /// <summary>
        /// Get or set the theme to apply to the appearance of this control. This will bind the control's brushes to this theme.
        /// For a ThemedWindow, it is highly recommended for this to be of type <see cref="SsuiAppTheme"/>.
        /// </summary>
        /// <remarks>
        /// This theme can be used to centrally apply a consistent appearance to this window and all child controls that support 
        /// SSUI theming, as they will inherit the theme from this window.
        /// When this is set, this will also apply the theme's background, foreground, and border brushes to this window.
        /// <para/>
        /// This is set to null by default, so that brushes are not automatically bound to any SsuiTheme.
        /// </remarks>
        [Category("Appearance")]
        [Description("Get or set the theme to apply to the appearance of this control.")]
        public SsuiTheme? SsuiTheme { get => GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }

        /// <summary>The backing styled property for <see cref="SsuiTheme"/>. See the related property for details.</summary>
        public static readonly StyledProperty<SsuiTheme?> SsuiThemeProperty
            = ThemedControl.SsuiThemeProperty.AddOwner<ThemedWindow>(); //(new StyledPropertyMetadata<SsuiTheme?>(coerce: EnsureIsSsuiAppTheme));

        /// <summary>
        /// The backing routed event object for <see cref="SsuiThemeChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent<RoutedPropertyChangedEventArgs> SsuiThemeChangedEvent =
            RoutedEvent.Register<ThemedWindow, RoutedPropertyChangedEventArgs>(nameof(SsuiThemeChanged), RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when the <see cref="SsuiTheme"/> property is changed.
        /// </summary>
        public event EventHandler<RoutedPropertyChangedEventArgs>? SsuiThemeChanged
        {
            add => AddHandler(SsuiThemeChangedEvent, value);
            remove => RemoveHandler(SsuiThemeChangedEvent, value);
        }

        /// <summary>
        /// Coerces the value of <see cref="SsuiTheme"/>. If the value is not of type <see cref="SsuiAppTheme"/>, then this will return null.
        /// </summary>
        protected static SsuiTheme? EnsureIsSsuiAppTheme(AvaloniaObject obj, SsuiTheme? value)
        {
            if (value is SsuiAppTheme sat)
            {
                return sat;
            }
            else { return null; }
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
            = ThemedControl.UseLightBorderProperty.AddOwner<ThemedWindow>();

        private void OnUseLightBorderChange(AvaloniaPropertyChangedEventArgs e)
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
        /// </remarks>
        [Category("Appearance"), DefaultValue(false)]
        [Description("Get or set if the SsuiTheme's accent theme should be used rather than the standard theme for this control.")]
        public bool UseAccentTheme { get => GetValue(UseAccentThemeProperty); set => SetValue(UseAccentThemeProperty, value); }

        /// <summary>The backing styled property for <see cref="UseAccentTheme"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> UseAccentThemeProperty
            = ThemedControl.UseAccentThemeProperty.AddOwner<ThemedWindow>();

        private void OnUseAccentThemeChange(AvaloniaPropertyChangedEventArgs e)
        {
            if (_skipReapply) return;
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);
        }

        #endregion

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
            if (ssuiTheme is SsuiAppTheme sat)
            {
                if (useAccentTheme)
                {
                    ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty, sat.AccentTheme);
                }
                else
                {
                    ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty, sat);
                }

                ApplyThemeBinding(BackgroundProperty, SsuiAppTheme.WindowBackgroundProperty, ssuiTheme);
            }
            else
            {
                ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty);
                ApplyThemeBinding(BackgroundProperty, SsuiTheme.PanelBackgroundProperty);
            }

            ApplyThemeBinding(ForegroundProperty, SsuiTheme.ForegroundProperty, ssuiTheme);
        }

        /// <summary>
        /// The backing routed event for <see cref="SsuiThemeApplied"/>. See the related event for details.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> SsuiThemeAppliedEvent =
            RoutedEvent.Register<ThemedWindow, RoutedEventArgs>(nameof(SsuiThemeApplied), RoutingStrategies.Bubble);

        /// <summary>
        /// Raised whenenver the SsuiTheme's values have been applied to this control.
        /// </summary>
        public event EventHandler<RoutedEventArgs>? SsuiThemeApplied
        {
            add => AddHandler(SsuiThemeAppliedEvent, value);
            remove => RemoveHandler(SsuiThemeAppliedEvent, value);
        }

        /// <summary>
        /// Clear the <see cref="SsuiTheme"/> property and remove all bindings.
        /// </summary>
        public void ClearSsuiTheme()
        {
            foreach (var item in _themeBindings)
            {
                item.Value.Dispose();
            }

            _themeBindings.Clear();

            SsuiTheme = null;
        }

        #endregion

        #region Bindings

        private Dictionary<string, IDisposable> _themeBindings = [];

        /// <summary>
        /// Create and set a binding for a brush property, by binding it to a property in <see cref="SolidShineUi.SsuiTheme"/>.
        /// </summary>
        /// <param name="controlProperty">the property on this control to bind</param>
        /// <param name="ssuiThemeProperty">the property in <see cref="SolidShineUi.SsuiTheme"/> to bind this control's property to</param>
        /// <exception cref="ArgumentException">
        /// thrown if <paramref name="ssuiThemeProperty"/> is not a property in <see cref="SolidShineUi.SsuiTheme"/> or a class that inherits from SsuiTheme
        /// </exception>
        /// <remarks>
        /// If <see cref="SsuiTheme"/> is <c>null</c>, then instead the binding is cleared, and <c>null</c> is returned.
        /// </remarks>
        protected BindingExpressionBase? ApplyThemeBinding(AvaloniaProperty controlProperty, AvaloniaProperty ssuiThemeProperty)
        {
            return ApplyThemeBinding(controlProperty, ssuiThemeProperty, SsuiTheme);
        }

        /// <summary>
        /// Create and set a binding for a brush property, by binding it to a property in <see cref="SolidShineUi.SsuiTheme"/>.
        /// </summary>
        /// <param name="controlProperty">the property on this control to bind</param>
        /// <param name="ssuiThemeProperty">the name of the property in <see cref="SolidShineUi.SsuiTheme"/> to bind this control's property to</param>
        /// <param name="source">the specific SsuiTheme object to bind to (such as <see cref="SsuiAppTheme"/>'s AccentTheme or SubitemTheme)</param>
        /// <exception cref="ArgumentException">
        /// thrown if <paramref name="ssuiThemeProperty"/> is not a property in <see cref="SolidShineUi.SsuiTheme"/> or a class that inherits from SsuiTheme
        /// </exception>
        /// <remarks>
        /// If <paramref name="source"/> is <c>null</c>, then instead the binding is cleared, and <c>null</c> is returned.
        /// </remarks>
        protected BindingExpressionBase? ApplyThemeBinding(AvaloniaProperty controlProperty, AvaloniaProperty ssuiThemeProperty, SsuiTheme? source)
        {

            // if the theme value provided is null, then we'll just undo any binding that exists (which should undo it to the previous SsuiTheme)
            if (source == null)
            {
                ClearThemeBinding(controlProperty);
                return null;
            }
            else
            {
                // if we already have an existing binding, let's clear it first
                if (_themeBindings.TryGetValue(controlProperty.Name, out var existingBinding))
                {
                    existingBinding.Dispose();
                }

                BindingExpressionBase binding = Bind(controlProperty, SsuiTheme.CreateBinding(ssuiThemeProperty, source));
                _themeBindings[controlProperty.Name] = binding;

                return binding;
            }
        }

        /// <summary>
        /// Disconnect a binding on a property in this control that was previously bound to a SsuiTheme property using
        /// <c>ApplyThemeBinding</c>. This will reset the property to its default value, unless a lower priority binding
        /// or value was set.
        /// </summary>
        /// <param name="propertyName">The name of the property on this control to remove the binding for</param>
        /// <returns><c>true</c> if the binding was cleared and removed; <c>false</c> if no binding was found</returns>
        protected bool ClearThemeBinding(string propertyName)
        {
            if (_themeBindings.TryGetValue(propertyName, out var binding))
            {
                binding.Dispose();
                _themeBindings.Remove(propertyName);
                return true;
            }
            else return false;

        }

        /// <summary>
        /// Disconnect a binding on a property in this control that was previously bound to a SsuiTheme property using
        /// <c>ApplyThemeBinding</c>. This will reset the property to its default value, unless a lower priority binding
        /// or value was set.
        /// </summary>
        /// <param name="property">The property on this control to remove the binding for</param>
        /// <returns><c>true</c> if the binding was cleared and removed; <c>false</c> if no binding was found</returns>
        protected bool ClearThemeBinding(AvaloniaProperty property)
        {
            return ClearThemeBinding(property.Name);
        }

        #endregion

    }
}
