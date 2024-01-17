using Avalonia;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi
{
    /// <summary>
    /// A flat-styled button that displays a menu when it is clicked.
    /// </summary>
    public class MenuButton : FlatButton
    {

        public MenuButton()
        {
            Click += MenuButton_Click;
        }

        private void OnColorSchemeChanged(ColorScheme newValue)
        {
            //Menu?.ApplyColorScheme(cs);
        }

        private void MenuButton_Click(object? sender, RoutedEventArgs e)
        {
            
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            switch (change.Property.Name)
            {
                case nameof(ColorScheme):
                    OnColorSchemeChanged(change.GetNewValue<ColorScheme>());
                    break;
            }

            base.OnPropertyChanged(change);
        }
    }
}
