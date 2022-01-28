using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using SolidShineUi;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// Interaction logic for BooleanEditor.xaml
    /// </summary>
    public partial class BooleanEditor : UserControl, IPropertyEditor
    {
        public BooleanEditor()
        {
            InitializeComponent();
        }

        public List<Type> ValidTypes => (new[] { typeof(bool), typeof(Nullable<bool>) }).ToList();

        public bool EditorAllowsModifying => true;

        public ColorScheme ColorScheme { set { chkValue.ColorScheme = value; } }

        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        private Type _propType = typeof(bool);

        public bool IsPropertyWritable
        {
            get => chkValue.IsEnabled;
            set => chkValue.IsEnabled = value;
        }


#if NETCOREAPP
        public event EventHandler? ValueChanged;

        public object? GetValue()
        {
            if (_propType == typeof(bool))
            {
                return chkValue.IsChecked;
            }
            else if (_propType == typeof(Nullable<bool>) || _propType == typeof(bool?))
            {
                return chkValue.CheckState switch
                {
                    CheckState.Checked => true,
                    CheckState.Unchecked => false,
                    CheckState.Indeterminate => null,
                    _ => null,
                };
            }
            else
            {
                return null;
            }
        }

        public void LoadValue(object? value, Type type)
        {
            if (type == typeof(bool))
            {
                _propType = type;
                chkValue.IsChecked = (bool)(value ?? false);
            }
            else if (type == typeof(Nullable<bool>) || type == typeof(bool?))
            {
                _propType = type;

                chkValue.TriStateClick = true;
                var val = (bool?)value;
                if (val == null)
                {
                    chkValue.CheckState = CheckState.Indeterminate;
                    txtValue.Text = "(null)";
                }
                else
                {
                    chkValue.CheckState = val.Value ? CheckState.Checked : CheckState.Unchecked;
                    txtValue.Text = val.Value ? "True" : "False";
                }
            }
            else
            {
                // this is not a boolean
                // this shouldn't be encountered, but if this is, just do nothing
            }
        }
#else
        public event EventHandler ValueChanged;

        public object GetValue()
        {
            if (_propType == typeof(bool))
            {
                return chkValue.IsChecked;
            }
            else if (_propType == typeof(Nullable<bool>))
            {
                switch (chkValue.CheckState)
                {
                    case CheckState.Unchecked:
                        return false;
                    case CheckState.Checked:
                        return true;
                    case CheckState.Indeterminate:
                        return null;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void LoadValue(object value, Type type)
        {
            if (type == typeof(bool))
            {
                chkValue.IsChecked = (bool)value;
            }
            else if (type == typeof(Nullable<bool>))
            {
                chkValue.TriStateClick = true;
                var val = (Nullable<bool>)value;
                if (val == null)
                {
                    chkValue.CheckState = CheckState.Indeterminate;
                }
                else
                {
                    chkValue.CheckState = val.Value ? CheckState.Checked : CheckState.Unchecked;
                }
            }
        }
#endif

        private void chkValue_CheckChanged(object sender, RoutedEventArgs e)
        {
            switch (chkValue.CheckState)
            {
                case CheckState.Unchecked:
                    txtValue.Text = "False";
                    break;
                case CheckState.Checked:
                    txtValue.Text = "True";
                    break;
                case CheckState.Indeterminate:
                    txtValue.Text = "(null)";
                    break;
                default:
                    txtValue.Text = "(null)";
                    break;
            }
            ValueChanged?.Invoke(sender, e);
        }
    }
}
