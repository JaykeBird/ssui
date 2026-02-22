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

            // load in string values
            lblViewEdit.Text = Strings.ViewEdit;
            mnuEdit.Header = Strings.ViewEditList;
            mnuEmptyList.Header = Strings.SetToNewEmptyList;
        }

        /// <inheritdoc/>
        public List<Type> ValidTypes => new List<Type> { typeof(List<>) };

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        bool _writable = true;

        /// <inheritdoc/>
        public bool IsPropertyWritable { get => _writable; set { _writable = value; mnuEmptyList.IsEnabled = value; } }

        /// <inheritdoc/>
        public void SetHostControl(IPropertyEditorHost host) { _parent = host; }

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
        IList? listVal;

        IPropertyEditorHost? _parent = null;

        /// <inheritdoc/>
        public event EventHandler? ValueChanged;

        /// <inheritdoc/>
        public object? GetValue()
        {
            return listVal;
        }
#else
        IList listVal;
        
        IPropertyEditorHost _parent = null;
        
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
                    addmi = type.GetMethods().FirstOrDefault((mi) => mi.Name == "Add" && mi.GetParameters().Length == 1);
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
                contentsData = Strings.Null;
            }
            else
            {
                contentsData = listVal.Count + " " + Strings.Items;
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
                led.SsuiTheme = _parent?.GetThemeForDialogs() ?? SsuiThemes.SystemTheme;
                led.Owner = _parent?.GetWindow();
                led.LoadEnumerable(listVal, _listType, propEditorType);
                led.Description = $"collection of {_listType.Name} ({listVal.Count} {Strings.Items}), in a {listVal.GetType().Name.Replace("`1", "")}:";

                led.ShowDialog();
            }
        }

        private void mnuEmptyList_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog md = new MessageDialog();
            md.SsuiTheme = _parent?.GetThemeForDialogs() ?? SsuiThemes.SystemTheme;
            md.ShowDialog(Strings.AreYouSureNewEmptyList, null, _parent?.GetWindow(), Strings.ConfirmEmptyList, 
                image: MessageDialogImage.Warning, buttonDisplay: MessageDialogButtonDisplay.Two, okButtonText: "Yes", cancelButtonText: "No");

            if (md.DialogResult != MessageDialogResult.OK) return;

            // TODO: look into exceptions to handle
            var newList = Activator.CreateInstance(typeof(List<>).MakeGenericType(_listType));

            if (newList != null && newList is IList il)
            {
                listVal = il;
                RenderListDataText();
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
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
