using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi
{
    /// <summary>
    /// A helper class to easily include Solid Shine UI controls in your application. Include this in your application's <c>App.xaml</c> or <c>App.axaml</c> file.
    /// </summary>
    /// <remarks>
    /// This class contains a reference to the control themes and styles that set the appearance of the Solid Shine UI controls. Without this, you'll need to either
    /// directly include the <c>SolidShineUiControls.axaml</c> ResourceDictionary as one of your application's resources, or provide your own themes/styles for the controls
    /// that you want to use.
    /// <para/>
    /// This is not necessary to use if you're only interacting with the non-UI classes of Solid Shine UI, such as <see cref="ArithmeticParser"/> or <see cref="ColorsHelper"/>.
    /// </remarks>
    public class SolidShineUi : Styles
    {
        // based upon https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Themes.Fluent/FluentTheme.xaml.cs

        //// added this in so that it works with the designer
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public SolidShineUi() { }

        /// <summary>
        /// Create a new SolidShineUI styles object.
        /// </summary>
        /// <param name="sp">The parent's service provider.</param>
        public SolidShineUi(IServiceProvider? sp = null)
        {
            AvaloniaXamlLoader.Load(sp, this);
        }

    }
}
