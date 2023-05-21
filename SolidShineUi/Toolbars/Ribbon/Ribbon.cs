using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SolidShineUi.Toolbars.Ribbon
{
    /// <summary>
    /// A toolbar that displays various commands under a series of tabs, similar to what is present in Microsoft Office or Autodesk software.
    /// </summary>
    public class Ribbon : Control
    {
        static Ribbon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ribbon), new FrameworkPropertyMetadata(typeof(Ribbon)));
        }

        public Ribbon()
        {
            SetValue(ItemsPropertyKey, new ObservableCollection<RibbonTab>());
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        private static readonly DependencyPropertyKey ItemsPropertyKey
            = DependencyProperty.RegisterReadOnly("Items", typeof(ObservableCollection<RibbonTab>), typeof(RibbonTab),
            new FrameworkPropertyMetadata(new ObservableCollection<RibbonTab>()));

        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the list of tabs in this Ribbon. This Items property can be used to add and remove items.
        /// </summary>
        [Category("Common")]
        public ObservableCollection<RibbonTab> Items
        {
            get { return (ObservableCollection<RibbonTab>)GetValue(ItemsProperty); }
            private set { SetValue(ItemsPropertyKey, value); }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty TopRightElementProperty = DependencyProperty.Register(
            "TopRightElement", typeof(UIElement), typeof(Ribbon),
            new PropertyMetadata(null));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets or sets the UI element to place in the top-right of the control.
        /// </summary>
        public UIElement TopRightElement
        {
            get
            {
                return (UIElement)GetValue(TopRightElementProperty);
            }
            set
            {
                SetValue(TopRightElementProperty, value);
            }
        }


        public bool ShowOnlyTabs { get => (bool)GetValue(ShowOnlyTabsProperty); set => SetValue(ShowOnlyTabsProperty, value); }

        public static DependencyProperty ShowOnlyTabsProperty
            = DependencyProperty.Register("ShowOnlyTabs", typeof(bool), typeof(Ribbon),
            new FrameworkPropertyMetadata(false));

        public bool AllowControlReordering { get => (bool)GetValue(AllowControlReorderingProperty); set => SetValue(AllowControlReorderingProperty, value); }

        public static DependencyProperty AllowControlReorderingProperty
            = DependencyProperty.Register("AllowControlReordering", typeof(bool), typeof(Ribbon),
            new FrameworkPropertyMetadata(true));

    }
}
