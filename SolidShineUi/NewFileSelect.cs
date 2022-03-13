using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Diagnostics;
using Microsoft.Win32;
using System.Linq;
using SolidShineUi.Utils;

namespace SolidShineUi
{
    public class NewFileSelect : Control
    {
        static NewFileSelect()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NewFileSelect), new FrameworkPropertyMetadata(typeof(NewFileSelect)));
        }

        public NewFileSelect()
        {
            DragEnter += NewFileSelect_DragEnter;
            DragLeave += NewFileSelect_DragLeave;
            Drop += NewFileSelect_Drop;

            SetValue(BorderThicknessProperty, new Thickness(1));
            SetValue(PaddingProperty, new Thickness(2));
            SetValue(DispalyFilenamesProperty, false);

            SetValue(SelectedFilesPropertyKey, new FilenameStringCollection());
            SelectedFiles.AddingItem += SelectedFiles_AddingItem;
            SelectedFiles.CollectionChanged += SelectedFiles_CollectionChanged;

            InternalCornerRadiusChanged += FileSelect_InternalCornerRadiusChanged;
            InternalAllowMultipleFilesChanged += fs_InternalAllowMultipleFilesChanged;

            CommandBindings.Add(new CommandBinding(BrowseForFile, OnBrowseForFile, CanExecuteAlways));
            CommandBindings.Add(new CommandBinding(ClearSelectedFiles, OnClearSelectedFiles, CanExecuteIfNotEmpty));
            CommandBindings.Add(new CommandBinding(CopyFilenames, OnCopyFilenames, CanExecuteIfNotEmpty));
            CommandBindings.Add(new CommandBinding(OpenContainingFolders, OnOpenFilesContainingFolders, CanExecuteIfNotEmpty));
            CommandBindings.Add(new CommandBinding(RemoveSelectedFile, OnRemoveSelectedFile, CanExecuteIfFilePresent));
        }

        #region Drag-Drop events
        private void NewFileSelect_DragLeave(object sender, DragEventArgs e)
        {

        }

        private void NewFileSelect_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void NewFileSelect_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] fl)
                {
                    InternalSelectFiles(fl);
                }
            }
        }
        #endregion

        #region Color Scheme
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(NewFileSelect),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            ColorScheme cs = (e.NewValue as ColorScheme)!;
#else
            ColorScheme cs = e.NewValue as ColorScheme;
#endif

            if (d is NewFileSelect c)
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

            if (cs.IsHighContrast)
            {
                ButtonBackground = cs.BackgroundColor.ToBrush();
                ButtonHighlightBrush = cs.HighlightColor.ToBrush();
                ButtonClickBrush = cs.HighlightColor.ToBrush();
            }
            else
            {
                ButtonBackground = cs.SecondaryColor.ToBrush();
                ButtonHighlightBrush = cs.SecondHighlightColor.ToBrush();
                ButtonClickBrush = cs.ThirdHighlightColor.ToBrush();
            }

            if (ContextMenu is ContextMenu cm)
            {
                cm.ApplyColorScheme(cs);
            }
        }
        #endregion

        #region Brushes

        [Category("Brushes")]
        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        [Category("Brushes")]
        public Brush ButtonBackground
        {
            get => (Brush)GetValue(ButtonBackgroundProperty);
            set => SetValue(ButtonBackgroundProperty, value);
        }

        [Category("Brushes")]
        public Brush ButtonHighlightBrush
        {
            get => (Brush)GetValue(ButtonHighlightBrushProperty);
            set => SetValue(ButtonHighlightBrushProperty, value);
        }

        [Category("Brushes")]
        public Brush ButtonClickBrush
        {
            get => (Brush)GetValue(ButtonClickBrushProperty);
            set => SetValue(ButtonClickBrushProperty, value);
        }

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
            "BackgroundDisabledBrush", typeof(Brush), typeof(NewFileSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            "BorderDisabledBrush", typeof(Brush), typeof(NewFileSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(NewFileSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly new DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(NewFileSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public static readonly DependencyProperty ButtonBackgroundProperty = DependencyProperty.Register(
            "ButtonBackground", typeof(Brush), typeof(NewFileSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty ButtonHighlightBrushProperty = DependencyProperty.Register(
            "ButtonHighlightBrush", typeof(Brush), typeof(NewFileSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.Gainsboro)));

        public static readonly DependencyProperty ButtonClickBrushProperty = DependencyProperty.Register(
            "ButtonClickBrush", typeof(Brush), typeof(NewFileSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        #endregion

        #region Corner Radius

        [Category("Appearance")]
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", typeof(CornerRadius), typeof(NewFileSelect),
            new PropertyMetadata(new CornerRadius(0), new PropertyChangedCallback(OnCornerRadiusChanged)));

        protected event DependencyPropertyChangedEventHandler InternalCornerRadiusChanged;

        public static readonly RoutedEvent CornerRadiusChangedEvent = EventManager.RegisterRoutedEvent(
            "CornerRadiusChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NewFileSelect));

        public event RoutedEventHandler CornerRadiusChanged
        {
            add { AddHandler(CornerRadiusChangedEvent, value); }
            remove { RemoveHandler(CornerRadiusChangedEvent, value); }
        }

        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NewFileSelect f)
            {
                f.InternalCornerRadiusChanged?.Invoke(f, e);
            }
        }

        private void FileSelect_InternalCornerRadiusChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RoutedEventArgs re = new RoutedEventArgs(CornerRadiusChangedEvent);
            RaiseEvent(re);
        }

        [Category("Appearance")]
        public CornerRadius ButtonCornerRadius
        {
            get => (CornerRadius)GetValue(ButtonCornerRadiusProperty);
            set => SetValue(ButtonCornerRadiusProperty, value);
        }

        public static readonly DependencyProperty ButtonCornerRadiusProperty = DependencyProperty.Register(
            "ButtonCornerRadius", typeof(CornerRadius), typeof(NewFileSelect),
            new PropertyMetadata(new CornerRadius(0)));

        #endregion

        #region Other Appearance Properties

        public static readonly DependencyProperty BrowseButtonTextProperty = DependencyProperty.Register(
            "BrowseButtonText", typeof(string), typeof(NewFileSelect),
            new PropertyMetadata("Browse..."));

        public static readonly DependencyProperty NoFilesSelectedMessageProperty = DependencyProperty.Register(
            "NoFilesSelectedMessage", typeof(string), typeof(NewFileSelect),
            new PropertyMetadata("(no files selected)"));

        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register(
            "ShowIcon", typeof(bool), typeof(NewFileSelect),
            new PropertyMetadata(true));

        [Category("Common")]
        public string BrowseButtonText
        {
            get => (string)GetValue(BrowseButtonTextProperty);
            set => SetValue(BrowseButtonTextProperty, value);
        }

        [Category("Common")]
        public string NoFilesSelectedMessage
        {
            get => (string)GetValue(NoFilesSelectedMessageProperty);
            set => SetValue(NoFilesSelectedMessageProperty, value);
        }

        [Category("Common")]
        public bool ShowIcon
        {
            get => (bool)GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }

        public static readonly DependencyProperty ButtonPlacementProperty = DependencyProperty.Register(
            "ButtonPlacement", typeof(PlacementDirection), typeof(NewFileSelect),
            new PropertyMetadata(PlacementDirection.Right));

        public PlacementDirection ButtonPlacement
        {
            get => (PlacementDirection)GetValue(ButtonPlacementProperty);
            set => SetValue(ButtonPlacementProperty, value);
        }

        public static readonly DependencyProperty DispalyFilenamesProperty = DependencyProperty.Register(
            "DisplayFilenames", typeof(bool), typeof(NewFileSelect),
            new PropertyMetadata(true));

        public bool DisplayFilenames
        {
            get => (bool)GetValue(DispalyFilenamesProperty);
            set => SetValue(DispalyFilenamesProperty, value);
        }

        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = DependencyProperty.Register(
            "HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(NewFileSelect),
            new FrameworkPropertyMetadata(ScrollBarVisibility.Disabled));

        [Category("Layout")]
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty);
            set => SetValue(HorizontalScrollBarVisibilityProperty, value);
        }

        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = DependencyProperty.Register(
            "VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(NewFileSelect),
            new FrameworkPropertyMetadata(ScrollBarVisibility.Auto));

        [Category("Layout")]
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty);
            set => SetValue(VerticalScrollBarVisibilityProperty, value);
        }

        #endregion

        #region Browse Button / Commands

        #region Commands

        public static readonly RoutedCommand BrowseForFile = new RoutedCommand();
        public static readonly RoutedCommand ClearSelectedFiles = new RoutedCommand();
        public static readonly RoutedCommand CopyFilenames = new RoutedCommand();
        public static readonly RoutedCommand OpenContainingFolders = new RoutedCommand();
        public static readonly RoutedCommand RemoveSelectedFile = new RoutedCommand();

        private void OnBrowseForFile(object sender, ExecutedRoutedEventArgs e)
        {
            // open file dialog
            BrowseFiles();
        }

        private void OnClearSelectedFiles(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedFiles.Clear();
        }

        private void CanExecuteAlways(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CanExecuteIfNotEmpty(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedFiles.Count > 0;
        }

        private void CanExecuteIfFilePresent(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter != null)
            {
                if (e.Parameter is string name)
                {
                    e.CanExecute = SelectedFiles.Contains(name);
                    return;
                }
            }

            e.CanExecute = false;
        }

        private void OnCopyFilenames(object sender, ExecutedRoutedEventArgs e)
        {
            // copy filenames
            CopyFilenamesToClipboard();
        }

        private void OnOpenFilesContainingFolders(object sender, ExecutedRoutedEventArgs e)
        {
            // open containing folders for selected files
            OpenContainingFoldersOfSelectedFiles();
        }

        private void OnRemoveSelectedFile(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter != null)
            {
                if (e.Parameter is string s)
                {
                    SelectedFiles.Remove(s);
                }
            }
        }

        #endregion

        #region Base Functions

        /// <summary>
        /// Clears all files from the SelectedFiles list. No files are then selected.
        /// </summary>
        public void ClearAllSelectedFiles()
        {
            SelectedFiles.Clear();
        }

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
            if (SelectedFiles.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (string item in SelectedFiles)
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
        /// Open the containing folder in Windows Explorer for each of the selected files. If multiple files are in the same folder, that folder may be opened multiple times.
        /// </summary>
        public void OpenContainingFoldersOfSelectedFiles()
        {
            foreach (string item in SelectedFiles)
            {
                Process.Start("explorer.exe", "/select,\"" + item + "\"");
            }
        }

        #endregion

        #endregion

        #region Selected Files

        private static readonly DependencyPropertyKey SelectedFilesPropertyKey
            = DependencyProperty.RegisterReadOnly("SelectedFiles", typeof(FilenameStringCollection), typeof(NewFileSelect),
            new FrameworkPropertyMetadata(new FilenameStringCollection()));

        public static readonly DependencyProperty SelectedFilesProperty = SelectedFilesPropertyKey.DependencyProperty;

        /// <summary>
        /// Get or set the list of items in this SelectPanel. This Items property can be used to add items, remove items, and also select items via the Select method.
        /// </summary>
        [Category("Common")]
        public FilenameStringCollection SelectedFiles
        {
            get { return (FilenameStringCollection)GetValue(SelectedFilesProperty); }
            private set { SetValue(SelectedFilesPropertyKey, value); }
        }

        private void SelectedFiles_AddingItem(object sender, AddingItemStringEventArgs e)
        {
            if (!File.Exists(e.Item) && !FileMatchesFilter(e.Item, FileFilter.Split(';')))
            {
                e.Cancel = true;
            }
        }

#if NETCOREAPP
        private void SelectedFiles_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#else
        private void SelectedFiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#endif
        {
            LoadTemplateItems();

            RoutedEventArgs re = new RoutedEventArgs(SelectionChangedEvent);
            RaiseEvent(re);
        }

        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NewFileSelect));

        /// <summary>
        /// Raised when the list of selected files is changed.
        /// </summary>
        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        #endregion

        #region Connections to Control Template

        bool itemsLoaded = false;

#if NETCOREAPP
        ItemsControl? ic = null;
        FlatButton? fb = null;
        Grid? ch = null;
#else
        ItemsControl ic = null;
        FlatButton fb = null;
        Grid ch = null;
#endif

        void LoadTemplateItems()
        {
            if (!itemsLoaded)
            {
                ic = (ItemsControl)GetTemplateChild("PART_Ic");
                fb = (FlatButton)GetTemplateChild("PART_Button");
                ch = (Grid)GetTemplateChild("PART_Content");

                if (ic != null && fb != null && ch != null)
                {
                    itemsLoaded = true;
                }
            }
        }

        #endregion

        #region File Filtering Properties

        public const string ALL_FILES_FILTER = "*.*";

        public static readonly DependencyProperty FileFilterProperty = DependencyProperty.Register(
            "FileFilter", typeof(string), typeof(NewFileSelect),
            new PropertyMetadata("*.*"));

        public static readonly DependencyProperty AllowMultipleFilesProperty = DependencyProperty.Register(
            "AllowMultipleFiles", typeof(bool), typeof(NewFileSelect),
            new PropertyMetadata(true, new PropertyChangedCallback(OnInternalAllowMultipleFilesChanged)));

        [Category("Common")]
        public string FileFilter
        {
            get => (string)GetValue(FileFilterProperty);
            set => SetValue(FileFilterProperty, value);
        }

        [Category("Common")]
        public bool AllowMultipleFiles
        {
            get => (bool)GetValue(AllowMultipleFilesProperty);
            set => SetValue(AllowMultipleFilesProperty, value);
        }

        protected event DependencyPropertyChangedEventHandler InternalAllowMultipleFilesChanged;

#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? AllowMultipleFilesChanged;
#else
        public event DependencyPropertyChangedEventHandler AllowMultipleFilesChanged;
#endif

        private static void OnInternalAllowMultipleFilesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NewFileSelect s)
            {
                s.InternalAllowMultipleFilesChanged?.Invoke(s, e);
            }
        }
        private void fs_InternalAllowMultipleFilesChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            AllowMultipleFilesChanged?.Invoke(this, e);
            SelectedFiles.Capacity = AllowMultipleFiles ? -1 : 1;
        }

        #endregion

        #region Select Files

        /// <summary>
        /// Select a file.
        /// </summary>
        /// <param name="file">The full path to the file.</param>
        /// <remarks>If there is no file at the specified path, it is not selected.</remarks>
        public void SelectFiles(string file)
        {
            if (File.Exists(file))
            {
                InternalSelectFiles(file);
            }
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
            // we only need to match at least one filter wildcard
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
            if (files.Count == 0) return;

            if (AllowMultipleFiles)
            {
                foreach (string file in files)
                {
                    SelectedFiles.Add(file);
                }
            }
            else
            {
                SelectedFiles.Clear();

                SelectedFiles.Add(files[0]);
            }

            //switch (files.Count)
            //{
            //    case 0:
            //        txtName.Text = "(no files selected)";
            //        imgIcon.Source = null;
            //        break;
            //    case 1:
            //        txtName.Text = Path.GetFileName(files[0]);
            //        imgIcon.Source = NativeMethods.GetSmallIcon(files[0]);
            //        break;
            //    default:
            //        txtName.Text = files.Count + " files selected";
            //        imgIcon.Source = GetStackImage(MessageDialogImageColor.Color);
            //        break;
            //}

            //_acceptedFiles = files;

            //RoutedEventArgs e = new RoutedEventArgs(SelectionChangedEvent);
            //RaiseEvent(e);
        }
        #endregion

        #region File Stream Functions

        /// <summary>
        /// Create a file stream for each selected file, which can be used for reading the files.
        /// </summary>
        /// <exception cref="InvalidOperationException">No files are currently selected.</exception>
        public FileStream[] OpenFilesForReading()
        {
            if (SelectedFiles.Count == 0)
            {
                throw new InvalidOperationException("No files are currently selected.");
            }

            FileStream[] files = new FileStream[SelectedFiles.Count];
            int i = 0;

            foreach (string file in SelectedFiles)
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
            if (SelectedFiles.Count == 0)
            {
                throw new InvalidOperationException("No files are currently selected.");
            }

            FileStream[] files = new FileStream[SelectedFiles.Count];
            int i = 0;

            foreach (string file in SelectedFiles)
            {
                files[i] = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                i++;
            }

            return files;
        }

        #endregion

    }

    public enum PlacementDirection
    {
        Hidden = 0,
        Top = 1,
        Left = 2,
        Right = 3,
        Bottom = 4,
    }
}
