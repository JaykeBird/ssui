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

namespace SolidShineUi
{
    /// <summary>
    /// A dialog for the user to enter in a string (in response to a message or prompt).
    /// </summary>
    public partial class StringInputDialog : FlatWindow
    {

        #region Window Actions

        /// <summary>
        /// Create a StringInputDialog with nothing preset.
        /// </summary>
        public StringInputDialog()
        {
            InitializeComponent();
            ColorScheme = new ColorScheme();
        }

        /// <summary>
        /// Create a StringInputDialog with a color scheme.
        /// </summary>
        /// <param name="cs">The color scheme to use for the window.</param>
        public StringInputDialog(ColorScheme cs)
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
        public StringInputDialog(ColorScheme cs, string title, string desc, string value = "")
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
            CheckValidation();
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
        /// Get or set whether the Enter key can be used to confirm the dialog. If enabled, pressing down the Enter key will be treated as if the user pressed "OK".
        /// </summary>
        public bool EnterKeyConfirms { get => (bool)GetValue(EnterKeyConfirmsProperty); set => SetValue(EnterKeyConfirmsProperty, value); }

        /// <summary>
        /// A dependency property backing the related property. Please see <see cref="EnterKeyConfirms"/> for details.
        /// </summary>
        public static DependencyProperty EnterKeyConfirmsProperty
            = DependencyProperty.Register("EnterKeyConfirms", typeof(bool), typeof(StringInputDialog),
            new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Get or set whether the Escape key can be used to cancel the dialog. If enabled, pressing down the Escape key will be treated as if the user pressed "Cancel".
        /// </summary>
        public bool EscapeKeyCancels { get => (bool)GetValue(EscapeKeyCancelsProperty); set => SetValue(EscapeKeyCancelsProperty, value); }

        /// <summary>
        /// A dependency property backing the related property. Please see <see cref="EscapeKeyCancels"/> for details.
        /// </summary>
        public static DependencyProperty EscapeKeyCancelsProperty
            = DependencyProperty.Register("EscapeKeyCancels", typeof(bool), typeof(StringInputDialog),
            new FrameworkPropertyMetadata(true));

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
        /// <remarks>Try to keep the description to about a sentence long. If you do have a lengthier description, you may need to resize the window to make it fit properly.
        /// Ideally, the overall design of the program should make it apparent what the user should enter into the text box without reading the description.
        /// However, the description is helpful to remind the user what is being asked of them here, and also to potentially clarify the types of values that are valid or invalid.</remarks>
        public string Description { get => (string)GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value); }

        /// <summary>
        /// A dependency property backing the related property. See <see cref="Description"/> for details.
        /// </summary>
        public static DependencyProperty DescriptionProperty
            = DependencyProperty.Register("Description", typeof(string), typeof(StringInputDialog),
            new FrameworkPropertyMetadata("Enter a value:"));

        #region Data Validation

        /// <summary>
        /// Get or set the data validation function that should be used to make sure the inputted string matches an expected format.
        /// </summary>
        /// <remarks>
        /// Set this to <c>null</c> if you want to disable data validation.
        /// This function is called every time that the text in the text box is changed, so it's important to have a function that is relatively fast, so as
        /// to avoid blocking the UI thread.
        /// The function will be given the current text in the text box, and is asked to return a bool if it matches the expected validation
        /// (<c>true</c> for a good match, <c>false</c> for an invalid string).
        /// </remarks>
#if NETCOREAPP
        public Func<string, bool>? ValidationFunction { get; set; } = null;
#else
        public Func<string, bool> ValidationFunction { get; set; } = null;
#endif

        /// <summary>
        /// Get or set the string to display in the UI when the validation returns true (a success), indicating a good and valid string.
        /// </summary>
        public string ValidationSuccessString { get => (string)GetValue(ValidationSuccessStringProperty); set => SetValue(ValidationSuccessStringProperty, value); }

        /// <summary>
        /// A dependency property backing the related property. See <see cref="ValidationSuccessString"/> for details.
        /// </summary>
        public static DependencyProperty ValidationSuccessStringProperty
            = DependencyProperty.Register("ValidationSuccessString", typeof(string), typeof(StringInputDialog),
            new FrameworkPropertyMetadata(""));

        /// <summary>
        /// Get or set the string to display in the UI when the validation returns false (a failure), indicating an invalid or unusable string.
        /// </summary>
        public string ValidationFailureString { get => (string)GetValue(ValidationFailureStringProperty); set => SetValue(ValidationFailureStringProperty, value); }

        /// <summary>
        /// A dependency property backing the related property. See <see cref="ValidationFailureString"/> for details.
        /// </summary>
        public static DependencyProperty ValidationFailureStringProperty
            = DependencyProperty.Register("ValidationFailureString", typeof(string), typeof(StringInputDialog),
            new FrameworkPropertyMetadata("Input is not valid"));

        void CheckValidation()
        {
            if (ValidationFunction != null)
            {
                bool result = ValidationFunction(txtValue.Text);
                if (result)
                {
                    txtValidation.Text = ValidationSuccessString;
                    btnOK.IsEnabled = true;
                }
                else
                {
                    txtValidation.Text = ValidationFailureString;
                    btnOK.IsEnabled = false;
                }
            }
            else
            {
                // if validation is disabled, make sure the OK button isn't stuck disabled
                txtValidation.Text = "";
                btnOK.IsEnabled = true;
            }
        }

        #endregion

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txtValue_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void txtValue_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CheckValidation();
        }
    }
}
