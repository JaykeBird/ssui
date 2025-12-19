using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using static SolidShineUi.Utils.IconLoader;
using SolidShineUi.PropertyList.Dialogs;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A proeprty editor for editing <see cref="string"/> objects.
    /// </summary>
    public partial class StringEditor : UserControl, IPropertyEditor
    {
        /// <summary>Create a StringEditor.</summary>
        public StringEditor()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public void SetHostControl(IPropertyEditorHost host) { _parent = host; }

#if NETCOREAPP
        IPropertyEditorHost? _parent = null;
#else
        IPropertyEditorHost _parent = null;
#endif

        /// <inheritdoc/>
        public void ApplySsuiTheme(SsuiTheme cs)
        {
            btnMenu.SsuiTheme = cs;
            imgMenu.Source = LoadIcon("ThreeDots", cs.IconVariation);
        }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
        /// <inheritdoc/>
        public event EventHandler? ValueChanged;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void LoadValue(object? value, Type type)
        {
            _itemType = type;
            if (type == typeof(string))
            {
                txtText.Text = (value ?? "").ToString();
            }
        }
#else
        /// <inheritdoc/>
        public event EventHandler ValueChanged;
        
        /// <inheritdoc/>
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
        
        /// <inheritdoc/>
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
            MultilineStringInputDialog sid = new MultilineStringInputDialog("String Multi-Line Editor", "Enter a value:", txtText.Text);
            sid.Owner = _parent?.GetWindow();
            sid.SsuiTheme = _parent?.GetThemeForDialogs() ?? SsuiThemes.SystemTheme;
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
            // I was hopeful this feature could be used to, say, allow users to edit a string character by character
            // however, my current ListEditorDialog has limitations when loading an IEnumerable (and not an IList), which a string is
            // thus, it doesn't really seem worth it for me to enable this feature

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
                led.Owner = _parent?.GetWindow();
                led.SsuiTheme = _parent?.GetThemeForDialogs() ?? SsuiThemes.SystemTheme;
                led.LoadEnumerable(txtText.Text, typeof(char), propEditorType);
                led.Description = "collection string, of type char, with " + txtText.Text.Length + " items:";

                led.ShowDialog();
            }
        }
    }
}
