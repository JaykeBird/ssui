using SolidShineUi;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

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
            lblSsuiVersion.Text = "For SolidShineUI version " + (Assembly.GetAssembly(typeof(FlatWindow))?.GetName().Version?.ToString() ?? "unknown version");
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
