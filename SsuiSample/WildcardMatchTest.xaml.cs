using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using SolidShineUi;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for WildcardMatchTest.xaml
    /// </summary>
    public partial class WildcardMatchTest : ThemedUserControl
    {
        public WildcardMatchTest()
        {
            InitializeComponent();
        }

        SsuiAppTheme TryGetSsuiAppTheme()
        {
            if (SsuiTheme is SsuiAppTheme sat)
            {
                // in most cases, it should be this - the inherited SsuiTheme should be an SsuiAppTheme
                return sat;
            }
            else if (Window.GetWindow(this) is ThemedWindow fw)
            {
                // okay, let's try to pull from the parent window if possible, as it should have a SsuiAppTheme as its theme
                return fw.SsuiTheme;
            }
            else
            {
                // okay, I guess we'll just go with the default
                return new SsuiAppTheme();
            }
        }

        public void RunWildcard()
        {
            foreach (SelectableItem item in selList.Items.OfType<SelectableItem>())
            {
                if (WildcardMatch.MatchesWildcard(item.Text, txtMatch.Text, chkCase.IsChecked))
                {
                    item.RightText = "matches";
                }
                else
                {
                    item.RightText = "no match";
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            StringInputDialog sid = new StringInputDialog();
            sid.Owner = Window.GetWindow(this);
            sid.Title = "Wildcard Match";
            sid.SsuiTheme = TryGetSsuiAppTheme();
            
            sid.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            sid.Description = "The text to match against:";

            sid.ShowDialog();

            if (sid.DialogResult)
            {
                SelectableItem si = new SelectableItem();
                si.Text = sid.Value;
                selList.Items.Add(si);
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            selList.RemoveSelectedItems();
        }

        private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            selList.Items.Clear();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            foreach (SelectableItem item in selList.Items.OfType<SelectableItem>())
            {
                item.RightText = "";
            }
        }

        private void btnDeselect_Click(object sender, RoutedEventArgs e)
        {
            selList.Items.ClearSelection();
        }

        private void btnMatch_Click(object sender, RoutedEventArgs e)
        {
            RunWildcard();
        }
    }
}
