using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static SolidShineUi.Utils.IconLoader;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A property editor for editing <see cref="Size"/> objects.
    /// </summary>
    public partial class SizeEditor : UserControl, IPropertyEditor
    {
        /// <inheritdoc/>
        public List<Type> ValidTypes => new List<Type> { typeof(Size), typeof(Size?) };

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public bool IsPropertyWritable
        {
            get => nudHeight.IsEnabled;
            set
            {
                nudHeight.IsEnabled = value;
                nudWidth.IsEnabled = value;
                btnMenu.IsEnabled = value;
            }
        }

        /// <inheritdoc/>
        public void SetHostControl(IPropertyEditorHost host) { /* _host = host; */ }

        /// <inheritdoc/>
        public void ApplySsuiTheme(SsuiTheme theme)
        {
            nudHeight.SsuiTheme = theme;
            nudWidth.SsuiTheme = theme;
            btnMenu.SsuiTheme = theme;

            imgWidth.Source = LoadIcon("LeftRightArrow", theme.IconVariation);
            imgHeight.Source = LoadIcon("UpDownArrow", theme.IconVariation);
            imgMenu.Source = LoadIcon("ThreeDots", theme.IconVariation);
        }

        /// <summary>
        /// Create a SizeEditor.
        /// </summary>
        public SizeEditor()
        {
            InitializeComponent();

            // load in string values
            mnuSetNull.Header = Strings.SetAsNull;
            mnuSetZero.Header = Strings.SetAllToZero;
        }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement() { return this; }

        /// <inheritdoc/>
#if NETCOREAPP
        public void LoadValue(object? value, Type type)
#else
        public void LoadValue(object value, Type type)
#endif
        {
            if (type == typeof(Size) || type == typeof(Size?))
            {
                if (type == typeof(Size?))
                {
                    mnuSetNull.IsEnabled = true;
                }

                if (value != null)
                {
                    if (value is Size t)
                    {
                        _internalAction = true;
                        nudWidth.Value = t.Width;
                        nudHeight.Value = t.Height;
                        _internalAction = false;
                    }
                    else
                    {
                        // uhhh?
                        SetAllToValue(0);
                    }
                }
                else
                {
                    // null
                    SetAsNull();
                    SetAllToValue(0);
                }
            }
            else
            {
                // uhhh?
                SetAllToValue(0);
            }
        }

#if NETCOREAPP
        /// <inheritdoc/>
        public object? GetValue()
        {
            if (mnuSetNull.IsChecked == true)
            {
                return null;
            }
            else
            {
                return new Size(nudWidth.Value, nudHeight.Value);
            }
        }

        /// <inheritdoc/>
        public event EventHandler? ValueChanged;
#else
        /// <inheritdoc/>
        public object GetValue()
        {
            if (mnuSetNull.IsChecked == true)
            {
                return null;
            }
            else
            {
                return new Size(nudWidth.Value, nudHeight.Value);
            }
        }
        
        /// <inheritdoc/>
        public event EventHandler ValueChanged;
#endif

        bool _internalAction = false;

        private void mnuSetZero_Click(object sender, RoutedEventArgs e)
        {
            SetAllToValue(0);
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void mnuSetOne_Click(object sender, RoutedEventArgs e)
        {
            SetAllToValue(1);
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private void nudLeft_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!_internalAction)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        void SetAllToValue(double d)
        {
            _internalAction = true;
            nudWidth.Value = d;
            nudHeight.Value = d;
            UnsetAsNull();
            _internalAction = false;
        }

        void SetAsNull()
        {
            mnuSetNull.IsEnabled = true;
            mnuSetNull.IsChecked = true;
            nudWidth.IsEnabled = false;
            nudHeight.IsEnabled = false;
        }

        void UnsetAsNull()
        {
            mnuSetNull.IsChecked = false;
            nudWidth.IsEnabled = true;
            nudHeight.IsEnabled = true;
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
    }
}
