using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using KeyBoard = System.Windows.Input.Keyboard;

namespace SolidShineUi.KeyboardShortcuts
{
    public class KeyboardShortcutHandler
    {
        public KeyboardShortcutHandler(Window w)
        {
            Window = w;

            w.KeyDown += window_KeyDown;
            w.KeyUp += window_KeyUp;

            keyCheck = new DispatcherTimer(new TimeSpan(400), DispatcherPriority.Input, KeyModifierCheck, w.Dispatcher);
        }

        public Window Window { get; set; }
        public KeyRegistry KeyRegistry { get; } = new KeyRegistry();

        private DispatcherTimer keyCheck;

        bool CtrlPressed = false;
        bool ShiftPressed = false;
        bool AltPressed = false;

        public void LoadShortcutsFromList(List<KeyboardShortcut> ks)
        {
            foreach (KeyboardShortcut item in ks)
            {
                KeyRegistry.RegisterKeyShortcut(item);
            }
        }

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

        public async Task WriteShortcutsToFileAsync(string file)
        {
            await KeyboardShortcutsIo.WriteToFile(KeyRegistry, file);
        }

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

#if NETCOREAPP
            (IKeyAction? m, string s) = KeyRegistry.GetActionForKey(e.Key, ShiftPressed, AltPressed, CtrlPressed);
#else
            (IKeyAction m, string s) = KeyRegistry.GetActionForKey(e.Key, ShiftPressed, AltPressed, CtrlPressed);
#endif

            if (m != null)
            {
                m.Execute();
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
                if (!KeyBoard.IsKeyDown(Key.LeftCtrl) && !KeyBoard.IsKeyDown(Key.RightCtrl))
                {
                    CtrlPressed = false;
                }
            }

            if (ShiftPressed)
            {
                if (!KeyBoard.IsKeyDown(Key.LeftShift) && !KeyBoard.IsKeyDown(Key.RightShift))
                {
                    ShiftPressed = false;
                }
            }

            if (AltPressed)
            {
                if (!KeyBoard.IsKeyDown(Key.LeftAlt) && !KeyBoard.IsKeyDown(Key.RightAlt) && !KeyBoard.IsKeyDown(Key.System))
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
