using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using static SolidShineUi.Utils.IconLoader;
using SolidShineUi;
using SolidShineUi.PropertyList.Dialogs;
using SolidShineUi.Utils;
using System.Text;
using System.IO;
using System.Diagnostics;

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

        /// <inheritdoc/>
        public ColorScheme ColorScheme
        {
            set
            {
                _cs = value;
                nudValue.ColorScheme = value;
                btnMenu.ColorScheme = value;
                btnColor.ColorScheme = value;
                imgMenu.Source = LoadIcon("ThreeDots", value);
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

            Color? col = null;

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

            if (col == null)
            {
                SetAsNull();
            }
            else
            {
                Color ccol = col.Value;
                
                nudValue.Value = int.Parse(ccol.GetHexStringWithAlpha(), System.Globalization.NumberStyles.HexNumber);
            }

            _internalAction = false;
        }

        private void nudValue_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_internalAction) return;

            _col = ColorsHelper.CreateFromHex(nudValue.Value.ToString("X8"));
            brdrColor.Background = new SolidColorBrush(_col);

            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

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
                nudValue.Value = int.Parse(_col.GetHexStringWithAlpha(), System.Globalization.NumberStyles.HexNumber);
                brdrColor.Background = new SolidColorBrush(_col);
                _internalAction = false;
            }
        }
    }
}
