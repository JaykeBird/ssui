﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// Interaction logic for StringEditor.xaml
    /// </summary>
    public partial class StringEditor : UserControl, IPropertyEditor
    {
        public StringEditor()
        {
            InitializeComponent();
        }

        public bool EditorAllowsModifying => true;

        public ColorScheme ColorScheme
        {
            set
            {
                btnMenu.ColorScheme = value;

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
                txtText.IsEnabled = value && !setAsNull;
            }
        }

        bool setAsNull = false;

        private Type _itemType = typeof(string);
        public List<Type> ValidTypes => (new[] { typeof(string) }).ToList();

        private void mnuSetNull_Click(object sender, RoutedEventArgs e)
        {
            if (mnuSetNull.IsChecked)
            {
                // do not set as null
                setAsNull = false;
                txtText.IsEnabled = true;
                mnuSetNull.IsChecked = false;
            }
            else
            {
                // do set as null
                setAsNull = true;
                txtText.IsEnabled = false;
                mnuSetNull.IsChecked = true;
            }
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

#if NETCOREAPP
        public event EventHandler? ValueChanged;

        public object? GetValue()
        {
            if (_itemType == typeof(string))
            {
                if (setAsNull)
                {
                    return null;
                }
                else
                {
                    return txtText.Text.ToString();
                }
            }
            else return null;
        }

        public void LoadValue(object? value, Type type)
        {
            _itemType = type;
            if (type == typeof(string))
            {
                txtText.Text = (value ?? "").ToString();
            }
        }
#else
        public event EventHandler ValueChanged;

        public object GetValue()
        {
            if (_itemType == typeof(string))
            {
                if (setAsNull)
                {
                    return null;
                }
                else
                {
                    return txtText.Text.ToString();
                }
            }
            else return null;
        }

        public void LoadValue(object value, Type type)
        {
            _itemType = type;
            if (type == typeof(string))
            {
                txtText.Text = (value ?? "").ToString();
            }
        }
#endif

        private void txtText_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

    }
}
