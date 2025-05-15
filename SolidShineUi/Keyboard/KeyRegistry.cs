using System;
using System.Collections.Generic;
using System.Linq;
#if AVALONIA
using Avalonia.Input;
#else
using System.Windows.Input;
#endif
using static SolidShineUi.KeyboardShortcuts.KeyboardShortcut;

namespace SolidShineUi.KeyboardShortcuts
{
    /// <summary>
    /// The generic event arguments for keyboard shortcut related events in the KeyRegistry.
    /// </summary>
    public class KeyboardShortcutEventArgs
    {
        /// <summary>
        /// The keyboard shortcut referenced in the event.
        /// </summary>
        public KeyboardShortcut KeyboardShortcut { get; private set; }

        /// <summary>
        /// Create a KeyboardShortcutEventArgs.
        /// </summary>
        /// <param name="shortcut">The keyboard shortcut referenced in the event.</param>
        public KeyboardShortcutEventArgs(KeyboardShortcut shortcut)
        {
            KeyboardShortcut = shortcut;
        }
    }


    /// <summary>
    /// A delegate to be used with events regarding keyboard shortcuts.
    /// </summary>
    /// <param name="sender">The object where the event was raised.</param>
    /// <param name="e">The event arguments associated with this event.</param>
    public delegate void KeyboardShortcutEventHandler(object sender, KeyboardShortcutEventArgs e);

    /// <summary>
    /// A class to hold and handle keyboard shortcuts, and determine which shortcut to use based upon key presses.
    /// </summary>
    public class KeyRegistry
    {

#if NETCOREAPP || AVALONIA
        /// <summary>
        /// Raised when a shortcut is added (registered) to this KeyRegistry.
        /// </summary>
        public event KeyboardShortcutEventHandler? ShortcutRegistered;
        /// <summary>
        /// Raised when a shortcut is removed (unregistered) from this KeyRegistry.
        /// </summary>
        public event KeyboardShortcutEventHandler? ShortcutUnregistered;
#else
        /// <summary>
        /// Raised when a shortcut is added (registered) to this KeyRegistry.
        /// </summary>
        public event KeyboardShortcutEventHandler ShortcutRegistered;
        /// <summary>
        /// Raised when a shortcut is removed (unregistered) from this KeyRegistry.
        /// </summary>
        public event KeyboardShortcutEventHandler ShortcutUnregistered;
#endif

        private List<KeyboardShortcut> Ksr_All = new List<KeyboardShortcut>();

        /// <summary>
        /// A read-only list of all keyboard shortcuts registed in this registry. Use the RegisteryKeyShortcut and UnregisterKeyShortcut methods to add or remove shortcuts.
        /// </summary>
        public IReadOnlyList<KeyboardShortcut> RegisteredShortcuts { get => Ksr_All.AsReadOnly(); }

        private Dictionary<Key, KeyboardShortcut> Ksr_None = new Dictionary<Key, KeyboardShortcut>();
        private Dictionary<Key, KeyboardShortcut> Ksr_Ctrl = new Dictionary<Key, KeyboardShortcut>();
        private Dictionary<Key, KeyboardShortcut> Ksr_Alt = new Dictionary<Key, KeyboardShortcut>();
        private Dictionary<Key, KeyboardShortcut> Ksr_Shift = new Dictionary<Key, KeyboardShortcut>();
        private Dictionary<Key, KeyboardShortcut> Ksr_AltShift = new Dictionary<Key, KeyboardShortcut>();
        private Dictionary<Key, KeyboardShortcut> Ksr_CtrlAlt = new Dictionary<Key, KeyboardShortcut>();
        private Dictionary<Key, KeyboardShortcut> Ksr_CtrlShift = new Dictionary<Key, KeyboardShortcut>();
        private Dictionary<Key, KeyboardShortcut> Ksr_CtrlAltShift = new Dictionary<Key, KeyboardShortcut>();

        /// <summary>
        /// Get or set if shortcuts can be created and registered using all possible keys while certain modifiers aren't pressed. Default is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// This is meant to prevent users from activating keyboard shortcuts while they are typing text (for example, if a shortcut is registered
        /// for pressing just the "G" key, and then a user types the word "egg" into a text box, that keyboard shortcut may unintentionally get
        /// activated twice). This should be kept as <c>false</c> in programs that heavily feature or rely upon text input, but can be allowed
        /// (by changing this to <c>true</c>) in other programs that have little to no text input.
        /// <para/>
        /// While this is <c>false</c>, keyboard shortcuts that have no modifier, only the Shift modifier, or only the Alt modifier and that doesn't have one of the
        /// preallowed safe keys listed in <see cref="UnmodifiedSafeKeys"/>, then the shortcut is silently not added. (Shift and Alt is included here too
        /// to prevent shortcuts while a user is holding down the Shift key to capitalize a letter, or the Alt key to enter an Alt key code).
        /// </remarks>
        public bool AllowAllNonModifiedKeysInShortcuts { get; set; } = false;

        /// <summary>
        /// Register a keyboard shortcut with the registry.
        /// </summary>
        /// <param name="kc">The keyboard shortcut to register.</param>
        public void RegisterKeyShortcut(KeyboardShortcut kc)
        {
            if (Ksr_All.Any(ks => ks.KeyString == kc.KeyString))
            {
                throw new ArgumentException("Cannot add this shortcut as an existing shortcut already uses this keyboard combination and key. " +
                    "Unregister the existing shortcut first before registering this one.", nameof(kc));
            }

            Ksr_All.Add(kc);

            switch (kc.Combination)
            {
                case KeyboardCombination.None:
                    if (UnmodifiedSafeKeys.Contains(kc.Key) || AllowAllNonModifiedKeysInShortcuts)
                    {
                        Ksr_None.Add(kc.Key, kc);
                    }
                    else
                    {
                        return;
                    }
                    break;
                case KeyboardCombination.Ctrl:
                    Ksr_Ctrl.Add(kc.Key, kc);
                    break;
                case KeyboardCombination.Alt:
                    if (UnmodifiedSafeKeys.Contains(kc.Key) || AllowAllNonModifiedKeysInShortcuts)
                    {
                        Ksr_Alt.Add(kc.Key, kc);
                    }
                    else
                    {
                        return;
                    }
                    break;
                case KeyboardCombination.Shift:
                    if (UnmodifiedSafeKeys.Contains(kc.Key) || AllowAllNonModifiedKeysInShortcuts)
                    {
                        Ksr_Shift.Add(kc.Key, kc);
                    }
                    else
                    {
                        return;
                    }
                    break;
                case KeyboardCombination.AltShift:
                    Ksr_AltShift.Add(kc.Key, kc);
                    break;
                case KeyboardCombination.CtrlAlt:
                    Ksr_CtrlAlt.Add(kc.Key, kc);
                    break;
                case KeyboardCombination.CtrlShift:
                    Ksr_CtrlShift.Add(kc.Key, kc);
                    break;
                case KeyboardCombination.CtrlAltShift:
                    Ksr_CtrlAltShift.Add(kc.Key, kc);
                    break;
            }

            ShortcutRegistered?.Invoke(this, new KeyboardShortcutEventArgs(kc));
        }

        /// <summary>
        /// Register a keyboard shortcut with the registry.
        /// </summary>
        /// <param name="combination">The combination of modifier keys to press for this shortcut.</param>
        /// <param name="key">The specific key that activates this keyboard shortcut.</param>
        /// <param name="action">The action to perform when the keyboard shortcut is pressed.</param>
        public void RegisterKeyShortcut(KeyboardCombination combination, Key key, IKeyAction action)
        {
            KeyboardShortcut kc = new KeyboardShortcut(combination, key, action);
            RegisterKeyShortcut(kc);
        }

        /// <summary>
        /// Unregister (remove) a shortcut from this registry.
        /// </summary>
        /// <param name="ks">The keyboard shortcut to unregister.</param>
        /// <remarks>This overload of this method looks for a specific keyboard shortcut instance that is currently in the registry.
        /// To remove based upon the keys being pressed, use the other overload instead (<see cref="UnregisterKeyShortcut(KeyboardCombination, Key)"/>).</remarks>
        /// <returns>Returns if the shortcut was able to be removed or not. Will return false if this registry does not contain this specific shortcut.</returns>
        public bool UnregisterKeyShortcut(KeyboardShortcut ks)
        {
            bool success = true;

            if (Ksr_All.Contains(ks))
            {
                if (ks != null)
                {
                    Ksr_All.Remove(ks);
                }
                else
                {
                    return false;
                }

                Key key = ks.Key;

                switch (ks.Combination)
                {
                    case KeyboardCombination.None:
                        try
                        {
                            return Ksr_None.Remove(key);
                        }
                        catch (ArgumentNullException)
                        {
                            success = false;
                        }
                        break;
                    case KeyboardCombination.Ctrl:
                        try
                        {
                            return Ksr_Ctrl.Remove(key);
                        }
                        catch (ArgumentNullException)
                        {
                            success = false;
                        }
                        break;
                    case KeyboardCombination.Alt:
                        try
                        {
                            return Ksr_Alt.Remove(key);
                        }
                        catch (ArgumentNullException)
                        {
                            success = false;
                        }
                        break;
                    case KeyboardCombination.Shift:
                        try
                        {
                            return Ksr_Shift.Remove(key);
                        }
                        catch (ArgumentNullException)
                        {
                            success = false;
                        }
                        break;
                    case KeyboardCombination.AltShift:
                        try
                        {
                            return Ksr_AltShift.Remove(key);
                        }
                        catch (ArgumentNullException)
                        {
                            success = false;
                        }
                        break;
                    case KeyboardCombination.CtrlAlt:
                        try
                        {
                            return Ksr_CtrlAlt.Remove(key);
                        }
                        catch (ArgumentNullException)
                        {
                            success = false;
                        }
                        break;
                    case KeyboardCombination.CtrlShift:
                        try
                        {
                            return Ksr_CtrlShift.Remove(key);
                        }
                        catch (ArgumentNullException)
                        {
                            success = false;
                        }
                        break;
                    case KeyboardCombination.CtrlAltShift:
                        try
                        {
                            return Ksr_CtrlAltShift.Remove(key);
                        }
                        catch (ArgumentNullException)
                        {
                            success = false;
                        }
                        break;
                }

                ShortcutUnregistered?.Invoke(this, new KeyboardShortcutEventArgs(ks));

                return success;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Unregister (remove) a shortcut from this registry.
        /// </summary>
        /// <param name="combination">The combination of modifier keys to be pressed for this shortcut.</param>
        /// <param name="key">The key to press to activate this shortcut.</param>
        /// <returns>True if the shortcut was found and removed; false if there is no shortcut with this particular key and modifier key combination.</returns>
        public bool UnregisterKeyShortcut(KeyboardCombination combination, Key key)
        {
#if NETCOREAPP || AVALONIA
            KeyboardShortcut? ks = (from KeyboardShortcut kc in Ksr_All
                                   where kc.Key == key
                                   where kc.Combination == combination
                                   select kc).First() ?? null;
#else
            KeyboardShortcut ks = (from KeyboardShortcut kc in Ksr_All
                                    where kc.Key == key
                                    where kc.Combination == combination
                                    select kc).First() ?? null;
#endif

            if (ks != null)
            {
                return UnregisterKeyShortcut(ks);
            }
            else
            {
                return false;
            }
        }

        ///// <summary>
        ///// Set if menu items should display the keyboard shortcut combinations directly in the user interface.
        ///// </summary>
        ///// <param name="display">True to display keyboard shortcut combinations, false to not have them displayed.</param>
        //public void ApplyDisplaySettings(bool display)
        //{
        //    foreach (KeyboardShortcut item in Ksr_All)
        //    {
        //        item.ApplyKeyDisplaySettings(display);
        //    }
        //}

#if AVALONIA
        /// <summary>
        /// Get a list of keyboard shortcuts registered to a certain method.
        /// </summary>
        /// <param name="methodId">The name of the method. If you used <see cref="RoutedEventKeyAction.CreateListFromMenu(Avalonia.Controls.Menu)"/> to fill from a menu, 
        /// the name will be the name of the MenuItem itself.</param>
        public IEnumerable<KeyboardShortcut> GetShortcutsForMethod(string methodId)
#else
        /// <summary>
        /// Get a list of keyboard shortcuts registered to a certain method.
        /// </summary>
        /// <param name="methodId">The name of the method. If you used <see cref="RoutedEventKeyAction.CreateListFromMenu(Menu)"/> to fill from a menu, 
        /// the name will be the name of the MenuItem itself.</param>
        public IEnumerable<KeyboardShortcut> GetShortcutsForMethod(string methodId)
#endif
        {
            return from KeyboardShortcut kc in Ksr_All where kc.MethodId == methodId select kc;
        }

        /// <summary>
        /// Get the IKeyAction associated with a certain keyboard shortcut.
        /// </summary>
        /// <param name="key">The key for this shortcut.</param>
        /// <param name="combination">The combination of modifier keys to have pressed for this shortcut.</param>
        /// <returns>A Tuple containing the IKeyAction associated with this shortcut, if there is one (null if there is not), 
        /// and a friendly string that displays the keyboard combination to press (i.e. "Ctrl+Alt+V").</returns>
#if NETCOREAPP || AVALONIA
        public (IKeyAction?, string) GetActionForKey(KeyboardCombination combination, Key key)
#else
        public (IKeyAction, string) GetActionForKey(KeyboardCombination combination, Key key)
#endif
        {
            bool shift = false;
            bool alt = false;
            bool ctrl = false;

            switch (combination)
            {
                case KeyboardCombination.None:
                    break;
                case KeyboardCombination.Ctrl:
                    ctrl = true;
                    break;
                case KeyboardCombination.Alt:
                    alt = true;
                    break;
                case KeyboardCombination.Shift:
                    shift = true;
                    break;
                case KeyboardCombination.AltShift:
                    alt = true;
                    shift = true;
                    break;
                case KeyboardCombination.CtrlAlt:
                    ctrl = true;
                    alt = true;
                    break;
                case KeyboardCombination.CtrlShift:
                    ctrl = true;
                    shift = true;
                    break;
                case KeyboardCombination.CtrlAltShift:
                    ctrl = true;
                    shift = true;
                    alt = true;
                    break;
                default:
                    break;
            }

            return GetActionForKey(key, shift, alt, ctrl);
        }

        /// <summary>
        /// Get the IKeyAction associated with a certain keyboard shortcut.
        /// </summary>
        /// <param name="key">The key for this shortcut.</param>
        /// <param name="shift">Set if the Shift key is part of this shortcut.</param>
        /// <param name="alt">Set if the Alt key is part of this shortcut.</param>
        /// <param name="ctrl">Set if the Ctrl key is part of this shortcut.</param>
        /// <returns>A Tuple containing the IKeyAction associated with this shortcut, if there is one (null if there is not), 
        /// and a friendly string that displays the keyboard combination to press (i.e. "Ctrl+Alt+V").</returns>
#if NETCOREAPP || AVALONIA
        public (IKeyAction?, string) GetActionForKey(Key key, bool shift, bool alt, bool ctrl)
#else
        public (IKeyAction, string) GetActionForKey(Key key, bool shift, bool alt, bool ctrl)
#endif
        {
            if (ctrl)
            {
                if (shift)
                {
                    if (alt)
                    {
                        // Ctrl + Shift + Alt + whatever
                        if (Ksr_CtrlAltShift.TryGetValue(key, out var value1))
                        {
                            return (value1.Action, "Ctrl + Shift + Alt + " + key);
                        }
                        else
                        {
                            return (null, "Ctrl + Shift + Alt + " + key);
                        }
                    }

                    // Ctrl + Shift + whatever
                    if (Ksr_CtrlShift.TryGetValue(key, out var value2))
                    {
                        return (value2.Action, "Ctrl + Shift + " + key);
                    }
                    else
                    {
                        return (null, "Ctrl + Shift + " + key);
                    }
                }

                if (alt)
                {
                    // Ctrl + Alt + whatever
                    if (Ksr_CtrlAlt.TryGetValue(key, out var value3))
                    {
                        return (value3.Action, "Ctrl + Alt + " + key);
                    }
                    else
                    {
                        return (null, "Ctrl + Alt + " + key);
                    }
                }

                // Ctrl + whatever
                if (Ksr_Ctrl.TryGetValue(key, out var value4))
                {
                    return (value4.Action, "Ctrl + " + key);
                }
                else
                {
                    return (null, "Ctrl + " + key);
                }
            }

            if (alt)
            {
                if (shift)
                {
                    // Alt + Shift + whatever
                    if (Ksr_AltShift.TryGetValue(key, out var value5))
                    {
                        return (value5.Action, "Shift + Alt + " + key);
                    }
                    else
                    {
                        return (null, "Shift + Alt + " + key);
                    }
                }

                // Alt + whatever
                if (Ksr_Alt.TryGetValue(key, out var value6))
                {
                    return (value6.Action, "Alt + " + key);
                }
                else
                {
                    return (null, "Alt + " + key);
                }
            }

            if (shift)
            {
                // Shift + whatever
                // (Note: only some keys are allowed for Shift + key shortcuts)
                if (Ksr_Shift.TryGetValue(key, out var value7))
                {
                    return (value7.Action, "Shift + " + key);
                }
                else
                {
                    return (null, "Ctrl + Shift + " + key);
                }
            }

            // finally, just keys with no modifiers
            // (Note: only some keys are allowed for unmodified shortcuts)
            if (Ksr_None.TryGetValue(key, out var value8))
            {
                return (value8.Action, key.ToString());
            }
            else
            {
                return (null, key.ToString());
            }
        }
    }

    /// <summary>
    /// A key registry that has a master key registry that it syncs up with. Useful in scenarios where you want to have multiple layers of shortcuts, 
    /// with only some layers being applicable at certain points.
    /// </summary>
    public class SubKeyRegistry : KeyRegistry
    {
        private KeyRegistry masterRegistry = new KeyRegistry();

        /// <summary>
        /// Create a SubKeyRegistry.
        /// </summary>
        /// <param name="master">The master KeyRegistry to stay in sync with.</param>
        public SubKeyRegistry(KeyRegistry master)
        {
            masterRegistry = master;
            //GetMethodFunction = getMethodFunc;
            //GetMenuFunction = getMenuFunc;

            master.ShortcutRegistered += Master_ShortcutRegistered;
            master.ShortcutUnregistered += Master_ShortcutUnregistered;

            RegisterExistingShortcuts();
        }

        /// <summary>
        /// Register all shortcuts into this SubKeyRegistry that are currently in the master KeyRegistry.
        /// </summary>
        private void RegisterExistingShortcuts()
        {
            foreach (KeyboardShortcut kc in masterRegistry.RegisteredShortcuts)
            {
                try
                {
                    RegisterKeyShortcut(kc);
                    //RegisterKeyShortcut(kc.Combination, kc.Key, kc.Action);
                }
                catch (ArgumentOutOfRangeException)
                {

                }
            }
        }

        //public Func<string, RoutedEventHandler> GetMethodFunction { get; private set; }
        //public Func<string, MenuItem> GetMenuFunction { get; private set; }

        private void Master_ShortcutRegistered(object sender, KeyboardShortcutEventArgs e)
        {
            KeyboardShortcut kc = e.KeyboardShortcut;

            try
            {
                RegisterKeyShortcut(kc);
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void Master_ShortcutUnregistered(object sender, KeyboardShortcutEventArgs e)
        {
            UnregisterKeyShortcut(e.KeyboardShortcut);
        }

        //public new void ApplyDisplaySettings(bool display)
        //{
        //    foreach (KeyboardShortcut item in Ksr_All)
        //    {
        //        try
        //        {
        //            MenuItem mi = GetMenuFunction(item.MethodId);

        //            if (mi != null)
        //            {
        //                if (display)
        //                {
        //                    mi.ToolTip = null;
        //                    mi.InputGestureText = item.KeyString;
        //                }
        //                else
        //                {
        //                    mi.InputGestureText = "";
        //                    ToolTip tt = new ToolTip();
        //                    tt.Content = item.KeyString;
        //                    mi.ToolTip = tt;
        //                }
        //            }
        //        }
        //        catch (ArgumentOutOfRangeException)
        //        {

        //        }
        //    }
        //}
    }
}
