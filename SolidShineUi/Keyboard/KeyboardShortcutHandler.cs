using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using Keyboard = System.Windows.Input.Keyboard;

namespace SolidShineUi.KeyboardShortcuts
{

    /// <summary>
    /// A helper class to manage keyboard shortcut support for a given WPF window. Connect this to a window to easily add and manage keyboard shortcuts.
    /// </summary>
    /// <remarks>
    /// This class listens to a Window's KeyDown and KeyUp events to detect when keyboard shortcuts are pressed and activate them. See online documentation for more info about keyboard shortcut support.
    /// </remarks>
    public class KeyboardShortcutHandler
    {
        /// <summary>
        /// Create a KeyboardShortcutHandler and connect it to a window.
        /// </summary>
        /// <param name="w">The window to connect to this KeyboardShortcutHandler.</param>
        public KeyboardShortcutHandler(Window w)
        {
            Window = w;

            w.KeyDown += window_KeyDown;
            w.KeyUp += window_KeyUp;

            keyCheck = new DispatcherTimer(new TimeSpan(400), DispatcherPriority.Input, KeyModifierCheck, w.Dispatcher);
        }

        /// <summary>
        /// Get the window that this KeyboardShortcutHandler is connected to. When keys are pressed in this window and <see cref="IsActive"/> is <c>true</c>, this will activate keyboard shortcuts.
        /// </summary>
        public Window Window { get; private set; }

        /// <summary>
        /// Get the KeyRegistry used by this KeyboardShortcutHandler. The KeyRegistry contains a list of all the shortcuts registered.
        /// </summary>
        public KeyRegistry KeyRegistry { get; } = new KeyRegistry();

        private DispatcherTimer keyCheck;

        bool CtrlPressed = false;
        bool ShiftPressed = false;
        bool AltPressed = false;

        /// <summary>
        /// Load in and register a list of keyboard shortcuts.
        /// </summary>
        /// <param name="ks">The list of shortcuts to load.</param>
        public void LoadShortcutsFromList(List<KeyboardShortcut> ks)
        {
            foreach (KeyboardShortcut item in ks)
            {
                KeyRegistry.RegisterKeyShortcut(item);
            }
        }

        /// <summary>
        /// Load in and register keyboard shortcuts from a file. A KeyActionList is needed to map the shortcuts to their actions.
        /// </summary>
        /// <param name="file">The file to load from.</param>
        /// <param name="methodList">The list of actions available for keyboard shortcuts, to use for mapping.</param>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the file cannot be found.</exception>
        /// <remarks>
        /// If there is a shortcut in the file that references an action not in the <paramref name="methodList"/>, that shortcut is skipped.
        /// Also, if a shortcut in the file has the same keyboard combination and key as a shortcut already registed, that shortcut is skipped.
        /// </remarks>
        public void LoadShortcutsFromFile(string file, KeyActionList methodList)
        {
            var list = KeyboardShortcutsIo.LoadFromFile(file, methodList);
            foreach (KeyboardShortcut item in list)
            {
                try
                {
                    KeyRegistry.RegisterKeyShortcut(item);
                }
                catch (ArgumentException)
                {
                    // thrown if there's already a shortcut registered with this particular keyboard combination and key
                    // skip the particular shortcut
                }
            }
        }

#if (NETCOREAPP || NET45_OR_GREATER)
        /// <summary>
        /// Write the currently registered keyboard shortcuts to a file, which can be loaded in later.
        /// </summary>
        /// <param name="file">the path of the file to write to</param>
        public async Task WriteShortcutsToFileAsync(string file)
        {
            await KeyboardShortcutsIo.WriteToFileAsync(KeyRegistry, file);
        }
#endif

        /// <summary>
        /// Write the currently registered keyboard shortcuts to a file, which can be loaded in later.
        /// </summary>
        /// <param name="file">the path of the file to write to</param>
        public void WriteShortcutsToFile(string file)
        {
            KeyboardShortcutsIo.WriteToFile(KeyRegistry, file);
        }

        /// <summary>
        /// Get or set if this keyboard shortcut handler is currently active. If <c>false</c>, then it will not detect keyboard shortcuts or invoke actions.
        /// </summary>
        public bool IsActive { get; set; } = true;

        ///// <summary>
        ///// Set if menu items should display the keyboard shortcut combinations directly in the user interface.
        ///// </summary>
        ///// <param name="display">True to display keyboard shortcut combinations, false to not have them displayed.</param>
        //public void ApplyDisplaySettings(bool display)
        //{
        //    KeyRegistry.ApplyDisplaySettings(display);
        //}

        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            // first, check for modifier key changes

            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                CtrlPressed = true;
                keyCheck.Start();
                return;
            }

            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                ShiftPressed = true;
                keyCheck.Start();
                return;
            }

            if (e.Key == Key.LeftAlt || e.Key == Key.RightAlt || e.Key == Key.System)
            {
                AltPressed = true;
                keyCheck.Start();

                //if (!gwi.MenuBarVisible && !CtrlPressed && !ShiftPressed)
                //{

                //    firstMenuShow = true;
                //    txtMnu.Text = "true";
                //    ShowMenuBarTemporarily();
                //}

                //menu.Focus();

                return;
            }

            // secondly, check for keyboard shortcuts!

            if (IsActive)
            {
#if NETCOREAPP
                (IKeyAction? m, string s) = KeyRegistry.GetActionForKey(e.Key, ShiftPressed, AltPressed, CtrlPressed);
#else
                (IKeyAction m, string s) = KeyRegistry.GetActionForKey(e.Key, ShiftPressed, AltPressed, CtrlPressed);
#endif

                m?.Execute();
            }

            return;
        }

        private void window_KeyUp(object sender, KeyEventArgs e)
        {
            // use to monitor modifier key changes

            CtrlPressed &= (e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl);

            ShiftPressed &= (e.Key != Key.LeftShift && e.Key != Key.RightShift);

            AltPressed &= (e.Key != Key.LeftAlt && e.Key != Key.RightAlt && e.Key != Key.System);
        }

#if NETCOREAPP
        private void KeyModifierCheck(object? sender, EventArgs e)
#else
        private void KeyModifierCheck(object sender, EventArgs e)
#endif
        {
            if (CtrlPressed)
            {
                if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    CtrlPressed = false;
                }
            }

            if (ShiftPressed)
            {
                if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                {
                    ShiftPressed = false;
                }
            }

            if (AltPressed)
            {
                if (!Keyboard.IsKeyDown(Key.LeftAlt) && !Keyboard.IsKeyDown(Key.RightAlt) && !Keyboard.IsKeyDown(Key.System))
                {
                    AltPressed = false;
                }
            }

            if (!CtrlPressed && !ShiftPressed && !AltPressed)
            {
                keyCheck.Stop();
            }
        }

    }
}
