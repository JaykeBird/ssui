using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using SolidShineUi.PropertyList.Dialogs;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A property editor for editing <see cref="Brush"/> objects.
    /// </summary>
    public partial class BrushEditor : UserControl, IPropertyEditor
    {
        /// <summary>
        /// Create a new BrushEditor.
        /// </summary>
        public BrushEditor()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public List<Type> ValidTypes => (new[] { typeof(Brush), typeof(SolidColorBrush), typeof(LinearGradientBrush), typeof(RadialGradientBrush), 
            typeof(ImageBrush), typeof(BitmapCacheBrush), typeof(DrawingBrush) }).ToList();

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public ExperimentalPropertyList ParentPropertyList { set { } }

        private ColorScheme _cs = new ColorScheme();

        /// <inheritdoc/>
        public ColorScheme ColorScheme
        {
            set
            {
                _cs = value;
                btnMenu.ColorScheme = value;
                btnEditBrush.ColorScheme = value;
                selChange.ColorScheme = value;
                brdrPop.BorderBrush = value.BorderColor.ToBrush();
                brdrPop.Background = value.ThirdHighlightColor.ToBrush();

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

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        /// <inheritdoc/>
        public bool IsPropertyWritable
        {
            get => btnBrush.IsEnabled;
            set => btnBrush.IsEnabled = value;
        }

        Type _propType = typeof(Brush);
#if NETCOREAPP
        Brush? _dataValue = new SolidColorBrush(Colors.Black);
#else
        Brush _dataValue = new SolidColorBrush(Colors.Black);
#endif

        #region GetValue / LoadValue

#if NETCOREAPP
        /// <inheritdoc/>
        public event EventHandler? ValueChanged;

        /// <inheritdoc/>
        public object? GetValue()
        {
            return _dataValue;
        }

        /// <inheritdoc/>
        public void LoadValue(object? value, Type type)
        {
            if (value == null)
            {
                txtBrushType.Text = "(null)";
                btnBrush.Background = Colors.Black.ToBrush();
                btnBrush.HighlightBrush = Colors.Black.ToBrush();
                btnBrush.ClickBrush = Colors.Black.ToBrush();
                btnBrush.DisabledBrush = Colors.Black.ToBrush();

                txtCurrentBrush.Text = "(null brush)";
                txtCurrentValue.Text = "";
                btnEditBrush.IsEnabled = false;
                btnEditBrush.Content = "Edit...";
                return;
            }

            if (!(value is Brush))
            {
                txtBrushType.Text = "Unknown";
                btnBrush.Background = Colors.Black.ToBrush();
                btnBrush.HighlightBrush = Colors.Black.ToBrush();
                btnBrush.ClickBrush = Colors.Black.ToBrush();
                btnBrush.DisabledBrush = Colors.Black.ToBrush();

                txtCurrentBrush.Text = "(unknown)";
                txtCurrentValue.Text = value.ToString();
                btnEditBrush.IsEnabled = false;
                btnEditBrush.Content = "Edit...";
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
        /// <inheritdoc/>
        public event EventHandler ValueChanged;

        /// <inheritdoc/>
        public object GetValue()
        {
            return _dataValue;
        }

        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
        {
            if (value == null)
            {
                txtBrushType.Text = "(null)";
                btnBrush.Background = Colors.Black.ToBrush();
                btnBrush.HighlightBrush = Colors.Black.ToBrush();
                btnBrush.ClickBrush = Colors.Black.ToBrush();
                btnBrush.DisabledBrush = Colors.Black.ToBrush();

                txtCurrentBrush.Text = "(null brush)";
                txtCurrentValue.Text = "";
                btnEditBrush.IsEnabled = false;
                btnEditBrush.Content = "Edit...";
                return;
            }

            if (!(value is Brush))
            {
                txtBrushType.Text = "Unknown";
                btnBrush.Background = Colors.Black.ToBrush();
                btnBrush.HighlightBrush = Colors.Black.ToBrush();
                btnBrush.ClickBrush = Colors.Black.ToBrush();
                btnBrush.DisabledBrush = Colors.Black.ToBrush();

                txtCurrentBrush.Text = "(unknown)";
                txtCurrentValue.Text = value.ToString();
                btnEditBrush.IsEnabled = false;
                btnEditBrush.Content = "Edit...";
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

                txtCurrentBrush.Text = "Solid Color Brush";
                txtCurrentValue.Text = value.ToString();
                btnEditBrush.IsEnabled = true;
                btnEditBrush.Content = "Edit Color...";
            }
            else if (brushType == typeof(LinearGradientBrush))
            {
                txtBrushType.Text = "Gradient";
                btnBrush.Background = (LinearGradientBrush)value;
                btnBrush.HighlightBrush = (LinearGradientBrush)value;
                btnBrush.ClickBrush = (LinearGradientBrush)value;
                btnBrush.DisabledBrush = (LinearGradientBrush)value;

                txtCurrentBrush.Text = "Linear Gradient Brush";
                txtCurrentValue.Text = GetGradientDescriptor((LinearGradientBrush)value);
                btnEditBrush.IsEnabled = true;
                btnEditBrush.Content = "Edit Gradient...";
            }
            else if (brushType == typeof(RadialGradientBrush))
            {
                txtBrushType.Text = "Gradient";
                btnBrush.Background = (RadialGradientBrush)value;
                btnBrush.HighlightBrush = (RadialGradientBrush)value;
                btnBrush.ClickBrush = (RadialGradientBrush)value;
                btnBrush.DisabledBrush = (RadialGradientBrush)value;

                txtCurrentBrush.Text = "Radial Gradient Brush";
                txtCurrentValue.Text = value.ToString();
                btnEditBrush.IsEnabled = true;
                btnEditBrush.Content = "Edit Gradient...";
            }
            else if (brushType == typeof(ImageBrush))
            {
                txtBrushType.Text = "Image";
                btnBrush.Background = (ImageBrush)value;
                btnBrush.HighlightBrush = (ImageBrush)value;
                btnBrush.ClickBrush = (ImageBrush)value;
                btnBrush.DisabledBrush = (ImageBrush)value;

                txtCurrentBrush.Text = "Image Brush";
                txtCurrentValue.Text = value.ToString();
                btnEditBrush.IsEnabled = true;
                btnEditBrush.Content = "Edit...";
            }
            else if (brushType == typeof(BitmapCacheBrush))
            {
                txtBrushType.Text = "Bitmap Cache";
                btnBrush.Background = Colors.LightGray.ToBrush();
                btnBrush.HighlightBrush = Colors.LightGray.ToBrush();
                btnBrush.ClickBrush = Colors.LightGray.ToBrush();
                btnBrush.DisabledBrush = Colors.LightGray.ToBrush();

                txtCurrentBrush.Text = "Bitmap Cache Brush";
                txtCurrentValue.Text = value.ToString();
                btnEditBrush.IsEnabled = true;
                btnEditBrush.Content = "Brush Info...";
            }
            else if (brushType == typeof(DrawingBrush))
            {
                txtBrushType.Text = "Drawing";
                btnBrush.Background = Colors.LightGray.ToBrush();
                btnBrush.HighlightBrush = Colors.LightGray.ToBrush();
                btnBrush.ClickBrush = Colors.LightGray.ToBrush();
                btnBrush.DisabledBrush = Colors.LightGray.ToBrush();

                txtCurrentBrush.Text = "Drawing Brush";
                txtCurrentValue.Text = value.ToString();
                btnEditBrush.IsEnabled = true;
                btnEditBrush.Content = "Edit...";
            }
            else
            {
                txtBrushType.Text = "Unknown";
                btnBrush.Background = Colors.Black.ToBrush();
                btnBrush.HighlightBrush = Colors.Black.ToBrush();
                btnBrush.ClickBrush = Colors.Black.ToBrush();
                btnBrush.DisabledBrush = Colors.Black.ToBrush();

                txtCurrentBrush.Text = "(unknown)";
                txtCurrentValue.Text = value.ToString();
                btnEditBrush.IsEnabled = false;
                btnEditBrush.Content = "Edit...";
            }
        }

        string GetGradientDescriptor(LinearGradientBrush brush)
        {
            double height = brush.StartPoint.Y - brush.EndPoint.Y;
            double width = brush.StartPoint.X - brush.EndPoint.Y;

            double angleR = Math.Tan(height / width);
            // time for some trigonometry! remember that from high school?
            // after converting from radians to degrees, it seems there's some random numbers at the end, so let's round it to XX.X degrees
            double angle = Math.Round(angleR * 180 / Math.PI, 1);

            if (brush.GradientStops.Count == 2)
            {
                return $"Angle: {angle}º, {brush.GradientStops[0].Color.GetHexString()} - {brush.GradientStops[1].Color.GetHexString()}";
            }

            return $"Angle: {angle}º, {brush.GradientStops.Count} stops";
        }

#endregion
        
        private void btnBrush_Click(object sender, RoutedEventArgs e)
        {
            if (_dataValue == null)
            {
                // how did we reach this?
                return;
            }

            if (_propType == typeof(SolidColorBrush))
            {
                ColorPickerDialog cpd = new ColorPickerDialog(_cs, ((SolidColorBrush)_dataValue).Color);
                cpd.Owner = Window.GetWindow(this);
                cpd.ShowDialog();
                if (cpd.DialogResult)
                {
                    // update color
                    _dataValue = new SolidColorBrush(cpd.SelectedColor);

                    btnBrush.Background = (SolidColorBrush)_dataValue;
                    btnBrush.HighlightBrush = (SolidColorBrush)_dataValue;
                    btnBrush.ClickBrush = (SolidColorBrush)_dataValue;
                    btnBrush.DisabledBrush = (SolidColorBrush)_dataValue;

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            else if (_propType == typeof(LinearGradientBrush))
            {
                LinearGradientEditorDialog lged = new LinearGradientEditorDialog(_cs, (LinearGradientBrush)_dataValue);
                //lged.LoadGradient((LinearGradientBrush)_dataValue);
                lged.Owner = Window.GetWindow(this);
                lged.ShowDialog();

                if (lged.DialogResult)
                {
                    // update info
                    // _dataValue = new SolidColorBrush(cpd.SelectedColor);
                    // ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            else if (_propType == typeof(RadialGradientBrush))
            {
                // need to add gradient editor dialog
            }
            else if (_propType == typeof(ImageBrush))
            {
                // need to add image brush editor dialog
            }
            else if (_propType == typeof(DrawingBrush))
            {
                // need to add drawing brush editor dialog
            }
            else if (_propType == typeof(BitmapCacheBrush))
            {
                // not supported for editing, just display some info
            }
            else
            {
                // uhhhhhhhhhhhhh
            }
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            popBrush.PlacementTarget = btnMenu;
            popBrush.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;

            popBrush.IsOpen = true;
            popBrush.StaysOpen = false;
        }

        private void siNothing_Click(object sender, EventArgs e)
        {
            // change to null
            _dataValue = null;
            _propType = typeof(Brush);
            ValueChanged?.Invoke(this, EventArgs.Empty);

            txtBrushType.Text = "(null)";
            btnBrush.Background = Colors.Black.ToBrush();
            btnBrush.HighlightBrush = Colors.Black.ToBrush();
            btnBrush.ClickBrush = Colors.Black.ToBrush();
            btnBrush.DisabledBrush = Colors.Black.ToBrush();

            txtCurrentBrush.Text = "(null brush)";
            txtCurrentValue.Text = "";
            btnEditBrush.IsEnabled = false;
            btnEditBrush.Content = "Edit...";
        }

        private void siSolid_Click(object sender, EventArgs e)
        {
            // change to solid color brush
            _dataValue = new SolidColorBrush(Colors.Red);
            _propType = typeof(SolidColorBrush);
            ValueChanged?.Invoke(this, EventArgs.Empty);

            SetUiButtons(typeof(SolidColorBrush), _dataValue);
        }

        private void siLinear_Click(object sender, EventArgs e)
        {
            // change to linear gradient brush
            _dataValue = new LinearGradientBrush(Colors.Green, Colors.Orange, 45);
            _propType = typeof(LinearGradientBrush);
            ValueChanged?.Invoke(this, EventArgs.Empty);

            SetUiButtons(typeof(LinearGradientBrush), _dataValue);
        }

        private void siRadial_Click(object sender, EventArgs e)
        {
            // change to radial gradient brush
            _dataValue = new RadialGradientBrush(Colors.Green, Colors.Orange);
            _propType = typeof(RadialGradientBrush);
            ValueChanged?.Invoke(this, EventArgs.Empty);

            SetUiButtons(typeof(RadialGradientBrush), _dataValue);
        }

        private void siImage_Click(object sender, EventArgs e)
        {
            // change to image brush
            // display image brush editor dialog, don't actually immediately change
        }
    }
}
