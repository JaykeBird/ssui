using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;

namespace SolidShineUi.Ribbon
{
    /// <summary>
    /// A combo-box to display in a <see cref="RibbonGroup"/>.
    /// </summary>
    [ContentProperty("Items")]
    public class RibbonComboBox : RibbonContentControl
    {
        private ComboBox comboBox;

        /// <summary>
        /// Create a RibbonComboBox.
        /// </summary>
        public RibbonComboBox()
        {
            comboBox = new ComboBox();
            Content = comboBox;
            BaseComboBox = comboBox;

            SetupBinding();
        }

        void SetupBinding()
        {
            // ComboBox
            SetBinding(IsDropDownOpenProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(IsDropDownOpen)) });
            SetBinding(IsEditableProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(IsEditable)) });
            SetBinding(IsReadOnlyProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(IsReadOnly)) });
            SetBinding(MaxDropDownHeightProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(MaxDropDownHeight)) });
            SetBinding(ShouldPreserveUserEnteredPrefixProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(ShouldPreserveUserEnteredPrefix)) });
            SetBinding(StaysOpenOnEditProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(StaysOpenOnEdit)) });
            SetBinding(TextProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(Text)) });
            // Selector
            SetBinding(IsSynchronizedWithCurrentItemProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(IsSynchronizedWithCurrentItem)) });
            SetBinding(SelectedIndexProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(SelectedIndex)) });
            SetBinding(SelectedItemProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(SelectedItem)) });
            SetBinding(SelectedValueProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(SelectedValue)) });
            SetBinding(SelectedValuePathProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(SelectedValuePath)) });
            // ItemsControl
            SetBinding(AlternationCountProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(AlternationCount)) });
            SetBinding(DisplayMemberPathProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(DisplayMemberPath)) });
            SetBinding(GroupStyleSelectorProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(GroupStyleSelector)) });
            SetBinding(IsTextSearchCaseSensitiveProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(IsTextSearchCaseSensitive)) });
            SetBinding(IsTextSearchEnabledProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(IsTextSearchEnabled)) });
            SetBinding(ItemBindingGroupProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(ItemBindingGroup)) });
            SetBinding(ItemContainerStyleProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(ItemContainerStyle)) });
            SetBinding(ItemContainerStyleSelectorProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(ItemContainerStyleSelector)) });
            SetBinding(ItemsPanelProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(ItemsPanel)) });
            SetBinding(ItemsSourceProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(ItemsSource)) });
            SetBinding(ItemStringFormatProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(ItemStringFormat)) });
            SetBinding(ItemTemplateProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(ItemTemplate)) });
            SetBinding(ItemTemplateSelectorProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(ItemTemplateSelector)) });
            // Other
            SetBinding(ComboBoxWidthProperty,
                new Binding() { Source = comboBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(Width)) });

            // Event
            comboBox.SelectionChanged += (s, e) =>
            {
                e.Source = this; // mask this as the source of the event (the original combobox can still be found via OriginalSource)
                SelectionChangedEventArgs se =
                    new SelectionChangedEventArgs(SelectionChangedEvent, e.RemovedItems, e.AddedItems);
                RaiseEvent(se);
            };
        }

        #region Properties

        /// <summary>
        /// Get the base combo-box item in this control.
        /// </summary>
        /// <remarks>
        /// This is the actual <see cref="ComboBox"/> that is displayed in this <see cref="RibbonComboBox"/>.
        /// All properties exposed in the RibbonComboBox are bound directly to this base ComboBox.
        /// </remarks>
        public ComboBox BaseComboBox { get => (ComboBox)GetValue(BaseComboBoxProperty); private set => SetValue(BaseComboBoxPropertyKey, value); }

        private static readonly DependencyPropertyKey BaseComboBoxPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(BaseComboBox), typeof(ComboBox), typeof(RibbonComboBox), new FrameworkPropertyMetadata());

        /// <summary>The backing dependency property for <see cref="ComboBox"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BaseComboBoxProperty = BaseComboBoxPropertyKey.DependencyProperty;

        // expose the properties from the base ComboBox as its this class's own properties
        // then, we can bind to these with binding in the SetupControl method

        #region ComboBox properties

        /// <summary>
        /// Gets or sets a value that indicates whether the drop-down for a combo box is currently open.
        /// </summary>
        public bool IsDropDownOpen { get => (bool)GetValue(IsDropDownOpenProperty); set => SetValue(IsDropDownOpenProperty, value); }

        /// <summary>The backing dependency property for <see cref="IsDropDownOpen"/>. See the related property for details.</summary>
        public static readonly DependencyProperty IsDropDownOpenProperty
            = ComboBox.IsDropDownOpenProperty.AddOwner(typeof(RibbonComboBox), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value that enables or disables editing of the text in text box of the <see cref="ComboBox"/>.
        /// </summary>
        public bool IsEditable { get => (bool)GetValue(IsEditableProperty); set => SetValue(IsEditableProperty, value); }

        /// <summary>The backing dependency property for <see cref="IsEditable"/>. See the related property for details.</summary>
        public static readonly DependencyProperty IsEditableProperty
            = ComboBox.IsEditableProperty.AddOwner(typeof(RibbonComboBox), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value that enables selection-only mode, in which the contents of the combo box are selectable
        /// but not editable.
        /// </summary>
        public bool IsReadOnly { get => (bool)GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }

        /// <summary>The backing dependency property for <see cref="IsReadOnly"/>. See the related property for details.</summary>
        public static readonly DependencyProperty IsReadOnlyProperty
            = ComboBox.IsReadOnlyProperty.AddOwner(typeof(RibbonComboBox), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets the maximum height for a combo box drop-down.
        /// </summary>
        public double MaxDropDownHeight { get => (double)GetValue(MaxDropDownHeightProperty); set => SetValue(MaxDropDownHeightProperty, value); }

        /// <summary>The backing dependency property for <see cref="MaxDropDownHeight"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MaxDropDownHeightProperty
            = ComboBox.MaxDropDownHeightProperty.AddOwner(typeof(RibbonComboBox), new FrameworkPropertyMetadata(double.NaN));

        /// <summary>
        /// Gets or sets a value that indicates whether the ComboBox keeps the user's input or replaces the input
        /// with a matching item.
        /// </summary>
        public bool ShouldPreserveUserEnteredPrefix { get => (bool)GetValue(ShouldPreserveUserEnteredPrefixProperty); set => SetValue(ShouldPreserveUserEnteredPrefixProperty, value); }

        /// <summary>The backing dependency property for <see cref="ShouldPreserveUserEnteredPrefix"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ShouldPreserveUserEnteredPrefixProperty
            = ComboBox.ShouldPreserveUserEnteredPrefixProperty.AddOwner(typeof(RibbonComboBox), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether a ComboBox that is open and displays a drop-down control will remain open when a user
        /// clicks the <see cref="TextBox"/>.
        /// </summary>
        public bool StaysOpenOnEdit { get => (bool)GetValue(StaysOpenOnEditProperty); set => SetValue(StaysOpenOnEditProperty, value); }

        /// <summary>The backing dependency property for <see cref="StaysOpenOnEdit"/>. See the related property for details.</summary>
        public static readonly DependencyProperty StaysOpenOnEditProperty
            = ComboBox.StaysOpenOnEditProperty.AddOwner(typeof(RibbonComboBox), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets the text of the currently selected item.
        /// </summary>
        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        /// <summary>The backing dependency property for <see cref="Text"/>. See the related property for details.</summary>
        public static readonly DependencyProperty TextProperty
            = ComboBox.TextProperty.AddOwner(typeof(RibbonComboBox), new FrameworkPropertyMetadata(""));

        #endregion

        #region Selector properties
        
        /// <summary>
        /// Gets or sets a value that indicates whether a ComboBox should keep the <see cref="SelectedItem"/> synchronized with
        /// the current item in the <see cref="Items"/> property.
        /// </summary>
        public bool? IsSynchronizedWithCurrentItem { get => (bool?)GetValue(IsSynchronizedWithCurrentItemProperty); set => SetValue(IsSynchronizedWithCurrentItemProperty, value); }

        /// <summary>The backing dependency property for <see cref="IsSynchronizedWithCurrentItem"/>. See the related property for details.</summary>
        public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty
            = Selector.IsSynchronizedWithCurrentItemProperty.AddOwner(typeof(RibbonComboBox), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the index of the first item in the current selection or returns negative one (-1) if the selection is empty.
        /// </summary>
        public int SelectedIndex { get => (int)GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectedIndex"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedIndexProperty
            = Selector.SelectedIndexProperty.AddOwner(typeof(RibbonComboBox), new FrameworkPropertyMetadata(-1));

        /// <summary>
        /// Gets or sets the first item in the current selection or returns null if the selection is empty.
        /// </summary>
        public object SelectedItem { get => (object)GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectedItem"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedItemProperty
            = Selector.SelectedItemProperty.AddOwner(typeof(RibbonComboBox));

        /// <summary>
        /// Gets or sets the value of the <see cref="SelectedItem"/>, obtained by using <see cref="SelectedValuePath"/>.
        /// </summary>
        public object SelectedValue { get => (object)GetValue(SelectedValueProperty); set => SetValue(SelectedValueProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectedValue"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedValueProperty
            = Selector.SelectedValueProperty.AddOwner(typeof(RibbonComboBox));

        /// <summary>
        /// Gets or sets the path that is used to get the <see cref="SelectedValue"/> from the <see cref="SelectedItem"/>.
        /// </summary>
        public string SelectedValuePath { get => (string)GetValue(SelectedValuePathProperty); set => SetValue(SelectedValuePathProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectedValuePath"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedValuePathProperty
            = Selector.SelectedValuePathProperty.AddOwner(typeof(RibbonComboBox), new FrameworkPropertyMetadata(""));

        #endregion

        #region ItemControl properties

        /// <summary>
        /// Gets or sets the number of alternating item containers in the combo-box, which enables alternating containers
        /// to have a unique appearance.
        /// </summary>
        public int AlternationCount { get => (int)GetValue(AlternationCountProperty); set => SetValue(AlternationCountProperty, value); }

        /// <summary>The backing dependency property for <see cref="AlternationCount"/>. See the related property for details.</summary>
        public static readonly DependencyProperty AlternationCountProperty
            = ItemsControl.AlternationCountProperty.AddOwner(typeof(RibbonComboBox));

        /// <summary>
        /// Gets or sets a path to a value on the source object to serve as the visual representation of the object.
        /// </summary>
        public string DisplayMemberPath { get => (string)GetValue(DisplayMemberPathProperty); set => SetValue(DisplayMemberPathProperty, value); }

        /// <summary>The backing dependency property for <see cref="DisplayMemberPath"/>. See the related property for details.</summary>
        public static readonly DependencyProperty DisplayMemberPathProperty
            = ItemsControl.DisplayMemberPathProperty.AddOwner(typeof(RibbonComboBox), new FrameworkPropertyMetadata(""));

        /// <summary>
        /// Gets or sets a method that enables you to provide custom selection logic for a GroupStyle to apply to each group in a collection.
        /// </summary>
        public GroupStyleSelector GroupStyleSelector { get => (GroupStyleSelector)GetValue(GroupStyleSelectorProperty); set => SetValue(GroupStyleSelectorProperty, value); }

        /// <summary>The backing dependency property for <see cref="GroupStyleSelector"/>. See the related property for details.</summary>
        public static readonly DependencyProperty GroupStyleSelectorProperty
            = ItemsControl.GroupStyleSelectorProperty.AddOwner(typeof(RibbonComboBox));

        /// <summary>
        /// Gets or sets a value that indicates whether <see cref="TextSearch"/> is enabled on this combo-box.
        /// </summary>
        public bool IsTextSearchEnabled { get => (bool)GetValue(IsTextSearchEnabledProperty); set => SetValue(IsTextSearchEnabledProperty, value); }

        /// <summary>The backing dependency property for <see cref="IsTextSearchEnabled"/>. See the related property for details.</summary>
        public static readonly DependencyProperty IsTextSearchEnabledProperty
            = ItemsControl.IsTextSearchEnabledProperty.AddOwner(typeof(RibbonComboBox), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value that indicates whether case is a condition when searching for items.
        /// </summary>
        public bool IsTextSearchCaseSensitive { get => (bool)GetValue(IsTextSearchCaseSensitiveProperty); set => SetValue(IsTextSearchCaseSensitiveProperty, value); }

        /// <summary>The backing dependency property for <see cref="IsTextSearchCaseSensitive"/>. See the related property for details.</summary>
        public static readonly DependencyProperty IsTextSearchCaseSensitiveProperty
            = ItemsControl.IsTextSearchCaseSensitiveProperty.AddOwner(typeof(RibbonComboBox));

        /// <summary>
        /// Gets or sets the <see cref="BindingGroup"/> that is copied to each item in the combo-box.
        /// </summary>
        public BindingGroup ItemBindingGroup { get => (BindingGroup)GetValue(ItemBindingGroupProperty); set => SetValue(ItemBindingGroupProperty, value); }

        /// <summary>The backing dependency property for <see cref="ItemBindingGroup"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ItemBindingGroupProperty
            = ItemsControl.ItemBindingGroupProperty.AddOwner(typeof(RibbonComboBox));

        /// <summary>
        /// Gets or sets the <see cref="Style"/> that is applied to the cintainer element generated for each item in this combo-box.
        /// </summary>
        public Style ItemContainerStyle { get => (Style)GetValue(ItemContainerStyleProperty); set => SetValue(ItemContainerStyleProperty, value); }

        /// <summary>The backing dependency property for <see cref="ItemContainerStyle"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ItemContainerStyleProperty
            = ItemsControl.ItemContainerStyleProperty.AddOwner(typeof(RibbonComboBox), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets custom style-selection logic for a style that can be applied to each generated container element.
        /// </summary>
        public StyleSelector ItemContainerStyleSelector { get => (StyleSelector)GetValue(ItemContainerStyleSelectorProperty); set => SetValue(ItemContainerStyleSelectorProperty, value); }

        /// <summary>The backing dependency property for <see cref="ItemContainerStyleSelector"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ItemContainerStyleSelectorProperty
            = ItemsControl.ItemContainerStyleSelectorProperty.AddOwner(typeof(RibbonComboBox), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the template that defines the panel that controls the layout of items.
        /// </summary>
        public ItemsPanelTemplate ItemsPanel { get => (ItemsPanelTemplate)GetValue(ItemsPanelProperty); set => SetValue(ItemsPanelProperty, value); }

        /// <summary>The backing dependency property for <see cref="ItemsPanel"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ItemsPanelProperty
            = ItemsControl.ItemsPanelProperty.AddOwner(typeof(RibbonComboBox));

        /// <summary>
        /// Gets or sets a collection used to generate the content of the combo-box.
        /// </summary>
        public IEnumerable ItemsSource { get => (IEnumerable)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

        /// <summary>The backing dependency property for <see cref="ItemsSource"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ItemsSourceProperty
            = ItemsControl.ItemsSourceProperty.AddOwner(typeof(RibbonComboBox), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets a compsite string that specifies how to format the items in the combo-box if they are displayed as strings.
        /// </summary>
        public string ItemStringFormat { get => (string)GetValue(ItemStringFormatProperty); set => SetValue(ItemStringFormatProperty, value); }

        /// <summary>The backing dependency property for <see cref="ItemStringFormat"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ItemStringFormatProperty
            = ItemsControl.ItemStringFormatProperty.AddOwner(typeof(RibbonComboBox));

        /// <summary>
        /// Get or sets the <see cref="DataTemplate"/> used to display each item.
        /// </summary>
        public DataTemplate ItemTemplate { get => (DataTemplate)GetValue(ItemTemplateProperty); set => SetValue(ItemTemplateProperty, value); }

        /// <summary>The backing dependency property for <see cref="ItemTemplate"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ItemTemplateProperty
            = ItemsControl.ItemTemplateProperty.AddOwner(typeof(RibbonComboBox), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the custom logic for choosing a template used to display each item.
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector { get => (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); set => SetValue(ItemTemplateSelectorProperty, value); }

        /// <summary>The backing dependency property for <see cref="ItemTemplateSelector"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty
            = ItemsControl.ItemTemplateSelectorProperty.AddOwner(typeof(RibbonComboBox), new FrameworkPropertyMetadata(null));

        #endregion

        /// <summary>
        /// Gets or sets the width of the base combo-box.
        /// </summary>
        public double ComboBoxWidth { get => (double)GetValue(ComboBoxWidthProperty); set => SetValue(ComboBoxWidthProperty, value); }

        /// <summary>The backing dependency property for <see cref="ComboBoxWidth"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ComboBoxWidthProperty
            = DependencyProperty.Register(nameof(ComboBoxWidth), typeof(double), typeof(RibbonComboBox),
            new FrameworkPropertyMetadata(double.NaN));

        #region Other Get-only Properties

        /// <summary>
        /// Gets the collection used to generate the contents of the combo-box.
        /// </summary>
        public ItemCollection Items { get => comboBox.Items; }

        /// <summary>
        /// Gets the item that is displayed in the selection box.
        /// </summary>
        public object SelectionBoxItem { get => comboBox.SelectionBoxItem; }

        /// <summary>
        /// Gets a composite string that specifies how to format the selected item in the selection box if it is displayed as a string.
        /// </summary>
        public string SelectionBoxItemStringFormat { get => comboBox.SelectionBoxItemStringFormat; }

        /// <summary>
        /// Gets the item template of the selection box content.
        /// </summary>
        public DataTemplate SelectionBoxItemTemplate { get => comboBox.SelectionBoxItemTemplate; }

        /// <summary>
        /// Gets the actual rendered width of the base combo-box.
        /// </summary>
        public double ComboBoxActualWidth { get => comboBox.ActualWidth; }

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the selection of the combo-box changes.
        /// </summary>
        public event System.Windows.Controls.SelectionChangedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        /// <summary>The backing routed event for <see cref="SelectionChanged"/>. See the related event for details.</summary>
        public static RoutedEvent SelectionChangedEvent = Selector.SelectionChangedEvent.AddOwner(typeof(RibbonComboBox));

        #endregion
    }
}
