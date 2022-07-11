using SolidShineUi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for TwoBasicSelectPanels.xaml
    /// </summary>
    public partial class TwoBasicSelectPanels : UserControl
    {
        public TwoBasicSelectPanels()
        {
            InitializeComponent();
        }

        #region ColorScheme

        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;

        public static DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(TwoBasicSelectPanels),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorScheme cs = e.NewValue as ColorScheme;

            if (d is TwoBasicSelectPanels s)
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
            basic1.Items.Add(new SelectableItem("TEXT"));
        }

        private void btnAddRight_Click(object sender, RoutedEventArgs e)
        {
            var list = (SelectableListItemCollection)Resources["selList"];
            list.Add(new SelectableItem("TEXT"));
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            basic1.Items.Clear();
            basic2.Items.Clear();
        }
    }

    internal class SelectableListItemCollection : SelectableCollection<SelectableUserControl>
    {

    }
}
