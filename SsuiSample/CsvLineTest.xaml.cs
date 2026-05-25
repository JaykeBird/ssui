using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SolidShineUi;
using SolidShineUi.Utils;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for CsvLineTest.xaml
    /// </summary>
    public partial class CsvLineTest : ThemedUserControl
    {
        public CsvLineTest()
        {
            InitializeComponent();
        }

        private void btnAddFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV Files|*.csv|All Files|*.*";
            ofd.Title = "Import CSV File";
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    string[] lines = System.IO.File.ReadAllLines(ofd.FileName);
                    foreach (string line in lines)
                    {
                        AddLine(line);
                    }
                }
                catch (Exception ex)
                {
                    txtResults.Text = "Error reading file: " + ex.Message;
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLine.Text))
            {
                AddLine(txtLine.Text);
                txtLine.Clear();
            }
        }

        private void txtLine_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (!string.IsNullOrEmpty(txtLine.Text))
                {
                    AddLine(txtLine.Text);
                    txtLine.Clear();
                }
            }
        }

        void AddLine(string line)
        {
            if (CsvLineHelper.CsvLineToArray(line, out string[] fields))
            {
                // Successfully parsed the line into fields
                // You can do something with the fields here, e.g. add them to a list
                var sel = new SelectableItem(string.Join(",", fields));
                sel.RightText = fields.Length.ToString();
                sel.ToolTip = string.Join("\n", fields);

                selList.Items.Add(sel);
            }
            else
            {
                txtResults.Text = "Invalid CSV line: " + line;
                // MessageBox.Show("Invalid CSV line: " + line, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void btnDeselect_Click(object sender, RoutedEventArgs e)
        {
            selList.Items.ClearSelection();
        }
    }
}
