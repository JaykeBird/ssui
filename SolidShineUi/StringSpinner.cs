using SolidShineUi.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace SolidShineUi
{

    /// <summary>
    /// A control that allows you to select a string from a larger list, by using up and down buttons.
    /// </summary>
    /// <remarks>
    /// This is built on the same logic as <see cref="IntegerSpinner"/> or <see cref="DoubleSpinner"/>, but for the purpose of having a list of strings that can be selected from.
    /// This is inspired by the <c>DomainUpDown</c> control in Windows Forms. If considering this control, please also compare this to the experience with WPF's <c>ComboBox</c>
    /// to see what will be the better user experience; this control has the limitation where users aren't able to see the full list of items at one time.
    /// </remarks>
    [DefaultEvent(nameof(ValueChanged))]
    [ContentProperty(nameof(Items))]
    [Localizability(LocalizationCategory.ComboBox)]
    public class StringSpinner : SpinnerBase
    {
        static StringSpinner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StringSpinner), new FrameworkPropertyMetadata(typeof(StringSpinner)));
        }

        /// <summary>
        /// Create a StringSpinner.
        /// </summary>
        public StringSpinner()
        {
            // set up ValidateValue to run whenever these properties are updated (Value)
            AddPropertyChangedTrigger(ValueProperty, typeof(StringSpinner));

            CommandBindings.Add(new CommandBinding(StepUp, (o, e) => DoStepUp(), (o, e) => e.CanExecute = !IsAtMaxValue));
            CommandBindings.Add(new CommandBinding(StepDown, (o, e) => DoStepDown(), (o, e) => e.CanExecute = !IsAtMinValue));

            Items = new ObservableCollection<string>();
            Items.CollectionChanged += Items_CollectionChanged;
        }

        /// <summary>
        /// Get the string value that is currently selected in this control.
        /// </summary>
        /// <remarks>
        /// Use <see cref="SelectedIndex"/> to select a different string programmatically, like so: <c>mySpinner.SelectedIndex = mySpinner.Items.IndexOf("my string");</c>
        /// </remarks>
        [ReadOnly(true)]
        [Category("Common")]
        [Description("Get the string value that is currently selected in this control.")]
        public string Value { get => (string)GetValue(ValueProperty); private set => SetValue(ValuePropertyKey, value); }

        private static readonly DependencyPropertyKey ValuePropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(Value), typeof(string), typeof(StringSpinner),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, (d, e) => d.PerformAs<StringSpinner>((s) => s.OnValueChanged(s, e))));

        /// <summary>The backing dependency property for <see cref="Value"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ValueProperty = ValuePropertyKey.DependencyProperty;


        #region Base StringSpinner Properties

        /// <summary>
        /// Get the list of string values to select from in this StringSpinner control. Add items to this list via <c>Items.Add("string")</c>.
        /// </summary>
        [Category("Common")]
        [Description("Get the list of string values to select from in this StringSpinner control.")]
        public ObservableCollection<string> Items { get => (ObservableCollection<string>)GetValue(ItemsProperty); private set => SetValue(ItemsPropertyKey, value); }

        private static readonly DependencyPropertyKey ItemsPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(Items), typeof(ObservableCollection<string>), typeof(StringSpinner),
            new FrameworkPropertyMetadata());

        /// <summary>The backing dependency property for <see cref="Items"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

        private int _minValue = 0;
        private int _selected = 0;

        /// <summary>
        /// Get the highest index value that is allowed to be set via <see cref="SelectedIndex"/> in this control. This will be equal to <c>Items.Count - 1</c>.
        /// </summary>
        [ReadOnly(true)]
        [Category("Common")]
        [Description("Get the highest index value that is allowed to be set via \"SelectedIndex\" in this control.")]
        public int MaxValue { get => (int)GetValue(MaxValueProperty); private set => SetValue(MaxValuePropertyKey, value); }

        private static readonly DependencyPropertyKey MaxValuePropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(MaxValue), typeof(int), typeof(StringSpinner),
            new FrameworkPropertyMetadata(0, (d, e) => d.PerformAs<StringSpinner>((o) => o.RaiseMaxValueChanged(o, e))));

        /// <summary>The backing dependency property for <see cref="MaxValue"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MaxValueProperty = MaxValuePropertyKey.DependencyProperty;

        /// <summary>
        /// Get or set the index of the selected string in this control. This value must be between 0 and <see cref="MaxValue"/>, inclusive.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">thrown if the value inputted is below 0 or above MaxValue</exception>
        [Category("Common")]
        [Description("Get or set the index of the selected string in this control.")]
        public int SelectedIndex
        {
            get => _selected;
            set
            {
                if (value > MaxValue || value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), 
                        "This value is not within the allowed range of values. Value cannot be less than 0, or greater than MaxValue.");
                }
                else
                {
                    _selected = value;
                    Value = Items[value];
                }
            }
        }

#if NETCOREAPP
        private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#else
        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#endif
        {
            ValidateValue();
            if (Items.Count <= 0)
            {
                MaxValue = 0;
                _selected = 0;
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

        private void OnValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateValue(e);
        }

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

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            LoadTemplateItems();
            UpdateUI(); // this will make sure the text box is primed with the current Value
        }

        bool itemsLoaded = false;

#if NETCOREAPP
        TextBox? txtValue = null;
#else
        TextBox txtValue = null;
#endif

        void LoadTemplateItems()
        {
            if (!itemsLoaded)
            {
                txtValue = (TextBox)GetTemplateChild("PART_Text");

                if (txtValue != null)
                {
                    txtValue.TextChanged += txtValue_TextChanged;
                    txtValue.LostFocus += txtValue_LostFocus;
                    txtValue.KeyDown += txtValue_KeyDown;
                    txtValue.KeyUp += txtValue_KeyUp;

                    itemsLoaded = true;
                }
            }
        }
        #endregion

        #region TextBox
        private void txtValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            // raised when a new value is typed into the textbox
            // Value is updated as the text is being typed in (if it can be parsed), and full validation occurs once the text box loses focus or the user presses the Enter key

            if (txtValue == null)
            {
                return;
            }

            _updateBox = false;
            if (Items.Contains(txtValue.Text))
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

        private void txtValue_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateValue();
        }

        private void txtValue_KeyDown(object sender, KeyEventArgs e)
        {
            TextBoxKeyDown(e);
        }

        private void txtValue_KeyUp(object sender, KeyEventArgs e)
        {
            TextBoxKeyUp(e);
        }

        /// <inheritdoc/>
        protected override void ValidateValue()
        {
            if (Items.Contains(Value))
            {
                _selected = Items.IndexOf(Value);

                IsAtMinValue = SelectedIndex == _minValue;
                IsAtMaxValue = SelectedIndex == MaxValue;
            }
            else if (Items.Count == 0)
            {
                _selected = 0;
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
        protected override void UpdateValue(DependencyPropertyChangedEventArgs e)
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
    }
}
