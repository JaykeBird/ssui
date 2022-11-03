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
    /// Interaction logic for ThicknessEditor.xaml
    /// </summary>
    public partial class VersionEditor : UserControl, IPropertyEditor
    {
        public List<Type> ValidTypes => new List<Type> { typeof(Version) };

        public bool EditorAllowsModifying => true;

        public bool IsPropertyWritable { get => nudLeft.IsEnabled;
            set
            {
                nudLeft.IsEnabled = value;
                nudTop.IsEnabled = value;
                nudBottom.IsEnabled = value;
                nudRight.IsEnabled = value;
                btnMenu.IsEnabled = value;
            }
        }

        public ExperimentalPropertyList ParentPropertyList { set { } }

        private ColorScheme _cs = new ColorScheme();

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

        public VersionEditor()
        {
            InitializeComponent();
        }

        public FrameworkElement GetFrameworkElement() { return this; }

#if NETCOREAPP
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

        public object? GetValue()
        {
            return new Version(nudLeft.Value, nudTop.Value, nudRight.Value, nudBottom.Value);
        }

        public event EventHandler? ValueChanged;
#else
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

        public object GetValue()
        {
            return new Version(nudLeft.Value, nudTop.Value, nudRight.Value, nudBottom.Value);
        }

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
            Clipboard.SetText(new Version(nudLeft.Value, nudTop.Value, nudRight.Value, nudBottom.Value).ToString());
        }

        private void mnuParse_Click(object sender, RoutedEventArgs e)
        {
            bool done = false;
            StringInputDialog sid = new StringInputDialog(_cs, "Parse Version from String", "Please enter the string of the Version value you want to set this property to (only use numbers and periods):");
            sid.Owner = Window.GetWindow(this);
            while (!done)
            {
                sid.ShowDialog();
                if (sid.DialogResult == true)
                {
#if NETCOREAPP
                    Version? v = new Version();
#else
                    Version v = new Version();
#endif
                    bool res = Version.TryParse(sid.Value, out v);
                    if (res && v != null) // null check shouldn't be necessary, but to satisfy the nullability system in .NET, here it is lol
                    {
                        done = true;
                        _internalAction = true;
                        nudLeft.Value = v.Major;
                        nudTop.Value = v.Minor;
                        nudRight.Value = v.Build;
                        nudBottom.Value = v.Revision;
                        _internalAction = false;
                    }
                    else
                    {
                        MessageDialog md = new MessageDialog(_cs);
                        md.Owner = Window.GetWindow(this);
                        md.ShowDialog("The string entered could not be formatted as a string. Make sure there are only numbers and periods. Do you want to retry?", null, null,
                            "Parse String Error", MessageDialogButtonDisplay.Two, MessageDialogImage.Error, customOkButtonText: "Retry");
                        if (md.DialogResult == MessageDialogResult.Cancel)
                        {
                            done = true;
                        }
                    }
                }
            }
        }
    }
}
