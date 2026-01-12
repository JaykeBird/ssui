using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SolidShineUi;
using SolidShineUi.PropertyList.Dialogs;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// An editor for viewing and editing lists
    /// </summary>
    public partial class EnumerableEditor : UserControl, IPropertyEditor
    {
        /// <summary>
        /// Create an EnumerableEditor.
        /// </summary>
        public EnumerableEditor()
        {
            InitializeComponent();
        }

        // TODO: add section to load in the propertyeditor type needed for child items in the list

        /// <inheritdoc/>
        public List<Type> ValidTypes => new List<Type> { typeof(IEnumerable<>) };

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        bool _writable = true;

        /// <inheritdoc/>
        public bool IsPropertyWritable { get => _writable; set => _writable = value; }

        /// <inheritdoc/>
        public void SetHostControl(IPropertyEditorHost host) { _host = host; }

        /// <inheritdoc/>
        public void ApplySsuiTheme(SsuiTheme theme)
        {
            btnMenu.SsuiTheme = theme;
            imgMenu.Source = Utils.IconLoader.LoadIcon("ThreeDots", theme.IconVariation);
        }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        Type _listType = typeof(object);

#if NETCOREAPP
        IEnumerable? listVal;

        IPropertyEditorHost? _host = null;

        /// <inheritdoc/>
        public event EventHandler? ValueChanged;

        /// <inheritdoc/>
        public object? GetValue()
        {
            return listVal;
        }

        /// <inheritdoc/>
        public void LoadValue(object? value, Type type)
        {
            if (type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type))
            {
                Type listType = type.GenericTypeArguments[0];
                _listType = listType;

                listVal = (IEnumerable?)value;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                _listType = typeof(object);

                // this is an IEnumerable, just not a generic type (IEnumerable<T>)
                listVal = (IEnumerable?)value;
            }
            else
            {
                // type is not a IEnumerable
            }

            RenderListDataText(listVal, type);
        }
#else
        IEnumerable listVal;
        
        IPropertyEditorHost _host = null;
        
        /// <inheritdoc/>
        public event EventHandler ValueChanged;
        
        /// <inheritdoc/>
        public object GetValue()
        {
            return listVal;
        }
        
        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
        {
            if (type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type))
            {
                Type listType = type.GenericTypeArguments[0];
                _listType = listType;

                listVal = (IEnumerable)value;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                _listType = typeof(object);

                // this is an IEnumerable, just not a generic type (IEnumerable<T>)
                listVal = (IEnumerable)value;
            }
            else
            {
                // type is not a IEnumerable
            }

            RenderListDataText(listVal, type);
        }

#endif

        /// <summary>
        /// Open the ListEditorDialog, with its contents being the list or collection of this property.
        /// </summary>
        public void OpenListDialog()
        {
            if (listVal != null)
            {
#if NETCOREAPP
                IPropertyEditor? ipe = _host?.CreateEditorForType(_listType);
                Type? propEditorType = null;
#else
                IPropertyEditor ipe = _host?.CreateEditorForType(_listType);
                Type propEditorType = null;
#endif

                if (ipe != null)
                {
                    propEditorType = ipe.GetType();
                }

                ListEditorDialog led = new ListEditorDialog();
                led.SsuiTheme = _host?.GetThemeForDialogs() ?? SsuiThemes.SystemTheme;
                led.LoadEnumerable(listVal, _listType, propEditorType);
                if (listVal is ICollection icol)
                {
                    led.Description = $"collection of {_listType.Name} ({icol.Count} items), stored in a {listVal.GetType().Name.Replace("`1", "")}:";
                }
                else
                {
                    led.Description = $"collection of {_listType.Name}, stored in a {listVal.GetType().Name.Replace("`1", "")}:";
                }

                led.Owner = Window.GetWindow(this);
                led.ShowDialog();
            }
            else
            {
                MessageBox.Show("No value is set for this property (it is set to null).");
            }
        }

        private void mnuEmptyList_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            OpenListDialog();
            ValueChanged?.Invoke(this, EventArgs.Empty); // not a guarantee that the value actually changed, but at current, I don't have a way to detect if changes actually occurred

            Type type = listVal?.GetType() ?? typeof(object);
            RenderListDataText(listVal, type);
        }

#if NETCOREAPP
        void RenderListDataText(object? value, Type type)
#else
        void RenderListDataText(object value, Type type)
#endif
        {

            if (type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type))
            {
                string contentsData;// = "()";

                if (value == null)
                {
                    // null value
                    contentsData = "(null)";
                }
                else
                {
                    if (listVal is ICollection icol)
                    {
                        contentsData = icol.Count + " items";
                    }
                    else
                    {
                        contentsData = "collection";
                    }
                }

                txtListData.Text = contentsData + " (" + _listType.Name + ")";
            }
            else if (typeof(ICollection).IsAssignableFrom(type))
            {
                if (value == null)
                {
                    txtListData.Text = "(null)";
                }
                else
                {
                    txtListData.Text = (((ICollection)value)?.Count ?? 0) + " items in " + type.Name;
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                // this is an IEnumerable, just not a generic type (IEnumerable<T>)
                txtListData.Text = "collection in " + type.Name;
            }
            else
            {
                // type is not a IEnumerable
            }
        }
    }
}
