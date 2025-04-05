using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xaml;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;

namespace SolidShineUi
{
    /// <summary>
    /// A tab that can be added onto a <see cref="TabControl"/>. Each tab has a title, icon, and close button by default.
    /// </summary>
    [ContentProperty("Content")]
    [Localizability(LocalizationCategory.Title)]
    public class TabItem : DependencyObject
    {
        /// <summary>
        /// Create a TabItem.
        /// </summary>
        public TabItem()
        {
            //Self = this;

            InternalTitleChanged += tabItem_InternalTitleChanged;
            InternalIsDirtyChanged += tabItem_InternalIsDirtyChanged;
            InternalCanCloseChanged += tabItem_InternalCanCloseChanged;
            InternalCanSelectChanged += tabItem_InternalCanSelectChanged;
            InternalIconChanged += tabItem_InternalIconChanged;
            InternalContentChanged += tabItem_InternalContentChanged;
            InternalVisibilityChanged += tabItem_InternalVisibilityChanged;
            InternalShowIconChanged += tabItem_InternalShowIconChanged;
            InternalAllowDropChanged += tabItem_InternalAllowDropChanged;
            InternalToolTipChanged += tabItem_InternalToolTipChanged;
            InternalDisplayDirtyStateChanged += tabItem_InternalDisplayDirtyStateChanged;
            InternalTabBackgroundChanged += tabItem_InternalTabBackgroundChanged;
            InternalPaddingChanged += tabItem_InternalPaddingChanged;
        }

        /// <summary>
        /// Create a TabItem, with the title preset.
        /// </summary>
        /// <param name="title">The title of this tab.</param>
        public TabItem(string title) : this()
        {
            Title = title;
        }

        /// <summary>
        /// Create a TabItem, by copying the properties of an existing TabItem.
        /// </summary>
        /// <param name="other">The other TabItem to copy from.</param>
        /// <remarks>
        /// The values of the <see cref="Content"/> and <see cref="ExtraTabElement"/> properties are not copied over, as UI elements can only have 1 parent.
        /// </remarks>
        public TabItem(TabItem other) : this()
        {
            // I don't copy the Content or ExtraTabElement properties, since child UI elements can only have 1 parent
            // instead, the user should clone these properties themselves or come up with their own solution

            Title = other.Title;
            IsDirty = other.IsDirty;
            DisplayDirtyState = other.DisplayDirtyState;
            CanClose = other.CanClose;
            ShowIcon = other.ShowIcon;
            CanSelect = other.CanSelect;
            Icon = other.Icon;
            TabBackground = other.TabBackground;
            Padding = other.Padding;
            Visibility = other.Visibility;
            TabContextMenu = other.TabContextMenu;
            AllowDrop = other.AllowDrop;
            ToolTip = other.ToolTip;
            FontSize = other.FontSize;
            FontStyle = other.FontStyle;
            FontFamily = other.FontFamily;
            FontWeight = other.FontWeight;
            FontStretch = other.FontStretch;
        }

        #region Title

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TabItem),
            new PropertyMetadata("New Tab", new PropertyChangedCallback(OnInternalTitleChanged)));

        ///<summary>
        /// Get or set the title of this tab.
        ///</summary>
        [Category("Common")]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalTitleChanged;

        /// <summary>
        /// Raised when the Title property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? TitleChanged;
#else
        public event DependencyPropertyChangedEventHandler TitleChanged;
#endif

        private static void OnInternalTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalTitleChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalTitleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TitleChanged?.Invoke(this, e);
        }
        #endregion

        #region IsDirty

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register("IsDirty", typeof(bool), typeof(TabItem),
            new PropertyMetadata(false, new PropertyChangedCallback(OnInternalIsDirtyChanged)));

        ///<summary>
        /// Get or set if the tab is dirty.
        ///</summary>
        [Category("Common")]
        public bool IsDirty
        {
            get { return (bool)GetValue(IsDirtyProperty); }
            set { SetValue(IsDirtyProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalIsDirtyChanged;

        /// <summary>
        /// Raised when the IsDirty property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? IsDirtyChanged;
#else
        public event DependencyPropertyChangedEventHandler IsDirtyChanged;
#endif

        private static void OnInternalIsDirtyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalIsDirtyChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalIsDirtyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            IsDirtyChanged?.Invoke(this, e);
        }
        #endregion

        #region DisplayDirtyState

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty DisplayDirtyStateProperty = DependencyProperty.Register("DisplayDirtyState", typeof(bool), typeof(TabItem),
            new PropertyMetadata(true, new PropertyChangedCallback(OnInternalDisplayDirtyStateChanged)));

        ///<summary>
        /// Get or set if the tab should display the state of its IsDirty property. When set to true, an asterisk (*) will be added to the title when it is dirty.
        ///</summary>
        [Category("Common")]
        public bool DisplayDirtyState
        {
            get { return (bool)GetValue(DisplayDirtyStateProperty); }
            set { SetValue(DisplayDirtyStateProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalDisplayDirtyStateChanged;

        /// <summary>
        /// Raised when the DisplayDirtyState property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? DisplayDirtyStateChanged;
#else
        public event DependencyPropertyChangedEventHandler DisplayDirtyStateChanged;
#endif

        private static void OnInternalDisplayDirtyStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalDisplayDirtyStateChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalDisplayDirtyStateChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DisplayDirtyStateChanged?.Invoke(this, e);
        }
        #endregion

        #region CanClose

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty CanCloseProperty = DependencyProperty.Register("CanClose", typeof(bool), typeof(TabItem),
            new PropertyMetadata(true, new PropertyChangedCallback(OnInternalCanCloseChanged)));

        ///<summary>
        /// Get or set if this tab can be closed via the UI (i.e. the close button).
        ///</summary>
        ///<remarks>
        /// This property does not prevent this tab being closed via other means, such as <see cref="Close()"/> or <see cref="SelectableCollection{TabItem}.Remove(TabItem)"/>.
        /// </remarks>
        [Category("Common")]
        public bool CanClose
        {
            get { return (bool)GetValue(CanCloseProperty); }
            set { SetValue(CanCloseProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalCanCloseChanged;

        /// <summary>
        /// Raised when the CanClosed property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? CanCloseChanged;
#else
        public event DependencyPropertyChangedEventHandler CanCloseChanged;
#endif

        private static void OnInternalCanCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalCanCloseChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalCanCloseChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CanCloseChanged?.Invoke(this, e);
        }
        #endregion

        #region ShowIcon

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register("ShowIcon", typeof(bool), typeof(TabItem),
            new PropertyMetadata(true, new PropertyChangedCallback(OnInternalShowIconChanged)));

        ///<summary>
        /// Get or set if an icon is shown for this tab. The icon section of the tab will be visible if this is true, even if there is no icon set.
        ///</summary>
        [Category("Common")]
        public bool ShowIcon
        {
            get { return (bool)GetValue(ShowIconProperty); }
            set { SetValue(ShowIconProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalShowIconChanged;

        /// <summary>
        /// Raised when the ShowIcon property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ShowIconChanged;
#else
        public event DependencyPropertyChangedEventHandler ShowIconChanged;
#endif

        private static void OnInternalShowIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalShowIconChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalShowIconChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ShowIconChanged?.Invoke(this, e);
        }
        #endregion

        #region CanSelect

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty CanSelectProperty = DependencyProperty.Register("CanSelect", typeof(bool), typeof(TabItem),
            new PropertyMetadata(true, new PropertyChangedCallback(OnInternalCanSelectChanged)));

        ///<summary>
        /// Get or set whether this tab can be selected via UI.
        ///</summary>
        [Category("Common")]
        public bool CanSelect
        {
            get { return (bool)GetValue(CanSelectProperty); }
            set { SetValue(CanSelectProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalCanSelectChanged;

        /// <summary>
        /// Raised when the CanSelect property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? CanSelectChanged;
#else
        public event DependencyPropertyChangedEventHandler CanSelectChanged;
#endif

        private static void OnInternalCanSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalCanSelectChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalCanSelectChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CanSelectChanged?.Invoke(this, e);
        }
        #endregion

        #region Icon

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(TabItem),
            new PropertyMetadata(null, new PropertyChangedCallback(OnInternalIconChanged)));

        ///<summary>
        /// Get or set the icon to display with this tab.
        ///</summary>
        [Category("Common")]
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalIconChanged;

        /// <summary>
        /// Raised when the tab's icon is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? IconChanged;
#else
        public event DependencyPropertyChangedEventHandler IconChanged;
#endif

        private static void OnInternalIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalIconChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalIconChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            IconChanged?.Invoke(this, e);
        }
        #endregion

        #region TabBackground

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty TabBackgroundProperty = DependencyProperty.Register("TabBackground", typeof(Brush), typeof(TabItem),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent), new PropertyChangedCallback(OnInternalTabBackgroundChanged)));

        ///<summary>
        /// Get or set the brush displayed in the background of the tab. This will override the color of the TabControl itself, but transparency does allow the standard color to show through.
        ///</summary>
        [Category("Brush")]
        public Brush TabBackground
        {
            get { return (Brush)GetValue(TabBackgroundProperty); }
            set { SetValue(TabBackgroundProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalTabBackgroundChanged;

        /// <summary>
        /// Raised when the tab's background is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? TabBackgroundChanged;
#else
        public event DependencyPropertyChangedEventHandler TabBackgroundChanged;
#endif

        private static void OnInternalTabBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalTabBackgroundChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalTabBackgroundChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TabBackgroundChanged?.Invoke(this, e);
        }
        #endregion

        #region Content

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(UIElement), typeof(TabItem),
            new PropertyMetadata(null, new PropertyChangedCallback(OnInternalContentChanged)));

        ///<summary>
        /// Get or set the content to display when this tab is selected.
        ///</summary>
        [Category("Common")]
        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalContentChanged;

        /// <summary>
        /// Raised when the tab's content property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ContentChanged;
#else
        public event DependencyPropertyChangedEventHandler ContentChanged;
#endif

        private static void OnInternalContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalContentChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalContentChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ContentChanged?.Invoke(this, e);
        }
        #endregion

        #region Padding

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(TabItem),
            new PropertyMetadata(new Thickness(4,0,4,0), new PropertyChangedCallback(OnInternalPaddingChanged)));

        ///<summary>
        /// Get or set the padding (or space) applied around the tab's title and icon. (This does not set the padding for the content.)
        ///</summary>
        [Category("Common")]
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalPaddingChanged;

        /// <summary>
        /// Raised when the Padding property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? PaddingChanged;
#else
        public event DependencyPropertyChangedEventHandler PaddingChanged;
#endif

        private static void OnInternalPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalPaddingChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalPaddingChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PaddingChanged?.Invoke(this, e);
        }

        #endregion

        #region Visibility

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty VisibilityProperty = DependencyProperty.Register("Visibility", typeof(Visibility), typeof(TabItem),
            new PropertyMetadata(System.Windows.Visibility.Visible, new PropertyChangedCallback(OnInternalVisibilityChanged)));

        ///<summary>
        /// Get or set if this tab is visually displayed in the UI.
        ///</summary>
        [Category("Appearance")]
        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalVisibilityChanged;

        /// <summary>
        /// Raised when the tab's visibility property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? VisibilityChanged;
#else
        public event DependencyPropertyChangedEventHandler VisibilityChanged;
#endif

        private static void OnInternalVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalVisibilityChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            VisibilityChanged?.Invoke(this, e);
        }
        #endregion

        #region TabContextMenu

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty TabContextMenuProperty = DependencyProperty.Register("TabContextMenu", typeof(ContextMenu), typeof(TabItem),
            new PropertyMetadata(null));

#if NETCOREAPP
        /// <summary>
        /// Get or set the menu that appears when you right-click on the tab in the TabControl.
        /// </summary>
        [Category("Common")]
        public ContextMenu? TabContextMenu
        {
            get { return (ContextMenu)GetValue(TabContextMenuProperty); }
            set { SetValue(TabContextMenuProperty, value); }
        }
#else
        /// <summary>
        /// Get or set the menu that appears when you right-click on the tab in the TabControl.
        /// </summary>
        [Category("Common")]
        public ContextMenu TabContextMenu
        {
            get { return (ContextMenu)GetValue(TabContextMenuProperty); }
            set { SetValue(TabContextMenuProperty, value); }
        }
#endif

        #endregion

        #region AllowDrop

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty AllowDropProperty = DependencyProperty.Register("AllowDrop", typeof(bool), typeof(TabItem),
            new PropertyMetadata(false, new PropertyChangedCallback(OnInternalAllowDropChanged)));

        ///<summary>
        /// Get or set if this tab can be used as a target in drag-and-drop operations.
        ///</summary>
        ///<remarks>
        /// Note that this only sets if this tab can recieve data dropped onto it in the tab bar. The elements inside the tab can set their own AllowDrop property if you want to drop items inside the tab's contents.
        /// This property does not need to be set to allow tab drag-and-drop on the TabControl (for rearranging tabs). Tab drag-and-drop is controlled via the <see cref="TabControl.AllowTabDragDrop"/> property.
        /// </remarks>
        [Category("Common")]
        public bool AllowDrop
        {
            get { return (bool)GetValue(AllowDropProperty); }
            set { SetValue(AllowDropProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalAllowDropChanged;

        /// <summary>
        /// Raised when the AllowDrop property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? AllowDropChanged;
#else
        public event DependencyPropertyChangedEventHandler AllowDropChanged;
#endif

        private static void OnInternalAllowDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalAllowDropChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalAllowDropChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            AllowDropChanged?.Invoke(this, e);
        }
        #endregion

        #region Drag and drop events

        // The TabDisplayItem is the visual item that actually displays tabs in the TabControl
        // When a drag event occurs in the TabDisplayItem, it calls this RaiseDragEvent method so that the TabItem can raise the respective event.

        /// <summary>
        /// Instructs the TabItem to raise a particular drag-and-drop event, used internally to support drag-and-drop in TabControls.
        /// </summary>
        /// <param name="ev">The name of the event to raise.</param>
        /// <param name="e">The DragEventArgs to include with the event.</param>
        /// <remarks>
        /// Due to how the WPF architecture is set up, routed events are not possible within the TabItem class, and thus regular .NET events are used instead.
        /// This method allows visual representations of a TabItem (such as TabDisplayItem) to handle drag and drop events and hand them off to the TabItem.
        /// Although the TabItem and TabDisplayItem in Solid Shine UI don't support being a drag source by themselves (excepting to drag and drop tabs within
        /// a TabControl), the GiveFeedback and QueryContinueDrag events are present as well, in case it is needed by classes that inherit TabItem.
        /// See <see cref="RaiseGiveFeedbackEvent(GiveFeedbackEventArgs, bool)"/> and <see cref="RaiseQueryContinueDragEvent(QueryContinueDragEventArgs, bool)"/>
        /// for methods for raising those events.
        /// </remarks>
        internal protected void RaiseDragEvent(string ev, DragEventArgs e)
        {
            switch (ev)
            {
                case "DragEnter":
                    DragEnter?.Invoke(this, e);
                    break;
                case "DragOver":
                    DragOver?.Invoke(this, e);
                    break;
                case "DragLeave":
                    DragLeave?.Invoke(this, e);
                    break;
                case "Drop":
                    Drop?.Invoke(this, e);
                    break;
                case "PreviewDragEnter":
                    PreviewDragEnter?.Invoke(this, e);
                    break;
                case "PreviewDragOver":
                    PreviewDragOver?.Invoke(this, e);
                    break;
                case "PreviewDragLeave":
                    PreviewDragLeave?.Invoke(this, e);
                    break;
                case "PreviewDrop":
                    PreviewDrop?.Invoke(this, e);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Invoke the GiveFeedback event for drag-and-drop.
        /// </summary>
        /// <param name="e">The GiveFeedbackEventArgs to pass to the event.</param>
        /// <param name="preview">Determine if the PreviewGiveFeedback event should be raised, rather than GiveFeedback.</param>
        /// <remarks>See the remarks for <see cref="RaiseDragEvent(string, DragEventArgs)"/> for more information.</remarks>
        internal protected void RaiseGiveFeedbackEvent(GiveFeedbackEventArgs e, bool preview = false)
        {
            if (preview)
            {
                PreviewGiveFeedback?.Invoke(this, e);
            }
            else
            {
                GiveFeedback?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Invoke the QueryContinueDrag event for drag-and-drop.
        /// </summary>
        /// <param name="e">The QueryContinueDragEventArgs to pass to the event.</param>
        /// <param name="preview">Determine if the PreviewQueryContinueDrag event should be raised, rather than QueryContinueDrag.</param>
        /// <remarks>See the remarks for <see cref="RaiseDragEvent(string, DragEventArgs)"/> for more information.</remarks>
        internal protected void RaiseQueryContinueDragEvent(QueryContinueDragEventArgs e, bool preview = false)
        {
            if (preview)
            {
                PreviewQueryContinueDrag?.Invoke(this, e);
            }
            else
            {
                QueryContinueDrag?.Invoke(this, e);
            }
        }

#if NETCOREAPP
        /// <summary>
        /// Raised when data is dragged into the TabItem.
        /// </summary>
        /// <remarks>
        /// This specifically is raised when data is dragged within a TabItem's visual representation (such as a TabDisplayItem).
        /// Note that this event is not raised when a tab is dragged over another tab.
        /// Due to how the WPF architecture is designed, this is not a routed event.
        /// </remarks>
        public event DragEventHandler? DragEnter;
        /// <summary>
        /// Raised when data is dragged within this TabItem. This event is raised continuously while the data is over the tab.
        /// </summary>
        /// <remarks>
        /// This specifically is raised when data is dragged within a TabItem's visual representation (such as a TabDisplayItem).
        /// Note that this event is not raised when a tab is dragged over another tab.
        /// Due to how the WPF architecture is designed, this is not a routed event.
        /// </remarks>
        public event DragEventHandler? DragOver;
        /// <summary>
        /// Raised when data is dragged out of this TabItem, and is not dropped.
        /// </summary>
        /// <remarks>
        /// This specifically is raised when data is dragged within a TabItem's visual representation (such as a TabDisplayItem).
        /// Note that this event is not raised when a tab is dragged over another tab.
        /// Due to how the WPF architecture is designed, this is not a routed event.
        /// </remarks>
        public event DragEventHandler? DragLeave;
        /// <summary>
        /// Raised when data is dropped within this TabItem.
        /// </summary>
        /// <remarks>
        /// This specifically is raised when data is dragged within a TabItem's visual representation (such as a TabDisplayItem).
        /// Note that this event is not raised when a tab is dragged over another tab.
        /// Due to how the WPF architecture is designed, this is not a routed event.
        /// </remarks>
        public event DragEventHandler? Drop;

        /// <summary>
        /// Raised while a drag-and-drop action is occurring, allowing feedback to be provided to the user and the drop target.
        /// </summary>
        public event GiveFeedbackEventHandler? GiveFeedback;
        /// <summary>
        /// Raised while a drag-and-drop action is occurring, allowing the drag-and-drop to be cancelled in certain situations.
        /// </summary>
        public event QueryContinueDragEventHandler? QueryContinueDrag;

        /// <summary>
        /// Raised when data is dragged into this TabItem. This is a tunneling version of the <see cref="DragEnter"/> event.
        /// </summary>
        public event DragEventHandler? PreviewDragEnter;
        /// <summary>
        /// Raised when data is dragged within this TabItem. This is a tunneling version of the <see cref="DragOver"/> event.
        /// </summary>
        public event DragEventHandler? PreviewDragOver;
        /// <summary>
        /// Raised when data is dragged out of this TabItem, without being dropped. This is a tunneling version of the <see cref="DragLeave"/> event.
        /// </summary>
        public event DragEventHandler? PreviewDragLeave;
        /// <summary>
        /// Raised when data is dropped within this TabItem. This is a tunneling version of the <see cref="Drop"/> event.
        /// </summary>
        public event DragEventHandler? PreviewDrop;

        /// <summary>
        /// Raised while a drag-and-drop action is occurring, allowing feedback to be provided to the user and the drop target.
        /// This is a tunneling version of the <see cref="GiveFeedback"/> event.
        /// </summary>
        public event GiveFeedbackEventHandler? PreviewGiveFeedback;
        /// <summary>
        /// Raised while a drag-and-drop action is occurring, allowing the drag-and-drop to be cancelled in certain situations.
        /// This is a tunneling version of the <see cref="QueryContinueDrag"/> event.
        /// </summary>
        public event QueryContinueDragEventHandler? PreviewQueryContinueDrag;
#else
        /// <summary>
        /// Raised when data is dragged into the TabItem.
        /// </summary>
        /// <remarks>
        /// This specifically is raised when data is dragged within a TabItem's visual representation (such as a TabDisplayItem).
        /// Note that this event is not raised when a tab is dragged over another tab.
        /// Due to how the WPF architecture is designed, this is not a routed event.
        /// </remarks>
        public event DragEventHandler DragEnter;
        /// <summary>
        /// Raised when data is dragged within this TabItem. This event is raised continuously while the data is over the tab.
        /// </summary>
        /// <remarks>
        /// This specifically is raised when data is dragged within a TabItem's visual representation (such as a TabDisplayItem).
        /// Note that this event is not raised when a tab is dragged over another tab.
        /// Due to how the WPF architecture is designed, this is not a routed event.
        /// </remarks>
        public event DragEventHandler DragOver;
        /// <summary>
        /// Raised when data is dragged out of this TabItem, and is not dropped.
        /// </summary>
        /// <remarks>
        /// This specifically is raised when data is dragged within a TabItem's visual representation (such as a TabDisplayItem).
        /// Note that this event is not raised when a tab is dragged over another tab.
        /// Due to how the WPF architecture is designed, this is not a routed event.
        /// </remarks>
        public event DragEventHandler DragLeave;
        /// <summary>
        /// Raised when data is dropped within this TabItem.
        /// </summary>
        /// <remarks>
        /// This specifically is raised when data is dragged within a TabItem's visual representation (such as a TabDisplayItem).
        /// Note that this event is not raised when a tab is dragged over another tab.
        /// Due to how the WPF architecture is designed, this is not a routed event.
        /// </remarks>
        public event DragEventHandler Drop;

        /// <summary>
        /// Raised while a drag-and-drop action is occurring, allowing feedback to be provided to the user and the drop target.
        /// </summary>
        public event GiveFeedbackEventHandler GiveFeedback;
        /// <summary>
        /// Raised while a drag-and-drop action is occurring, allowing the drag-and-drop to be cancelled in certain situations.
        /// </summary>
        public event QueryContinueDragEventHandler QueryContinueDrag;

        /// <summary>
        /// Raised when data is dragged into this TabItem. This is a tunneling version of the <see cref="DragEnter"/> event.
        /// </summary>
        public event DragEventHandler PreviewDragEnter;
        /// <summary>
        /// Raised when data is dragged within this TabItem. This is a tunneling version of the <see cref="DragOver"/> event.
        /// </summary>
        public event DragEventHandler PreviewDragOver;
        /// <summary>
        /// Raised when data is dragged out of this TabItem, without being dropped. This is a tunneling version of the <see cref="DragLeave"/> event.
        /// </summary>
        public event DragEventHandler PreviewDragLeave;
        /// <summary>
        /// Raised when data is dropped within this TabItem. This is a tunneling version of the <see cref="Drop"/> event.
        /// </summary>
        public event DragEventHandler PreviewDrop;

        /// <summary>
        /// Raised while a drag-and-drop action is occurring, allowing feedback to be provided to the user and the drop target.
        /// This is a tunneling version of the <see cref="GiveFeedback"/> event.
        /// </summary>
        public event GiveFeedbackEventHandler PreviewGiveFeedback;
        /// <summary>
        /// Raised while a drag-and-drop action is occurring, allowing the drag-and-drop to be cancelled in certain situations.
        /// This is a tunneling version of the <see cref="QueryContinueDrag"/> event.
        /// </summary>
        public event QueryContinueDragEventHandler PreviewQueryContinueDrag;
#endif

        #endregion

        #region ToolTip

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty ToolTipProperty = DependencyProperty.Register("ToolTip", typeof(object), typeof(TabItem),
            new PropertyMetadata(null, new PropertyChangedCallback(OnInternalToolTipChanged)));

        ///<summary>
        /// Get or set the ToolTip to display when the mouse is hovering over this tab.
        ///</summary>
        [Category("Common")]
        public object ToolTip
        {
            get { return (object)GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalToolTipChanged;

        /// <summary>
        /// Raised when the tab's ToolTip property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ToolTipChanged;
#else
        public event DependencyPropertyChangedEventHandler ToolTipChanged;
#endif

        private static void OnInternalToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalToolTipChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalToolTipChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ToolTipChanged?.Invoke(this, e);
        }
        #endregion

        #region ExtraTabElement

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty ExtraTabElementProperty = DependencyProperty.Register("ExtraTabElement", typeof(UIElement), typeof(TabItem),
            new PropertyMetadata(null));

        /// <summary>
        /// Get or set the element to display on the right side of the tab, next to the Close button.
        /// </summary>
        public UIElement ExtraTabElement
        {
            get { return (UIElement)GetValue(ExtraTabElementProperty); }
            set { SetValue(ExtraTabElementProperty, value); }
        }

        #endregion

        #region Font Properties

        /// <summary>The backing dependency property for <see cref="FontSize"/>. See the related property for details.</summary>
        public static readonly DependencyProperty FontSizeProperty = Control.FontSizeProperty.AddOwner(typeof(TabItem));

        /// <summary>
        /// Get or set the font size to use for the <see cref="Title"/> and other text in the TabItem.
        /// </summary>
        [Category("Appearance")]
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        /// <summary>The backing dependency property for <see cref="FontFamily"/>. See the related property for details.</summary>
        public static readonly DependencyProperty FontFamilyProperty = Control.FontFamilyProperty.AddOwner(typeof(TabItem));

        /// <summary>
        /// Get or set the font family to use for the <see cref="Title"/> and other text in the TabItem.
        /// </summary>
        [Category("Appearance")]
        public FontFamily FontFamily
        {
            get => (FontFamily)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        /// <summary>The backing dependency property for <see cref="FontStyle"/>. See the related property for details.</summary>
        public static readonly DependencyProperty FontStyleProperty = Control.FontStyleProperty.AddOwner(typeof(TabItem));

        /// <summary>
        /// Get or set the font style to use for the <see cref="Title"/> and other text in the TabItem.
        /// </summary>
        [Category("Appearance")]
        public FontStyle FontStyle
        {
            get => (FontStyle)GetValue(FontStyleProperty);
            set => SetValue(FontStyleProperty, value);
        }

        /// <summary>The backing dependency property for <see cref="FontStretch"/>. See the related property for details.</summary>
        public static readonly DependencyProperty FontStretchProperty = Control.FontStretchProperty.AddOwner(typeof(TabItem));

        /// <summary>
        /// Get or set the font stretch to use for the <see cref="Title"/> and other text in the TabItem.
        /// </summary>
        [Category("Appearance")]
        public FontStretch FontStretch
        {
            get => (FontStretch)GetValue(FontStretchProperty);
            set => SetValue(FontStretchProperty, value);
        }

        /// <summary>The backing dependency property for <see cref="FontWeight"/>. See the related property for details.</summary>
        public static readonly DependencyProperty FontWeightProperty = Control.FontWeightProperty.AddOwner(typeof(TabItem));

        /// <summary>
        /// Get or set the font weight to use for the <see cref="Title"/> and other text in the TabItem.
        /// </summary>
        [Category("Appearance")]
        public FontWeight FontWeight
        {
            get => (FontWeight)GetValue(FontWeightProperty);
            set => SetValue(FontWeightProperty, value);
        }

        #endregion

        /// <summary>
        /// Close this tab (and remove it from the TabControl), if it is currently in a TabControl.
        /// </summary>
        /// <remarks>
        /// This function can be run regardless of the state of the <c>CanClose</c> property.
        /// </remarks>
        public void Close()
        {
            RequestTabClosing?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Attempt to bring the current tab into view on the tab bar
        /// </summary>
        public void BringIntoView()
        {
            BringIntoViewRequested?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// Used internally to request closing of this tab.
        /// </summary>
#if NETCOREAPP
        internal protected event EventHandler? RequestTabClosing;

        /// <summary>
        /// Used internally to request bringing this tab into view.
        /// </summary>
        internal protected event EventHandler? BringIntoViewRequested;
#else
        internal protected event EventHandler RequestTabClosing;
        
        /// <summary>
        /// Used internally to request bringing this tab into view.
        /// </summary>
        internal protected event EventHandler BringIntoViewRequested;
#endif

        ///<summary>
        /// Get or set if this tab is visible in the UI.
        ///</summary>
        [Category("Appearance")]
        public bool IsVisible
        {
            get
            {
                return Visibility == Visibility.Visible;
            }
            set
            {
                Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

    }
}
