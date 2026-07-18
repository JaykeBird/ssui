using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Metadata;

namespace SolidShineUi
{
    /// <summary>
    /// A tab that can be added onto a <see cref="TabControl"/>. Each tab has a title, icon, and close button by default.
    /// </summary>
    public class TabItem : AvaloniaObject
    {
        /// <summary>
        /// Create a TabItem.
        /// </summary>
        public TabItem()
        {

        }

        /// <summary>
        /// Create a TabItem, with the title preset.
        /// </summary>
        /// <param name="title">The title of this tab.</param>
        public TabItem(string title) : this()
        {
            Title = title;
        }


        /// <summary>
        /// Create a TabItem, by copying the properties of an existing TabItem.
        /// </summary>
        /// <param name="other">The other TabItem to copy from.</param>
        /// <remarks>
        /// The values of the <see cref="Content"/> and <see cref="ExtraTabElement"/> properties are not copied over, as UI elements can only have 1 parent.
        /// </remarks>
        public TabItem(TabItem other) : this()
        {
            // I don't copy the Content or ExtraTabElement properties, since child UI elements can only have 1 parent
            // instead, the user should clone these properties themselves or come up with their own solution

            Title = other.Title;
            IsDirty = other.IsDirty;
            DisplayDirtyState = other.DisplayDirtyState;
            CanClose = other.CanClose;
            ShowIcon = other.ShowIcon;
            CanSelect = other.CanSelect;
            Icon = other.Icon;
            TabBackground = other.TabBackground;
            Padding = other.Padding;
            IsVisible = other.IsVisible;
            TabContextMenu = other.TabContextMenu;
            AllowDrop = other.AllowDrop;
            ToolTip = other.ToolTip;
            FontSize = other.FontSize;
            FontStyle = other.FontStyle;
            FontFamily = other.FontFamily;
            FontWeight = other.FontWeight;
            FontStretch = other.FontStretch;
        }

        ///<summary>
        /// Get or set the title of this tab.
        ///</summary>
        public string Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        /// <summary>The backing styled property for <see cref="Title"/>. See the related property for details.</summary>
        public static readonly StyledProperty<string> TitleProperty
            = AvaloniaProperty.Register<TabItem, string>(nameof(Title), "New Tab");

        ///<summary>
        /// Get or set if the tab is dirty.
        ///</summary>
        public bool IsDirty { get => GetValue(IsDirtyProperty); set => SetValue(IsDirtyProperty, value); }

        /// <summary>The backing styled property for <see cref="IsDirty"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> IsDirtyProperty
            = AvaloniaProperty.Register<TabItem, bool>(nameof(IsDirty), false);

        ///<summary>
        /// Get or set if the tab should display the state of its IsDirty property. When set to true, an asterisk (*) will be added to the title when it is dirty.
        ///</summary>
        public bool DisplayDirtyState { get => GetValue(DisplayDirtyStateProperty); set => SetValue(DisplayDirtyStateProperty, value); }

        /// <summary>The backing styled property for <see cref="DisplayDirtyState"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> DisplayDirtyStateProperty
            = AvaloniaProperty.Register<TabItem, bool>(nameof(DisplayDirtyState), true);


        ///<summary>
        /// Get or set if this tab can be closed via the UI (i.e. the close button).
        ///</summary>
        ///<remarks>
        /// This property does not prevent this tab being closed via other means, such as <see cref="Close()"/> or <see cref="SelectableCollection{TabItem}.Remove(TabItem)"/>.
        /// </remarks>
        public bool CanClose { get => GetValue(CanCloseProperty); set => SetValue(CanCloseProperty, value); }

        /// <summary>The backing styled property for <see cref="CanClose"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> CanCloseProperty
            = AvaloniaProperty.Register<TabItem, bool>(nameof(CanClose), true);

        ///<summary>
        /// Get or set if an icon is shown for this tab. The icon section of the tab will be visible if this is true, even if there is no icon set.
        ///</summary>
        public bool ShowIcon { get => GetValue(ShowIconProperty); set => SetValue(ShowIconProperty, value); }

        /// <summary>The backing styled property for <see cref="ShowIcon"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> ShowIconProperty
            = AvaloniaProperty.Register<TabItem, bool>(nameof(ShowIcon), true);

        ///<summary>
        /// Get or set whether this tab can be selected via UI.
        ///</summary>
        public bool CanSelect { get => GetValue(CanSelectProperty); set => SetValue(CanSelectProperty, value); }

        /// <summary>The backing styled property for <see cref="CanSelect"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> CanSelectProperty
            = AvaloniaProperty.Register<TabItem, bool>(nameof(CanSelect), true);

        ///<summary>
        /// Get or set the icon to display with this tab.
        ///</summary>
        public IImage? Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }

        /// <summary>The backing styled property for <see cref="Icon"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IImage?> IconProperty
            = AvaloniaProperty.Register<TabItem, IImage?>(nameof(Icon), null);

        ///<summary>
        /// Get or set the brush displayed in the background of the tab. This will override the color of the TabControl itself, but transparency does allow the standard color to show through.
        ///</summary>
        public IBrush TabBackground { get => GetValue(TabBackgroundProperty); set => SetValue(TabBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="TabBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> TabBackgroundProperty
            = AvaloniaProperty.Register<TabItem, IBrush>(nameof(TabBackground), new SolidColorBrush(Colors.Transparent));

        ///<summary>
        /// Get or set the content to display when this tab is selected.
        ///</summary>
        [Content]
        public Control? Content { get => GetValue(ContentProperty); set => SetValue(ContentProperty, value); }

        /// <summary>The backing styled property for <see cref="Content"/>. See the related property for details.</summary>
        public static readonly StyledProperty<Control?> ContentProperty
            = AvaloniaProperty.Register<TabItem, Control?>(nameof(Content), null);

        ///<summary>
        /// Get or set the padding (or space) applied around the tab's title and icon. (This does not set the padding for the content.)
        ///</summary>
        public Thickness Padding { get => GetValue(PaddingProperty); set => SetValue(PaddingProperty, value); }

        /// <summary>The backing styled property for <see cref="Padding"/>. See the related property for details.</summary>
        public static readonly StyledProperty<Thickness> PaddingProperty
            = AvaloniaProperty.Register<TabItem, Thickness>(nameof(Padding), new Thickness(left: 4, top: 0, right: 4, bottom: 0));

        ///<summary>
        /// Get or set if this tab is visually displayed in the UI.
        ///</summary>
        public bool IsVisible { get => GetValue(IsVisibleProperty); set => SetValue(IsVisibleProperty, value); }

        /// <summary>The backing styled property for <see cref="IsVisible"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> IsVisibleProperty
            = AvaloniaProperty.Register<TabItem, bool>(nameof(IsVisible), true);

        /// <summary>
        /// Get or set the menu that appears when you right-click on the tab in the TabControl.
        /// </summary>
        public ContextMenu? TabContextMenu { get => GetValue(TabContextMenuProperty); set => SetValue(TabContextMenuProperty, value); }

        /// <summary>The backing styled property for <see cref="TabContextMenu"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ContextMenu?> TabContextMenuProperty
            = AvaloniaProperty.Register<TabItem, ContextMenu?>(nameof(TabContextMenu), null);

        ///<summary>
        /// Get or set if this tab can be used as a target in drag-and-drop operations.
        ///</summary>
        ///<remarks>
        /// Note that this only sets if this tab can recieve data dropped onto it in the tab bar. The elements inside the tab can set their own AllowDrop property if you want to drop items inside the tab's contents.
        /// This property does not need to be set to allow tab drag-and-drop on the TabControl (for rearranging tabs). Tab drag-and-drop is controlled via the <see cref="TabControl.AllowTabDragDrop"/> property.
        /// </remarks>
        public bool AllowDrop { get => GetValue(AllowDropProperty); set => SetValue(AllowDropProperty, value); }

        /// <summary>The backing styled property for <see cref="AllowDrop"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> AllowDropProperty
            = AvaloniaProperty.Register<TabItem, bool>(nameof(AllowDrop), false);

        ///<summary>
        /// Get or set the ToolTip to display when the mouse is hovering over this tab.
        ///</summary>
        public object? ToolTip { get => GetValue(ToolTipProperty); set => SetValue(ToolTipProperty, value); }

        /// <summary>The backing styled property for <see cref="ToolTip"/>. See the related property for details.</summary>
        public static readonly StyledProperty<object?> ToolTipProperty
            = AvaloniaProperty.Register<TabItem, object?>(nameof(ToolTip), null);

        /// <summary>
        /// Get or set the element to display on the right side of the tab, next to the Close button.
        /// </summary>
        public Control? ExtraTabElement { get => GetValue(ExtraTabElementProperty); set => SetValue(ExtraTabElementProperty, value); }

        /// <summary>The backing styled property for <see cref="ExtraTabElement"/>. See the related property for details.</summary>
        public static readonly StyledProperty<Control?> ExtraTabElementProperty
            = AvaloniaProperty.Register<TabItem, Control?>(nameof(ExtraTabElement), null);

        #region Font Properties

        /// <summary>
        /// Get or set the font size to use for the <see cref="Title"/> and other text in the TabItem.
        /// </summary>
        public double FontSize { get => GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }

        /// <summary>
        /// Get or set the font family to use for the <see cref="Title"/> and other text in the TabItem.
        /// </summary>
        public FontFamily FontFamily { get => GetValue(FontFamilyProperty); set => SetValue(FontFamilyProperty, value); }

        /// <summary>
        /// Get or set the font style to use for the <see cref="Title"/> and other text in the TabItem.
        /// </summary>
        public FontStyle FontStyle { get => GetValue(FontStyleProperty); set => SetValue(FontStyleProperty, value); }

        /// <summary>
        /// Get or set the font stretch to use for the <see cref="Title"/> and other text in the TabItem.
        /// </summary>
        public FontStretch FontStretch { get => GetValue(FontStretchProperty); set => SetValue(FontStretchProperty, value); }

        /// <summary>
        /// Get or set the font weight to use for the <see cref="Title"/> and other text in the TabItem.
        /// </summary>
        public FontWeight FontWeight { get => GetValue(FontWeightProperty); set => SetValue(FontWeightProperty, value); }


        /// <summary>The backing dependency property for <see cref="FontSize"/>. See the related property for details.</summary>
        public static readonly StyledProperty<double> FontSizeProperty = TextBlock.FontSizeProperty.AddOwner<TabItem>();

        /// <summary>The backing dependency property for <see cref="FontFamily"/>. See the related property for details.</summary>
        public static readonly StyledProperty<FontFamily> FontFamilyProperty = TextBlock.FontFamilyProperty.AddOwner<TabItem>();

        /// <summary>The backing dependency property for <see cref="FontStyle"/>. See the related property for details.</summary>
        public static readonly StyledProperty<FontStyle> FontStyleProperty = TextBlock.FontStyleProperty.AddOwner<TabItem>();

        /// <summary>The backing dependency property for <see cref="FontStretch"/>. See the related property for details.</summary>
        public static readonly StyledProperty<FontStretch> FontStretchProperty = TextBlock.FontStretchProperty.AddOwner<TabItem>()
            ;
        /// <summary>The backing dependency property for <see cref="FontWeight"/>. See the related property for details.</summary>
        public static readonly StyledProperty<FontWeight> FontWeightProperty = TextBlock.FontWeightProperty.AddOwner<TabItem>();

        #endregion

        /// <summary>
        /// Close this tab (and remove it from the TabControl), if it is currently in a TabControl.
        /// </summary>
        /// <remarks>
        /// This function can be run regardless of the state of the <c>CanClose</c> property.
        /// </remarks>
        public void Close()
        {
            RequestTabClosing?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Attempt to bring the current tab into view on the tab bar
        /// </summary>
        public void BringIntoView()
        {
            BringIntoViewRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Used internally to request closing of this tab.
        /// </summary>
        internal protected event EventHandler? RequestTabClosing;

        /// <summary>
        /// Used internally to request bringing this tab into view.
        /// </summary>
        internal protected event EventHandler? BringIntoViewRequested;
    }
}
