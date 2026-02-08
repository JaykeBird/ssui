using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SolidShineUi;

namespace SolidShineUi.PropertyList.Dialogs
{
    /// <summary>
    /// A dialog for the user to select a char or Rune via its Unicode code point. Primarily designed for the <see cref="PropertyEditors.CharEditor"/>, but can be used independently as well.
    /// </summary>
    public partial class RectEditDialog : FlatWindow
    {

        // bool _internalAction = false;

        #region Window Constructors / Loaded

        /// <summary>
        /// Create a CharInputDialog.
        /// </summary>
        public RectEditDialog()
        {
            InitializeComponent();
            ColorScheme = new ColorScheme();
        }

        /// <summary>
        /// Create a CharInputDialog with a color scheme.
        /// </summary>
        /// <param name="cs">The color scheme to use for the window.</param>
        public RectEditDialog(ColorScheme cs)
        {
            InitializeComponent();
            ColorScheme = cs;
        }

        private void FlatWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Owner != null && Owner.Icon != null)
            {
                Icon = Owner.Icon.Clone();
            }
            else
            {
                ShowIcon = false;
            }
        }

        #endregion

        #region Dialog properties

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
        /// Get or set the description text to display above the text box. This text should describe why you're asking the user to select a character here.
        /// </summary>
        /// <remarks>Try to keep the description to about a sentence long.</remarks>
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

        #endregion

        /// <summary>
        /// Get the <see cref="Rect"/> value within this dialog.
        /// </summary>
        /// <returns></returns>
        public Rect GetRect()
        {
            return edtRect.GetRect();
        }

        /// <summary>
        /// Set the <see cref="Rect"/> value to edit.
        /// </summary>
        /// <param name="r"></param>
        public void SetRect(Rect r)
        {
            edtRect.LoadRect(r);
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
    }
}
