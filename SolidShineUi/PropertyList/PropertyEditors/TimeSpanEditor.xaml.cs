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
    /// A property editor for <see cref="TimeSpan"/> values, as well as <c>TimeOnly</c> values for .NET 6 and higher.
    /// </summary>
    public partial class TimeSpanEditor : UserControl, IPropertyEditor
    {
        /// <summary>
        /// Create an TimeSpanEditor.
        /// </summary>
        public TimeSpanEditor()
        {
            InitializeComponent();
        }

#if NET6_0_OR_GREATER
        /// <inheritdoc/>
        public List<Type> ValidTypes => new List<Type> { typeof(TimeSpan), typeof(TimeOnly) };
#else
        /// <inheritdoc/>
        public List<Type> ValidTypes => (new[] { typeof(TimeSpan) }).ToList();
#endif

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public ExperimentalPropertyList ParentPropertyList { set { } }

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
            spinner.ColorScheme = cs;
            btnMenu.ColorScheme = cs;
            imgMenu.Source = Utils.IconLoader.LoadIcon("ThreeDots", cs);
        }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        /// <inheritdoc/>
        public bool IsPropertyWritable
        {
            get => spinner.IsEnabled;
            set
            {
                spinner.IsEnabled = value;
                btnMenu.IsEnabled = value;
            }
        }

        Type _propType = typeof(TimeSpan);

#if NETCOREAPP
        
        /// <inheritdoc/>
        public event EventHandler? ValueChanged;
        
        /// <inheritdoc/>
        public object? GetValue()
        {
            if (_propType == typeof(TimeSpan))
            {
                return spinner.Value;
            }
#if NET6_0_OR_GREATER
            else if (_propType == typeof(TimeOnly))
            {
                return spinner.GetValueAsTimeOnly();
            }
#endif
            else
            {
                return spinner.Value;
            }
        }
        
        /// <inheritdoc/>
        public void LoadValue(object? value, Type type)
        {
            _propType = type;

            if (value == null)
            {
                SetAsNull();
            }

            LoadUi();

#if NET6_0_OR_GREATER
            if (value is TimeOnly ti)
            {
                spinner.SetValueFromTimeOnly(ti);
            }
            else
            {
                spinner.Value = (TimeSpan)(value ?? TimeSpan.Zero);
            }
#else
            spinner.Value = (TimeSpan)(value ?? TimeSpan.Zero);
#endif
        }
#else

        /// <inheritdoc/>
        public event EventHandler ValueChanged;

        /// <inheritdoc/>
        public object GetValue()
        {
            if (_propType == typeof(TimeSpan))
            {
                return spinner.Value;
            }
            else
            {
                return spinner.Value;
            }
        }

        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
        {
            _propType = type;

            if (value == null)
            {
                SetAsNull();
            }

            LoadUi();
            
            spinner.Value = (TimeSpan)(value ?? TimeSpan.Zero);
        }
#endif

        void LoadUi()
        {
#if NET6_0_OR_GREATER
            if (_propType == typeof(TimeSpan))
            {
                SetMaxMin(TimeSpan.MaxValue, TimeSpan.MinValue);
            }
            else if (_propType == typeof(TimeOnly))
            {
                SetMaxMin(TimeOnly.MinValue.ToTimeSpan(), TimeOnly.MaxValue.ToTimeSpan());
            }
#else
            SetMaxMin(TimeSpan.MaxValue, TimeSpan.MinValue);
#endif

            void SetMaxMin(TimeSpan max, TimeSpan min)
            {
                spinner.MaxValue = max;
                spinner.MinValue = min;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private void spinner_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        void SetAsNull()
        {
            mnuSetNull.IsEnabled = true;
            mnuSetNull.IsChecked = true;
            spinner.IsEnabled = false;
        }

        void UnsetAsNull()
        {
            mnuSetNull.IsChecked = false;
            spinner.IsEnabled = true;
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
            if (spinner.Use12HourClock)
            {
                spinner.Use12HourClock = false;
                mnuDisplayClock.IsChecked = false;
            }
            else
            {
                spinner.Use12HourClock = true;
                mnuDisplayClock.IsChecked = true;
            }
        }

        private void mnuMidnight_Click(object sender, RoutedEventArgs e)
        {
            spinner.Value = new TimeSpan(0, 0, 0);
        }

        private void mnuNoon_Click(object sender, RoutedEventArgs e)
        {
            spinner.Value = new TimeSpan(12, 0, 0);
        }

        private void mnuCurrent_Click(object sender, RoutedEventArgs e)
        {
            spinner.Value = DateTime.Now.TimeOfDay;
        }
    }
}
