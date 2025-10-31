using System;
using System.Windows;
using System.Windows.Documents;
using SolidShineUi;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for FlowDocumentTest.xaml
    /// </summary>
    public partial class FlowDocumentTest : ThemedUserControl
    {
        // this is meant to test and make sure that Block's FlowDirection property actually has inheritance

        public FlowDocumentTest()
        {
            InitializeComponent();
        }

        private static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlowDocumentTest ft && e.NewValue is ColorScheme cs)
            {
                ft.brdr.BorderBrush = cs.BorderColor.ToBrush();
                ft.brdr.Background = cs.LightBackgroundColor.ToBrush();
                ft.section.Background = cs.SecondHighlightColor.ToBrush();
            }
        }

        private void chkFlow1_CheckChanged(object sender, RoutedEventArgs e)
        {
            // border
            if (brdr == null) return;

            switch (chkFlow1.CheckState)
            {
                case CheckState.Unchecked:
                    brdr.FlowDirection = FlowDirection.LeftToRight;
                    break;
                case CheckState.Checked:
                    brdr.FlowDirection = FlowDirection.RightToLeft;
                    break;
                case CheckState.Indeterminate:
                    brdr.ClearValue(FlowDirectionProperty);
                    break;
                default:
                    chkFlow1.CheckState = CheckState.Indeterminate;
                    break;
            }
        }

        private void chkFlow2_CheckChanged(object sender, RoutedEventArgs e)
        {
            // section
            if (section == null) return;

            switch (chkFlow2.CheckState)
            {
                case CheckState.Unchecked:
                    section.FlowDirection = FlowDirection.LeftToRight;
                    break;
                case CheckState.Checked:
                    section.FlowDirection = FlowDirection.RightToLeft;
                    break;
                case CheckState.Indeterminate:
                    section.ClearValue(Block.FlowDirectionProperty);
                    break;
                default:
                    chkFlow2.CheckState = CheckState.Indeterminate;
                    break;
            }
        }

        private void chkFlow3_CheckChanged(object sender, RoutedEventArgs e)
        {
            // paragraph 2
            if (para2 == null) return;

            switch (chkFlow3.CheckState)
            {
                case CheckState.Unchecked:
                    para2.FlowDirection = FlowDirection.LeftToRight;
                    break;
                case CheckState.Checked:
                    para2.FlowDirection = FlowDirection.RightToLeft;
                    break;
                case CheckState.Indeterminate:
                    para2.ClearValue(Block.FlowDirectionProperty);
                    break;
                default:
                    chkFlow3.CheckState = CheckState.Indeterminate;
                    break;
            }
        }
    }
}
