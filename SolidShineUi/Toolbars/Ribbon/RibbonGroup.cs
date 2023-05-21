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

namespace SolidShineUi.Toolbars.Ribbon
{
    public class RibbonGroup : Control
    {
        static RibbonGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ribbon), new FrameworkPropertyMetadata(typeof(RibbonGroup)));
        }

        public RibbonGroup()
        {
            SetValue(ItemsPropertyKey, new ObservableCollection<IRibbonItem>());
        }

        [Category("Common")]
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public static DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(string), typeof(RibbonGroup),
            new FrameworkPropertyMetadata("Group"));


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        private static readonly DependencyPropertyKey ItemsPropertyKey
            = DependencyProperty.RegisterReadOnly("Items", typeof(ObservableCollection<IRibbonItem>), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(new ObservableCollection<IRibbonItem>()));

        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the list of items in this RibbonGroup. This Items property can be used to add and remove items.
        /// </summary>
        [Category("Common")]
        public ObservableCollection<IRibbonItem> Items
        {
            get { return (ObservableCollection<IRibbonItem>)GetValue(ItemsProperty); }
            private set { SetValue(ItemsPropertyKey, value); }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        private static readonly DependencyPropertyKey ExpandedItemsPropertyKey
            = DependencyProperty.RegisterReadOnly("ExpandedItems", typeof(ObservableCollection<IRibbonItem>), typeof(RibbonGroup),
            new FrameworkPropertyMetadata(new ObservableCollection<IRibbonItem>()));

        public static readonly DependencyProperty ExpandedItemsProperty = ExpandedItemsPropertyKey.DependencyProperty;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the list of items in this RibbonGroup's expanded panel. This Items property can be used to add and remove items. If there are no items, the expanded panel will not be shown.
        /// </summary>
        [Category("Common")]
        public ObservableCollection<IRibbonItem> ExpandedItems
        {
            get { return (ObservableCollection<IRibbonItem>)GetValue(ExpandedItemsProperty); }
            private set { SetValue(ExpandedItemsPropertyKey, value); }
        }

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


        public event EventHandler LauncherClick;

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
