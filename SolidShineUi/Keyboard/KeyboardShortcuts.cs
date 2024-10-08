﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace SolidShineUi.KeyboardShortcuts
{
    /// <summary>
    /// A list of all combinations of modifier keys being pressed.
    /// </summary>
    public enum KeyboardCombination
    {
        /// <summary>
        /// No modifier keys pressed.
        /// </summary>
        None = 0,
        /// <summary>
        /// Ctrl modifier key being pressed.
        /// </summary>
        Ctrl = 1,
        /// <summary>
        /// Alt modifier key being pressed.
        /// </summary>
        Alt = 2,
        /// <summary>
        /// Shift modifier key being pressed.
        /// </summary>
        Shift = 3,
        /// <summary>
        /// Alt and Shift modifier keys being pressed.
        /// </summary>
        AltShift = 4,
        /// <summary>
        /// Ctrl and Alt modifier keys being pressed.
        /// </summary>
        CtrlAlt = 5,
        /// <summary>
        /// Ctrl and Shift modifier keys being pressed.
        /// </summary>
        CtrlShift = 6,
        /// <summary>
        /// Ctrl, Shift, and Alt modifier keys being pressed.
        /// </summary>
        CtrlAltShift = 7,
    }

    /// <summary>
    /// A helper class to save or load keyboard shortcuts into files.
    /// </summary>
    public static class KeyboardShortcutsIo
    {
        /// <summary>
        /// Load a list of keyboard shortcuts from a file. The list of available actions is also needed to map the shortcuts to these actions.
        /// </summary>
        /// <param name="file">The file to load from.</param>
        /// <param name="list">A list of actions available to be accessed via keyboard shortcuts.</param>
        /// <returns>A list of keyboard shortcuts, which can be loaded into a KeyRegistry.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
        /// <remarks>If there is a shortcut in this file that reference an action ID that isn't on the <paramref name="list"/>, then that shortcut is skipped.</remarks>
        public static List<KeyboardShortcut> LoadFromFile(string file, KeyActionList list)
        {
            if (!File.Exists(file))
            {
                throw new FileNotFoundException("The specified file does not exist. Keyboard shortcuts cannot be loaded.", file);
            }

            List<KeyboardShortcut> entries = new List<KeyboardShortcut>();

            XmlReader xr = XmlReader.Create(file);

            xr.MoveToContent();

            while (xr.Read())
            {
                if (xr.NodeType == XmlNodeType.Element)
                {
                    //xr.ReadStartElement("ks");

#if NETCOREAPP
                    string? c = xr.GetAttribute("comb");
                    string? k = xr.GetAttribute("keyid");
                    string? m = xr.GetAttribute("action");
#else
                    string c = xr.GetAttribute("comb");
                    string k = xr.GetAttribute("keyid");
                    string m = xr.GetAttribute("action");
#endif
                    if (string.IsNullOrEmpty(m))
                    {
                        m = xr.GetAttribute("methodid");
                    }

                    if (string.IsNullOrEmpty(c) || string.IsNullOrEmpty(k) || string.IsNullOrEmpty(m))
                    {
                        // skip item
                    }
                    else if (Enum.TryParse(c, out KeyboardCombination kc))
                    {
                        // try to load keys by their internal integer value
                        // from https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.key?view=net-5.0 (the middle column)
                        if (int.TryParse(k, out int kv))
                        {
#if NETCOREAPP
                            IKeyAction? a = null;
#else
                            IKeyAction a = null;
#endif

                            if (list != null)
                            {
                                // checks the key action list for "Xxx", "mnuXxx", and "btnXxx"
                                // this is a carryover from internal usage, but I figure it should still be helpful
                                // if desired, I can remove this or make a toggle for it in a future release
                                if (list.ContainsId(m))
                                {
                                    a = list[m];
                                }
                                else if (list.ContainsId("mnu" + m))
                                {
                                    m = "mnu" + m;

                                    a = list[m];
                                }
                                else if (list.ContainsId("btn" + m))
                                {
                                    m = "btn" + m;

                                    a = list[m];
                                }
                            }

                            if (a != null)
                            {
                                KeyboardShortcut ke = new KeyboardShortcut(kc, (Key)kv, a);
                                entries.Add(ke);
                            }
                        }
                        else
                        {
                            // if the integer-based value doesn't work, try to parse based upon the name of the Key in the enum
                            // again from https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.key?view=net-5.0 (the left column)
                            if (Enum.TryParse(k, out Key kz))
                            {
#if NETCOREAPP
                                IKeyAction? a = null;
#else
                                IKeyAction a = null;
#endif

                                if (list != null)
                                {
                                    // checks the key action list for "Xxx", "mnuXxx", and "btnXxx"
                                    // this is a carryover from internal usage, but I figure it should still be helpful
                                    // if desired, I can remove this or make a toggle for it in a future release
                                    if (list.ContainsId(m))
                                    {
                                        a = list[m];
                                    }
                                    else if (list.ContainsId("mnu" + m))
                                    {
                                        m = "mnu" + m;

                                        a = list[m];
                                    }
                                    else if (list.ContainsId("btn" + m))
                                    {
                                        m = "btn" + m;

                                        a = list[m];
                                    }
                                }

                                if (a != null)
                                {
                                    KeyboardShortcut ke = new KeyboardShortcut(kc, kz, a);
                                    entries.Add(ke);
                                }
                            }
                        }
                    }

                    //xr.ReadEndElement();
                }

            }

            xr.Close();

            return entries;
        }

        /// <summary>
        /// Write a list of keyboard shortcuts to a file.
        /// </summary>
        /// <param name="registry">They keyboard shortcut registry to read from.</param>
        /// <param name="file">The file to write to.</param>
        /// <returns>The filename of the file written to. This should match the filename passed in.</returns>
        public static string WriteToFile(KeyRegistry registry, string file)
        {
            XmlWriter w = XmlWriter.Create(file, new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = true
            });

            w.WriteStartDocument();

            w.WriteStartElement("", "shortcuts", "");

            foreach (KeyboardShortcut item in registry.RegisteredShortcuts)
            {
                w.WriteStartElement("", "ks", "");
                w.WriteAttributeString("", "comb", "", item.Combination.ToString("f"));
                w.WriteAttributeString("", "keyid", "", item.Key.ToString("d"));
                w.WriteAttributeString("", "action", "", item.MethodId);
                w.WriteEndElement();
            }

            w.WriteEndElement();
            w.WriteEndDocument();

            w.Flush();
            w.Close();

            return file;
        }

        /// <summary>
        /// Write a list of keyboard shortcuts to a file.
        /// </summary>
        /// <param name="registry">They keyboard shortcut registry to read from.</param>
        /// <param name="file">The file to write to.</param>
        /// <returns>The filename of the file written to. This should match the filename passed in.</returns>
        public static async Task<string> WriteToFileAsync(KeyRegistry registry, string file)
        {
            XmlWriter w = XmlWriter.Create(file, new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Async = true,
                Indent = true
            });

            await w.WriteStartDocumentAsync();

            await w.WriteStartElementAsync("", "shortcuts", "");

            foreach (KeyboardShortcut item in registry.RegisteredShortcuts)
            {
                await w.WriteStartElementAsync("", "ks", "");
                await w.WriteAttributeStringAsync("", "comb", "", item.Combination.ToString("f"));
                await w.WriteAttributeStringAsync("", "keyid", "", item.Key.ToString("d"));
                await w.WriteAttributeStringAsync("", "action", "", item.MethodId);
                await w.WriteEndElementAsync();
            }

            await w.WriteEndElementAsync();
            await w.WriteEndDocumentAsync();

            await w.FlushAsync();
            w.Close();

            return file;
        }
    }

    /// <summary>
    /// Represents a single keyboard shortcut entry.
    /// </summary>
    public class KeyboardShortcut
    {
        /// <summary>
        /// Get a list of keys that are safe to use for keyboard shortcuts without having Ctrl or Alt pressed.
        /// Using keys other than these in a program that has text input may end up with conflicts or unintended issues as users try to type.
        /// </summary>
        public static readonly Key[] UnmodifiedSafeKeys = { Key.F1, Key.F2, Key.F3, Key.F4, Key.F5, Key.F6, Key.F7, Key.F8, Key.F9, Key.F10, Key.F11, Key.F12, Key.F13,
            Key.F14, Key.F15, Key.F16, Key.F17, Key.F18, Key.F19, Key.F20, Key.F21, Key.F22, Key.F23, Key.F24, Key.Delete, Key.Home, Key.PageDown,
            Key.PageUp, Key.End, Key.Insert, Key.Next, Key.Prior, Key.BrowserBack, Key.BrowserFavorites, Key.BrowserForward, Key.BrowserHome,
            Key.BrowserRefresh, Key.BrowserSearch, Key.BrowserStop, Key.MediaNextTrack, Key.MediaPlayPause, Key.MediaPreviousTrack, Key.MediaStop,
            Key.Pause, Key.Play, Key.Print, Key.Help, Key.LaunchApplication1, Key.LaunchApplication2, Key.LaunchMail, Key.Zoom, };

        //public KeyboardShortcut() { }

        /// <summary>
        /// Create a keyboard shortcut entry.
        /// </summary>
        /// <param name="combination">The key combination needed, if any, for activating this shortcut.</param>
        /// <param name="key">The specific key that corresponds to this shortcut.</param>
        /// <param name="action">The action to perform when this shortcut is activated.</param>
        public KeyboardShortcut(KeyboardCombination combination, Key key, IKeyAction action)
        {
            Combination = combination;
            Key = key;

            Action = action;
            MethodId = action.ID;

            GenerateKeyString();

            SourceElement = action.SourceElement;
        }

        ///// <summary>
        ///// Update the display settings for the menu item corresponding with this keyboard shortcut.
        ///// </summary>
        ///// <param name="display">Set whether the shortcut should always be displayed on the right-side of the menu item</param>
        //public void ApplyKeyDisplaySettings(bool display)
        //{
        //    if (MethodHolder != null)
        //    {
        //        if (display)
        //        {
        //            MethodHolder.ToolTip = null;
        //            MethodHolder.InputGestureText = KeyString;
        //        }
        //        else
        //        {
        //            MethodHolder.InputGestureText = "";
        //            System.Windows.Controls.ToolTip tt = new System.Windows.Controls.ToolTip();
        //            tt.Content = KeyString;
        //            MethodHolder.ToolTip = tt;
        //        }
        //    }
        //}

        /// <summary>
        /// The control key combination needed before activating this shortcut. This shortcut will not activate unless these keys are also held down.
        /// </summary>
        public KeyboardCombination Combination { get; private set; } = KeyboardCombination.None;
        /// <summary>
        /// The specific key that, when pressed, triggers the shortcut.
        /// </summary>
        public Key Key { get; private set; } = Key.None;

        /// <summary>
        /// The string to display to the user, so they know which keys to press to activate the shortcut.
        /// </summary>
        public string KeyString { get; private set; } = "";

        /// <summary>
        /// Generate the string to display to the user, so they know which keys to press to activate the shortcut, such as "Ctrl+G" or "F12".
        /// </summary>
        private void GenerateKeyString()
        {
            string keyBase = Key.ToString("G");

            // use switch to format key strings that look odd (i.e. number row keys being called "D1", "D2", etc.)
            switch (keyBase)
            {
                case "D0":
                case "D1":
                case "D2":
                case "D3":
                case "D4":
                case "D5":
                case "D6":
                case "D7":
                case "D8":
                case "D9":
                    keyBase = keyBase.Substring(1);
                    break;
                default:
                    break;
            }

            switch (Combination)
            {
                case KeyboardCombination.None:
                    KeyString = keyBase;
                    break;
                case KeyboardCombination.Ctrl:
                    KeyString = "Ctrl+" + keyBase;
                    break;
                case KeyboardCombination.Alt:
                    KeyString = "Alt+" + keyBase;
                    break;
                case KeyboardCombination.Shift:
                    KeyString = "Shift+" + keyBase;
                    break;
                case KeyboardCombination.AltShift:
                    KeyString = "Alt+Shift+" + keyBase;
                    break;
                case KeyboardCombination.CtrlAlt:
                    KeyString = "Ctrl+Alt+" + keyBase;
                    break;
                case KeyboardCombination.CtrlShift:
                    KeyString = "Ctrl+Shift+" + keyBase;
                    break;
                case KeyboardCombination.CtrlAltShift:
                    KeyString = "Ctrl+Alt+Shift+" + keyBase;
                    break;
            }
        }

        /// <summary>
        /// The action associated with this keyboard shortcut. When this shortcut is activated, this is the action that will occur.
        /// </summary>
        public IKeyAction Action { get; private set; }

        /// <summary>
        /// Get or set the ID associated with the action of the keyboard shortcut, which is used for mapping shortcuts to actions (for example while loading or saving files). This should match the ID in the Action property.
        /// </summary>
        public string MethodId { get; set; } = "";

        /// <summary>
        /// Get or set the element associated with this keyboard shortcut and action (such as a menu item if the action is associated with the command of a menu item). Having an element is not required.
        /// </summary>
#if NETCOREAPP
        public UIElement? SourceElement { get; set; } = null;
#else
        public UIElement SourceElement { get; set; } = null;
#endif
    }
}
