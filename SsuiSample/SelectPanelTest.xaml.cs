using SolidShineUi;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SsuiSample.Resources;
using SolidShineUi.Utils;

namespace SsuiSample
{

    public partial class SelectPanelTest : UserControl
    {
        public SelectPanelTest()
        {
            InitializeComponent();

            // Select Panel options
            selPanel.Items.CanSelectMultiple = true;
        }

        private void btnAddBlank_Click(object sender, RoutedEventArgs e)
        {
            SelectableItem si = new SelectableItem();
            si.Text = "Basic";
            si.TextTrimming = TextTrimming.CharacterEllipsis;
            si.ShowImage = false;
            si.ShowCheckbox = chkShowChk.IsChecked;

            selPanel.Items.Add(si);
        }

        private void mnuSelImage_Click(object sender, RoutedEventArgs e)
        {
            ImageTextListItem itle = new ImageTextListItem();
            itle.Title = "Image";
            itle.ShowIcon = true;
            itle.Icon = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/ImageColor.png", UriKind.RelativeOrAbsolute));
            itle.CanRemove = false;

            selPanel.Items.Add(itle);
        }


        private void mnuSelImageLegacy_Click(object sender, RoutedEventArgs e)
        {
            SelectableItem si = new SelectableItem();
            si.Text = "Image";
            si.TextTrimming = TextTrimming.CharacterEllipsis;
            si.ShowImage = true;
            si.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/ImageColor.png", UriKind.RelativeOrAbsolute));
            si.ImageWidth = 16;
            si.ShowCheckbox = chkShowChk.IsChecked;

            selPanel.Items.Add(si);
        }

        private void mnuSelEdit_Click(object sender, RoutedEventArgs e)
        {
            SelectableItem si = new SelectableItem();
            si.Text = "Editable";
            si.AllowTextEditing = true;
            si.TextTrimming = TextTrimming.CharacterEllipsis;
            si.ShowImage = false;
            si.ShowCheckbox = chkShowChk.IsChecked;

            selPanel.Items.Add(si);
        }

        private void mnuSelSep_Click(object sender, RoutedEventArgs e)
        {
            selPanel.Items.Add(new SeparatorItem());
        }

        private void btnSelRemove_Click(object sender, RoutedEventArgs e)
        {
            var items = new List<SelectableUserControl>(selPanel.Items.SelectedItems);
            foreach (var item in items)
            {
                selPanel.Items.Remove(item);
            }
        }

        private void btnSelRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            selPanel.Items.Clear();
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
            lblTotalItems.Text = selPanel.Items.Count.ToString();
            lblSelItems.Text = selPanel.Items.SelectedItems.Count.ToString();
        }

        private void selPanel_ItemsRemoved(object sender, SelectionChangedEventArgs e)
        {
            lblTotalItems.Text = selPanel.Items.Count.ToString();
            lblSelItems.Text = selPanel.Items.SelectedItems.Count.ToString();
        }

        private void selPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblTotalItems.Text = selPanel.Items.Count.ToString();
            lblSelItems.Text = selPanel.Items.SelectedItems.Count.ToString();
        }

        private void btnSelAll_Click(object sender, RoutedEventArgs e)
        {
            selPanel.Items.SelectRange(selPanel.Items);
        }

        private void btnSelNone_Click(object sender, RoutedEventArgs e)
        {
            selPanel.Items.ClearSelection();
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

        private void chkShowChk_CheckChanged(object sender, RoutedEventArgs e)
        {
            foreach (SelectableUserControl item in selPanel.Items)
            {
                if (item is SelectableItem si)
                {
                    si.ShowCheckbox = chkShowChk.IsChecked;
                }
            }
        }
    }
}
