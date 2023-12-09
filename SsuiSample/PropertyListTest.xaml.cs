using SolidShineUi;
using SolidShineUi.PropertyList;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for PropertyListTest.xaml
    /// </summary>
    public partial class PropertyListTest : UserControl
    {
        public PropertyListTest()
        {
            InitializeComponent();

            // set PropertyList properties
            prop.DisplayOptions = PropertyListDisplayFlags.HidePropertyListHide;
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
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(PropertyListTest),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ColorScheme cs)
            {
                if (d is PropertyListTest s)
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

        private void btnSel1_Click(object sender, RoutedEventArgs e)
        {
            prop.LoadObject(cbbComboBox);
        }

        private void btnSel2_Click(object sender, RoutedEventArgs e)
        {
            prop.LoadObject(txtTextBox);
        }

        private void btnSel3_Click(object sender, RoutedEventArgs e)
        {
            prop.LoadObject(btnButton);
        }

        private void btnSel4_Click(object sender, RoutedEventArgs e)
        {
            prop.LoadObject(ctrFileSelect);
        }

        private void btnSel5_Click(object sender, RoutedEventArgs e)
        {
            prop.LoadObject(grdItems);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            prop.Clear();
        }

        private void btnSelCs_Click(object sender, RoutedEventArgs e)
        {
            prop.LoadObject(ColorScheme);
        }

        private void btnSelObj_Click(object sender, RoutedEventArgs e)
        {
            prop.LoadObject(ptto);
            prop.ObjectDisplayName = "Internal test object";
        }

        PropertyTestTestObject ptto = new PropertyTestTestObject();

        public class PropertyTestTestObject
        {
            public System.Collections.Generic.List<int> Numbers { get; set; } = new System.Collections.Generic.List<int>() { 5, 12, 48, 2 };

            public string TestString { get; set; } = "noodles";

            [PropertyListHide]
            public double DoubleNumber { get; set; } = 3.6;

            public decimal ActualDecimal { get; set; } = 7.3m;

            public Brush TestBrush { get; set; } = BrushFactory.Create(Colors.Green, Colors.Orange, 90.0);

            public char? CharChar { get; set; } = 'c';
            public char CharCharY { get; set; } = 'h';

#if NETCOREAPP
            public Rune RuneChar { get; set; } = new Rune('r');

#endif

            public Color? ColCol { get; set; } = Colors.Orange;

            public FontStyle? NullStyle { get; set; } = null;

            public Thickness? NullThickness { get; set; } = null;

            public Version VerVersion { get; set; } = new Version(1, 9, 5);
        }
    }
}
