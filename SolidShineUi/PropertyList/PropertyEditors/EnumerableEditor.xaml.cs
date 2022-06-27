using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SolidShineUi;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// An editor for viewing and editing lists
    /// </summary>
    public partial class EnumerableEditor : UserControl, IPropertyEditor
    {
        public EnumerableEditor()
        {
            InitializeComponent();
        }

        // TODO: add section to load in the propertyeditor type needed for child items in the list

        public List<Type> ValidTypes => new List<Type> { typeof(IEnumerable<>) };

        public bool EditorAllowsModifying => true;

        public bool IsPropertyWritable { get => btnMenu.IsEnabled; set => btnMenu.IsEnabled = value; }

        public ColorScheme ColorScheme {
            set
            {
                btnMenu.ColorScheme = value;
                imgMenu.Source = Utils.IconLoader.LoadIcon("ThreeDots", value);
            }
        }

        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

#if NETCOREAPP
        object? listVal;

        public event EventHandler? ValueChanged;

        public object? GetValue()
        {
            return listVal;
        }

        public void LoadValue(object? value, Type type)
        {
            string contentsData = "";

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                Type listType = type.GenericTypeArguments[0];

                listVal = value;
                if (listVal == null)
                {
                    // null value
                    contentsData = "(null)";
                }
                else
                {
                    IEnumerable lval = (IEnumerable)listVal;

                    if (lval == null)
                    {
                        // this is not a list!
                    }
                    else
                    {
                        contentsData = "collection of";
                    }
                }

                txtListData.Text = contentsData + " (" + listType.Name + ")";
            }
            else
            {
                // type is not a List
            }
        }
#else
        object listVal;

        public event EventHandler ValueChanged;

        public object GetValue()
        {
            return listVal;
        }

        public void LoadValue(object value, Type type)
        {
            string contentsData = "";

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type listType = type.GenericTypeArguments[0];

                listVal = value;
                if (listVal == null)
                {
                    // null value
                    contentsData = "(null)";
                }
                else
                {
                    IEnumerable lval = (IEnumerable)listVal;

                    if (lval == null)
                    {
                        // this is not a list!
                    }
                    else
                    {
                        contentsData = "collection of";
                    }
                }

                txtListData.Text = contentsData + " (" + type.GenericTypeArguments[0].Name + ")";
            }
            else
            {
                // type is not a List
            }
        }

#endif

        private void mnuEmptyList_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
