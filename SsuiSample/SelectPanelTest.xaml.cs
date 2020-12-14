using SolidShineUi;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SsuiSample.Resources;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for SelectPanel.xaml
    /// </summary>
    public partial class SelectPanelTest : UserControl
    {
        public SelectPanelTest()
        {
            InitializeComponent();

            // Select Panel options
            selPanel.MultiSelect = true;
        }

        private void btnAddBlank_Click(object sender, RoutedEventArgs e)
        {
            SelectableItem si = new SelectableItem();
            si.Text = "Basic";
            si.TextTrimming = TextTrimming.CharacterEllipsis;
            si.ShowImage = false;

            selPanel.AddItem(si);
        }

        private void mnuSelImage_Click(object sender, RoutedEventArgs e)
        {
            SelectableItem si = new SelectableItem();
            si.Text = "Image";
            si.TextTrimming = TextTrimming.CharacterEllipsis;
            si.ShowImage = true;
            si.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/ImageColor.png", UriKind.RelativeOrAbsolute));
            si.ImageWidth = 16;

            selPanel.AddItem(si);
        }

        private void mnuSelEdit_Click(object sender, RoutedEventArgs e)
        {
            SelectableItem si = new SelectableItem();
            si.Text = "Editable";
            si.AllowTextEditing = true;
            si.TextTrimming = TextTrimming.CharacterEllipsis;
            si.ShowImage = false;

            selPanel.AddItem(si);
        }

        private void mnuSelSep_Click(object sender, RoutedEventArgs e)
        {
            selPanel.AddItem(new SeparatorItem());
        }

        private void btnSelRemove_Click(object sender, RoutedEventArgs e)
        {
            selPanel.RemoveSelectedItems();
        }

        private void btnSelRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            selPanel.Clear();
        }

        private void btnSelUp_Click(object sender, RoutedEventArgs e)
        {
            selPanel.MoveSelectedItemsUp();
        }

        private void btnSelDown_Click(object sender, RoutedEventArgs e)
        {
            selPanel.MoveSelectedItemsDown();
        }

        private void selPanel_ItemsAdded(object sender, SelectionChangedEventArgs e)
        {
            lblTotalItems.Text = selPanel.Count.ToString();
            lblSelItems.Text = selPanel.SelectionCount.ToString();
        }

        private void selPanel_ItemsRemoved(object sender, SelectionChangedEventArgs e)
        {
            lblTotalItems.Text = selPanel.Count.ToString();
            lblSelItems.Text = selPanel.SelectionCount.ToString();
        }

        private void selPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblTotalItems.Text = selPanel.Count.ToString();
            lblSelItems.Text = selPanel.SelectionCount.ToString();
        }

        private void btnSelAll_Click(object sender, RoutedEventArgs e)
        {
            selPanel.SelectAll();
        }

        private void btnSelNone_Click(object sender, RoutedEventArgs e)
        {
            selPanel.DeselectAll();
        }

        #region ColorScheme

        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;

        public static DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(SelectPanelTest),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorScheme cs = e.NewValue as ColorScheme;

            if (d is SelectPanelTest s)
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

    }
}
