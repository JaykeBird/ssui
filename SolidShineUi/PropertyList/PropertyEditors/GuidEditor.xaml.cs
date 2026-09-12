using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SolidShineUi.Utils;
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
        public bool IsPropertyWritable { get => btnMenu.IsEnabled; set => btnMenu.IsEnabled = value; } // _nullable not needed

        /// <inheritdoc/>
        public ExperimentalPropertyList ParentPropertyList { set { } }

        /// <inheritdoc/>
        public ColorScheme ColorScheme { get => _cs; set { ApplyColorScheme(value); } }

        /// <summary>
        /// Set the visual appearance of this control via a ColorScheme.
        /// </summary>
        /// <param name="value">the color scheme to apply</param>
        public void ApplyColorScheme(ColorScheme value)
        {
            btnMenu.ColorScheme = value;
            _cs = value;

            imgNew.Source = LoadIcon("Reload", _cs);
            imgFontEdit.Source = LoadIcon("ThreeDots", _cs);
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
                return guid;
            }
        }

#if NETCOREAPP
        /// <inheritdoc/>
        public void LoadValue(object? value, Type type)
#else        
        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
#endif
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
            sid.Description = "Enter in a valid Guid:";

            sid.ColorScheme = _cs;
            sid.Owner = Window.GetWindow(this);
            sid.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            sid.ValidationFunction = (s) => { return Guid.TryParse(s, out Guid _); };
            sid.ValidationFailureString = "Not a valid Guid";

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
