using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SolidShineUi;
using SolidShineUi.Utils;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for ColorsTest.xaml
    /// </summary>
    public partial class ColorsTest : ThemedUserControl
    {
        public ColorsTest()
        {
            InitializeComponent();
        }

        SsuiAppTheme TryGetSsuiAppTheme()
        {
            if (SsuiTheme is SsuiAppTheme sat)
            {
                // in most cases, it should be this - the inherited SsuiTheme should be an SsuiAppTheme
                return sat;
            }
            else if (Window.GetWindow(this) is ThemedWindow fw)
            {
                // okay, let's try to pull from the parent window if possible, as it should have a SsuiAppTheme as its theme
                return fw.SsuiTheme;
            }
            else
            {
                // okay, I guess we'll just go with the default
                return new SsuiAppTheme();
            }
        }

        ColorListItemDisplay cDisp = ColorListItemDisplay.Hex;

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cpd = new ColorPickerDialog(btnColor.Color);
            cpd.SsuiTheme = TryGetSsuiAppTheme();

            cpd.ShowDialog();
            if (cpd.DialogResult)
            {
                ColorListItem cli = new ColorListItem(cpd.SelectedColor, cDisp);
                selInputs.Items.Add(cli);
            }
            Recalculate();
        }

        private void btnAddX11_Click(object sender, RoutedEventArgs e)
        {
            foreach ((Color c, string name) in ColorsHelper.GetAllX11ColorsAndNames())
            {
                ColorListItem cli = new ColorListItem(c, cDisp);
                cli.ToolTip = name;
                selInputs.Items.Add(cli);
            }
            Recalculate();
        }

        private void btnColor_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cpd = new ColorPickerDialog(btnColor.Color);
            cpd.SsuiTheme = TryGetSsuiAppTheme();

            cpd.ShowDialog();
            if (cpd.DialogResult)
            {
                btnColor.Color = cpd.SelectedColor;
            }
            Recalculate();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            selInputs.RemoveSelectedItems();
            Recalculate();
        }

        private void cbbFunction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Recalculate();
        }

        private void Recalculate()
        {
            if (cbbFunction == null || selInputs == null || selResult == null) return;
            if (cbbFunction.SelectedIndex == -1) return;

            selResult.Items.Clear();

            // if we don't have any items, no point in doing anything
            if (selInputs.Items.Count == 0) return;

            switch (cbbFunction.SelectedIndex)
            {
                case 0: //<ComboBoxItem>Invert</ComboBoxItem>
                    RunCalculation((c) => c.GetInversion());
                    break;
                case 1: //<ComboBoxItem>Grayscale (Flat)</ComboBoxItem>
                    RunCalculation((c) => c.ToGrayscale(ColorGrayscaleMethod.FlatAverage));
                    break;
                case 2: //<ComboBoxItem>Grayscale (Rec 601)</ComboBoxItem>
                    RunCalculation((c) => c.ToGrayscale(ColorGrayscaleMethod.Rec601));
                    break;
                case 3: //<ComboBoxItem>Grayscale (Rec 709)</ComboBoxItem>
                    RunCalculation((c) => c.ToGrayscale(ColorGrayscaleMethod.Rec709));
                    break;
                case 4: //<ComboBoxItem>Grayscale (Rec 601, no gamma)</ComboBoxItem>
                    RunCalculation((c) => c.ToGrayscale(ColorGrayscaleMethod.Rec601_NoGamma));
                    break;
                case 5: //<ComboBoxItem>Grayscale (Rec 709, no gamma)</ComboBoxItem>
                    RunCalculation((c) => c.ToGrayscale(ColorGrayscaleMethod.Rec709_NoGamma));
                    break;
                case 6: //<ComboBoxItem>Grayscale (Desaturate)</ComboBoxItem>
                    RunCalculation((c) => c.ToGrayscale(ColorGrayscaleMethod.Desaturate));
                    break;
                case 7:
                    RunCalculation((c) => c.ToGrayscale(ColorGrayscaleMethod.Luminance));
                    break;
                case 8: //<ComboBoxItem>Convert to Pure Hue</ComboBoxItem>
                    RunCalculation((c) =>
                    {
                        ColorsHelper.ToHSV(c, out double h, out double _, out double _);
                        return ColorsHelper.CreateFromHSV(h, 1, 1);
                    });
                    break;
                case 9: //<ComboBoxItem>Blend 25%</ComboBoxItem>
                    RunCalculation((c) => ColorsHelper.BlendWithGamma(btnColor.Color, c, 0.25));
                    break;
                case 10: //<ComboBoxItem>Blend 50%</ComboBoxItem>
                    RunCalculation((c) => ColorsHelper.BlendWithGamma(btnColor.Color, c, 0.5));
                    break;
                case 11: //<ComboBoxItem>Blend 75%</ComboBoxItem>
                    RunCalculation((c) => ColorsHelper.BlendWithGamma(btnColor.Color, c, 0.75));
                    break;
                case 12: //<ComboBoxItem>Blend 25%, no gamma</ComboBoxItem>
                    RunCalculation((c) => ColorsHelper.Blend(btnColor.Color, c, 0.25));
                    break;
                case 13: //<ComboBoxItem>Blend 50%, no gamma</ComboBoxItem>
                    RunCalculation((c) => ColorsHelper.Blend(btnColor.Color, c, 0.5));
                    break;
                case 14: //<ComboBoxItem>Blend 75%, no gamma</ComboBoxItem>
                    RunCalculation((c) => ColorsHelper.Blend(btnColor.Color, c, 0.75));
                    break;
                case 15: //<ComboBoxItem>Convert to HSV and Back</ComboBoxItem>
                    RunCalculation((c) =>
                    {
                        ColorsHelper.ToHSV(c, out double h, out double s, out double v);
                        return ColorsHelper.CreateFromHSV(h, s, v);
                    });
                    break;
                case 16: //<ComboBoxItem>Convert to HSL and Back</ComboBoxItem>
                    RunCalculation((c) =>
                    {
                        ColorsHelper.ToHSL(c, out double h, out double s, out double l);
                        return ColorsHelper.CreateFromHSL(h, s, l);
                    });
                    break;
                case 17: //<ComboBoxItem>Multiply By 2</ComboBoxItem>
                    RunCalculation((c) => c * 2);
                    break;
                default:
                    break;
            }
        }

        void RunCalculation(Func<Color, Color> function)
        {
            foreach (var item in selInputs.Items)
            {
                if (item is ColorListItem cli)
                {
                    Color resultCol = function(cli.Color);
                    ColorListItem resultCli = new ColorListItem(resultCol, cDisp);
                    selResult.Items.Add(resultCli);
                }
            }
        }

        private void cbbDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbDisplay.SelectedIndex == -1) return;

            cDisp = cbbDisplay.SelectedEnumValueAsEnum<ColorListItemDisplay>();

            if (selInputs == null || selResult == null) return;

            foreach (var item in selInputs.Items)
            {
                if (item is ColorListItem cli)
                {
                    cli.DisplayMode = cDisp;
                }
            }
            foreach (var item in selResult.Items)
            {
                if (item is ColorListItem cli)
                {
                    cli.DisplayMode = cDisp;
                }
            }
        }

        bool _internalScrollAction = false;

        private void selInputs_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_internalScrollAction) return;

            if (!chkScrollMatch.IsChecked)
            {
                _internalScrollAction = false;
                return;
            }
            
            _internalScrollAction = true;

            if (e.VerticalChange != 0)
            {
                selResult.ScrollToVerticalOffset(e.VerticalOffset);
            }

            _internalScrollAction = false;
        }

        private void selResult_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_internalScrollAction) return;

            if (!chkScrollMatch.IsChecked)
            {
                _internalScrollAction = false;
                return;
            }

            _internalScrollAction = true;

            if (e.VerticalChange != 0)
            {
                selInputs.ScrollToVerticalOffset(e.VerticalOffset);
            }

            _internalScrollAction = false;
        }
    }
}
