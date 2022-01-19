using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// Interaction logic for BrushEditor.xaml
    /// </summary>
    public partial class BrushEditor : UserControl, IPropertyEditor
    {
        public BrushEditor()
        {
            InitializeComponent();
        }

        public List<Type> ValidTypes => (new[] { typeof(Brush), typeof(SolidColorBrush), typeof(LinearGradientBrush), typeof(RadialGradientBrush), 
            typeof(ImageBrush), typeof(BitmapCacheBrush), typeof(DrawingBrush) }).ToList();

        public bool CanEdit => false;

        private ColorScheme _cs = new ColorScheme();

        public ColorScheme ColorScheme
        {
            set
            {
                _cs = value;
                btnMenu.ColorScheme = value;
                
                if (value.IsHighContrast)
                {
                    btnBrush.BorderBrush = value.BorderColor.ToBrush();
                    btnBrush.BorderHighlightBrush = value.BorderColor.ToBrush();
                    btnBrush.BorderSelectedBrush = value.BorderColor.ToBrush();
                    btnBrush.BorderDisabledBrush = value.DarkDisabledColor.ToBrush();
                    btnBrush.DisabledBrush = value.BackgroundColor.ToBrush();
                    btnBrush.Foreground = value.ForegroundColor.ToBrush();
                    btnBrush.ClickBrush = value.ThirdHighlightColor.ToBrush();
                }
                else
                {
                    btnBrush.BorderBrush = value.BorderColor.ToBrush();
                    btnBrush.BorderDisabledBrush = value.DarkDisabledColor.ToBrush();
                    btnBrush.SelectedBrush = value.ThirdHighlightColor.ToBrush();
                    btnBrush.BorderHighlightBrush = value.HighlightColor.ToBrush();
                    btnBrush.BorderSelectedBrush = value.SelectionColor.ToBrush();
                    btnBrush.Foreground = value.ForegroundColor.ToBrush();
                    btnBrush.ClickBrush = value.ThirdHighlightColor.ToBrush();
                }
            }
        }

        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        Type _propType = typeof(Brush);
        Brush _dataValue = new SolidColorBrush(Colors.Black);

#if NETCOREAPP
        public event EventHandler? ValueChanged;

        public object? GetValue()
        {
            return _dataValue;
        }

        public void LoadValue(object? value, Type type)
        {
            if (value == null)
            {
                txtBrushType.Text = "(null)";
                btnBrush.Background = Colors.Black.ToBrush();
                btnBrush.HighlightBrush = Colors.Black.ToBrush();
                btnBrush.ClickBrush = Colors.Black.ToBrush();
                return;
            }

            if (!(value is Brush))
            {
                txtBrushType.Text = "Unknown";
                btnBrush.Background = Colors.Black.ToBrush();
                btnBrush.HighlightBrush = Colors.Black.ToBrush();
                btnBrush.ClickBrush = Colors.Black.ToBrush();
                return;
            }

            Type brushType = type;

            if (brushType == typeof(Brush))
            {
                brushType = value.GetType();
            }

            _propType = brushType;
            _dataValue = (Brush)value;

            if (brushType == typeof(SolidColorBrush))
            {
                txtBrushType.Text = "Solid Color";
                btnBrush.Background = (SolidColorBrush)value;
                btnBrush.HighlightBrush = (SolidColorBrush)value;
                btnBrush.ClickBrush = (SolidColorBrush)value;
            }
            else if (brushType == typeof(LinearGradientBrush))
            {
                txtBrushType.Text = "Gradient";
                btnBrush.Background = (LinearGradientBrush)value;
                btnBrush.HighlightBrush = (LinearGradientBrush)value;
                btnBrush.ClickBrush = (LinearGradientBrush)value;
            }
            else if (brushType == typeof(RadialGradientBrush))
            {
                txtBrushType.Text = "Gradient";
                btnBrush.Background = (RadialGradientBrush)value;
                btnBrush.HighlightBrush = (RadialGradientBrush)value;
                btnBrush.ClickBrush = (RadialGradientBrush)value;
            }
            else if (brushType == typeof(ImageBrush))
            {
                txtBrushType.Text = "Image";
                btnBrush.Background = (ImageBrush)value;
                btnBrush.HighlightBrush = (ImageBrush)value;
                btnBrush.ClickBrush = (ImageBrush)value;
            }
            else if (brushType == typeof(BitmapCacheBrush))
            {
                txtBrushType.Text = "Bitmap Cache";
                btnBrush.Background = Colors.LightGray.ToBrush();
                btnBrush.HighlightBrush = Colors.LightGray.ToBrush();
                btnBrush.ClickBrush = Colors.LightGray.ToBrush();
            }
            else if (brushType == typeof(DrawingBrush))
            {
                txtBrushType.Text = "Drawing";
                btnBrush.Background = Colors.LightGray.ToBrush();
                btnBrush.HighlightBrush = Colors.LightGray.ToBrush();
                btnBrush.ClickBrush = Colors.LightGray.ToBrush();
            }
            else
            {
                txtBrushType.Text = "Unknown";
                btnBrush.Background = Colors.Black.ToBrush();
                btnBrush.HighlightBrush = Colors.Black.ToBrush();
                btnBrush.ClickBrush = Colors.Black.ToBrush();
            }
        }
#else
        public event EventHandler ValueChanged;

        public object GetValue()
        {
            return _dataValue;
        }

        public void LoadValue(object value, Type type)
        {
            if (value == null)
            {
                txtBrushType.Text = "(null)";
                btnBrush.Background = Colors.Black.ToBrush();
                btnBrush.HighlightBrush = Colors.Black.ToBrush();
                btnBrush.ClickBrush = Colors.Black.ToBrush();
                return;
            }

            if (!(value is Brush))
            {
                txtBrushType.Text = "Unknown";
                btnBrush.Background = Colors.Black.ToBrush();
                btnBrush.HighlightBrush = Colors.Black.ToBrush();
                btnBrush.ClickBrush = Colors.Black.ToBrush();
                return;
            }

            Type brushType = type;

            if (brushType == typeof(Brush))
            {
                brushType = value.GetType();
            }

            _propType = brushType;
            _dataValue = (Brush)value;

            if (brushType == typeof(SolidColorBrush))
            {
                txtBrushType.Text = "Solid Color";
                btnBrush.Background = (SolidColorBrush)value;
                btnBrush.HighlightBrush = (SolidColorBrush)value;
                btnBrush.ClickBrush = (SolidColorBrush)value;
            }
            else if (brushType == typeof(LinearGradientBrush))
            {
                txtBrushType.Text = "Gradient";
                btnBrush.Background = (LinearGradientBrush)value;
                btnBrush.HighlightBrush = (LinearGradientBrush)value;
                btnBrush.ClickBrush = (LinearGradientBrush)value;
            }
            else if (brushType == typeof(RadialGradientBrush))
            {
                txtBrushType.Text = "Gradient";
                btnBrush.Background = (RadialGradientBrush)value;
                btnBrush.HighlightBrush = (RadialGradientBrush)value;
                btnBrush.ClickBrush = (RadialGradientBrush)value;
            }
            else if (brushType == typeof(ImageBrush))
            {
                txtBrushType.Text = "Image";
                btnBrush.Background = (ImageBrush)value;
                btnBrush.HighlightBrush = (ImageBrush)value;
                btnBrush.ClickBrush = (ImageBrush)value;
            }
            else if (brushType == typeof(BitmapCacheBrush))
            {
                txtBrushType.Text = "Bitmap Cache";
                btnBrush.Background = Colors.LightGray.ToBrush();
                btnBrush.HighlightBrush = Colors.LightGray.ToBrush();
                btnBrush.ClickBrush = Colors.LightGray.ToBrush();
            }
            else if (brushType == typeof(DrawingBrush))
            {
                txtBrushType.Text = "Drawing";
                btnBrush.Background = Colors.LightGray.ToBrush();
                btnBrush.HighlightBrush = Colors.LightGray.ToBrush();
                btnBrush.ClickBrush = Colors.LightGray.ToBrush();
            }
            else
            {
                txtBrushType.Text = "Unknown";
                btnBrush.Background = Colors.Black.ToBrush();
                btnBrush.HighlightBrush = Colors.Black.ToBrush();
                btnBrush.ClickBrush = Colors.Black.ToBrush();
            }
        }
#endif
        private void btnBrush_Click(object sender, RoutedEventArgs e)
        {
            if (_propType == typeof(SolidColorBrush))
            {
                ColorPickerDialog cpd = new ColorPickerDialog(_cs, ((SolidColorBrush)_dataValue).Color);
                cpd.ShowDialog();
                if (cpd.DialogResult)
                {
                    // update color
                }
            }
            else
            {
                // others not supported yet
            }
        }

    }
}
