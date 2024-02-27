using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace SolidShineUi.Ribbon
{
    /// <summary>
    /// A button to display on a <see cref="Ribbon"/> (generally within a <see cref="RibbonGroup"/> on a <see cref="RibbonTab"/>), which displays a menu when clicked.
    /// </summary>
    [ContentProperty("Menu")]
    public class RibbonMenuButton : MenuButton, IRibbonItem
    {
        static RibbonMenuButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonMenuButton), new FrameworkPropertyMetadata(typeof(RibbonMenuButton)));
        }

        public RibbonElementSize StandardSize { get => (RibbonElementSize)GetValue(StandardSizeProperty); set => SetValue(StandardSizeProperty, value); }

        public static DependencyProperty StandardSizeProperty
            = DependencyProperty.Register("StandardSize", typeof(RibbonElementSize), typeof(RibbonMenuButton),
            new FrameworkPropertyMetadata(RibbonElementSize.Large));

        public RibbonElementSize CompactSize { get => (RibbonElementSize)GetValue(CompactSizeProperty); set => SetValue(CompactSizeProperty, value); }

        public static DependencyProperty CompactSizeProperty
            = DependencyProperty.Register("CompactSize", typeof(RibbonElementSize), typeof(RibbonMenuButton),
            new FrameworkPropertyMetadata(RibbonElementSize.Small));

        public string AccessKey { get => (string)GetValue(AccessKeyProperty); set => SetValue(AccessKeyProperty, value); }

        public static DependencyProperty AccessKeyProperty
            = DependencyProperty.Register("AccessKey", typeof(string), typeof(RibbonMenuButton),
            new FrameworkPropertyMetadata("B"));

        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public static DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(string), typeof(RibbonMenuButton),
            new FrameworkPropertyMetadata("Button"));

        public ImageSource LargeIcon { get => (ImageSource)GetValue(LargeIconProperty); set => SetValue(LargeIconProperty, value); }

        public static DependencyProperty LargeIconProperty
            = DependencyProperty.Register("LargeIcon", typeof(ImageSource), typeof(RibbonMenuButton),
            new FrameworkPropertyMetadata(null));

        public ImageSource SmallIcon { get => (ImageSource)GetValue(SmallIconProperty); set => SetValue(SmallIconProperty, value); }

        public static DependencyProperty SmallIconProperty
            = DependencyProperty.Register("SmallIcon", typeof(ImageSource), typeof(RibbonMenuButton),
            new FrameworkPropertyMetadata(null));

        public bool IsCompacted { get => (bool)GetValue(IsCompactedProperty); set => SetValue(IsCompactedProperty, value); }

        public static DependencyProperty IsCompactedProperty
            = DependencyProperty.Register("IsCompacted", typeof(bool), typeof(RibbonMenuButton),
            new FrameworkPropertyMetadata(false));

        public int CompactOrder { get => (int)GetValue(CompactOrderProperty); set => SetValue(CompactOrderProperty, value); }

        public static DependencyProperty CompactOrderProperty
            = DependencyProperty.Register("CompactOrder", typeof(int), typeof(RibbonMenuButton),
            new FrameworkPropertyMetadata(0));

    }
}
