using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SolidShineUi.Ribbon
{
    public class RibbonCheckBox : CheckBox, IRibbonItem
    {
        static RibbonCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonCheckBox), new FrameworkPropertyMetadata(typeof(RibbonCheckBox)));
        }

        public RibbonElementSize StandardSize { get => (RibbonElementSize)GetValue(StandardSizeProperty); set => SetValue(StandardSizeProperty, value); }

        public static DependencyProperty StandardSizeProperty
            = DependencyProperty.Register("StandardSize", typeof(RibbonElementSize), typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata(RibbonElementSize.Large));

        public RibbonElementSize CompactSize { get => (RibbonElementSize)GetValue(CompactSizeProperty); set => SetValue(CompactSizeProperty, value); }

        public static DependencyProperty CompactSizeProperty
            = DependencyProperty.Register("CompactSize", typeof(RibbonElementSize), typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata(RibbonElementSize.Small));

        public string AccessKey { get => (string)GetValue(AccessKeyProperty); set => SetValue(AccessKeyProperty, value); }

        public static DependencyProperty AccessKeyProperty
            = DependencyProperty.Register("AccessKey", typeof(string), typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata("C"));

        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public static DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(string), typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata("Check"));

        public ImageSource LargeIcon { get => (ImageSource)GetValue(LargeIconProperty); set => SetValue(LargeIconProperty, value); }

        public static DependencyProperty LargeIconProperty
            = DependencyProperty.Register("LargeIcon", typeof(ImageSource), typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata(null));

        public ImageSource SmallIcon { get => (ImageSource)GetValue(SmallIconProperty); set => SetValue(SmallIconProperty, value); }

        public static DependencyProperty SmallIconProperty
            = DependencyProperty.Register("SmallIcon", typeof(ImageSource), typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata(null));

        public bool IsCompacted { get => (bool)GetValue(IsCompactedProperty); set => SetValue(IsCompactedProperty, value); }

        public static DependencyProperty IsCompactedProperty
            = DependencyProperty.Register("IsCompacted", typeof(bool), typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata(false));

        public int CompactOrder { get => (int)GetValue(CompactOrderProperty); set => SetValue(CompactOrderProperty, value); }

        public static DependencyProperty CompactOrderProperty
            = DependencyProperty.Register("CompactOrder", typeof(int), typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata(0));

    }
}
