using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SolidShineUi
{
    /// <summary>
    /// A WPF window that supports theming by Solid Shine UI's <see cref="SsuiAppTheme"/>.
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
            // set a binding so that the attached property for this window will be updated whenever we change SsuiTheme
            SetBinding(ThemedControl.SsuiThemeProperty, new Binding(nameof(SsuiTheme)) { Source = this, Mode = BindingMode.OneWay });
        }

        /// <summary>
        /// Get or set the SSUI theme to use for this window. This theme will affect the appearance of this window, as well as any child controls that
        /// support SSUI theming.
        /// </summary>
        /// <remarks>
        /// This property will set the <see cref="ThemedControl.SsuiThemeProperty"/> property for this window, which any child controls will inherit.
        /// Setting that attached property on this window will not go the other way and 
        /// </remarks>
        public SsuiAppTheme SsuiTheme { get => (SsuiAppTheme)GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }

        /// <summary>The backing dependency property for <see cref="SsuiTheme"/>. See the related property for details.</summary>
        public static DependencyProperty SsuiThemeProperty
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

            //// update the SsuiTheme attached property
            //// this will allow this to propogate for lower down the list
            //ThemedControl.SetSsuiThemeProperty(this, (SsuiAppTheme)e.NewValue);

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

            OnApplySsuiTheme(ssuiTheme);
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
            Background = ssuiTheme.WindowBackground;
            BorderBrush = ssuiTheme.BorderBrush;
            Foreground = ssuiTheme.Foreground;
        }
    }
}
