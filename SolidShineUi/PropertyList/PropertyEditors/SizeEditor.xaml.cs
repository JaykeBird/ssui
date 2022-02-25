using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static SolidShineUi.Utils.IconLoader;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// Interaction logic for SizeEditor.xaml
    /// </summary>
    public partial class SizeEditor : UserControl, IPropertyEditor
    {
        public List<Type> ValidTypes => new List<Type> { typeof(Size) };

        public bool EditorAllowsModifying => true;

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
        public ColorScheme ColorScheme
        {
            set
            {
                nudHeight.ColorScheme = value;
                nudWidth.ColorScheme = value;
                btnMenu.ColorScheme = value;

                if (value.BackgroundColor == Colors.Black || value.ForegroundColor == Colors.White)
                {
                    imgWidth.Source = LoadIcon("LeftRightArrow", ICON_WHITE);
                    imgHeight.Source = LoadIcon("UpDownArrow", ICON_WHITE);
                    imgFontEdit.Source = LoadIcon("ThreeDots", ICON_WHITE);
                }
                else if (value.BackgroundColor == Colors.White)
                {
                    imgWidth.Source = LoadIcon("LeftRightArrow", ICON_BLACK);
                    imgHeight.Source = LoadIcon("UpDownArrow", ICON_BLACK);
                    imgFontEdit.Source = LoadIcon("ThreeDots", ICON_BLACK);
                }
                else
                {
                    imgWidth.Source = LoadIcon("LeftRightArrow", ICON_COLOR);
                    imgHeight.Source = LoadIcon("UpDownArrow", ICON_COLOR);
                    imgFontEdit.Source = LoadIcon("ThreeDots", ICON_COLOR);
                }
            }
        }

        public SizeEditor()
        {
            InitializeComponent();
        }

        public FrameworkElement GetFrameworkElement() { return this; }

#if NETCOREAPP
        public void LoadValue(object? value, Type type)
        {
            if (type == typeof(Size))
            {
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
            return new Size(nudWidth.Value, nudHeight.Value);
        }

        public event EventHandler? ValueChanged;
#else
        public void LoadValue(object value, Type type)
        {
            if (type == typeof(Size))
            {
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
            return new Size(nudWidth.Value, nudHeight.Value);
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
            _internalAction = false;
        }
    }
}
