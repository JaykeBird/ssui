using SolidShineUi;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

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

            // the second SelectPanel, basic2, serves as a way to test ItemsSource with a stored SelectableCollection
            // for testing/trying with a SelectableCollectionView, uncomment the below text

            //var lcv = new SelectableCollectionView<SelectableUserControl>((Collection<SelectableUserControl>)Resources["selList"]);
            //lcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("Text", System.ComponentModel.ListSortDirection.Ascending));
            //basic2.ItemsSource = lcv;
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
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(TwoBasicSelectPanels),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ColorScheme cs)
            {
                if (d is TwoBasicSelectPanels s)
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

            var list = (SelectableListItemCollection)Resources["selList"];
            list.Clear();
        }
    }

    // WPF's XAML doesn't support generics, so this is just a basic wrapper so that I can use it as a Resource in XAML
    internal class SelectableListItemCollection : SelectableCollection<SelectableUserControl>
    {

    }
}
