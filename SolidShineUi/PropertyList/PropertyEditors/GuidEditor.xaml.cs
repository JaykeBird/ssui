using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// Interaction logic for FontFamilyEditor.xaml
    /// </summary>
    public partial class GuidEditor : UserControl, IPropertyEditor
    {
        public GuidEditor()
        {
            InitializeComponent();
        }

        public List<Type> ValidTypes => (new[] { typeof(Guid) }).ToList();

        public bool EditorAllowsModifying => true;

        public bool IsPropertyWritable { get => btnEdit.IsEnabled; set => btnEdit.IsEnabled = value; }
        public ColorScheme ColorScheme { set 
            { 
                btnEdit.ColorScheme = value;
                btnMenu.ColorScheme = value;
                _cs = value;

                if (value.BackgroundColor == Colors.Black || value.ForegroundColor == Colors.White)
                {
                    imgMenu.Source = new BitmapImage(new Uri("/SolidShineUi;component/Images/ThreeDotsWhite.png", UriKind.Relative));
                    imgFontEdit.Source = new BitmapImage(new Uri("/SolidShineUi;component/Images/ReloadWhite.png", UriKind.Relative));
                }
                else if (value.BackgroundColor == Colors.White)
                {
                    imgMenu.Source = new BitmapImage(new Uri("/SolidShineUi;component/Images/ThreeDotsBlack.png", UriKind.Relative));
                    imgFontEdit.Source = new BitmapImage(new Uri("/SolidShineUi;component/Images/ReloadBlack.png", UriKind.Relative));
                }
                else
                {
                    imgMenu.Source = new BitmapImage(new Uri("/SolidShineUi;component/Images/ThreeDotsColor.png", UriKind.Relative));
                    imgFontEdit.Source = new BitmapImage(new Uri("/SolidShineUi;component/Images/ReloadColor.png", UriKind.Relative));
                }
            }
        }

        private ColorScheme _cs = new ColorScheme();

        private Guid guid = Guid.Empty;

        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

#if NETCOREAPP
        public event EventHandler? ValueChanged;

        public object? GetValue()
        {
            return guid;
        }

        public void LoadValue(object? value, Type type)
        {
            if (value == null)
            {
                // for this editor, I don't really handle null?
                guid = Guid.Empty;
                txtFontName.Text = guid.ToString("B");
            }
            else if (value is Guid g)
            {
                guid = g;
                txtFontName.Text = guid.ToString("B");
            }
            else
            {
                // this object is not a Guid? what is it here???
                guid = Guid.Empty;
                txtFontName.Text = guid.ToString("B");
            }
        }

#else
        public event EventHandler ValueChanged;

        public object GetValue()
        {
            return guid;
        }

        public void LoadValue(object value, Type type)
        {
            if (value == null)
            {
                // for this editor, I don't really handle null?
                guid = Guid.Empty;
                txtFontName.Text = guid.ToString("B");
            }
            else if (value is Guid g)
            {
                guid = g;
                txtFontName.Text = guid.ToString("B");
            }
            else
            {
                // this object is not a Guid? what is it here???
                guid = Guid.Empty;
                txtFontName.Text = guid.ToString("B");
            }
        }
#endif

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            guid = Guid.NewGuid();
            txtFontName.Text = guid.ToString("B");
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void mnuEmptyGuid_Click(object sender, RoutedEventArgs e)
        {
            guid = Guid.Empty;
            txtFontName.Text = guid.ToString("B");
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void mnuSetGuid_Click(object sender, RoutedEventArgs e)
        {
            StringInputDialog sid = new StringInputDialog();
            sid.Title = "Enter Guid";
            sid.Description = "Enter in a valid Guid for this property:";

            sid.ColorScheme = _cs;
            sid.Owner = Window.GetWindow(this);

            sid.ShowDialog();
            if (sid.DialogResult)
            {
                bool res = Guid.TryParse(sid.Value, out Guid g);
                if (res)
                {
                    guid = g;
                    txtFontName.Text = guid.ToString("B");
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
