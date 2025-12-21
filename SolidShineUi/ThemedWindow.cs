using SolidShineUi.Utils;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SolidShineUi
{
    /// <summary>
    /// A WPF window that supports theming by Solid Shine UI's <see cref="SsuiAppTheme"/>. Set the <see cref="SsuiTheme"/> property to apply this theme
    /// to all child Solid Shine UI controls.
    /// </summary>
    /// <remarks>
    /// For a window that also has Solid Shine UI's stylings and extra features, see <see cref="FlatWindow"/>.
    /// </remarks>
    public class ThemedWindow : Window
    {
        /// <summary>
        /// Create a ThemedWindow.
        /// </summary>
        public ThemedWindow() : base()
        {
            // set a binding so that the attached property for this window will be updated whenever we change SsuiTheme.
            // Although I think ONeWay is automatically implied if not set, I explicitly want to set it here.
            // We cannot do TwoWay, since ThemedControl's SsuiTheme is a SsuiTheme type, and this window's SsuiTheme is a SsuiAppTheme type
            // and SSuiAppTheme can be used as a SsuiTheme, but it does not work the other way around

            SetBinding(ThemedControl.SsuiThemeProperty, new Binding(nameof(SsuiTheme)) { Source = this, Mode = BindingMode.OneWay });
        }

        /// <summary>
        /// Get or set the SSUI theme to use for this window. This theme will affect the appearance of this window, as well as any child controls that
        /// support SSUI theming.
        /// </summary>
        /// <remarks>
        /// This property will set the <see cref="ThemedControl.SsuiThemeProperty"/> property for this window, which any child controls will inherit.
        /// Setting that attached property on this window will not update this property.
        /// Note that setting this will likely overwrite any brush values that are set in all child controls (that have a SsuiTheme property to inherit),
        /// so if you had set a control to use a specific brush, either re-update that brush after updating this property, or include that
        /// brush property's name in that child control's <see cref="ThemedControl.ThemeValueExclude"/>.
        /// <para/>
        /// Child controls that inherit this theme can also use this theme's accent theme (if set) by setting <see cref="ThemedControl.UseAccentTheme"/>
        /// to <c>true</c> on that control.
        /// </remarks>
        [Category("Appearance")]
        public SsuiAppTheme SsuiTheme { get => (SsuiAppTheme)GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }

        /// <summary>The backing dependency property for <see cref="SsuiTheme"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SsuiThemeProperty
            = DependencyProperty.Register(nameof(SsuiTheme), typeof(SsuiAppTheme), typeof(ThemedWindow),
            new FrameworkPropertyMetadata(new SsuiAppTheme(), (d, e) => d.PerformAs<ThemedWindow>((o) => o.OnSsuiThemeChanged(e))));

        /// <summary>
        /// The backing routed event object for <see cref="SsuiThemeChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent SsuiThemeChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(SsuiThemeChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<SsuiAppTheme>), typeof(ThemedWindow));

        /// <summary>
        /// Raised when the <see cref="SsuiTheme"/> property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<SsuiAppTheme> SsuiThemeChanged
        {
            add { AddHandler(SsuiThemeChangedEvent, value); }
            remove { RemoveHandler(SsuiThemeChangedEvent, value); }
        }

        private void OnSsuiThemeChanged(DependencyPropertyChangedEventArgs e)
        {
            // the binding should get this to update the ThemedControl.SsuiThemeProperty for this window

            if (e.NewValue is SsuiAppTheme snew)
            {
                // apply visuals
                ApplySsuiTheme(snew);

                // call routed event
                RoutedPropertyChangedEventArgs<SsuiAppTheme> re = new RoutedPropertyChangedEventArgs<SsuiAppTheme>
                    ((SsuiAppTheme)e.OldValue, snew, SsuiThemeChangedEvent);
                re.Source = this;
                RaiseEvent(re);
            }
        }


        /// <summary>
        /// Apply a <see cref="SsuiAppTheme"/> to this window. This applies a consistent appearance to this window and child controls.
        /// </summary>
        /// <param name="ssuiTheme">the theme value to apply</param>
        public void ApplySsuiTheme(SsuiAppTheme ssuiTheme)
        {
            if (ssuiTheme != SsuiTheme)
            {
                // update the property first, and then that'll call back here
                SsuiTheme = ssuiTheme;
                return;
            }

            if (ssuiTheme != null) OnApplySsuiTheme(ssuiTheme);
        }

        /// <summary>
        /// Raised when the SsuiTheme value is changed, or when <see cref="ApplySsuiTheme(SsuiAppTheme)"/> is called.
        /// Inheriting windows should override this to update their brushes to reflect this new theme.
        /// </summary>
        /// <param name="ssuiTheme">the theme value to apply</param>
        /// <remarks>
        /// The base method will by default set the <c>Background</c>, <c>BorderBrush</c>, and <c>Foreground</c> values to match the theme; 
        /// if a custom brush is needed for those properties, then update it again after this is called.
        /// </remarks>
        protected virtual void OnApplySsuiTheme(SsuiAppTheme ssuiTheme)
        {
            ApplyThemeBinding(BackgroundProperty, SsuiAppTheme.WindowBackgroundProperty, ssuiTheme);
            ApplyThemeBinding(BorderBrushProperty, SsuiAppTheme.BorderBrushProperty, ssuiTheme);
            ApplyThemeBinding(ForegroundProperty, SsuiAppTheme.ForegroundProperty, ssuiTheme);

            //Background = ssuiTheme.WindowBackground;
            //BorderBrush = ssuiTheme.BorderBrush;
            //Foreground = ssuiTheme.Foreground;
        }

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
        public static readonly DependencyProperty ThemeValueExcludeProperty = ThemedControl.ThemeValueExcludeProperty.AddOwner(typeof(ThemedWindow));

        #endregion


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
                return SetBinding(brushProperty, SolidShineUi.SsuiTheme.CreateBinding(ssuiThemeProperty, source));
            }
        }

    }
}
