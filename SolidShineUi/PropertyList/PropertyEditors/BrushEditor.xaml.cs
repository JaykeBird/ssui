using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using SolidShineUi.PropertyList.Dialogs;
using SolidShineUi.Utils;
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
        public void SetHostControl(IPropertyEditorHost host) { _host = host; }

        // private SsuiTheme _cs = new SsuiTheme();

#if NETCOREAPP
        private IPropertyEditorHost? _host = null;
#else
        private IPropertyEditorHost _host = null;
#endif

        /// <summary>
        /// Set the visual apperance of this control via a ColorScheme.
        /// </summary>
        /// <param name="theme">the color scheme to apply</param>
        public void ApplySsuiTheme(SsuiTheme theme)
        {
            // _cs = theme;
            btnMenu.SsuiTheme = theme;
            btnEditBrush.SsuiTheme = theme;
            brdrPop.SsuiTheme = theme;

            btnBrush.BorderBrush = theme.BorderBrush;
            btnBrush.BorderDisabledBrush = theme.DisabledBorderBrush;
            btnBrush.SelectedBrush = theme.SelectedBackgroundBrush;
            btnBrush.BorderHighlightBrush = theme.HighlightBorderBrush;
            btnBrush.BorderSelectedBrush = theme.SelectedBorderBrush;
            btnBrush.Foreground = theme.Foreground;
            btnBrush.ClickBrush = theme.ClickBrush;

            brdrBottom.BorderBrush = theme.BorderBrush;

            imgMenu.Source = IconLoader.LoadIcon("ThreeDots", theme.IconVariation);
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

                foreach (var item in brdrPop.Items)
                {
                    if (item is UIElement ele)
                    {
                        ele.IsEnabled = value;
                    }
                }
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

#if NETCOREAPP
        /// <inheritdoc/>
        public event EventHandler? ValueChanged;

        /// <inheritdoc/>
        public object? GetValue()
        {
            return CopyBrush();
        }
#else
        /// <inheritdoc/>
        public event EventHandler ValueChanged;

        /// <inheritdoc/>
        public object GetValue()
        {
            return CopyBrush();
        }
#endif

        /// <inheritdoc/>
#if NETCOREAPP
        public void LoadValue(object? value, Type type)
#else
        public void LoadValue(object value, Type type)
#endif
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
                UpdatePreview("(unknown)", null);

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

#if NETCOREAPP
        private Brush? CopyBrush()
#else
        private Brush CopyBrush()
#endif
        {
            // if (_dataValue == null) return null;

            return _dataValue?.CloneCurrentValue() ?? null;
        }

        #endregion

        #region UI / Editing

        #region Strings

        static string Str_Null = "(null)";
        static string Str_Unknown = "Unknown";
        static string Str_SolidColor = "Solid Color";
        static string Str_Gradient = "Gradient";
        static string Str_Image = "Image";
        static string Str_BitmapCache = "Bitmap Cache";
        static string Str_Drawing = "Drawing";

        #endregion

        #region UI Setups

        private void SetUiButtons(Brush value)
        {
            SetUiButtons(value.GetType(), value);
        }
        
        /// <summary>
        /// Update the text and display of the UI for this BrushEditor. This includes updating the preview and the descriptive text for the brush.
        /// </summary>
        /// <param name="brushType">the type of the brush</param>
        /// <param name="value">the actual brush value (if this is not a Brush, then this is treated as "unknown")</param>
        private void SetUiButtons(Type brushType, object value)
        {
            if (brushType == typeof(SolidColorBrush))
            {
                UpdatePreview(Str_SolidColor, (SolidColorBrush)value);

                txtCurrentBrush.Text = "Solid Color Brush";
                txtCurrentValue.Text = value.ToString();
                btnEditBrush.IsEnabled = true;
                btnEditBrush.Content = "Edit Color...";
            }
            else if (brushType == typeof(LinearGradientBrush))
            {
                UpdatePreview(Str_Gradient, (LinearGradientBrush)value);

                txtCurrentBrush.Text = "Linear Gradient Brush";
                txtCurrentValue.Text = GetGradientDescriptor((LinearGradientBrush)value);
                btnEditBrush.IsEnabled = true;
                btnEditBrush.Content = "Edit Gradient...";
            }
            else if (brushType == typeof(RadialGradientBrush))
            {
                UpdatePreview(Str_Gradient, (RadialGradientBrush)value);

                txtCurrentBrush.Text = "Radial Gradient Brush";
                txtCurrentValue.Text = GetGradientDescriptor((RadialGradientBrush)value);
                btnEditBrush.IsEnabled = true;
                btnEditBrush.Content = "Edit Gradient...";
            }
            else if (brushType == typeof(ImageBrush))
            {
                UpdatePreview(Str_Image, (ImageBrush)value);

                txtCurrentBrush.Text = "Image Brush";
                txtCurrentValue.Text = GetImageDescriptor((ImageBrush)value);
                btnEditBrush.IsEnabled = true;
                btnEditBrush.Content = "Edit Brush...";
            }
            else if (brushType == typeof(BitmapCacheBrush))
            {
                UpdatePreview(Str_BitmapCache, Colors.LightGray.ToBrush());

                txtCurrentBrush.Text = "Bitmap Cache Brush";
                txtCurrentValue.Text = value.ToString();
                btnEditBrush.IsEnabled = false;
                btnEditBrush.Content = "Edit...";
            }
            else if (brushType == typeof(DrawingBrush))
            {
                UpdatePreview(Str_Drawing, Colors.LightGray.ToBrush());

                txtCurrentBrush.Text = "Drawing Brush";
                txtCurrentValue.Text = value.ToString();
                btnEditBrush.IsEnabled = false;
                btnEditBrush.Content = "Edit...";
            }
            else
            {
                UpdatePreview(Str_Unknown, null);

                txtCurrentBrush.Text = "(unknown)";
                txtCurrentValue.Text = value.ToString();
                btnEditBrush.IsEnabled = false;
                btnEditBrush.Content = "Edit...";
            }
        }

        static string GetGradientDescriptor(GradientBrush gb)
        {
            if (gb is LinearGradientBrush lgb)
            {
                return GetGradientDescriptor(lgb);
            }
            else if (gb is RadialGradientBrush rgb)
            {
                return GetGradientDescriptor(rgb);
            }
            else
            {
                return $"Gradient, {gb.GradientStops.Count} stops";
            }
        }

        static string GetGradientDescriptor(LinearGradientBrush brush)
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

        static string GetGradientDescriptor(RadialGradientBrush brush)
        {
            if (brush.GradientStops.Count == 2)
            {
                return $"Radial, #{brush.GradientStops[0].Color.GetHexString()} - #{brush.GradientStops[1].Color.GetHexString()}";
            }

            return $"Radial, {brush.GradientStops.Count} stops";
        }

        static string GetImageDescriptor(ImageBrush br)
        {
            ImageSource isrc = br.ImageSource;
            if (isrc is BitmapImage bi)
            {
                // https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.bitmapimage.urisource
                if (bi.UriSource != null)
                {
                    string source = bi.UriSource.ToString();
                    return source.Truncate(45);
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

        #endregion

        #region Preview

#if NETCOREAPP
        Brush? previewBrush = null;
#else
        Brush previewBrush = null;
#endif

        void UpdatePreviewToNull()
        {
            UpdatePreview(Str_Null, null);
        }

#if NETCOREAPP
        private void UpdatePreview(string text, Brush? brushData)
#else
        private void UpdatePreview(string text, Brush brushData)
#endif
        {
            if (brushData == null)
            {
                previewBrush = BrushFactory.CreateCheckerboardBrush(6, Colors.Gainsboro.ToBrush(), Colors.Silver.ToBrush());
            }
            else
            {
                previewBrush = brushData;
            }

            txtBrushType.Text = text;
            btnBrush.Background = previewBrush;
            btnBrush.HighlightBrush = previewBrush;
            btnBrush.ClickBrush = previewBrush;
            btnBrush.DisabledBrush = previewBrush;
        }

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
                ColorPickerDialog cpd = new ColorPickerDialog(((SolidColorBrush)_dataValue).Color);
                //cpd.SsuiTheme = _cs;
                cpd.Owner = Window.GetWindow(this);
                cpd.SsuiTheme = _host?.GetThemeForDialogs() ?? new SsuiAppTheme();
                cpd.ShowDialog();
                if (cpd.DialogResult)
                {
                    // update color
                    UpdateValue(new SolidColorBrush(cpd.SelectedColor));
                }
            }
            else if (_actualType == typeof(LinearGradientBrush))
            {
                LinearGradientEditorDialog lged = new LinearGradientEditorDialog((LinearGradientBrush)_dataValue);
                lged.Owner = Window.GetWindow(this);
                lged.SsuiTheme = _host?.GetThemeForDialogs() ?? new SsuiAppTheme();
                lged.ShowDialog();

                if (lged.DialogResult)
                {
                    // update info
                    UpdateValue(lged.GetGradientBrush());
                }
            }
            else if (_actualType == typeof(RadialGradientBrush))
            {
                RadialGradientEditorDialog lged = new RadialGradientEditorDialog((RadialGradientBrush)_dataValue);
                lged.Owner = Window.GetWindow(this);
                lged.SsuiTheme = _host?.GetThemeForDialogs() ?? new SsuiAppTheme();
                lged.ShowDialog();

                if (lged.DialogResult)
                {
                    // update info
                    UpdateValue(lged.GetGradientBrush());
                }
            }
            else if (_actualType == typeof(ImageBrush))
            {
                ImageBrushEditorDialog ibre = new ImageBrushEditorDialog();
                ibre.LoadImage((ImageBrush)_dataValue);
                ibre.Owner = Window.GetWindow(this);
                ibre.SsuiTheme = _host?.GetThemeForDialogs() ?? new SsuiAppTheme();
                ibre.ShowDialog();

                if (ibre.DialogResult)
                {
                    // update info
                    UpdateValue(ibre.GetImageBrush());
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

            void UpdateValue(Brush b)
            {
                _dataValue = b;
                Brush brushCopy = b.CloneCurrentValue();

                UpdatePreview(txtBrushType.Text, brushCopy);

                if (b is GradientBrush gb)
                {
                    txtCurrentValue.Text = GetGradientDescriptor(gb);
                }
                else if (b is ImageBrush ib)
                {
                    txtCurrentValue.Text = GetImageDescriptor(ib);
                }
                else
                {
                    txtCurrentValue.Text = b.ToString(null);
                }

                ValueChanged?.Invoke(this, EventArgs.Empty);
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

        private void siNothing_Click(object sender, RoutedEventArgs e)
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

        private void siSolid_Click(object sender, RoutedEventArgs e)
        {
            // change to solid color brush
            _dataValue = new SolidColorBrush(Colors.Red);
            _actualType = typeof(SolidColorBrush);
            ValueChanged?.Invoke(this, EventArgs.Empty);

            SetUiButtons(_dataValue);
        }

        private void siLinear_Click(object sender, RoutedEventArgs e)
        {
            // change to linear gradient brush
            _dataValue = new LinearGradientBrush(Colors.Green, Colors.Orange, 45);
            _actualType = typeof(LinearGradientBrush);
            ValueChanged?.Invoke(this, EventArgs.Empty);

            SetUiButtons(_dataValue);
        }

        private void siRadial_Click(object sender, RoutedEventArgs e)
        {
            // change to radial gradient brush
            _dataValue = new RadialGradientBrush(Colors.Green, Colors.Orange);
            _actualType = typeof(RadialGradientBrush);
            ValueChanged?.Invoke(this, EventArgs.Empty);

            SetUiButtons(_dataValue);
        }

        private void siImage_Click(object sender, RoutedEventArgs e)
        {
            // change to image brush
            // display image brush editor dialog, don't actually immediately change
            ImageBrushEditorDialog ibre = new ImageBrushEditorDialog();
            ibre.LoadImage(new ImageBrush(MessageDialogImageConverter.GetImage(MessageDialogImage.Question, IconVariation.Color)));
            ibre.Owner = Window.GetWindow(this);
            ibre.SsuiTheme = _host?.GetThemeForDialogs() ?? new SsuiAppTheme();
            ibre.ShowDialog();

            if (ibre.DialogResult)
            {
                // update info
                _dataValue = ibre.GetImageBrush();
                _actualType = typeof(ImageBrush);

                SetUiButtons(_dataValue);

                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Brush Transforms

        private void btnEditTransform_Click(object sender, RoutedEventArgs e)
        {
            if (_dataValue == null)
            {
                // this is null, so nothing is needed lol
                return;
            }
            else
            {
                Transform t = _dataValue.Transform;
                TransformEditDialog ted = new TransformEditDialog();
                ted.ImportTransforms(t);
                ted.SsuiTheme = _host?.GetThemeForDialogs() ?? new SsuiAppTheme();
                ted.ShowDialog();

                if (ted.DialogResult == true)
                {
                    if (_dataValue.IsFrozen)
                    {
                        // just create a new brush
                        _dataValue = _dataValue.CloneCurrentValue();
                    }
                    _dataValue.Transform = ted.ExportSingleTransform();
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void btnEditRelativeTransform_Click(object sender, RoutedEventArgs e)
        {
            if (_dataValue == null)
            {
                // this is null, so nothing is needed lol
                return;
            }
            else
            {
                Transform t = _dataValue.RelativeTransform;
                TransformEditDialog ted = new TransformEditDialog();
                ted.ImportTransforms(t);
                ted.SsuiTheme = _host?.GetThemeForDialogs() ?? new SsuiAppTheme();
                ted.ShowDialog();

                if (ted.DialogResult == true)
                {
                    if (_dataValue.IsFrozen)
                    {
                        // just create a new brush
                        _dataValue = _dataValue.CloneCurrentValue();
                    }
                    _dataValue.RelativeTransform = ted.ExportSingleTransform();
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        private void btnBrush_SsuiThemeChanged(object sender, RoutedEventArgs e)
        {
            // reset colors back to the expected ones
            UpdatePreview(txtBrushType.Text, previewBrush);
        }

        #endregion
    }
}
