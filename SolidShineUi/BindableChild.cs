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

        public static readonly DependencyProperty BindableChildProperty =
            DependencyProperty.RegisterAttached("BindableChild", typeof(UIElement), typeof(BindableChild), new UIPropertyMetadata(null, BindableChildPropertyChanged));

        static void BindableChildPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Decorator ele)
            {
                ele.Child = (UIElement)e.NewValue;
            }
        }
    }

    public class BindableChildConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#if NETCOREAPP
            if (value == null) return null!;
#else
            if (value == null) return null;
#endif

            //var border = new Border();
            //border.Child = (value as UIElement);
            //return border;

            return new Border()
            {
                Child = value as UIElement
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Border b)
            {
                return b.Child;
            }
            else
            {
#if NETCOREAPP
                return null!;
#else
                return null;
#endif
            }
        }
    }
}