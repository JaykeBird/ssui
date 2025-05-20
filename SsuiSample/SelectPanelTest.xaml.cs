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

        #region Add Item Methods

        private void btnAddBlank_Click(object sender, RoutedEventArgs e)
        {
            SelectableItem si = new SelectableItem();
            si.Text = "Basic";
            si.TextTrimming = TextTrimming.CharacterEllipsis;
            si.ShowImage = false;
            si.ShowCheckbox = chkShowChk.IsChecked;

            selPanel.Items.Add(si);
        }

        private void mnuAddImageBasic_Click(object sender, RoutedEventArgs e)
        {
            ImageTextListItem itle = new ImageTextListItem();
            itle.Title = "Image";
            itle.ShowIcon = true;
            itle.Icon = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/ImageColor.png", UriKind.RelativeOrAbsolute));
            itle.CanRemove = false;

            selPanel.Items.Add(itle);
        }

        private void mnuAddImageFull_Click(object sender, RoutedEventArgs e)
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

        private void mnuAddEditable_Click(object sender, RoutedEventArgs e)
        {
            SelectableItem si = new SelectableItem();
            si.Text = "Editable";
            si.AllowTextEditing = true;
            si.TextTrimming = TextTrimming.CharacterEllipsis;
            si.ShowImage = false;
            si.ShowCheckbox = chkShowChk.IsChecked;

            selPanel.Items.Add(si);
        }

        private void mnuAddButton_Click(object sender, RoutedEventArgs e)
        {
            FlatButton si = new FlatButton();
            si.Content = "Button";
            si.SelectOnClick = true;
            si.HorizontalAlignment = HorizontalAlignment.Stretch;

            selPanel.Items.Add(si);
        }

        private void mnuAddSeparator_Click(object sender, RoutedEventArgs e)
        {
            selPanel.Items.Add(new SeparatorItem());
        }

        #endregion

        private void btnSelRemove_Click(object sender, RoutedEventArgs e)
        {
            var items = new List<IClickSelectableControl>(selPanel.Items.SelectedItems);
            foreach (var item in items)
            {
                selPanel.Items.Remove(item);
            }
        }

        private void btnSelRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            selPanel.Items.Clear();
        }

        private void btnSelMoveUp_Click(object sender, RoutedEventArgs e)
        {
            selPanel.MoveSelectedItemsUp();
        }

        private void btnSelMoveDown_Click(object sender, RoutedEventArgs e)
        {
            selPanel.MoveSelectedItemsDown();
        }
        private void btnSelAll_Click(object sender, RoutedEventArgs e)
        {
            selPanel.Items.SelectRange(selPanel.Items);
        }

        private void btnSelNone_Click(object sender, RoutedEventArgs e)
        {
            selPanel.Items.ClearSelection();
        }

        #region Select Panel Event Listeners

        private void selPanel_ItemsAdded(object sender, RoutedSelectionChangedEventArgs<IClickSelectableControl> e)
        {
            lblTotalItems.Text = selPanel.Items.Count.ToString();
            lblSelItems.Text = selPanel.Items.SelectedItems.Count.ToString();
        }

        private void selPanel_ItemsRemoved(object sender, RoutedSelectionChangedEventArgs<IClickSelectableControl> e)
        {
            lblTotalItems.Text = selPanel.Items.Count.ToString();
            lblSelItems.Text = selPanel.Items.SelectedItems.Count.ToString();
        }

        private void selPanel_SelectionChanged(object sender, RoutedSelectionChangedEventArgs<IClickSelectableControl> e)
        {
            lblTotalItems.Text = selPanel.Items.Count.ToString();
            lblSelItems.Text = selPanel.Items.SelectedItems.Count.ToString();
        }

        #endregion

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
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(SelectPanelTest),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ColorScheme cs)
            {
                if (d is SelectPanelTest s)
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

        private void chkShowChk_CheckChanged(object sender, RoutedEventArgs e)
        {
            foreach (IClickSelectableControl item in selPanel.Items)
            {
                if (item is SelectableItem si)
                {
                    si.ShowCheckbox = chkShowChk.IsChecked;
                }
            }
        }
    }
}
