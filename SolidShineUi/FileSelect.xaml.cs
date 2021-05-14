using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.ComponentModel;
using static SolidShineUi.MessageDialogImageConverter;
using System.Windows.Input;

namespace SolidShineUi
{
    /// <summary>
    /// Interaction logic for FileSelect.xaml
    /// </summary>
    [DefaultEvent("SelectionChanged")]
    public partial class FileSelect : UserControl
    {
        public FileSelect()
        {
            InitializeComponent();

            if (_showIcon)
            {
                imgIcon.Visibility = Visibility.Visible;
                colImage.Width = new GridLength(24);
            }
            else
            {
                imgIcon.Visibility = Visibility.Collapsed;
                colImage.Width = new GridLength(4);
            }

            AllowDrop = true;

            InternalBorderThicknessChanged += FileSelect_InternalBorderThicknessChanged;
            InternalCornerRadiusChanged += FileSelect_InternalCornerRadiusChanged;

            MouseDown += UserControl_MouseDown;
            MouseUp += UserControl_MouseUp;
            TouchDown += UserControl_TouchDown;
            TouchUp += UserControl_TouchUp;
            StylusDown += UserControl_StylusDown;
            StylusUp += UserControl_StylusUp;

            KeyDown += UserControl_KeyDown;
            KeyUp += UserControl_KeyUp;

            RightClick += FileSelect_RightClick;
        }

        private void grdBase_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] fl)
                {
                    InternalSelectFiles(fl);
                }
            }
        }

        #region Color Scheme
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(FileSelect),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            ColorScheme cs = (e.NewValue as ColorScheme)!;
#else
            ColorScheme cs = e.NewValue as ColorScheme;
#endif

            if (d is FileSelect c)
            {
                c.ColorSchemeChanged?.Invoke(d, e);
                c.ApplyColorScheme(cs);
            }
        }

        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        public void ApplyColorScheme(ColorScheme cs)
        {
            if (cs == null)
            {
                return;
            }
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }

            btnBrowse.ApplyColorScheme(cs);
            //cm.ApplyColorScheme(cs);

            if (cs.IsHighContrast)
            {
                Background = cs.BackgroundColor.ToBrush();

                BackgroundDisabledBrush = cs.BackgroundColor.ToBrush();
                BorderDisabledBrush = cs.LightDisabledColor.ToBrush();
            }
            else
            {
                Background = cs.LightBackgroundColor.ToBrush();

                BackgroundDisabledBrush = cs.LightDisabledColor.ToBrush();
                BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
            }

            BorderBrush = cs.BorderColor.ToBrush();
            Foreground = cs.ForegroundColor.ToBrush();

            if (IsEnabled)
            {
                brdrBase.BorderBrush = BorderBrush;
                brdrBase.Background = Background;
            }
            else
            {
                brdrBase.BorderBrush = BorderDisabledBrush;
                brdrBase.Background = BackgroundDisabledBrush;
            }
        }
        #endregion

        #region Brushes

        [Category("Brushes")]
        public Brush BackgroundDisabledBrush
        {
            get => (Brush)GetValue(BackgroundDisabledBrushProperty);
            set => SetValue(BackgroundDisabledBrushProperty, value);
        }

        [Category("Brushes")]
        public Brush BorderDisabledBrush
        {
            get => (Brush)GetValue(BorderDisabledBrushProperty);
            set => SetValue(BorderDisabledBrushProperty, value);
        }

        [Category("Brushes")]
        public new Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        public static readonly DependencyProperty BackgroundDisabledBrushProperty = DependencyProperty.Register(
            "BackgroundDisabledBrush", typeof(Brush), typeof(FileSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            "BorderDisabledBrush", typeof(Brush), typeof(FileSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(FileSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        #endregion

        #region Border Thickness / Corner Radius
        [Category("Appearance")]
        public Thickness ControlBorderThickness
        {
            get => (Thickness)GetValue(ControlBorderThicknessProperty);
            set => SetValue(ControlBorderThicknessProperty, value);
        }

        [Category("Miscellaneous")]
        public new Thickness BorderThickness
        {
            get => ControlBorderThickness;
        }

        [Category("Appearance")]
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty ControlBorderThicknessProperty = DependencyProperty.Register(
            "ControlBorderThickness", typeof(Thickness), typeof(FileSelect),
            new PropertyMetadata(new Thickness(1), new PropertyChangedCallback(OnBorderThicknessChanged)));

        protected event DependencyPropertyChangedEventHandler InternalBorderThicknessChanged;

        public static readonly RoutedEvent BorderThicknessChangedEvent = EventManager.RegisterRoutedEvent(
            "BorderThicknessChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FileSelect));

        public event RoutedEventHandler BorderThicknessChanged
        {
            add { AddHandler(BorderThicknessChangedEvent, value); }
            remove { RemoveHandler(BorderThicknessChangedEvent, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", typeof(CornerRadius), typeof(FileSelect),
            new PropertyMetadata(new CornerRadius(0), new PropertyChangedCallback(OnCornerRadiusChanged)));

        protected event DependencyPropertyChangedEventHandler InternalCornerRadiusChanged;

        public static readonly RoutedEvent CornerRadiusChangedEvent = EventManager.RegisterRoutedEvent(
            "CornerRadiusChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FileSelect));

        public event RoutedEventHandler CornerRadiusChanged
        {
            add { AddHandler(CornerRadiusChangedEvent, value); }
            remove { RemoveHandler(CornerRadiusChangedEvent, value); }
        }

        private static void OnBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FileSelect f)
            {
                f.InternalBorderThicknessChanged?.Invoke(f, e);
            }
        }

        private void FileSelect_InternalBorderThicknessChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            brdrBase.BorderThickness = ControlBorderThickness;
            RoutedEventArgs re = new RoutedEventArgs(BorderThicknessChangedEvent);
            RaiseEvent(re);
        }

        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FileSelect f)
            {
                f.InternalCornerRadiusChanged?.Invoke(f, e);
            }
        }

        private void FileSelect_InternalCornerRadiusChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            brdrBase.CornerRadius = CornerRadius;
            btnBrowse.CornerRadius = CornerRadius;
            RoutedEventArgs re = new RoutedEventArgs(BorderThicknessChangedEvent);
            RaiseEvent(re);
        }

        #endregion

        #region Click Handling

        #region Routed Events

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FileSelect));

        /// <summary>
        /// Raised when the user clicks on the button, via a mouse click, touch, a stylus, or via the keyboard.
        /// </summary>
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public static readonly RoutedEvent RightClickEvent = EventManager.RegisterRoutedEvent(
            "RightClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FileSelect));

        /// <summary>
        /// Raised when the user right-clicks on the button, via a mouse click or via the keyboard.
        /// </summary>
        public event RoutedEventHandler RightClick
        {
            add { AddHandler(RightClickEvent, value); }
            remove { RemoveHandler(RightClickEvent, value); }
        }

        #endregion

        #region Variables/Properties
        bool initiatingClick = false;

        //bool sel = false;

        /// <summary>
        /// Gets or sets whether the Click event should be raised when the button is pressed, rather than when it is released.
        /// </summary>
        public bool ClickOnPress { get; set; } = false;

        #endregion

        /// <summary>
        /// Sets up the button to be clicked. This must be run before PerformClick.
        /// </summary>
        /// <param name="rightClick">Determine whether this should be treated as a right click (which usually invokes a context menu).</param>
        void PerformPress(bool rightClick = false)
        {
            initiatingClick = true;

            if (ClickOnPress)
            {
                PerformClick(rightClick);
            }
        }

        /// <summary>
        /// If the button is prepared by PerformPress, perform the Click actions, including raising the Click event.
        /// </summary>
        /// <param name="rightClick">Determine whether this should be treated as a right click (which usually invokes a context menu).</param>
        void PerformClick(bool rightClick = false)
        {
            if (initiatingClick)
            {
                if (rightClick)
                {
                    RoutedEventArgs rre = new RoutedEventArgs(RightClickEvent);
                    RaiseEvent(rre);
                    return;
                }

                RoutedEventArgs re = new RoutedEventArgs(ClickEvent);
                RaiseEvent(re);
                initiatingClick = false;
            }
        }

        /// <summary>
        /// Perform a click programattically. The button responds the same way as if it was clicked by the user.
        /// </summary>
        public void DoClick()
        {
            PerformPress();
            PerformClick();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PerformPress(e.ChangedButton == MouseButton.Right);
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            PerformClick(e.ChangedButton == MouseButton.Right);
            e.Handled = true;
        }

#if NETCOREAPP
        private void UserControl_TouchDown(object? sender, TouchEventArgs e)
        {
            PerformPress();
        }

        private void UserControl_TouchUp(object? sender, TouchEventArgs e)
        {
            PerformClick();
        }

#else
        private void UserControl_TouchDown(object sender, TouchEventArgs e)
        {
            PerformPress();
        }

        private void UserControl_TouchUp(object sender, TouchEventArgs e)
        {
            PerformClick();
        }
#endif

        private void UserControl_StylusDown(object sender, StylusDownEventArgs e)
        {
            PerformPress();
        }

        private void UserControl_StylusUp(object sender, StylusEventArgs e)
        {
            PerformClick();
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                PerformPress();
            }
        }

        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                PerformClick();
            }
            else if (e.Key == Key.Apps)
            {
                PerformClick(true);
            }
        }
        #endregion

        bool _showIcon = true;

        public bool ShowIcon
        {
            get
            {
                return _showIcon;
                //return imgIcon.Visibility == Visibility.Visible;
            }
            set
            {
                _showIcon = value;

                if (imgIcon != null && colImage != null)
                {
                    if (value)
                    {
                        imgIcon.Visibility = Visibility.Visible;
                        colImage.Width = new GridLength(24);
                    }
                    else
                    {
                        imgIcon.Visibility = Visibility.Collapsed;
                        colImage.Width = new GridLength(4);
                    }
                }
            }
        }

        private List<string> _acceptedFiles = new List<string>();

        /// <summary>
        /// Get or set the list of selected files. The list contains each file's path.
        /// </summary>
        public List<string> SelectedFiles 
        { 
            get
            {
                return _acceptedFiles;
            }
            set
            {
                SelectFiles(value);
            }
        }

        /// <summary>
        /// Get the number of selected files.
        /// </summary>
        public int SelectedFilesCount
        {
            get { return _acceptedFiles.Count; }
        }

        /// <summary>
        /// Get or set the selected file. If multiple files are selected, returns the first file. If no files are selected, returns an empty string.
        /// </summary>
        public string SelectedFile
        {
            get
            {
                if (_acceptedFiles.Count > 0)
                {
                    return _acceptedFiles.First();
                }
                else
                {
                    return "";
                }
            }
            set
            {
                SelectFiles(value);
            }
        }

        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FileSelect));

        /// <summary>
        /// Raised when the list of selected files is changed.
        /// </summary>
        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        /// <summary>
        /// Get or set the file filter to use when selecting files. Use semicolons (;) to separate multiple extensions/filters (i.e. <c>"*.docx;*.xlsx;*.pptx"</c>).
        /// </summary>
        /// <remarks>Default value is <c>ALL_FILES_FILTER</c>, which is <c>"*.*"</c>.</remarks>
        public string FileFilter
        {
            get
            {
                return filter;
            }
            set
            {
                if (value.Contains("|"))
                {
                    throw new ArgumentException("FileFilter cannot contain a '|' character.");
                }
                else
                {
                    filter = value;
                }
            }
        }

        private string filter = ALL_FILES_FILTER;

        public const string ALL_FILES_FILTER = "*.*";

        /// <summary>
        /// Get or set if multiple files can be selected or dropped in. If false, only one file can be selected.
        /// </summary>
        public bool AllowMultipleFiles { get; set; } = false;

        /// <summary>
        /// Display an OpenFileDialog for the user to select files.
        /// </summary>
        public void BrowseFiles()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (FileFilter.Contains("|"))
            {
                throw new ArgumentException("FileFilter property cannot contain a '|' character.");
            }

            ofd.Filter = FileFilter == ALL_FILES_FILTER ? "All Files|*.*" : "Accepted Files|" + FileFilter;
            ofd.Title = "Select Files";
            ofd.Multiselect = AllowMultipleFiles;

            if (ofd.ShowDialog().GetValueOrDefault(false))
            {
                if (AllowMultipleFiles)
                {
                    InternalSelectFiles(ofd.FileNames);
                }
                else
                {
                    InternalSelectFiles(ofd.FileName);
                }
            }
        }

        /// <summary>
        /// Copy the filenames for all the selected files to the clipboard. Each file path is on its own separate line.
        /// </summary>
        /// <exception cref="InvalidOperationException">No files are currently selected.</exception>
        public void CopyFilenamesToClipboard()
        {
            if (_acceptedFiles.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (string item in _acceptedFiles)
                {
                    sb.AppendLine(item);
                }

                Clipboard.SetText(sb.ToString());
            }
            else
            {
                throw new InvalidOperationException("No files are currently selected.");
            }
        }

        /// <summary>
        /// Open the containing folder in Windows Explorer for each of the selected files. If multiple files are in the same folder, that folder will be opened multiple times.
        /// </summary>
        public void OpenContainingFolders()
        {
            foreach (string item in _acceptedFiles)
            {
                Process.Start("explorer.exe", "/select,\"" + item + "\"");
            }
        }

        /// <summary>
        /// Create a file stream for each selected file, which can be used for reading the files.
        /// </summary>
        /// <exception cref="InvalidOperationException">No files are currently selected.</exception>
        public FileStream[] OpenFilesForReading()
        {
            if (_acceptedFiles.Count == 0)
            {
                throw new InvalidOperationException("No files are currently selected.");
            }

            FileStream[] files = new FileStream[_acceptedFiles.Count];
            int i = 0;

            foreach (string file in _acceptedFiles)
            {
                files[i] = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read);
                i++;
            }

            return files;
        }

        /// <summary>
        /// Create a file stream for each selected file, which can be used for reading or writing the files.
        /// </summary>
        /// <exception cref="InvalidOperationException">No files are currently selected.</exception>
        public FileStream[] OpenFilesForWriting()
        {
            if (_acceptedFiles.Count == 0)
            {
                throw new InvalidOperationException("No files are currently selected.");
            }

            FileStream[] files = new FileStream[_acceptedFiles.Count];
            int i = 0;

            foreach (string file in _acceptedFiles)
            {
                files[i] = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                i++;
            }

            return files;
        }

        /// <summary>
        /// Remove all selected files.
        /// </summary>
        public void Clear()
        {
            InternalSelectFiles(new List<string> { });
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            BrowseFiles();
        }

        /// <summary>
        /// Select a file.
        /// </summary>
        /// <param name="file">The full path to the file.</param>
        /// <remarks>If there is no file at the specified path, it is not selected.</remarks>
        public void SelectFiles(string file)
        {
            List<string> allowedFiles = new List<string>();

            if (File.Exists(file))
            {
                allowedFiles.Add(file);
            }

            InternalSelectFiles(allowedFiles);
            //return allowedFiles;
        }

        /// <summary>
        /// Select a list of files.
        /// </summary>
        /// <param name="files">An enumeration of files, each string being a separate full path to a file.</param>
        /// <remarks>If there is no file at the specified path, it is not selected.</remarks>
        public void SelectFiles(IEnumerable<string> files)
        {
            List<string> allowedFiles = new List<string>();

            foreach (string file in files)
            {
                if (File.Exists(file))
                {
                    allowedFiles.Add(file);
                }
            }

            InternalSelectFiles(allowedFiles);
            //return allowedFiles;
        }

        #region Accept files process
        void InternalSelectFiles(string file)
        {
            InternalSelectFiles(new List<string> { file });
        }

        void InternalSelectFiles(IEnumerable<string> files)
        {
            // check how many files have been selected
            if (!files.Any()) //(files.Count() == 0)
            {
                // no files were selected? okay lol
                AcceptFiles(new List<string>());
            }
            else if (files.Count() == 1)
            {
                // only one file
                string filename = files.First();
                if (FileFilter != ALL_FILES_FILTER)
                {
                    if (FileMatchesFilter(filename, FileFilter.Split(';')))
                    {
                        // accept file
                        AcceptFiles(new List<string> { filename });
                    }
                }
                else
                {
                    // accept file
                    AcceptFiles(new List<string> { filename });
                }
            }
            else
            {
                // multiple files inputted
                if (AllowMultipleFiles)
                {
                    List<string> allowedFiles = GetFilteredFiles(files);

                    // accept allowedFiles
                    AcceptFiles(allowedFiles);
                }
                else
                {
                    // only take the first file
                    string filename = files.First();
                    if (FileFilter != ALL_FILES_FILTER)
                    {
                        if (FileMatchesFilter(filename, FileFilter.Split(';')))
                        {
                            // accept file
                            AcceptFiles(new List<string> { filename });
                        }
                    }
                    else
                    {
                        // accept file
                        AcceptFiles(new List<string> { filename });
                    }
                }
            }
        }

        private static bool FileMatchesFilter(string file, string[] filters)
        {
            // we only need to match at least one filter regex
            // so the moment one is matched, jump out of the foreach (in case there's a lot of filters)
            // start "match" variable as false, as the || (or) operator will set it to true if the regex matches, or keep it false if it doesn't
            // we just need to check that "match" is true at the end, doesn't matter which regex worked

            bool match = false;
            foreach (string filter in filters)
            {
                // match wildcards
                match = match || WildcardMatch.MatchesWildcard(Path.GetFileName(file), filter, true);

                // source: https://stackoverflow.com/a/30300521/2987285
                //string r = "^" + Regex.Escape(filter).Replace("\\?", ".").Replace("\\*", ".*") + "$";
                //string r = "^" + filter;
                //if (filter.Contains("."))
                //{
                //    r = r.Replace(".", "\\.");
                //}
                //match = match || Regex.IsMatch(file, r, RegexOptions.IgnoreCase);

                if (match) break;
            }

            return match;
        }

        private List<string> GetFilteredFiles(IEnumerable<string> files)
        {
            if (FileFilter == ALL_FILES_FILTER)
            {
                // no filtration needed
                return files.ToList();
            }

            List<string> allowedFiles = new List<string>();

            // get filter list to use
            string[] filters = FileFilter.Split(';');

            foreach (string file in files)
            {
                if (FileMatchesFilter(file, filters))
                {
                    allowedFiles.Add(file);
                }
            }

            return allowedFiles;
        }

        void AcceptFiles(List<string> files)
        {
            switch (files.Count)
            {
                case 0:
                    txtName.Text = "(no files selected)";
                    imgIcon.Source = null;
                    break;
                case 1:
                    txtName.Text = Path.GetFileName(files[0]);
                    imgIcon.Source = NativeMethods.GetSmallIcon(files[0]);
                    break;
                default:
                    txtName.Text = files.Count + " files selected";
                    imgIcon.Source = GetStackImage(MessageDialogImageColor.Color);
                    break;
            }

            _acceptedFiles = files;

            RoutedEventArgs e = new RoutedEventArgs(SelectionChangedEvent);
            RaiseEvent(e);
        }
        #endregion

        /// <summary>
        /// Return a 16x16 stacked files image.
        /// </summary>
        /// <param name="color">The color to use for the image. Use black or white for high-contrast themes.</param>
        /// <exception cref="ArgumentException">Thrown if the image could not found and loaded. Suggests the library may be corrupted.</exception>
        /// <returns></returns>
#if NETCOREAPP
        private static BitmapImage? GetStackImage(MessageDialogImageColor color)
#else
        private static BitmapImage GetStackImage(MessageDialogImageColor color)
#endif
        {
            string packuri = "/SolidShineUi;component/Images/Stack";
            BitmapImage img;

            try
            {
                img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(packuri + color.ToString("f") + ".png", UriKind.RelativeOrAbsolute);
                img.EndInit();
            }
            catch (IOException ex)
            {
                throw new ArgumentException("Cannot locate the appropriate icon to display. This library may be corrupted, please redownload it from an official source.", ex);
            }

            return img;
        }

        #region Context menu

        /// <summary>
        /// Get or set whether the context menu is displayed when the control is right-clicked.
        /// </summary>
        public bool AllowContextMenu { get; set; } = true;

        public void DisplayContextMenu()
        {
            if (AllowContextMenu)
            {

                bool hasFiles = _acceptedFiles.Count != 0;

                ContextMenu cm = new ContextMenu();
                cm.ApplyColorScheme(ColorScheme);

#pragma warning disable IDE0017 // Simplify object initialization
                MenuItem mi1 = new MenuItem();
                mi1.Header = "Browse...";
                mi1.Click += btnBrowse_Click;
                cm.Items.Add(mi1);

                cm.Items.Add(new Separator());

                MenuItem mi2 = new MenuItem();
                mi2.Header = "Open Containing Folders";
                mi2.IsEnabled = hasFiles;
                mi2.Click += mnuFolder_Click;
                cm.Items.Add(mi2);

                MenuItem mi3 = new MenuItem();
                mi3.Header = "Copy Filenames";
                mi3.IsEnabled = hasFiles;
                mi3.Click += mnuCopy_Click;
                cm.Items.Add(mi3);

                cm.Items.Add(new Separator());

                MenuItem mi4 = new MenuItem();
                mi4.Header = "Clear Selection";
                mi4.IsEnabled = hasFiles;
                mi4.Click += mnuClear_Click;
                cm.Items.Add(mi4);

                cm.PlacementTarget = this;
                cm.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                cm.HorizontalOffset = 2;
                cm.VerticalOffset = 2;
#pragma warning restore IDE0017 // Simplify object initialization

                cm.IsOpen = true;
            }
        }

        private void FileSelect_RightClick(object sender, RoutedEventArgs e)
        {
            DisplayContextMenu();
        }

        private void mnuClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void mnuCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CopyFilenamesToClipboard();
            }
            catch (InvalidOperationException)
            {

            }
        }

        private void mnuFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenContainingFolders();
        }
        #endregion

        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsEnabled)
            {
                brdrBase.BorderBrush = BorderBrush;
                brdrBase.Background = Background;
            }
            else
            {
                brdrBase.BorderBrush = BorderDisabledBrush;
                brdrBase.Background = BackgroundDisabledBrush;
            }
        }
    }
}
