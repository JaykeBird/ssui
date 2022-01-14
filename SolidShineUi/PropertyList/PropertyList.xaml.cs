using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Globalization;

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
                pei.LoadProperty(item, item.GetValue(_baseObject));
                stkProperties.Children.Add(pei);
            }
        }

        #endregion

        #region Sort and Filter

        private PropertySortOption _sort = PropertySortOption.Category;

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
                return comp.Compare(GetCategoryOfProperty(x), GetCategoryOfProperty(y));
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
    }

    public enum PropertySortOption { Name = 0, Category = 1 }
}
