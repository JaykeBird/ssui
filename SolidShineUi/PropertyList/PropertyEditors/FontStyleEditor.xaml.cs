using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// Interaction logic for FontStyleEditor.xaml
    /// </summary>
    public partial class FontStyleEditor : UserControl, IPropertyEditor
    {
        public FontStyleEditor()
        {
            InitializeComponent();
        }

        public List<Type> ValidTypes => (new[] { typeof(FontStyle) }).ToList();

        public bool EditorAllowsModifying => true;

        public bool IsPropertyWritable { get => cbbStyles.IsEnabled; set => cbbStyles.IsEnabled = value; }
        public ColorScheme ColorScheme
        { 
            set
            {
                if (value.BackgroundColor == Colors.Black || value.ForegroundColor == Colors.White)
                {
                    imgItalic.Source = new BitmapImage(new Uri("/SolidShineUi;component/Images/TextItalicWhite.png", UriKind.Relative));
                }
                else if (value.BackgroundColor == Colors.White)
                {
                    imgItalic.Source = new BitmapImage(new Uri("/SolidShineUi;component/Images/TextItalicBlack.png", UriKind.Relative));
                }
                else
                {
                    imgItalic.Source = new BitmapImage(new Uri("/SolidShineUi;component/Images/TextItalicColor.png", UriKind.Relative));
                }
            }
        }

        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        bool _raiseEvents = false;

        private void cbbStyles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_raiseEvents)
            {
                ValueChanged?.Invoke(this, e);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _raiseEvents = true;
        }

#if NETCOREAPP
        public event EventHandler? ValueChanged;

        public object? GetValue()
        {
            switch (cbbStyles.SelectedIndex)
            {
                case 0:
                    return FontStyles.Normal;
                case 1:
                    return FontStyles.Italic;
                case 2:
                    return FontStyles.Oblique;
                default:
                    return FontStyles.Normal;
            }
        }

        public void LoadValue(object? value, Type type)
        {
            _raiseEvents = false;
            if (value == null)
            {
                cbbStyles.SelectedIndex = 0;
            }
            else if (value is FontStyle fs)
            {
                if (fs == FontStyles.Italic)
                {
                    cbbStyles.SelectedIndex = 1;
                }
                else if (fs == FontStyles.Oblique)
                {
                    cbbStyles.SelectedIndex = 2;
                }
                else
                {
                    cbbStyles.SelectedIndex = 0;
                }
            }
            else
            {
                cbbStyles.SelectedIndex = 0;
            }
            _raiseEvents = true;
        }
#else
        public event EventHandler ValueChanged;

        public object GetValue()
        {
            switch (cbbStyles.SelectedIndex)
            {
                case 0:
                    return FontStyles.Normal;
                case 1:
                    return FontStyles.Italic;
                case 2:
                    return FontStyles.Oblique;
                default:
                    return FontStyles.Normal;
            }
        }

        public void LoadValue(object value, Type type)
        {
            _raiseEvents = false;
            if (value == null)
            {
                cbbStyles.SelectedIndex = 0;
            }
            else if (value is FontStyle fs)
            {
                if (fs == FontStyles.Italic)
                {
                    cbbStyles.SelectedIndex = 1;
                }
                else if (fs == FontStyles.Oblique)
                {
                    cbbStyles.SelectedIndex = 2;
                }
                else
                {
                    cbbStyles.SelectedIndex = 0;
                }
            }
            else
            {
                cbbStyles.SelectedIndex = 0;
            }
            _raiseEvents = true;
        }
#endif
    }
}
