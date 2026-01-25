using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// A WPF Border with a <c>Foreground</c> property.
    /// </summary>
    /// <remarks>
    /// This UI element uses the Control's <c>Foreground</c> property, so it can by default inherit Foreground like all other standard WPF controls.
    /// However, this provides the ability to set a different <c>Foreground</c> for child controls to them inherit instead - an ability that isn't
    /// present in your basic WPF content controls like <c>Border</c> or <c>Grid</c> since they don't have a Foreground property.
    /// <para/>
    /// This is used in a few places in Solid Shine UI in order to do things like override the Foreground when the control is in a certain state.
    /// </remarks>
    public class ForeBorder : Border
    {

        /// <summary>
        /// The backing dependency property for <see cref="Foreground"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty = Control.ForegroundProperty.AddOwner(typeof(ForeBorder));

        /// <summary>
        /// Get or set the brush to use for foreground elements in child controls.
        /// </summary>
        public Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty); 
            set => SetValue(ForegroundProperty, value);
        }

    }
}
