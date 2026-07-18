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
    public partial class TwoBasicSelectPanels : ThemedUserControl
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
