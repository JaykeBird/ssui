using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SolidShineUi;
using SolidShineUi.PropertyList.Dialogs;
using SolidShineUi.Utils;
using static SolidShineUi.Utils.IconLoader;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A proeprty editor for editing <see cref="Color"/> objects.
    /// </summary>
    public partial class ColorEditor : UserControl, IPropertyEditor
    {
        /// <summary>Create a ColorEditor.</summary>
        public ColorEditor()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public List<Type> ValidTypes => (new[] { typeof(Color), typeof(Color?) }).ToList();

        bool _internalAction = false;

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public ExperimentalPropertyList ParentPropertyList { set { _parentPropertyList = value; } }

        ColorScheme _cs = new ColorScheme();
#if NETCOREAPP
        ExperimentalPropertyList? _parentPropertyList = null;
#else
        ExperimentalPropertyList _parentPropertyList = null;
#endif
        Color _col = Colors.White;

        /// <summary>
        /// Set the visual appearance of this control via the ColorScheme.
        /// </summary>
        /// <param name="value">the color scheme to apply</param>
        public void ApplyColorScheme(ColorScheme value)
        {
            _cs = value;
            nudValue.ColorScheme = value;
            btnMenu.ColorScheme = value;
            btnColor.ColorScheme = value;
            imgMenu.Source = LoadIcon("ThreeDots", value);
        }

        /// <inheritdoc/>
        public ColorScheme ColorScheme
        {
            set
            {
                ApplyColorScheme(value);
            }
        }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        /// <inheritdoc/>
        public bool IsPropertyWritable
        {
            get => btnMenu.IsEnabled;
            set 
            { 
                btnMenu.IsEnabled = value;
                nudValue.IsEnabled = value && !nullSet;
            }
        }

        bool nullSet = false;

        void SetAsNull()
        {
            nullSet = true;
            nudValue.IsEnabled = false;
            mnuSetNull.IsEnabled = true;
            mnuSetNull.IsChecked = true;
        }

        void UnsetAsNull()
        {
            // do not set as null
            nullSet = false;
            mnuSetNull.IsChecked = false;
            nudValue.IsEnabled = true;
        }

        private void mnuSetNull_Click(object sender, RoutedEventArgs e)
        {
            _internalAction = true;
            if (mnuSetNull.IsChecked)
            {
                UnsetAsNull();
            }
            else
            {
                // do set as null
                SetAsNull();
            }
            _internalAction = false;
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

#if NETCOREAPP
        /// <inheritdoc/>
        public event EventHandler? ValueChanged;

        /// <inheritdoc/>
        public object? GetValue()
#else
        /// <inheritdoc/>
        public event EventHandler ValueChanged;
        
        /// <inheritdoc/>
        public object GetValue()
#endif
        {
            if (nullSet)
            {
                return null;
            }
            else
            {
                return _col;
            }
        }

        /// <inheritdoc/>
#if NETCOREAPP
        public void LoadValue(object? value, Type type)
#else
        public void LoadValue(object value, Type type)
#endif
        {
            _internalAction = true;

            Color? col;// = null;

            // get the value and store it in a temporary value
            if (type == typeof(Color))
            {
                col = (Color?)value;
            }
            else if (type == typeof(Color?))
            {
                col = (Color?)value;
                mnuSetNull.IsEnabled = true;
            }
            else
            {
                // this isn't a Color
                col = null;
            }

            // now, let's update the UI
            if (col == null)
            {
                SetAsNull();
            }
            else
            {
                Color ccol = col.Value;

                string hex = ccol.GetHexStringWithAlpha();
                nudValue.Value = int.Parse(hex, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo);
                brdrColor.Background = new SolidColorBrush(ccol);
            }

            _internalAction = false;
        }

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0060 // Remove unused parameter
        private void nudValue_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_internalAction) return;

            _col = ColorsHelper.CreateFromHex(nudValue.Value.ToString("X8", NumberFormatInfo.CurrentInfo));
            brdrColor.Background = new SolidColorBrush(_col);

            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0051 // Remove unused private members

        private void cbbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_internalAction) return;
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void mnuColor_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cpd = new ColorPickerDialog(_cs, _col);
            if (_parentPropertyList != null)
            {
                cpd.Owner = Window.GetWindow(_parentPropertyList);
            }

            cpd.ShowDialog();

            if (cpd.DialogResult)
            {
                _internalAction = true;
                _col = cpd.SelectedColor;
                nudValue.Value = int.Parse(_col.GetHexStringWithAlpha(), NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo);
                brdrColor.Background = new SolidColorBrush(_col);
                _internalAction = false;
            }
        }
    }
}
