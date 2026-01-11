using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Input;
using System.Diagnostics;
using Microsoft.Win32;
using System.Linq;
using SolidShineUi.Utils;
using System.Windows.Markup;

namespace SolidShineUi
{

    /// <summary>
    /// A control that provides a customizable UI for users to select files on their computer, similar to the "<c>input type="file"</c>" element in HTML.
    /// </summary>
    [DefaultEvent(nameof(SelectionChanged)), ContentProperty(nameof(SelectedFiles))]
    [Localizability(LocalizationCategory.ListBox)]
    public class FileSelect : ThemedControl
    {
        static FileSelect()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileSelect), new FrameworkPropertyMetadata(typeof(FileSelect)));
        }

        /// <summary>
        /// Create a new FileSelect control.
        /// </summary>
        public FileSelect()
        {
            DragEnter += NewFileSelect_DragEnter;
            DragLeave += NewFileSelect_DragLeave;
            Drop += NewFileSelect_Drop;

            SetValue(BorderThicknessProperty, new Thickness(1));
            SetValue(PaddingProperty, new Thickness(2));
            SetValue(DisplayFilenamesProperty, false);

            SetValue(SelectedFilesPropertyKey, new LimitableStringCollection());
            SelectedFiles.ItemAdding += SelectedFiles_AddingItem;
            SelectedFiles.CollectionChanged += SelectedFiles_CollectionChanged;

            CommandBindings.Add(new CommandBinding(BrowseForFile, OnBrowseForFile, CanExecuteAlways));
            CommandBindings.Add(new CommandBinding(ClearSelectedFiles, OnClearSelectedFiles, CanExecuteIfNotEmpty));
            CommandBindings.Add(new CommandBinding(CopyFilenames, OnCopyFilenames, CanExecuteIfNotEmpty));
            CommandBindings.Add(new CommandBinding(OpenContainingFolders, OnOpenFilesContainingFolders, CanExecuteIfNotEmpty));
            CommandBindings.Add(new CommandBinding(RemoveSelectedFile, OnRemoveSelectedFile, CanExecuteIfFilePresent));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, OnBrowseForFile, CanExecuteAlways));
            CommandBindings.Add(new CommandBinding(SelectFile, OnSelectFileCommand, CanExecuteAlways));
        }

        #region Template IO

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            LoadTemplateItems();
        }

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

        #region Color Scheme / SsuiTheme

        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

        /// <summary>
        /// A dependency property object backing the related ColorScheme property. See <see cref="ColorScheme"/> for more details.
        /// </summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(FileSelect),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            ColorScheme cs = (e.NewValue as ColorScheme)!;
#else
            ColorScheme cs = e.NewValue as ColorScheme;
#endif

            if (d is FileSelect c)
            {
                c.ApplyColorScheme(cs);
                c.ColorSchemeChanged?.Invoke(d, e);
            }
        }

        /// <summary>
        /// Get or set the color scheme used for this control. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        /// <summary>
        /// Apply a color scheme to this control. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply.</param>
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

            // I couldn't find a way to bind the default context menu's ColorScheme to this control's ColorScheme, so I decided to do it this way
            // there may be situations where this is undesirable, but I'd imagine it's not as likely as the alternative where a developer wants a context menu with the default colors
            if (ContextMenu is ContextMenu cm)
            {
                cm.ApplyColorScheme(cs);
            }
        }

        /// <inheritdoc/>
        protected override void OnApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {
            base.OnApplySsuiTheme(ssuiTheme, useLightBorder, useAccentTheme);

            if (ssuiTheme is SsuiAppTheme sat && useAccentTheme)
            {
                ApplyTheme(sat.AccentTheme);
            }
            else
            {
                ApplyTheme(ssuiTheme);
            }

            if (ContextMenu is SolidShineUi.ContextMenu cm)
            {
                cm.SetBinding(SsuiThemeProperty, new System.Windows.Data.Binding(nameof(SsuiTheme)) { Source = this });
            }

            void ApplyTheme(SsuiTheme theme)
            {
                ApplyThemeBinding(BackgroundProperty, SsuiTheme.ControlBackgroundProperty, theme);
                ApplyThemeBinding(BackgroundDisabledBrushProperty, SsuiTheme.DisabledBackgroundProperty, theme);
                ApplyThemeBinding(BorderDisabledBrushProperty, SsuiTheme.DisabledBorderBrushProperty, theme);
                ApplyThemeBinding(ForegroundProperty, SsuiTheme.ForegroundProperty, theme);
                ApplyThemeBinding(ButtonBackgroundProperty, SsuiTheme.ButtonBackgroundProperty, theme);
                ApplyThemeBinding(ButtonHighlightBrushProperty, SsuiTheme.HighlightBrushProperty, theme);
                ApplyThemeBinding(ButtonClickBrushProperty, SsuiTheme.ClickBrushProperty, theme);

                ApplyThemeBinding(CornerRadiusProperty, SsuiTheme.CornerRadiusProperty, theme);
                ApplyThemeBinding(ButtonCornerRadiusProperty, SsuiTheme.CornerRadiusProperty, theme);
            }
        }

        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the brush used for the background of the control.
        /// </summary>
        [Category("Brushes")]
        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of the Browse button in the control.
        /// </summary>
        [Category("Brushes")]
        public Brush ButtonBackground
        {
            get => (Brush)GetValue(ButtonBackgroundProperty);
            set => SetValue(ButtonBackgroundProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of the Browse button while the mouse is over it.
        /// </summary>
        [Category("Brushes")]
        public Brush ButtonHighlightBrush
        {
            get => (Brush)GetValue(ButtonHighlightBrushProperty);
            set => SetValue(ButtonHighlightBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of the Browse button while the mouse is clicking it.
        /// </summary>
        [Category("Brushes")]
        public Brush ButtonClickBrush
        {
            get => (Brush)GetValue(ButtonClickBrushProperty);
            set => SetValue(ButtonClickBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of the control (and button) when the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush BackgroundDisabledBrush
        {
            get => (Brush)GetValue(BackgroundDisabledBrushProperty);
            set => SetValue(BackgroundDisabledBrushProperty, value);
        }
        
        /// <summary>
        /// Get or set the brush used for the border of the control (and button) when the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush BorderDisabledBrush
        {
            get => (Brush)GetValue(BorderDisabledBrushProperty);
            set => SetValue(BorderDisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border around the edges of the control.
        /// </summary>
        [Category("Brushes")]
        public new Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty BackgroundDisabledBrushProperty = DependencyProperty.Register(
            "BackgroundDisabledBrush", typeof(Brush), typeof(FileSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            "BorderDisabledBrush", typeof(Brush), typeof(FileSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(FileSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly new DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(FileSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public static readonly DependencyProperty ButtonBackgroundProperty = DependencyProperty.Register(
            "ButtonBackground", typeof(Brush), typeof(FileSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty ButtonHighlightBrushProperty = DependencyProperty.Register(
            "ButtonHighlightBrush", typeof(Brush), typeof(FileSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.Gainsboro)));

        public static readonly DependencyProperty ButtonClickBrushProperty = DependencyProperty.Register(
            "ButtonClickBrush", typeof(Brush), typeof(FileSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion

        #region Corner Radius

        /// <summary>
        /// Get or set how much rounding to apply to the corners of the control. Setting the corners to 0 means no rounding/straight corners.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set how much rounding to apply to the corners of the control. Setting the corners to 0 means no rounding/straight corners.")]
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="CornerRadius"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius), typeof(CornerRadius), typeof(FileSelect),
            new PropertyMetadata(new CornerRadius(0), new PropertyChangedCallback(OnCornerRadiusChanged)));

        /// <summary>
        /// The backing routed event for <see cref="CornerRadiusChanged"/>. See the related event for details.
        /// </summary>
        public static readonly RoutedEvent CornerRadiusChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(CornerRadiusChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FileSelect));

        /// <summary>
        /// Raised when the <see cref="CornerRadius"/> property is changed.
        /// </summary>
        public event RoutedEventHandler CornerRadiusChanged
        {
            add { AddHandler(CornerRadiusChangedEvent, value); }
            remove { RemoveHandler(CornerRadiusChangedEvent, value); }
        }

        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FileSelect f)
            {
                RoutedEventArgs re = new RoutedEventArgs(CornerRadiusChangedEvent);
                f.RaiseEvent(re);
            }
        }

        /// <summary>
        /// Get or set how much rounding to apply to the Browse button inside the control. Settings the corners to 0 means no rounding/straight corners.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set how much rounding to apply to the Browse button inside the control. Settings the corners to 0 means no rounding/straight corners.")]
        public CornerRadius ButtonCornerRadius
        {
            get => (CornerRadius)GetValue(ButtonCornerRadiusProperty);
            set => SetValue(ButtonCornerRadiusProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="ButtonCornerRadius"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ButtonCornerRadiusProperty = DependencyProperty.Register(
            nameof(ButtonCornerRadius), typeof(CornerRadius), typeof(FileSelect), new PropertyMetadata(new CornerRadius(0)));

        #endregion

        #region Other Appearance Properties

        /// <summary>
        /// The backing dependency property for <see cref="BrowseButtonText"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty BrowseButtonTextProperty = DependencyProperty.Register(
            nameof(BrowseButtonText), typeof(string), typeof(FileSelect),
            new FrameworkPropertyMetadata("Browse..."));

        /// <summary>
        /// The backing dependency property for <see cref="NoFilesSelectedMessage"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty NoFilesSelectedMessageProperty = DependencyProperty.Register(
            nameof(NoFilesSelectedMessage), typeof(string), typeof(FileSelect),
            new FrameworkPropertyMetadata("(no files selected)"));

        /// <summary>
        /// Get or set the text displayed in the Browse button. The default value is "Browse...".
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the text displayed in the Browse button. The default value is \"Browse...\".")]
        public string BrowseButtonText
        {
            get => (string)GetValue(BrowseButtonTextProperty);
            set => SetValue(BrowseButtonTextProperty, value);
        }

        /// <summary>
        /// Get or set the message displayed when no files have been selected. The default value is "(no files selected)".
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the message displayed when no files have been selected. The default value is \"(no files selected)\".")]
        public string NoFilesSelectedMessage
        {
            get => (string)GetValue(NoFilesSelectedMessageProperty);
            set => SetValue(NoFilesSelectedMessageProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="ShowIcon"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register(
            nameof(ShowIcon), typeof(bool), typeof(FileSelect), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Get or set if the file icons are shown for selected files.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set if the file icons are shown for selected files.")]
        public bool ShowIcon
        {
            get => (bool)GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="ButtonPlacement"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ButtonPlacementProperty = DependencyProperty.Register(
            nameof(ButtonPlacement), typeof(PlacementDirection), typeof(FileSelect),
            new FrameworkPropertyMetadata(PlacementDirection.Right));

        /// <summary>
        /// Get or set the placement location of the Browse button within the control.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the placement location of the Browse button within the control.")]
        public PlacementDirection ButtonPlacement
        {
            get => (PlacementDirection)GetValue(ButtonPlacementProperty);
            set => SetValue(ButtonPlacementProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="DisplayFilenames"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty DisplayFilenamesProperty = DependencyProperty.Register(
            nameof(DisplayFilenames), typeof(bool), typeof(FileSelect),
            new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Get or set if the filenames of the selected files are displayed. If true, then all selected files are displayed in a vertical list (with scrolling if needed).
        /// If false, then only a count is displayed (i.e. "3 file(s) selected").
        /// </summary>
        [Category("Common")]
        [Description("Get or set if the filenames of the selected files are displayed.")]
        public bool DisplayFilenames
        {
            get => (bool)GetValue(DisplayFilenamesProperty);
            set => SetValue(DisplayFilenamesProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="FileListPadding"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty FileListPaddingProperty = DependencyProperty.Register(
            nameof(FileListPadding), typeof(Thickness), typeof(FileSelect),
            new FrameworkPropertyMetadata(new Thickness(0)));

        /// <summary>
        /// Get or set the padding to put around the file list portion of the control (the portion that actually has the files listed).
        /// </summary>
        /// <remarks>
        /// This can be used to add some space between the Browse button and the list of files, in situations where some extra space may be needed there.
        /// Use <see cref="Control.Padding"/> to add space between the edges of this FileSelect and its contents (the file list and Browse button together). 
        /// </remarks>
        [Category("Layout")]
        [Description("Get or set the padding to put around the file list portion of the control (the portion that actually has the files listed).")]
        public Thickness FileListPadding
        {
            get => (Thickness)GetValue(FileListPaddingProperty);
            set => SetValue(FileListPaddingProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="HorizontalScrollBarVisibility"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = DependencyProperty.Register(
            nameof(HorizontalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(FileSelect),
            new FrameworkPropertyMetadata(ScrollBarVisibility.Disabled));

        /// <summary>
        /// Get or set the appearance of the horizontal scroll bar when displaying the list of filenames. Does nothing if <c>DisplayFilenames</c> is set to false.
        /// </summary>
        [Category("Layout")]
        [Description("Get or set the appearance of the horizontal scroll bar when displaying the list of filenames.")]
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty);
            set => SetValue(HorizontalScrollBarVisibilityProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="VerticalScrollBarVisibility"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = DependencyProperty.Register(
            nameof(VerticalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(FileSelect),
            new FrameworkPropertyMetadata(ScrollBarVisibility.Auto));

        /// <summary>
        /// Get or set the appearance of the vertical scroll bar when displaying the list of filenames. Does nothing if <c>DisplayFilenames</c> is set to false.
        /// </summary>
        [Category("Layout")]
        [Description("Get or set the appearance of the vertical scroll bar when displaying the list of filenames.")]
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty);
            set => SetValue(VerticalScrollBarVisibilityProperty, value);
        }

        #endregion

        #region Context Menu / Commands

        #region Commands

        /// <summary>A WPF command that when executed, displays the Browse for Files dialog so a user can select one (or more) files.</summary>
        public static readonly RoutedCommand BrowseForFile = new RoutedCommand();
        /// <summary>A WPF command that when executed, clears all files from the SelectedFiles list. Only executable if there is one or more files currently selected.</summary>
        public static readonly RoutedCommand ClearSelectedFiles = new RoutedCommand();
        /// <summary>A WPF command that when executed, copies a list of the selected files to the clipboard. Only executable if there is one or more files currently selected.</summary>
        public static readonly RoutedCommand CopyFilenames = new RoutedCommand();
        /// <summary>A WPF command that when executed, opens the containing folders of each selected file. Only executable if there is one or more files currently selected.</summary>
        public static readonly RoutedCommand OpenContainingFolders = new RoutedCommand();
        /// <summary>A WPF command that when executed, removes the specific file passed in through the command parameter. Only executable if the file is present in the list of selected files.</summary>
        public static readonly RoutedCommand RemoveSelectedFile = new RoutedCommand();
        /// <summary>A WPF command that when executed, adds a specified file to be selected in this control. Pass the filename(s) as the command's parameter.</summary>
        public static readonly RoutedCommand SelectFile = new RoutedCommand();

        private void OnBrowseForFile(object sender, ExecutedRoutedEventArgs e)
        {
            // open file dialog
            BrowseFiles();
        }

        private void OnClearSelectedFiles(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedFiles.Clear();
        }

        private void OnSelectFileCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is string s)
            {
                SelectFiles(s);
            }
            else if (e.Parameter is IEnumerable<string> ie)
            {
                SelectFiles(ie);
            }
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

        #region FileFilter / AllowMultipleFiles

        /// <summary>
        /// Defines the standard filter for displaying all files (as in, no filter is applied).
        /// </summary>
        public const string ALL_FILES_FILTER = "*.*";

        /// <summary>
        /// Defines the delimiter character to use when specifying multiple filters for <see cref="FileFilter"/>.
        /// </summary>
        public static char FILTER_DELIMITER = ';';

        /// <summary>
        /// The backing dependency property for <see cref="FileFilter"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty FileFilterProperty = DependencyProperty.Register(
            nameof(FileFilter), typeof(string), typeof(FileSelect), new PropertyMetadata(ALL_FILES_FILTER));

        /// <summary>
        /// The backing dependency property for <see cref="AllowMultipleFiles"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty AllowMultipleFilesProperty = DependencyProperty.Register(
            nameof(AllowMultipleFiles), typeof(bool), typeof(FileSelect),
            new PropertyMetadata(true, new PropertyChangedCallback(OnAllowMultipleFilesChanged)));

        /// <summary>
        /// Get or set the file filter used when selecting files. Use the <see cref="FILTER_DELIMITER"/> character (usually a semicolon ";")
        /// to separate multiple extensions/filters (i.e. <c>*.docx;*.xlsx;*.pptx</c>). Supports <c>*</c> and <c>?</c> wildcard characters.
        /// </summary>
        /// <remarks>
        /// If the filter is changed after files have been selected, the existing selected files are not re-run against the new filter. 
        /// This only applies to files added in afterwards.
        /// </remarks>
        [Category("Common")]
        [Description("Get or set the file filter used when selecting files. Use the filter delimiter (usually a semicolon \";\") to separate " +
            "multiple extensions/filters. Supports * and ? wildcard characters.")]
        public string FileFilter
        {
            get => (string)GetValue(FileFilterProperty);
            set => SetValue(FileFilterProperty, value);
        }

        /// <summary>
        /// Get or set if multiple files can be selected with the FileSelect. If false, only one file can be selected.
        /// </summary>
        /// <remarks>
        /// If you want to set a specific max number of files that can be selected, set this to "true" and then set the max value via the <c>SelectedFiles.Capacity</c> property.
        /// </remarks>
        [Category("Common")]
        [Description("Get or set if multiple files can be selected with the FileSelect. If false, only one file can be selected.")]
        public bool AllowMultipleFiles
        {
            get => (bool)GetValue(AllowMultipleFilesProperty);
            set => SetValue(AllowMultipleFilesProperty, value);
        }

        /// <summary>
        /// Raised when the <see cref="AllowMultipleFiles"/> property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? AllowMultipleFilesChanged;
#else
        public event DependencyPropertyChangedEventHandler AllowMultipleFilesChanged;
#endif

        private static void OnAllowMultipleFilesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FileSelect s)
            {
                s.AllowMultipleFilesChanged?.Invoke(s, e);
                s.SelectedFiles.MaxCount = s.AllowMultipleFiles ? -1 : 1;
            }
        }

        #endregion

        #region Selected Files

        private static readonly DependencyPropertyKey SelectedFilesPropertyKey
            = DependencyProperty.RegisterReadOnly("SelectedFiles", typeof(LimitableStringCollection), typeof(FileSelect),
            new FrameworkPropertyMetadata(new LimitableStringCollection()));

        /// <summary>
        /// The backing dependency property for <see cref="SelectedFiles"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty SelectedFilesProperty = SelectedFilesPropertyKey.DependencyProperty;

        /// <summary>
        /// Get the list of files selected in this FileSelect. You can add or remove items from the collection, or set the collection's max size via the <c>Capacity</c> property.
        /// </summary>
        [Category("Common")]
        [Description("Get the list of files selected in this FileSelect.")]
        public LimitableStringCollection SelectedFiles
        {
            get { return (LimitableStringCollection)GetValue(SelectedFilesProperty); }
            private set { SetValue(SelectedFilesPropertyKey, value); }
        }

        private void SelectedFiles_AddingItem(object sender, CancelableItemEventArgs<string> e)
        {
            if (!File.Exists(e.Item) && FileMustExist || !FileMatchesFilter(e.Item, FileFilter.Split(FILTER_DELIMITER)))
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
            RoutedEventArgs re = new RoutedEventArgs(SelectionChangedEvent);
            RaiseEvent(re);
        }

        /// <summary>
        /// The backing routed event for <see cref="SelectionChanged"/>. See the related event for details.
        /// </summary>
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(SelectionChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FileSelect));

        /// <summary>
        /// Raised when the list of selected files (<see cref="SelectedFiles"/>) is changed.
        /// </summary>
        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        /// <summary>
        /// Get or set if a file must exist at a specified path before it can be added to <see cref="SelectedFiles"/>. Default is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// If set to <c>false</c>, users might be able to input file paths for files that currently don't exist. This could be useful in some scenarios.
        /// Even when this is set to <c>true</c>, it may be best to verify a file exists when you're acting upon it anyway, just to ensure it didn't
        /// get deleted between the time that the user selected a file and you're acting upon it.
        /// <para />
        /// If this property is changed after files have been selected, the existing selected files are not re-checked. This only applies to files added in afterwards.
        /// </remarks>
        [Category("Common")]
        [Description("Get or set if a file must exist at a specified path before it can be added to SelectedFiles. Default is true.")]
        public bool FileMustExist { get => (bool)GetValue(FileMustExistProperty); set => SetValue(FileMustExistProperty, value); }

        /// <summary>The backing dependency property for <see cref="FileMustExist"/>. See the related property for details.</summary>
        public static readonly DependencyProperty FileMustExistProperty
            = DependencyProperty.Register("FileMustExist", typeof(bool), typeof(FileSelect),
            new FrameworkPropertyMetadata(true));

        #region Select Files functions

        /// <summary>
        /// Select a file. The selected files, if they match the <see cref="FileFilter"/>, will be added to <see cref="SelectedFiles"/>.
        /// </summary>
        /// <param name="file">The full path to the file</param>
        /// <remarks>If there is no file at the specified path, it is not selected.</remarks>
        public void SelectFiles(string file)
        {
            if (FileMustExist && !File.Exists(file))
            {
                return;
            }

            InternalSelectFiles(file);
        }

        /// <summary>
        /// Select a list of files. The selected files, if they match the <see cref="FileFilter"/>, will be added to <see cref="SelectedFiles"/>.
        /// </summary>
        /// <param name="files">An enumeration of files, each string being a full path to a file</param>
        /// <remarks>
        /// If <see cref="AllowMultipleFiles"/> is set to <c>false</c>, only the first file from the list will be selected.
        /// If there is no file at the specified path, it is not selected.
        /// </remarks>
        public void SelectFiles(IEnumerable<string> files)
        {
            List<string> allowedFiles = new List<string>();

            foreach (string file in files)
            {
                if (FileMustExist && !File.Exists(file))
                {
                    continue;
                }

                allowedFiles.Add(file);
            }

            InternalSelectFiles(allowedFiles);
        }

        #endregion

        #region Internal file accepting functions

        void InternalSelectFiles(string file)
        {
            InternalSelectFiles(new List<string> { file });
        }

        void InternalSelectFiles(IEnumerable<string> files)
        {
            // check how many files have been selected
            if (!files.Any())
            {
                // no files were selected? okay, let's blank the control
                AcceptFiles(new List<string>());
            }
            else if (files.Count() == 1)
            {
                // only one file
                string filename = files.First();
                if (FileFilter != ALL_FILES_FILTER)
                {
                    if (FileMatchesFilter(filename, FileFilter.Split(FILTER_DELIMITER)))
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
                // multiple files inputted, can we accept multiple files?
                if (AllowMultipleFiles)
                {
                    // yes, we can accept multiple - now let's check each one against the filter
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
                        if (FileMatchesFilter(filename, FileFilter.Split(FILTER_DELIMITER)))
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

        /// <summary>
        /// Check if a file matches against a collection of filters.
        /// </summary>
        /// <param name="file">The file path to check.</param>
        /// <param name="filters">The filters to check against. If using the FileFilter property, split the string with the <c>string.Split</c> function, using the ";" delimiter.</param>
        /// <returns></returns>
        private static bool FileMatchesFilter(string file, string[] filters)
        {
            // we only need to match at least one filter wildcard
            // so the moment one is matched, jump out of the foreach (in case there's a lot of filters)
            // start "match" variable as false, as the || (or) operator will set it to true if the wildcard matches, or keep it false if it doesn't
            // we just need to check that "match" is true at the end, doesn't really matter which wildcard worked

            bool match = false;
            foreach (string filter in filters)
            {
                // match wildcards
                match = match || WildcardMatch.MatchesWildcard(Path.GetFileName(file), filter, true);

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
            string[] filters = FileFilter.Split(FILTER_DELIMITER);

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
        }

        #endregion

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

    /// <summary>
    /// Used to indicate the placement of a UI element within a larger parent element/control.
    /// </summary>
    public enum PlacementDirection
    {
        /// <summary>
        /// Hide the UI element.
        /// </summary>
        [Description("Hide the UI element.")]
        Hidden = 0,
        /// <summary>
        /// Display the UI element at the top side of the control.
        /// </summary>
        [Description("Display the UI element at the top side of the control.")]
        Top = 1,
        /// <summary>
        /// Display the UI element at the left side of the control.
        /// </summary>
        [Description("Display the UI element at the left side of the control.")]
        Left = 2,
        /// <summary>
        /// Display the UI element at the right side of the control.
        /// </summary>
        [Description("Display the UI element at the right side of the control.")]
        Right = 3,
        /// <summary>
        /// Display the UI element at the bottom side of the control.
        /// </summary>
        [Description("Display the UI element at the bottom side of the control.")]
        Bottom = 4,
    }
}
