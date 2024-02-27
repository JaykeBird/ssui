using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace SolidShineUi.Ribbon
{
    /// <summary>
    /// A tab for a <see cref="Ribbon"/>, which can display a list of commands that a user can select.
    /// </summary>
    [ContentProperty("Items")]
    public class RibbonTab : DependencyObject
    {
        /// <summary>
        /// Create a RibbonTab.
        /// </summary>
        public RibbonTab()
        {
            SetValue(ItemsPropertyKey, new ObservableCollection<RibbonGroup>());

            Items.CollectionChanged += Items_CollectionChanged;
        }

        /// <summary>
        /// Get or set the Title for this tab. This is displayed on the Ribbon UI for users to select.
        /// </summary>
        /// <remarks>
        /// The title should succinctly explain the commands that will be found on this tab. Similar commands should be grouped together on one tab,
        /// and the title can identify all these commands in a general sense. For example, a title of "View" could be used for a tab that houses commands
        /// that are related to changing the view, zoom, or other aspects of displaying your application's content.
        /// <para/>
        /// Common convention is to use the title "Home" for the first tab on your Ribbon, which will contain your application's most commonly used commands.
        /// </remarks>
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public static DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(string), typeof(RibbonTab),
            new FrameworkPropertyMetadata("Tab"));

        public bool FitContentsToWidth { get => (bool)GetValue(FitContentsToWidthProperty); set => SetValue(FitContentsToWidthProperty, value); }

        public static DependencyProperty FitContentsToWidthProperty
            = DependencyProperty.Register("FitContentsToWidth", typeof(bool), typeof(RibbonTab),
            new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Get or set the visibility for this RibbonTab. A value of <c>Collapsed</c> will cause this RibbonTab to not appear on the Ribbon.
        /// </summary>
        public Visibility Visibility { get => (Visibility)GetValue(VisibilityProperty); set => SetValue(VisibilityProperty, value); }

        public static DependencyProperty VisibilityProperty
            = DependencyProperty.Register("Visibility", typeof(Visibility), typeof(RibbonTab),
            new FrameworkPropertyMetadata(Visibility.Visible,
                new PropertyChangedCallback((o, e) => o.PerformAs<RibbonTab>((t) => t.VisibilityChanged?.Invoke(t, e)))));

        /// <summary>
        /// Raised when the <see cref="Visibility"/> value has changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? VisibilityChanged;
#else
        public event DependencyPropertyChangedEventHandler VisibilityChanged;
#endif

        #region Items

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        private static readonly DependencyPropertyKey ItemsPropertyKey
            = DependencyProperty.RegisterReadOnly("Items", typeof(ObservableCollection<RibbonGroup>), typeof(RibbonTab),
            new FrameworkPropertyMetadata(new ObservableCollection<RibbonGroup>()));

        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the list of groups in this RibbonTab. This Items property can be used to add and remove items.
        /// </summary>
        [Category("Common")]
        public ObservableCollection<RibbonGroup> Items
        {
            get { return (ObservableCollection<RibbonGroup>)GetValue(ItemsProperty); }
            private set { SetValue(ItemsPropertyKey, value); }
        }

#if NETCOREAPP
        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
#else
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
#endif
        {
            //if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
            //{
            //    foreach (var item in e.NewItems)
            //    {
            //        if (item is RibbonGroup group)
            //        {

            //        }
            //    }
            //}
        }
        #endregion

        #region Padding

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(RibbonTab),
            new PropertyMetadata(new Thickness(12, 0, 12, 0), new PropertyChangedCallback((o, e) => o.PerformAs<RibbonTab>((t) => t.PaddingChanged?.Invoke(t, e)))));

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
        /// Raised when the Padding property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? PaddingChanged;
#else
        public event DependencyPropertyChangedEventHandler PaddingChanged;
#endif

        #endregion

        #region Contextual Tab
        /// <summary>
        /// Get or set if this is a contextual tab. A contextual tab should only be displayed in the situations or contexts where the commands within it are useful,
        /// and otherwise should be hidden (such as displaying controls for editing a picture only while a picture is selected)
        /// </summary>
        /// <remarks>
        /// To help contextual tabs stand out in the UI, they're often assigned a special color. Use <see cref="ContextualColor"/> to set that color for the tab.
        /// </remarks>
        public bool IsContextual { get => (bool)GetValue(IsContextualProperty); set => SetValue(IsContextualProperty, value); }

        /// <summary>
        /// The backing dependency property for <see cref="IsContextual"/>. See the related property for details.
        /// </summary>
        public static DependencyProperty IsContextualProperty
            = DependencyProperty.Register("IsContextual", typeof(bool), typeof(RibbonTab),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get or set the color to use with this tab, if this is a contextual tab (if <see cref="IsContextual"/> is set to <c>true</c>).
        /// </summary>
        public Color ContextualColor { get => (Color)GetValue(ContextualColorProperty); set => SetValue(ContextualColorProperty, value); }

        /// <summary>
        /// The backing dependency property for <see cref="ContextualColor"/>. See the related property for details.
        /// </summary>
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
