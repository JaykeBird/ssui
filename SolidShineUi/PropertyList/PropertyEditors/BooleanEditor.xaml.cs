using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using SolidShineUi;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A property editor for editing <see cref="bool"/> objects.
    /// </summary>
    public partial class BooleanEditor : UserControl, IPropertyEditor
    {
        /// <summary>
        /// Create a new BooleanEditor.
        /// </summary>
        public BooleanEditor()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public List<Type> ValidTypes => (new[] { typeof(bool), typeof(Nullable<bool>) }).ToList();

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public void ApplySsuiTheme(SsuiTheme theme)
        {
            chkValue.SsuiTheme = theme;
        }

        /// <inheritdoc/>
        public void SetHostControl(IPropertyEditorHost host) { }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        private Type _propType = typeof(bool);

        /// <inheritdoc/>
        public bool IsPropertyWritable
        {
            get => chkValue.IsEnabled;
            set => chkValue.IsEnabled = value;
        }


#if NETCOREAPP
        /// <inheritdoc/>
        public event EventHandler? ValueChanged;
#else
        /// <inheritdoc/>
        public event EventHandler ValueChanged;
#endif


#if NETCOREAPP
        /// <inheritdoc/>
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
#else
        /// <inheritdoc/>
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

#endif

#if NETCOREAPP
        /// <inheritdoc/>
        public void LoadValue(object? value, Type type)
#else
        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
#endif
        {
            if (type == typeof(bool))
            {
                _propType = type;
                chkValue.IsChecked = (bool) (value ?? false);
            }
            else if (type == typeof(Nullable<bool>) || type == typeof(bool?))
            {
                _propType = type;

                chkValue.TriStateClick = true;
                var val = (bool?)value;
                if (val == null)
                {
                    chkValue.CheckState = CheckState.Indeterminate;
                    // txtValue.Text = "(null)";
                }
                else
                {
                    chkValue.CheckState = val.Value ? CheckState.Checked : CheckState.Unchecked;
                    // txtValue.Text = val.Value ? "True" : "False";
                }
            }
            else
            {
                // this is not a boolean
                // this shouldn't be encountered, but if this is, just do nothing
            }
        }

        private void chkValue_CheckChanged(object sender, RoutedEventArgs e)
        {
            switch (chkValue.CheckState)
            {
                case CheckState.Unchecked:
                    txtValue.Text = Strings.False;
                    break;
                case CheckState.Checked:
                    txtValue.Text = Strings.True;
                    break;
                case CheckState.Indeterminate:
                    txtValue.Text = Strings.Null;
                    break;
                default:
                    txtValue.Text = Strings.Null;
                    break;
            }
            ValueChanged?.Invoke(sender, e);
        }
    }
}
