using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace SolidShineUi
{
    /// <summary>
    /// A flat-styled button that displays a menu when it is clicked.
    /// </summary>
    [Localizability(LocalizationCategory.Button)]
    public class MenuButton : FlatButton
    {

        static MenuButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuButton), new FrameworkPropertyMetadata(typeof(MenuButton)));
        }

        /// <summary>
        /// Create a new MenuButton.
        /// </summary>
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


        #region Menu

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register("Menu", typeof(ContextMenu), typeof(MenuButton),
            new PropertyMetadata(null));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

#if NETCOREAPP
        /// <summary>
        /// Get or set the menu that appears when the button is clicked.
        /// </summary>
        [Category("Common")]
        public ContextMenu? Menu
        {
            get { return (ContextMenu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// This event is raised when this MenuButtons's menu is closed.
        /// </summary>
        public EventHandler? MenuClosed;
#else
        /// <summary>
        /// Get or set the menu that appears when the button is clicked.
        /// </summary>
        [Category("Common")]
        public ContextMenu Menu
        {
            get { return (ContextMenu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// This event is raised when this MenuButtons's menu is closed.
        /// </summary>
        public EventHandler MenuClosed;
#endif

        /// <summary>
        /// Get or set if the menu should close automatically. Remember to set the <c>StaysOpenOnClick</c> property for child menu items as well.
        /// </summary>
        [Category("Common")]
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

        #endregion

        #region Placement

        /// <summary>
        /// Get or set the placement mode for the MenuButton's menu.
        /// </summary>
        public PlacementMode MenuPlacement { get => (PlacementMode)GetValue(MenuPlacementProperty); set => SetValue(MenuPlacementProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuPlacement"/>. See the related property for details.</summary>
        public static DependencyProperty MenuPlacementProperty
            = DependencyProperty.Register("MenuPlacement", typeof(PlacementMode), typeof(MenuButton),
            new FrameworkPropertyMetadata(PlacementMode.Bottom));


        /// <summary>
        /// Get or set the placement target for the MenuButton's menu. Set to <c>null</c> to set the target to this MenuButton.
        /// </summary>
#if NETCOREAPP
        public UIElement? MenuPlacementTarget { get => (UIElement)GetValue(MenuPlacementTargetProperty); set => SetValue(MenuPlacementTargetProperty, value); }
#else
        public UIElement MenuPlacementTarget { get => (UIElement)GetValue(MenuPlacementTargetProperty); set => SetValue(MenuPlacementTargetProperty, value); }
#endif

        /// <summary>The backing dependency property for <see cref="MenuPlacementTarget"/>. See the related property for details.</summary>
        public static DependencyProperty MenuPlacementTargetProperty
            = DependencyProperty.Register("MenuPlacementTarget", typeof(UIElement), typeof(MenuButton),
            new FrameworkPropertyMetadata(null));


        /// <summary>
        /// Get or set the placement rectangle for the MenuButton's menu. This sets the area relative to the button that the menu is positioned.
        /// </summary>
        public Rect MenuPlacementRectangle { get => (Rect)GetValue(MenuPlacementRectangleProperty); set => SetValue(MenuPlacementRectangleProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuPlacementRectangle"/>. See the related property for details.</summary>
        public static DependencyProperty MenuPlacementRectangleProperty
            = DependencyProperty.Register("MenuPlacementRectangle", typeof(Rect), typeof(MenuButton),
            new FrameworkPropertyMetadata(Rect.Empty));

        #endregion


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ShowMenuArrowProperty = DependencyProperty.Register(
            "ShowMenuArrow", typeof(bool), typeof(MenuButton),
            new PropertyMetadata(true));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set if an arrow should be shown to the right of the button content to indicate the button as a menu button.
        /// </summary>
        [Category("Common")]
        public bool ShowMenuArrow
        {
            get => (bool)GetValue(ShowMenuArrowProperty);
            set => SetValue(ShowMenuArrowProperty, value);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty KeepMenuArrowOnRightProperty = DependencyProperty.Register(
            "KeepMenuArrowOnRight", typeof(bool), typeof(MenuButton),
            new PropertyMetadata(false));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set if the arrow should be kept to the right side of the button, even if the content of the button is left or center aligned.
        /// </summary>
        [Category("Common")]
        public bool KeepMenuArrowOnRight
        {
            get => (bool)GetValue(KeepMenuArrowOnRightProperty);
            set => SetValue(KeepMenuArrowOnRightProperty, value);
        }

        /// <summary>
        /// Apply a color scheme to this control, and set some other optional appearance settings. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply</param>
        /// <param name="transparentBack">Set if the button should have no background when not focused or highlighted. This can also be achieved with the <c>TransparentBack</c> property.</param>
        /// <param name="useAccentColors">Set if accent colors should be used for this button, rather than the main color scheme colors.
        /// This can also be achieved with the <c>UseAccentColors</c> property.
        /// </param>
        public new void ApplyColorScheme(ColorScheme cs, bool transparentBack = false, bool useAccentColors = false)
        {
            base.ApplyColorScheme(cs, transparentBack, useAccentColors);
            if (Menu != null) Menu.ApplyColorScheme(cs);
        }

        /// <summary>
        /// Internal method for opening up the menu when the button is clicked
        /// </summary>
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (Menu != null)
            {
                Menu.Placement = MenuPlacement;
                Menu.PlacementTarget = MenuPlacementTarget ?? this;
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
