using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SolidShineUi
{

    /// <summary>
    /// Event arguments for the TabClosing event in TabControl.
    /// </summary>
    public class TabItemClosingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Create a TabItemClosingEventArgs.
        /// </summary>
        /// <param name="t">The tab item being closed.</param>
        public TabItemClosingEventArgs(TabItem t)
        {
            TabItem = t;
        }

        /// <summary>
        /// The TabItem being closed (and removed from the TabControl).
        /// </summary>
        public TabItem TabItem { get; private set; }
    }

    /// <summary>
    /// Event arguments for the TabChanged event in TabControl.
    /// </summary>
    public class TabItemChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Create a TabItemChangeEventArgs.
        /// </summary>
        /// <param name="t">The TabItem being changed to.</param>
        public TabItemChangeEventArgs(TabItem t)
        {
            TabItem = t;
        }

        /// <summary>
        /// The TabItem being changed to. This is the new TabItem being displayed.
        /// </summary>
        public TabItem TabItem { get; private set; }
    }

    /// <summary>
    /// Event arguments for a TabItem being dropped onto a TabControl. Primarily used internally.
    /// </summary>
    public class TabItemDropEventArgs : EventArgs
    {
        /// <summary>
        /// Create a TabItemDropEventArgs.
        /// </summary>
        /// <param name="sourceTabItem">The tab item that triggered the event.</param>
        /// <param name="droppedTabItem">The tab item to be dropped.</param>
        /// <param name="before">Determine whether to drop the <paramref name="droppedTabItem"/> before or after the <paramref name="sourceTabItem"/>.</param>
        public TabItemDropEventArgs(TabItem sourceTabItem, TabItem droppedTabItem, bool before)
        {
            SourceTabItem = sourceTabItem;
            DroppedTabItem = droppedTabItem;
            PlaceBefore = before;
        }

        /// <summary>
        /// Get the TabItem that triggered the TabItemDrop event.
        /// </summary>
        public TabItem SourceTabItem { get; private set; }

        /// <summary>
        /// Get the TabItem that is being dropped.
        /// </summary>
        public TabItem DroppedTabItem { get; private set; }

        /// <summary>
        /// Get whether the dropped TabItem should be put before or after the source TabItem.
        /// </summary>
        public bool PlaceBefore { get; private set; }
    }


    /// <summary>
    /// A delegate to be used with events regarding the selected <see cref="TabItem"/> changing in a <see cref="TabControl"/>, such as <see cref="TabControl.TabChanged"/>.
    /// </summary>
    /// <param name="sender">The object where the event was raised.</param>
    /// <param name="e">The event arguments associated with this event.</param>
    public delegate void TabItemChangeEventHandler(object sender, TabItemChangeEventArgs e);

    /// <summary>
    /// A delegate to be used with the <see cref="TabControl.TabClosing"/> event, which is raised immediately before a tab is closed (to provide an ability to cancel closing the tab).
    /// </summary>
    /// <param name="sender">The object where the event was raised.</param>
    /// <param name="e">The event arguments associated with this event.</param>
    public delegate void TabItemClosingEventHandler(object sender, TabItemClosingEventArgs e);
    
    /// <summary>
    /// A delegate to be used with events regarding dropping a TabItem into a TabControl.
    /// </summary>
    /// <param name="sender">The object where the event was raised.</param>
    /// <param name="e">The event arguments associated with this event.</param>
    public delegate void TabItemDropEventHandler(object sender, TabItemDropEventArgs e);
}
