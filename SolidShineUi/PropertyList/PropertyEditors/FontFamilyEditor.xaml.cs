using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static SolidShineUi.Utils.IconLoader;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A property editor for editing <see cref="FontFamily"/> objects.
    /// </summary>
    public partial class FontFamilyEditor : UserControl, IPropertyEditor
    {
        /// <summary>
        /// Create a FontFamilyEditor.
        /// </summary>
        public FontFamilyEditor()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public List<Type> ValidTypes => (new[] { typeof(FontFamily) }).ToList();

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public bool IsPropertyWritable { get => btnEdit.IsEnabled; set => btnEdit.IsEnabled = value; }

        /// <inheritdoc/>
        public void SetHostControl(IPropertyEditorHost host) { /* _host = host; */ }

        /// <inheritdoc/>
        public ColorScheme ColorScheme { set => ApplyColorScheme(value); }

        /// <inheritdoc/>
        public void ApplyColorScheme(ColorScheme value)
        {
            _cs = value;
            btnEdit.ColorScheme = value;
            imgFontEdit.Source = LoadIcon("Font", value);
        }

        private ColorScheme _cs = new ColorScheme();

#if NETCOREAPP
        private FontFamily? font = new FontFamily("Segoe UI");
#else
        private FontFamily font = new FontFamily("Segoe UI");
#endif

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
        {
            if (font == null)
            {
                return null;
            }
            else
            {
                return font;
            }
        }

        /// <inheritdoc/>
        public void LoadValue(object? value, Type type)
        {
            if (value == null)
            {
                font = null;
                txtFontName.Text = "(null)";
            }
            else if (value is FontFamily f)
            {
                font = f;
                txtFontName.Text = f.Source;
            }
            else
            {
                // this object is not a FontFamily? what is it here???
                font = null;
                txtFontName.Text = "(no font selected)";
            }
        }

#else
        /// <inheritdoc/>
        public event EventHandler ValueChanged;
        
        /// <inheritdoc/>
        public object GetValue()
        {
            return font;
        }
        
        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
        {
            if (value == null)
            {
                font = null;
                txtFontName.Text = "(null)";
            }
            else if (value is FontFamily f)
            {
                font = f;
                txtFontName.Text = f.Source;
            }
            else
            {
                // this object is not a FontFamily? what is it here???
                font = null;
                txtFontName.Text = "(no font selected)";
            }
        }
#endif

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            FontSelectDialog dlg = new FontSelectDialog
            {
                ColorScheme = _cs,
                Owner = Window.GetWindow(this),

                ShowDecorations = false,
                ShowSizes = false,
                ShowStyles = false,
                ShowWeights = false,

                SelectedFontFamily = font ?? new FontFamily("Segoe UI"),
                SelectedFontSize = 16.0 // used for the preview box
            };

            dlg.ShowDialog();

            if (dlg.DialogResult)
            {
                font = dlg.SelectedFontFamily;
                txtFontName.Text = dlg.SelectedFontFamily.Source;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
