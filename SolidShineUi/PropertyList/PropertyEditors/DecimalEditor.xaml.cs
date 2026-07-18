using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using SolidShineUi.Utils;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A property editor for <see cref="decimal"/> objects.
    /// </summary>
    public partial class DecimalEditor : UserControl, IPropertyEditor
    {
        /// <summary>
        /// Create a new DecimalEditor.
        /// </summary>
        public DecimalEditor()
        {
            InitializeComponent();

            // load in string values
            mnuCopy.Header = Strings.CopyFullValue;
            mnuSetValue.Header = Strings.SetExactValue;
            mnuSetNull.Header = Strings.SetAsNull;
        }

        /// <inheritdoc/>
        public List<Type> ValidTypes => (new[] { typeof(decimal), typeof(decimal?) }).ToList();

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public void SetHostControl(IPropertyEditorHost host) { _host = host; }

#if NETCOREAPP
        private IPropertyEditorHost? _host = null;
#else
        private IPropertyEditorHost _host = null;
#endif

        /// <inheritdoc/>
        public void ApplySsuiTheme(SsuiTheme theme)
        {
            dblSpinner.SsuiTheme = theme;
            btnMenu.SsuiTheme = theme;
            imgMenu.Source = IconLoader.LoadIcon("ThreeDots", theme.IconVariation);
        }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        /// <inheritdoc/>
        public bool IsPropertyWritable
        {
            get => dblSpinner.IsEnabled;
            set => dblSpinner.IsEnabled = value;
        }

        Type _propType = typeof(decimal);
        bool _internalAction = false;
        decimal _internalValue = 0M;

        /// <inheritdoc/>
#if NETCOREAPP
        public event EventHandler? ValueChanged;

        /// <inheritdoc/>
        public object? GetValue()
#else
        public event EventHandler ValueChanged;

        /// <inheritdoc/>
        public object GetValue()
#endif
        {
#if NET5_0_OR_GREATER
            // being able to return two different types via a conditioner operator is only allowed in .NET 5 or later
            if (_propType == typeof(decimal?))
            {
                return mnuSetNull.IsChecked ? null : _internalValue;
            }
#else
            if (_propType == typeof(decimal?))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else
                {
                    return _internalValue;
                }
            }
#endif
            else
            {
                return _internalValue;
            }
        }

        /// <inheritdoc/>
#if NETCOREAPP
        public void LoadValue(object? value, Type type)
#else
        public void LoadValue(object value, Type type)
#endif
        {
            if (type == typeof(decimal?))
            {
                mnuSetNull.IsEnabled = true;
            }

            if (value == null)
            {
                SetAsNull();
            }
            else
            {
                UnsetAsNull();
            }

            _propType = type;
            _internalAction = true;
            if (value is decimal d)
            {
                _internalValue = d;
                dblSpinner.Value = (double)d;
            }
            else
            {
                _internalValue = 0m;
                dblSpinner.Value = 0d;
            }
            _internalAction = false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private void dblSpinner_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_internalAction) return;
            _internalValue = (decimal)dblSpinner.Value;
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        void SetAsNull()
        {
            mnuSetNull.IsEnabled = true;
            mnuSetNull.IsChecked = true;
            dblSpinner.IsEnabled = false;
        }

        void UnsetAsNull()
        {
            mnuSetNull.IsChecked = false;
            dblSpinner.IsEnabled = true;
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

        private void mnuSetValue_Click(object sender, RoutedEventArgs e)
        {
            StringInputDialog sid = new StringInputDialog("Set Decimal", "Enter in the exact decimal value to use:", _internalValue.ToString(provider: null));
            sid.SsuiTheme = _host?.GetThemeForDialogs() ?? SsuiThemes.SystemTheme;
            sid.Owner = _host?.GetWindow();

            sid.ValidationFunction = (s) => { return decimal.TryParse(s, out _); };
            sid.ValidationFailureString = "Not a valid decimal value";

            sid.ShowDialog();
            if (sid.DialogResult)
            {
                _internalValue = decimal.Parse(sid.Value, null);
                _internalAction = true;
                dblSpinner.Value = (double)_internalValue;
                _internalAction = false;
            }
        }

        private void mnuCopy_Click(object sender, RoutedEventArgs e)
        {
            // https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#general-format-specifier-g
            // "However, if the number is a Decimal and the precision specifier is omitted, fixed-point notation is always used and trailing zeros are preserved."
            Clipboard.SetText(_internalValue.ToString("G", null));
        }
    }
}
