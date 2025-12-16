using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// A WPF Border with a <c>Foreground</c> property.
    /// </summary>
    /// <remarks>
    /// This UI element uses the Control's <c>Foreground</c> property, so it participates in the inheritance propogation.
    /// </remarks>
    public class ForeBorder : Border
    {

        public DependencyProperty ForegroundProperty = Control.ForegroundProperty.AddOwner(typeof(ForeBorder));

        public Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty); 
            set => SetValue(ForegroundProperty, value);
        }

    }
}
