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
        public ExperimentalPropertyList ParentPropertyList { set { _parent = value; } }

        ColorScheme _cs = new ColorScheme();

        /// <inheritdoc/>
        public ColorScheme ColorScheme {
            set
            {
                _cs = value;
                btnMenu.ColorScheme = value;
                imgMenu.Source = Utils.IconLoader.LoadIcon("ThreeDots", value);
            }
        }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        Type _listType = typeof(object);

#if NETCOREAPP
        IEnumerable? listVal;

        ExperimentalPropertyList? _parent = null;

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
            string contentsData = "";

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                Type listType = type.GenericTypeArguments[0];
                _listType = listType;

                listVal = (IEnumerable?)value;
                if (listVal == null)
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

                txtListData.Text = contentsData + " (" + listType.Name + ")";
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                // this is an IEnumerable, just not a generic type (IEnumerable<T>)
                listVal = (IEnumerable?)value;

                txtListData.Text = "collection - " + type.Name;
            }
            else
            {
                // type is not a IEnumerable
            }
        }
#else
        IEnumerable listVal;
        
        ExperimentalPropertyList _parent = null;
        
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
            string contentsData = "";

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type listType = type.GenericTypeArguments[0];

                listVal = (IEnumerable)value;
                if (listVal == null)
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

                txtListData.Text = contentsData + " (" + type.GenericTypeArguments[0].Name + ")";
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                // this is an IEnumerable, just not a generic type (IEnumerable<T>)
                listVal = (IEnumerable)value;

                txtListData.Text = "collection - " + type.Name;
            }
            else
            {
                // type is not a List
            }
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
                IPropertyEditor? ipe = _parent?.CreateEditorForType(_listType);
                Type? propEditorType = null;
#else
                IPropertyEditor ipe = _parent?.CreateEditorForType(_listType);
                Type propEditorType = null;
#endif

                if (ipe != null)
                {
                    propEditorType = ipe.GetType();
                }

                ListEditorDialog led = new ListEditorDialog();
                led.ColorScheme = _cs;
                led.LoadEnumerable(listVal, _listType, propEditorType);
                led.Description = "collection " + listVal.GetType().Name.Replace("`1", "") + ", of type " + _listType.Name + ((listVal is ICollection icol) ? ", with " + icol.Count + " items:" : ":");

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

            Type type = listVal?.GetType() ?? typeof(object);
            string contentsData = "()";
            
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                Type listType = type.GenericTypeArguments[0];

                if (listVal == null)
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

                txtListData.Text = contentsData + " (" + listType.Name + ")";
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {

                txtListData.Text = "collection - " + type.Name;
            }
            else
            {
                // type is not a IEnumerable
            }
        }
    }
}
