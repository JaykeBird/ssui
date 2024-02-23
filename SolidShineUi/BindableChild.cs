using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

// unfortunately, I didn't write here where I had originally gotten this code from
// but it appears to be this:
// https://social.msdn.microsoft.com/Forums/sqlserver/en-US/62daec04-4437-4695-8d1f-8054459ea340/a-converter-return-a-border-it-doesnt-work?forum=wpf
// thanks to Pete Laker

namespace SolidShineUi
{
    /// <summary>
    /// Add the ability to set a child to an object via WPF binding.
    /// </summary>
    public class BindableChild
    {
        /// <summary>
        /// Get the bindable child element for this object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static UIElement GetBindableChild(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(BindableChildProperty);
        }

        /// <summary>
        /// Set the bindable child element for this object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetBindableChild(DependencyObject obj, UIElement value)
        {
            obj.SetValue(BindableChildProperty, value);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty BindableChildProperty =
            DependencyProperty.RegisterAttached("BindableChild", typeof(UIElement), typeof(BindableChild), new UIPropertyMetadata(null, BindableChildPropertyChanged));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        static void BindableChildPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Decorator ele)
            {
                ele.Child = (UIElement)e.NewValue;
            }
        }
    }

    /// <summary>
    /// A converter used to set the child of a control to a bound element.
    /// </summary>
    public class BindableChildConverter : IValueConverter
    {
        /// <summary>
        /// Convert an UIElement to be the child of a Border.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;

            return new Border()
            {
                Child = value as UIElement
            };
        }

        /// <summary>
        /// Convert a Border's child to get the UIElement it represents.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Border b)
            {
                return b.Child;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}