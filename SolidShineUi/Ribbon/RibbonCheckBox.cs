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
    /// <summary>
    /// A checkbox control to display in a <see cref="RibbonGroup"/>.
    /// </summary>
    public class RibbonCheckBox : CheckBox, IRibbonItem
    {
        static RibbonCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonCheckBox), new FrameworkPropertyMetadata(typeof(RibbonCheckBox)));
        }

        /// <summary>
        /// Get or set the size to use for this control. 
        /// For <see cref="RibbonCheckBox"/>, the only valid values are <c>Small</c> and <c>IconOnly</c>; all other values will be treated as <c>Small</c>.
        /// </summary>
        /// <remarks>
        /// When the parent group is compacted, it'll request all controls within the group to use its <see cref="CompactSize"/> instead of its <see cref="StandardSize"/>.
        /// </remarks>
        public RibbonElementSize StandardSize { get => (RibbonElementSize)GetValue(StandardSizeProperty); set => SetValue(StandardSizeProperty, value); }

        public static DependencyProperty StandardSizeProperty
            = DependencyProperty.Register("StandardSize", typeof(RibbonElementSize), typeof(RibbonCheckBox),
            new FrameworkPropertyMetadata(RibbonElementSize.Large));


        /// <summary>
        /// Get or set the size to use for this control, when the parent group is being compacted.
        /// For <see cref="RibbonCheckBox"/>, the only valid values are <c>Small</c> and <c>IconOnly</c>; all other values will be treated as <c>Small</c>.
        /// </summary>
        /// <remarks>
        /// When the parent group is compacted, it'll request all controls within the group to use its <see cref="CompactSize"/> instead of its <see cref="StandardSize"/>.
        /// For important and commonly used controls in a group, the <c>CompactSize</c> may still be same type as the <c>StandardSize</c>, but for less important or 
        /// more infrequently used controls, it's recommended to go down a size value for <c>CompactSize</c>.
        /// </remarks>
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

        /// <summary>
        /// Get or set if this control is currently being compacted (and thus should use <see cref="CompactSize"/> rather than <see cref="StandardSize"/>).
        /// Generally, this shouldn't be set manually; instead, the parent <see cref="RibbonGroup"/> will set this for controls when the group itself is being compacted.
        /// </summary>
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
