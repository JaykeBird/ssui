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
        public ColorScheme ColorScheme
        {
            set
            {
                ApplyColorScheme(value);
            }
        }

        /// <inheritdoc/>
        public void ApplyColorScheme(ColorScheme cs)
        {
            _cs = cs;
            btnMenu.ColorScheme = cs;
            imgMenu.Source = Utils.IconLoader.LoadIcon("ThreeDots", cs);
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

#endif

        /// <inheritdoc/>
#if NETCOREAPP
        public void LoadValue(object? value, Type type)
#else
        public void LoadValue(object value, Type type)
#endif
        {

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type listType = type.GenericTypeArguments[0];
                _listType = listType;

#if NETCOREAPP
                listVal = (IList?)value;
#else
                listVal = (IList)value;
#endif
                RenderListDataText();
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                // this is an IList, but isn't a generic type (i.e. not List<T>)

#if NETCOREAPP
                listVal = (IList?)value;

                // let's try to determine the list type by finding the Add method
                MethodInfo? addmi = null;
#else
                listVal = (IList)value;

                // let's try to determine the list type by finding the Add method
                MethodInfo addmi = null;
#endif

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
                RenderListDataText();

            }
            else
            {
                // type is not a List
            }
        }

        void RenderListDataText()
        {
            string contentsData;

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

        /// <summary>
        /// Open the ListEditorDialog, with the contents being the list of this property.
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
                led.Description = $"collection of {_listType.Name} ({listVal.Count} items), stored in a {listVal.GetType().Name.Replace("`1", "")}:";

                led.ShowDialog();
            }
        }

        private void mnuEmptyList_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            OpenListDialog();
            ValueChanged?.Invoke(this, EventArgs.Empty); // not a guarantee that the value actually changed, but at current, I don't have a way to detect if changes actually occurred

            // anyway, let's set up the UI
            RenderListDataText();
        }
    }
}
