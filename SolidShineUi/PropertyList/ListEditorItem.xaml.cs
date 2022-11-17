using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using SolidShineUi.Utils;
using static SolidShineUi.PropertyList.PropertyEditorItem;

namespace SolidShineUi.PropertyList
{
    /// <summary>
    /// An entry in a list or collection, for display in the <see cref="Dialogs.ListEditorDialog"/>, and use in a <see cref="SelectPanel"/>.
    /// </summary>
    public partial class ListEditorItem : SelectableUserControl
    {
        /// <summary>
        /// Create a ListEditorItem.
        /// </summary>
        public ListEditorItem()
        {
            InitializeComponent();
            InternalVisibleControlsChanged += editor_InternalVisibleControlsChanged;
        }

        /// <summary>
        /// When overridden by a derived class, this method is automatically called each time the color scheme is updated by the parent SelectPanel. Use this to update child controls.
        /// </summary>
        /// <param name="cs">The new color scheme.</param>
        public override void ApplyColorScheme(ColorScheme cs)
        {
            btnMoveDn.ApplyColorScheme(cs);
            btnMoveUp.ApplyColorScheme(cs);
            btnRemove.ApplyColorScheme(cs);

            imgMoveDn.Source = IconLoader.LoadIcon("Down", cs);
            imgMoveUp.Source = IconLoader.LoadIcon("Up", cs);
            imgRemove.Source = IconLoader.LoadIcon("Exit", cs);

            base.ApplyColorScheme(cs);
        }

        #region Properties / Events

        /// <summary>
        /// Get or set the index of this item in the larger collection or list.
        /// </summary>
        public int ItemIndex { get => (int)GetValue(ItemIndexProperty); set => SetValue(ItemIndexProperty, value); }

        /// <summary>
        /// Get or set the controls that visible for this ListEditorItem.
        /// </summary>
        /// <remarks>
        /// "None" should be used for a basic IEnumerable type, "RemoveOnly" should be used for an ICollection type, and "All" should be used for an IList type.
        /// </remarks>
        public ListEditorItemControls VisibleControls
        { 
            get => (ListEditorItemControls)GetValue(VisibleControlsProperty);
            set => SetValue(VisibleControlsProperty, value);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static DependencyProperty ItemIndexProperty
            = DependencyProperty.Register("ItemIndex", typeof(int), typeof(ListEditorItem),
            new FrameworkPropertyMetadata(0));

        public static DependencyProperty VisibleControlsProperty
            = DependencyProperty.Register("VisibleControls", typeof(ListEditorItemControls), typeof(ListEditorItem),
            new FrameworkPropertyMetadata(ListEditorItemControls.None, new PropertyChangedCallback(InternalOnVisibleControlsChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        Type objectType = typeof(object);

        private static void InternalOnVisibleControlsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListEditorItem i)
            {
                i.InternalVisibleControlsChanged?.Invoke(d, e);
            }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalVisibleControlsChanged;

        /// <summary>
        /// Raised when the VisibleControls property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? VisibleControlsChanged;
#else
        public event DependencyPropertyChangedEventHandler VisibleControlsChanged;
#endif

        private void editor_InternalVisibleControlsChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            switch (VisibleControls)
            {
                case ListEditorItemControls.None:
                    stkControls.Visibility = Visibility.Collapsed;
                    break;
                case ListEditorItemControls.RemoveOnly:
                    stkControls.Visibility = Visibility.Visible;
                    btnMoveDn.Visibility = Visibility.Collapsed;
                    btnMoveUp.Visibility = Visibility.Collapsed;
                    break;
                case ListEditorItemControls.All:
                    stkControls.Visibility = Visibility.Visible;
                    btnMoveDn.Visibility = Visibility.Visible;
                    btnMoveUp.Visibility = Visibility.Visible;
                    break;
                default:
                    stkControls.Visibility = Visibility.Collapsed;
                    break;
            }
            VisibleControlsChanged?.Invoke(sender, e);
        }

#if NETCOREAPP

        object? _value = null;

        /// <summary>
        /// Get or set the value of the property shown.
        /// </summary>
        public object? ItemValue { get => _value; set { _value = value; txtValue.Text = (value ?? "(null)").ToString(); } }

        private object? _oldValue = null;

        /// <summary>
        /// Get or set the editor control to use to allow editing the value of this property.
        /// </summary>
        public IPropertyEditor? PropertyEditorControl { get; set; }

        /// <summary>
        /// Raised when the value of this property is changed, by use of a <see cref="PropertyEditorControl"/>.
        /// </summary>
        public event PropertyEditorValueChangedEventHandler? PropertyEditorValueChanged;

        /// <summary>
        /// Raised when the Remove button is clicked, indicating the user wants to remove this item from the list.
        /// </summary>
        public event EventHandler? RequestRemove;
        /// <summary>
        /// Raised when the Move Up button is clicked, indicating the user wants to move this item up in the list.
        /// </summary>
        public event EventHandler? RequestMoveUp;
        /// <summary>
        /// Raised when the Move Down button is clicked, indicating the user wants to move this item down in the list.
        /// </summary>
        public event EventHandler? RequestMoveDown;
#else

        object _value = null;

        /// <summary>
        /// Get or set the value of the property shown.
        /// </summary>
        public object ItemValue { get => _value; set { _value = value; txtValue.Text = (value ?? "(null)").ToString(); } }

        private object _oldValue = null;

        /// <summary>
        /// Get or set the editor control to use to allow editing the value of this property.
        /// </summary>
        public IPropertyEditor PropertyEditorControl { get; set; }

        /// <summary>
        /// Raised when the value of this property is changed, by use of a <see cref="PropertyEditorControl"/>.
        /// </summary>
        public event PropertyEditorValueChangedEventHandler PropertyEditorValueChanged;

        /// <summary>
        /// Raised when the Remove button is clicked, indicating the user wants to remove this item from the list.
        /// </summary>
        public event EventHandler RequestRemove;
        /// <summary>
        /// Raised when the Move Up button is clicked, indicating the user wants to move this item up in the list.
        /// </summary>
        public event EventHandler RequestMoveUp;
        /// <summary>
        /// Raised when the Move Down button is clicked, indicating the user wants to move this item down in the list.
        /// </summary>
        public event EventHandler RequestMoveDown;
#endif
#endregion

        /// <summary>
        /// Load in a property to show in this PropertyEditorItem, with (if possible) a IPropertyEditor control to allow editing the property value.
        /// </summary>
        /// <param name="value">The value of the property, in regards to the object being observed in the parent <see cref="ExperimentalPropertyList"/>.</param>
        /// <param name="type">The type of the item being passed in.</param>
        /// <param name="editor">The IPropertyEditor control, if present, that is useable for editing the value of this property.</param>
#if NETCOREAPP
        public void LoadItem(object? value, Type type, IPropertyEditor? editor)
#else
        public void LoadItem(object value, Type type, IPropertyEditor editor)
#endif
        {
            ItemValue = value;
            if (editor != null)
            {
                PropertyEditorControl = editor;
                editor.LoadValue(value, type);
                txtValue.Visibility = Visibility.Collapsed;
                FrameworkElement uie = PropertyEditorControl.GetFrameworkElement();
                PropertyEditorControl.ValueChanged += PropertyEditorControl_ValueChanged;
                uie.VerticalAlignment = VerticalAlignment.Stretch;
                uie.Width = double.NaN; // Auto
                grdValue.Children.Add(uie);
            }
        }

#if NETCOREAPP
        private void PropertyEditorControl_ValueChanged(object? sender, EventArgs e)
#else
        private void PropertyEditorControl_ValueChanged(object sender, EventArgs e)
#endif
        {
            if (PropertyEditorControl != null)
            {
                _oldValue = ItemValue;
                ItemValue = PropertyEditorControl.GetValue();
                var ev = new PropertyEditorValueChangedEventArgs(_oldValue, ItemValue, "", null);
                PropertyEditorValueChanged?.Invoke(this, ev);

                if (ev.ChangeFailed)
                {
                    ItemValue = ev.FailedChangePropertyValue;
                    
                    PropertyEditorControl.LoadValue(ItemValue, objectType);
                }
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            RequestRemove?.Invoke(this, e);
        }

        private void btnMoveDn_Click(object sender, RoutedEventArgs e)
        {
            RequestMoveDown?.Invoke(this, e);
        }

        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            RequestMoveUp?.Invoke(this, e);
        }
    }

    /// <summary>
    /// Represents which controls are available for a ListEditorItem.
    /// </summary>
    public enum ListEditorItemControls
    {
        /// <summary>
        /// No controls are shown.
        /// </summary>
        None = 0,
        /// <summary>
        /// Only the "Remove" control is shown.
        /// </summary>
        RemoveOnly = 1,
        /// <summary>
        /// Both the "Remove" and movement controls are shown.
        /// </summary>
        All = 2,
    }
}
