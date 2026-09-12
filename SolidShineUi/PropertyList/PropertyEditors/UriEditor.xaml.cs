using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using static SolidShineUi.Utils.IconLoader;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A property editor for editing <see cref="Uri"/> objects.
    /// </summary>
    public partial class UriEditor : UserControl, IPropertyEditor
    {
        /// <inheritdoc/>
        public UriEditor()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public ExperimentalPropertyList ParentPropertyList { set { } }

        /// <inheritdoc/>
        public ColorScheme ColorScheme { set { ApplyColorScheme(value); } }

        /// <summary>
        /// Set the visual appearance of this control via the ColorScheme.
        /// </summary>
        /// <param name="value">the color scheme to apply</param>
        public void ApplyColorScheme(ColorScheme value)
        {
            btnMenu.ColorScheme = value;

            imgMenu.Source = LoadIcon("ThreeDots", value);
        }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        bool _writable = true;

        /// <inheritdoc/>
        public bool IsPropertyWritable
        {
            get => _writable;
            set 
            {
                _writable = value;
                mnuSelectFile.IsEnabled = value;
                txtText.IsEnabled = value;// && !setAsNull;
                                          // _nullable not needed
            }
        }

        bool setAsNull = false;

        private Uri _uri = new Uri("file://C:/");

        private Type _itemType = typeof(Uri);

        /// <inheritdoc/>
        public List<Type> ValidTypes => (new[] { typeof(Uri) }).ToList();

        private void mnuSetNull_Click(object sender, RoutedEventArgs e)
        {
            //if (mnuSetNull.IsChecked)
            //{
            //    // do not set as null
            //    setAsNull = false;
            //    txtText.IsEnabled = true;
            //    mnuSetNull.IsChecked = false;
            //}
            //else
            //{
            //    // do set as null
            //    setAsNull = true;
            //    txtText.IsEnabled = false;
            //    mnuSetNull.IsChecked = true;
            //}
            //ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        const string strNull = "(null)";

#if NETCOREAPP
        /// <inheritdoc/>
        public event EventHandler? ValueChanged;

        /// <inheritdoc/>
        public object? GetValue()
#else
        /// <inheritdoc/>
        public event EventHandler ValueChanged;

        /// <inheritdoc/>
        public object GetValue()
#endif
        {
            if (_itemType == typeof(Uri))
            {
                if (setAsNull)
                {
                    return null;
                }
                else
                {
                    return _uri;
                }
            }
            else return null;
        }

#if NETCOREAPP
        /// <inheritdoc/>
        public void LoadValue(object? value, Type type)
#else
        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
#endif
        {
            _itemType = type;
            if (type == typeof(Uri))
            {
                if (value == null)
                {
                    _uri = new Uri("file://C:/");
                    _internalAction = true;
                    txtText.Text = strNull;
                    _internalAction = false;
                }
                else
                {
                    Uri u = (Uri)value;
                    if (u != null)
                    {
                        _uri = u;
                        _internalAction = true;
                        txtText.Text = u.ToString();
                        _internalAction = false;
                    }
                    else
                    {
                        _uri = new Uri("file://C:/");
                        _internalAction = true;
                        txtText.Text = strNull;
                        _internalAction = false;
                    }
                }
            }
        }

        private void txtText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_internalAction) return;
#if NETCOREAPP
            bool res = Uri.TryCreate(txtText.Text, UriKind.RelativeOrAbsolute, out Uri? u);
#else
            bool res = Uri.TryCreate(txtText.Text, UriKind.RelativeOrAbsolute, out Uri u);
#endif
            if (res)
            {
#if NETCOREAPP
                _uri = u!;
#else
                _uri = u;
#endif
                ValueChanged?.Invoke(this, e);
                _uriNeedsReset = false;
            }
            else
            {
                _uriNeedsReset = true;
            }
        }

        bool _uriNeedsReset = false;
        bool _internalAction = false;

        private void mnuSelectFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "All Files|*.*";
            ofd.Multiselect = false;
            ofd.Title = "Select File";

            var res = ofd.ShowDialog(Window.GetWindow(this));

            if (res.HasValue && res.Value)
            {
                string file = ofd.FileName;
                Uri u = new Uri(file);
                if (u != null)
                {
                    _uri = u;
                    _internalAction = true;
                    txtText.Text = u.ToString();
                    _internalAction = false;
                }
                else
                {
                    _uri = new Uri("file://C:/");
                    _internalAction = true;
                    txtText.Text = "(null)";
                    _internalAction = false;
                }
            }
        }

        private void mnuOpenUrl_Click(object sender, RoutedEventArgs e)
        {
#if (NETCOREAPP || NET47_OR_GREATER)
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(_uri.AbsoluteUri) { UseShellExecute = true });
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", _uri.AbsoluteUri);
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", _uri.AbsoluteUri);
            }
#else
            Process.Start(new ProcessStartInfo(_uri.AbsoluteUri) { UseShellExecute = true });
#endif
        }

        private void txtText_LostFocus(object sender, RoutedEventArgs e)
        {
            _internalAction = true;
            if (_uriNeedsReset)
            {
                txtText.Text = _uri.ToString();
                _uriNeedsReset = false;
            }
            _internalAction = false;
        }
    }
}
