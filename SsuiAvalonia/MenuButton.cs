using Avalonia;
using Avalonia.Controls;
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

        /// <summary>
        /// Create a MenuButton.
        /// </summary>
        public MenuButton()
        {
            Click += MenuButton_Click;
        }

        private void MenuButton_Click(object? sender, RoutedEventArgs e)
        {
            if (Menu != null)
            {
                Menu.Placement = MenuPlacement;
                Menu.PlacementTarget = MenuPlacementTarget ?? this;
                Menu.PlacementRect = MenuPlacementRectangle;
                Menu.HorizontalOffset = MenuHorizontalOffset;
                Menu.VerticalOffset = MenuVerticalOffset;
                Menu.Open(this);
                Menu.Closing += Menu_Closing;
                Menu.Closed += Menu_Closed;
            }
        }

        #region Color Scheme

        private void OnColorSchemeChanged(ColorScheme newValue)
        {
            Menu?.ApplyColorScheme(newValue);
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

        #endregion

        #region Menu

        /// <summary>
        /// Get or set the menu that appears when the button is clicked.
        /// </summary>
        public ContextMenu? Menu { get => GetValue(MenuProperty); set => SetValue(MenuProperty, value); }

        /// <summary>The backing styled property for <see cref="Menu"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ContextMenu?> MenuProperty
            = AvaloniaProperty.Register<MenuButton, ContextMenu?>(nameof(Menu), null);

        /// <summary>
        /// This event is raised when this MenuButtons's menu is closed.
        /// </summary>
        public EventHandler? MenuClosed;

        private void Menu_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_staysOpen) { e.Cancel = true; }
        }

        private void Menu_Closed(object? sender, RoutedEventArgs e)
        {
            MenuClosed?.Invoke(this, e);
        }

        private bool _staysOpen = false;

        /// <summary>
        /// Get or set if the menu should close automatically. If <c>true</c>, the menu will continue to remain open until this is changed back to <c>false</c>.
        /// </summary>
        /// <remarks>
        /// When this is set to <c>false</c>, the menu will close when a menu item is selected, or when the user clicks outside of the menu or moves focus.
        /// Due to the differences between how Avalonia and WPF handle their context menus, this functions a bit differently in the two versions when set to <c>true</c>.
        /// <para/>
        /// In the WPF version of Solid Shine UI, <c>StaysOpen</c> will keep the context menu open until a menu item is clicked (unless the menu item also has <c>StaysOpenOnClick</c>
        /// set to true), but other methods to close the menu don't generally work (such as clicking outside of the menu). So the best ways to close the menu is to have a "Close" menu
        /// item that doesn't have <c>StaysOpenOnClick</c> applied, or some other code or function that directly sets the menu's <c>IsOpen</c> property to <c>false</c>. You do not
        /// need to explicitly change <c>StaysOpen</c> to <c>false</c> in order to allow the menu to close, so this value can remain unchanged as long as you want this behavior.
        /// <para/>
        /// In the Avalonia version of Solid Shine UI, <c>StaysOpen</c> will set the context menu to refuse to close; when the context menu is about to close, the MenuButton cancels 
        /// that action. This also means that clicking any menu items will not close the menu, nor will setting the menu's <c>IsOpen</c> property to <c>false</c>.
        /// Instead, to close the context menu, you will need to change <c>StaysOpen</c> back to <c>false</c> before you change <c>IsOpen</c> to <c>false</c> or click out of the menu,
        /// and then you will need to re-set <c>StaysOpen</c> back to <c>true</c> whenever you want this behavior to occur again.
        /// </remarks>
        public bool StaysOpen { get => _staysOpen; set => SetAndRaise(StaysOpenProperty, ref _staysOpen, value); }

        /// <summary>The backing direct property for <see cref="StaysOpen"/>. See the related property for details.</summary>
        public static readonly DirectProperty<MenuButton, bool> StaysOpenProperty
            = AvaloniaProperty.RegisterDirect<MenuButton, bool>(nameof(StaysOpen), (s) => s.StaysOpen, (s, v) => s.StaysOpen = v, unsetValue: false);

        #region Placement

        /// <summary>
        /// Get or set the placement mode for the MenuButton's menu.
        /// </summary>
        public PlacementMode MenuPlacement { get => GetValue(MenuPlacementProperty); set => SetValue(MenuPlacementProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuPlacement"/>. See the related property for details.</summary>
        public static readonly StyledProperty<PlacementMode> MenuPlacementProperty
            = AvaloniaProperty.Register<MenuButton, PlacementMode>(nameof(MenuPlacement), PlacementMode.BottomEdgeAlignedLeft);

        /// <summary>
        /// Get or set the placement target for the MenuButton's menu. Set to <c>null</c> to set the target to this MenuButton.
        /// </summary>
        public Control? MenuPlacementTarget { get => GetValue(MenuPlacementTargetProperty); set => SetValue(MenuPlacementTargetProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuPlacementTarget"/>. See the related property for details.</summary>
        public static readonly StyledProperty<Control?> MenuPlacementTargetProperty
            = AvaloniaProperty.Register<MenuButton, Control?>(nameof(MenuPlacementTarget), null);

        /// <summary>
        /// Get or set the placement rectangle for the MenuButton's menu. This sets the area relative to the button that the menu is positioned.
        /// </summary>
        public Rect? MenuPlacementRectangle { get => GetValue(MenuPlacementRectangleProperty); set => SetValue(MenuPlacementRectangleProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuPlacementRectangle"/>. See the related property for details.</summary>
        public static readonly StyledProperty<Rect?> MenuPlacementRectangleProperty
            = AvaloniaProperty.Register<MenuButton, Rect?>(nameof(MenuPlacementRectangle), null);

        /// <summary>
        /// Get or set how far offset the menu is horizontally (left or right) from its placement target/rectangle when it's opened.
        /// </summary>
        public double MenuHorizontalOffset { get => GetValue(MenuHorizontalOffsetProperty); set => SetValue(MenuHorizontalOffsetProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuHorizontalOffset"/>. See the related property for details.</summary>
        public static readonly StyledProperty<double> MenuHorizontalOffsetProperty
            = AvaloniaProperty.Register<MenuButton, double>(nameof(MenuHorizontalOffset), 0.0);


        /// <summary>
        /// Get or set how far offset the menu is vertically (up or down) from its placement target/rectangle when it's opened.
        /// </summary>
        public double MenuVerticalOffset { get => GetValue(MenuVerticalOffsetProperty); set => SetValue(MenuVerticalOffsetProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuVerticalOffset"/>. See the related property for details.</summary>
        public static readonly StyledProperty<double> MenuVerticalOffsetProperty
            = AvaloniaProperty.Register<MenuButton, double>(nameof(MenuVerticalOffset), -1.0);


        #endregion

        #endregion

        #region Menu Arrow

        /// <summary>
        /// Get or set if an arrow should be shown to the right of the button content to indicate the button as a menu button.
        /// </summary>
        public bool ShowMenuArrow { get => GetValue(ShowMenuArrowProperty); set => SetValue(ShowMenuArrowProperty, value); }

        /// <summary>The backing styled property for <see cref="ShowMenuArrow"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> ShowMenuArrowProperty
            = AvaloniaProperty.Register<MenuButton, bool>(nameof(ShowMenuArrow), true);

        /// <summary>
        /// Get or set if the arrow should be kept to the right side of the button, even if the content of the button is left or center aligned 
        /// (via <see cref="Avalonia.Controls.ContentControl.HorizontalContentAlignment"/>).
        /// </summary>
        public bool KeepMenuArrowOnRight { get => GetValue(KeepMenuArrowOnRightProperty); set => SetValue(KeepMenuArrowOnRightProperty, value); }

        /// <summary>The backing styled property for <see cref="KeepMenuArrowOnRight"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> KeepMenuArrowOnRightProperty
            = AvaloniaProperty.Register<MenuButton, bool>(nameof(KeepMenuArrowOnRight), false);

        #endregion

    }
}
