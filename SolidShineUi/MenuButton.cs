using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SolidShineUi
{
    /// <summary>
    /// A flat-styled button that displays a menu when it is clicked.
    /// </summary>
    public class MenuButton : FlatButton
    {

        static MenuButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuButton), new FrameworkPropertyMetadata(typeof(MenuButton)));
        }

        public MenuButton()
        {
            ColorSchemeChanged += OnColorSchemeChanged;
            Click += MenuButton_Click;
        }

        private void OnColorSchemeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            ColorScheme cs = (e.NewValue as ColorScheme)!;
#else
            ColorScheme cs = e.NewValue as ColorScheme;
#endif

            if (Menu != null) Menu.ApplyColorScheme(cs);
        }

#if NETCOREAPP
        /// <summary>
        /// Get or set the menu that appears when the button is clicked.
        /// </summary>
        public ContextMenu? Menu { get; set; } = null;

        /// <summary>
        /// This event is raised when this MenuButtons's menu is closed.
        /// </summary>
        public EventHandler? MenuClosed;
#else
        /// <summary>
        /// Get or set the menu that appears when the button is clicked.
        /// </summary>
        [Category("Common")]
        public ContextMenu Menu { get; set; } = null;

        /// <summary>
        /// This event is raised when this MenuButtons's menu is closed.
        /// </summary>
        public EventHandler MenuClosed;
#endif

        /// <summary>
        /// Get or set the placement mode for the MenuButton's menu.
        /// </summary>
        public System.Windows.Controls.Primitives.PlacementMode MenuPlacement { get; set; } = System.Windows.Controls.Primitives.PlacementMode.Bottom;

        /// <summary>
        /// Get or set the placement rectangle for the MenuButton's menu. This sets the area relative to the button that the menu is positioned.
        /// </summary>
        public Rect MenuPlacementRectangle { get; set; } = Rect.Empty;

        /// <summary>
        /// Get or set if the menu should close automatically. Remember to set the <c>StaysOpenOnClick</c> property for child menu items as well.
        /// </summary>
        public bool StaysOpen
        {
            get
            {
                if (Menu != null) return Menu.StaysOpen;
                else return false;
            }
            set
            {
                if (Menu != null) Menu.StaysOpen = value;
            }
        }

        public static readonly DependencyProperty ShowMenuArrowProperty = DependencyProperty.Register(
            "ShowMenuArrow", typeof(bool), typeof(MenuButton),
            new PropertyMetadata(true));

        /// <summary>
        /// Get or set if an arrow should be shown to the right of the button content to indicate the button as a menu button.
        /// </summary>
        public bool ShowMenuArrow
        {
            get => (bool)GetValue(ShowMenuArrowProperty);
            set => SetValue(ShowMenuArrowProperty, value);
        }

        public new void ApplyColorScheme(ColorScheme cs, bool transparentBack = false, bool useAccentColors = false)
        {
            base.ApplyColorScheme(cs, transparentBack, useAccentColors);
            if (Menu != null) Menu.ApplyColorScheme(cs);
        }

        public new void ApplyColorScheme(HighContrastOption hco, bool transparentBack = false)
        {
            base.ApplyColorScheme(hco, transparentBack);
            if (Menu != null) Menu.ApplyColorScheme(hco);
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (Menu != null)
            {
                Menu.Placement = MenuPlacement;
                Menu.PlacementTarget = this;
                Menu.PlacementRectangle = MenuPlacementRectangle;
                Menu.HorizontalOffset = 0;
                Menu.VerticalOffset = -1;
                Menu.IsOpen = true;
                Menu.Closed += Menu_Closed;
            }
        }

        private void Menu_Closed(object sender, RoutedEventArgs e)
        {
            MenuClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}
