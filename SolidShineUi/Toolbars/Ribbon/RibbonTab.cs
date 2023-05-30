using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace SolidShineUi.Toolbars.Ribbon
{
    /// <summary>
    /// A tab for a <see cref="Ribbon"/>, which can display a list of commands that a user can select.
    /// </summary>
    [ContentProperty("Items")]
    public class RibbonTab : DependencyObject
    {
        public RibbonTab()
        {
            SetValue(ItemsPropertyKey, new ObservableCollection<RibbonGroup>());

            InternalPaddingChanged += tabItem_InternalPaddingChanged;
        }

        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public static DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(string), typeof(RibbonTab),
            new FrameworkPropertyMetadata("Tab"));

        public bool FitContentsToWidth { get => (bool)GetValue(FitContentsToWidthProperty); set => SetValue(FitContentsToWidthProperty, value); }

        public static DependencyProperty FitContentsToWidthProperty
            = DependencyProperty.Register("FitContentsToWidth", typeof(bool), typeof(RibbonTab),
            new FrameworkPropertyMetadata(true));

        public Visibility Visibility { get => (Visibility)GetValue(VisibilityProperty); set => SetValue(VisibilityProperty, value); }

        public static DependencyProperty VisibilityProperty
            = DependencyProperty.Register("Visibility", typeof(Visibility), typeof(RibbonTab),
            new FrameworkPropertyMetadata(Visibility.Visible));

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        private static readonly DependencyPropertyKey ItemsPropertyKey
            = DependencyProperty.RegisterReadOnly("Items", typeof(ObservableCollection<RibbonGroup>), typeof(RibbonTab),
            new FrameworkPropertyMetadata(new ObservableCollection<RibbonGroup>()));

        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the list of items in this RibbonGroup. This Items property can be used to add and remove items.
        /// </summary>
        [Category("Common")]
        public ObservableCollection<RibbonGroup> Items
        {
            get { return (ObservableCollection<RibbonGroup>)GetValue(ItemsProperty); }
            private set { SetValue(ItemsPropertyKey, value); }
        }

        #region Padding

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(RibbonTab),
            new PropertyMetadata(new Thickness(4, 0, 4, 0), new PropertyChangedCallback(OnInternalPaddingChanged)));

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
            if (d is RibbonTab s)
            {
                s.InternalPaddingChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalPaddingChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PaddingChanged?.Invoke(this, e);
        }

        #endregion

        #region Color Scheme
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
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(RibbonTab),
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

            if (d is RibbonTab c)
            {
                c.ColorSchemeChanged?.Invoke(d, e);
                c.ApplyColorScheme(cs);
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
        }
        #endregion

        #region Contextual Tab
        public bool IsContextual { get => (bool)GetValue(IsContextualProperty); set => SetValue(IsContextualProperty, value); }

        public static DependencyProperty IsContextualProperty
            = DependencyProperty.Register("IsContextual", typeof(bool), typeof(RibbonTab),
            new FrameworkPropertyMetadata(false));

        public Color ContextualColor { get => (Color)GetValue(ContextualColorProperty); set => SetValue(ContextualColorProperty, value); }

        public static DependencyProperty ContextualColorProperty
            = DependencyProperty.Register("ContextualColor", typeof(Color), typeof(RibbonTab),
            new FrameworkPropertyMetadata(Colors.Blue));
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

        /// <summary>
        /// Attempt to bring the current tab into view on the Ribbon.
        /// </summary>
        public void BringIntoView()
        {
            InternalBringIntoViewRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Internal event for handling the <see cref="BringIntoView"/> function. When this is raised, this should prompt the UI to bring the related <see cref="Utils.RibbonTabDisplayItem"/> 
        /// (or other similar control) into view (see <see cref="FrameworkElement.BringIntoView()"/>).
        /// </summary>
#if NETCOREAPP
        internal protected event EventHandler? InternalBringIntoViewRequested;
#else
        internal protected event EventHandler InternalBringIntoViewRequested;
#endif
    }
}
