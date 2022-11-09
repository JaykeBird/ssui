using Microsoft.Win32;
using SolidShineUi.PropertyList;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;

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
            
            if (Uri.IsWellFormedUriString(System.Net.WebUtility.UrlEncode(txtSource.Text), UriKind.RelativeOrAbsolute))
            {
                //_newSourceSet = true;
                BitmapImage bi = new BitmapImage(new Uri(txtSource.Text));
                LoadImageSource(bi);
            }
        }

        private void btnSourceSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.jfif;*.bmp;*.gif;*.ico;*.wmp;*.tif;*.tiff|All Files|*.*";
            bool? res = ofd.ShowDialog();
            if (res.GetValueOrDefault(false))
            {
                txtSource.Text = ofd.FileName;
            }
        }

        void LoadImageSource(ImageSource isrc)
        {
            _source = isrc;
            _internalAction = true;
            if (isrc is BitmapImage bi)
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
            }
            else if (isrc is BitmapSource)
            {
                // bitmap source
                txtSource.Text = "(image from bitmap source, click here to change to a URL or file path)";
                txtSource.FontStyle = FontStyles.Italic;
                _blankSourceOnEnter = true;
            }
            else if (isrc is DrawingImage)
            {
                // maybe in the future, I can display some options or settings
                txtSource.Text = "(image from drawing, click here to change to a URL or file path)";
                txtSource.FontStyle = FontStyles.Italic;
                _blankSourceOnEnter = true;
            }
            else
            {
                // I don't know
                txtSource.Text = "(image from unknown source, click here to change to a URL or file path)";
                txtSource.FontStyle = FontStyles.Italic;
                _blankSourceOnEnter = true;
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

        private void nudOpacity_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdatePreview();
        }

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
