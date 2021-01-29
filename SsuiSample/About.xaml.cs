using SolidShineUi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : FlatWindow
    {
        public About()
        {
            InitializeComponent();

            lblNetVersion.Text = "Running on .NET version " + Environment.Version;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
