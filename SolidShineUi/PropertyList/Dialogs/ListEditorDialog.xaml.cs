using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using SolidShineUi;
using System.Windows.Controls;
using System.Reflection;

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
        Type? baseType = null;

        Type? editorType = null;

        ExperimentalPropertyList? parentList = null;
#else
        Type baseType = null;

        Type editorType = null;

        ExperimentalPropertyList parentList = null;
#endif

        int count = -1;

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
            new FrameworkPropertyMetadata("Full Colletion May Not Be Available"));

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

            if (baseType.GetConstructor(Type.EmptyTypes) == null)
            {
                btnAdd.IsEnabled = false;
                txtCannotAdd.Visibility = Visibility.Visible;
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
            
            if (!isList)
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

        ListEditorItem CreateListItem(object item, int index)
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
                        selList.Items.Remove(lei);
                        count--;
                    }
                }
            };

            lei.PropertyEditorValueChanged += (s, e) =>
            {
                if (isList && canEdit)
                {
                    ValueChanged(e.NewValue, item);
                }
                else
                {
                    e.ChangeFailed = true;
                    e.FailedChangePropertyValue = item;
                }
            };

            return lei;
        }

#if NETCOREAPP
        void ValueChanged(object? newValue, object baseItem)
#else
        void ValueChanged(object newValue, object baseItem)
#endif
        {
            if (baseObject is IList icol)
            {
                int index = icol.IndexOf(baseItem);
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
                var newItem = Activator.CreateInstance(baseType);
                if (newItem != null)
                {
                    ListEditorItem lei = CreateListItem(newItem, count);
                    ((IList)baseObject).Add(newItem);
                    count++;
                    selList.Items.Add(lei);
                }
                else
                {
                    btnAdd.IsEnabled = false;
                }
            }
            else
            {
                btnAdd.IsEnabled = false;
            }
        }
    }
}
