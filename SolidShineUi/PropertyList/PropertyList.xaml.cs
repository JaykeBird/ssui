using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Globalization;
using SolidShineUi.PropertyList.PropertyEditors;

namespace SolidShineUi.PropertyList
{
    /// <summary>
    /// Interaction logic for PropertyList.xaml
    /// </summary>
    public partial class ExperimentalPropertyList : UserControl
    {
        public ExperimentalPropertyList()
        {
            InitializeComponent();
            PreregisterEditors();
        }

        #region Basics / Object Loading

        private List<PropertyInfo> properties = new List<PropertyInfo>();
#if NETCOREAPP
        private object? _baseObject = null;
#else
        private object _baseObject = null;
#endif

        public void LoadObject(object o)
        {
            _baseObject = o;
            Type type = o.GetType();
            var nameProp = type.GetProperty("Name");

            if (nameProp != null)
            {
                txtName.Text = (nameProp.GetValue(o) ?? "No name").ToString();
            }
            else
            {
                txtName.Text = "No name";
            }
            txtType.Text = "Type: " + type.Name; // name of the type (not the object)

            // load all properties
            properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            SortList();

            LoadPropertyList(properties);
        }

        void LoadPropertyList(IEnumerable<PropertyInfo> properties)
        {
            stkProperties.Children.Clear();

            foreach (PropertyInfo item in properties)
            {
                PropertyEditorItem pei = new PropertyEditorItem();
#if NETCOREAPP
                IPropertyEditor? ipe = null;
#else
                IPropertyEditor ipe = null;
#endif
                if (registeredEditors.ContainsKey(item.PropertyType))
                {
                    object o = Activator.CreateInstance(registeredEditors[item.PropertyType]) ?? new object();
                    if (o is IPropertyEditor i)
                    {
                        ipe = i;
                    }
                }
                pei.LoadProperty(item, item.GetValue(_baseObject), ipe);
                stkProperties.Children.Add(pei);
            }
        }

#endregion

#region Sort and Filter

        private PropertySortOption _sort = PropertySortOption.Name;

        public PropertySortOption SortOption { get => _sort; set { _sort = value; SortList(); } }

        void SortList()
        {
            switch (SortOption)
            {
                case PropertySortOption.Name:
                    properties.Sort(ComparePropertyNames);
                    break;
                case PropertySortOption.Category:
                    properties.Sort(ComparePropertyCategories);
                    break;
                default:
                    properties.Sort(ComparePropertyNames);
                    break;
            }
        }

        private IEnumerable<PropertyInfo> Filter(string filter)
        {
#if NETCOREAPP
            return properties.Where(p => p.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase) || (p.PropertyType.FullName ?? "").Contains(filter, StringComparison.InvariantCultureIgnoreCase));
#else
            return properties.Where(p => p.Name.Contains(filter) || (p.PropertyType.FullName ?? "").Contains(filter));
#endif
        }

        private static int ComparePropertyNames(PropertyInfo x, PropertyInfo y)
        {
            var comp = StringComparer.InvariantCulture;

            if (x == null || y == null)
            {
                // either one or both are null
                if (x == null && y == null) return 0;
                else if (x == null) return -1;
                else if (y == null) return 1;
                else return 0; // not quite sure how we got here lol
            }
            else
            {
                return comp.Compare(x.Name, y.Name);
            }
        }

        private static int ComparePropertyCategories(PropertyInfo x, PropertyInfo y)
        {
            var comp = StringComparer.InvariantCulture;

            if (x == null || y == null)
            {
                // either one or both are null
                if (x == null && y == null) return 0;
                else if (x == null) return -1;
                else if (y == null) return 1;
                else return 0; // not quite sure how we got here lol
            }
            else
            {
                string xCat = GetCategoryOfProperty(x);
                string yCat = GetCategoryOfProperty(y);

                if (xCat == "" || yCat == "")
                {
                    if (xCat == "" && yCat == "") return ComparePropertyNames(x, y); // if the result is "equal" (0), then compare names
                    else if (xCat == "") return 1;
                    else if (yCat == "") return -1;
                    else return 0; // not quite sure how we got here lol
                }
                else
                {
                    int res = comp.Compare(xCat, yCat);
                    if (res == 0) return ComparePropertyNames(x, y);
                    else return res;
                }
            }
        }

#endregion

        private static string GetCategoryOfProperty(PropertyInfo pi)
        {
            IEnumerable<CategoryAttribute> attributes = pi.GetCustomAttributes<CategoryAttribute>(true);
            
            if (attributes.Any())
            {
                return attributes.First().Category;
            }
            else
            {
                return "";
            }
        }

#region Sort and View menu
        private void btnName_Click(object sender, RoutedEventArgs e)
        {
            SortOption = PropertySortOption.Name;
            LoadPropertyList(properties);
        }

        private void btnCategory_Click(object sender, RoutedEventArgs e)
        {
            SortOption = PropertySortOption.Category;
            LoadPropertyList(properties);
        }

        private void mnuTypesCol_Click(object sender, RoutedEventArgs e)
        {
            if (mnuTypesCol.IsChecked)
            {
                // hide column
                colTypes.Width = new GridLength(0, GridUnitType.Pixel);
                mnuTypesCol.IsChecked = false;
            }
            else
            {
                // show column
                colTypes.Width = new GridLength(40, GridUnitType.Pixel);
                mnuTypesCol.IsChecked = true;
            }
        }
#endregion

#region Registered Editors
        private Dictionary<Type, Type> registeredEditors = new Dictionary<Type, Type>();

        public void RegisterEditor(Type type, Type editor)
        {
            if (!editor.GetInterfaces().Contains(typeof(IPropertyEditor)))
            {
                throw new ArgumentException("The editor must inherit the IPropertyEditor interface.", nameof(editor));
            }

            if (registeredEditors.ContainsKey(type))
            {
                registeredEditors[type] = editor;
            }
            else
            {
                registeredEditors.Add(type, editor);
            }
        }

        private void PreregisterEditors()
        {
            RegisterEditor(typeof(bool), typeof(BooleanEditor));
            RegisterEditor(typeof(bool?), typeof(BooleanEditor));
            RegisterEditor(typeof(Nullable<bool>), typeof(BooleanEditor));
            RegisterEditor(typeof(string), typeof(StringEditor));
            RegisterEditor(typeof(double), typeof(DoubleEditor));
            RegisterEditor(typeof(float), typeof(DoubleEditor));
            RegisterEditor(typeof(int), typeof(IntegerEditor));
            RegisterEditor(typeof(short), typeof(IntegerEditor));
            RegisterEditor(typeof(ushort), typeof(IntegerEditor));
            RegisterEditor(typeof(byte), typeof(IntegerEditor));
            RegisterEditor(typeof(sbyte), typeof(IntegerEditor));
        }

#endregion
    }

    public enum PropertySortOption { Name = 0, Category = 1 }
}
