using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using SolidShineUi;
using System.Windows.Controls;
using System.Reflection;
using System.Linq;

namespace SolidShineUi.PropertyList.Dialogs
{
    /// <summary>
    /// A dialog for viewing and editing the contents of a list or IEnumerable.
    /// </summary>
    public partial class ListEditorDialog : FlatWindow
    {
        /// <summary>
        /// Create a ListEditorDialog.
        /// </summary>
        public ListEditorDialog()
        {
            InitializeComponent();
        }

        private void window_SourceInitialized(object sender, EventArgs e)
        {
            DisableMinimizeAction();
        }

        #region Base variables

        IEnumerable baseObject = new List<string>();

        bool isCollection = false;
        bool isList = false;

        bool canEdit = true;

#if NETCOREAPP
        /// <summary>the type of the items contained in the list (i.e. a <c>List&lt;string&gt;</c> has a baseType <c>string</c>)</summary>
        Type? baseType = null;

        /// <summary>the type of the IPropertyEditor, if there is one, to use with this list</summary>
        Type? editorType = null;

        /// <summary>the parent PropertyList control</summary>
        ExperimentalPropertyList? parentList = null;
#else
        /// <summary>the type of the items contained in the list (i.e. a <c>List&lt;string&gt;</c> has a baseType <c>string</c>)</summary>
        Type baseType = null;

        /// <summary>the type of the IPropertyEditor, if there is one, to use with this list</summary>
        Type editorType = null;

        /// <summary>the parent PropertyList control</summary>
        ExperimentalPropertyList parentList = null;
#endif

        int count = -1;

        int addMode = ADD_CANNOT_ADD;
        const int ADD_CANNOT_ADD = 0;
        const int ADD_PRIMITIVE_TYPE = 1;
        const int ADD_STRING_MODE = 2;
        const int ADD_STANDARD = 3;

        #endregion

        #region Dependency Properties

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static DependencyProperty DescriptionProperty
            = DependencyProperty.Register("Description", typeof(string), typeof(ListEditorDialog),
            new FrameworkPropertyMetadata("Edit and view collection:"));

        public static DependencyProperty ListOfItemsLabelProperty
            = DependencyProperty.Register("ListOfItemsLabel", typeof(string), typeof(ListEditorDialog),
            new FrameworkPropertyMetadata("List of items:"));

        public static DependencyProperty EditItemLabelProperty
            = DependencyProperty.Register("EditItemLabel", typeof(string), typeof(ListEditorDialog),
            new FrameworkPropertyMetadata("Edit current item:"));

        public static DependencyProperty EnumerableWarningTitleLabelProperty
            = DependencyProperty.Register("EnumerableWarningTitleLabel", typeof(string), typeof(ListEditorDialog),
            new FrameworkPropertyMetadata("Full Collection May Not Be Available"));

        public static DependencyProperty EnumerableWarningDescriptionLabelProperty
            = DependencyProperty.Register("EnumerableWarningDescriptionLabel", typeof(string), typeof(ListEditorDialog),
            new FrameworkPropertyMetadata("Note that with this collection, it may not yet be fully populated. For example, this collection could require some processing or downloading" +
            "to populate.You can load in what's currently in this collection, but it may be incomplete, or it may take time or cause additional code to be executed."));

        public static DependencyProperty LoadCollectionLabelProperty
            = DependencyProperty.Register("LoadCollectionLabel", typeof(string), typeof(ListEditorDialog),
            new FrameworkPropertyMetadata("Load Collection"));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the description text to display at the top of the dialog.
        /// This should describe the collection/list being viewed, and maybe some other general info like the type of the items in the collection and the total count of items.
        /// </summary>
        public string Description { get => (string)GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value); }

        /// <summary>
        /// Get or set the text in the "List of items:" label.
        /// </summary>
        public string ListOfItemsLabel { get => (string)GetValue(ListOfItemsLabelProperty); set => SetValue(ListOfItemsLabelProperty, value); }

        /// <summary>
        /// Get or set the text in the "Edit current item:" label.
        /// </summary>
        public string EditItemLabel { get => (string)GetValue(EditItemLabelProperty); set => SetValue(EditItemLabelProperty, value); }

        /// <summary>
        /// Get or set the text in the "Full Collection May Not Be Available" label, displayed at the top of the IEnumerable warning section.
        /// </summary>
        public string EnumerableWarningTitleLabel { get => (string)GetValue(EnumerableWarningTitleLabelProperty); set => SetValue(EnumerableWarningTitleLabelProperty, value); }

        /// <summary>
        /// Get or set the text in the description of the IEnumerable warning section.
        /// </summary>
        public string EnumerableWarningDescriptionLabel { get => (string)GetValue(EnumerableWarningDescriptionLabelProperty); set => SetValue(EnumerableWarningDescriptionLabelProperty, value); }

        /// <summary>
        /// Get or set the text of the "Load Collection" label.
        /// </summary>
        public string LoadCollectionLabel { get => (string)GetValue(LoadCollectionLabelProperty); set => SetValue(LoadCollectionLabelProperty, value); }

        #endregion

        /// <summary>
        /// Load in a list or IEnumerable to display in this dialog.
        /// </summary>
        /// <param name="items">The items to load into the dialog.</param>
        /// <param name="enumerableType">The type of items being contained in this IEnumerable.</param>
        /// <param name="editor">The editor type for the type of the items in the IEnumerable, if there is any</param>
#if NETCOREAPP
        public void LoadEnumerable(IEnumerable items, Type enumerableType, Type? editor = null)
#else
        public void LoadEnumerable(IEnumerable items, Type enumerableType, Type editor = null)
#endif
        {
            baseObject = items;
            baseType = enumerableType;
            editorType = editor;

            if (editorType != null)
            {
                propList.Visibility = Visibility.Collapsed;
                txtAvailableEditor.Visibility = Visibility.Visible;
            }

            // first, let's determine what we're working with here

            if (items is ICollection coll)
            {
                isCollection = true;

                count = coll.Count;

                if (items is IList)
                {
                    isList = true;
                }
            }

            // okay, so now let's enable features
            if (isCollection)
            {
                EnumerateItems();
            }
            else
            {
                brdrShowEnumerable.Visibility = Visibility.Visible;
            }

            if (isList)
            {
                DetermineIfCanAdd();
            }
            else
            {
                btnAdd.Visibility = Visibility.Collapsed;
            }

        }

        private void EnumerateItems()
        {
            int index = 0;

            foreach (var item in baseObject)
            {
                ListEditorItem lei = CreateListItem(item, index);

                selList.Items.Add(lei);
                index++;
            }
        }

        // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types
        private static Type[] basicTypes = new Type[] { typeof(bool), typeof(byte), typeof(double), typeof(float), typeof(int), typeof(uint), typeof(long), typeof(short), typeof(string),
            typeof(sbyte), typeof(char), typeof(decimal), typeof(ulong), typeof(ushort)};

        private void DetermineIfCanAdd()
        {
            // at this point, we've already determined this is a List of items
            // we should also already have the baseType that we're working from

            if (baseType == null)
            {
                CannotAdd();
            }            
            else if (baseType.GetConstructor(Type.EmptyTypes) == null)
            {
                // does not have a parameterless constructor
                if (basicTypes.Contains(baseType))
                {
                    addMode = ADD_PRIMITIVE_TYPE;
                }
                else if (baseType.GetConstructor(new Type[] { typeof(string) }) != null)
                {
                    // there is a string-based constructor that we can use
                    addMode = ADD_STRING_MODE;
                }
                else
                {
                    CannotAdd();
                }
            }
            else
            {
                // has a parameterless constructor
                addMode = ADD_STANDARD;
            }

            void CannotAdd()
            {
                addMode = ADD_CANNOT_ADD;
                btnAdd.IsEnabled = false;
                txtCannotAdd.Visibility = Visibility.Visible;
            }
        }

        #region CreateListItem
#if NETCOREAPP
        ListEditorItem CreateListItem(object? item, int index)
#else
        ListEditorItem CreateListItem(object item, int index)
#endif
        {
            ListEditorItem lei = new ListEditorItem();
#if NETCOREAPP
            IPropertyEditor? editor = null;
#else
            IPropertyEditor editor = null;
#endif
            if (editorType != null)
            {
#if NETCOREAPP
                editor = (IPropertyEditor?)Activator.CreateInstance(editorType);
#else
                editor = (IPropertyEditor)Activator.CreateInstance(editorType);
#endif

                if (editor != null)
                {
                    editor.ColorScheme = ColorScheme;
                    if (parentList != null)
                    {
                        editor.ParentPropertyList = parentList;
                    }
                }
            }
            lei.LoadItem(item, baseType ?? typeof(object), editor);
            lei.ItemIndex = index;

            lei.VisibleControls = isList ? ListEditorItemControls.RemoveOnly : ListEditorItemControls.None;
            lei.RequestRemove += (s, e) =>
            {
                if (isList && canEdit)
                {
                    if (baseObject is IList icol)
                    {
                        icol.Remove(lei.ItemValue);

                        // if this item is currently shown in the PropertyList, then unload it from the PropertyList
                        if (propList.GetCurrentlyLoadedObject() == lei.ItemValue)
                        {
                            propList.Clear();
                        }

                        selList.Items.Remove(lei);
                        count--;
                    }
                }
            };

            lei.PropertyEditorValueChanged += (s, e) =>
            {
                if (isList && canEdit)
                {
                    ValueChanged(e.NewValue, item, e);
                }
                else
                {
                    e.ChangeFailed = true;
                    e.FailedChangePropertyValue = item;
                }
            };

            return lei;
        }
        #endregion

#if NETCOREAPP
        void ValueChanged(object? newValue, object? baseItem, PropertyEditorValueChangedEventArgs e)
#else
        void ValueChanged(object newValue, object baseItem, PropertyEditorValueChangedEventArgs e)
#endif
        {
            if (baseObject is IList icol)
            {
                int index = icol.IndexOf(baseItem);
                if (index != -1)
                {
                    // this means the base list actually removed this item in the interim
                    // for now, we'll mark this as a failed change
                    // TODO: actually remove the offending ListEditorItem from the dialog
                    e.ChangeFailed = true;
                    e.FailedChangePropertyValue = null;
                    return;
                }
                icol[index] = newValue;
            }
        }

        private void btnEnumWarning_Click(object sender, RoutedEventArgs e)
        {
            brdrShowEnumerable.Visibility = Visibility.Collapsed;
            EnumerateItems();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void selList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (propList.Visibility == Visibility.Visible)
            {
                if (selList.Items.SelectedItems.Count == 0) return;
                ListEditorItem lei = (ListEditorItem)selList.Items.SelectedItems[0];
                if (lei.ItemValue != null)
                {
                    propList.LoadObject(lei.ItemValue);
                    if (propList.ObjectDisplayName == ExperimentalPropertyList.NO_NAME)
                    {
                        propList.ObjectDisplayName = "Item " + lei.ItemIndex;
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (isList && baseType != null)
            {
                switch (addMode)
                {
                    case ADD_STANDARD:
                        var newItem = Activator.CreateInstance(baseType);
                        if (newItem != null)
                        {
                            CreateItem(newItem);
                        }
                        else
                        {
                            btnAdd.IsEnabled = false;
                        }
                        break;
                    case ADD_STRING_MODE:
                        StringInputDialog sid = new StringInputDialog(ColorScheme, "Add Item", "Enter a string value to use for creating a new item:");
                        sid.Owner = this;
                        sid.ShowDialog();
                        if (sid.DialogResult)
                        {
                            var newSItem = Activator.CreateInstance(baseType, new object[] { sid.Value });
                            if (newSItem != null)
                            {
                                CreateItem(newSItem);
                            }
                        }
                        break;
                    case ADD_PRIMITIVE_TYPE:
                        // let's go down the list of basic types
                        // I've tried to sort by what is probably the most common first
                        if (baseType == typeof(bool)) { CreateItem(false); }
                        else if (baseType == typeof(string)) { CreateItem(""); }
                        else if (baseType == typeof(int)) { CreateItem(0); }
                        else if (baseType == typeof(byte)) { CreateItem((byte)0); }
                        else if (baseType == typeof(double)) { CreateItem(0.0d); }
                        else if (baseType == typeof(uint)) { CreateItem(0u); }
                        else if (baseType == typeof(long)) { CreateItem(0L); }
                        else if (baseType == typeof(float)) { CreateItem(0.0f); }
                        else if (baseType == typeof(ulong)) { CreateItem(0UL); }
                        else if (baseType == typeof(short)) { CreateItem((short)0); }
                        else if (baseType == typeof(ushort)) { CreateItem((ushort)0); }
                        else if (baseType == typeof(sbyte)) { CreateItem((sbyte)0); }
                        else if (baseType == typeof(char)) { CreateItem('A'); }
                        else if (baseType == typeof(decimal)) { CreateItem(0.0m); }
                        break;
                    default:
                        // cannot add?
                        btnAdd.IsEnabled = false;
                        break;
                }
            }
            else
            {
                btnAdd.IsEnabled = false;
            }

#if NETCOREAPP
            void CreateItem(object? item)
#else
            void CreateItem(object item)
#endif
            {
                ListEditorItem alei = CreateListItem(item, count);
                ((IList)baseObject).Add(item);
                count++;
                selList.Items.Add(alei);
            }
        }
    }
}
