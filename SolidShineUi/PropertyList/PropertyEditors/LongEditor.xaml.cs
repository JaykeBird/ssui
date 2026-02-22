using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A property editor for <see cref="long"/>, <see cref="uint"/>, and <see cref="ulong"/> types.
    /// </summary>
    public partial class LongEditor : UserControl, IPropertyEditor
    {
        /// <summary>
        /// Create an IntegerEditor.
        /// </summary>
        public LongEditor()
        {
            InitializeComponent();

            // load in string values
            mnuCopy.Header = Strings.CopyFullValue;
            mnuDisplayHex.Header = Strings.DisplayAsHex;
            mnuSetNull.Header = Strings.SetAsNull;

            btnEnableEdit.Content = Strings.DecreaseAndEdit;
        }

        /// <inheritdoc/>
        public List<Type> ValidTypes => (new[] { typeof(long), typeof(uint), typeof(ulong), 
            typeof(long?), typeof(uint?), typeof(ulong?) }).ToList();

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public void SetHostControl(IPropertyEditorHost host) { /* _host = host; */ }

        /// <inheritdoc/>
        public void ApplySsuiTheme(SsuiTheme theme)
        {
            intSpinner.SsuiTheme = theme;
            btnMenu.SsuiTheme = theme;
            imgMenu.Source = IconLoader.LoadIcon("ThreeDots", theme.IconVariation);
            btnEnableEdit.SsuiTheme = theme;
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
            set { intSpinner.IsEnabled = value; btnMenu.IsEnabled = value; }
        }

        Type _propType = typeof(long);

        bool ulong2long = false;
        ulong ulongbig = 0L;

        bool _internalAction = false;

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
            if (_propType == typeof(long))
            {
                return intSpinner.Value;
            }
            else if (_propType == typeof(uint))
            {
                return (uint)intSpinner.Value;
            }
            else if (_propType == typeof(ulong))
            {
                if (ulong2long)
                {
                    return ulongbig;
                }
                else
                {
                    return (ulong)intSpinner.Value;
                }
            }
            else if (_propType == typeof(long?))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else
                {
                    return intSpinner.Value;
                }
            }
            else if (_propType == typeof(uint?))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else
                {
                    return (uint)intSpinner.Value;
                }
            }
            else if (_propType == typeof(ulong?))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else
                {
                    if (ulong2long)
                    {
                        return ulongbig;
                    }
                    else
                    {
                        return (ulong)intSpinner.Value;
                    }
                }
            }
            else
            {
                return intSpinner.Value;
            }
        }

#if NETCOREAPP
        /// <inheritdoc/>
        public void LoadValue(object? value, Type type)
#else
        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
#endif
        {
            _propType = type;

            if (value == null)
            {
                SetAsNull();
            }

            LoadUi();

            _internalAction = true;
            if (value is ulong ul && ul > long.MaxValue)
            {
                // the value is larger than can be displayed as a long; thus, I'll just display the text and offer a way to edit from there
                ulong2long = true;
                ulongbig = ul;
                intSpinner.Value = 0L;

                txtUlongLong.Text = ul.ToString(NumberFormatInfo.CurrentInfo);
                txtUlongLong.ToolTip = ul.ToString("D", NumberFormatInfo.CurrentInfo);

                btnEnableEdit.Visibility = Visibility.Visible;
                txtUlongLong.Visibility = Visibility.Visible;
                mnuDisplayHex.IsEnabled = false;
                intSpinner.Visibility = Visibility.Collapsed;
            }
            else
            {
                intSpinner.Value = (long)(value ?? 0);
            }
            _internalAction = false;
        }

        void LoadUi()
        {
            if (_propType == typeof(long))
            {
                SetMinMax(long.MinValue, long.MaxValue);
            }
            else if (_propType == typeof(uint))
            {
                SetMinMax(uint.MinValue, uint.MaxValue);
            }
            else if (_propType == typeof(ulong))
            {
                SetMinMax(0, long.MaxValue);
            }
            else if (_propType == typeof(long?))
            {
                SetMinMax(long.MinValue, long.MaxValue);
                mnuSetNull.IsEnabled = true;
            }
            else if (_propType == typeof(uint?))
            {
                SetMinMax(uint.MinValue, uint.MaxValue);
                mnuSetNull.IsEnabled = true;
            }
            else if (_propType == typeof(ulong?))
            {
                SetMinMax(0, long.MaxValue);
                mnuSetNull.IsEnabled = true;
            }

            void SetMinMax(long min, long max)
            {
                intSpinner.MaxValue = max;
                intSpinner.MinValue = min;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private void intSpinner_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_internalAction) return;
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        void SetAsNull()
        {
            mnuSetNull.IsEnabled = true;
            mnuSetNull.IsChecked = true;
            intSpinner.IsEnabled = false;
        }

        void UnsetAsNull()
        {
            mnuSetNull.IsChecked = false;
            intSpinner.IsEnabled = true;
        }

        private void mnuSetNull_Click(object sender, RoutedEventArgs e)
        {
            if (mnuSetNull.IsChecked)
            {
                UnsetAsNull();
            }
            else
            {
                SetAsNull();
            }
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void mnuDisplayHex_Click(object sender, RoutedEventArgs e)
        {
            if (intSpinner.DisplayAsHex)
            {
                intSpinner.DisplayAsHex = false;
                mnuDisplayHex.IsChecked = false;
            }
            else
            {
                intSpinner.DisplayAsHex = true;
                mnuDisplayHex.IsChecked = true;
            }
        }

        private void btnEnableEdit_Click(object sender, RoutedEventArgs e)
        {
            ulong2long = false;
            btnEnableEdit.Visibility = Visibility.Collapsed;
            txtUlongLong.Visibility = Visibility.Collapsed;
            mnuDisplayHex.IsEnabled = true;
            intSpinner.Visibility = Visibility.Visible;
            intSpinner.Value = long.MaxValue;
        }

        private void mnuCopy_Click(object sender, RoutedEventArgs e)
        {
            if (ulong2long)
            {
                Clipboard.SetText(ulongbig.ToString("D", NumberFormatInfo.CurrentInfo));
            }
            else
            {
                if (intSpinner.DisplayAsHex)
                {
                    Clipboard.SetText(intSpinner.Value.ToString("X", NumberFormatInfo.CurrentInfo));
                }
                else
                {
                    Clipboard.SetText(intSpinner.Value.ToString("D", NumberFormatInfo.CurrentInfo));
                }
            }
        }
    }
}
