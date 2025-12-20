using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SolidShineUi.PropertyList.Dialogs;
using SolidShineUi.Utils;

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
        public void SetHostControl(IPropertyEditorHost host) { _host = host; }


        /// <inheritdoc/>
        public void ApplySsuiTheme(SsuiTheme theme)
        {
            btnMenu.SsuiTheme = theme;
            imgMenu.Source = IconLoader.LoadIcon("ThreeDots", theme.IconVariation);
        }

        private Rect rect = Rect.Empty;

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

#if NETCOREAPP
        IPropertyEditorHost? _host = null;

        /// <inheritdoc/>
        public event EventHandler? ValueChanged;

        /// <inheritdoc/>
        public object? GetValue()
#else
        IPropertyEditorHost _host = null;

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
                txtFontName.Text = rect.ToString(null);
            }
            else
            {
                // this object is not a Guid? what is it here???
                rect = Rect.Empty;
                txtFontName.Text = rect.ToString(null);
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
            txtFontName.Text = rect.ToString(null);
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
            txtFontName.Text = rect.ToString(null);
            UnsetAsNull();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void mnuEdit_Click(object sender, RoutedEventArgs e)
        {
            RectEditDialog red = new RectEditDialog();
            red.SetRect(rect);
            red.Owner = _host?.GetWindow();
            red.SsuiTheme = _host?.GetThemeForDialogs() ?? SsuiThemes.SystemTheme;
            red.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            red.ShowDialog();

            if (red.DialogResult)
            {
                rect = red.GetRect();
            }

            txtFontName.Text = rect.ToString(null);
            UnsetAsNull();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void mnuEmpty_Click(object sender, RoutedEventArgs e)
        {
            rect = Rect.Empty;
            txtFontName.Text = rect.ToString(null);
            UnsetAsNull();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
