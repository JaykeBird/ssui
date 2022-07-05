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
    /// Interaction logic for FontFamilyEditor.xaml
    /// </summary>
    public partial class FontFamilyEditor : UserControl, IPropertyEditor
    {
        public FontFamilyEditor()
        {
            InitializeComponent();
        }

        public List<Type> ValidTypes => (new[] { typeof(FontFamily) }).ToList();

        public bool EditorAllowsModifying => true;

        public bool IsPropertyWritable { get => btnEdit.IsEnabled; set => btnEdit.IsEnabled = value; }

        public ExperimentalPropertyList ParentPropertyList { set { } }

        public ColorScheme ColorScheme { set 
            { 
                btnEdit.ColorScheme = value;
                _cs = value;

                imgFontEdit.Source = LoadIcon("String", value);
            }
        }

        private ColorScheme _cs = new ColorScheme();

        private FontFamily font = new FontFamily("Segoe UI");

        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

#if NETCOREAPP
        public event EventHandler? ValueChanged;

        public object? GetValue()
        {
            return font;
        }

        public void LoadValue(object? value, Type type)
        {
            if (value == null)
            {
                // for this editor, I don't really handle null?
                font = new FontFamily("Segoe UI");
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
                font = new FontFamily("Segoe UI");
                txtFontName.Text = "(no font selected)";
            }
        }

#else
        public event EventHandler ValueChanged;

        public object GetValue()
        {
            return font;
        }

        public void LoadValue(object value, Type type)
        {
            if (value == null)
            {
                // for this editor, I don't really handle null?
                font = new FontFamily("Segoe UI");
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
                font = new FontFamily("Segoe UI");
                txtFontName.Text = "(no font selected)";
            }
        }
#endif

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            FontSelectDialog dlg = new FontSelectDialog();
            dlg.ColorScheme = _cs;
            dlg.Owner = Window.GetWindow(this);

            dlg.ShowDecorations = false;
            dlg.ShowSizes = false;
            dlg.ShowStyles = false;
            dlg.ShowWeights = false;

            dlg.SelectedFontFamily = font;
            dlg.SelectedFontSize = 16.0; // used for the preview box

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
