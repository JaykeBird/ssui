using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using ContextMenu = SolidShineUi.ContextMenu;

namespace SolidShineUi
{

    /// <summary>
    /// A popup context menu, with a large section also available for displaying custom content.
    /// </summary>
    public class ContentContextMenu : ContextMenu
    {
        /// <summary>
        /// Create a new ContentContextMenu.
        /// </summary>
        public ContentContextMenu() { }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            switch (change.Property.Name)
            {
                case nameof(Content):
                    ContentChanged(change);
                    break;
            }
        }

        #region ContentControl Properties

        // https://docs.avaloniaui.net/api/avalonia/controls/contentcontrol#content-property

        /// <summary>
        /// Gets or sets the content to display.
        /// </summary>
        public object? Content { get => GetValue(ContentProperty); set => SetValue(ContentProperty, value); }

        /// <summary>The backing styled property for <see cref="Content"/>. See the related property for details.</summary>
        public static readonly StyledProperty<object?> ContentProperty
            = ContentControl.ContentProperty.AddOwner<ContentContextMenu>();

        /// <summary>
        /// Gets or sets the data template used to display the content of the control.
        /// </summary>
        public IDataTemplate? ContentTemplate { get => GetValue(ContentTemplateProperty); set => SetValue(ContentTemplateProperty, value); }

        /// <summary>The backing styled property for <see cref="ContentTemplate"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty
            = ContentControl.ContentTemplateProperty.AddOwner<ContentContextMenu>();

        /// <summary>
        /// Gets or sets the horizontal alignment of the content within the control.
        /// </summary>
        public HorizontalAlignment HorizontalContentAlignment { get => GetValue(HorizontalContentAlignmentProperty); set => SetValue(HorizontalContentAlignmentProperty, value); }

        /// <summary>The backing styled property for <see cref="HorizontalContentAlignment"/>. See the related property for details.</summary>
        public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty
            = ContentControl.HorizontalContentAlignmentProperty.AddOwner<ContentContextMenu>();

        /// <summary>
        /// Gets or sets the vertical alignment of the content within the control.
        /// </summary>
        public VerticalAlignment VerticalContentAlignment { get => GetValue(VerticalContentAlignmentProperty); set => SetValue(VerticalContentAlignmentProperty, value); }

        /// <summary>The backing styled property for <see cref="VerticalContentAlignment"/>. See the related property for details.</summary>
        public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty
            = ContentControl.VerticalContentAlignmentProperty.AddOwner<ContentContextMenu>();

        private void ContentChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.OldValue is ILogical oldChild)
            {
                LogicalChildren.Remove(oldChild);
            }

            if (e.NewValue is ILogical newChild)
            {
                LogicalChildren.Add(newChild);
            }

            HasContent = e.NewValue != null;
        }

        private bool _hasContent = false;

        /// <summary>
        /// Get or set if this control currently has a non-null <c>Content</c> value.
        /// </summary>
        public bool HasContent { get => _hasContent; private set => SetAndRaise(HasContentProperty, ref _hasContent, value); }

        /// <summary>The backing direct property for <see cref="HasContent"/>. See the related property for details.</summary>
        public static readonly DirectProperty<ContentContextMenu, bool> HasContentProperty
            = AvaloniaProperty.RegisterDirect<ContentContextMenu, bool>(nameof(HasContent), (s) => s.HasContent, unsetValue: false);

        #endregion

        #region Menu Placement/Appearance

        /// <summary>
        /// Get or set the side of the control to place the menu items in <c>Items</c>.
        /// </summary>
        public PlacementDirection MenuPlacement { get => GetValue(MenuPlacementProperty); set => SetValue(MenuPlacementProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuPlacement"/>. See the related property for details.</summary>
        public static readonly StyledProperty<PlacementDirection> MenuPlacementProperty
            = AvaloniaProperty.Register<ContentContextMenu, PlacementDirection>(nameof(MenuPlacement), PlacementDirection.Bottom);

        /// <summary>
        /// Get or set the brush to use for the separator between the menu and content areas of the control.
        /// </summary>
        /// <remarks>
        /// This does not affect the separators between menu items in the menu itself; you will want to use a custom style for those separators.
        /// </remarks>
        public IBrush? SeparatorBrush { get => GetValue(SeparatorBrushProperty); set => SetValue(SeparatorBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="SeparatorBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> SeparatorBrushProperty
            = AvaloniaProperty.Register<ContentContextMenu, IBrush?>(nameof(SeparatorBrush), null);
        
        // TODO: add in MenuItemPadding property (to this and all the menu controls)

        #endregion

    }
}
