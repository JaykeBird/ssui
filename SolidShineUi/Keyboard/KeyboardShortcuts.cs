using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace SolidShineUi.Keyboard
{
    public enum KeyboardCombination
    {
        None = 0,
        Ctrl = 1,
        Alt = 2,
        Shift = 3,
        AltShift = 4,
        CtrlAlt = 5,
        CtrlShift = 6,
        CtrlAltShift = 7,
    }

    //public static class KeyboardConstants
    //{

    //    public static Key[] SafeKeys = { Key.F1, Key.F2, Key.F3, Key.F4, Key.F5, Key.F6, Key.F7, Key.F8, Key.F9, Key.F10, Key.F11, Key.F12, Key.F13,
    //        Key.F14, Key.F15, Key.F16, Key.F17, Key.F18, Key.F19, Key.F20, Key.F21, Key.F22, Key.F23, Key.F24, Key.Delete, Key.Home, Key.PageDown,
    //        Key.PageUp, Key.End, Key.Insert, Key.Next, Key.Prior, Key.BrowserBack, Key.BrowserFavorites, Key.BrowserForward, Key.BrowserHome,
    //        Key.BrowserRefresh, Key.BrowserSearch, Key.BrowserStop, Key.MediaNextTrack, Key.MediaPlayPause, Key.MediaPreviousTrack, Key.MediaStop,
    //        Key.Pause, Key.Play, Key.Print, Key.Help, Key.LaunchApplication1, Key.LaunchApplication2, Key.LaunchMail, Key.Zoom, };

    //}

    //public class KeyEntry
    //{
    //    public KeyboardCombination KeyboardCombination { get; set; } = KeyboardCombination.None;
    //    public Key Key { get; set; } = Key.None;
    //    public string MethodId { get; set; } = "";

    //    public KeyEntry()
    //    {

    //    }

    //    public KeyEntry(KeyboardCombination combination, Key key, string methodId)
    //    {
    //        KeyboardCombination = combination;
    //        Key = key;
    //        MethodId = methodId;
    //    }
    //}

    public static class KeyboardShortcutsIo
    {
        public static List<KeyboardShortcut> LoadFromFile(string file, KeyActionList list)
        {
            if (!File.Exists(file))
            {
                throw new ArgumentException("The specified file does not exist. Keyboard shortcuts cannot be loaded.");
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
                    string? m = xr.GetAttribute("methodid");
#else
                    string c = xr.GetAttribute("comb");
                    string k = xr.GetAttribute("keyid");
                    string m = xr.GetAttribute("methodid");
#endif

                    if (string.IsNullOrEmpty(c) || string.IsNullOrEmpty(k) || string.IsNullOrEmpty(m))
                    {
                        // skip item
                    }
                    else if (Enum.TryParse(c, out KeyboardCombination kc))
                    {
                        if (int.TryParse(k, out int kv))
                        {
#if NETCOREAPP
                            IKeyAction? a = null;
#else
                            IKeyAction a = null;
#endif

                            if (list != null)
                            {
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
                            if (Enum.TryParse(k, out Key kz))
                            {
#if NETCOREAPP
                                IKeyAction? a = null;
#else
                                IKeyAction a = null;
#endif

                                if (list != null)
                                {
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

        public static async Task<string> WriteToFile(KeyRegistry registry, string file)
        {
            XmlWriter w = XmlWriter.Create(file, new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Async = true,
                Indent = true
            });

            await w.WriteStartDocumentAsync();

            await w.WriteStartElementAsync("", "shortcuts", "");

            foreach (KeyboardShortcut item in registry.Ksr_All)
            {
                await w.WriteStartElementAsync("", "ks", "");
                await w.WriteAttributeStringAsync("", "comb", "", item.Combination.ToString("f"));
                await w.WriteAttributeStringAsync("", "keyid", "", item.Key.ToString("d"));
                await w.WriteAttributeStringAsync("", "methodid", "", item.MethodId);
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
        /// <param name="method">The method to run when this shortcut is used.</param>
        /// <param name="methodId">The ID string/name for this method.</param>
        /// <param name="menuItem">The menu item corresponding with the method.</param>
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
        /// Generate the string to display to the user, so they know which keys to press to activate the shortcut. 
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

        public IKeyAction Action { get; private set; }
        public string MethodId { get; set; } = "";

#if NETCOREAPP
        public UIElement? SourceElement { get; set; } = null;
#else
        public UIElement SourceElement { get; set; } = null;
#endif
    }
}
