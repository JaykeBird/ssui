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
    /// A property editor for <see cref="FontWeight"/> objects.
    /// </summary>
    public partial class FontWeightEditor : UserControl, IPropertyEditor
    {
        /// <summary>
        /// Create a FontWeightEditor.
        /// </summary>
        public FontWeightEditor()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public List<Type> ValidTypes => (new[] { typeof(FontWeight), typeof(FontWeight?) }).ToList();

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public bool IsPropertyWritable
        {
            get => btnMenu.IsEnabled;
            set
            {
                nudWeight.IsEnabled = value;
                cbbWeight.IsEnabled = value;
                btnMenu.IsEnabled = value;
            }
        }

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
            nudWeight.ColorScheme = cs;
            btnMenu.ColorScheme = cs;
            imgItalic.Source = LoadIcon("TextBold", cs);
            imgMenu.Source = LoadIcon("ThreeDots", cs);
        }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        bool _raiseEvents = false;

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0051 // Remove unused private members

        private void cbbStyles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_raiseEvents)
            {
                ValueChanged?.Invoke(this, e);
            }
        }

        private void nudWeight_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_raiseEvents)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _raiseEvents = true;
        }

#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0051 // Remove unused private members

        private FontWeight GetWeightFromSelection()
        {
            // https://learn.microsoft.com/en-us/dotnet/api/system.windows.fontweights?view=windowsdesktop-6.0

            //<ComboBoxItem Content="Thin" /> <!-- 100 -->
            //<ComboBoxItem Content="ExtraLight" />  <!-- 200 -->
            //<ComboBoxItem Content="Light" /> <!-- 300 -->
            //<ComboBoxItem Content="Normal" /> <!-- 400 -->
            //<ComboBoxItem Content="Medium" /> <!-- 500 -->
            //<ComboBoxItem Content="SemiBold" /> <!-- 600 -->
            //<ComboBoxItem Content="Bold" /> <!-- 700 -->
            //<ComboBoxItem Content="ExtraBold" /> <!-- 800 -->
            //<ComboBoxItem Content="Black" /> <!-- 900 -->
            //<ComboBoxItem Content="ExtraBlack" /> <!-- 950 -->

            switch (cbbWeight.SelectedIndex)
            {
                case 0:
                    return FontWeights.Thin;
                case 1:
                    return FontWeights.ExtraLight;
                case 2:
                    return FontWeights.Light;
                case 3:
                    return FontWeights.Normal;
                case 4:
                    return FontWeights.Medium;
                case 5:
                    return FontWeights.SemiBold;
                case 6:
                    return FontWeights.Bold;
                case 7:
                    return FontWeights.ExtraBold;
                case 8:
                    return FontWeights.Black;
                case 9:
                    return FontWeights.ExtraBlack;
                default:
                    return FontWeights.Normal;
            }
        }

        void SetSelectionFromWeight(FontWeight fw)
        {
            if (fw == FontWeights.Thin)
            {
                cbbWeight.SelectedIndex = 0;
            }
            else if (fw == FontWeights.ExtraLight || fw == FontWeights.UltraLight)
            {
                cbbWeight.SelectedIndex = 1;
            }
            else if (fw == FontWeights.Light)
            {
                cbbWeight.SelectedIndex = 2;
            }
            else if (fw == FontWeights.Normal || fw == FontWeights.Regular)
            {
                cbbWeight.SelectedIndex = 3;
            }
            else if (fw == FontWeights.Medium)
            {
                cbbWeight.SelectedIndex = 4;
            }
            else if (fw == FontWeights.DemiBold || fw == FontWeights.SemiBold)
            {
                cbbWeight.SelectedIndex = 5;
            }
            else if (fw == FontWeights.Bold)
            {
                cbbWeight.SelectedIndex = 6;
            }
            else if (fw == FontWeights.ExtraBold || fw == FontWeights.UltraBold)
            {
                cbbWeight.SelectedIndex = 7;
            }
            else if (fw == FontWeights.Black || fw == FontWeights.Heavy)
            {
                cbbWeight.SelectedIndex = 8;
            }
            else if (fw == FontWeights.ExtraBlack || fw == FontWeights.UltraBlack)
            {
                cbbWeight.SelectedIndex = 9;
            }
            else
            {
                SetToIntegerMode();
                nudWeight.Value = fw.ToOpenTypeWeight();
            }
        }

#if NETCOREAPP
        /// <inheritdoc/>
        public event EventHandler? ValueChanged;

        /// <inheritdoc/>
        public object? GetValue()
        {
            if (mnuNull.IsChecked)
            {
                return null;
            }
            else if (cbbWeight.Visibility == Visibility.Visible)
            {
                return GetWeightFromSelection();
            }
            else
            {
                return FontWeight.FromOpenTypeWeight(nudWeight.Value);
            }
        }

        /// <inheritdoc/>
        public void LoadValue(object? value, Type type)
        {
            if (type == typeof(FontWeight?))
            {
                mnuNull.IsEnabled = true;
            }

            _raiseEvents = false;
            if (value == null)
            {
                cbbWeight.SelectedIndex = 3;
                SetAsNull();
            }
            else if (value is FontWeight fw)
            {
                SetSelectionFromWeight(fw);
            }
            else
            {
                cbbWeight.SelectedIndex = 3;
            }
            _raiseEvents = true;
        }
#else
        /// <inheritdoc/>
        public event EventHandler ValueChanged;
        
        /// <inheritdoc/>
        public object GetValue()
        {
            if (mnuNull.IsChecked)
            {
                return null;
            }
            else if (cbbWeight.Visibility == Visibility.Visible)
            {
                return GetWeightFromSelection();
            }
            else
            {
                return FontWeight.FromOpenTypeWeight(nudWeight.Value);
            }
        }
        
        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
        {
            _raiseEvents = false;
            if (value == null)
            {
                cbbWeight.SelectedIndex = 3;
            }
            else if (value is FontWeight fw)
            {
                SetSelectionFromWeight(fw);
            }
            else
            {
                cbbWeight.SelectedIndex = 3;
            }
            _raiseEvents = true;
        }
#endif

        void SetToIntegerMode()
        {
            _raiseEvents = false;
            cbbWeight.Visibility = Visibility.Collapsed;
            nudWeight.Visibility = Visibility.Visible;
            nudWeight.Value = GetWeightFromSelection().ToOpenTypeWeight();

            mnuDropdown.IsChecked = false;
            mnuInteger.IsChecked = true;
            _raiseEvents = true;
        }

        void SetToDropDownMode()
        {
            _raiseEvents = false;
            cbbWeight.Visibility = Visibility.Visible;
            nudWeight.Visibility = Visibility.Collapsed;

            mnuDropdown.IsChecked = true;
            mnuInteger.IsChecked = false;
            _raiseEvents = true;
        }

        private void mnuDropdown_Click(object sender, RoutedEventArgs e)
        {
            SetToDropDownMode();
        }

        private void mnuInteger_Click(object sender, RoutedEventArgs e)
        {
            if (nudWeight.Visibility == Visibility.Visible) return;
            SetToIntegerMode();
        }

        void SetAsNull()
        {
            mnuNull.IsEnabled = true;
            mnuNull.IsChecked = true;
            nudWeight.IsEnabled = false;
            cbbWeight.IsEnabled = false;
        }

        void UnsetAsNull()
        {
            mnuNull.IsChecked = false;
            nudWeight.IsEnabled = true;
            cbbWeight.IsEnabled = true;
        }

        private void mnuNull_Click(object sender, RoutedEventArgs e)
        {
            if (mnuNull.IsChecked)
            {
                UnsetAsNull();
            }
            else
            {
                SetAsNull();
            }
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
