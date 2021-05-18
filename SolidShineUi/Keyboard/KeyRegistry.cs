using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static SolidShineUi.KeyboardShortcuts.KeyboardShortcut;
using System.Windows.Controls;

namespace SolidShineUi.KeyboardShortcuts
{
    public class KeyboardShortcutEventArgs
    {
        public KeyboardShortcut KeyboardShortcut { get; private set; }

        public KeyboardShortcutEventArgs(KeyboardShortcut shortcut)
        {
            KeyboardShortcut = shortcut;
        }
    }

    public class KeyRegistry
    {
        public delegate void KeyboardShortcutEventHandler(object sender, KeyboardShortcutEventArgs e);

#if NETCOREAPP
        public event KeyboardShortcutEventHandler? ShortcutRegistered;
        public event KeyboardShortcutEventHandler? ShortcutUnregistered;
#else
        public event KeyboardShortcutEventHandler ShortcutRegistered;
        public event KeyboardShortcutEventHandler ShortcutUnregistered;
#endif

        public List<KeyboardShortcut> Ksr_All = new List<KeyboardShortcut>();

        public Dictionary<Key, KeyboardShortcut> Ksr_None = new Dictionary<Key, KeyboardShortcut>();
        public Dictionary<Key, KeyboardShortcut> Ksr_Ctrl = new Dictionary<Key, KeyboardShortcut>();
        public Dictionary<Key, KeyboardShortcut> Ksr_Alt = new Dictionary<Key, KeyboardShortcut>();
        public Dictionary<Key, KeyboardShortcut> Ksr_Shift = new Dictionary<Key, KeyboardShortcut>();
        public Dictionary<Key, KeyboardShortcut> Ksr_AltShift = new Dictionary<Key, KeyboardShortcut>();
        public Dictionary<Key, KeyboardShortcut> Ksr_CtrlAlt = new Dictionary<Key, KeyboardShortcut>();
        public Dictionary<Key, KeyboardShortcut> Ksr_CtrlShift = new Dictionary<Key, KeyboardShortcut>();
        public Dictionary<Key, KeyboardShortcut> Ksr_CtrlAltShift = new Dictionary<Key, KeyboardShortcut>();

        public void RegisterKeyShortcut(KeyboardShortcut kc)
        {
            if (Ksr_All.Any(ks => ks.KeyString == kc.KeyString))
            {
                throw new ArgumentException("Cannot add this shortcut as an existing shortcut already uses this keyboard combination and key. Unregister the existing shortcut first before registering this one.", nameof(kc));
            }

            Ksr_All.Add(kc);

            switch (kc.Combination)
            {
                case KeyboardCombination.None:
                    if (UnmodifiedSafeKeys.Contains(kc.Key))
                    {
                        Ksr_None.Add(kc.Key, kc);
                    }
                    break;
                case KeyboardCombination.Ctrl:
                    Ksr_Ctrl.Add(kc.Key, kc);
                    break;
                case KeyboardCombination.Alt:
                    if (UnmodifiedSafeKeys.Contains(kc.Key))
                    {
                        Ksr_Alt.Add(kc.Key, kc);
                    }
                    break;
                case KeyboardCombination.Shift:
                    if (UnmodifiedSafeKeys.Contains(kc.Key))
                    {
                        Ksr_Shift.Add(kc.Key, kc);
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

        public void RegisterKeyShortcut(KeyboardCombination combination, Key key, IKeyAction action)
        {
            KeyboardShortcut kc = new KeyboardShortcut(combination, key, action);
            RegisterKeyShortcut(kc);
        }

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

        public bool UnregisterKeyShortcut(KeyboardCombination combination, Key key)
        {
#if NETCOREAPP
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

        /// <summary>
        /// Get a list of keyboard shortcuts registered to a certain method.
        /// </summary>
        /// <param name="methodId">The name of the method. If you used the RoutedMethodRegistry to fill from a menu, the name will be the name of the MenuItem itself.</param>
        /// <returns></returns>
        public IEnumerable<KeyboardShortcut> GetShortcutsForMethod(string methodId)
        {
            return from KeyboardShortcut kc in Ksr_All where kc.MethodId == methodId select kc;
        }

        /// <summary>
        /// Get the IKeyAction associated with a certain keyboard shortcut.
        /// </summary>
        /// <param name="key">The key for this shortcut.</param>
        /// <param name="combination">The combination of modifier keys to have pressed for this shortcut.</param>
        /// <returns>A Tuple containing the IKeyAction associated with this shortcut, if there is one (null if there is not), and a friendly string that displays the keyboard combination to press (i.e. "Ctrl+Alt+V").</returns>
#if NETCOREAPP
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
        /// <returns>A Tuple containing the IKeyAction associated with this shortcut, if there is one (null if there is not), and a friendly string that displays the keyboard combination to press (i.e. "Ctrl+Alt+V").</returns>
#if NETCOREAPP
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
                        if (Ksr_CtrlAltShift.ContainsKey(key))
                        {
                            return ((Ksr_CtrlAltShift[key]).Action, "Ctrl + Shift + Alt + " + key);
                        }
                        else
                        {
                            return (null, "Ctrl + Shift + Alt + " + key);
                        }
                    }

                    // Ctrl + Shift + whatever
                    if (Ksr_CtrlShift.ContainsKey(key))
                    {
                        return ((Ksr_CtrlShift[key]).Action, "Ctrl + Shift + " + key);
                    }
                    else
                    {
                        return (null, "Ctrl + Shift + " + key);
                    }
                }

                if (alt)
                {
                    // Ctrl + Alt + whatever
                    if (Ksr_CtrlAlt.ContainsKey(key))
                    {
                        return ((Ksr_CtrlAlt[key]).Action, "Ctrl + Alt + " + key);
                    }
                    else
                    {
                        return (null, "Ctrl + Alt + " + key);
                    }
                }

                // Ctrl + whatever
                if (Ksr_Ctrl.ContainsKey(key))
                {
                    return ((Ksr_Ctrl[key]).Action, "Ctrl + " + key);
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
                    if (Ksr_AltShift.ContainsKey(key))
                    {
                        return ((Ksr_AltShift[key]).Action, "Shift + Alt + " + key);
                    }
                    else
                    {
                        return (null, "Shift + Alt + " + key);
                    }
                }

                // Alt + whatever
                // (Note: only some keys are allowed for Alt + key shortcuts)
                if (Ksr_Alt.ContainsKey(key))
                {
                    return ((Ksr_Alt[key]).Action, "Alt + " + key);
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
                if (Ksr_Shift.ContainsKey(key))
                {
                    return ((Ksr_Shift[key]).Action, "Shift + " + key);
                }
                else
                {
                    return (null, "Ctrl + Shift + " + key);
                }
            }

            // finally, just keys with no modifiers
            // (Note: only some keys are allowed for unmodified shortcuts)
            if (Ksr_None.ContainsKey(key))
            {
                return ((Ksr_None[key]).Action, key.ToString());
            }
            else
            {
                return (null, key.ToString());
            }
        }
    }

    public class SubKeyRegistry : KeyRegistry
    {
        private KeyRegistry masterRegistry = new KeyRegistry();

        public SubKeyRegistry(KeyRegistry master, Func<string, RoutedEventHandler> getMethodFunc, Func<string, MenuItem> getMenuFunc)
        {
            masterRegistry = master;
            GetMethodFunction = getMethodFunc;
            GetMenuFunction = getMenuFunc;

            master.ShortcutRegistered += Master_ShortcutRegistered;
            master.ShortcutUnregistered += Master_ShortcutUnregistered;
        }

        public void RegisterExistingShortcuts()
        {
            foreach (KeyboardShortcut kc in masterRegistry.Ksr_All)
            {
                try
                {
                    RegisterKeyShortcut(kc.Combination, kc.Key, kc.Action);
                }
                catch (ArgumentOutOfRangeException)
                {

                }
            }
        }

        public Func<string, RoutedEventHandler> GetMethodFunction { get; private set; }
        public Func<string, MenuItem> GetMenuFunction { get; private set; }

        private void Master_ShortcutRegistered(object sender, KeyboardShortcutEventArgs e)
        {
            KeyboardShortcut kc = e.KeyboardShortcut;

            try
            {
                RegisterKeyShortcut(kc.Combination, kc.Key, kc.Action);
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void Master_ShortcutUnregistered(object sender, KeyboardShortcutEventArgs e)
        {
            UnregisterKeyShortcut(e.KeyboardShortcut.Combination, e.KeyboardShortcut.Key);
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
