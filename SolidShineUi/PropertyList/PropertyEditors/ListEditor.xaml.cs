using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using SolidShineUi;
using SolidShineUi.PropertyList.Dialogs;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// An editor for viewing and editing lists
    /// </summary>
    public partial class ListEditor : UserControl, IPropertyEditor
    {
        /// <summary>
        /// Create a ListEditor.
        /// </summary>
        public ListEditor()
        {
            InitializeComponent();
        }

        // TODO: add section to load in the propertyeditor type needed for child items in the list

        /// <inheritdoc/>
        public List<Type> ValidTypes => new List<Type> { typeof(List<>) };

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        bool _writable = true;

        /// <inheritdoc/>
        public bool IsPropertyWritable { get => _writable; set { _writable = value; mnuEmptyGuid.IsEnabled = value; } }

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
        IList? listVal;

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

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type listType = type.GenericTypeArguments[0];
                _listType = listType;

                listVal = (IList?)value;
                if (listVal == null)
                {
                    // null value
                    contentsData = "(null)";
                }
                else
                {
                    contentsData = listVal.Count + "items";
                }

                txtListData.Text = contentsData + " (" + listType.Name + ")";
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                // this is an IList, but isn't a generic type (i.e. not List<T>)
                listVal = (IList?)value;

                // let's try to determine the list type by finding the Add method

                MethodInfo? addmi = null;

                try
                {
                    addmi = type.GetMethod("Add");
                }
                catch (AmbiguousMatchException)
                {
                    // there is more than one Add method
                    // let's try to find the one that only has 1 parameter
                    addmi = type.GetMethods().Where((mi) => mi.Name == "Add" && mi.GetParameters().Length == 1).FirstOrDefault();
                }

                if (addmi != null)
                {
                    var pi = addmi.GetParameters();
                    if (pi.Length == 1)
                    {
                        _listType = pi[0].ParameterType;
                    }
                }
                else
                {
                    // type is a List, but doesn't have a proper Add method
                    // how does that happen? lol
                }

                // anyway, let's set up the UI
                if (listVal == null)
                {
                    // null value
                    contentsData = "(null)";
                }
                else
                {
                    contentsData = listVal.Count + " items";
                }

                txtListData.Text = contentsData + " (" + _listType.Name + ")";
            }
            else
            {
                // type is not a List
            }
        }
#else
        IList listVal;
        
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
                
                listVal = (IList)value;
                if (listVal == null)
                {
                    // null value
                    contentsData = "(null)";
                }
                else
                {
                    contentsData = listVal.Count + "items";
                }

                txtListData.Text = contentsData + " (" + type.GenericTypeArguments[0].Name + ")";
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                // this is an IList, but isn't a generic type (i.e. not List<T>)
                listVal = (IList)value;

                // let's try to determine the list type by finding the Add method

                MethodInfo addmi = null;

                try
                {
                    addmi = type.GetMethod("Add");
                }
                catch (AmbiguousMatchException)
                {
                    // there is more than one Add method
                    // let's try to find the one that only has 1 parameter
                    addmi = type.GetMethods().Where((mi) => mi.Name == "Add" && mi.GetParameters().Length == 1).FirstOrDefault();
                }

                if (addmi != null)
                {
                    var pi = addmi.GetParameters();
                    if (pi.Length == 1)
                    {
                        _listType = pi[0].ParameterType;
                    }
                }
                else
                {
                    // type is a List, but doesn't have a proper Add method
                    // how does that happen? lol
                }

                // anyway, let's set up the UI
                if (listVal == null)
                {
                    // null value
                    contentsData = "(null)";
                }
                else
                {
                    contentsData = listVal.Count + " items";
                }

                txtListData.Text = contentsData + " (" + _listType.Name + ")";
            }
            else
            {
                // type is not a List
            }
        }

#endif

        void OpenListDialog()
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
                led.Description = "collection " + listVal.GetType().Name.Replace("`1", "") + ", of type " + _listType.Name + ", with " + listVal.Count + " items:";

                led.ShowDialog();
            }
        }

        private void mnuEmptyList_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            OpenListDialog();
        }
    }
}
