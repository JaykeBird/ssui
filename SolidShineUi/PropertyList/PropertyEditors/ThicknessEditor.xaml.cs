using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static SolidShineUi.Utils.IconLoader;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// Interaction logic for ThicknessEditor.xaml
    /// </summary>
    public partial class ThicknessEditor : UserControl, IPropertyEditor
    {
        /// <inheritdoc/>
        public List<Type> ValidTypes => new List<Type> { typeof(Thickness), typeof(Thickness?) };

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public bool IsPropertyWritable { get => nudLeft.IsEnabled;
            set
            {
                nudLeft.IsEnabled = value;
                nudTop.IsEnabled = value;
                nudBottom.IsEnabled = value;
                nudRight.IsEnabled = value;
                btnMenu.IsEnabled = value;
            }
        }

        /// <inheritdoc/>
        public ExperimentalPropertyList ParentPropertyList { set { } }

        /// <inheritdoc/>
        public ColorScheme ColorScheme
        {
            set
            {
                nudLeft.ColorScheme = value;
                nudTop.ColorScheme = value;
                nudRight.ColorScheme = value;
                nudBottom.ColorScheme = value;
                btnMenu.ColorScheme = value;

                if (value.BackgroundColor == Colors.Black || value.ForegroundColor == Colors.White)
                {
                    imgLeft.Source = LoadIcon("LeftArrow", ICON_WHITE);
                    imgRight.Source = LoadIcon("RightArrow", ICON_WHITE);
                    imgTop.Source = LoadIcon("UpArrow", ICON_WHITE);
                    imgBottom.Source = LoadIcon("DownArrow", ICON_WHITE);
                    imgFontEdit.Source = LoadIcon("ThreeDots", ICON_WHITE);
                }
                else if (value.BackgroundColor == Colors.White)
                {
                    imgLeft.Source = LoadIcon("LeftArrow", ICON_BLACK);
                    imgRight.Source = LoadIcon("RightArrow", ICON_BLACK);
                    imgTop.Source = LoadIcon("UpArrow", ICON_BLACK);
                    imgBottom.Source = LoadIcon("DownArrow", ICON_BLACK);
                    imgFontEdit.Source = LoadIcon("ThreeDots", ICON_BLACK);
                }
                else
                {
                    imgLeft.Source = LoadIcon("LeftArrow", ICON_COLOR);
                    imgRight.Source = LoadIcon("RightArrow", ICON_COLOR);
                    imgTop.Source = LoadIcon("UpArrow", ICON_COLOR);
                    imgBottom.Source = LoadIcon("DownArrow", ICON_COLOR);
                    imgFontEdit.Source = LoadIcon("ThreeDots", ICON_COLOR);
                }
            }
        }

        /// <inheritdoc/>
        public ThicknessEditor()
        {
            InitializeComponent();
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
            if (type == typeof(Thickness) || type == typeof(Thickness?))
            {
                if (type == typeof(Thickness?))
                {
                    mnuSetNull.IsEnabled = true;
                }

                if (value != null)
                {
                    if (value is Thickness t)
                    {
                        _internalAction = true;
                        nudLeft.Value = t.Left;
                        nudTop.Value = t.Top;
                        nudRight.Value = t.Right;
                        nudBottom.Value = t.Bottom;
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

        /// <inheritdoc/>
#if NETCOREAPP
        public object? GetValue()
        {
            if (mnuSetNull.IsChecked == true)
            {
                return null;
            }
            else
            {
                return new Thickness(nudLeft.Value, nudTop.Value, nudRight.Value, nudBottom.Value);
            }
        }

        /// <inheritdoc/>
        public event EventHandler? ValueChanged;
#else
        public object GetValue()
        {
            return new Thickness(nudLeft.Value, nudTop.Value, nudRight.Value, nudBottom.Value);
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
            nudLeft.Value = d;
            nudTop.Value = d;
            nudRight.Value = d;
            nudBottom.Value = d;
            UnsetAsNull();
            _internalAction = false;
        }

        void SetAsNull()
        {
            mnuSetNull.IsEnabled = true;
            mnuSetNull.IsChecked = true;
            nudLeft.IsEnabled = false;
            nudTop.IsEnabled = false;
            nudRight.IsEnabled = false;
            nudBottom.IsEnabled = false;
        }

        void UnsetAsNull()
        {
            mnuSetNull.IsChecked = false;
            nudLeft.IsEnabled = true;
            nudTop.IsEnabled = true;
            nudRight.IsEnabled = true;
            nudBottom.IsEnabled = true;
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
