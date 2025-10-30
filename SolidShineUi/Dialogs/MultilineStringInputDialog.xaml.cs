﻿using System;
using System.Windows;
using System.Windows.Input;

namespace SolidShineUi
{
    /// <summary>
    /// A dialog for the user to enter in a string (in response to a message or prompt), with the ability to enter in multiple lines of text.
    /// </summary>
    public partial class MultilineStringInputDialog : FlatWindow
    {

        #region Window Actions

        /// <summary>
        /// Create a StringInputDialog with nothing preset.
        /// </summary>
        public MultilineStringInputDialog()
        {
            InitializeComponent();
            ColorScheme = new ColorScheme();
        }

        /// <summary>
        /// Create a StringInputDialog with a color scheme.
        /// </summary>
        /// <param name="cs">The color scheme to use for the window.</param>
        public MultilineStringInputDialog(ColorScheme cs)
        {
            InitializeComponent();
            ColorScheme = cs;
        }

        /// <summary>
        /// Create a StringInputBox with prefilled values.
        /// </summary>
        /// <param name="cs">The color scheme to use for the window.</param>
        /// <param name="title">The title of the window.</param>
        /// <param name="desc">The description to give to the user.</param>
        /// <param name="value">The value to place in the text box. By default, the text box is empty.</param>
        public MultilineStringInputDialog(ColorScheme cs, string title, string desc, string value = "")
        {
            InitializeComponent();
            ColorScheme = cs;

            Title = title;
            txtDesc.Text = desc;

            txtValue.Text = value;
        }

        private void FlatWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
        }

        #endregion

        /// <summary>
        /// Get or set the text value of the input dialog's text box.
        /// </summary>
        public string Value
        {
            get => txtValue.Text;
            set => txtValue.Text = value;
        }

        /// <summary>
        /// Get the result of the dialog when it is closed. "False" refers to the user cancelling the operation, while "True" refers to the user confirming, by clicking "OK" or pressing the Enter key.
        /// </summary>
        public new bool DialogResult { get; private set; } = false;

        /// <summary>
        /// Get or set whether the Enter key can be used to confirm the dialog. If enabled, pressing down Ctrl+Enter will be treated as if the user pressed "OK".
        /// </summary>
        /// <remarks>
        /// With the MultilineStringInputDialog, pressing Enter will move down to a new line. To confirm, users must press both the Control and Enter keys at the same time.
        /// </remarks>
        public bool EnterKeyConfirms { get; set; } = true;

        /// <summary>
        /// Get or set whether the Escape key can be used to cancel the dialog. If enabled, pressing down the Escape key will be treated as if the user pressed "Cancel".
        /// </summary>
        public bool EscapeKeyCancels { get; set; } = true;

        /// <summary>
        /// Get or set whether all of the text in the text box should be selected when the text box receives focus.
        /// </summary>
        public bool SelectTextOnFocus
        {
            get => txtValue.SelectOnFocus;
            set => txtValue.SelectOnFocus = value;
        }

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

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control && EnterKeyConfirms)
            {
                btnOK_Click(this, e);
            }
            else if (e.Key == Key.Escape && EscapeKeyCancels)
            {
                btnCancel_Click(this, e);
            }
        }

        private void window_SourceInitialized(object sender, EventArgs e)
        {
            DisableMinimizeAction();
        }
    }
}
