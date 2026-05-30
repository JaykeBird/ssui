using SolidShineUi.PropertyList.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text; // need this for handling Rune support in .NET
using System.Windows;
using System.Windows.Controls;
using static SolidShineUi.Utils.IconLoader;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A proeprty editor for editing <see cref="char"/> objects.
    /// </summary>
    public partial class CharEditor : UserControl, IPropertyEditor
    {
        /// <summary>Create a CharEditor.</summary>
        public CharEditor()
        {
            InitializeComponent();

            // set up strings
            mnuSetNull.Header = Strings.SetAsNull;
            mnuMultiline.Header = Strings.EnterInUnicodeValue;
        }

        private Type _itemType = typeof(char);

        /// <inheritdoc/>
#if NETCOREAPP
        public List<Type> ValidTypes => (new[] { typeof(char), typeof(Rune) }).ToList();

        private object? _value = null;
#else
        public List<Type> ValidTypes => (new[] { typeof(char) }).ToList();

        private object _value = null;
#endif

        bool _internalAction = false;

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public void SetHostControl(IPropertyEditorHost host) { _parent = host; }

        // ColorScheme _cs = new ColorScheme();
#if NETCOREAPP
        IPropertyEditorHost? _parent = null;
#else
        IPropertyEditorHost _parent = null;
#endif

        /// <inheritdoc/>
        public void ApplySsuiTheme(SsuiTheme theme)
        {
            btnMenu.SsuiTheme = theme;
            imgMenu.Source = LoadIcon("ThreeDots", theme.IconVariation);
        }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        /// <inheritdoc/>
        public bool IsPropertyWritable
        {
            get => btnMenu.IsEnabled;
            set 
            { 
                btnMenu.IsEnabled = value;
                txtText.IsEnabled = value && !nullSet;
            }
        }

        bool nullSet = false;

        void SetAsNull()
        {
            nullSet = true;
            txtText.IsEnabled = false;
            mnuSetNull.IsChecked = true;
            txtText.Text = Strings.Null;
            txtValue.Text = "";
        }

        void UnsetAsNull()
        {
            // do not set as null
            nullSet = false;
            mnuSetNull.IsChecked = false;

            if (_itemType == typeof(char?) || _itemType == typeof(char))
            {
                if (char.IsSurrogate((char)(_value ?? 'a')))
                {
                    txtText.IsEnabled = false;
                    txtText.Text = Strings.Surrogate;
                }
                else
                {
                    txtText.IsEnabled = true;
                    txtText.Text = ((char)(_value ?? 'a')).ToString();
                }

                txtValue.Text = ((int)(char)(_value ?? 'a')).ToString("X4", NumberFormatInfo.CurrentInfo);
            }
#if NETCOREAPP
            else if (_itemType == typeof(Rune?) || _itemType == typeof(Rune))
            {

                txtText.IsEnabled = true;
                txtText.Text = ((Rune)(_value ?? 'a')).ToString();
                txtValue.Text = ((Rune)(_value ?? 'a')).Value.ToString("X4", NumberFormatInfo.CurrentInfo);
            }
#endif
            else
            {
                // uhhh
                txtText.IsEnabled = false;

            }
        }

        private void mnuSetNull_Click(object sender, RoutedEventArgs e)
        {
            _internalAction = true;
            if (mnuSetNull.IsChecked)
            {
                UnsetAsNull();
            }
            else
            {
                // do set as null
                SetAsNull();
            }
            _internalAction = false;
            ValueChanged?.Invoke(this, EventArgs.Empty);
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
            if (nullSet)
            {
                return null;
            }
            else
            {
                return _value;
            }
        }

        /// <inheritdoc/>
#if NETCOREAPP
        public void LoadValue(object? value, Type type)
#else
        public void LoadValue(object value, Type type)
#endif
        {
            _internalAction = true;
            _itemType = type;
            if (type == typeof(char))
            {
                char c = (char)(value ?? '\0');
                if (char.IsSurrogate(c))
                {
                    txtText.Text = Strings.Surrogate;
                    txtText.IsEnabled = false;
                }
                else
                {
                    txtText.Text = c.ToString();
                    txtText.IsEnabled = true;
                }
                _value = c;
                txtValue.Text = ((int)c).ToString("X4", NumberFormatInfo.CurrentInfo);
            }
            else if (type == typeof(char?))
            {
                char? c = (char?)(value ?? null);
                mnuSetNull.IsEnabled = true;
                _value = c;
                if (c == null)
                {
                    SetAsNull();
                    _value = 'a'; // set that if the user unsets the property from null, there's a default value that's put in
                }
                else
                {
                    if (char.IsSurrogate(c ?? '\0'))
                    {
                        txtText.Text = Strings.Surrogate;
                        txtText.IsEnabled = false;
                    }
                    else
                    {
                        txtText.Text = c.ToString();
                        txtText.IsEnabled = true;
                    }
                    txtValue.Text = ((int)(c ?? '\0')).ToString("X4", NumberFormatInfo.CurrentInfo);
                }
            }
#if NETCOREAPP
            else if (type == typeof(Rune))
            {
                Rune r = (Rune)(value ?? '\0');
                txtText.Text = r.ToString();
                _value = r;
                txtValue.Text = r.Value.ToString("X4", NumberFormatInfo.CurrentInfo);
            }
            else if (type == typeof(Rune?))
            {
                Rune? r = (Rune?)(value ?? null);
                mnuSetNull.IsEnabled = true;
                _value = r;
                if (r == null)
                {
                    SetAsNull();
                    _value = 'a'; // set that if the user unsets the property from null, there's a default value that's put in
                }
                else
                {
                    txtText.Text = (r ?? (Rune)'a').ToString();
                    txtValue.Text = (r ?? (Rune)'a').Value.ToString("X4", NumberFormatInfo.CurrentInfo);
                }
            }
#endif
            else
            {
                // this isn't a char or Rune?
                _value = value;
                txtText.Text = Strings.NotAChar;
                txtText.IsEnabled = false;
                txtValue.Text = "";
            }
            _internalAction = false;
        }

        private void txtText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_internalAction) return;

            if (_itemType == typeof(char) || _itemType == typeof(char?))
            {
                string val = txtText.Text;
                if (val.Length <= 0)
                {
                    _value = '\0';
                }
                else if (val.Length == 1)
                {
                    _value = val[0];
                }
                else
                {
                    // length is more than 1
                    _value = val[0];
                    _internalAction = true;
                    txtText.Text = val[0].ToString();
                    _internalAction = false;
                }
                txtValue.Text = ((int)(char)(_value ?? '\0')).ToString("X4", NumberFormatInfo.CurrentInfo);
            }
#if NETCOREAPP
            else if (_itemType == typeof(Rune) || _itemType == typeof(Rune?))
            {
                string val = txtText.Text;
                if (val.Length <= 0)
                {
                    _value = new Rune('\0');
                }
                else if (val.Length == 1)
                {
                    _value = Rune.GetRuneAt(val, 0);
                }
                else
                {
                    // length is more than 1
                    _value = Rune.GetRuneAt(val, 0);
                    _internalAction = true;
                    txtText.Text = _value.ToString();
                    _internalAction = false;
                }
                txtValue.Text = ((Rune)(_value ?? '\0')).Value.ToString("X4", NumberFormatInfo.CurrentInfo);
            }
#endif
            else
            {
                _internalAction = true;
                txtText.Text = Strings.NotAChar;
                _internalAction = false;
                txtText.IsEnabled = false;
            }

            ValueChanged?.Invoke(this, e);
        }

        private void mnuMultiline_Click(object sender, RoutedEventArgs e)
        {
#if NETCOREAPP
            if (_itemType == typeof(Rune) || _itemType == typeof(Rune?))
            {
                CharInputDialog sid = new CharInputDialog((Rune)(_value ?? '\0'));
                sid.Title = "Enter Rune as Unicode";
                sid.Owner = _parent?.GetWindow() ?? Window.GetWindow(this);
                sid.SsuiTheme = _parent?.GetThemeForDialogs() ?? new SsuiAppTheme();
                sid.ShowDialog();

                if (sid.DialogResult)
                {
                    _internalAction = true;
                    Rune r = sid.ValueAsRune;
                    txtText.Text = r.ToString();
                    _value = r;
                    txtValue.Text = r.Value.ToString("X4", NumberFormatInfo.CurrentInfo);
                    _internalAction = false;
                }
            }
            else
            {
                LoadDialogAsChar();
            }
#else
            LoadDialogAsChar();
#endif

            void LoadDialogAsChar()
            {
                CharInputDialog sid = new CharInputDialog((char)(_value ?? '\0'));
                sid.Owner = _parent?.GetWindow() ?? Window.GetWindow(this);
                sid.SsuiTheme = _parent?.GetThemeForDialogs() ?? new SsuiAppTheme();
                sid.ShowDialog();

                if (sid.DialogResult)
                {
                    _internalAction = true;
                    char c = sid.ValueAsChar;
                    if (char.IsSurrogate(c))
                    {
                        txtText.Text = Strings.Surrogate;
                        txtText.IsEnabled = false;
                    }
                    else
                    {
                        txtText.Text = c.ToString();
                        txtText.IsEnabled = true;
                    }
                    _value = c;
                    txtValue.Text = ((int)c).ToString("X4", NumberFormatInfo.CurrentInfo);
                    _internalAction = false;
                }
            }
        }

        private void mnuCharmap_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("C:\\WINDOWS\\system32\\charmap.exe"))
            {
                Process.Start("C:\\WINDOWS\\system32\\charmap.exe");
            }
        }
    }
}
