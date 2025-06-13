using SolidShineUi.Utils;
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
    /// Most Ribbon tabs are comprised of multiple groups, and all of the items in each group
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
            SetValue(ExpandedItemsPropertyKey, new ObservableCollection<IRibbonItem>());

            CommandBindings.Add(new System.Windows.Input.CommandBinding(RibbonCommands.DialogLauncherAction, (s, e) => LauncherClick?.Invoke(this, e),
                (s, e) => e.CanExecute = ShowLauncher && IsLauncherEnabled));
        }

        #region Items

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

        #region Expanded Items

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
        /// <remarks>
        /// The expanded panel can display additional items that are related to the other items and name of this group, but don't need to be accessed regularly. This takes inspiration from
        /// a similar feature present in the Ribbon present in some of Autodesk's software.
        /// </remarks>
        [Category("Common")]
        public ObservableCollection<IRibbonItem> ExpandedItems
        {
            get { return (ObservableCollection<IRibbonItem>)GetValue(ExpandedItemsProperty); }
            private set { SetValue(ExpandedItemsPropertyKey, value); }
        }

        /// <summary>
        /// Get or set if the expanded panel is open for this RibbonGroup. Use <see cref="ExpandedItems"/> to add items to the expanded panel; 
        /// this property does nothing if there are no items. You can keep the expanded panel open using <see cref="IsExpanderPinned"/>.
        /// </summary>
        /// <remarks>
        /// The expanded panel can display additional items that are related to the other items and name of this group, but don't need to be accessed regularly. This takes inspiration from
        /// a similar feature present in the Ribbon present in some of Autodesk's software.
        /// </remarks>
        public bool HasExpanderOpen { get => (bool)GetValue(HasExpanderOpenProperty); set => SetValue(HasExpanderOpenProperty, value); }

        /// <summary>The backing dependency property for <see cref="HasExpanderOpen"/>. See the related property for details.</summary>
        public static DependencyProperty HasExpanderOpenProperty
            = DependencyProperty.Register("HasExpanderOpen", typeof(bool), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get or set if the expanded panel is open and currently pinned for this RibbonGroup. Use <see cref="ExpandedItems"/> to add items to the expanded panel; 
        /// this property does nothing if there are no items. While this is <c>true</c>, the expanded panel will remain open.
        /// </summary>
        /// <remarks>
        /// The expanded panel can display additional items that are related to the other items and name of this group, but don't need to be accessed regularly. This takes inspiration from
        /// a similar feature present in the Ribbon present in some of Autodesk's software.
        /// </remarks>
        public bool IsExpanderPinned { get => (bool)GetValue(IsExpanderPinnedProperty); set => SetValue(IsExpanderPinnedProperty, value); }

        /// <summary>The backing dependency property for <see cref="IsExpanderPinned"/>. See the related property for details.</summary>
        public static DependencyProperty IsExpanderPinnedProperty
            = DependencyProperty.Register("IsExpanderPinned", typeof(bool), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(false));

        #endregion

        #endregion

        #region Header/Title

        /// <summary>
        /// Get or set the title to display at the bottom of the group. Use a title name that succinctly describes the commands that are contained in this group.
        /// </summary>
        [Category("Common")]
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        /// <summary>
        /// The backing dependency property object for <see cref="Title"/> property. See the related property for details.
        /// </summary>
        public static DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(string), typeof(RibbonGroup),
            new FrameworkPropertyMetadata("Group"));

        /// <summary>
        /// Get or set if the header (which contains the group's title and the dialog launcher button) is displayed at the bottom of the group or not.
        /// </summary>
        [Category("Common")]
        public bool ShowGroupHeader { get => (bool)GetValue(ShowGroupHeaderProperty); set => SetValue(ShowGroupHeaderProperty, value); }

        /// <summary>
        /// The backing dependency property object for <see cref="ShowGroupHeader"/> property. See the related property for details.
        /// </summary>
        public static DependencyProperty ShowGroupHeaderProperty
            = DependencyProperty.Register("ShowGroupHeader", typeof(bool), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(true));

        #endregion

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

        /// <summary>
        /// Get if this current group is collapsed down to just an icon (as in, <see cref="CompactSize" /> is just <c>IconOnly</c>).
        /// </summary>
        /// <remarks>
        /// Use <see cref="CompactSize"/> to actually change the size of this group.
        /// </remarks>
        public bool IsCollapsed { get => (bool)GetValue(IsCollapsedProperty); private set => SetValue(IsCollapsedPropertyKey, value); }

        private static readonly DependencyPropertyKey IsCollapsedPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsCollapsed), typeof(bool), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(false));

        /// <summary>The backing dependency property for <see cref="IsCollapsed"/>. See the related property for details.</summary>
        public static readonly DependencyProperty IsCollapsedProperty = IsCollapsedPropertyKey.DependencyProperty;

        /// <summary>
        /// Get or set the sorting order value for this group, for when the Ribbon needs to do compacting.
        /// Groups with a higher compact order value will be compacted down first, when needed.
        /// </summary>
        /// <remarks>
        /// Compacting of groups is done to attempt to make sure all items in all groups on a tab can be displayed on screen without scrolling. The parent Ribbon control will attempt to do
        /// compacting when it's not wide enough to accomodate all groups at its normal (<c>Standard</c>) size. Compacting will begin with the group with the highest <c>CompactOrder</c> value
        /// first, and move downward from there if more compacting is still needed. Compacting can be disabled for a particular Ribbon tab by setting <see cref="RibbonTab.FitContentsToWidth"/>
        /// to <c>false</c>.
        /// </remarks>
        public int CompactOrder { get => (int)GetValue(CompactOrderProperty); set => SetValue(CompactOrderProperty, value); }

        public static DependencyProperty CompactOrderProperty
            = DependencyProperty.Register("CompactOrder", typeof(int), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(0));

        /// <summary>
        /// Get or set the current compacted size for this group. By default, the group should be set to <c>Standard</c>, unless it was compacted by the main <see cref="Ribbon"/> control.
        /// </summary>
        /// <remarks>
        /// Compacting of groups is done to attempt to make sure all items in all groups on a tab can be displayed on screen without scrolling. The parent Ribbon control will attempt to do
        /// compacting when it's not wide enough to accomodate all groups at its <c>Standard</c> size. Compacting will begin with the group with the highest <see cref="CompactOrder"/> value
        /// first, and move downward from there if more compacting is still needed. Compacting can be disabled for a particular Ribbon tab by setting <see cref="RibbonTab.FitContentsToWidth"/>
        /// to <c>false</c>.
        /// <para/>
        /// When a RibbonGroup is compacted down to <c>IconOnly</c>, the actual items in the Ribbon aren't visible anymore, and instead the whole group is represented by just its
        /// <see cref="Title"/> and <see cref="GroupIcon"/>.
        /// </remarks>
        public GroupSizeMode CompactSize { get => (GroupSizeMode)GetValue(CompactSizeProperty); set => SetValue(CompactSizeProperty, value); }

        /// <summary>
        /// The backing dependency property for <see cref="CompactSize"/>. See the related property for details.
        /// </summary>
        public static DependencyProperty CompactSizeProperty
            = DependencyProperty.Register("CompactSize", typeof(GroupSizeMode), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(GroupSizeMode.Standard, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => d.PerformAs<RibbonGroup>((o) => o.UpdateCompactSize())));

        void UpdateCompactSize()
        {
            if (CompactSize == GroupSizeMode.IconOnly)
            {
                // everything is hidden away, it's just the group title and icon now
                IsCollapsed = true;
            }
            else
            {
                IsCollapsed = false;

                foreach (IRibbonItem item in Items)
                {
                    item.IsCompacted = CompactSize == GroupSizeMode.Compact;
                }
            }

        }

        /// <summary>
        /// The icon to associate with this RibbonGroup. This icon should be related to that of the items in this group; you could even just reuse the icon of the most prominent item
        /// in this group. This icon is used if the <see cref="CompactSize"/> value is changed to <c>IconOnly</c>, or in other locations to represent the group as a whole.
        /// </summary>
        public ImageSource GroupIcon { get => (ImageSource)GetValue(GroupIconProperty); set => SetValue(GroupIconProperty, value); }

        /// <summary>
        /// The backing dependency property for <see cref="GroupIcon"/>. See the related property for details.
        /// </summary>
        public static DependencyProperty GroupIconProperty
            = DependencyProperty.Register("GroupIcon", typeof(ImageSource), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(null));

        #endregion

        #region Launcher

        /// <summary>
        /// Get or set if the dialog launcher should be shown at the bottom-right corner of this Ribbon group.
        /// </summary>
        /// <remarks>
        /// The dialog launcher is a small button which usually is used to launch a dialog box or other UI that contains more options that are related to the commands in a Ribbon group, but
        /// were not placed onto the Ribbon group. For example, a Ribbon group with commands for changing font and text options (like font style, size, color, etc.) could have a dialog launcher
        /// that when clicked, opens a Font dialog that contains even more options for editing fonts.
        /// <para />
        /// The dialog launcher should be used for displaying another piece of UI, such as a dialog box or side pane - it shouldn't directly cause something to happen with your app's content.
        /// While the dialog launcher button was present from the start with Microsoft Office's Ribbon control, many other programs using the Ribbon do not have any dialog launcher buttons at 
        /// all.
        /// </remarks>
        public bool ShowLauncher { get => (bool)GetValue(ShowLauncherProperty); set => SetValue(ShowLauncherProperty, value); }

        /// <summary>
        /// The dependency property object for the <see cref="ShowLauncher"/> property. See the related property for details.
        /// </summary>
        public static DependencyProperty ShowLauncherProperty
            = DependencyProperty.Register("ShowLauncher", typeof(bool), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get or set the tooltip to display when the dialog launcher button has focus or the mouse over it.
        /// </summary>
        public ToolTip LauncherTooltip { get => (ToolTip)GetValue(LauncherTooltipProperty); set => SetValue(LauncherTooltipProperty, value); }

        /// <summary>
        /// The dependency property object for the <see cref="LauncherTooltip"/> property. See the related property for details.
        /// </summary>
        public static DependencyProperty LauncherTooltipProperty
            = DependencyProperty.Register("LauncherTooltip", typeof(ToolTip), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Get or set if the dialog launcher button should be enabled. Use <see cref="ShowLauncher"/> to hide the button entirely.
        /// </summary>
        /// <remarks>
        /// When in a situation where the dialog launcher button's action isn't doable or should be disabled, use this property to disable the button rather than using <see cref="ShowLauncher"/>
        /// to hide the button. The button should remain visible even when it's disabled, so that users can still have the knowledge that this button is present, even if not usable in this context.
        /// </remarks>
        public bool IsLauncherEnabled { get => (bool)GetValue(IsLauncherEnabledProperty); set => SetValue(IsLauncherEnabledProperty, value); }

        /// <summary>
        /// The dependency property object for the <see cref="IsLauncherEnabled"/> property. See the related property for details.
        /// </summary>
        public static DependencyProperty IsLauncherEnabledProperty
            = DependencyProperty.Register("IsLauncherEnabled", typeof(bool), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Raised when the dialog launcher button in the bottom-right corner of the group is clicked. 
        /// The launcher is only shown and clickable if <see cref="ShowLauncher"/> is set to <c>true</c> (and <see cref="IsLauncherEnabled"/> is also <c>true</c>).
        /// </summary>
#if NETCOREAPP
        public event EventHandler? LauncherClick;
#else
        public event EventHandler LauncherClick;
#endif

        #endregion

        #region Undocking

        // not to be implemented in 2.0

        //public bool CanBeUndocked { get => (bool)GetValue(CanBeUndockedProperty); set => SetValue(CanBeUndockedProperty, value); }

        //public static DependencyProperty CanBeUndockedProperty
        //    = DependencyProperty.Register("CanBeUndocked", typeof(bool), typeof(RibbonGroup),
        //    new FrameworkPropertyMetadata(true));

        //public bool IsUndocked { get => (bool)GetValue(IsUndockedProperty); set => SetValue(IsUndockedProperty, value); }

        //public static DependencyProperty IsUndockedProperty
        //    = DependencyProperty.Register("IsUndocked", typeof(bool), typeof(RibbonGroup),
        //    new FrameworkPropertyMetadata(false));

        //public Point UndockedPosition { get => (Point)GetValue(UndockedPositionProperty); set => SetValue(UndockedPositionProperty, value); }

        //public static DependencyProperty UndockedPositionProperty
        //    = DependencyProperty.Register("UndockedPosition", typeof(Point), typeof(RibbonGroup),
        //    new FrameworkPropertyMetadata(null));

        #endregion

    }
}
