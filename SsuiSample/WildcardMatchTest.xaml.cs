using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SolidShineUi;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for WildcardMatchTest.xaml
    /// </summary>
    public partial class WildcardMatchTest : UserControl
    {

        public WildcardMatchTest()
        {
            InitializeComponent();
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
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(WildcardMatchTest),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ColorScheme cs)
            {
                if (d is WildcardMatchTest s)
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
            sid.ColorScheme = ColorScheme;
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
