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
    /// A property editor for editing <see cref="Guid"/> editors.
    /// </summary>
    public partial class GuidEditor : UserControl, IPropertyEditor
    {

        /// <summary>
        /// Create a GuidEditor.
        /// </summary>
        public GuidEditor()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public List<Type> ValidTypes => (new[] { typeof(Guid), typeof(Guid?) }).ToList();

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public bool IsPropertyWritable { get => btnMenu.IsEnabled; set => btnMenu.IsEnabled = value; }

        /// <inheritdoc/>
        public ExperimentalPropertyList ParentPropertyList { set { } }

        /// <inheritdoc/>
        public ColorScheme ColorScheme { set 
            { 
                btnMenu.ColorScheme = value;
                _cs = value;

                if (value.BackgroundColor == Colors.Black || value.ForegroundColor == Colors.White)
                {
                    imgNew.Source = LoadIcon("Reload", Utils.IconVariation.White);
                    imgFontEdit.Source = LoadIcon("ThreeDots", Utils.IconVariation.White);
                }
                else if (value.BackgroundColor == Colors.White)
                {
                    imgNew.Source = LoadIcon("Reload", Utils.IconVariation.Black);
                    imgFontEdit.Source = LoadIcon("ThreeDots", Utils.IconVariation.Black);
                }
                else
                {
                    imgNew.Source = LoadIcon("Reload", Utils.IconVariation.Color);
                    imgFontEdit.Source = LoadIcon("ThreeDots", Utils.IconVariation.Color);
                }
            }
        }

        private ColorScheme _cs = new ColorScheme();

        private Guid guid = Guid.Empty;

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
            if (mnuSetNull.IsChecked == true)
            {
                return null;
            }
            else
            {
                return guid;
            }
        }

        /// <inheritdoc/>
        public void LoadValue(object? value, Type type)
        {
            if (type == typeof(Guid?))
            {
                mnuSetNull.IsEnabled = true;
            }

            if (value == null)
            {
                guid = Guid.Empty;
                SetAsNull();
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
        /// <inheritdoc/>
        public event EventHandler ValueChanged;
        
        /// <inheritdoc/>
        public object GetValue()
        {
            if (mnuSetNull.IsChecked == true)
            {
                return null;
            }
            else
            {
                return guid;
            }
        }
        
        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
        {
            if (value == null)
            {
                guid = Guid.Empty;
                SetAsNull();
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
            UnsetAsNull();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void mnuEmptyGuid_Click(object sender, RoutedEventArgs e)
        {
            guid = Guid.Empty;
            txtFontName.Text = guid.ToString("B");
            UnsetAsNull();
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
                    UnsetAsNull();
                }
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
            txtFontName.Text = guid.ToString("B");
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
    }
}
