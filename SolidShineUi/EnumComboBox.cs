using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SolidShineUi
{
    /// <summary>
    /// A ComboBox where the items are populated from an Enum.
    /// </summary>
    public class EnumComboBox : ComboBox
    {

        /// <summary>
        /// Create an EnumComboBox.
        /// </summary>
        public EnumComboBox()
        {

        }

        #region EnumProperty


        /// <summary>The backing dependency property for <see cref="Enum"/>. See the related property for details.</summary>
        public static readonly DependencyProperty EnumProperty = DependencyProperty.Register(
            "Enum", typeof(Type), typeof(EnumComboBox),
            new PropertyMetadata(null, new PropertyChangedCallback(OnInternalEnumChanged)));

        /// <summary>The backing routed event for <see cref="EnumChanged"/>. See the related event for details.</summary>
        public static readonly RoutedEvent EnumChangedEvent = EventManager.RegisterRoutedEvent(
            "EnumChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EnumComboBox));

        /// <summary>
        /// Raised when the <see cref="Enum"/> property is changed.
        /// </summary>
        public event RoutedEventHandler EnumChanged
        {
            add { AddHandler(EnumChangedEvent, value); }
            remove { RemoveHandler(EnumChangedEvent, value); }
        }

        private static void OnInternalEnumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EnumComboBox s)
            {
                s.OnEnumChanged(e);
            }
        }

        private void OnEnumChanged(DependencyPropertyChangedEventArgs _)
        {
            if (Enum.BaseType != typeof(Enum))
            {
                throw new ArgumentException("Type '" + Enum.FullName + "' is not an enumerator.");
            }

            Items.Clear();

            foreach (string item in System.Enum.GetNames(Enum))
            {
                Items.Add(item);
            }

            SelectedIndex = 0;

            RoutedEventArgs re = new RoutedEventArgs(EnumChangedEvent);
            RaiseEvent(re);
        }

        /// <summary>
        /// The enum to use with this EnumComboBox. When the Enum value is set, the ComboBox's items are changed to match the different values of the enum. 
        /// </summary>
        public Type Enum
        {
            get => (Type)GetValue(EnumProperty);
            set => SetValue(EnumProperty, value);
        }

        #endregion

        /// <summary>
        /// Get or set the selected item of the EnumComboBox. To avoid casting when getting this value, use the SelectedEnumValueAsEnum function.
        /// </summary>
        public object SelectedEnumValue
        {
            get
            {
                return System.Enum.Parse(Enum, SelectedItem.ToString() ?? "");
            }
            set
            {
                if (System.Enum.IsDefined(Enum, value))
                {
                    SelectedItem = value.ToString();
                }
            }
        }

        /// <summary>
        /// Get the selected item of the EnumComboBox, returned as a value of that enum.
        /// </summary>
        /// <typeparam name="T">the enum type to get the item as</typeparam>
        /// <exception cref="ArgumentException">Thrown if the selected item is not contained in <typeparamref name="T"/></exception>
        public T SelectedEnumValueAsEnum<T>()
        {
            if (System.Enum.IsDefined(typeof(T), SelectedItem))
            {
                return (T)System.Enum.Parse(typeof(T), SelectedItem.ToString() ?? "");
            }
            else
            {
                throw new ArgumentException("Type 'T' is not an enumerator that contains this item.");
            }
        }

    }
}
