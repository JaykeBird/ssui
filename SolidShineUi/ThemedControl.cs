using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SolidShineUi
{
    /// <summary>
    /// A WPF control that has extra properties to apply theming from an <see cref="SsuiTheme"/>.
    /// </summary>
    public abstract class ThemedControl : Control
    {
        static ThemedControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ThemedControl), new FrameworkPropertyMetadata(typeof(ThemedControl)));
        }

        /// <summary>
        /// Create a ThemedControl.
        /// </summary>
        public ThemedControl()
        {
            // according to this, this might be a memory leak issue, I'll have to see
            // https://agsmith.wordpress.com/2008/04/07/propertydescriptor-addvaluechanged-alternative/
            DependencyPropertyDescriptor.FromProperty(SsuiThemeProperty, typeof(ThemedControl)).AddValueChanged(this, (s, e) => OnSsuiThemeChanged());
        }

        /// <summary>
        /// Get or set the theme to apply to the appearance of this control.
        /// </summary>
        public SsuiTheme SsuiTheme { get => (SsuiTheme)GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }

        /// <summary>The backing dependency property for <see cref="SsuiTheme"/>. See the related property for details.</summary>
        public static DependencyProperty SsuiThemeProperty
            = DependencyProperty.RegisterAttached(nameof(SsuiTheme), typeof(SsuiTheme), typeof(ThemedControl),
            new FrameworkPropertyMetadata(new SsuiTheme(), FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Get the <see cref="SsuiTheme"/> value applied to the specified framework element.
        /// </summary>
        /// <param name="tc">the element to get the <see cref="SsuiThemeProperty"/> value of</param>
        /// <returns></returns>
        public static SsuiTheme GetSsuiThemeProperty(FrameworkElement tc)
        {
            return (SsuiTheme)tc.GetValue(SsuiThemeProperty);
        }

        /// <summary>
        /// Set the <see cref="SsuiTheme"/> value for a specified framework element.
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="value"></param>
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

        private void OnSsuiThemeChanged()
        {
            //RoutedPropertyChangedEventArgs<SsuiTheme> re = new RoutedPropertyChangedEventArgs<SsuiTheme>
            //    ((SsuiTheme)e.OldValue, (SsuiTheme)e.NewValue, SsuiThemeChangedEvent);
            RoutedEventArgs re = new RoutedEventArgs(SsuiThemeChangedEvent, this);
            RaiseEvent(re);
        }

        /// <summary>
        /// Apply a <see cref="SsuiTheme"/> to this control. This applies a consistent appearance to this control and child controls.
        /// </summary>
        /// <param name="ssuiTheme">the theme value to apply</param>
        public abstract void ApplySsuiTheme(SsuiTheme ssuiTheme);

        /// <summary>
        /// Apply a <see cref="SsuiTheme"/> to this control. This applies a consistent appearance to this control and child controls.
        /// </summary>
        /// <param name="ssuiTheme">the theme value to apply</param>
        /// <param name="useLightBorder">whether a light border color should be used in place of the standard one</param>
        /// <param name="useAccentColors">
        /// whether the accent theme should be used in place of the standard one; for this to apply, <paramref name="ssuiTheme"/>
        /// should be of type <see cref="SsuiAppTheme"/>
        /// </param>
        public abstract void ApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentColors = false);
    }
}
