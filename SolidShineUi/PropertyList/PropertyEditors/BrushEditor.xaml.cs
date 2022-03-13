using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        public bool EditorAllowsModifying => false;

        private ColorScheme _cs = new ColorScheme();

        public ColorScheme ColorScheme
        {
            set
            {
                _cs = value;
                btnMenu.ColorScheme = value;
                btnNullBrush.ColorScheme = value;
                btnSolidColor.ColorScheme = value;
                btnLinGradient.ColorScheme = value;
                btnRadGradient.ColorScheme = value;
                btnImageBrush.ColorScheme = value;

                if (value.IsHighContrast)
                {
                    btnBrush.BorderBrush = value.BorderColor.ToBrush();
                    btnBrush.BorderHighlightBrush = value.BorderColor.ToBrush();
                    btnBrush.BorderSelectedBrush = value.BorderColor.ToBrush();
                    btnBrush.BorderDisabledBrush = value.DarkDisabledColor.ToBrush();
                    //btnBrush.DisabledBrush = value.BackgroundColor.ToBrush();
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

                if (value.BackgroundColor == Colors.Black || value.ForegroundColor == Colors.White)
                {
                    imgMenu.Source = new BitmapImage(new Uri("/SolidShineUi;component/Images/ThreeDotsWhite.png", UriKind.Relative));
                }
                else if (value.BackgroundColor == Colors.White)
                {
                    imgMenu.Source = new BitmapImage(new Uri("/SolidShineUi;component/Images/ThreeDotsBlack.png", UriKind.Relative));
                }
                else
                {
                    imgMenu.Source = new BitmapImage(new Uri("/SolidShineUi;component/Images/ThreeDotsColor.png", UriKind.Relative));
                }
            }
        }

        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }
        public bool IsPropertyWritable
        {
            get => btnBrush.IsEnabled;
            set => btnBrush.IsEnabled = value;
        }

        Type _propType = typeof(Brush);
        Brush _dataValue = new SolidColorBrush(Colors.Black);

        #region GetValue / LoadValue

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
                btnBrush.DisabledBrush = Colors.Black.ToBrush();

                btnNullBrush.IsSelected = true;
                btnSolidColor.IsSelected = false;
                btnLinGradient.IsSelected = false;
                btnRadGradient.IsSelected = false;
                btnImageBrush.IsSelected = false;
                btnMenu.IsSelected = false;
                return;
            }

            if (!(value is Brush))
            {
                txtBrushType.Text = "Unknown";
                btnBrush.Background = Colors.Black.ToBrush();
                btnBrush.HighlightBrush = Colors.Black.ToBrush();
                btnBrush.ClickBrush = Colors.Black.ToBrush();
                btnBrush.DisabledBrush = Colors.Black.ToBrush();

                btnNullBrush.IsSelected = false;
                btnSolidColor.IsSelected = false;
                btnLinGradient.IsSelected = false;
                btnRadGradient.IsSelected = false;
                btnImageBrush.IsSelected = false;
                btnMenu.IsSelected = true;
                return;
            }

            Type brushType = type;

            if (brushType == typeof(Brush))
            {
                brushType = value.GetType();
            }

            _propType = brushType;
            _dataValue = (Brush)value;

            SetUiButtons(brushType, value);
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
                btnBrush.DisabledBrush = Colors.Black.ToBrush();

                btnNullBrush.IsSelected = true;
                btnSolidColor.IsSelected = false;
                btnLinGradient.IsSelected = false;
                btnRadGradient.IsSelected = false;
                btnImageBrush.IsSelected = false;
                btnMenu.IsSelected = false;
                return;
            }

            if (!(value is Brush))
            {
                txtBrushType.Text = "Unknown";
                btnBrush.Background = Colors.Black.ToBrush();
                btnBrush.HighlightBrush = Colors.Black.ToBrush();
                btnBrush.ClickBrush = Colors.Black.ToBrush();
                btnBrush.DisabledBrush = Colors.Black.ToBrush();

                btnNullBrush.IsSelected = false;
                btnSolidColor.IsSelected = false;
                btnLinGradient.IsSelected = false;
                btnRadGradient.IsSelected = false;
                btnImageBrush.IsSelected = false;
                btnMenu.IsSelected = true;
                return;
            }

            Type brushType = type;

            if (brushType == typeof(Brush))
            {
                brushType = value.GetType();
            }

            _propType = brushType;
            _dataValue = (Brush)value;

            SetUiButtons(brushType, value);
        }
#endif

        private void SetUiButtons(Type brushType, object value)
        {
            if (brushType == typeof(SolidColorBrush))
            {
                txtBrushType.Text = "Solid Color";
                btnBrush.Background = (SolidColorBrush)value;
                btnBrush.HighlightBrush = (SolidColorBrush)value;
                btnBrush.ClickBrush = (SolidColorBrush)value;
                btnBrush.DisabledBrush = (SolidColorBrush)value;

                btnNullBrush.IsSelected = false;
                btnSolidColor.IsSelected = true;
                btnLinGradient.IsSelected = false;
                btnRadGradient.IsSelected = false;
                btnImageBrush.IsSelected = false;
                btnMenu.IsSelected = false;
            }
            else if (brushType == typeof(LinearGradientBrush))
            {
                txtBrushType.Text = "Gradient";
                btnBrush.Background = (LinearGradientBrush)value;
                btnBrush.HighlightBrush = (LinearGradientBrush)value;
                btnBrush.ClickBrush = (LinearGradientBrush)value;
                btnBrush.DisabledBrush = (LinearGradientBrush)value;

                btnNullBrush.IsSelected = false;
                btnSolidColor.IsSelected = false;
                btnLinGradient.IsSelected = true;
                btnRadGradient.IsSelected = false;
                btnImageBrush.IsSelected = false;
                btnMenu.IsSelected = false;
            }
            else if (brushType == typeof(RadialGradientBrush))
            {
                txtBrushType.Text = "Gradient";
                btnBrush.Background = (RadialGradientBrush)value;
                btnBrush.HighlightBrush = (RadialGradientBrush)value;
                btnBrush.ClickBrush = (RadialGradientBrush)value;
                btnBrush.DisabledBrush = (RadialGradientBrush)value;

                btnNullBrush.IsSelected = false;
                btnSolidColor.IsSelected = false;
                btnLinGradient.IsSelected = false;
                btnRadGradient.IsSelected = true;
                btnImageBrush.IsSelected = false;
                btnMenu.IsSelected = false;
            }
            else if (brushType == typeof(ImageBrush))
            {
                txtBrushType.Text = "Image";
                btnBrush.Background = (ImageBrush)value;
                btnBrush.HighlightBrush = (ImageBrush)value;
                btnBrush.ClickBrush = (ImageBrush)value;
                btnBrush.DisabledBrush = (ImageBrush)value;

                btnNullBrush.IsSelected = false;
                btnSolidColor.IsSelected = false;
                btnLinGradient.IsSelected = false;
                btnRadGradient.IsSelected = false;
                btnImageBrush.IsSelected = true;
                btnMenu.IsSelected = false;
            }
            else if (brushType == typeof(BitmapCacheBrush))
            {
                txtBrushType.Text = "Bitmap Cache";
                btnBrush.Background = Colors.LightGray.ToBrush();
                btnBrush.HighlightBrush = Colors.LightGray.ToBrush();
                btnBrush.ClickBrush = Colors.LightGray.ToBrush();
                btnBrush.DisabledBrush = Colors.LightGray.ToBrush();

                btnNullBrush.IsSelected = false;
                btnSolidColor.IsSelected = false;
                btnLinGradient.IsSelected = false;
                btnRadGradient.IsSelected = false;
                btnImageBrush.IsSelected = false;
                btnMenu.IsSelected = true;
            }
            else if (brushType == typeof(DrawingBrush))
            {
                txtBrushType.Text = "Drawing";
                btnBrush.Background = Colors.LightGray.ToBrush();
                btnBrush.HighlightBrush = Colors.LightGray.ToBrush();
                btnBrush.ClickBrush = Colors.LightGray.ToBrush();
                btnBrush.DisabledBrush = Colors.LightGray.ToBrush();

                btnNullBrush.IsSelected = false;
                btnSolidColor.IsSelected = false;
                btnLinGradient.IsSelected = false;
                btnRadGradient.IsSelected = false;
                btnImageBrush.IsSelected = false;
                btnMenu.IsSelected = true;
            }
            else
            {
                txtBrushType.Text = "Unknown";
                btnBrush.Background = Colors.Black.ToBrush();
                btnBrush.HighlightBrush = Colors.Black.ToBrush();
                btnBrush.ClickBrush = Colors.Black.ToBrush();
                btnBrush.DisabledBrush = Colors.Black.ToBrush();

                btnNullBrush.IsSelected = false;
                btnSolidColor.IsSelected = false;
                btnLinGradient.IsSelected = false;
                btnRadGradient.IsSelected = false;
                btnImageBrush.IsSelected = false;
                btnMenu.IsSelected = true;
            }
        }

        #endregion
        
        private void btnBrush_Click(object sender, RoutedEventArgs e)
        {
            if (_propType == typeof(SolidColorBrush))
            {
                ColorPickerDialog cpd = new ColorPickerDialog(_cs, ((SolidColorBrush)_dataValue).Color);
                cpd.ShowDialog();
                if (cpd.DialogResult)
                {
                    // update color
                    _dataValue = new SolidColorBrush(cpd.SelectedColor);
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                // others not supported yet
            }
        }

    }
}
