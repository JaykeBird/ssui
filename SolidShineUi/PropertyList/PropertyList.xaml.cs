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
using System.Collections.ObjectModel;
using System.Windows.Media;

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


        #region ColorScheme

#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

        public static DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ExperimentalPropertyList w)
            {
                w.ColorSchemeChanged?.Invoke(d, e);
#if NETCOREAPP
                w.ApplyColorScheme((e.NewValue as ColorScheme)!);
#else
                w.ApplyColorScheme((e.NewValue as ColorScheme));
#endif
            }
        }

        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        public void ApplyColorScheme(ColorScheme cs)
        {
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }
        }

#endregion

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
                if (!item.CanWrite)
                {
                    // readonly property
                }

                Type propType = item.PropertyType;
                // let's do some tests on the properties
                if (propType.IsEnum)
                {
                    // load enum editor
                    object o = Activator.CreateInstance(registeredEditors[typeof(Enum)] ?? typeof(EnumEditor)) ?? new object();
                    if (o is IPropertyEditor i)
                    {
                        i.ColorScheme = ColorScheme;
                        ipe = i;
                    }
                }
                else if (item.PropertyType.IsGenericType && item.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type itemType = item.PropertyType.GetGenericArguments()[0];
                    // TODO: add handling for list items
                }
                else
                {
                    if (registeredEditors.ContainsKey(propType))
                    {
                        object o = Activator.CreateInstance(registeredEditors[item.PropertyType]) ?? new object();
                        if (o is IPropertyEditor i)
                        {
                            i.ColorScheme = ColorScheme;
                            ipe = i;
                        }
                    }
                }
                    
                pei.LoadProperty(item, item.GetValue(_baseObject), ipe);
                pei.PropertyEditorValueChanged += editor_PropertyEditorValueChanged;
                stkProperties.Children.Add(pei);
            }
        }

#if NETCOREAPP
        private void editor_PropertyEditorValueChanged(object? sender, PropertyEditorValueChangedEventArgs e)
#else
        private void editor_PropertyEditorValueChanged(object sender, PropertyEditorValueChangedEventArgs e)
#endif
        {
            if (e.PropertyInfo != null)
            {
                try
                {
                    e.PropertyInfo.SetValue(_baseObject, e.NewValue);
                }
                catch (ArgumentException)
                {
                    // property doesn't have a setter, probably

                    e.ChangeFailed = true;
                    e.FailedChangePropertyValue = e.PropertyInfo.GetValue(_baseObject);
                }
                catch (TargetInvocationException)
                {
                    // tried to set property, but some exception occurred
                    // use the InnerException to learn more as to what the issue was

                    e.ChangeFailed = true;
                    e.FailedChangePropertyValue = e.PropertyInfo.GetValue(_baseObject);
                }
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

        public ReadOnlyDictionary<Type, Type> RegisteredPropertyEditors { get => new ReadOnlyDictionary<Type, Type>(registeredEditors); }

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
            RegisterEditor(typeof(Enum), typeof(EnumEditor));
            RegisterEditor(typeof(Brush), typeof(BrushEditor));
            RegisterEditor(typeof(SolidColorBrush), typeof(BrushEditor));
            RegisterEditor(typeof(LinearGradientBrush), typeof(BrushEditor));
            RegisterEditor(typeof(RadialGradientBrush), typeof(BrushEditor));
            RegisterEditor(typeof(ImageBrush), typeof(BrushEditor));
            RegisterEditor(typeof(DrawingBrush), typeof(BrushEditor));
            RegisterEditor(typeof(BitmapCacheBrush), typeof(BrushEditor));
        }

#endregion
    }

    public enum PropertySortOption { Name = 0, Category = 1 }
}
