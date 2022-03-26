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
        public List<Type> ValidTypes => new List<Type> { typeof(Thickness) };

        public bool EditorAllowsModifying => true;

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

        public ThicknessEditor()
        {
            InitializeComponent();
        }

        public FrameworkElement GetFrameworkElement() { return this; }

#if NETCOREAPP
        public void LoadValue(object? value, Type type)
        {
            if (type == typeof(Thickness))
            {
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
                    // null? treat it as Thickness of value 0
                    SetAllToValue(0);
                }
            }
            else
            {
                // uhhh?
                SetAllToValue(0);
            }
        }

        public object? GetValue()
        {
            return new Thickness(nudLeft.Value, nudTop.Value, nudRight.Value, nudBottom.Value);
        }

        public event EventHandler? ValueChanged;
#else
        public void LoadValue(object value, Type type)
        {
            if (type == typeof(Thickness))
            {
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
                    // null? treat it as Thickness of value 0
                    SetAllToValue(0);
                }
            }
            else
            {
                // uhhh?
                SetAllToValue(0);
            }
        }

        public object GetValue()
        {
            return new Thickness(nudLeft.Value, nudTop.Value, nudRight.Value, nudBottom.Value);
        }

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
            _internalAction = false;
        }
    }
}
