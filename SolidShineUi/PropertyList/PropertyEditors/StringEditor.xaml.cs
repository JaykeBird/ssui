using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using static SolidShineUi.Utils.IconLoader;
using SolidShineUi.PropertyList.Dialogs;
using SolidShineUi.Utils;

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

        public bool EditorAllowsModifying => true;

        public ExperimentalPropertyList ParentPropertyList { set { _parent = value; } }

        ColorScheme _cs = new ColorScheme();
        ExperimentalPropertyList _parent = new ExperimentalPropertyList();

        public ColorScheme ColorScheme
        {
            set
            {
                _cs = value;
                btnMenu.ColorScheme = value;
                imgMenu.Source = LoadIcon("ThreeDots", value);
            }
        }

        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }
        public bool IsPropertyWritable
        {
            get => btnMenu.IsEnabled;
            set 
            { 
                btnMenu.IsEnabled = value;
                txtText.IsEnabled = value && !setAsNull;
            }
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
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

#if NETCOREAPP
        public event EventHandler? ValueChanged;

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
                    return txtText.Text.ToString();
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
        public event EventHandler ValueChanged;

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
                    return txtText.Text.ToString();
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

        private void txtText_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        private void mnuOpenAsList_Click(object sender, RoutedEventArgs e)
        {
            OpenAsList();
        }

        private void mnuMultiline_Click(object sender, RoutedEventArgs e)
        {
            MultilineStringInputDialog sid = new MultilineStringInputDialog(_cs, "String Multi-Line Editor", "Enter a value:", txtText.Text);
            sid.Owner = Window.GetWindow(this);
            sid.ShowDialog();

            if (sid.DialogResult)
            {
                txtText.Text = sid.Value;
            }
        }

        /// <summary>
        /// Open the ListEditorDialog, with the contents being the list of this property.
        /// </summary>
        public void OpenAsList()
        {
            if (txtText.Text != null)
            {
#if NETCOREAPP
                IPropertyEditor? ipe = _parent?.CreateEditorForType(typeof(char));
                Type? propEditorType = null;
#else
                IPropertyEditor ipe = _parent?.CreateEditorForType(typeof(char));
                Type propEditorType = null;
#endif

                if (ipe != null)
                {
                    propEditorType = ipe.GetType();
                }

                ListEditorDialog led = new ListEditorDialog();
                led.Owner = Window.GetWindow(this);
                led.ColorScheme = _cs;
                led.LoadEnumerable(txtText.Text, typeof(char), propEditorType);
                led.Description = "collection string, of type char, with " + txtText.Text.Count() + " items:";

                led.ShowDialog();
            }
        }
    }
}
