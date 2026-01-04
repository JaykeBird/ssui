using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            SsuiThemeApplied += OnSsuiThemeApplied;
            Click += MenuButton_Click;
        }

        private void OnSsuiThemeApplied(object sender, RoutedEventArgs e)
        {
            Menu?.SetBinding(ThemedControl.SsuiThemeProperty, new System.Windows.Data.Binding(nameof(SsuiTheme)) { Source = this });
        }

        private void OnColorSchemeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ColorScheme cs)
            {
                Menu?.ApplyColorScheme(cs);
            }
        }

        #region Menu
        
        /// <summary>The backing dependency property for <see cref="Menu"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuProperty
            = DependencyProperty.Register(nameof(Menu), typeof(ContextMenu), typeof(MenuButton),
            new FrameworkPropertyMetadata(null, (d, e) => d.PerformAs<MenuButton>((o) => o.OnMenuChanged(e))));

        /// <summary>
        /// Raised when the <see cref="Menu"/> property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? MenuChanged;
#else
        public event DependencyPropertyChangedEventHandler MenuChanged;
#endif

        private void OnMenuChanged(DependencyPropertyChangedEventArgs e)
        {
            MenuChanged?.Invoke(this, e);
        }


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
        /// This event is raised when this MenuButton's menu is about to open.
        /// </summary>
        public event CancelEventHandler? MenuOpening;

        /// <summary>
        /// This event is raised when this MenuButton's menu has been opened.
        /// </summary>
        public event EventHandler? MenuOpened;

        /// <summary>
        /// This event is raised when this MenuButton's menu has been closed.
        /// </summary>
        public event EventHandler? MenuClosed;

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
        /// This event is raised when this MenuButton's menu is about to open.
        /// </summary>
        public event CancelEventHandler MenuOpening;

        /// <summary>
        /// This event is raised when this MenuButton's menu has been opened.
        /// </summary>
        public event EventHandler MenuOpened;

        /// <summary>
        /// This event is raised when this MenuButton's menu has been closed.
        /// </summary>
        public event EventHandler MenuClosed;
#endif

        /// <summary>
        /// Get or set if the menu should close automatically. Remember to set the <c>StaysOpenOnClick</c> property for child menu items as well.
        /// </summary>
        /// <remarks>
        /// When this is set to <c>false</c>, the menu will close when a menu item is selected, or when the user clicks outside of the menu or moves focus.
        /// Due to the differences between how Avalonia and WPF handle their context menus, this functions a bit differently in the two versions when set to <c>true</c>.
        /// <para/>
        /// In the WPF version of Solid Shine UI, <c>StaysOpen</c> will keep the context menu open until a menu item is clicked (unless the menu item also has <c>StaysOpenOnClick</c>
        /// set to true), but other methods to close the menu don't generally work (such as clicking outside of the menu). So the best ways to close the menu is to have a "Close" menu
        /// item that doesn't have <c>StaysOpenOnClick</c> applied, or some other code or function that directly sets the menu's <c>IsOpen</c> property to <c>false</c>. You do not
        /// need to explicitly change <c>StaysOpen</c> to <c>false</c> in order to allow the menu to close via explicity setting the property, so this value can remain unchanged as 
        /// long as you want this behavior.
        /// </remarks>
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

        // /// <para/>
        // /// In the Avalonia version of Solid Shine UI, <c>StaysOpen</c> will set the context menu to refuse to close; when the context menu is about to close, the MenuButton cancels 
        // /// that action. This also means that clicking any menu items will not close the menu, nor will setting the menu's <c>IsOpen</c> property to <c>false</c>.
        // /// Instead, to close the context menu, you will need to change <c>StaysOpen</c> back to <c>false</c> before you change <c>IsOpen</c> to <c>false</c> or click out of the menu,
        // /// and then you will need to re-set <c>StaysOpen</c> back to <c>true</c> whenever you want this behavior to occur again.

        private void Menu_Closed(object sender, RoutedEventArgs e)
        {
            MenuClosed?.Invoke(this, EventArgs.Empty);
        }

        #region Placement

        /// <summary>
        /// Get or set the placement mode for the MenuButton's menu.
        /// </summary>
        public PlacementMode MenuPlacement { get => (PlacementMode)GetValue(MenuPlacementProperty); set => SetValue(MenuPlacementProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuPlacement"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuPlacementProperty
            = DependencyProperty.Register(nameof(MenuPlacement), typeof(PlacementMode), typeof(MenuButton),
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
        public static readonly DependencyProperty MenuPlacementTargetProperty
            = DependencyProperty.Register(nameof(MenuPlacementTarget), typeof(UIElement), typeof(MenuButton),
            new FrameworkPropertyMetadata(null));


        /// <summary>
        /// Get or set the placement rectangle for the MenuButton's menu. This sets the area relative to the button that the menu is positioned.
        /// </summary>
        public Rect MenuPlacementRectangle { get => (Rect)GetValue(MenuPlacementRectangleProperty); set => SetValue(MenuPlacementRectangleProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuPlacementRectangle"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuPlacementRectangleProperty
            = DependencyProperty.Register(nameof(MenuPlacementRectangle), typeof(Rect), typeof(MenuButton),
            new FrameworkPropertyMetadata(Rect.Empty));

        /// <summary>
        /// Get or set how far offset the menu is horizontally (left or right) from its placement target/rectangle when it's opened.
        /// </summary>
        public double MenuHorizontalOffset { get => (double)GetValue(MenuHorizontalOffsetProperty); set => SetValue(MenuHorizontalOffsetProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuHorizontalOffset"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuHorizontalOffsetProperty
            = DependencyProperty.Register(nameof(MenuHorizontalOffset), typeof(double), typeof(MenuButton),
            new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// Get or set how far offset the menu is vertically (up or down) from its placement target/rectangle when it's opened.
        /// </summary>
        public double MenuVerticalOffset { get => (double)GetValue(MenuVerticalOffsetProperty); set => SetValue(MenuVerticalOffsetProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuVerticalOffset"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuVerticalOffsetProperty
            = DependencyProperty.Register(nameof(MenuVerticalOffset), typeof(double), typeof(MenuButton),
            new FrameworkPropertyMetadata(-1.0));


        #endregion

        #endregion

        #region Arrow

        /// <summary>
        /// The backing dependency property for <see cref="ShowMenuArrow"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ShowMenuArrowProperty = DependencyProperty.Register(
            nameof(ShowMenuArrow), typeof(bool), typeof(MenuButton), new PropertyMetadata(true));

        /// <summary>
        /// Get or set if an arrow should be shown to the right of the button content to indicate the button as a menu button.
        /// </summary>
        [Category("Common")]
        public bool ShowMenuArrow
        {
            get => (bool)GetValue(ShowMenuArrowProperty);
            set => SetValue(ShowMenuArrowProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="KeepMenuArrowOnRight"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty KeepMenuArrowOnRightProperty = DependencyProperty.Register(
            nameof(KeepMenuArrowOnRight), typeof(bool), typeof(MenuButton), new PropertyMetadata(false));

        /// <summary>
        /// Get or set if the arrow should be kept to the right side of the button, even if the content of the button is left or center aligned 
        /// (via <see cref="Control.HorizontalContentAlignment"/>).
        /// </summary>
        [Category("Common")]
        public bool KeepMenuArrowOnRight
        {
            get => (bool)GetValue(KeepMenuArrowOnRightProperty);
            set => SetValue(KeepMenuArrowOnRightProperty, value);
        }

        #endregion

        /// <summary>
        /// Internal method for opening up the menu when the button is clicked
        /// </summary>
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            OpenMenu();
        }

        /// <summary>
        /// Display this menu button's menu programmatically. This will open the menu at the set placement target and location.
        /// </summary>
        public void OpenMenu()
        {
            if (Menu != null)
            {
                // first, raise MenuOpening event
                CancelEventArgs ce = new CancelEventArgs(false);
                MenuOpening?.Invoke(this, ce);
                if (ce.Cancel) return;

                // then, set up the full menu and show it
                Menu.Placement = MenuPlacement;
                Menu.PlacementTarget = MenuPlacementTarget ?? this;
                Menu.PlacementRectangle = MenuPlacementRectangle;
                Menu.HorizontalOffset = MenuHorizontalOffset;
                Menu.VerticalOffset = MenuVerticalOffset;
                Menu.IsOpen = true;
                Menu.Closed += Menu_Closed;

                // finally, we can raise the MenuOpened event
                MenuOpened?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
