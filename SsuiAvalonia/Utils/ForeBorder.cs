using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace SolidShineUi.Utils
{

    /// <summary>
    /// An Avalonia Border with a <c>Foreground</c> property.
    /// </summary>
    /// <remarks>
    /// This UI element uses the TemplatedControl's <c>Foreground</c> property, so it can by default inherit Foreground like all other standard 
    /// Avalonia controls. However, this provides the ability to set a different <c>Foreground</c> for child controls to them inherit instead - 
    /// an ability that isn't present in your basic content controls like <c>Border</c> or <c>Grid</c> since they don't have a Foreground property.
    /// <para/>
    /// This is used in a few places in Solid Shine UI in order to do things like override the Foreground when the control is in a certain state.
    /// </remarks>
    public class ForeBorder : Border
    {

        /// <summary>
        /// Get or set the brush to use for foreground elements in child controls.
        /// </summary>
        public IBrush? Foreground { get => GetValue(ForegroundProperty); set => SetValue(ForegroundProperty, value); }

        /// <summary>The backing styled property for <see cref="Foreground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> ForegroundProperty
            = TextElement.ForegroundProperty.AddOwner<ForeBorder>();

    }
}
