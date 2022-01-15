using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// Interaction logic for StringEditor.xaml
    /// </summary>
    public partial class StringEditor : UserControl, IPropertyEditor
    {
        public StringEditor()
        {
            InitializeComponent();
        }


        public bool CanEdit => true;

        public ColorScheme ColorScheme { set => btnMenu.ColorScheme = value; }

        public UIElement GetUiElement()
        {
            return this;
        }

        bool setAsNull = false;

        private Type _itemType = typeof(string);
        public List<Type> ValidTypes => (new[] { typeof(string) }).ToList();

        private void mnuSetNull_Click(object sender, RoutedEventArgs e)
        {
            if (mnuSetNull.IsChecked)
            {
                // do not set as null
                setAsNull = false;
                txtText.IsEnabled = true;
                mnuSetNull.IsChecked = false;
            }
            else
            {
                // do set as null
                setAsNull = true;
                txtText.IsEnabled = false;
                mnuSetNull.IsChecked = true;
            }
        }

#if NETCOREAPP

        public object? GetValue()
        {
            if (_itemType == typeof(string))
            {
                if (setAsNull)
                {
                    return null;
                }
                else
                {
                    return txtText.ToString();
                }
            }
            else return null;
        }

        public void LoadValue(object? value, Type type)
        {
            _itemType = type;
            if (type == typeof(string))
            {
                txtText.Text = (value ?? "").ToString();
            }
        }
#else
        public object GetValue()
        {
            if (_itemType == typeof(string))
            {
                if (setAsNull)
                {
                    return null;
                }
                else
                {
                    return txtText.ToString();
                }
            }
            else return null;
        }

        public void LoadValue(object value, Type type)
        {
            _itemType = type;
            if (type == typeof(string))
            {
                txtText.Text = (value ?? "").ToString();
            }
        }
#endif


    }
}
