using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

#if AVALONIA
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
#else
using System.Windows;
using System.Windows.Controls;
#endif

namespace SolidShineUi.KeyboardShortcuts
{
    /// <summary>
    /// A key action that executes a routed event when activated.
    /// </summary>
    public class RoutedEventKeyAction : IKeyAction
    {
#if AVALONIA
        EventHandler<RoutedEventArgs>? reh = null;

        /// <summary>
        /// Gets the UI element that this action is related to, if any.
        /// </summary>
        public InputElement? SourceElement { get; private set; }

        /// <summary>
        /// Create a RoutedEventKeyAction.
        /// </summary>
        /// <param name="reh">The RoutedEventHandler to invoke when this key action is activated.</param>
        /// <param name="methodId">The unique ID to associate with this key action.</param>
        /// <param name="sourceElement">The UI element, if any, associated with this RoutedEventHandler. For example, it could be a menu item or button that would alternatively invoke this routed event handler.</param>
        public RoutedEventKeyAction(EventHandler<RoutedEventArgs> reh, string methodId, InputElement? sourceElement = null)
#elif NETCOREAPP
        RoutedEventHandler? reh = null;

        /// <summary>
        /// Gets the UI element that this action is related to, if any.
        /// </summary>
        public UIElement? SourceElement { get; private set; }

        /// <summary>
        /// Create a RoutedEventKeyAction.
        /// </summary>
        /// <param name="reh">The RoutedEventHandler to invoke when this key action is activated.</param>
        /// <param name="methodId">The unique ID to associate with this key action.</param>
        /// <param name="sourceElement">The UI element, if any, associated with this RoutedEventHandler. For example, it could be a menu item or button that would alternatively invoke this routed event handler.</param>
        public RoutedEventKeyAction(RoutedEventHandler reh, string methodId, UIElement? sourceElement = null)
#else
        RoutedEventHandler reh = null;

        /// <summary>
        /// Gets the UI element that this action is related to, if any.
        /// </summary>
        public UIElement SourceElement { get; private set; }

        /// <summary>
        /// Create a RoutedEventKeyAction.
        /// </summary>
        /// <param name="reh">The RoutedEventHandler to invoke when this key action is activated.</param>
        /// <param name="methodId">The unique ID to associate with this key action.</param>
        /// <param name="sourceElement">The UI element, if any, associated with this RoutedEventHandler. For example, it could be a menu item or button that would alternatively invoke this routed event handler.</param>
        public RoutedEventKeyAction(RoutedEventHandler reh, string methodId, UIElement sourceElement = null)
#endif
        {
            ID = methodId;
            this.reh = reh;
            SourceElement = sourceElement;
        }

        /// <summary>
        /// Get or set the unique ID associated with this key action.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Activate this key action. This will execute the stored event handler.
        /// </summary>
        /// <remarks>
        /// The <see cref="SourceElement"/> will be passed as the <c>source</c> for the event handler, and this RoutedEventKeyAction will be
        /// listed as the original source under <see cref="RoutedEventArgs.Source"/>.
        /// </remarks>
        public void Execute()
        {
#if AVALONIA
            reh?.Invoke((object?)SourceElement ?? this, new RoutedEventArgs(InputElement.KeyDownEvent, this));
#elif NETCOREAPP
            reh?.Invoke((object?)SourceElement ?? this, new RoutedEventArgs(UIElement.KeyDownEvent, this));
#else
            reh?.Invoke((object)SourceElement ?? this, new RoutedEventArgs(UIElement.KeyDownEvent, this));
#endif
        }

#if AVALONIA
        // Avalonia stores all routed event handlers for all Avalonia objects using an internal _eventHandlers field
        // https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Base/Interactivity/Interactive.cs

        /// <summary>
        /// Create a list of key actions from all the menu items in a particular menu. Each menu item with a name and Click event handler will be added to this list.
        /// </summary>
        /// <param name="m">The menu to load from.</param>
        /// <remarks>No exceptions are thrown from this method; a blank list is returned if no menu items could be added. Each menu item should have a name (via the <c>Name</c> or <c>x:Name</c> properties).
        /// If multiple menu items do not have a name, then only the first one encountered will be added to the list.</remarks>
        /// <returns>A list of key actions created from the menu passed in.</returns>
        public static KeyActionList CreateListFromMenu(Avalonia.Controls.Menu m)
        {
            KeyActionList rekya = new KeyActionList();

            foreach (object? element in m.Items)
            {
                if (element is MenuItem mi && element != null)
                {
                    if (mi.Items.Count > 0)
                    {
                        rekya.AddRange(CreateListFromMenuItem(mi));
                    }
                    else
                    {
                        // if this a top-level MenuItem that has a click event of its own, then okay
                        try
                        {
                            RoutedEventKeyAction? ka = CreateFromClickEvent(mi);
                            if (ka != null)
                            {
                                rekya.Add(ka);
                            }
                        }
                        catch (MissingMemberException) { }
                        catch (InvalidOperationException) { }
                    }
                }
            }

            return rekya;
        }

        private static List<RoutedEventKeyAction> CreateListFromMenuItem(MenuItem mmi)
        {
            List<RoutedEventKeyAction> rekya = new List<RoutedEventKeyAction>();

            foreach (object? item in mmi.Items)
            {
                if (item is MenuItem mi && item != null)
                {
                    try
                    {
                        RoutedEventKeyAction? ka = CreateFromClickEvent(mi);
                        if (ka != null)
                        {
                            rekya.Add(ka);
                        }
                    }
                    catch (MissingMemberException) { }
                    catch (InvalidOperationException) { }

                    if (mi.Items.Count > 0)
                    {
                        rekya.AddRange(CreateListFromMenuItem(mi));
                    }
                }
            }

            return rekya;
        }

        /// <summary>
        /// Create a RoutedEventKeyAction based upon a Click event handler of a control.
        /// </summary>
        /// <param name="i">The control to look at (and pull the event handler from)</param>
        /// <returns>A <see cref="RoutedEventKeyAction"/> if there is an event handler on the object's Click event, or <c>null</c> if no handler or Click event could be found</returns>
        /// <remarks>
        /// Note that the Click event must be an Avalonia routed event. This uses <see cref="CreateFromRoutedEvent(Interactive, string)"/> to locate the event handler via .NET reflection,
        /// so look at the notes of that method for more details about how it works.
        /// <para/>
        /// No exceptions are thrown as a result of this method; if a valid RoutedEventKeyAction could not be created, then <c>null</c> is returned.
        /// Use <see cref="CreateFromRoutedEvent(Interactive, string)"/> if you do want thrown exceptions.
        /// <para/>
        /// The returned RoutedEventKeyAction will continue to hold onto and execute the event handler even after it was unregistered from the event,
        /// and it will also result in the object holding the event handler continuing to not be garbage collected, so it's best to utilize this to pull event
        /// handlers on objects that will persist as long as this RoutedEventKeyAction will.
        /// </remarks>
        public static RoutedEventKeyAction? CreateFromClickEvent(Interactive i)
        {
            try
            {
                return CreateFromRoutedEvent(i, "Click");
            }
            catch (MissingMemberException)
            {
                return null;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        /// <summary>
        /// Create a RoutedEventKeyAction based upon an event handler for a control's routed event.
        /// </summary>
        /// <param name="i">the Avalonia-based object to look at (and pull the event handler from)</param>
        /// <param name="routedEventName">the name of the routed event to get the handler of</param>
        /// <exception cref="MissingMemberException">thrown if this object does not have an event with the name in <paramref name="routedEventName"/></exception>
        /// <returns>
        /// A new <see cref="RoutedEventKeyAction"/> that can activate the found event handler, 
        /// or <c>null</c> if there are no event handlers for the <paramref name="routedEventName"/> event
        /// </returns>
        /// <remarks>
        /// .NET reflection is used access the internal code of Avalonia and locate a registered event handler for an Avalonia routed event.
        /// This won't work on events that aren't Avalonia routed events, and this also won't work if there are no handlers for the given event.
        /// This code looks at the current list of registered event handlers for a given event; if there are multiple handlers, then only the first is selected.
        /// The returned RoutedEventKeyAction will continue to hold onto and execute the event handler even after it was unregistered from the event,
        /// and it will also result in the object holding the event handler to not be garbage collected, so it's best to utilize this with the event
        /// handlers on objects that will persist as long as this RoutedEventKeyAction.
        /// </remarks>
        public static RoutedEventKeyAction? CreateFromRoutedEvent(Interactive i, string routedEventName)
        {
            var ehListField = i.GetType().GetField("_eventHandlers", BindingFlags.Instance | BindingFlags.NonPublic);
            var ehList = ehListField?.GetValue(i);
            if (ehList == null)
            {
                // if this is null, then either the internal design has radically changed
                // or the control hasn't actually had this event set yet (or any event for that matter)

                throw new MissingMemberException("This object does not have any event handlers set up for routed events.", nameof(i));
            }
            else if (ehList is IDictionary id)
            {
                RoutedEvent? ourEvent = null;

                foreach (object item in id.Keys)
                {
                    if (item is RoutedEvent re)
                    {
                        if (re.Name == routedEventName)
                        {
                            // we have our click event (or whatever)
                            ourEvent = re;
                        }
                    }
                }

                if (ourEvent == null)
                {
                    throw new MissingMemberException("This object does not have a handler for the event \"" + routedEventName + "\".", nameof(i));
                }
                else
                {
                    var subList = id[ourEvent];
                    if (subList != null)
                    {
                        if (subList is IList l)
                        {
                            // soo this is a List of EventSubscription objects
                            // but EventSubscription is a private struct
                            // so in GetHandlerFromEventSubscription, we'll use reflection again to dive into the object and get what we need

                            if (l.Count > 0)
                            {
                                Delegate? dg = GetHandlerFromEventSubscription(l[0]);
                                if (dg is EventHandler<RoutedEventArgs> e)
                                {
                                    InputElement? source = null;
                                    if (i is InputElement ie) source = ie;
                                    return new RoutedEventKeyAction(e, routedEventName, source);
                                }
                            }
                            else
                            {
                                return null;
                                //throw new MissingMemberException("This object has no Click event handlers.", nameof(i));
                            }
                        }
                    }
                }
            }

            return null;

            static Delegate? GetHandlerFromEventSubscription(object? subscription)
            {
                if (subscription is null)
                {
                    return null;
                    //throw new ArgumentNullException(nameof(subscription));
                }

                // so EventSubscription is a private struct, soooo I'll have to rely back upon reflection
                Type subType = subscription.GetType();
                PropertyInfo? handlerProp = subType.GetProperty("Handler", BindingFlags.Instance | BindingFlags.Public);
                if (handlerProp != null)
                {
                    return (Delegate?)handlerProp.GetValue(subscription, null);
                }

                return null;
            }
        }

#else
        /// <summary>
        /// Create a list of key actions from all the menu items in a particular menu. Each menu item with a name and Click event handler will be added to this list.
        /// </summary>
        /// <param name="m">The menu to load from.</param>
        /// <remarks>No exceptions are thrown from this method; a blank list is returned if no menu items could be added. Each menu item should have a name (via the <c>Name</c> or <c>x:Name</c> properties).
        /// If multiple menu items do not have a name, then only the first one encountered will be added to the list.</remarks>
        /// <returns>A list of key actions created from the menu passed in.</returns>
        public static KeyActionList CreateListFromMenu(Menu m)
        {
            KeyActionList rekya = new KeyActionList();

#if NETCOREAPP
            foreach (object? element in m.Items)
#else
            foreach (object element in m.Items)
#endif
            {
                if (element is MenuItem mi && element != null)
                {
                    if (mi.Items.Count > 0)
                    {
                        rekya.AddRange(CreateListFromMenuItem(mi));
                    }
                    else
                    {
                        // if this a top-level MenuItem that has a click event of its own, then okay
                        try
                        {
                            rekya.Add(CreateFromMenuItem(mi));
                        }
                        catch (MissingMemberException) { }
                        catch (InvalidOperationException) { }
                    }
                }
            }

            return rekya;
        }

        private static List<RoutedEventKeyAction> CreateListFromMenuItem(MenuItem mmi)
        {
            List<RoutedEventKeyAction> rekya = new List<RoutedEventKeyAction>();

#if NETCOREAPP
            foreach (object? item in mmi.Items)
#else
            foreach (object item in mmi.Items)
#endif
            {
                if (item is MenuItem mi && item != null)
                {
                    try
                    {
                        rekya.Add(CreateFromMenuItem(mi));
                    }
                    catch (MissingMemberException) { }
                    catch (InvalidOperationException) { }

                    if (mi.Items.Count > 0)
                    {
                        rekya.AddRange(CreateListFromMenuItem(mi));
                    }
                }
            }

            return rekya;
        }

        /// <summary>
        /// Generate a RoutedEventKeyAction using the Click event of a MenuItem. If the MenuItem doesn't have the Click event handled, an exception is thrown.
        /// </summary>
        /// <param name="mi">The menu item to use the Click event from.</param>
        /// <exception cref="MissingMethodException">Thrown if the MenuItem does not have any Click event handlers.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there was an issue entering into the MenuItem via reflection. If this error occurs, please contact the library creator.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the MenuItem is null.</exception>
        public static RoutedEventKeyAction CreateFromMenuItem(MenuItem mi)
        {
            if (mi != null)
            {
                try
                {
                    // source: https://stackoverflow.com/questions/982709/removing-routed-event-handlers-through-reflection
                    var eventInfo = mi.GetType().GetEvent("Click", BindingFlags.Public | BindingFlags.FlattenHierarchy);
                    TypeInfo t = mi.GetType().GetTypeInfo();
                    var clickEvent = t.DeclaredFields.Where((ei) => { return ei.Name == "ClickEvent"; }).First();
#if NETCOREAPP
                    RoutedEvent re = (RoutedEvent)clickEvent.GetValue(mi)!;

                    PropertyInfo pi = t.GetProperty("EventHandlersStore", BindingFlags.Instance | BindingFlags.NonPublic)!;
                    var ehs = pi.GetValue(mi);
                    if (ehs != null)
                    {
                        var delegates = (RoutedEventHandlerInfo[]?)ehs.GetType().GetMethod("GetRoutedEventHandlers")!?.Invoke(ehs, new object[] { MenuItem.ClickEvent });
#else
                    RoutedEvent re = (RoutedEvent)clickEvent.GetValue(mi);

                    PropertyInfo pi = t.GetProperty("EventHandlersStore", BindingFlags.Instance | BindingFlags.NonPublic);
                    var ehs = pi.GetValue(mi);
                    if (ehs != null)
                    {
                        var delegates = (RoutedEventHandlerInfo[])ehs.GetType().GetMethod("GetRoutedEventHandlers")?.Invoke(ehs, new object[] { MenuItem.ClickEvent });
#endif
                        if (delegates != null)
                        {
                            if (delegates.Length > 0)
                            {
                                var dele = delegates[0].Handler;
                                var handler = (RoutedEventHandler)delegates[0].Handler;
                                return new RoutedEventKeyAction(handler, mi.Name, mi);
                            }
                            else
                            {
                                throw new MissingMethodException("This menu item has no click event handlers.");
                            }
                        }
                        else
                        {
                            throw new MissingMethodException("This menu item has no click event handlers.");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Could not enter into the menu item to determine any click event handlers it had. Please contact the library creator if you see this error.");
                    }
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException("Could not enter into the menu item to determine any click event handlers it had. Please contact the library creator if you see this error.", ex);
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(mi));
            }
        }
#endif
    }
}
