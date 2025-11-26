using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using SolidShineUi.Utils;
using System.Net;

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
            ColorSchemeChanged += dialog_ColorSchemeChanged;
        }

        /// <summary>
        /// Create a ImageBrushEditorDialog, with the color scheme pre-defined.
        /// </summary>
        public ImageBrushEditorDialog(ColorScheme cs)
        {
            InitializeComponent();
            ColorSchemeChanged += dialog_ColorSchemeChanged;
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

        private void dialog_ColorSchemeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            imgSetFullPort.Source = IconLoader.LoadIcon("FullFill", ColorScheme);
            imgSetFullView.Source = IconLoader.LoadIcon("FullFill", ColorScheme);
            imgOpen.Source = IconLoader.LoadIcon("Open", ColorScheme);
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

        Size imageSize = new Size(0, 0);

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
                if (Uri.IsWellFormedUriString(txtSource.Text, UriKind.Absolute) || File.Exists(txtSource.Text))
#endif
                {
                    try
                    {
                        //_newSourceSet = true;
                        BitmapImage bi = new BitmapImage(new Uri(txtSource.Text), new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.BypassCache));
                        LoadImageSource(bi);
                    }
                    catch (UriFormatException)
                    {

                    }
                    catch (IOException)
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

            // note that support for newer versions of TLS (including the most modern, 1.3) is not available on older versions of .NET Framework
            // most notably, .NET Framework 4.0 only supports TLS 1.0, which is now considered deprecated and should not be used
            // .NET Framework 4.5 added support for TLS 1.1 and 1.2, but support for TLS 1.3 was only added with .NET Framework 4.8
            // for those reasons, most attempts to load images from the internet on these older .NET Framework versions will fail

            // as long as http://nginx.org/nginx.png still loads without an HTTPS redirect, we can use that to test that image loading does work in .NET Framework 4.0
            // (or also test http://www.baidu.com/img/PCtm_d9c8750bed0b3c7d089fa7d55720d6cf.png, or an image on a different site from https://whynohttps.com)

#if (NETCOREAPP || NET45_OR_GREATER)
            string click_here = "click here to type/paste in a URL or file path";
#else
            string click_here = "click here to type/paste in a file path";
#endif

            if (isrc is BitmapSource bsc)
            {
                if (bsc.IsDownloading)
                {
                    imgSize.Text = "Downloading...";
                    imgSize.ToolTip = null;
                    bsc.DownloadFailed += DownloadFailed;
                    bsc.DownloadCompleted += (x, y) => LoadImageSource(bsc);
                    return;
                }
            }

            _source = isrc;
            _internalAction = true;
            if (isrc == null)
            {
                // I don't know
                txtSource.Text = $"(image source is null, {click_here})";
                txtSource.FontStyle = FontStyles.Italic;
                _blankSourceOnEnter = true;
                imgSize.Text = "- x -";
                imgSize.ToolTip = null;
                imageSize = new Size(0, 0);
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
                    txtSource.Text = $"(image from stream, {click_here})";
                    txtSource.FontStyle = FontStyles.Italic;
                    _blankSourceOnEnter = true;
                }
                imageSize = new Size(Math.Round(bi.Width, 2), Math.Round(bi.Height, 2));
                imgSize.Text = $"{imageSize.Height} x {imageSize.Width}";
                imgSize.ToolTip = $"Device independent size: {bi.Height} x {bi.Width}\nActual pixel size: {bi.PixelHeight} x {bi.PixelWidth}";
            }
            else if (isrc is BitmapSource bs)
            {
                // bitmap source
                txtSource.Text = $"(image from bitmap source, {click_here})";
                txtSource.FontStyle = FontStyles.Italic;
                _blankSourceOnEnter = true;
                imageSize = new Size(Math.Round(bs.Width, 2), Math.Round(bs.Height, 2));
                imgSize.Text = $"{imageSize.Height} x {imageSize.Width}";
                imgSize.ToolTip = $"Device independent size: {bs.Height} x {bs.Width}\nActual pixel size: {bs.PixelHeight} x {bs.PixelWidth}";
            }
            else if (isrc is DrawingImage di)
            {
                // maybe in the future, I can display some options or settings
                txtSource.Text = $"(image from drawing, {click_here})";
                txtSource.FontStyle = FontStyles.Italic;
                _blankSourceOnEnter = true;
                imageSize = new Size(Math.Round(di.Width, 2), Math.Round(di.Height, 2));
                imgSize.Text = $"{imageSize.Height} x {imageSize.Width}";
                imgSize.ToolTip = null;
            }
            else
            {
                // I don't know
                txtSource.Text = $"(image from unknown source, {click_here})";
                txtSource.FontStyle = FontStyles.Italic;
                _blankSourceOnEnter = true;
                imageSize = new Size(Math.Round(isrc.Width, 2), Math.Round(isrc.Height, 2));
                imgSize.Text = $"{imageSize.Height} x {imageSize.Width}";
                imgSize.ToolTip = null;
            }
            imgSource.Source = isrc;
            _internalAction = false;
        }

#if NETCOREAPP
        void DownloadFailed(object? sender, ExceptionEventArgs e)
#else
        void DownloadFailed(object sender, ExceptionEventArgs e)
#endif
        {
            MessageDialog md = new MessageDialog(ColorScheme);

            md.ShowDialog($"The image at the above URL could not be downloaded due to:\n\n{e.ErrorException}", owner: this, title: "Image Download Failed",
                image: MessageDialogImage.Error, buttonDisplay: MessageDialogButtonDisplay.Auto);

            var bs = MessageDialogImageConverter.GetImage(MessageDialogImage.Question, MessageDialogImageConverter.MessageDialogImageColor.Color);
            if (bs != null)
            {
                LoadImageSource(bs);
            }
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
            cbbViewUnits.SelectedIndex = br.ViewboxUnits == BrushMappingMode.Absolute ? 0 : 1;
            cbbPortUnits.SelectedIndex = br.ViewportUnits == BrushMappingMode.Absolute ? 0 : 1;

            SetPortUnitsVisibility();
            SetViewUnitsVisibility();

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
            ibr.ViewportUnits = cbbPortUnits.SelectedIndex == 0 ? BrushMappingMode.Absolute : BrushMappingMode.RelativeToBoundingBox;
            ibr.ViewboxUnits = cbbViewUnits.SelectedIndex == 0 ? BrushMappingMode.Absolute : BrushMappingMode.RelativeToBoundingBox;

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

        private void cbbAlignmentX_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

            if (cbbAlignmentX != null && cbbAlignmentY != null)
            {
                switch (cbbAlignmentX.SelectedEnumValueAsEnum<AlignmentX>())
                {
                    case AlignmentX.Left:
                        imgSource.HorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    case AlignmentX.Right:
                        imgSource.HorizontalAlignment = HorizontalAlignment.Right;
                        break;
                    case AlignmentX.Center:
                        imgSource.HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                }
                switch (cbbAlignmentY.SelectedEnumValueAsEnum<AlignmentY>())
                {
                    case AlignmentY.Top:
                        imgSource.VerticalAlignment = VerticalAlignment.Top;
                        break;
                    case AlignmentY.Bottom:
                        imgSource.VerticalAlignment = VerticalAlignment.Bottom;
                        break;
                    case AlignmentY.Center:
                        imgSource.VerticalAlignment = VerticalAlignment.Center;
                        break;
                }
            }
        }

        void SetPortUnitsVisibility()
        {
            Visibility va = Visibility.Collapsed;
            Visibility vr = Visibility.Visible;

            if ((BrushMappingMode)cbbPortUnits.SelectedIndex == 0)
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

        void SetViewUnitsVisibility()
        {
            Visibility va = Visibility.Collapsed;
            Visibility vr = Visibility.Visible;

            if ((BrushMappingMode)cbbViewUnits.SelectedIndex == 0)
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

        private void cbbPortUnits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (nudPortAH == null) return;

            SetPortUnitsVisibility();
        }

        private void cbbViewUnits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (nudViewAH == null) return;

            SetViewUnitsVisibility();
        }

        #region Presets Menu

        void ApplyPreset(TileMode tm, Rect viewBox, BrushMappingMode viewBoxUnits, Rect viewPort, BrushMappingMode viewPortUnits)
        {
            ApplyPreset(tm, viewBox, viewBoxUnits == BrushMappingMode.Absolute, viewPort, viewPortUnits == BrushMappingMode.Absolute);
        }

        void ApplyPreset(TileMode tm, Rect viewBox, bool viewBoxAbsolute, Rect viewPort, bool viewPortAbsolute)
        {
            cbbTileMode.SelectedEnumValue = tm;
            cbbViewUnits.SelectedIndex = viewBoxAbsolute ? 0 : 1;
            cbbPortUnits.SelectedIndex = viewPortAbsolute ? 0 : 1;

            if (viewPortAbsolute)
            {
                nudPortAH.Value = viewPort.Height;
                nudPortAW.Value = viewPort.Width;
                nudPortAX.Value = viewPort.X;
                nudPortAY.Value = viewPort.Y;
            }
            else
            {
                nudPortH.Value = viewPort.Height;
                nudPortW.Value = viewPort.Width;
                nudPortX.Value = viewPort.X;
                nudPortY.Value = viewPort.Y;
            }

            if (viewBoxAbsolute)
            {
                nudViewAH.Value = viewBox.Height;
                nudViewAW.Value = viewBox.Width;
                nudViewAX.Value = viewBox.X;
                nudViewAY.Value = viewBox.Y;
            }
            else
            {
                nudViewH.Value = viewBox.Height;
                nudViewW.Value = viewBox.Width;
                nudViewX.Value = viewBox.X;
                nudViewY.Value = viewBox.Y;
            }
        }

        private static Rect FullRelative = new Rect(0, 0, 1, 1);

        private void mnuPresetNone_Click(object sender, RoutedEventArgs e)
        {
            ApplyPreset(TileMode.None, FullRelative, false, FullRelative, false);
        }

        private void mnuPresetTile_Click(object sender, RoutedEventArgs e)
        {
            ApplyPreset(TileMode.Tile, FullRelative, false, new Rect(0, 0, 0.5, 0.5), false);
        }

        private void mnuPresetTile4_Click(object sender, RoutedEventArgs e)
        {
            ApplyPreset(TileMode.Tile, FullRelative, false, new Rect(0, 0, 0.25, 0.25), false);
        }

        private void mnuPresetFlip1_Click(object sender, RoutedEventArgs e)
        {
            // top
            ApplyPreset(TileMode.FlipY, new Rect(0, 0, 1, 0.5), false, new Rect(0, 0, 1, 0.5), false);
        }

        private void mnuPresetFlip2_Click(object sender, RoutedEventArgs e)
        {
            // bottom
            ApplyPreset(TileMode.FlipY, new Rect(0, 0.5, 1, 0.5), false, new Rect(0, 0.5, 1, 0.5), false);
        }

        private void mnuPresetFlip3_Click(object sender, RoutedEventArgs e)
        {
            // left
            ApplyPreset(TileMode.FlipX, new Rect(0, 0, 0.5, 1), false, new Rect(0, 0, 0.5, 1), false);
        }

        private void mnuPresetFlip4_Click(object sender, RoutedEventArgs e)
        {
            // right
            ApplyPreset(TileMode.FlipX, new Rect(0.5, 0, 0.5, 1), false, new Rect(0.5, 0, 0.5, 1), false);
        }

        #endregion

        private void btnSetFullView_Click(object sender, RoutedEventArgs e)
        {
            if (cbbViewUnits.SelectedIndex == 0)
            {
                // absolute
                nudViewAH.Value = imageSize.Height;
                nudViewAW.Value = imageSize.Width;
                nudViewAX.Value = 0.0;
                nudViewAY.Value = 0.0;
            }
            else
            {
                // relative
                nudViewH.Value = 1.0;
                nudViewW.Value = 1.0;
                nudViewX.Value = 0.0;
                nudViewY.Value = 0.0;
            }
        }

        private void btnSetFullPort_Click(object sender, RoutedEventArgs e)
        {
            if (cbbPortUnits.SelectedIndex == 0)
            {
                // absolute
                nudPortAH.Value = imageSize.Height;
                nudPortAW.Value = imageSize.Width;
                nudPortAX.Value = 0.0;
                nudPortAY.Value = 0.0;
            }
            else
            {
                // relative
                nudPortH.Value = 1.0;
                nudPortW.Value = 1.0;
                nudPortX.Value = 0.0;
                nudPortY.Value = 0.0;
            }
        }
    }
}
