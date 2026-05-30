using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A property editor for <see cref="Cursor"/> objects.
    /// </summary>
    public partial class CursorEditor : UserControl, IPropertyEditor
    {
        /// <summary>
        /// Create a CursorEditor.
        /// </summary>
        public CursorEditor()
        {
            InitializeComponent();

            // load in string values
            mnuDropdown.Header = Strings.SelectSystemCursor;
            mnuFileSelect.Header = Strings.LoadCursorFromFile;
            mnuNull.Header = Strings.SetAsNull;
            txtNull.Text = Strings.Null;
            lblFromFile.Text = Strings.FromFile;

            fs.NoFilesSelectedMessage = Strings.NoFilesSelected;
            fs.BrowseButtonText = Strings.Browse;
        }

        /// <inheritdoc/>
        public List<Type> ValidTypes => new List<Type> { typeof(Cursor) };

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public bool IsPropertyWritable
        {
            get => btnMenu.IsEnabled;
            set
            {
                cbbCursors.IsEnabled = value;
                fs.IsEnabled = value;
                btnMenu.IsEnabled = value;
            }
        }

        /// <inheritdoc/>
        public void ApplySsuiTheme(SsuiTheme theme)
        {
            fs.SsuiTheme = theme;
            btnMenu.SsuiTheme = theme;
            imgItalic.Source = IconLoader.LoadIcon("Select", theme.IconVariation);
            imgMenu.Source = IconLoader.LoadIcon("ThreeDots", theme.IconVariation);
        }

        /// <inheritdoc/>
        public void SetHostControl(IPropertyEditorHost host) { /* _host = host; */ }

        /// <inheritdoc/>
#if NETCOREAPP
        public event EventHandler? ValueChanged;
#else
        public event EventHandler ValueChanged;
#endif

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

#if NETCOREAPP
        Cursor? cval = Cursors.Arrow;
#else
        Cursor cval = Cursors.Arrow;
#endif

        bool _internalAction = false;

        /// <inheritdoc/>
#if NETCOREAPP
        public object? GetValue()
#else
        public object GetValue()
#endif
        {
            return cval;
        }

        /// <inheritdoc/>
#if NETCOREAPP
        public void LoadValue(object? value, Type type)
#else
        public void LoadValue(object value, Type type)
#endif
        {
            if (value == null)
            {
                _internalAction = true;
                cbbCursors.SelectedEnumValue = CursorType.Arrow;
                SetValueToNull();
                _internalAction = false;
            }
            else if (value is Cursor c)
            {
                string curVal = c.ToString();
                _internalAction = true;

                if (Enum.TryParse(curVal, true, out CursorType ctt))
                {
                    DisplayDropDown();

                    // this is a predefined cursor
                    cbbCursors.SelectedEnumValue = ctt;
                    cval = ctt.ToCursor();
                }
                else
                {
                    DisplayFileSelect();

                    if (File.Exists(curVal))
                    {
                        // this is a cursor from a file
                        fs.SelectedFiles.Clear();
                        fs.SelectedFiles.Add(curVal);
                    }
                    else
                    {
                        // this probably was a cursor from a file, but it doesn't exist
                        fs.SelectedFiles.Clear();
                        cval = Cursors.Arrow;
                    }
                }
                _internalAction = false;
            }
        }

#if NETCOREAPP
        void SetValue(Cursor? c)
#else
        void SetValue(Cursor c)
#endif
        {
            cval = c;
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void cbbCursors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_internalAction)
            {
                SetValue(cbbCursors.SelectedEnumValueAsEnum<CursorType>().ToCursor());
            }
        }

        private void fs_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (!_internalAction)
            {
                if (fs.SelectedFiles.Count > 0)
                {
                    SetValue(new Cursor(fs.SelectedFiles[0]));
                }
                else
                {
                    SetValue(Cursors.Arrow);
                }
            }
        }

        void DisplayFileSelect()
        {
            cbbCursors.Visibility = Visibility.Collapsed;
            stkFile.Visibility = Visibility.Visible;
            txtNull.Visibility = Visibility.Collapsed;

            mnuDropdown.IsChecked = false;
            mnuFileSelect.IsChecked = true;
            mnuNull.IsChecked = false;

            if (fs.SelectedFiles.Count > 0)
            {
                SetValue(new Cursor(fs.SelectedFiles[0], false));
            }
            else
            {
                SetValue(Cursors.Arrow);
            }
        }

        void DisplayDropDown()
        {
            cbbCursors.Visibility = Visibility.Visible;
            stkFile.Visibility = Visibility.Collapsed;
            txtNull.Visibility = Visibility.Collapsed;

            mnuDropdown.IsChecked = true;
            mnuFileSelect.IsChecked = false;
            mnuNull.IsChecked = false;

            SetValue(cbbCursors.SelectedEnumValueAsEnum<CursorType>().ToCursor());
        }

        void SetValueToNull()
        {
            cbbCursors.Visibility = Visibility.Collapsed;
            stkFile.Visibility = Visibility.Collapsed;
            txtNull.Visibility = Visibility.Visible;

            mnuDropdown.IsChecked = false;
            mnuFileSelect.IsChecked = false;
            mnuNull.IsChecked = true;

            if (!_internalAction)
            {
                SetValue(null);
            }
        }

        private void mnuDropdown_Click(object sender, RoutedEventArgs e)
        {
            DisplayDropDown();
        }

        private void mnuFileSelect_Click(object sender, RoutedEventArgs e)
        {
            DisplayFileSelect();
        }

        private void mnuNull_Click(object sender, RoutedEventArgs e)
        {
            SetValueToNull();
        }
    }
}
