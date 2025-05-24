using SolidShineUi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for DialogsTest.xaml
    /// </summary>
    public partial class DialogsTest : UserControl
    {
        public DialogsTest()
        {
            InitializeComponent();
        }

        #region ColorScheme

        /// <summary>
        /// Raised when the value of <see cref="ColorScheme"/> changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

        public static DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(DialogsTest),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ColorScheme cs)
            {
                if (d is DialogsTest s)
                {
                    s.ColorSchemeChanged?.Invoke(d, e);
                    s.ApplyColorScheme(cs);
                }
            }
        }

        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        public void ApplyColorScheme(ColorScheme cs)
        {
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }
        }
        #endregion

        private void btnSetText_Click(object sender, RoutedEventArgs e)
        {
            StringInputDialog sid = new StringInputDialog(ColorScheme, "Set Text", "Set the sample text to use:", txtSampleText.Text);

            // by default, these are all set to true, but I'll list them here so you can play with them if desired
            sid.SelectTextOnFocus = true;
            sid.EnterKeyConfirms = true;
            sid.EscapeKeyCancels = true;

            sid.Owner = Window.GetWindow(this);
            sid.ShowDialog();
            
            if (sid.DialogResult)
            {
                txtSampleText.Text = sid.Value;
            }
        }

        private void btnSetFont_Click(object sender, RoutedEventArgs e)
        {
            FontSelectDialog fsd = new FontSelectDialog();
            fsd.ColorScheme = ColorScheme;

            // load in from control
            fsd.SelectedFontFamily = txtSampleText.FontFamily;
            fsd.SelectedFontSize = txtSampleText.FontSize;
            fsd.SelectedFontStyle = txtSampleText.FontStyle;
            fsd.SelectedFontWeight = txtSampleText.FontWeight;
            fsd.SelectedTextDecorations = txtSampleText.TextDecorations;

            // this also works to load in from control
            // uncomment this and comment out the section above if you want to try it; the experience should be identical
            // fsd.LoadFromControl(txtSampleText);

            // by default, these are all set to true, but I'll list them here so you can play with them if desired
            fsd.ShowDecorations = true;
            fsd.ShowSizes = true;
            fsd.ShowStyles = true;
            fsd.ShowWeights = true;

            fsd.PreviewText = txtSampleText.Text;

            fsd.Owner = Window.GetWindow(this);
            fsd.ShowDialog();

            if (fsd.DialogResult)
            {
                txtSampleText.FontFamily = fsd.SelectedFontFamily;
                txtSampleText.FontSize = fsd.SelectedFontSize;
                txtSampleText.FontStyle = fsd.SelectedFontStyle;
                txtSampleText.FontWeight = fsd.SelectedFontWeight;
                txtSampleText.TextDecorations = fsd.SelectedTextDecorations;

                // this also works to set the control
                // uncomment this and comment out the section above if you want to try it; the experience should be identical
                // fsd.SetToControl(txtSampleText);
            }
        }

        private void btnSetColor_Click(object sender, RoutedEventArgs e)
        {
            Color c = Colors.Black;
            if (txtSampleText.Foreground is SolidColorBrush sc)
            {
                c = sc.Color;
            }
            ColorPickerDialog cpd = new ColorPickerDialog(ColorScheme, c);

            // by default, these are all set to true, but I'll list them here so you can play with them if desired
            cpd.ShowImageTab = true;
            cpd.ShowPaletteFileTab = true;
            cpd.ShowSlidersTab = true;
            cpd.ShowSwatchesTab = true;

            cpd.ShowTransparencyControls = true;

            cpd.Owner = Window.GetWindow(this);
            cpd.ShowDialog();

            if (cpd.DialogResult)
            {
                txtSampleText.Foreground = new SolidColorBrush(cpd.SelectedColor);
            }
        }
    }
}
