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

        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;

        public static DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(PropertyListTest),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorScheme cs = e.NewValue as ColorScheme;

            if (d is PropertyListTest s)
            {
                s.ColorSchemeChanged?.Invoke(d, e);
                s.ApplyColorScheme(cs);
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

            public DateOnly ReallyOldDay { get; set; } = DateOnly.FromDayNumber(2614); // lol I dunno a random number

            public TimeOnly AfterNoon { get; set; } = new TimeOnly(12, 23, 40);
#endif

            public Color? ColCol { get; set; } = Colors.Orange;

            public FontStyle? NullStyle { get; set; } = null;

            public Thickness? NullThickness { get; set; } = null;

            public Version VerVersion { get; set; } = new Version(1, 9, 5);

            public DateTime? OldTime { get; set; } = new DateTime(1981, 06, 12, 15, 16, 20);
        }
    }
}
