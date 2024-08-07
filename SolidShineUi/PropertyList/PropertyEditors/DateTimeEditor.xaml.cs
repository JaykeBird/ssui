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
    /// A property editor for <see cref="DateTime"/> values, as well as <c>TimeOnly</c> values for .NET 6 and higher.
    /// </summary>
    public partial class DateTimeEditor : UserControl, IPropertyEditor
    {
        /// <summary>
        /// Create an DateTimeEditor.
        /// </summary>
        public DateTimeEditor()
        {
            InitializeComponent();
        }

#if NET6_0_OR_GREATER
        /// <inheritdoc/>
        public List<Type> ValidTypes => new List<Type> { typeof(DateTime), typeof(DateOnly) };
#else
        /// <inheritdoc/>
        public List<Type> ValidTypes => (new[] { typeof(DateTime) }).ToList();
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
            get => picker.IsEnabled;
            set
            {
                spinner.IsEnabled = !dateOnlyValue && value;
                btnMenu.IsEnabled = value;
                picker.IsEnabled = value;
            }
        }

        Type _propType = typeof(DateTime);
        bool dateOnlyValue = false;
        bool _internalAction = false;

#if NETCOREAPP

        /// <inheritdoc/>
        public event EventHandler? ValueChanged;
        
        /// <inheritdoc/>
        public object? GetValue()
        {
            if (_propType == typeof(DateTime))
            {
                if (picker.SelectedDate == null) return null;
                else
                {
#if NET6_0_OR_GREATER
                    return DateOnly.FromDateTime(picker.SelectedDate.Value).ToDateTime(spinner.GetValueAsTimeOnly());
#else   
                    return picker.SelectedDate.Value.Date.Add(spinner.Value);
#endif
                }
            }
#if NET6_0_OR_GREATER
            else if (_propType == typeof(DateOnly))
            {
                if (picker.SelectedDate == null) return null;
                else return DateOnly.FromDateTime(picker.SelectedDate.Value);
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
            if (value is DateOnly da)
            {
                spinner.IsEnabled = false;
                dateOnlyValue = true;
            }
            else if (value is DateTime dt)
            {
                spinner.Value = dt.TimeOfDay;
            }
#else
            if (value is DateTime dt)
            {
                spinner.Value = dt.TimeOfDay;
            }
#endif
        }
#else

        /// <inheritdoc/>
        public event EventHandler ValueChanged;

        /// <inheritdoc/>
        public object GetValue()
        {
            return picker.SelectedDate.Value.Date.Add(spinner.Value);
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
            _internalAction = true;
            mnuSetNull.IsEnabled = true;
            mnuSetNull.IsChecked = true;
            picker.SelectedDate = null;
            spinner.IsEnabled = false;
            _internalAction = false;
        }

        void UnsetAsNull()
        {
            _internalAction = true;
            mnuSetNull.IsChecked = false;
            picker.SelectedDate = DateTime.Today;
            spinner.IsEnabled = !dateOnlyValue;
            _internalAction = false;
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
            DateTime dt = DateTime.Now;
            picker.SelectedDate = dt.Date;
            spinner.Value = dt.TimeOfDay;
        }

        private void picker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_internalAction) return;

            if (picker.SelectedDate == null)
            {
                SetAsNull();
            }
            else
            {
                mnuSetNull.IsChecked = false;
                spinner.IsEnabled = !dateOnlyValue;
            }
        }
    }
}
