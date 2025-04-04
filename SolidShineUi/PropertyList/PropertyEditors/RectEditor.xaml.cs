using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static SolidShineUi.Utils.IconLoader;
using SolidShineUi.PropertyList.Dialogs;
using System.Security.Cryptography;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A property editor for editing <see cref="Rect"/> values.
    /// </summary>
    public partial class RectEditor : UserControl, IPropertyEditor
    {

        /// <summary>
        /// Create a RectEditor.
        /// </summary>
        public RectEditor()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public List<Type> ValidTypes => (new[] { typeof(Rect), typeof(Rect?) }).ToList();

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public bool IsPropertyWritable { get => btnMenu.IsEnabled; set => btnMenu.IsEnabled = value; }

        /// <inheritdoc/>
        public ExperimentalPropertyList ParentPropertyList { set { } }

        /// <inheritdoc/>
        public ColorScheme ColorScheme { set => ApplyColorScheme(value); }

        /// <inheritdoc/>
        public void ApplyColorScheme(ColorScheme cs)
        {
            btnMenu.ColorScheme = cs;
            _cs = cs;

            if (cs.BackgroundColor == Colors.Black || cs.ForegroundColor == Colors.White)
            {
                // imgNew.Source = LoadIcon("Reload", Utils.IconVariation.White);
                imgFontEdit.Source = LoadIcon("ThreeDots", Utils.IconVariation.White);
            }
            else if (cs.BackgroundColor == Colors.White)
            {
                // imgNew.Source = LoadIcon("Reload", Utils.IconVariation.Black);
                imgFontEdit.Source = LoadIcon("ThreeDots", Utils.IconVariation.Black);
            }
            else
            {
                // imgNew.Source = LoadIcon("Reload", Utils.IconVariation.Color);
                imgFontEdit.Source = LoadIcon("ThreeDots", Utils.IconVariation.Color);
            }
        }

        private ColorScheme _cs = new ColorScheme();

        private Rect rect = Rect.Empty;

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

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
            if (mnuSetNull.IsChecked == true)
            {
                return null;
            }
            else
            {
                return rect;
            }
        }

#if NETCOREAPP
        /// <inheritdoc/>
        public void LoadValue(object? value, Type type)
        {
#else       
        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
        {
#endif
            if (type == typeof(Rect?))
            {
                mnuSetNull.IsEnabled = true;
            }

            if (value == null)
            {
                rect = Rect.Empty;
                SetAsNull();
            }
            else if (value is Rect g)
            {
                rect = g;
                txtFontName.Text = rect.ToString();
            }
            else
            {
                // this object is not a Guid? what is it here???
                rect = Rect.Empty;
                txtFontName.Text = rect.ToString();
            }
        }

        void SetAsNull()
        {
            mnuSetNull.IsEnabled = true;
            mnuSetNull.IsChecked = true;
            txtFontName.Text = "(null)";
        }

        void UnsetAsNull()
        {
            mnuSetNull.IsChecked = false;
            txtFontName.Text = rect.ToString();
        }

        private void mnuSetNull_Click(object sender, RoutedEventArgs e)
        {
            if (mnuSetNull.IsChecked)
            {
                UnsetAsNull();
            }
            else
            {
                SetAsNull();
            }
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void mnu11Rect_Click(object sender, RoutedEventArgs e)
        {
            rect = new Rect(0, 0, 1, 1);
            txtFontName.Text = rect.ToString();
            UnsetAsNull();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void mnuEdit_Click(object sender, RoutedEventArgs e)
        {
            RectEditDialog red = new RectEditDialog(_cs);
            red.SetRect(rect);
            red.Owner = Window.GetWindow(this);
            red.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            red.ShowDialog();

            if (red.DialogResult)
            {
                rect = red.GetRect();
            }

            txtFontName.Text = rect.ToString();
            UnsetAsNull();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void mnuEmpty_Click(object sender, RoutedEventArgs e)
        {
            rect = Rect.Empty;
            txtFontName.Text = rect.ToString();
            UnsetAsNull();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
