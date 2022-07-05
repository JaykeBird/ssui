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
    public partial class PointEditor : UserControl, IPropertyEditor
    {
        public List<Type> ValidTypes => new List<Type> { typeof(Point) };

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

        public ExperimentalPropertyList ParentPropertyList { set { } }

        public ColorScheme ColorScheme
        {
            set
            {
                nudHeight.ColorScheme = value;
                nudWidth.ColorScheme = value;
                btnMenu.ColorScheme = value;

                imgFontEdit.Source = LoadIcon("ThreeDots", value);
            }
        }

        public PointEditor()
        {
            InitializeComponent();
        }

        public FrameworkElement GetFrameworkElement() { return this; }

#if NETCOREAPP
        public void LoadValue(object? value, Type type)
        {
            if (type == typeof(Point))
            {
                if (value != null)
                {
                    if (value is Point t)
                    {
                        _internalAction = true;
                        nudWidth.Value = t.X;
                        nudHeight.Value = t.Y;
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
            return new Point(nudWidth.Value, nudHeight.Value);
        }

        public event EventHandler? ValueChanged;
#else
        public void LoadValue(object value, Type type)
        {
            if (type == typeof(Point))
            {
                if (value != null)
                {
                    if (value is Point t)
                    {
                        _internalAction = true;
                        nudWidth.Value = t.X;
                        nudHeight.Value = t.Y;
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
            return new Point(nudWidth.Value, nudHeight.Value);
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
            nudWidth.Value = d;
            nudHeight.Value = d;
            _internalAction = false;
        }
    }
}
