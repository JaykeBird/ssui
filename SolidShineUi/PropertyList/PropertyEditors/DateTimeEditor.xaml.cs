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
        public List<Type> ValidTypes => new List<Type> { typeof(DateTime), typeof(DateOnly), typeof(DateTime?), typeof(DateOnly?) };
#else
        /// <inheritdoc/>
        public List<Type> ValidTypes => (new[] { typeof(DateTime), typeof(DateTime?) }).ToList();
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
                spinner.IsEnabled = value;
                btnMenu.IsEnabled = value;
                picker.IsEnabled = value;
            }
        }

        Type _propType = typeof(DateTime);
        bool _internalAction = false;
        bool _nullAllowed = false;

#if NETCOREAPP

        /// <inheritdoc/>
        public event EventHandler? ValueChanged;

        /// <inheritdoc/>
        public object? GetValue()
        {
#if NET6_0_OR_GREATER
            if (_propType == typeof(DateOnly) || _propType == typeof(DateOnly?))
            {
                if (picker.SelectedDate == null) return null;
                else return DateOnly.FromDateTime(picker.SelectedDate.Value);
            }
            else
            {
                if (picker.SelectedDate == null) return null;
                else
                {
                    return picker.SelectedDate.Value.Date.Add(spinner.Value);
                }
            }
#else
            if (picker.SelectedDate == null) return null;
            else
            {
                return picker.SelectedDate.Value.Date.Add(spinner.Value);
            }
#endif
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

            if (_propType == typeof(DateTime?))
            {
                DateTime? dtn = (DateTime?)value ?? null;
                if (dtn.HasValue)
                {
                    spinner.Value = dtn.Value.TimeOfDay;
                    picker.SelectedDate = dtn.Value.Date;
                    AllowNulls();
                }
                else
                {
                    SetAsNull();
                }
            }
#if NET6_0_OR_GREATER
            else if (_propType == typeof(DateOnly?))
            {
                DateOnly? dan = (DateOnly?)value ?? null;
                spinner.Visibility = Visibility.Collapsed;
                colTime.Width = new GridLength(0);

                if (dan.HasValue)
                {
                    AllowNulls();
                    picker.SelectedDate = dan.Value.ToDateTime(new TimeOnly(0, 0, 0));
                }
                else
                {
                    SetAsNull();
                }
            }
            else if (value is DateOnly da)
            {
                spinner.Visibility = Visibility.Collapsed;
                colTime.Width = new GridLength(0);

                picker.SelectedDate = da.ToDateTime(new TimeOnly(0, 0, 0));
            }
#endif
            else if (value is DateTime dt)
            {
                spinner.Value = dt.TimeOfDay;
                picker.SelectedDate = dt.Date;
            }
            else
            {
                SetAsNull();
            }
        }
#else

        /// <inheritdoc/>
        public event EventHandler ValueChanged;

        /// <inheritdoc/>
        public object GetValue()
        {
            if (picker.SelectedDate == null) return null;
            else
            {
                return picker.SelectedDate.Value.Date.Add(spinner.Value);
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

            if (value is DateTime dt)
            {
                spinner.Value = dt.TimeOfDay;
                picker.SelectedDate = dt.Date;
            }
            else if (value is DateTime?)
            {
                DateTime? dtn = (DateTime?)value ?? null;
                if (dtn.HasValue)
                {
                    spinner.Value = dtn.Value.TimeOfDay;
                    picker.SelectedDate = dtn.Value.Date;
                    AllowNulls();
                }
                else
                {
                    SetAsNull();
                }
            }
            else
            {
                SetAsNull();
            }
        }
#endif

        void LoadUi()
        {
            // 1 millisecond before 24:00:00
            spinner.MaxValue = new TimeSpan(1, 0, 0, 0).Add(new TimeSpan(0, 0, 0, 0, -1));
            spinner.MinValue = new TimeSpan(0, 0, 0);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private void spinner_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        void AllowNulls()
        {
            mnuSetNull.IsEnabled = true;
            _nullAllowed = true;
        }

        void SetAsNull()
        {
            _internalAction = true;
            mnuSetNull.IsEnabled = true;
            mnuSetNull.IsChecked = true;
            picker.SelectedDate = null;
            spinner.IsEnabled = false;
            _nullAllowed = true;
            _internalAction = false;
        }

        void UnsetAsNull(bool setDate = true)
        {
            _internalAction = true;
            mnuSetNull.IsChecked = false;
            if (setDate) picker.SelectedDate = DateTime.Today;
            spinner.IsEnabled = true;
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
            if (_internalAction) return;

            if (picker.SelectedDate == null)
            {
                if (_nullAllowed)
                {
                    SetAsNull();
                }
                else
                {
                    // just reset it to today
                    picker.SelectedDate = DateTime.Today.Date;
                }
            }
            else
            {
                UnsetAsNull(false);
            }
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
