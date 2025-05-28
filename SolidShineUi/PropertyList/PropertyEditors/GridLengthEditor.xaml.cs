using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using static SolidShineUi.Utils.IconLoader;
using SolidShineUi.PropertyList.Dialogs;
using SolidShineUi.Utils;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A proeprty editor for editing <see cref="GridLength"/> objects.
    /// </summary>
    public partial class GridLengthEditor : UserControl, IPropertyEditor
    {
        /// <summary>Create a GridLengthEditor.</summary>
        public GridLengthEditor()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public List<Type> ValidTypes => (new[] { typeof(GridLength), typeof(GridLength?) }).ToList();

        bool _internalAction = false;

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public void SetHostControl(IPropertyEditorHost host) { /* _host = host; */ }

        //ColorScheme _cs = new ColorScheme();

        /// <inheritdoc/>
        public ColorScheme ColorScheme
        {
            set
            {
                ApplyColorScheme(value);
            }
        }

        /// <inheritdoc/>
        public void ApplyColorScheme(ColorScheme cs)
        {
            //_cs = value;
            nudValue.ColorScheme = cs;
            btnMenu.ColorScheme = cs;
            imgMenu.Source = LoadIcon("ThreeDots", cs);
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
                cbbType.IsEnabled = value && !nullSet;
                nudValue.IsEnabled = value && !nullSet;
            }
        }

        bool nullSet = false;

        void SetAsNull()
        {
            nullSet = true;
            cbbType.IsEnabled = false;
            nudValue.IsEnabled = false;
            mnuSetNull.IsEnabled = true;
            mnuSetNull.IsChecked = true;
        }

        void UnsetAsNull()
        {
            // do not set as null
            nullSet = false;
            mnuSetNull.IsChecked = false;
            cbbType.IsEnabled = true;
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
                return new GridLength(nudValue.Value, cbbType.SelectedEnumValueAsEnum<GridUnitType>());
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
            //_itemType = type;

            GridLength? gl;// = null;

            if (type == typeof(GridLength))
            {
                gl = (GridLength?)value;
            }
            else if (type == typeof(GridLength?))
            {
                gl = (GridLength?)value;
                mnuSetNull.IsEnabled = true;
            }
            else
            {
                // this isn't a GridLength
                gl = null;
            }

            if (gl == null)
            {
                SetAsNull();
            }
            else
            {
                cbbType.SelectedEnumValue = gl.Value.GridUnitType;
                nudValue.Value = gl.Value.Value;
            }

            _internalAction = false;
        }

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0060 // Remove unused parameter
        private void nudValue_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_internalAction) return;
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void cbbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_internalAction) return;
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE0060 // Remove unused parameter
    }
}
