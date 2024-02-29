using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static SolidShineUi.Utils.IconLoader;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A property editor for <see cref="FontStyle"/> objects.
    /// </summary>
    public partial class FontStyleEditor : UserControl, IPropertyEditor
    {
        /// <summary>
        /// Create a FontStyleEditor.
        /// </summary>
        public FontStyleEditor()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public List<Type> ValidTypes => (new[] { typeof(FontStyle), typeof(FontStyle?) }).ToList();

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public bool IsPropertyWritable { get => cbbStyles.IsEnabled; set => cbbStyles.IsEnabled = value; }

        /// <inheritdoc/>
        public ExperimentalPropertyList ParentPropertyList { set { } }

        /// <inheritdoc/>
        public ColorScheme ColorScheme
        { 
            set
            {
                ApplyColorScheme(value);
            }
        }

        /// <inheritdoc/>
        public void ApplyColorScheme(ColorScheme cs)
        {
            imgItalic.Source = LoadIcon("TextItalic", cs);
        }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        bool _raiseEvents = false;

        private void cbbStyles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbStyles.SelectedIndex == 3 && !canNull)
            {
                canNull = true;
            }

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
        /// <inheritdoc/>
        public event EventHandler? ValueChanged;

        /// <inheritdoc/>
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
                case 3:
                    if (canNull) { return null; }
                    else { return FontStyles.Normal; }
                default:
                    return FontStyles.Normal;
            }
        }

        /// <inheritdoc/>
        public void LoadValue(object? value, Type type)
        {
            _raiseEvents = false;
            if (value == null)
            {
                EnableNull();
                cbbStyles.SelectedIndex = 3;
            }
            else if (value is FontStyle fs)
            {
                if (type == typeof(FontStyle?)) EnableNull();

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
        /// <inheritdoc/>
        public event EventHandler ValueChanged;
        
        /// <inheritdoc/>
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
                case 3:
                    if (canNull) { return null; }
                    else { return FontStyles.Normal; }
                default:
                    return FontStyles.Normal;
            }
        }
        
        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
        {
            _raiseEvents = false;
            if (value == null)
            {
                EnableNull();
                cbbStyles.SelectedIndex = 3;
            }
            else if (value is FontStyle fs)
            {
                if (type == typeof(FontStyle?)) EnableNull();

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

        bool canNull = false;

        void EnableNull()
        {
            cbbNull.Visibility = Visibility.Visible;
            canNull = true;
        }
    }
}
