using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace SolidShineUi.Ribbon
{
    /// <summary>
    /// A holder for a group of <see cref="IRibbonItem"/> objects within a <see cref="RibbonTab"/>. 
    /// </summary>
    /// <remarks>
    /// Most tabs are comprised of multiple groups, and all of the items in each group
    /// usually have some correlation or related context to each other (such as copy, cut, and paste being in the same group because they're all related to interacting with the clipboard).
    /// </remarks>
    [ContentProperty("Items")]
    public class RibbonGroup : Control
    {
        static RibbonGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonGroup), new FrameworkPropertyMetadata(typeof(RibbonGroup)));
        }

        /// <summary>
        /// Create a RibbonGroup.
        /// </summary>
        public RibbonGroup()
        {
            SetValue(ItemsPropertyKey, new ObservableCollection<IRibbonItem>());

            CommandBindings.Add(new System.Windows.Input.CommandBinding(RibbonCommands.DialogLauncherAction, (s, e) => LauncherClick?.Invoke(this, e),
                (s, e) => e.CanExecute = ShowLauncher && IsLauncherEnabled));
        }

        /// <summary>
        /// Get or set the title to display at the bottom of the group. Use a title name that succinctly describes the commands that are contained in this group.
        /// </summary>
        [Category("Common")]
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        /// <summary>
        /// The backing dependency property object. See the related property for details.
        /// </summary>
        public static DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(string), typeof(RibbonGroup),
            new FrameworkPropertyMetadata("Group"));

        /// <summary>
        /// Get or set if the header (which contains the group's title and launcher icon) is displayed at the bottom of the group or not.
        /// </summary>
        [Category("Common")]
        public bool ShowGroupHeader { get => (bool)GetValue(ShowGroupHeaderProperty); set => SetValue(ShowGroupHeaderProperty, value); }

        /// <summary>
        /// The backing dependency property object. See the related property for details.
        /// </summary>
        public static DependencyProperty ShowGroupHeaderProperty
            = DependencyProperty.Register("ShowGroupHeader", typeof(bool), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(true));


        private static readonly DependencyPropertyKey ItemsPropertyKey
            = DependencyProperty.RegisterReadOnly("Items", typeof(ObservableCollection<IRibbonItem>), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(new ObservableCollection<IRibbonItem>()));

        /// <summary>
        /// The backing dependency property object. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Get or set the list of items in this RibbonGroup. This Items property can be used to add and remove items.
        /// </summary>
        [Category("Common")]
        public ObservableCollection<IRibbonItem> Items
        {
            get { return (ObservableCollection<IRibbonItem>)GetValue(ItemsProperty); }
            private set { SetValue(ItemsPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey ExpandedItemsPropertyKey
            = DependencyProperty.RegisterReadOnly("ExpandedItems", typeof(ObservableCollection<IRibbonItem>), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(new ObservableCollection<IRibbonItem>()));

        /// <summary>
        /// The backing dependency property object. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ExpandedItemsProperty = ExpandedItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Get or set the list of items in this RibbonGroup's expanded panel. This Items property can be used to add and remove items. If there are no items, the expanded panel will not be shown.
        /// </summary>
        [Category("Common")]
        public ObservableCollection<IRibbonItem> ExpandedItems
        {
            get { return (ObservableCollection<IRibbonItem>)GetValue(ExpandedItemsProperty); }
            private set { SetValue(ExpandedItemsPropertyKey, value); }
        }

        #region Right Separator

        /// <summary>
        /// Get or set if a vertical separator bar should be displayed on the far-right side of this control.
        /// </summary>
        public bool ShowGroupSeparator { get => (bool)GetValue(ShowGroupSeparatorProperty); set => SetValue(ShowGroupSeparatorProperty, value); }

        /// <summary>
        /// The backing dependency property object. See the related property for details.
        /// </summary>
        public static DependencyProperty ShowGroupSeparatorProperty
            = DependencyProperty.Register("ShowGroupSeparator", typeof(bool), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Get or set the brush for the vertical separator bar on the far-right side of this control.
        /// </summary>
        public Brush GroupSeparatorBrush { get => (Brush)GetValue(GroupSeparatorBrushProperty); set => SetValue(GroupSeparatorBrushProperty, value); }

        /// <summary>
        /// The backing dependency property object. See the related property for details.
        /// </summary>
        public static DependencyProperty GroupSeparatorBrushProperty
            = DependencyProperty.Register("GroupSeparatorBrush", typeof(Brush), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set how wide the vertical separator bar on the far-right side of this control.
        /// </summary>
        public double GroupSeparatorWidth { get => (double)GetValue(GroupSeparatorWidthProperty); set => SetValue(GroupSeparatorWidthProperty, value); }

        /// <summary>
        /// The backing dependency property object. See the related property for details.
        /// </summary>
        public static DependencyProperty GroupSeparatorWidthProperty
            = DependencyProperty.Register("GroupSeparatorWidth", typeof(double), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(1.0));


        #endregion

        #region Color Scheme
        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif


        /// <summary>
        /// A dependency property object backing the related ColorScheme property. See <see cref="ColorScheme"/> for more details.
        /// </summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            ColorScheme cs = (e.NewValue as ColorScheme)!;
#else
            ColorScheme cs = e.NewValue as ColorScheme;
#endif

            if (d is RibbonGroup c)
            {
                c.ColorSchemeChanged?.Invoke(d, e);
                c.ApplyColorScheme(cs);
            }
        }

        /// <summary>
        /// Get or set the color scheme used for this control. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        /// <summary>
        /// Apply a color scheme to this control. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply.</param>
        public void ApplyColorScheme(ColorScheme cs)
        {
            if (cs == null)
            {
                return;
            }
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }

            Foreground = cs.ForegroundColor.ToBrush();
            GroupSeparatorBrush = cs.BorderColor.ToBrush();
        }
        #endregion

        #region Resizing

        public bool IsCollapsed { get => (bool)GetValue(IsCollapsedProperty); set => SetValue(IsCollapsedProperty, value); }

        public static DependencyProperty IsCollapsedProperty
            = DependencyProperty.Register("IsCollapsed", typeof(bool), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(false));

        public int CompactOrder { get => (int)GetValue(CompactOrderProperty); set => SetValue(CompactOrderProperty, value); }

        public static DependencyProperty CompactOrderProperty
            = DependencyProperty.Register("CompactOrder", typeof(int), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(0));

        public GroupSizeMode SizeMode { get => (GroupSizeMode)GetValue(SizeModeProperty); set => SetValue(SizeModeProperty, value); }

        public static DependencyProperty SizeModeProperty
            = DependencyProperty.Register("SizeMode", typeof(GroupSizeMode), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(GroupSizeMode.Standard));

        #endregion

        #region Expanded Items

        public bool HasExpanderOpen { get => (bool)GetValue(HasExpanderOpenProperty); set => SetValue(HasExpanderOpenProperty, value); }

        public static DependencyProperty HasExpanderOpenProperty
            = DependencyProperty.Register("HasExpanderOpen", typeof(bool), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(false));

        public bool IsExpanderPinned { get => (bool)GetValue(IsExpanderPinnedProperty); set => SetValue(IsExpanderPinnedProperty, value); }

        public static DependencyProperty IsExpanderPinnedProperty
            = DependencyProperty.Register("IsExpanderPinned", typeof(bool), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(false));


        #endregion

        #region Launcher

        public bool ShowLauncher { get => (bool)GetValue(ShowLauncherProperty); set => SetValue(ShowLauncherProperty, value); }

        public static DependencyProperty ShowLauncherProperty
            = DependencyProperty.Register("ShowLauncher", typeof(bool), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(false));

        public ToolTip LauncherTooltip { get => (ToolTip)GetValue(LauncherTooltipProperty); set => SetValue(LauncherTooltipProperty, value); }

        public static DependencyProperty LauncherTooltipProperty
            = DependencyProperty.Register("LauncherTooltip", typeof(ToolTip), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(null));

        public bool IsLauncherEnabled { get => (bool)GetValue(IsLauncherEnabledProperty); set => SetValue(IsLauncherEnabledProperty, value); }

        public static DependencyProperty IsLauncherEnabledProperty
            = DependencyProperty.Register("IsLauncherEnabled", typeof(bool), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Raised when the launcher in the bottom-right corner of the group is clicked. 
        /// The launcher is only shown and clickable if <see cref="ShowLauncher"/> is set to <c>true</c>.
        /// </summary>
#if NETCOREAPP
        public event EventHandler? LauncherClick;
#else
        public event EventHandler LauncherClick;
#endif

        #endregion

        #region Undocking

        public bool CanBeUndocked { get => (bool)GetValue(CanBeUndockedProperty); set => SetValue(CanBeUndockedProperty, value); }

        public static DependencyProperty CanBeUndockedProperty
            = DependencyProperty.Register("CanBeUndocked", typeof(bool), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(true));

        public bool IsUndocked { get => (bool)GetValue(IsUndockedProperty); set => SetValue(IsUndockedProperty, value); }

        public static DependencyProperty IsUndockedProperty
            = DependencyProperty.Register("IsUndocked", typeof(bool), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(false));

        public Point UndockedPosition { get => (Point)GetValue(UndockedPositionProperty); set => SetValue(UndockedPositionProperty, value); }

        public static DependencyProperty UndockedPositionProperty
            = DependencyProperty.Register("UndockedPosition", typeof(Point), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(null));



        #endregion

    }
}
