using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static SolidShineUi.Utils.IconLoader;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A property editor for editing <see cref="Version"/> objects.
    /// </summary>
    public partial class VersionEditor : UserControl, IPropertyEditor
    {
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
        public ExperimentalPropertyList ParentPropertyList { set { } }

        private ColorScheme _cs = new ColorScheme();

        /// <inheritdoc/>
        public ColorScheme ColorScheme
        {
            set
            {
                _cs = value;
                nudLeft.ColorScheme = value;
                nudTop.ColorScheme = value;
                nudRight.ColorScheme = value;
                nudBottom.ColorScheme = value;
                btnMenu.ColorScheme = value;

                if (value.BackgroundColor == Colors.Black || value.ForegroundColor == Colors.White)
                {
                    imgFontEdit.Source = LoadIcon("ThreeDots", ICON_WHITE);
                }
                else if (value.BackgroundColor == Colors.White)
                {
                    imgFontEdit.Source = LoadIcon("ThreeDots", ICON_BLACK);
                }
                else
                {
                    imgFontEdit.Source = LoadIcon("ThreeDots", ICON_COLOR);
                }
            }
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
        public void LoadValue(object? value, Type type)
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

        /// <inheritdoc/>
        public object? GetValue()
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

        /// <inheritdoc/>
        public event EventHandler? ValueChanged;
#else
        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
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
        
        /// <inheritdoc/>
        public object GetValue()
        {
            return new Version(nudLeft.Value, nudTop.Value, nudRight.Value, nudBottom.Value);
        }
        
        /// <inheritdoc/>
        public event EventHandler ValueChanged;
#endif

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
            //bool done = false;
            StringInputDialog sid = new StringInputDialog(_cs, "Parse Version from String",
                "Enter a Version string (numbers and periods only):");
            sid.ValidationFunction = (s) => { return Version.TryParse(s, out _); };
            sid.ValidationFailureString = "Not a valid Version value";
            sid.Width = 350;
            sid.Owner = Window.GetWindow(this);

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
            //            while (!done)
            //            {
            //                sid.ShowDialog();
            //                if (sid.DialogResult == true)
            //                {
            //#if NETCOREAPP
            //                    Version? v = new Version();
            //#else
            //                    Version v = new Version();
            //#endif
            //                    bool res = Version.TryParse(sid.Value, out v);
            //                    if (res && v != null) // null check shouldn't be necessary, but to satisfy the nullability system in .NET, here it is lol
            //                    {
            //                        done = true;
            //                        _internalAction = true;
            //                        nudLeft.Value = v.Major;
            //                        nudTop.Value = v.Minor;
            //                        nudRight.Value = v.Build;
            //                        nudBottom.Value = v.Revision;
            //                        _internalAction = false;
            //                    }
            //                    else
            //                    {
            //                        MessageDialog md = new MessageDialog(_cs);
            //                        md.Owner = Window.GetWindow(this);
            //                        md.ShowDialog("The string entered could not be formatted as a string. Make sure there are only numbers and periods. Do you want to retry?", null, null,
            //                            "Parse String Error", MessageDialogButtonDisplay.Two, MessageDialogImage.Error, customOkButtonText: "Retry");
            //                        if (md.DialogResult == MessageDialogResult.Cancel)
            //                        {
            //                            done = true;
            //                        }
            //                    }
            //                }
        }
    }
}

