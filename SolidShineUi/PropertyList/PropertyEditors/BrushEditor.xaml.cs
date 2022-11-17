using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using SolidShineUi.PropertyList.Dialogs;
using System.Windows.Media.TextFormatting;

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
            set
            {
                btnBrush.IsEnabled = value;
                btnEditBrush.IsEnabled = value;
                selChange.IsEnabled = value;
            }
        }

        /// <summary>the type of the property itself, to determine what is allowed as brushes</summary>
        Type _propType = typeof(Brush);
        /// <summary>the type of the actual current value</summary>
        Type _actualType = typeof(Brush);
        /// <summary>the actual current value</summary>
#if NETCOREAPP
        Brush? _dataValue = new SolidColorBrush(Colors.Black);
#else
        Brush _dataValue = new SolidColorBrush(Colors.Black);
#endif

        #region GetValue / LoadValue

        #region Base GetValue / LoadValue
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
                UpdatePreviewToNull();

                txtCurrentBrush.Text = "(null brush)";
                txtCurrentValue.Text = "";
                btnEditBrush.IsEnabled = false;
                btnEditBrush.Content = "Edit...";
                return;
            }

            if (!(value is Brush))
            {
                UpdatePreviewToNull();
                txtBrushType.Text = "(unknown)";

                txtCurrentBrush.Text = "(unknown)";
                txtCurrentValue.Text = value.ToString();
                btnEditBrush.IsEnabled = false;
                btnEditBrush.Content = "Edit...";
                return;
            }

            Type brushType = type;

            _propType = brushType;
            _actualType = value.GetType();
            _dataValue = (Brush)value;

            SetUiButtons(value.GetType(), value);
            SetupAllowedBrushesUI();
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
                UpdatePreviewToNull();

                txtCurrentBrush.Text = "(null brush)";
                txtCurrentValue.Text = "";
                btnEditBrush.IsEnabled = false;
                btnEditBrush.Content = "Edit...";
                return;
            }

            if (!(value is Brush))
            {
                UpdatePreviewToNull();
                txtBrushType.Text = "(unknown)";

                txtCurrentBrush.Text = "(unknown)";
                txtCurrentValue.Text = value.ToString();
                btnEditBrush.IsEnabled = false;
                btnEditBrush.Content = "Edit...";
                return;
            }

            Type brushType = type;

            _propType = brushType;
            _actualType = value.GetType();
            _dataValue = (Brush)value;

            SetUiButtons(value.GetType(), value);
            SetupAllowedBrushesUI();
        }
#endif
        #endregion

        #region UI Setups
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
                txtCurrentValue.Text = GetGradientDescriptor((RadialGradientBrush)value);
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
                txtCurrentValue.Text = GetImageDescriptor((ImageBrush)value);
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
                btnEditBrush.IsEnabled = false;
                btnEditBrush.Content = "Edit...";
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
                btnEditBrush.IsEnabled = false;
                btnEditBrush.Content = "Edit...";
            }
            else
            {
                UpdatePreviewToNull();
                txtBrushType.Text = "(unknown)";

                txtCurrentBrush.Text = "(unknown)";
                txtCurrentValue.Text = value.ToString();
                btnEditBrush.IsEnabled = false;
                btnEditBrush.Content = "Edit...";
            }
        }

        string GetGradientDescriptor(LinearGradientBrush brush)
        {
            double height = brush.StartPoint.Y - brush.EndPoint.Y;
            double width = brush.StartPoint.X - brush.EndPoint.X;

            double angleR = Math.Atan2(height, width);
            // time for some trigonometry! remember that from high school?
            // after converting from radians to degrees, it seems there's some random numbers at the end, so let's round it to XX.X degrees
            double angle = Math.Round(angleR * 180 / Math.PI, 1);

            if (brush.GradientStops.Count == 2)
            {
                return $"Angle: {angle}º, #{brush.GradientStops[0].Color.GetHexString()} - #{brush.GradientStops[1].Color.GetHexString()}";
            }

            return $"Angle: {angle}º, {brush.GradientStops.Count} stops";
        }

        string GetGradientDescriptor(RadialGradientBrush brush)
        {
            if (brush.GradientStops.Count == 2)
            {
                return $"Radial, #{brush.GradientStops[0].Color.GetHexString()} - #{brush.GradientStops[1].Color.GetHexString()}";
            }

            return $"Radial, {brush.GradientStops.Count} stops";
        }

        string GetImageDescriptor(ImageBrush br)
        {
            ImageSource isrc = br.ImageSource;
            if (isrc is BitmapImage bi)
            {
                // https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.bitmapimage.urisource
                if (bi.UriSource != null)
                {
                    string source = bi.UriSource.ToString();

                    if (source.Length > 45)
                    {
                        if (char.IsSurrogate(source[44]))
                        {
                            return source.Substring(0, 44) + "...";
                        }
                        else
                        {
                            return source.Substring(0, 45) + "...";
                        }
                    }
                    else
                    {
                        return source;
                    }
                }
                else
                {
                    // stream source
                    return "(image from stream)";
                }
            }
            else if (isrc is BitmapSource)
            {
                // bitmap source
                return "(image from bitmap source)";
            }
            else if (isrc is DrawingImage)
            {
                // maybe in the future, I can display some options or settings
                return "(image from drawing)";
            }
            else
            {
                // I don't know
                return "(image from unknown source)";
            }
        }

        void SetupAllowedBrushesUI()
        {
            // sets which brush types can be applied to this property, based upon the property's type
            // (this prevents SolidColorBrushes being used in a property of type ImageBrush, for example)

            // null will always be available
            // (in situations with .NET where nullability is on, this might cause problems, but the
            // Property List control should already be able to handle this via the "property change failed" code)
            siNothing.Visibility = Visibility.Visible;

            siSolid.Visibility = Visibility.Collapsed;
            siLinear.Visibility = Visibility.Collapsed;
            siRadial.Visibility = Visibility.Collapsed;
            siImage.Visibility = Visibility.Collapsed;

            if (_propType == typeof(Brush))
            {
                // all brushes are allowed
                siSolid.Visibility = Visibility.Visible;
                siLinear.Visibility = Visibility.Visible;
                siRadial.Visibility = Visibility.Visible;
                siImage.Visibility = Visibility.Visible;
            }
            else if (_propType == typeof(SolidColorBrush))
            {
                siSolid.Visibility = Visibility.Visible;
                siLinear.Visibility = Visibility.Collapsed;
                siRadial.Visibility = Visibility.Collapsed;
                siImage.Visibility = Visibility.Collapsed;
            }
            else if (_propType == typeof(LinearGradientBrush))
            {
                siSolid.Visibility = Visibility.Collapsed;
                siLinear.Visibility = Visibility.Visible;
                siRadial.Visibility = Visibility.Collapsed;
                siImage.Visibility = Visibility.Collapsed;
            }
            else if (_propType == typeof(RadialGradientBrush))
            {
                siSolid.Visibility = Visibility.Collapsed;
                siLinear.Visibility = Visibility.Collapsed;
                siRadial.Visibility = Visibility.Visible;
                siImage.Visibility = Visibility.Collapsed;
            }
            else if (_propType == typeof(ImageBrush))
            {
                siSolid.Visibility = Visibility.Collapsed;
                siLinear.Visibility = Visibility.Collapsed;
                siRadial.Visibility = Visibility.Collapsed;
                siImage.Visibility = Visibility.Visible;
            }
            else if (_propType == typeof(TileBrush))
            {
                siSolid.Visibility = Visibility.Collapsed;
                siLinear.Visibility = Visibility.Collapsed;
                siRadial.Visibility = Visibility.Collapsed;
                siImage.Visibility = Visibility.Visible;
            }
            else if (_propType == typeof(GradientBrush))
            {
                siSolid.Visibility = Visibility.Collapsed;
                siLinear.Visibility = Visibility.Visible;
                siRadial.Visibility = Visibility.Visible;
                siImage.Visibility = Visibility.Collapsed;
            }
        }

        void UpdatePreviewToNull()
        {
            DrawingBrush db = BrushFactory.CreateCheckerboardBrush(6, Colors.Gainsboro.ToBrush(), Colors.Silver.ToBrush());

            txtBrushType.Text = "(null)";
            btnBrush.Background = db;
            btnBrush.HighlightBrush = db;
            btnBrush.ClickBrush = db;
            btnBrush.DisabledBrush = db;
        }

        #endregion

        #endregion

        #region Edit Current Brush
        private void btnBrush_Click(object sender, RoutedEventArgs e)
        {
            if (_dataValue == null)
            {
                // this is null, so nothing is needed lol
                return;
            }

            if (_actualType == typeof(SolidColorBrush))
            {
                ColorPickerDialog cpd = new ColorPickerDialog(_cs, ((SolidColorBrush)_dataValue).Color);
                cpd.Owner = Window.GetWindow(this);
                cpd.ShowDialog();
                if (cpd.DialogResult)
                {
                    // update color
                    _dataValue = new SolidColorBrush(cpd.SelectedColor);

                    UpdateMainPreview((SolidColorBrush)_dataValue);
                    txtCurrentValue.Text = _dataValue.ToString();

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            else if (_actualType == typeof(LinearGradientBrush))
            {
                LinearGradientEditorDialog lged = new LinearGradientEditorDialog(_cs, (LinearGradientBrush)_dataValue);
                lged.Owner = Window.GetWindow(this);
                lged.ShowDialog();

                if (lged.DialogResult)
                {
                    // update info
                    _dataValue = lged.GetGradientBrush();

                    UpdateMainPreview((LinearGradientBrush)_dataValue);
                    txtCurrentValue.Text = GetGradientDescriptor((LinearGradientBrush)_dataValue);

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            else if (_actualType == typeof(RadialGradientBrush))
            {
                RadialGradientEditorDialog lged = new RadialGradientEditorDialog(_cs, (RadialGradientBrush)_dataValue);
                lged.Owner = Window.GetWindow(this);
                lged.ShowDialog();

                if (lged.DialogResult)
                {
                    // update info
                    _dataValue = lged.GetGradientBrush();

                    UpdateMainPreview((RadialGradientBrush)_dataValue);
                    txtCurrentValue.Text = GetGradientDescriptor((RadialGradientBrush)_dataValue);

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            else if (_actualType == typeof(ImageBrush))
            {
                ImageBrushEditorDialog ibre = new ImageBrushEditorDialog(_cs);
                ibre.LoadImage((ImageBrush)_dataValue);
                ibre.Owner = Window.GetWindow(this);
                ibre.ShowDialog();

                if (ibre.DialogResult)
                {
                    // update info
                    _dataValue = ibre.GetImageBrush();

                    UpdateMainPreview((ImageBrush)_dataValue);
                    txtCurrentValue.Text = GetImageDescriptor((ImageBrush)_dataValue);

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            else if (_actualType == typeof(DrawingBrush))
            {
                // need to add drawing brush editor dialog
            }
            else if (_actualType == typeof(BitmapCacheBrush))
            {
                // not supported for editing, just display some info
            }
            else
            {
                // uhhhhhhhhhhhhh
            }

            void UpdateMainPreview(Brush b)
            {
                btnBrush.Background = b;
                btnBrush.HighlightBrush = b;
                btnBrush.ClickBrush = b;
                btnBrush.DisabledBrush = b;
            }
        }
        #endregion

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            popBrush.PlacementTarget = btnMenu;
            popBrush.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;

            popBrush.IsOpen = true;
            popBrush.StaysOpen = false;
        }

        #region Change Brush

        private void siNothing_Click(object sender, EventArgs e)
        {
            // change to null
            _dataValue = null;
            _actualType = typeof(Brush);
            ValueChanged?.Invoke(this, EventArgs.Empty);

            UpdatePreviewToNull();

            txtCurrentBrush.Text = "(null brush)";
            txtCurrentValue.Text = "";
            btnEditBrush.IsEnabled = false;
            btnEditBrush.Content = "Edit...";
        }

        private void siSolid_Click(object sender, EventArgs e)
        {
            // change to solid color brush
            _dataValue = new SolidColorBrush(Colors.Red);
            _actualType = typeof(SolidColorBrush);
            ValueChanged?.Invoke(this, EventArgs.Empty);

            SetUiButtons(typeof(SolidColorBrush), _dataValue);
        }

        private void siLinear_Click(object sender, EventArgs e)
        {
            // change to linear gradient brush
            _dataValue = new LinearGradientBrush(Colors.Green, Colors.Orange, 45);
            _actualType = typeof(LinearGradientBrush);
            ValueChanged?.Invoke(this, EventArgs.Empty);

            SetUiButtons(typeof(LinearGradientBrush), _dataValue);
        }

        private void siRadial_Click(object sender, EventArgs e)
        {
            // change to radial gradient brush
            _dataValue = new RadialGradientBrush(Colors.Green, Colors.Orange);
            _actualType = typeof(RadialGradientBrush);
            ValueChanged?.Invoke(this, EventArgs.Empty);

            SetUiButtons(typeof(RadialGradientBrush), _dataValue);
        }

        private void siImage_Click(object sender, EventArgs e)
        {
            // change to image brush
            // display image brush editor dialog, don't actually immediately change
            ImageBrushEditorDialog ibre = new ImageBrushEditorDialog(_cs);
            ibre.LoadImage(new ImageBrush(MessageDialogImageConverter.GetImage(MessageDialogImage.Question, MessageDialogImageConverter.MessageDialogImageColor.Color)));
            ibre.Owner = Window.GetWindow(this);
            ibre.ShowDialog();

            if (ibre.DialogResult)
            {
                // update info
                _dataValue = ibre.GetImageBrush();
                _actualType = typeof(ImageBrush);

                SetUiButtons(typeof(ImageBrush), _dataValue);

                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        #endregion
    }
}
