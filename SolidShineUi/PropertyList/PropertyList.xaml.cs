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
using System.Runtime.InteropServices;
using System.Text;

namespace SolidShineUi.PropertyList
{
    /// <summary>
    /// A control that can display the properties and values of a .NET object, with support for live editing of many of them.
    /// (Note that there are missing features and potential bugs.)
    /// </summary>
    public partial class ExperimentalPropertyList : UserControl
    {
        /// <summary>
        /// Create a PropertyList.
        /// </summary>
        public ExperimentalPropertyList()
        {
            InitializeComponent();
            PreregisterEditors();

            var colDescriptor = DependencyPropertyDescriptor.FromProperty(ColumnDefinition.WidthProperty, typeof(ColumnDefinition));
            colDescriptor.AddValueChanged(colNames, ColumnWidthChanged);
            colDescriptor.AddValueChanged(colTypes, ColumnWidthChanged);
            colDescriptor.AddValueChanged(colValues, ColumnWidthChanged);

            Clear();
        }


        #region ColorScheme

        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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

        /// <summary>
        /// Get or set the color scheme to apply to the control. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        /// <summary>
        /// Apply a color scheme to this control. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply.</param>
        public void ApplyColorScheme(ColorScheme cs)
        {
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }

#if NETCOREAPP
            foreach (UIElement? item in stkProperties.Children)
#else
            foreach (UIElement item in stkProperties.Children)
#endif
            {
                if (item == null) continue;
                if (item is PropertyEditorItem pei)
                {
                    if (pei.PropertyEditorControl != null)
                    {
                        pei.PropertyEditorControl.ColorScheme = cs;
                    }
                }
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

        /// <summary>
        /// Get the object that is currently being observed in this PropertyList.
        /// </summary>
#if NETCOREAPP
        public object? GetCurrentlyLoadedObject()
#else
        public object GetCurrentlyLoadedObject()
#endif
        {
            return _baseObject;
        }

        /// <summary>
        /// Reload the properties and values from the currently observed object.
        /// </summary>
        public void ReloadObject()
        {
            if (_baseObject != null)
            {
                LoadObject(_baseObject);
            }
        }

        /// <summary>
        /// Unload the currently observed object, so that nothing is observed.
        /// </summary>
        /// <remarks>
        /// This clears out the UI and resets internal properties; however, this does not need to be run before loading in another object.
        /// </remarks>
        public void Clear()
        {
            LoadObject(new object());
            _baseObject = null;
            ObjectDisplayName = "";
            txtType.Text = "Nothing loaded";
            txtType.ToolTip = "";

            btnRefresh.IsEnabled = false;
        }

        /// <summary>
        /// The string "No name", used for objects that don't have a Name property to get a name from.
        /// </summary>
        public static string NO_NAME = "No name";

        /// <summary>
        /// Set the object to observe. All properties of the observed object will be displayed in the ExperimentalPropertyList, alongside the values of these properties.
        /// </summary>
        /// <param name="o">The object to load and observe.</param>
        /// <remarks>
        /// Note that if the object has a property called "Name", that name will be displayed at the top of the ExperimentalPropertyList control.
        /// If this object doesn't have a Name property, or you want to set a different name, please use the <see cref="ObjectDisplayName"/> property.
        /// </remarks>
        public void LoadObject(object o)
        {
            _baseObject = o;
            Type type = o.GetType();
            var nameProp = type.GetProperty("Name");

            if (nameProp != null)
            {
                ObjectDisplayName = (nameProp.GetValue(o) ?? NO_NAME).ToString() ?? NO_NAME;
            }
            else
            {
                ObjectDisplayName = NO_NAME;
            }
            txtType.Text = "Type: " + type.Name; // name of the type (not the object)
            txtType.ToolTip = type.FullName;

            // load all properties
            properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            SortList();

            LoadPropertyList(properties);

            txtFilter.Text = "";
            _filterString = "";
            ShowInheritedProperties = true;
            mnuShowInherited.IsChecked = true;

            btnRefresh.IsEnabled = true;
        }

        #region Display Name
        /// <summary>
        /// Get or set the string used to name the object being observed. The PropertyList will try to set this automatically via looking at the Name property,
        /// or otherwise you can set a custom name to display.
        /// </summary>
        public string ObjectDisplayName { get => (string)GetValue(ObjectDisplayNameProperty); set => SetValue(ObjectDisplayNameProperty, value); }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static DependencyProperty ObjectDisplayNameProperty
            = DependencyProperty.Register("ObjectDisplayName", typeof(string), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata("No name"));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion


        void LoadPropertyList(IEnumerable<PropertyInfo> properties)
        {
            stkProperties.Children.Clear();

            Type baseType = _baseObject?.GetType() ?? typeof(object);

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

                if (item.DeclaringType != baseType)
                {
                    // item was inherited
                    pei.IsInherited = true;
                }
                pei.DeclaringType = item.DeclaringType;

                Type propType = item.PropertyType;
                // let's do some tests on the properties
                ipe = CreateEditorForType(propType);

                if (ipe != null)
                {
                    ipe.ColorScheme = ColorScheme;
                    ipe.ParentPropertyList = this;
                    ipe.IsPropertyWritable = item.CanWrite;
                }

                pei.LoadProperty(item, item.GetValue(_baseObject), ipe);
                pei.PropertyEditorValueChanged += editor_PropertyEditorValueChanged;
                pei.UpdateColumnWidths(colNames.Width, colTypes.Width, colValues.Width);
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

        private string _filterString = "";

        /// <summary>
        /// Get or set how the list of properties are sorted in this PropertyList.
        /// </summary>
        public PropertySortOption SortOption { get => _sort; set { _sort = value; SortList(); } }

        private bool _showInherited = true;

        /// <summary>
        /// Get or set if inherited properties (properties not defined directly in the observed object's type) are visible in the PropertyList.
        /// </summary>
        public bool ShowInheritedProperties { get => _showInherited; set { _showInherited = value; FilterProperties(_filterString); mnuShowInherited.IsChecked = value; } }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterProperties(txtFilter.Text);
        }

        #region Sort, Filter, Compare functions
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

        /// <summary>
        /// Change the text used to filter the list of properties. Applying a filter text will hide any properties that don't contain this text (or its type doesn't contain this text).
        /// </summary>
        /// <param name="filter">
        /// The filter text to apply. Use <c>null</c> or an empty string to not apply a filter. Start the string with "@" to only filter by property name only (not name or type).
        /// </param>
#if NETCOREAPP
        public void FilterProperties(string? filter)
#else
        public void FilterProperties(string filter)
#endif
        {
            if (stkProperties == null) return;

            if (string.IsNullOrEmpty(filter))
            {
                // no filter, display all
#if NETCOREAPP
                foreach (UIElement? item in stkProperties.Children)
#else
                foreach (UIElement item in stkProperties.Children)
#endif
                {
                    if (item == null) continue;
                    if (item is PropertyEditorItem pei)
                    {
                        if (pei != null)
                        {
                            pei.Visibility = Visibility.Visible;
                        }
                    }
                }
                _filterString = "";
            }
            else
            {
                IEnumerable<PropertyInfo> propInfos;
                if (filter.StartsWith("@"))
                {
                    propInfos = FilterNameOnly(filter.Substring(1));
                }
                else
                {
                    propInfos = Filter(filter);
                }

#if NETCOREAPP
                foreach (UIElement? item in stkProperties.Children)
#else
                foreach (UIElement item in stkProperties.Children)
#endif
                {
                    if (item == null) continue;
                    if (item is PropertyEditorItem pei)
                    {
                        if (pei != null)
                        {
                            if (propInfos.Contains(pei.PropertyInfo))
                            {
                                pei.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                pei.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                }
                _filterString = filter;
            }

            if (!_showInherited)
            {
#if NETCOREAPP
                foreach (UIElement? item in stkProperties.Children)
#else
                foreach (UIElement item in stkProperties.Children)
#endif
                {
                    if (item == null) continue;
                    if (item is PropertyEditorItem pei)
                    {
                        if (pei != null)
                        {
                            if (pei.IsInherited)
                            {
                                pei.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<PropertyInfo> FilterNameOnly(string filter)
        {
#if NETCOREAPP
            return properties.Where(p => p.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase));// || (p.PropertyType.FullName ?? "").Contains(filter, StringComparison.InvariantCultureIgnoreCase));
#else
            return properties.Where(p => p.Name.Contains(filter));// || (p.PropertyType.FullName ?? "").Contains(filter));
#endif
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
                splTypes.Visibility = Visibility.Collapsed;
            }
            else
            {
                // show column
                colTypes.Width = new GridLength(40, GridUnitType.Pixel);
                mnuTypesCol.IsChecked = true;
                splTypes.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region Registered Editors


        private Dictionary<Type, Type> registeredEditors = new Dictionary<Type, Type>();

        /// <summary>
        /// Get a list of editors currently registered for each property type.
        /// </summary>
        /// <remarks>
        /// The key of the dictionary is a property type. The corresponding value is the type of the editor that will handle viewing and editing that type.
        /// </remarks>
        public ReadOnlyDictionary<Type, Type> RegisteredPropertyEditors { get => new ReadOnlyDictionary<Type, Type>(registeredEditors); }

        /// <summary>
        /// Register an editor for a certain property type. When the control loads an object with a property of this type, the registered editor will be used to view and edit it.
        /// </summary>
        /// <param name="type">The type to register the <paramref name="editor"/> for.</param>
        /// <param name="editor">The type of the IPropertyEditor control that will handle viewing and editing <paramref name="type"/>.</param>
        /// <exception cref="ArgumentException">Thrown if the editor type does not inherit the IPropertyEditor interface.</exception>
        /// <remarks>If a different editor is already registered for a certain type, this will replace that registration and the control will use this editor instead.</remarks>
        public void RegisterEditor(Type type, Type editor)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type), "The property type cannot be null.");
            }

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

        /// <summary>
        /// Unregister the editor for a certain type.
        /// </summary>
        /// <param name="type">The property type to unregister the editor of. (For example, enter in <c>typeof(string)</c>, not <c>typeof(StringEditor)</c>).</param>
        /// <returns>Returns true on success. Returns false if the editor couldn't be unregistered, or there was no editor registered to begin with.</returns>
        public bool UnregisterEditor(Type type)
        {
            if (registeredEditors.ContainsKey(type))
            {
                return registeredEditors.Remove(type);
            }
            else return false;
        }

        private void PreregisterEditors()
        {
            RegisterEditor(typeof(bool), typeof(BooleanEditor));
            RegisterEditor(typeof(bool?), typeof(BooleanEditor));
            RegisterEditor(typeof(string), typeof(StringEditor));
            RegisterEditor(typeof(double), typeof(DoubleEditor));
            RegisterEditor(typeof(float), typeof(DoubleEditor));
            RegisterEditor(typeof(double?), typeof(DoubleEditor));
            RegisterEditor(typeof(float?), typeof(DoubleEditor));
#if NET5_0_OR_GREATER
            RegisterEditor(typeof(Half), typeof(DoubleEditor));
            RegisterEditor(typeof(Half?), typeof(DoubleEditor));
#endif
            RegisterEditor(typeof(int), typeof(IntegerEditor));
            RegisterEditor(typeof(short), typeof(IntegerEditor));
            RegisterEditor(typeof(ushort), typeof(IntegerEditor));
            RegisterEditor(typeof(byte), typeof(IntegerEditor));
            RegisterEditor(typeof(sbyte), typeof(IntegerEditor));
            RegisterEditor(typeof(int?), typeof(IntegerEditor));
            RegisterEditor(typeof(short?), typeof(IntegerEditor));
            RegisterEditor(typeof(ushort?), typeof(IntegerEditor));
            RegisterEditor(typeof(byte?), typeof(IntegerEditor));
            RegisterEditor(typeof(sbyte?), typeof(IntegerEditor));
            RegisterEditor(typeof(Enum), typeof(EnumEditor));
            RegisterEditor(typeof(Brush), typeof(BrushEditor));
            RegisterEditor(typeof(SolidColorBrush), typeof(BrushEditor));
            RegisterEditor(typeof(LinearGradientBrush), typeof(BrushEditor));
            RegisterEditor(typeof(RadialGradientBrush), typeof(BrushEditor));
            RegisterEditor(typeof(ImageBrush), typeof(BrushEditor));
            RegisterEditor(typeof(DrawingBrush), typeof(BrushEditor));
            RegisterEditor(typeof(GradientBrush), typeof(BrushEditor));
            RegisterEditor(typeof(TileBrush), typeof(BrushEditor));
            RegisterEditor(typeof(FontFamily), typeof(FontFamilyEditor));
            RegisterEditor(typeof(FontStyle), typeof(FontStyleEditor));
            RegisterEditor(typeof(FontWeight), typeof(FontWeightEditor));
            RegisterEditor(typeof(FontWeight?), typeof(FontWeightEditor));
            RegisterEditor(typeof(Uri), typeof(UriEditor));
            RegisterEditor(typeof(Guid), typeof(GuidEditor));
            RegisterEditor(typeof(Guid?), typeof(GuidEditor));
            RegisterEditor(typeof(Thickness), typeof(ThicknessEditor));
            RegisterEditor(typeof(Thickness?), typeof(ThicknessEditor));
            RegisterEditor(typeof(Size), typeof(SizeEditor));
            RegisterEditor(typeof(Size?), typeof(SizeEditor));
            RegisterEditor(typeof(Point), typeof(PointEditor));
            RegisterEditor(typeof(Point?), typeof(PointEditor));
            RegisterEditor(typeof(List<>), typeof(ListEditor));
            RegisterEditor(typeof(IEnumerable<>), typeof(EnumerableEditor));
            RegisterEditor(typeof(Version), typeof(VersionEditor));
            RegisterEditor(typeof(char), typeof(CharEditor));
            RegisterEditor(typeof(char?), typeof(CharEditor));
#if NETCOREAPP
            RegisterEditor(typeof(Rune), typeof(CharEditor));
            RegisterEditor(typeof(Rune?), typeof(CharEditor));
#endif
        }

        /// <summary>
        /// Create a new IPropertyEditor object appropriate for the passed-in type. This is based upon what types are registered in this PropertyList control.
        /// </summary>
        /// <param name="propType">The type for which to get a IPropertyEditor for.</param>
        /// <returns></returns>
#if NETCOREAPP
        public IPropertyEditor? CreateEditorForType(Type propType)
#else
        public IPropertyEditor CreateEditorForType(Type propType)
#endif
        {
            try
            {
                if (registeredEditors.ContainsKey(propType))
                {
                    object o = Activator.CreateInstance(registeredEditors[propType]) ?? new object();
                    if (o is IPropertyEditor i)
                    {
                        return i;
                    }
                }
                else if (propType.IsEnum)
                {
                    // load enum editor
                    object o = Activator.CreateInstance(registeredEditors[typeof(Enum)] ?? typeof(EnumEditor)) ?? new object();
                    if (o is IPropertyEditor i)
                    {
                        return i;
                    }
                }
                else if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(List<>) && registeredEditors.ContainsKey(typeof(List<>)))
                {
                    Type itemType = propType.GetGenericArguments()[0];
                    object o = Activator.CreateInstance(registeredEditors[typeof(List<>)]) ?? new object();
                    if (o is IPropertyEditor i)
                    {
                        return i;
                    }
                }
                else if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(IEnumerable<>) && registeredEditors.ContainsKey(typeof(IEnumerable<>)))
                {
                    Type itemType = propType.GetGenericArguments()[0];
                    object o = Activator.CreateInstance(registeredEditors[typeof(IEnumerable<>)]) ?? new object();
                    if (o is IPropertyEditor i)
                    {
                        return i;
                    }
                }
                else if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(IEnumerable) && registeredEditors.ContainsKey(typeof(IEnumerable<>)))
                {
                    object o = Activator.CreateInstance(registeredEditors[typeof(IEnumerable<>)]) ?? new object();
                    if (o is IPropertyEditor i)
                    {
                        return i;
                    }
                }
                else if (typeof(IList).IsAssignableFrom(propType))
                {
                    object o = Activator.CreateInstance(registeredEditors[typeof(List<>)]) ?? new object();
                    if (o is IPropertyEditor i)
                    {
                        return i;
                    }
                }
                else if (typeof(IEnumerable).IsAssignableFrom(propType))
                {
                    object o = Activator.CreateInstance(registeredEditors[typeof(IEnumerable<>)]) ?? new object();
                    if (o is IPropertyEditor i)
                    {
                        return i;
                    }
                }
                else
                {

                    //else
                    //{
                    return null;
                    //}
                }
            }
            catch (ArgumentException) { return null; } // the editor type added to the list is not a type that can be activated via Reflection (due to the type's limitations)
            catch (NotSupportedException) { return null; } // the editor type added to the list is not a type that can be activated via Reflection (due to its own reasonable limitations)
            catch (TargetInvocationException) { return null; } // an exception occurred while trying to activate the editor type
            catch (MemberAccessException) { return null; } // an abstract class was added in as an editor type, or the parameter-less constructor cannot be accessed
            catch (COMException) { return null; } // not super sure how we'd run into this one lol
            catch (InvalidComObjectException) { return null; } // also not sure how we'd get this one lol
            catch (TypeLoadException) { return null; } // the editor type isn't an actual valid type object

            return null;
        }

        #endregion

        #region Visual Elements

#if NETCOREAPP
        private void ColumnWidthChanged(object? sender, EventArgs e)
        {
            foreach (UIElement? item in stkProperties.Children)
            {
#else
        private void ColumnWidthChanged(object sender, EventArgs e)
                {
            foreach (UIElement item in stkProperties.Children)
            {
#endif
                if (item is PropertyEditorItem pei)
                {
                    pei.UpdateColumnWidths(colNames.Width, colTypes.Width, colValues.Width);
                }
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            ReloadObject();
        }


        #region Show/Hide Toolbar Items (Dependency properties)

        /// <summary>
        /// Get or set if the Filter textbox should be visible at the top of the PropertyList control.
        /// </summary>
        public bool ShowFilterBox { get => (bool)GetValue(ShowFilterBoxProperty); set => SetValue(ShowFilterBoxProperty, value); }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static DependencyProperty ShowFilterBoxProperty
            = DependencyProperty.Register("ShowFilterBox", typeof(bool), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(true));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set if the Reload button should be visible at the top of the PropertyList control.
        /// </summary>
        public bool ShowReloadButton { get => (bool)GetValue(ShowReloadButtonProperty); set => SetValue(ShowReloadButtonProperty, value); }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static DependencyProperty ShowReloadButtonProperty
            = DependencyProperty.Register("ShowReloadButton", typeof(bool), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(true));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set if the View and Sort menu button should be visible at the top of the PropertyList control.
        /// </summary>
        public bool ShowViewMenu { get => (bool)GetValue(ShowViewMenuProperty); set => SetValue(ShowViewMenuProperty, value); }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static DependencyProperty ShowViewMenuProperty
            = DependencyProperty.Register("ShowViewMenu", typeof(bool), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(true));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion

        #endregion


        private void mnuShowInherited_Click(object sender, RoutedEventArgs e)
        {
            if (ShowInheritedProperties)
            {
                ShowInheritedProperties = false;
                //mnuShowInherited.IsChecked = false;
            }
            else
            {
                ShowInheritedProperties = true;
                //mnuShowInherited.IsChecked = true;
            }
        }
    }

    /// <summary>
    /// Represents the sorting method used for sorting properties in a PropertyList.
    /// </summary>
    public enum PropertySortOption
    {
        /// <summary>
        /// Sorted alphabetically by property name
        /// </summary>
        Name = 0,
        /// <summary>
        /// Sorted by category (as determined by a Category attribute being present)
        /// </summary>
        Category = 1
    }
}
