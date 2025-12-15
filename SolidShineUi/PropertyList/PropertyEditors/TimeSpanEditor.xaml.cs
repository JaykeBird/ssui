using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Diagnostics;

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
            mnuSetNull.IsEnabledChanged += MnuSetNull_IsEnabledChanged;
        }

        private void MnuSetNull_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
#if DEBUG
            Debugger.Log(0, null, "TIMESPANEDITOR: SET NULL CHANGED");
#endif
            //throw new NotImplementedException();
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
        public void SetHostControl(IPropertyEditorHost host) { /* _host = host; */ }
        
        /// <inheritdoc/>
        public void ApplySsuiTheme(SsuiTheme cs)
        {
            spinner.SsuiTheme = cs;
            btnMenu.SsuiTheme = cs;
            imgMenu.Source = Utils.IconLoader.LoadIcon("ThreeDots", cs.IconVariation);
        }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        /// <inheritdoc/>
        public bool IsPropertyWritable
        {
            get => mnuCurrent.IsEnabled;
            set
            {
                spinner.IsEnabled = value;
                mnuCurrent.IsEnabled = value;
                mnuNoon.IsEnabled = value;
                mnuMidnight.IsEnabled = value;
                mnuSetNull.IsEnabled = _nullsAllowed && value;
            }
        }

        Type _propType = typeof(TimeSpan);
        bool _nullsAllowed = false;

#if NETCOREAPP
        
        /// <inheritdoc/>
        public event EventHandler? ValueChanged;
        
        /// <inheritdoc/>
        public object? GetValue()
        {
            
            if (_propType == typeof(TimeSpan?))
            {
                return mnuSetNull.IsChecked ? null : spinner.Value;
            }
#if NET6_0_OR_GREATER
            else if (_propType == typeof(TimeOnly))
            {
                return spinner.GetValueAsTimeOnly();
            }
            else if (_propType == typeof(TimeOnly?))
            {
                return mnuSetNull.IsChecked ? null : spinner.GetValueAsTimeOnly();
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

            if (_propType == typeof(TimeSpan?))
            {
                TimeSpan? val = (TimeSpan?)value ?? null;
                if (val == null)
                {
                    SetAsNull();
                }
                else
                {
                    spinner.Value = val.Value;
                }
            }
#if NET6_0_OR_GREATER
            else if (_propType == typeof(TimeOnly?))
            {
                TimeOnly? tin = (TimeOnly?)value ?? null;
                if (tin == null)
                {
                    SetAsNull();
                }
                else
                {
                    spinner.SetValueFromTimeOnly(tin.Value);
                }
            }
            else if (value is TimeOnly ti)
            {
                spinner.SetValueFromTimeOnly(ti);
            }
#else
            else
            {
                spinner.Value = (TimeSpan)(value ?? TimeSpan.Zero);
            }
#endif
        }
#else

        /// <inheritdoc/>
        public event EventHandler ValueChanged;

        /// <inheritdoc/>
        public object GetValue()
        {
            if (_propType == typeof(TimeSpan?))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else
                {
                    return spinner.Value;
                }
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
            
            if (_propType == typeof(TimeSpan?))
            {
                AllowNulls();
            }

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
                SetMaxMin(TimeOnly.MaxValue.ToTimeSpan(), TimeOnly.MinValue.ToTimeSpan());
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

        void AllowNulls()
        {
            mnuSetNull.IsEnabled = true;
            _nullsAllowed = true;
        }

        void SetAsNull()
        {
            mnuSetNull.IsEnabled = true;
            mnuSetNull.IsChecked = true;
            spinner.IsEnabled = false;
            _nullsAllowed = true;
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

        private void mnuCurrentShort_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan val = DateTime.Now.TimeOfDay;
            spinner.Value = new TimeSpan(0, val.Hours, val.Minutes, val.Seconds);
        }
    }
}
