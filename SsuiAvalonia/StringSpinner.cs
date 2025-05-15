using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi
{

    /// <summary>
    /// A control that allows you to select a string by using up and down buttons.
    /// </summary>
    /// <remarks>
    /// This is built on the same logic as <see cref="IntegerSpinner"/> or <see cref="DoubleSpinner"/>, but for the purpose of having a list of strings that can be selected from.
    /// This is inspired by the <c>DomainUpDown</c> control in Windows Forms. If considering this control, please also compare this to the experience with the built-in <c>ComboBox</c>
    /// to see what will be the better user experience; this control has the limitation where users aren't able to see the full list of items at one time.
    /// </remarks>
    public class StringSpinner : SpinnerBase, IAddChild
    {

        /// <summary>
        /// Create a StringSpinner.
        /// </summary>
        public StringSpinner()
        {
            AddPropertyChangedTrigger(ValueProperty);

            _items = new ObservableCollection<string>();
            _items.CollectionChanged += Items_CollectionChanged;
        }

        private int _minValue = 0;

        #region Properties

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            switch (change.Property.Name)
            {
                case nameof(Value):
                    OnValueChanged(change);
                    break;
            }
        }

        #region Value

        private string _value = "";

        /// <summary>
        /// Get the string value that is currently selected in this control.
        /// </summary>
        /// <remarks>
        /// Use <see cref="SelectedIndex"/> to select a different string programmatically, like so: <c>mySpinner.SelectedIndex = mySpinner.Items.IndexOf("my string");</c>
        /// </remarks>
        public string Value { get => _value; private set => SetAndRaise(ValueProperty, ref _value, value); }

        /// <summary>The backing direct property for <see cref="Value"/>. See the related property for details.</summary>
        public static readonly DirectProperty<StringSpinner, string> ValueProperty
            = AvaloniaProperty.RegisterDirect<StringSpinner, string>(nameof(Value), (s) => s.Value, unsetValue: "");

        private void OnValueChanged(AvaloniaPropertyChangedEventArgs e)
        {
            UpdateValue(e);
        }

        #endregion

        #region Items

        private ObservableCollection<string> _items;

        /// <summary>
        /// Get the list of string values to select from in this StringSpinner control. Add items to this list via <c>Items.Add("string")</c>.
        /// </summary>
        [Content]
        public ObservableCollection<string> Items { get => _items; private set => SetAndRaise(ItemsProperty, ref _items, value); }

        /// <summary>The backing direct property for <see cref="Items"/>. See the related property for details.</summary>
        public static readonly DirectProperty<StringSpinner, ObservableCollection<string>> ItemsProperty
            = AvaloniaProperty.RegisterDirect<StringSpinner, ObservableCollection<string>>(nameof(Items), (s) => s.Items);


        private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ValidateValue();
            if (Items.Count <= 0)
            {
                MaxValue = 0;
                SelectedIndex = 0;
                Value = "";
            }
            else
            {
                MaxValue = Items.Count - 1;

                if (_selected > MaxValue)
                {
                    SelectedIndex = MaxValue;
                }
            }
        }

        #endregion

        #region MaxValue

        private int _maxValue = 0;

        /// <summary>
        /// Get the highest index value that is allowed to be set via <see cref="SelectedIndex"/> in this control. This will be equal to <c>Items.Count - 1</c>.
        /// </summary>
        public int MaxValue { get => _maxValue; private set => SetAndRaise(MaxValueProperty, ref _maxValue, value); }

        /// <summary>The backing direct property for <see cref="MaxValue"/>. See the related property for details.</summary>
        public static readonly DirectProperty<StringSpinner, int> MaxValueProperty
            = AvaloniaProperty.RegisterDirect<StringSpinner, int>(nameof(MaxValue), (s) => s.MaxValue, unsetValue: 0);

        #endregion

        #region SelectedIndex

        private int _selected = 0;

        /// <summary>
        /// Get or set the index of the selected string in this control. This index must be between 0 and <see cref="MaxValue"/>, inclusive.
        /// </summary>
        public int SelectedIndex
        {
            get => _selected;
            set
            {
                if (value > MaxValue || value < 0)
                {
                    throw new IndexOutOfRangeException("This value is not within the allowed range of values. Value cannot be less than 0, or greater than MaxValue.");
                }
                else
                {
                    SetAndRaise(SelectedIndexProperty, ref _selected, value);
                    Value = Items[value];
                }
            }
        }

        /// <summary>The backing direct property for <see cref="SelectedIndex"/>. See the related property for details.</summary>
        public static readonly DirectProperty<StringSpinner, int> SelectedIndexProperty
            = AvaloniaProperty.RegisterDirect<StringSpinner, int>(nameof(SelectedIndex), (s) => s.SelectedIndex, (s, v) => s.SelectedIndex = v, unsetValue: 0);

        #endregion

        #endregion

        /// <inheritdoc/>
        protected override void DoStepDown()
        {
            if (_selected > _minValue)
            {
                SelectedIndex--;
            }
        }

        /// <inheritdoc/>
        protected override void DoStepUp()
        {
            if (_selected < MaxValue)
            {
                SelectedIndex++;
            }
        }

        #region Template IO

        TextBox? txtValue;
        FlatRepeatButton? btnStepUp;
        FlatRepeatButton? btnStepDown;

        bool itemsLoaded = false;

        /// <inheritdoc/>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            LoadTemplateItems(e);
        }


        void LoadTemplateItems(TemplateAppliedEventArgs e)
        {
            if (!itemsLoaded)
            {
                txtValue = (TextBox?)e.NameScope.Find("PART_Text");

                if (txtValue != null)
                {
                    txtValue.TextChanged += txtValue_TextChanged;
                    txtValue.LostFocus += txtValue_LostFocus;
                    txtValue.KeyDown += txtValue_KeyDown;
                    txtValue.KeyUp += txtValue_KeyUp;

                    itemsLoaded = true;
                }

                // Avalonia doesn't include a command system out of the box (usually I think most people rely upon Reactive), so for now, I'll need to call these events this way
                btnStepUp = (FlatRepeatButton?)e.NameScope.Find("PART_btnStepUp");
                btnStepDown = (FlatRepeatButton?)e.NameScope.Find("PART_btnStepDown");

                if (btnStepUp != null) { btnStepUp.Execute += (s, e) => DoStepUp(); }
                if (btnStepDown != null) { btnStepDown.Execute += (s, e) => DoStepDown(); }
            }
        }

        #endregion

        #region TextBox
        private void txtValue_TextChanged(object? sender, TextChangedEventArgs e)
        {
            // raised when a new value is typed into the textbox
            // Value is updated as the text is being typed in (if it can be parsed), and full validation occurs once the text box loses focus or the user presses the Enter key

            if (txtValue == null)
            {
                return;
            }

            _updateBox = false;
            if (txtValue.Text == null)
            {
                Value = "";
            }
            else if (Items.Contains(txtValue.Text))
            {
                Value = txtValue.Text;
            }
            else if (Items.Count == 0)
            {
                Value = "";
            }
            else
            {
                // this is not valid
                //currently do nothing
            }
            _updateBox = true;
        }

        private void txtValue_LostFocus(object? sender, RoutedEventArgs e)
        {
            ValidateValue();
        }

        private void txtValue_KeyDown(object? sender, KeyEventArgs e)
        {
            TextBoxKeyDown(e);
        }

        private void txtValue_KeyUp(object? sender, KeyEventArgs e)
        {
            TextBoxKeyUp(e);
        }

        /// <inheritdoc/>
        protected override void ValidateValue()
        {
            if (Items.Contains(Value))
            {
                SelectedIndex = Items.IndexOf(Value);

                IsAtMinValue = SelectedIndex == _minValue;
                IsAtMaxValue = SelectedIndex == MaxValue;
            }
            else if (Items.Count == 0)
            {
                SelectedIndex = 0;
                IsAtMinValue = true;
                IsAtMaxValue = true;
            }
            else
            {
                Value = Items[0];
                return;
            }

            base.ValidateValue();
        }

        /// <inheritdoc/>
        protected override void UpdateValue(AvaloniaPropertyChangedEventArgs e)
        {
            //int value = Value;

            if (!advanceTimer.Enabled)
            {
                ValidateValue();
            }
            base.UpdateValue(e);
        }

        /// <inheritdoc/>
        protected override void UpdateUI()
        {
            if (txtValue == null) return; // this is not good, as it means that the template didn't apply, or the applied template's text box won't get the updated value

            if (txtValue.Text != Value)
            {
                if (_updateBox) txtValue.Text = Value;
            }

            base.UpdateUI();
        }

        #endregion

        #region IAddChild implementation

        /// <inheritdoc/>
        public void AddChild(object child)
        {
            if (child != null && child.ToString() != null)
            {
                Items.Add(child.ToString() ?? "");
            }
        }

        #endregion
    }
}
