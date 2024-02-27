using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Media;

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
        }

        /// <inheritdoc/>
        public List<Type> ValidTypes => (new[] { typeof(decimal), typeof(decimal?) }).ToList();

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public ExperimentalPropertyList ParentPropertyList { set { _parent = value; } }

        private ColorScheme _cs = new ColorScheme();
#if NETCOREAPP
        private ExperimentalPropertyList? _parent = null;
#else
        private ExperimentalPropertyList _parent = null;
#endif

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
            _cs = cs;
            dblSpinner.ColorScheme = cs;
            btnMenu.ColorScheme = cs;
            if (cs.BackgroundColor == Colors.Black || cs.ForegroundColor == Colors.White)
            {
                imgMenu.Source = new BitmapImage(new Uri("/SolidShineUi;component/Images/ThreeDotsWhite.png", UriKind.Relative));
            }
            else if (cs.BackgroundColor == Colors.White)
            {
                imgMenu.Source = new BitmapImage(new Uri("/SolidShineUi;component/Images/ThreeDotsBlack.png", UriKind.Relative));
            }
            else
            {
                imgMenu.Source = new BitmapImage(new Uri("/SolidShineUi;component/Images/ThreeDotsColor.png", UriKind.Relative));
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
            StringInputDialog sid = new StringInputDialog(_cs, "Set Decimal", "Enter in the exact decimal value to use:", _internalValue.ToString());
            sid.ValidationFunction = (s) => { return decimal.TryParse(s, out _); };
            sid.ValidationFailureString = "Not a valid decimal value";
            if (_parent != null) sid.Owner = Window.GetWindow(_parent);
            sid.ShowDialog();
            if (sid.DialogResult)
            {
                _internalValue = decimal.Parse(sid.Value);
                _internalAction = true;
                dblSpinner.Value = (double)_internalValue;
                _internalAction = false;
            }
        }

        private void mnuCopy_Click(object sender, RoutedEventArgs e)
        {
            // https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#general-format-specifier-g
            // "However, if the number is a Decimal and the precision specifier is omitted, fixed-point notation is always used and trailing zeros are preserved."
            Clipboard.SetText(_internalValue.ToString("G"));
        }
    }
}
