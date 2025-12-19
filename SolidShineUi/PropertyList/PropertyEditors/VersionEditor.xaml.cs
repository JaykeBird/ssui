using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using static SolidShineUi.Utils.IconLoader;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A property editor for editing <see cref="Version"/> objects.
    /// </summary>
    public partial class VersionEditor : UserControl, IPropertyEditor
    {
        // if using this to backport to your code using Solid Shine UI 1.9.x, replace usages of "SsuiTheme" below with "ColorScheme"
        // you will also need to refer back to the source code for 1.9.10's VersionEditor to fix the other erroring lines/sections
        // (such as the lines in mnuParse_Click, the missing ParentPropertyList property, and IPropertyEditorHost not existing)

        /// <inheritdoc/>
        public List<Type> ValidTypes => new List<Type> { typeof(Version) };

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public bool IsPropertyWritable
        {
            get => nudLeft.IsEnabled;
            set
            {
                nudLeft.IsEnabled = value;
                nudTop.IsEnabled = value;
                nudBottom.IsEnabled = value;
                nudRight.IsEnabled = value;
                btnMenu.IsEnabled = value;
            }
        }

        /// <inheritdoc/>
        public void SetHostControl(IPropertyEditorHost host) { _host = host; }

#if NETCOREAPP
        private IPropertyEditorHost? _host = null;
#else
        private IPropertyEditorHost _host = null;
#endif
        
        /// <inheritdoc/>
        public void ApplySsuiTheme(SsuiTheme value)
        {
            nudLeft.SsuiTheme = value;
            nudTop.SsuiTheme = value;
            nudRight.SsuiTheme = value;
            nudBottom.SsuiTheme = value;
            btnMenu.SsuiTheme = value;

            imgFontEdit.Source = LoadIcon("ThreeDots", value.IconVariation);
        }

        /// <summary>
        /// Create a VersionEditor.
        /// </summary>
        public VersionEditor()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement() { return this; }

#if NETCOREAPP
        /// <inheritdoc/>
        public event EventHandler? ValueChanged;

        /// <inheritdoc/>
        public void LoadValue(object? value, Type type)
#else
        /// <inheritdoc/>
        public event EventHandler ValueChanged;

        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
#endif
        {
            if (type == typeof(Version))
            {
                if (value != null)
                {
                    if (value is Version t)
                    {
                        _internalAction = true;
                        nudLeft.Value = t.Major;
                        nudTop.Value = t.Minor;
                        nudRight.Value = t.Build;
                        nudBottom.Value = t.Revision;
                        _internalAction = false;
                    }
                    else
                    {
                        // uhhh?
                        SetAllToValue(0);
                    }
                }
                else
                {
                    // null? treat it as Thickness of value 0
                    SetAllToValue(0);
                }
            }
            else
            {
                // uhhh?
                SetAllToValue(0);
            }
        }

#if NETCOREAPP
        /// <inheritdoc/>
        public object? GetValue()
#else
        /// <inheritdoc/>
        public object GetValue()
#endif
        {
            if (nudBottom.Value < 0)
            {
                // there is no revision number
                if (nudRight.Value < 0)
                {
                    // no revision or build number
                    return new Version(nudLeft.Value, nudTop.Value);
                }
                else
                {
                    return new Version(nudLeft.Value, nudTop.Value, nudRight.Value);
                }
            }
            else
            {
                if (nudRight.Value < 0)
                {
                    // can't have a -1 build number but not a -1 revision number
                    // the better idea may be to make revision -1 rather than make build 0, but it might just be a horse a piece either way
                    _internalAction = true;
                    nudRight.Value = 0;
                    _internalAction = false;
                }
                return new Version(nudLeft.Value, nudTop.Value, nudRight.Value, nudBottom.Value);
            }
        }

        bool _internalAction = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private void nudLeft_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!_internalAction)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        void SetAllToValue(int v)
        {
            _internalAction = true;
            nudLeft.Value = v;
            nudTop.Value = v;
            nudRight.Value = v;
            nudBottom.Value = v;
            _internalAction = false;
        }

        private void mnuCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(GetValue()?.ToString() ?? "null");
        }

        private void mnuParse_Click(object sender, RoutedEventArgs e)
        {
            StringInputDialog sid = new StringInputDialog("Parse Version from String",
                "Enter a Version string (numbers and periods only):");

            sid.ValidationFunction = (s) => { return Version.TryParse(s, out _); };
            sid.ValidationFailureString = "Not a valid Version value";

            sid.Width = 350;
            sid.SsuiTheme = _host?.GetThemeForDialogs() ?? SsuiThemes.SystemTheme;
            sid.Owner = _host?.GetWindow();

            sid.ShowDialog();

            if (sid.DialogResult)
            {
                Version v = Version.Parse(sid.Value);

                _internalAction = true;
                nudLeft.Value = v.Major;
                nudTop.Value = v.Minor;
                nudRight.Value = v.Build;
                nudBottom.Value = v.Revision;
                _internalAction = false;
            }
        }
    }
}

