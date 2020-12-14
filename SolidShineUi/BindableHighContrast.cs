using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SolidShineUi
{
    // https://stackoverflow.com/questions/18708794/how-can-i-fall-back-to-a-high-contrast-color-in-wpf

    /// <summary>
    /// When set up properly, allows a WPF control style to be triggered when the Windows High Contrast mode setting is changed. See remarks for details.
    /// </summary>
    /// <remarks>
    /// For the control or style in question that you want to monitor changes to high contrast in, set the property 
    /// <c>flat:BindableHighContrast.HighContrast = "{DynamicResource ResourceKey={x:Static Member=SystemParameters.HighContrastKey}}"</c> in the control or style.
    /// Then you can use the trigger as such: <c>&lt;Trigger Property="flat:BindableHighContrast.HighContrast" Value="True"&gt; ... &lt;/Trigger&gt;</c>.
    /// Visit https://stackoverflow.com/questions/18708794/how-can-i-fall-back-to-a-high-contrast-color-in-wpf for more information and source.
    /// This class is identical to the "SystemParameterProperties" class in the accepted answer.
    /// </remarks>
    public static class BindableHighContrast
    {
        public static readonly DependencyProperty HighContrastProperty =
            DependencyProperty.RegisterAttached("HighContrast", typeof(bool), typeof(BindableHighContrast),
                new FrameworkPropertyMetadata() { Inherits = true });

        public static bool GetHighContrast(DependencyObject obj)
        {
            return (bool)obj.GetValue(HighContrastProperty);
        }

        public static void SetHighContrast(DependencyObject obj, bool value)
        {
            obj.SetValue(HighContrastProperty, value);
        }
    }
}
