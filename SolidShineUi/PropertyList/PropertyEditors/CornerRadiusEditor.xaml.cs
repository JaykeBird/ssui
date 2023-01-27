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
    /// A property editor for editing <see cref="CornerRadius"/> objects.
    /// </summary>
    public partial class CornerRadiusEditor : UserControl, IPropertyEditor
    {
        /// <inheritdoc/>
        public List<Type> ValidTypes => new List<Type> { typeof(CornerRadius), typeof(CornerRadius?) };

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public bool IsPropertyWritable { get => nudUpLeft.IsEnabled;
            set
            {
                nudUpLeft.IsEnabled = value;
                nudUpRight.IsEnabled = value;
                nudDownLeft.IsEnabled = value;
                nudDownRight.IsEnabled = value;
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
                nudUpLeft.ColorScheme = value;
                nudUpRight.ColorScheme = value;
                nudDownLeft.ColorScheme = value;
                nudDownRight.ColorScheme = value;
                btnMenu.ColorScheme = value;

                if (value.BackgroundColor == Colors.Black || value.ForegroundColor == Colors.White)
                {
                    imgLeft.Source = LoadIcon("UpLeftArrow", ICON_WHITE);
                    imgTop.Source = LoadIcon("UpRightArrow", ICON_WHITE);
                    imgRight.Source = LoadIcon("DownRightArrow", ICON_WHITE);
                    imgBottom.Source = LoadIcon("DownLeftArrow", ICON_WHITE);
                    imgFontEdit.Source = LoadIcon("ThreeDots", ICON_WHITE);
                }
                else if (value.BackgroundColor == Colors.White)
                {
                    imgLeft.Source = LoadIcon("UpLeftArrow", ICON_BLACK);
                    imgTop.Source = LoadIcon("UpRightArrow", ICON_BLACK);
                    imgRight.Source = LoadIcon("DownRightArrow", ICON_BLACK);
                    imgBottom.Source = LoadIcon("DownLeftArrow", ICON_BLACK);
                    imgFontEdit.Source = LoadIcon("ThreeDots", ICON_BLACK);
                }
                else
                {
                    imgLeft.Source = LoadIcon("UpLeftArrow", ICON_COLOR);
                    imgTop.Source = LoadIcon("UpRightArrow", ICON_COLOR);
                    imgRight.Source = LoadIcon("DownRightArrow", ICON_COLOR);
                    imgBottom.Source = LoadIcon("DownLeftArrow", ICON_COLOR);
                    imgFontEdit.Source = LoadIcon("ThreeDots", ICON_COLOR);
                }
            }
        }

        /// <inheritdoc/>
        public CornerRadiusEditor()
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
            if (type == typeof(CornerRadius) || type == typeof(CornerRadius?))
            {
                if (type == typeof(CornerRadius?))
                {
                    mnuSetNull.IsEnabled = true;
                }

                if (value != null)
                {
                    if (value is CornerRadius t)
                    {
                        _internalAction = true;
                        nudUpLeft   .Value = t.TopLeft;
                        nudUpRight  .Value = t.TopRight;
                        nudDownLeft .Value = t.BottomLeft;
                        nudDownRight.Value = t.BottomRight;
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
                return new CornerRadius(nudUpLeft.Value, nudUpRight.Value, nudDownRight.Value, nudDownLeft.Value);
            }
        }

        /// <inheritdoc/>
        public event EventHandler? ValueChanged;
#else
        public object GetValue()
        {
            if (mnuSetNull.IsChecked == true)
            {
                return null;
            }
            else
            {
                return new CornerRadius(nudUpLeft.Value, nudUpRight.Value, nudDownRight.Value, nudDownLeft.Value);
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
            SetAllToValue(5);
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
            nudUpLeft.Value = d;
            nudUpRight.Value = d;
            nudDownLeft.Value = d;
            nudDownRight.Value = d;
            UnsetAsNull();
            _internalAction = false;
        }

        void SetAsNull()
        {
            mnuSetNull.IsEnabled = true;
            mnuSetNull.IsChecked = true;
            nudUpLeft.IsEnabled = false;
            nudUpRight.IsEnabled = false;
            nudDownLeft.IsEnabled = false;
            nudDownRight.IsEnabled = false;
        }

        void UnsetAsNull()
        {
            mnuSetNull.IsChecked = false;
            nudUpLeft.IsEnabled = true;
            nudUpRight.IsEnabled = true;
            nudDownLeft.IsEnabled = true;
            nudDownRight.IsEnabled = true;
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
