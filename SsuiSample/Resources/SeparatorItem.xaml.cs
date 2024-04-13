using SolidShineUi;
using System.Windows.Media;
using System.Windows;
using System.Drawing.Printing;

namespace SsuiSample.Resources
{
    /// <summary>
    /// A basic separator
    /// </summary>
    public partial class SeparatorItem : SelectableUserControl
    {
        public SeparatorItem()
        {
            InitializeComponent();
        }

        public override void ApplyColorScheme(ColorScheme cs)
        {
            base.ApplyColorScheme(cs);

            SeparatorBrush = cs.BorderColor.ToBrush();
        }

        public Brush SeparatorBrush { get => (Brush)GetValue(SeparatorBrushProperty); set => SetValue(SeparatorBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="SeparatorBrush"/>. See the related property for details.</summary>
        public static DependencyProperty SeparatorBrushProperty
            = DependencyProperty.Register(nameof(SeparatorBrush), typeof(Brush), typeof(SeparatorItem),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        public Thickness SeparatorMargin { get => (Thickness)GetValue(SeparatorMarginProperty); set => SetValue(SeparatorMarginProperty, value); }

        /// <summary>The backing dependency property for <see cref="SeparatorMargin"/>. See the related property for details.</summary>
        public static DependencyProperty SeparatorMarginProperty
            = DependencyProperty.Register(nameof(SeparatorMargin), typeof(Thickness), typeof(SeparatorItem),
            new FrameworkPropertyMetadata(new Thickness(5, 0, 5, 0)));

        public double SeparatorHeight { get => (double)GetValue(SeparatorHeightProperty); set => SetValue(SeparatorHeightProperty, value); }

        /// <summary>The backing dependency property for <see cref="SeparatorHeight"/>. See the related property for details.</summary>
        public static DependencyProperty SeparatorHeightProperty
            = DependencyProperty.Register(nameof(SeparatorHeight), typeof(double), typeof(SeparatorItem),
            new FrameworkPropertyMetadata(1.0));


    }
}
