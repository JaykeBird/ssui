using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SolidShineUi.PropertyList.Dialogs
{
    /// <summary>
    /// A dialog for editing <see cref="ImageBrush"/> objects.
    /// </summary>
    public partial class ImageBrushEditorDialog : FlatWindow
    {
        #region Constructors / Window actions
        /// <summary>
        /// Create a ImageBrushEditorDialog.
        /// </summary>
        public ImageBrushEditorDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Create a ImageBrushEditorDialog, with the color scheme pre-defined.
        /// </summary>
        public ImageBrushEditorDialog(ColorScheme cs)
        {
            InitializeComponent();
            ColorScheme = cs;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //if (nudStartX == null) return;

            //RunUpdateAction(() =>
            //{
            //    nudStartX.Value = edtPoints.SelectedWidth1;
            //    nudStartY.Value = edtPoints.SelectedHeight1;

            //    nudEndX.Value = edtPoints.SelectedWidth2;
            //    nudEndY.Value = edtPoints.SelectedHeight2;
            //});
        }

        /// <summary>Get or set the result the user selected for this dialog; <c>true</c> is "OK", <c>false</c> is "Cancel" or the window was closed without making a choice.</summary>
        public new bool DialogResult { get; set; } = false;

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        #endregion

        #region Image Source box/rendering

        ImageSource _source = new BitmapImage();
        bool _blankSourceOnEnter = false;
        //bool _newSourceSet = false;
        bool _internalAction = false;
        private void txtSource_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_internalAction) return;
            if (_blankSourceOnEnter)
            {
                _internalAction = true;
                txtSource.Text = "";
                _internalAction = false;
                _blankSourceOnEnter = false;
                txtSource.FontStyle = FontStyles.Normal;
            }

            if (!string.IsNullOrWhiteSpace(txtSource.Text))
            {
#if (NETCOREAPP || NET45_OR_GREATER)
                if (Uri.IsWellFormedUriString(System.Net.WebUtility.UrlEncode(txtSource.Text), UriKind.RelativeOrAbsolute))
#else
                if (Uri.IsWellFormedUriString(txtSource.Text, UriKind.RelativeOrAbsolute))
#endif
                {
                    try
                    {
                        //_newSourceSet = true;
                        BitmapImage bi = new BitmapImage(new Uri(txtSource.Text));
                        LoadImageSource(bi);
                    }
                    catch (UriFormatException)
                    {

                    }
                }
            }
        }

        private void btnSourceSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.jfif;*.bmp;*.gif;*.ico;*.wmp;*.tif;*.tiff|All Files|*.*";
            bool? res = ofd.ShowDialog();
            if (res.GetValueOrDefault(false))
            {
                _blankSourceOnEnter = false;
                txtSource.FontStyle = FontStyles.Normal;
                txtSource.Text = ofd.FileName;
            }
        }

        void LoadImageSource(ImageSource isrc)
        {
            // TODO: do some testing to make sure the image source is valid

            _source = isrc;
            _internalAction = true;
            if (isrc == null)
            {
                // I don't know
                txtSource.Text = "(image source is null, click here to change to a URL or file path)";
                txtSource.FontStyle = FontStyles.Italic;
                _blankSourceOnEnter = true;
                imgSize.Text = "- x -";
                imgSize.ToolTip = null;
            }
            else if (isrc is BitmapImage bi)
            {
                // https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.bitmapimage.urisource
                if (bi.UriSource != null)
                {
                    txtSource.Text = bi.UriSource.ToString();
                    txtSource.FontStyle = FontStyles.Normal;
                    _blankSourceOnEnter = false;
                }
                else
                {
                    // stream source
                    txtSource.Text = "(image from stream, click here to change to a URL or file path)";
                    txtSource.FontStyle = FontStyles.Italic;
                    _blankSourceOnEnter = true;
                }
                imgSize.Text = Math.Round(bi.Height, 2).ToString() + " x " + Math.Round(bi.Width, 2).ToString();
                imgSize.ToolTip = "Device independent size: " + bi.Height.ToString() + " x " + bi.Width.ToString() + "\n" + "Actual pixel size: " + bi.PixelHeight.ToString() + " x " + bi.PixelWidth.ToString();
            }
            else if (isrc is BitmapSource bs)
            {
                // bitmap source
                txtSource.Text = "(image from bitmap source, click here to change to a URL or file path)";
                txtSource.FontStyle = FontStyles.Italic;
                _blankSourceOnEnter = true;
                imgSize.Text = Math.Round(bs.Height, 2).ToString() + " x " + Math.Round(bs.Width, 2).ToString();
                imgSize.ToolTip = "Device independent size: " + bs.Height.ToString() + " x " + bs.Width.ToString() + "\n" + "Actual pixel size: " + bs.PixelHeight.ToString() + " x " + bs.PixelWidth.ToString();
            }
            else if (isrc is DrawingImage di)
            {
                // maybe in the future, I can display some options or settings
                txtSource.Text = "(image from drawing, click here to change to a URL or file path)";
                txtSource.FontStyle = FontStyles.Italic;
                _blankSourceOnEnter = true;
                imgSize.Text = Math.Round(di.Height, 2).ToString() + " x " + Math.Round(di.Width, 2).ToString();
                imgSize.ToolTip = null;
            }
            else
            {
                // I don't know
                txtSource.Text = "(image from unknown source, click here to change to a URL or file path)";
                txtSource.FontStyle = FontStyles.Italic;
                _blankSourceOnEnter = true;
                imgSize.Text = Math.Round(isrc.Height, 2).ToString() + " x " + Math.Round(isrc.Width, 2).ToString();
                imgSize.ToolTip = null;
            }
            imgSource.Source = isrc;
            _internalAction = false;
        }
#endregion

        #region Load/Save brush

        /// <summary>
        /// Load in an ImageBrush into this dialog.
        /// </summary>
        public void LoadImage(ImageBrush br)
        {
            // load in source
            LoadImageSource(br.ImageSource);

            // load in basic properties
            cbbStretch.SelectedEnumValue = br.Stretch;
            cbbTileMode.SelectedEnumValue = br.TileMode;
            nudOpacity.Value = br.Opacity;

            // okay, now the viewport properties
            cbbAlignmentX.SelectedEnumValue = br.AlignmentX;
            cbbAlignmentY.SelectedEnumValue = br.AlignmentY;
            cbbViewUnits.SelectedEnumValue = br.ViewboxUnits;
            cbbPortUnits.SelectedEnumValue = br.ViewportUnits;

            if (br.ViewportUnits == BrushMappingMode.Absolute)
            {
                nudPortAH.Value = br.Viewport.Height;
                nudPortAW.Value = br.Viewport.Width;
                nudPortAX.Value = br.Viewport.X;
                nudPortAY.Value = br.Viewport.Y;
            }
            else
            {
                nudPortH.Value = br.Viewport.Height;
                nudPortW.Value = br.Viewport.Width;
                nudPortX.Value = br.Viewport.X;
                nudPortY.Value = br.Viewport.Y;
            }

            if (br.ViewboxUnits == BrushMappingMode.Absolute)
            {
                nudViewAH.Value = br.Viewbox.Height;
                nudViewAW.Value = br.Viewbox.Width;
                nudViewAX.Value = br.Viewbox.X;
                nudViewAY.Value = br.Viewbox.Y;
            }
            else
            {
                nudViewH.Value = br.Viewbox.Height;
                nudViewW.Value = br.Viewbox.Width;
                nudViewX.Value = br.Viewbox.X;
                nudViewY.Value = br.Viewbox.Y;
            }
        }

        /// <summary>
        /// Get an ImageBrush that represents the current options in the dialog.
        /// </summary>
        public ImageBrush GetImageBrush()
        {
            ImageBrush ibr = new ImageBrush(_source);

            ibr.Opacity = nudOpacity.Value;
            ibr.Stretch = cbbStretch.SelectedEnumValueAsEnum<Stretch>();
            ibr.TileMode = cbbTileMode.SelectedEnumValueAsEnum<TileMode>();
            ibr.AlignmentX = cbbAlignmentX.SelectedEnumValueAsEnum<AlignmentX>();
            ibr.AlignmentY = cbbAlignmentY.SelectedEnumValueAsEnum<AlignmentY>();
            ibr.ViewportUnits = cbbPortUnits.SelectedEnumValueAsEnum<BrushMappingMode>();
            ibr.ViewboxUnits = cbbViewUnits.SelectedEnumValueAsEnum<BrushMappingMode>();

            if (ibr.ViewportUnits == BrushMappingMode.Absolute)
            {
                ibr.Viewport = new Rect(nudPortAX.Value, nudPortAY.Value, nudPortAW.Value, nudPortAH.Value);
            }
            else
            {
                ibr.Viewport = new Rect(nudPortX.Value, nudPortY.Value, nudPortW.Value, nudPortH.Value);
            }

            if (ibr.ViewboxUnits == BrushMappingMode.Absolute)
            {
                ibr.Viewbox = new Rect(nudViewAX.Value, nudViewAY.Value, nudViewAW.Value, nudViewAH.Value);
            }
            else
            {
                ibr.Viewbox = new Rect(nudViewX.Value, nudViewY.Value, nudViewW.Value, nudViewH.Value);
            }

            return ibr;
        }

        #endregion

        private void cbbStretch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void cbbTileMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0060 // Remove unused parameter
        private void nudOpacity_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdatePreview();
        }
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0051 // Remove unused private members

        void UpdatePreview()
        {
            if (imgSource == null) return;
            imgSource.Stretch = cbbStretch.SelectedEnumValueAsEnum<Stretch>();
            imgSource.Opacity = nudOpacity?.Value ?? 1.0;
        }

        private void cbbPortUnits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Visibility va = Visibility.Collapsed;
            Visibility vr = Visibility.Visible;

            if ((BrushMappingMode)cbbPortUnits.SelectedEnumValue == BrushMappingMode.Absolute)
            {
                va = Visibility.Visible;
                vr = Visibility.Collapsed;
            }

            nudPortAH.Visibility = va;
            nudPortAW.Visibility = va;
            nudPortAX.Visibility = va;
            nudPortAY.Visibility = va;

            nudPortH.Visibility = vr;
            nudPortW.Visibility = vr;
            nudPortX.Visibility = vr;
            nudPortY.Visibility = vr;
        }

        private void cbbViewUnits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Visibility va = Visibility.Collapsed;
            Visibility vr = Visibility.Visible;

            if ((BrushMappingMode)cbbViewUnits.SelectedEnumValue == BrushMappingMode.Absolute)
            {
                va = Visibility.Visible;
                vr = Visibility.Collapsed;
            }

            nudViewAH.Visibility = va;
            nudViewAW.Visibility = va;
            nudViewAX.Visibility = va;
            nudViewAY.Visibility = va;

            nudViewH.Visibility = vr;
            nudViewW.Visibility = vr;
            nudViewX.Visibility = vr;
            nudViewY.Visibility = vr;
        }
    }
}
