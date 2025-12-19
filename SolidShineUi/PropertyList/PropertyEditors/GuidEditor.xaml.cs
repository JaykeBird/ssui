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
    /// A property editor for editing <see cref="Guid"/> values.
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
        public void SetHostControl(IPropertyEditorHost host) { _host = host; }

#if NETCOREAPP
        private IPropertyEditorHost? _host = null;
#else
        private IPropertyEditorHost _host = null;
#endif

        /// <inheritdoc/>
        public void ApplySsuiTheme(SsuiTheme cs)
        {
            btnMenu.SsuiTheme = cs;

            imgNew.Source = LoadIcon("Reload", cs.IconVariation);
            imgFontEdit.Source = LoadIcon("ThreeDots", cs.IconVariation);
        }

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

            sid.SsuiTheme = _host?.GetThemeForDialogs() ?? SsuiThemes.SystemTheme;
            sid.Owner = _host?.GetWindow() ?? Window.GetWindow(this);
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
