﻿using System;
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
using SolidShineUi.Utils;

namespace SolidShineUi.PropertyList
{
    /// <summary>
    /// A control that can display the properties and values of a .NET object, with support for live editing of many of them.
    /// This is the old version used in the Solid Shine UI 1.9 releases; please transition to the new <see cref="PropertyList"/> control.
    /// </summary>
    [Obsolete("This is the old ExperimentalPropertyList control used in Solid Shine UI 1.9; please transition to the new PropertyList control.")]
    public partial class ExperimentalPropertyList : UserControl, IPropertyEditorHost
    {
        /// <summary>
        /// Create a PropertyList.
        /// </summary>
        public ExperimentalPropertyList()
        {
            InitializeComponent();
            PreregisterEditors();

            // by default, let's set these foreground values to the base foreground
            TopPanelForeground = Foreground;
            HeaderForeground = Foreground;

            //txtType.Text = NOTHING_LOADED;

            var colDescriptor = DependencyPropertyDescriptor.FromProperty(ColumnDefinition.WidthProperty, typeof(ColumnDefinition));
            colDescriptor.AddValueChanged(colNames, ColumnWidthChanged);
            colDescriptor.AddValueChanged(colTypes, ColumnWidthChanged);
            colDescriptor.AddValueChanged(colValues, ColumnWidthChanged);

            GridlinePropertyChanged += (x, y) => { UpdateGridlines(); };

            DependencyPropertyDescriptor.FromProperty(ShowGridlinesProperty, typeof(ExperimentalPropertyList)).AddValueChanged(this, GridlinePropertyChanged);
            DependencyPropertyDescriptor.FromProperty(GridlineBrushProperty, typeof(ExperimentalPropertyList)).AddValueChanged(this, GridlinePropertyChanged);

            // use Clear to initialize the rest of the UI
            Clear();
        }

        #region Events

        /// <summary>
        /// Raised when the value of a property has changed via a property editor. This refers to the properties of the object currently being observed by the PropertyList control.
        /// </summary>
#if NETCOREAPP
        public event PropertyValueChangedEventHandler? PropertyValueChanged;
#else
        public event PropertyValueChangedEventHandler PropertyValueChanged;
#endif

        /// <summary>
        /// Raised when the currently loaded/observed object in the PropertyList control changes.
        /// </summary>
        /// <remarks>
        /// This is also raised when the ReloadObject 
        /// </remarks>
#if NETCOREAPP
        public event PropertyListObjectEventHandler? LoadedObjectChanged;
#else
        public event PropertyListObjectEventHandler LoadedObjectChanged;
#endif
        #endregion

        /// <summary>
        /// Get the internal contents of this PropertyList control.
        /// </summary>
        /// <remarks>
        /// To load in an object into the PropertyList, please use the <see cref="LoadObject(object)"/> function instead.
        /// It is highly unrecommended to access and modify the internal contents of the PropertyList control in this way, but it is possible.
        /// </remarks>
        public new object Content { get => base.Content; set { } }

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

        private static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ExperimentalPropertyList w)
            {
                w.ColorSchemeChanged?.Invoke(d, e);
#if NETCOREAPP
                w.ApplyColorScheme((e.NewValue as ColorScheme)!);
#else
                w.ApplyColorScheme(e.NewValue as ColorScheme);
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

            // set up brushes
            Background = cs.LightBackgroundColor.ToBrush();
            HeaderBackground = cs.ThirdHighlightColor.ToBrush();
            HeaderDividerBrush = cs.BorderColor.ToBrush();
            HeaderForeground = cs.ForegroundColor.ToBrush();
            TopPanelBackground = cs.BackgroundColor.ToBrush();
            TopPanelForeground = cs.ForegroundColor.ToBrush();
            ToolbarBackground = cs.BackgroundColor.ToBrush();
            ButtonHighlightBrush = cs.HighlightColor.ToBrush();
            ButtonClickBrush = cs.ThirdHighlightColor.ToBrush();

            btnRefresh.ColorScheme = cs;
            mnuView.ColorScheme = cs;

            // set up icons
            imgSearch.Source = IconLoader.LoadIcon("Search", cs);
            imgReload.Source = IconLoader.LoadIcon("Reload", cs);

        }

        #endregion

        #region Basics / Object Loading

        /// <summary>a list of all the properties in the loaded object's type</summary>
        private List<PropertyInfo> properties = new List<PropertyInfo>();
        /// <summary>the object that's currently loaded</summary>
#if NETCOREAPP
        private object? _baseObject = null;
#else
        private object _baseObject = null;
#endif

        private bool _isReloading = false;

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
            ReloadObject(false);
        }

        /// <summary>
        /// Reload the properties and values from the currently observed object, with the option to reset filter and view settings if desired.
        /// </summary>
        /// <param name="resetViewSettings">set if the filter and view settings should be reset when the object is reloaded</param>
        public void ReloadObject(bool resetViewSettings)
        {
            if (_baseObject != null)
            {
                _isReloading = true;

                //string _oldFilter = _filterString;
                //string _displName = ObjectDisplayName;
                //bool _oldInherits = _showInherited;
                //bool _oldReadOnly = _showReadOnly;

                LoadObject(_baseObject);

                if (resetViewSettings)
                {
                    _showReadOnly = true;
                    _showInherited = true;
                    FilterProperties("");
                }

                //if (!resetViewSettings)
                //{
                //    _showReadOnly = _oldReadOnly;
                //    _showInherited = _oldInherits;
                //    ObjectDisplayName = _displName;
                //    FilterProperties(_oldFilter);
                //}
                _isReloading = false;
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
            txtType.Text = NOTHING_LOADED;
            txtType.ToolTip = "";

            btnRefresh.IsEnabled = false;
        }

        /// <summary>
        /// The string "No name", used for objects that don't have a Name property to get a name from.
        /// </summary>
        public static string NO_NAME = "No name";

        /// <summary>
        /// The string "Nothing loaded", used when there is not an object loaded into this control.
        /// </summary>
        public static string NOTHING_LOADED = "Nothing loaded";

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
            txtType.Text = TypeLabel + PrettifyPropertyType(type); // name of the type (not the object)
            txtType.ToolTip = PrettifyPropertyType(type, true);

            // load all properties
            properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            SortList();

            LoadPropertyList(properties);

            FilterProperties(txtFilter.Text);

            //txtFilter.Text = "";
            //_filterString = "";
            //_showInherited = true;
            //mnuShowInherited.IsChecked = true;
            //_showReadOnly = true;
            //mnuShowReadOnly.IsChecked = true;

            btnRefresh.IsEnabled = true;

            LoadedObjectChanged?.Invoke(this, new PropertyListObjectEventArgs(_baseObject, type, _isReloading));
        }

        #region Display Name / Labels
        /// <summary>
        /// Get or set the string used to name the object being observed. The PropertyList will try to set this automatically via looking at the Name property,
        /// or otherwise you can set a custom name to display.
        /// </summary>
        [Category("Appearance")]
        public string ObjectDisplayName { get => (string)GetValue(ObjectDisplayNameProperty); set => SetValue(ObjectDisplayNameProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty ObjectDisplayNameProperty
            = DependencyProperty.Register("ObjectDisplayName", typeof(string), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata("No name"));

        /// <summary>
        /// Get or set the label to display next to the object's type at the top of the control.
        /// </summary>
        /// <remarks>
        /// You can hide the entire type label and display by setting <see cref="ShowTypeDisplay"/> to false.
        /// </remarks>
        [Category("Text Labels")]
        public string TypeLabel { get => (string)GetValue(TypeLabelProperty); set => SetValue(TypeLabelProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty TypeLabelProperty
            = DependencyProperty.Register("TypeLabel", typeof(string), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata("Type: "));

        /// <summary>
        /// Get or set the label to display in the View menu in the toolbar.
        /// </summary>
        [Category("Text Labels")]
        public string ViewMenuLabel { get => (string)GetValue(ViewMenuLabelProperty); set => SetValue(ViewMenuLabelProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty ViewMenuLabelProperty
            = DependencyProperty.Register("ViewMenuLabel", typeof(string), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata("View"));

        /// <summary>
        /// Get or set the label to display in the column header for the Name column.
        /// </summary>
        [Category("Text Labels")]
        public string NameHeaderLabel { get => (string)GetValue(NameHeaderLabelProperty); set => SetValue(NameHeaderLabelProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty NameHeaderLabelProperty
            = DependencyProperty.Register("NameHeaderLabel", typeof(string), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata("Name"));

        /// <summary>
        /// Get or set the label to display in the column header for the Type column.
        /// </summary>
        [Category("Text Labels")]
        public string TypeHeaderLabel { get => (string)GetValue(TypeHeaderLabelProperty); set => SetValue(TypeHeaderLabelProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty TypeHeaderLabelProperty
            = DependencyProperty.Register("TypeHeaderLabel", typeof(string), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata("Type"));

        /// <summary>
        /// Get or set the label to display in the column header for the Value column.
        /// </summary>
        [Category("Text Labels")]
        public string ValueHeaderLabel { get => (string)GetValue(ValueHeaderLabelProperty); set => SetValue(ValueHeaderLabelProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty ValueHeaderLabelProperty
            = DependencyProperty.Register("ValueHeaderLabel", typeof(string), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata("Value"));

        /// <summary>
        /// Get or set the label to display in a tool-tip when the Filter text box has focus or mouse over.
        /// </summary>
        [Category("Text Labels")]
        public string FilterBoxToolTip { get => (string)GetValue(FilterBoxToolTipProperty); set => SetValue(FilterBoxToolTipProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty FilterBoxToolTipProperty
            = DependencyProperty.Register("FilterBoxToolTip", typeof(string), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata("Filter (use @ to filter by name only)"));


        #endregion


        void LoadPropertyList(IEnumerable<PropertyInfo> properties)
        {
            stkProperties.Children.Clear();

            Type baseType = _baseObject?.GetType() ?? typeof(object);

            foreach (PropertyInfo item in properties)
            {
                // first, check the property against the DisplayOptions
                if (!CheckPropertyDisplay(item)) continue;

                // skip any set-only properties
                if (!item.CanRead) continue;

                PropertyEditorItem pei = new PropertyEditorItem();
#if NETCOREAPP
                IPropertyEditor? ipe = null;
#else
                IPropertyEditor ipe = null;
#endif
                //if (!item.CanWrite)
                //{
                //    // readonly property
                //}

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
                    ipe.ApplyColorScheme(ColorScheme);
                    ipe.SetHostControl(this);
                    ipe.IsPropertyWritable = item.CanWrite;
                }

                pei.LoadProperty(item, item.CanRead ? item.GetValue(_baseObject) : null, ipe);
                pei.PropertyEditorValueChanged += editor_PropertyEditorValueChanged;
                pei.UpdateColumnWidths(colNames.Width, colTypes.Width, colValues.Width);
                pei.ShowGridlines = ShowGridlines;
                pei.GridlineBrush = GridlineBrush;
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
                    PropertyValueChanged?.Invoke(this, new PropertyValueChangedEventArgs(e));
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
        /// Get or set how the list of properties are sorted in the PropertyList.
        /// </summary>
        /// <remarks>
        /// If this setting is changed, you will need to reload the object (<see cref="ReloadObject()"/>) or load a new object to apply that change.
        /// </remarks>
        [Category("Common")]
        public PropertySortOption SortOption { get => _sort; set { _sort = value; SortList(); } }

        /// <summary>
        /// Get or set the settings for what properties should be displayed in the PropertyList.
        /// </summary>
        /// <remarks>
        /// When loading in an object, the attributes for each property in that object are looked at. If a property has an attribute that matches what a flag disallows,
        /// that property is not loaded. If this setting is changed, you will need to reload the object (<see cref="ReloadObject()"/>) or load a new object to apply that change.
        /// Note that set-only properties are not supported by PropertyList and thus won't be displayed, regardless of this setting.
        /// </remarks>
        [Category("Common")]
        public PropertyListDisplayFlags DisplayOptions { get; set; } = PropertyListDisplayFlags.HidePropertyListHide;

        /// <summary>
        /// Get or set the string to use for filtering the properties. Only properties that match this filter text will be displayed,
        /// or use <c>null</c> or an empty string for no filtering.
        /// </summary>
        /// <remarks>
        /// If the first character is <c>@</c>, then only the property names will be matched. Otherwise, the property names or types may be matched.
        /// <para/>
        /// For version 2.0, this will be the only supported way to set or edit the filter text.
        /// Setting this value will call <see cref="FilterProperties(string)"/> to filter out properties that don't match this text.
        /// </remarks>
        [Category("Common")]
        public string FilterText
        {
            get => _filterString;
            set => FilterProperties(value);
        }

        private bool _showInherited = true;
        private bool _showReadOnly = true;

        /// <summary>
        /// Get or set if inherited properties (properties not defined directly in the observed object's type) are visible in the PropertyList.
        /// </summary>
        [Category("Common")]
        public bool ShowInheritedProperties { get => _showInherited; set { _showInherited = value; FilterProperties(_filterString); mnuShowInherited.IsChecked = value; } }

        /// <summary>
        /// Get or set if read-only properties (properties that only have a <c>get</c> section, and cannot be set/changed) are visible in the PropertyList.
        /// </summary>
        [Category("Common")]
        public bool ShowReadOnlyProperties { get => _showReadOnly; set { _showReadOnly = value; FilterProperties(_filterString); mnuShowReadOnly.IsChecked = value; } }

        /// <summary>
        /// Get the category that this property is said to be a part of (if this property has a <see cref="CategoryAttribute"/> attached to it).
        /// </summary>
        /// <param name="pi">The property to look up the category for.</param>
        /// <returns>The name of the category that this property is added to, or an empty string if there was no <see cref="CategoryAttribute"/> found.</returns>
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

        /// <summary>
        /// Check the property against the flags of <see cref="DisplayOptions"/>, and return if the property should be displayed.
        /// </summary>
        /// <param name="pi">The property to check.</param>
        /// <returns>True if the property should be displayed; false if the property has (or is lacking) the checked attributes and should be hidden.</returns>
        private bool CheckPropertyDisplay(PropertyInfo pi)
        {
            if (DisplayOptions.HasFlag(PropertyListDisplayFlags.ShowAll)) return true;
            else if (DisplayOptions.HasFlag(PropertyListDisplayFlags.OnlyShowPropertyListShow))
            {
                if (pi.GetCustomAttribute<PropertyListShowAttribute>() != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                // let's go through the list
                object[] attributes = pi.GetCustomAttributes(true);

                if (DisplayOptions.HasFlag(PropertyListDisplayFlags.HideObsolete) && attributes.Any(o => o is ObsoleteAttribute))
                {
                    return false;
                }

                if (DisplayOptions.HasFlag(PropertyListDisplayFlags.HidePropertyListHide) && attributes.Any(h => h is PropertyListHideAttribute))
                {
                    return false;
                }

                if (DisplayOptions.HasFlag(PropertyListDisplayFlags.HideBrowseableFalse) && attributes.Length > 0)
                {
                    var elist = attributes.Where(oo => oo is EditorBrowsableAttribute || oo is BrowsableAttribute);
                    if (elist.Any())
                    {
                        foreach (var item in elist)
                        {
                            if (item is EditorBrowsableAttribute eb && eb.State == EditorBrowsableState.Never)
                            {
                                return false;
                            }
                            else if (item is BrowsableAttribute bb && bb.Browsable == false)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

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
        /// <remarks>
        /// For version 2.0, this function will no longer be available. Please instead transition to using <see cref="FilterText"/>.
        /// </remarks>
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

            if (!_showReadOnly)
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
                            if (pei.IsReadOnly)
                            {
                                pei.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                }
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

        #region Sort and View menu

        private void btnName_Click(object sender, RoutedEventArgs e)
        {
            SortOption = PropertySortOption.Name;
            LoadPropertyList(properties);
            FilterProperties(_filterString);
        }

        private void btnCategory_Click(object sender, RoutedEventArgs e)
        {
            SortOption = PropertySortOption.Category;
            LoadPropertyList(properties);
            FilterProperties(_filterString);
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

        private void mnuShowInherited_Click(object sender, RoutedEventArgs e)
        {
            ShowInheritedProperties = !ShowInheritedProperties;
        }
        private void mnuShowReadOnly_Click(object sender, RoutedEventArgs e)
        {
            ShowReadOnlyProperties = !ShowReadOnlyProperties;
        }

        private void mnuGridlines_Click(object sender, RoutedEventArgs e)
        {
            ShowGridlines = !ShowGridlines;
        }

        private void mnuGridlineBrush_Click(object sender, RoutedEventArgs e)
        {
            Color col = Colors.LightGray;
            if (GridlineBrush is SolidColorBrush scb) col = scb.Color;
            ColorPickerDialog cpd = new ColorPickerDialog(ColorScheme, col);
            cpd.Owner = Window.GetWindow(this);
            cpd.ShowDialog();
            if (cpd.DialogResult)
            {
                GridlineBrush = new SolidColorBrush(cpd.SelectedColor);
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

            registeredEditors[type] = editor;
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
            RegisterEditor(typeof(long), typeof(LongEditor));
            RegisterEditor(typeof(uint), typeof(LongEditor));
            RegisterEditor(typeof(ulong), typeof(LongEditor));
            RegisterEditor(typeof(long?), typeof(LongEditor));
            RegisterEditor(typeof(uint?), typeof(LongEditor));
            RegisterEditor(typeof(ulong?), typeof(LongEditor));
            RegisterEditor(typeof(decimal), typeof(DecimalEditor));
            RegisterEditor(typeof(decimal?), typeof(DecimalEditor));
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
            RegisterEditor(typeof(FontStyle?), typeof(FontStyleEditor));
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
            RegisterEditor(typeof(GridLength), typeof(GridLengthEditor));
            RegisterEditor(typeof(GridLength?), typeof(GridLengthEditor));
            RegisterEditor(typeof(Color), typeof(ColorEditor));
            RegisterEditor(typeof(Color?), typeof(ColorEditor));
            RegisterEditor(typeof(CornerRadius), typeof(CornerRadiusEditor));
            RegisterEditor(typeof(CornerRadius?), typeof(CornerRadiusEditor));
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
        /// <returns>An IPropertyEditor that can be used for editing the type, if available; <c>null</c> otherwise</returns>
#if NETCOREAPP
        public IPropertyEditor? CreateEditorForType(Type propType)
#else
        public IPropertyEditor CreateEditorForType(Type propType)
#endif
        {
            try
            {
                if (registeredEditors.TryGetValue(propType, out var editorType) && editorType != null)
                {
                    object o = Activator.CreateInstance(editorType) ?? new object();
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

        #region Show/Hide Top Labels

        /// <summary>
        /// Get or set if the name display text is displayed at the top of the PropertyList control.
        /// </summary>
        /// <remarks>
        /// PropertyList tries to load the name of a control by looking at its <c>Name</c> property; otherwise, you can also custom set a name via the <see cref="ObjectDisplayName"/> property.
        /// </remarks>
        [Category("Appearance")]
        public bool ShowNameDisplay { get => (bool)GetValue(ShowNameDisplayProperty); set => SetValue(ShowNameDisplayProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty ShowNameDisplayProperty
            = DependencyProperty.Register("ShowNameDisplay", typeof(bool), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Get or set if the type display text should be visible at the top of the PropertyList control.
        /// </summary>
        [Category("Appearance")]
        public bool ShowTypeDisplay { get => (bool)GetValue(ShowTypeDisplayProperty); set => SetValue(ShowTypeDisplayProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty ShowTypeDisplayProperty
            = DependencyProperty.Register("ShowTypeDisplay", typeof(bool), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(true));

        #endregion

        #region Show/Hide Toolbar Items (Dependency properties)

        /// <summary>
        /// Get or set if the Filter textbox should be visible at the top of the PropertyList control.
        /// </summary>
        [Category("Appearance")]
        public bool ShowFilterBox { get => (bool)GetValue(ShowFilterBoxProperty); set => SetValue(ShowFilterBoxProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty ShowFilterBoxProperty
            = DependencyProperty.Register("ShowFilterBox", typeof(bool), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Get or set if the Reload button should be visible at the top of the PropertyList control.
        /// </summary>
        [Category("Appearance")]
        public bool ShowReloadButton { get => (bool)GetValue(ShowReloadButtonProperty); set => SetValue(ShowReloadButtonProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty ShowReloadButtonProperty
            = DependencyProperty.Register("ShowReloadButton", typeof(bool), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Get or set if the View and Sort menu button should be visible at the top of the PropertyList control.
        /// </summary>
        [Category("Appearance")]
        public bool ShowViewMenu { get => (bool)GetValue(ShowViewMenuProperty); set => SetValue(ShowViewMenuProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty ShowViewMenuProperty
            = DependencyProperty.Register("ShowViewMenu", typeof(bool), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(true));

        #endregion

        #region Gridlines

        /// <summary>
        /// Get or set if gridlines are visible between the properties in the control.
        /// </summary>
        [Category("Appearance")]
        public bool ShowGridlines { get => (bool)GetValue(ShowGridlinesProperty); set => SetValue(ShowGridlinesProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty ShowGridlinesProperty
            = DependencyProperty.Register("ShowGridlines", typeof(bool), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get or set the brush of the gridlines in the control. Use <see cref="ShowGridlines"/> to actually display the gridlines.
        /// </summary>
        [Category("Brushes")]
        public Brush GridlineBrush { get => (Brush)GetValue(GridlineBrushProperty); set => SetValue(GridlineBrushProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty GridlineBrushProperty
            = DependencyProperty.Register("GridlineBrush", typeof(Brush), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        private event EventHandler GridlinePropertyChanged;

        void UpdateGridlines()
        {
            mnuGridlines.IsChecked = ShowGridlines;

#if NETCOREAPP
            foreach (UIElement? item in stkProperties.Children)
#else
            foreach (UIElement item in stkProperties.Children)
#endif
            {
                if (item == null) continue;
                if (item is PropertyEditorItem pei)
                {
                    pei.GridlineBrush = GridlineBrush;
                    pei.ShowGridlines = ShowGridlines;
                }
            }
        }

        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the brush for the background of the column headers.
        /// </summary>
        [Category("Brushes")]
        public Brush HeaderBackground { get => (Brush)GetValue(HeaderBackgroundProperty); set => SetValue(HeaderBackgroundProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty HeaderBackgroundProperty
            = DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// Get or set the brush for the foreground of the column headers.
        /// </summary>
        [Category("Brushes")]
        public Brush HeaderForeground { get => (Brush)GetValue(HeaderForegroundProperty); set => SetValue(HeaderForegroundProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty HeaderForegroundProperty
            = DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Get or set the brush for the background of the toolbar near the top of the control.
        /// </summary>
        [Category("Brushes")]
        public Brush ToolbarBackground { get => (Brush)GetValue(ToolbarBackgroundProperty); set => SetValue(ToolbarBackgroundProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty ToolbarBackgroundProperty
            = DependencyProperty.Register("ToolbarBackground", typeof(Brush), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// Get or set the brush for the buttons on the toolbar when they are highlighted (i.e. mouse over or keyboard focus).
        /// </summary>
        [Category("Brushes")]
        public Brush ButtonHighlightBrush { get => (Brush)GetValue(ButtonHighlightBrushProperty); set => SetValue(ButtonHighlightBrushProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty ButtonHighlightBrushProperty
            = DependencyProperty.Register("ButtonHighlightBrush", typeof(Brush), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// Get or set the brush for the buttons on the toolbar when they are being clicked.
        /// </summary>
        [Category("Brushes")]
        public Brush ButtonClickBrush { get => (Brush)GetValue(ButtonClickBrushProperty); set => SetValue(ButtonClickBrushProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty ButtonClickBrushProperty
            = DependencyProperty.Register("ButtonClickBrush", typeof(Brush), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.DimGray)));

        /// <summary>
        /// Get or set the brush for the borders of the buttons on the toolbar, when the buttons are being highlighted or clicked. (The buttons display no border when not focused.)
        /// </summary>
        [Category("Brushes")]
        public Brush ButtonHighlightBorderBrush { get => (Brush)GetValue(ButtonHighlightBorderBrushProperty); set => SetValue(ButtonHighlightBorderBrushProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty ButtonHighlightBorderBrushProperty
            = DependencyProperty.Register("ButtonHighlightBorderBrush", typeof(Brush), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

        /// <summary>
        /// Get or set the brush for the background of the top panel of the control (where the name and type are displayed).
        /// </summary>
        [Category("Brushes")]
        public Brush TopPanelBackground { get => (Brush)GetValue(TopPanelBackgroundProperty); set => SetValue(TopPanelBackgroundProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty TopPanelBackgroundProperty
            = DependencyProperty.Register("TopPanelBackground", typeof(Brush), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// Get or set the brush for the foreground of the top panel of the control (where the name and type are displayed).
        /// </summary>
        [Category("Brushes")]
        public Brush TopPanelForeground { get => (Brush)GetValue(TopPanelForegroundProperty); set => SetValue(TopPanelForegroundProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty TopPanelForegroundProperty
            = DependencyProperty.Register("TopPanelForeground", typeof(Brush), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Get or set the brush for the dividers between the column headers.
        /// </summary>
        [Category("Brushes")]
        public Brush HeaderDividerBrush { get => (Brush)GetValue(HeaderDividerBrushProperty); set => SetValue(HeaderDividerBrushProperty, value); }

        /// <summary>
        /// Identifies a dependency property for this control. Please see the related property for more details.
        /// </summary>
        public static DependencyProperty HeaderDividerBrushProperty
            = DependencyProperty.Register("HeaderDividerBrush", typeof(Brush), typeof(ExperimentalPropertyList),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

        #endregion

        #endregion

        /// <summary>
        /// Get a slightly cleaned up and more human-readable name of a type.
        /// </summary>
        /// <param name="type">The type to get the name of.</param>
        /// <param name="fullName">Determine if the full name (including namespaces) should be returned or just the type's own name</param>
        /// <returns></returns>
        public static string PrettifyPropertyType(Type type, bool fullName = false)
        {
            string typeString = type.FullName ?? "(no type name)";
            string baseName = type.Name;

            if (type.IsGenericType)
            {
                var generics = type.GetGenericArguments();

                if (typeString.StartsWith("System.Nullable"))
                {
                    return (fullName ? generics[0].FullName : generics[0].Name) + "?";
                }

                string basebase = (fullName ? (type.GetGenericTypeDefinition().FullName ?? "System.Object") : baseName).Replace("`1", "").Replace("`2", "").Replace("`3", "").Replace("`4", "").Replace("`5", "");

                if (generics.Length == 1)
                {
                    return basebase + "<" + (fullName ? generics[0].FullName : generics[0].Name) + ">";
                }
                else
                {
                    return basebase + "<" + string.Join(",", generics.Select(x => fullName ? x.FullName : x.Name)) + ">";
                }
            }
            else
            {
                return fullName ? typeString : baseName;
            }
        }

        /// <inheritdoc/>
#if NETCOREAPP
        public Window? GetWindow()
#else
        public Window GetWindow()
#endif
        {
            return Window.GetWindow(this);
        }
    }
}
