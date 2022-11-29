using SolidShineUi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TabItem = SolidShineUi.TabItem;

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

            if (ti == null)
            {
                return;
            }

            StringInputDialog sid = new StringInputDialog(ColorScheme, "Set Text", "Set the title of this tab:", ti.Title);

            sid.Owner = Window.GetWindow(this);
            sid.ShowDialog();

            if (sid.DialogResult)
            {
                ti.Title = sid.Value;
            }
        }

        private void chkBottom_CheckChanged(object sender, RoutedEventArgs e)
        {
            tabControl.ShowTabsOnBottom = chkBottom.IsChecked;
        }

        private void cbbSelectTabClose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tabControl.SelectedTabClosedAction = cbbSelectTabClose.SelectedEnumValueAsEnum<SelectedTabCloseAction>();
        }

        private void TabItem_DragEnter(object sender, DragEventArgs e)
        {
            txtStatus.Text = "DragEnter on Tab 1";
        }

        private void TabItem_DragOver(object sender, DragEventArgs e)
        {
            txtStatus.Text = "DragOver on Tab 1";
        }

        private void TabItem_DragLeave(object sender, DragEventArgs e)
        {
            txtStatus.Text = "DragLeave on Tab 1";
        }

        private void TabItem_Drop(object sender, DragEventArgs e)
        {
            txtStatus.Text = "DragDrop on Tab 1";
        }

        bool changingTab = false;

        private void tabControl_TabChanged(object sender, TabItemChangeEventArgs e)
        {
            if (tabControl.SelectedTab != null)
            {
                changingTab = true;
                mnuDirty.IsChecked = tabControl.SelectedTab.IsDirty;
                mnuIconShow.IsChecked = tabControl.SelectedTab.ShowIcon;
                changingTab = false;
            }
        }

        private void btnSetIcon_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedTab != null)
            {
                TabItem ti = tabControl.SelectedTab;
                SolidShineUi.PropertyList.Dialogs.ImageBrushEditorDialog ibe = new SolidShineUi.PropertyList.Dialogs.ImageBrushEditorDialog(ColorScheme);
                ibe.LoadImage(new ImageBrush(ti.Icon));
                ibe.ShowDialog();
                if (ibe.DialogResult)
                {
                    ImageBrush br = ibe.GetImageBrush();
                    ti.Icon = br.ImageSource;
                }
            }
        }

        private void mnuDirty_Click(object sender, RoutedEventArgs e)
        {
            if (changingTab) return;

            tabControl.SelectedTab.IsDirty = mnuDirty.IsChecked;
        }

        private void mnuIconShow_Click(object sender, RoutedEventArgs e)
        {
            if (changingTab) return;

            tabControl.SelectedTab.ShowIcon = mnuIconShow.IsChecked;
        }

        private void mnuBackSolidColor_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedTab != null)
            {
                TabItem ti = tabControl.SelectedTab;
                Color precolor = Colors.Transparent;
                if (ti.TabBackground is SolidColorBrush si)
                {
                    precolor = si.Color;
                }
                ColorPickerDialog cpd = new ColorPickerDialog(ColorScheme, precolor);
                cpd.ShowDialog();
                if (cpd.DialogResult)
                {
                    ti.TabBackground = new SolidColorBrush(cpd.SelectedColor);
                }
            }
        }

        private void mnuBackSolidGradient_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedTab != null)
            {
                TabItem ti = tabControl.SelectedTab;
                Color precolor = Colors.Transparent;
                if (ti.TabBackground is SolidColorBrush si)
                {
                    precolor = si.Color;
                }
                ColorPickerDialog cpd = new ColorPickerDialog(ColorScheme, precolor);
                cpd.ShowDialog();
                if (cpd.DialogResult)
                {
                    ti.TabBackground = new LinearGradientBrush(cpd.SelectedColor, Colors.Transparent, 90);
                }
            }
        }

        private void mnuBackGradient_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedTab != null)
            {
                TabItem ti = tabControl.SelectedTab;
                LinearGradientBrush lgb = new LinearGradientBrush(Colors.Green, Colors.Transparent, 90);
                if (ti.TabBackground is LinearGradientBrush lg)
                {
                    lgb = lg;
                }
                SolidShineUi.PropertyList.Dialogs.LinearGradientEditorDialog lge = new SolidShineUi.PropertyList.Dialogs.LinearGradientEditorDialog(ColorScheme);
                lge.LoadGradient(lgb);
                lge.ShowDialog();
                if (lge.DialogResult)
                {
                    ti.TabBackground = lge.GetGradientBrush();
                }
            }
        }
    }
}
