using SolidShineUi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using SolidShineUi.Experimental;
using TabItem = SolidShineUi.Experimental.TabItem;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for DialogsTest.xaml
    /// </summary>
    public partial class TabControlTest : UserControl
    {
        public TabControlTest()
        {
            InitializeComponent();
        }

        #region ColorScheme

        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;

        public static DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(TabControlTest),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorScheme cs = e.NewValue as ColorScheme;

            if (d is TabControlTest s)
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            StringInputDialog sid = new StringInputDialog(ColorScheme, "Set Text", "Set the title of the new tab:", "New Tab");
            sid.Owner = Window.GetWindow(this);
            sid.ShowDialog();

            if (sid.DialogResult)
            {
                TabItem ti = new TabItem();
                ti.Title = sid.Value;
                ti.Content = new TextBlock { Text = sid.Value, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
                tabControl.Items.Add(ti);
            }

        }

        private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            tabControl.Items.Clear();
        }

        private void btnAddNoClose_Click(object sender, RoutedEventArgs e)
        {
            StringInputDialog sid = new StringInputDialog(ColorScheme, "Set Text", "Set the title of the new tab:", "New Tab");

            sid.Owner = Window.GetWindow(this);
            sid.ShowDialog();

            if (sid.DialogResult)
            {
                TabItem ti = new TabItem();
                ti.Title = sid.Value;
                ti.CanClose = false;
                ti.Content = new TextBlock { Text = sid.Value, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
                tabControl.Items.Add(ti);
            }
        }

        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
            TabItem ti = tabControl.CurrentTab;

            StringInputDialog sid = new StringInputDialog(ColorScheme, "Set Text", "Set the title of this tab:", ti.Title);

            sid.Owner = Window.GetWindow(this);
            sid.ShowDialog();

            if (sid.DialogResult)
            {
                ti.Title = sid.Value;
            }
        }
    }
}
