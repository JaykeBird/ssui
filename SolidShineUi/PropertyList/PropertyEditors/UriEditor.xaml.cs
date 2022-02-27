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
    /// Interaction logic for StringEditor.xaml
    /// </summary>
    public partial class UriEditor : UserControl, IPropertyEditor
    {
        public UriEditor()
        {
            InitializeComponent();
        }

        public bool EditorAllowsModifying => true;

        public ColorScheme ColorScheme
        {
            set
            {
                btnMenu.ColorScheme = value;

                imgMenu.Source = LoadIcon("ThreeDots", value);
            }
        }

        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }
        public bool IsPropertyWritable
        {
            get => btnMenu.IsEnabled;
            set 
            { 
                btnMenu.IsEnabled = value;
                txtText.IsEnabled = value;// && !setAsNull;
            }
        }

        bool setAsNull = false;

        private Uri _uri = new Uri("file://C:/");

        private Type _itemType = typeof(Uri);
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

#if NETCOREAPP
        public event EventHandler? ValueChanged;

        public object? GetValue()
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

        public void LoadValue(object? value, Type type)
        {
            _itemType = type;
            if (type == typeof(Uri))
            {
                if (value == null)
                {
                    _uri = new Uri("file://C:/");
                    _internalAction = true;
                    txtText.Text = "(null)";
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
                        txtText.Text = "(null)";
                        _internalAction = false;
                    }
                }
            }
        }

        private void txtText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_internalAction) return;
            bool res = Uri.TryCreate(txtText.Text, UriKind.RelativeOrAbsolute, out Uri? u);
            if (res)
            {
                ValueChanged?.Invoke(this, e);
                _uriNeedsReset = false;
            }
            else
            {
                _uriNeedsReset = true;
            }
        }
#else
        public event EventHandler ValueChanged;

        public object GetValue()
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

        public void LoadValue(object value, Type type)
        {
            _itemType = type;
            if (type == typeof(Uri))
            {
                if (value == null)
                {
                    _uri = new Uri("file://C:/");
                    txtText.Text = "(null)";
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
                        txtText.Text = "(null)";
                        _internalAction = false;
                    }
                }
            }
        }

        private void txtText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_internalAction) return;
            bool res = Uri.TryCreate(txtText.Text, UriKind.RelativeOrAbsolute, out Uri u);
            if (res)
            {
                ValueChanged?.Invoke(this, e);
                _uriNeedsReset = false;
            }
            else
            {
                _uriNeedsReset = true;
            }
        }
#endif

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
