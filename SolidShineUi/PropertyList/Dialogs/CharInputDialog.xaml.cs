using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SolidShineUi;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;

namespace SolidShineUi.PropertyList.Dialogs
{
    /// <summary>
    /// A dialog for the user to enter in a string (in response to a message or prompt).
    /// </summary>
    public partial class CharInputDialog : FlatWindow
    {

        #region Window Actions

        /// <summary>
        /// Create a StringInputDialog with nothing preset.
        /// </summary>
        public CharInputDialog()
        {
            InitializeComponent();
            ColorScheme = new ColorScheme();
        }

        /// <summary>
        /// Create a StringInputDialog with a color scheme.
        /// </summary>
        /// <param name="cs">The color scheme to use for the window.</param>
        public CharInputDialog(ColorScheme cs)
        {
            InitializeComponent();
            ColorScheme = cs;
        }

        /// <summary>
        /// Create a StringInputBox with prefilled values.
        /// </summary>
        /// <param name="cs">The color scheme to use for the window.</param>
        /// <param name="value">The value to preload into this dialog. The user is able to change the value though.</param>
        public CharInputDialog(ColorScheme cs, char value = 'a')
        {
            InitializeComponent();
            ColorScheme = cs;

            c = value;
            EnterValueIntoBoxes();
        }

#if NETCOREAPP
        /// <summary>
        /// Create a StringInputBox with prefilled values. If a Rune is entered, then the dialog operates in "Rune mode"; this doesn't affect the appearance, only how the dialog interacts with the user-entered values.
        /// </summary>
        /// <param name="cs">The color scheme to use for the window.</param>
        /// <param name="value">The value to preload into this dialog. The user is able to change the value though.</param>
        public CharInputDialog(ColorScheme cs, Rune value)
        {
            InitializeComponent();
            ColorScheme = cs;

            r = value;
            runeMode = true;
            EnterValueIntoBoxes();
        }
#endif

        private void FlatWindow_Loaded(object sender, RoutedEventArgs e)
        {
            nudDec.MinValue = char.MinValue;
            nudDec.MaxValue = char.MaxValue;
            txtHex.Focus();
        }

        #endregion

        bool runeMode = false;
        char c = 'a';
#if NETCOREAPP
        Rune r = (Rune)'a';
#endif

        bool _internalAction = false;

        void EnterValueIntoBoxes()
        {
            _internalAction = true;
            UpdateHexBox();
            UpdateDecBox();
            UpdatePreview();
            _internalAction = false;
        }

        void UpdateHexBox()
        {
            if (runeMode)
            {
#if NETCOREAPP
                txtHex.Text = r.Value.ToString("X");
#else
                txtHex.Text = "";
#endif
            }
            else
            {
                txtHex.Text = ((int)c).ToString("X");
            }
        }

        void UpdateDecBox()
        {
            if (runeMode)
            {
#if NETCOREAPP
                nudDec.Value = r.Value;
#else
                nudDec.Value = 0;
#endif
            }
            else
            {
                nudDec.Value = c;
            }
        }

        void UpdatePreview()
        {
            if (runeMode)
            {
#if NETCOREAPP
                txtPreview.Text = r.ToString();
#else
                txtPreview.Text = "(error - rune mode)";
#endif
            }
            else
            {
                if (char.IsSurrogate(c))
                {
                    txtPreview.Text = "(surrogate)";
                }
                else
                {
                    txtPreview.Text = new string(c, 1);
                }
            }
        }

        /// <summary>
        /// Get the value entered into this dialog, as a char.
        /// </summary>
        public char ValueAsChar
        {
            get
            {
#if NETCOREAPP
                if (runeMode)
                {
                    return r.ToString()[0];
                }
                else
                {
                    return c;
                }
#else
            return c;
#endif
            }
            set
            {
#if NETCOREAPP

#else
            c = value;
            EnterValueIntoBoxes();
#endif
            }
        }

#if NETCOREAPP
        /// <summary>
        /// Get the value entered into this dialog, as a Rune.
        /// </summary>
        public Rune ValueAsRune
        {
            get
            {
                if (runeMode)
                {
                    return r;
                }
                else
                {
                    return (Rune)c;
                }
            }
        }
#endif

        /// <summary>
        /// Get the result of the dialog when it is closed. "False" refers to the user cancelling the operation, while "True" refers to the user confirming, by clicking "OK" or pressing the Enter key.
        /// </summary>
        public new bool DialogResult { get; private set; } = false;

        /// <summary>
        /// Get or set whether the Enter key can be used to confirm the dialog. If enabled, pressing down the Enter key will be treated as if the user pressed "OK".
        /// </summary>
        public bool EnterKeyConfirms { get; set; } = true;

        /// <summary>
        /// Get or set whether the Escape key can be used to cancel the dialog. If enabled, pressing down the Escape key will be treated as if the user pressed "Cancel".
        /// </summary>
        public bool EscapeKeyCancels { get; set; } = true;

        /// <summary>
        /// Get or set the description text to display above the text box. This text should describe what the user should enter into the text box.
        /// </summary>
        /// <remarks>Try to keep the description to about a sentence long. 
        /// Ideally, the overall design of the program should make it apparent what the user should enter into the text box without reading the description.</remarks>
        public string Description
        {
            get
            {
                return txtDesc.Text;
            }
            set
            {
                txtDesc.Text = value;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txtName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && EnterKeyConfirms)
            {
                btnOK_Click(this, e);
            }
            else if (e.Key == System.Windows.Input.Key.Escape && EscapeKeyCancels)
            {
                btnCancel_Click(this, e);
            }
        }

        private static string UnicodeToChar(string hex)
        {
            try
            {
                int code = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                string unicodeString = char.ConvertFromUtf32(code);
                return unicodeString;
            }
            catch (FormatException)
            {
                return "a";
            }
        }

        private void txtHex_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_internalAction) return;

            string value = UnicodeToChar(txtHex.Text);

            if (runeMode)
            {
#if NETCOREAPP
                r = Rune.GetRuneAt(value, 0);
#else
                // shrug?
#endif
            }
            else
            {
                c = value[0];
            }

            UpdateDecBox();
            UpdatePreview();
        }

        private void nudDec_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_internalAction) return;

            if (runeMode)
            {
#if NETCOREAPP
                r = new Rune(nudDec.Value);
#else
                // shrug?
#endif
            }
            else
            {
                c = Convert.ToChar(nudDec.Value);
            }

            UpdateHexBox();
            UpdatePreview();
        }
    }
}
