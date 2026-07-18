using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;
using System.IO;
using System.Windows.Media.Imaging;
using static SolidShineUi.MessageDialogImageConverter;
using SolidShineUi.Utils;
using System.ComponentModel;

namespace SolidShineUi
{
    /// <summary>
    /// A dialog to display a message to the user, and potentially allowing them to select from a few options via selecting the 
    /// appropriate button. Similar to the generic WPF MessageBox.
    /// </summary>
    public partial class MessageDialog : FlatWindow
    {
        //DispatcherTimer exitTimer = new DispatcherTimer();
        DispatcherTimer invalidTimer = new DispatcherTimer();

        #region Window Actions

        #region Constructors

        /// <summary>
        /// Create a new MessageDialog.
        /// </summary>
        public MessageDialog()
        {
            InitializeComponent();

            invalidTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            invalidTimer.Tick += InvalidTimer_Tick;

            KeyUp += MessageDialog_KeyUp;
        }

        /// <summary>
        /// Create a new MessageDialog, with the ColorScheme property preset.
        /// </summary>
        /// <param name="cs">The ColorScheme to use for this MessageDialog.</param>
        public MessageDialog(ColorScheme cs)
        {
            InitializeComponent();

            SetValue(ColorSchemeProperty, cs);

            invalidTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            invalidTimer.Tick += InvalidTimer_Tick;

            KeyUp += MessageDialog_KeyUp;
        }

        #endregion

        #region Window Loaded / Invalid Timer

#if NETCOREAPP
        private void InvalidTimer_Tick(object? sender, EventArgs e)
#else
        private void InvalidTimer_Tick(object sender, EventArgs e)
#endif
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                InvalidateMeasure();
                InvalidateVisual();
            });

            invalidTimer.IsEnabled = false;
        }

        private void FlatWindow_Loaded(object sender, RoutedEventArgs e)
        {
            invalidTimer.Start();

            switch (DefaultDialogResult)
            {
                case MessageDialogResult.OK:
                    if (btnOK.Visibility == Visibility.Visible)
                    {
                        btnOK.Focus();
                    }
                    else
                    {
                        btnCancel.Focus();
                    }
                    break;
                case MessageDialogResult.Discard:
                    if (btnDiscard.Visibility == Visibility.Visible)
                    {
                        btnDiscard.Focus();
                    }
                    else if (btnCancel.Visibility == Visibility.Visible)
                    {
                        btnCancel.Focus();
                    }
                    else
                    {
                        btnOK.Focus();
                    }
                    break;
                case MessageDialogResult.Cancel:
                    if (btnCancel.Visibility == Visibility.Visible)
                    {
                        btnCancel.Focus();
                    }
                    else
                    {
                        btnOK.Focus();
                    }
                    break;
                case MessageDialogResult.Extra1:
                    if (extraButton1.Visibility == Visibility.Visible)
                    {
                        extraButton1.Focus();
                    }
                    else if (btnCancel.Visibility == Visibility.Visible)
                    {
                        btnCancel.Focus();
                    }
                    else
                    {
                        btnOK.Focus();
                    }
                    break;
                case MessageDialogResult.Extra2:
                    if (extraButton2.Visibility == Visibility.Visible)
                    {
                        extraButton2.Focus();
                    }
                    else if (btnCancel.Visibility == Visibility.Visible)
                    {
                        btnCancel.Focus();
                    }
                    else
                    {
                        btnOK.Focus();
                    }
                    break;
                case MessageDialogResult.Extra3:
                    if (extraButton3.Visibility == Visibility.Visible)
                    {
                        extraButton3.Focus();
                    }
                    else if (btnCancel.Visibility == Visibility.Visible)
                    {
                        btnCancel.Focus();
                    }
                    else
                    {
                        btnOK.Focus();
                    }
                    break;
            }
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>Get or set the text to display in the OK button. If empty, the button will not be displayed.</summary>
        /// <remarks>
        /// If the text of all bottom-row buttons (<c>OkButtonText</c>, <c>CancelButtonText</c>, and <c>DiscordButtonText</c>) is set to 
        /// null or empty, then the OK button will be shown with the value "OK".
        /// </remarks>
        public string OkButtonText { get => (string)GetValue(OkButtonTextProperty); set => SetValue(OkButtonTextProperty, value); }

        /// <summary>The backing dependency property for <see cref="OkButtonText"/>. See the related property for details.</summary>
        public static readonly DependencyProperty OkButtonTextProperty
            = DependencyProperty.Register(nameof(OkButtonText), typeof(string), typeof(MessageDialog),
            new FrameworkPropertyMetadata("OK"));

        /// <summary>Get or set the text to display in the Cancel button. If empty, the button will not be displayed.</summary>
        public string CancelButtonText { get => (string)GetValue(CancelButtonTextProperty); set => SetValue(CancelButtonTextProperty, value); }

        /// <summary>The backing dependency property for <see cref="CancelButtonText"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CancelButtonTextProperty
            = DependencyProperty.Register(nameof(CancelButtonText), typeof(string), typeof(MessageDialog),
            new FrameworkPropertyMetadata(""));

        /// <summary>Get or set the text to display in the Discard button. If empty, the button will not be displayed.</summary>
        public string DiscardButtonText { get => (string)GetValue(DiscardButtonTextProperty); set => SetValue(DiscardButtonTextProperty, value); }

        /// <summary>The backing dependency property for <see cref="DiscardButtonText"/>. See the related property for details.</summary>
        public static readonly DependencyProperty DiscardButtonTextProperty
            = DependencyProperty.Register(nameof(DiscardButtonText), typeof(string), typeof(MessageDialog),
            new FrameworkPropertyMetadata(""));


        /// <summary>Get or set the text to display in the first choice button. If empty, the button will not be displayed.</summary>
        public string ChoiceButton1Text { get => (string)GetValue(ChoiceButton1TextProperty); set => SetValue(ChoiceButton1TextProperty, value); }

        /// <summary>The backing dependency property for <see cref="ChoiceButton1Text"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ChoiceButton1TextProperty
            = DependencyProperty.Register(nameof(ChoiceButton1Text), typeof(string), typeof(MessageDialog),
            new FrameworkPropertyMetadata(""));

        /// <summary>Get or set the text to display in the second choice button. If empty, the button will not be displayed.</summary>
        public string ChoiceButton2Text { get => (string)GetValue(ChoiceButton2TextProperty); set => SetValue(ChoiceButton2TextProperty, value); }

        /// <summary>The backing dependency property for <see cref="ChoiceButton2Text"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ChoiceButton2TextProperty
            = DependencyProperty.Register(nameof(ChoiceButton2Text), typeof(string), typeof(MessageDialog),
            new FrameworkPropertyMetadata(""));

        /// <summary>Get or set the text to display in the third choice button. If empty, the button will not be displayed.</summary>
        public string ChoiceButton3Text { get => (string)GetValue(ChoiceButton3TextProperty); set => SetValue(ChoiceButton3TextProperty, value); }

        /// <summary>The backing dependency property for <see cref="ChoiceButton3Text"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ChoiceButton3TextProperty
            = DependencyProperty.Register(nameof(ChoiceButton3Text), typeof(string), typeof(MessageDialog),
            new FrameworkPropertyMetadata(""));

        /// <summary>Get the result of the message dialog, indicating which button the user pressed.</summary>
        [ReadOnly(true)]
        public new MessageDialogResult DialogResult
        {
            get => (MessageDialogResult)GetValue(DialogResultProperty); private set => SetValue(DialogResultPropertyKey, value); 
        }

        private static readonly DependencyPropertyKey DialogResultPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(DialogResult), typeof(MessageDialogResult), typeof(MessageDialog),
            new FrameworkPropertyMetadata(MessageDialogResult.Cancel));

        /// <summary>The backing dependency property for <see cref="DialogResult"/>. See the related property for details.</summary>
        public static readonly DependencyProperty DialogResultProperty = DialogResultPropertyKey.DependencyProperty;


        /// <summary>Get or set the text to display for the message in the dialog.</summary>
        public string Message { get => (string)GetValue(MessageProperty); set => SetValue(MessageProperty, value); }

        /// <summary>The backing dependency property for <see cref="Message"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MessageProperty
            = DependencyProperty.Register(nameof(Message), typeof(string), typeof(MessageDialog),
            new FrameworkPropertyMetadata("Message."));

        /// <summary>Get or set the text to display with the checkbox. If empty, the checkbox will not be displayed.</summary>
        /// <remarks>The checkbox can be used to display a "Remember my choice"-style option.</remarks>
        public string CheckBoxText { get => (string)GetValue(CheckBoxTextProperty); set => SetValue(CheckBoxTextProperty, value); }

        /// <summary>The backing dependency property for <see cref="CheckBoxText"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CheckBoxTextProperty
            = DependencyProperty.Register(nameof(CheckBoxText), typeof(string), typeof(MessageDialog),
            new FrameworkPropertyMetadata(""));

        /// <summary>Get or set the checked state of the checkbox. The checkbox is only displayed if <c>CheckBoxText</c> is not null or empty.</summary>
        /// <remarks>The checkbox can be used to display a "Remember my choice"-style option.</remarks>
        public bool CheckBoxValue { get => (bool)GetValue(CheckBoxValueProperty); set => SetValue(CheckBoxValueProperty, value); }

        /// <summary>The backing dependency property for <see cref="CheckBoxValue"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CheckBoxValueProperty
            = DependencyProperty.Register(nameof(CheckBoxValue), typeof(bool), typeof(MessageDialog),
            new FrameworkPropertyMetadata(false));


        private MessageDialogImage _image = MessageDialogImage.None;

        /// <summary>Get or set the image to display with the message.</summary>
        public MessageDialogImage Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;

                try
                {
                    imgIcon.Source = GetImage(value, SsuiTheme.IconVariation);
                }
                catch (ArgumentException)
                {
                    imgIcon.Source = null;
                }
                catch (IOException)
                {
                    imgIcon.Source = null;
                }
            }
        }


        #endregion

        #region Show Dialog

        /// <summary>
        /// Display this message dialog. Change the properties (such as <c>OkButtonText</c>, <c>CancelButtonText</c>, and <c>Message</c>) to 
        /// control the appearance of the message dialog.
        /// </summary>
        public new MessageDialogResult ShowDialog()
        {
            if (string.IsNullOrEmpty(CancelButtonText))
            {
                btnCancel.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnCancel.Visibility = Visibility.Visible;
                btnCancel.Content = CancelButtonText;
            }

            if (string.IsNullOrEmpty(DiscardButtonText))
            {
                btnDiscard.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnDiscard.Visibility = Visibility.Visible;
                btnDiscard.Content = DiscardButtonText;
            }

            if (string.IsNullOrEmpty(OkButtonText))
            {
                // if Cancel button, Discard button, and Ok button are all null/not shown, reset the Ok button and show that
                if (btnCancel.Visibility == Visibility.Collapsed && btnDiscard.Visibility == Visibility.Collapsed)
                {
                    OkButtonText = "OK";
                    btnOK.Visibility = Visibility.Visible;
                }
                else
                {
                    btnOK.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                btnOK.Content = OkButtonText;
                btnOK.Visibility = Visibility.Visible;
            }

            bool showChoiceButtons = false;

            if (!string.IsNullOrEmpty(ChoiceButton1Text))
            {
                extraButton1.Content = ChoiceButton1Text;
                extraButton1.Visibility = Visibility.Visible;
                showChoiceButtons = true;
            }

            if (!string.IsNullOrEmpty(ChoiceButton2Text))
            {
                extraButton2.Content = ChoiceButton2Text;
                extraButton2.Visibility = Visibility.Visible;
                showChoiceButtons = true;
            }

            if (!string.IsNullOrEmpty(ChoiceButton3Text))
            {
                extraButton3.Content = ChoiceButton3Text;
                extraButton3.Visibility = Visibility.Visible;
                showChoiceButtons = true;
            }

            if (showChoiceButtons)
            {
                stkExtraButtons.Margin = new Thickness(70, 10, 20, 10);
            }

            if (!string.IsNullOrEmpty(CheckBoxText))
            {
                chkBox.Content = CheckBoxText;
                chkBox.Visibility = Visibility.Visible;
            }

            //Image = Image;

            if (Owner != null)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            base.ShowDialog();

            return DialogResult;
        }

        /// <summary>
        /// Display this message dialog.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="owner">The owner window of this dialog. Use <c>null</c> if already set via property.</param>
        /// <param name="title">The window title for this dialog. Use <c>null</c> if already set via property.</param>
        /// <param name="image">The image to display with this dialog.</param>
        /// <remarks>
        /// Unless the <c>OkButtonText</c>, <c>CancelButtonText</c>, and/or <c>DiscardButtonText</c> properties were changed, this will by default
        /// by just an OK-only message dialog.
        /// </remarks>
#if NETCOREAPP
        public MessageDialogResult ShowDialog(string message, Window? owner = null, string title = "Dialog", MessageDialogImage image = MessageDialogImage.None)
#else
        public MessageDialogResult ShowDialog(string message, Window owner = null, string title = "Dialog", MessageDialogImage image = MessageDialogImage.None)
#endif
        {
            return ShowDialog(message, null, owner, title, image: image);
        }

        /// <summary>
        /// Display this message dialog.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="colorScheme">The color scheme to use with the dialog. Use <c>null</c> if already set via property.</param>
        /// <param name="owner">The owner window of this dialog. Use <c>null</c> if already set via property.</param>
        /// <param name="title">The window title for this dialog. Use <c>null</c> if already set via property.</param>
        /// <param name="buttonDisplay">Determine how many buttons should be displayed at the bottom of the dialog, either [OK], [OK] and [Cancel], 
        /// or [OK] [Discard] and [Cancel].</param>
        /// <param name="image">The image to display with this dialog.</param>
        /// <param name="defaultResult">The button to trigger by default if the user presses the "Enter" or "Space" keys</param>
        /// <param name="okButtonText">The text to use in the OK button. Use <c>null</c> if already set via property.</param>
        /// <param name="cancelButtonText">The text to use in the Cancel button. Use <c>null</c> if already set via property.</param>
        /// <param name="discardButtonText">The text to use in the Discard button. Use <c>null</c> if already set via property.</param>
        /// <param name="choiceButton1Text">The text to use in the first choice button. If this is set to a null or empty string, this button will not be displayed.</param>
        /// <param name="choiceButton2Text">The text to use in the second choice button. If this is set to a null or empty string, this button will not be displayed.</param>
        /// <param name="choiceButton3Text">The text to use in the third choice button. If this is set to a null or empty string, this button will not be displayed.</param>
        /// <param name="checkBoxText">The text to use in the check box. If this is set to a null or empty string, the check box will not be displayed.</param>
#if NETCOREAPP
        public MessageDialogResult ShowDialog(string message, ColorScheme? colorScheme = null, Window? owner = null, string title = "Dialog", 
        MessageDialogButtonDisplay buttonDisplay = MessageDialogButtonDisplay.Auto, 
            MessageDialogImage image = MessageDialogImage.None, MessageDialogResult defaultResult = MessageDialogResult.Cancel, 
            string? okButtonText = null, string? cancelButtonText = null, string? discardButtonText = null,
            string? choiceButton1Text = null, string? choiceButton2Text = null, string? choiceButton3Text = null, string? checkBoxText = null)
#else
        public MessageDialogResult ShowDialog(string message, ColorScheme colorScheme = null, Window owner = null, string title = "Dialog", 
            MessageDialogButtonDisplay buttonDisplay = MessageDialogButtonDisplay.Auto,
            MessageDialogImage image = MessageDialogImage.None, MessageDialogResult defaultResult = MessageDialogResult.Cancel,
            string okButtonText = null, string cancelButtonText = null, string discardButtonText = null,
            string choiceButton1Text = null, string choiceButton2Text = null, string choiceButton3Text = null, string checkBoxText = null)
#endif
        {
            txtMessage.Text = message;

            if (!string.IsNullOrEmpty(title))
            {
                Title = title;
            }

            if (colorScheme != null)
            {
                SetValue(ColorSchemeProperty, colorScheme);
            }

            if (owner != null)
            {
                Owner = owner;
            }

            if (!string.IsNullOrEmpty(okButtonText))
            {
                OkButtonText = okButtonText;
            }
            if (!string.IsNullOrEmpty(cancelButtonText))
            {
                CancelButtonText = cancelButtonText;
            }
            if (!string.IsNullOrEmpty(discardButtonText))
            {
                DiscardButtonText = discardButtonText;
            }

            switch (buttonDisplay)
            {
                case MessageDialogButtonDisplay.Auto:
                    if (string.IsNullOrEmpty(CancelButtonText))
                    {
                        btnCancel.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        btnCancel.Visibility = Visibility.Visible;
                        btnCancel.Content = CancelButtonText;
                    }

                    if (string.IsNullOrEmpty(DiscardButtonText))
                    {
                        btnDiscard.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        btnDiscard.Visibility = Visibility.Visible;
                        btnDiscard.Content = DiscardButtonText;
                    }

                    if (string.IsNullOrEmpty(OkButtonText))
                    {
                        // if Cancel button, Discard button, and Ok button are all null/not shown, reset the Ok button and show that
                        if (btnCancel.Visibility == Visibility.Collapsed && btnDiscard.Visibility == Visibility.Collapsed)
                        {
                            OkButtonText = "OK";
                            btnOK.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            btnOK.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        btnOK.Content = OkButtonText;
                        btnOK.Visibility = Visibility.Visible;
                    }
                    break;
                case MessageDialogButtonDisplay.One:
                    btnOK.Content = GetStringOrNull(OkButtonText, "OK");

                    btnOK.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Collapsed;
                    btnDiscard.Visibility = Visibility.Collapsed;
                    break;
                case MessageDialogButtonDisplay.Two:
                    btnOK.Content = GetStringOrNull(OkButtonText, "OK");
                    btnCancel.Content = GetStringOrNull(CancelButtonText, "Cancel");

                    btnOK.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Visible;
                    btnDiscard.Visibility = Visibility.Collapsed;
                    break;
                case MessageDialogButtonDisplay.Three:
                    btnOK.Content = GetStringOrNull(OkButtonText, "OK");
                    btnCancel.Content = GetStringOrNull(CancelButtonText, "Cancel");
                    btnDiscard.Content = GetStringOrNull(DiscardButtonText, "Discard");

                    btnOK.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Visible;
                    btnDiscard.Visibility = Visibility.Visible;
                    break;
                default:
                    // same as "Auto"
                    if (string.IsNullOrEmpty(CancelButtonText))
                    {
                        btnCancel.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        btnCancel.Visibility = Visibility.Visible;
                        btnCancel.Content = CancelButtonText;
                    }

                    if (string.IsNullOrEmpty(DiscardButtonText))
                    {
                        btnDiscard.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        btnDiscard.Visibility = Visibility.Visible;
                        btnDiscard.Content = DiscardButtonText;
                    }

                    if (string.IsNullOrEmpty(OkButtonText))
                    {
                        // if Cancel button, Discard button, and Ok button are all null/not shown, reset the Ok button and show that
                        if (btnCancel.Visibility == Visibility.Collapsed && btnDiscard.Visibility == Visibility.Collapsed)
                        {
                            OkButtonText = "OK";
                            btnOK.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            btnOK.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        btnOK.Content = OkButtonText;
                        btnOK.Visibility = Visibility.Visible;
                    }
                    break;
            }

            Image = image;

            if (!string.IsNullOrEmpty(choiceButton1Text))
            {
                ChoiceButton1Text = choiceButton1Text;
            }
            if (!string.IsNullOrEmpty(choiceButton2Text))
            {
                ChoiceButton2Text = choiceButton2Text;
            }
            if (!string.IsNullOrEmpty(choiceButton3Text))
            {
                ChoiceButton3Text = choiceButton3Text;
            }
            if (!string.IsNullOrEmpty(checkBoxText))
            {
                CheckBoxText = checkBoxText;
            }

            bool showChoiceButtons = false;

            if (!string.IsNullOrEmpty(ChoiceButton1Text))
            {
                extraButton1.Content = ChoiceButton1Text;
                extraButton1.Visibility = Visibility.Visible;
                showChoiceButtons = true;
            }

            if (!string.IsNullOrEmpty(ChoiceButton2Text))
            {
                extraButton2.Content = ChoiceButton2Text;
                extraButton2.Visibility = Visibility.Visible;
                showChoiceButtons = true;
            }

            if (!string.IsNullOrEmpty(ChoiceButton3Text))
            {
                extraButton3.Content = ChoiceButton3Text;
                extraButton3.Visibility = Visibility.Visible;
                showChoiceButtons = true;
            }

            if (!string.IsNullOrEmpty(CheckBoxText))
            {
                chkBox.Content = CheckBoxText;
                chkBox.Visibility = Visibility.Visible;
            }

            if (showChoiceButtons)
            {
                stkExtraButtons.Margin = new Thickness(70, 10, 20, 10);
            }

            DefaultDialogResult = defaultResult;

            if (Owner != null)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            base.ShowDialog();

            return DialogResult;
        }

#endregion


#if NETCOREAPP
        static string GetStringOrNull(string? value, string defaultValue)
#else
        static string GetStringOrNull(string value, string defaultValue)
#endif
        {
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        #region Button Presses

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = MessageDialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = MessageDialogResult.OK;
            Close();
        }

        private void btnDiscard_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = MessageDialogResult.Discard;
            Close();
        }

        private void extraButton1_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = MessageDialogResult.Extra1;
            Close();
        }

        private void extraButton2_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = MessageDialogResult.Extra2;
            Close();
        }

        private void extraButton3_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = MessageDialogResult.Extra3;
            Close();
        }

        #endregion

        #region Key Presses / Default Result
        
        /// <summary>
        /// Get or set the dialog result that will be considered the default when the dialog is opened.
        /// The corresponding button will be highlighted when the dialog is opened, and if the user
        /// presses the Space or Enter key, this result will be selected (if they don't move focus to another button).
        /// </summary>
        /// <remarks>
        /// If the result value set here doesn't correspond to a button that's shown on the dialog, the Cancel or OK button will be highlighted instead.
        /// </remarks>
        public MessageDialogResult DefaultDialogResult { get; set; } = MessageDialogResult.Cancel;

        private void MessageDialog_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.Escape:
                    // treat this as a cancel
                    if (btnCancel.Visibility == Visibility.Visible)
                    {
                        DialogResult = MessageDialogResult.Cancel;
                        Close();
                    }
                    else
                    {
                        DialogResult = MessageDialogResult.OK;
                        Close();
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

    }

    /// <summary>
    /// The image to use when displaying a message dialog.
    /// </summary>
    public enum MessageDialogImage
    {
        /// <summary>No image is displayed. This may be good for generic messages, but is not recommended for situations where the user should pay attention to the message.</summary>
        None = 0,
        /// <summary>A white X on a red background is displayed. This is good for situations where an error or exception has occurred, or something isn't acting as intended.</summary>
        Error = 1,
        /// <summary>An exclamation point on a yellow triangle is displayed. This is good when informing the user about potential errors or unintended side effects.</summary>
        Warning = 2,
        /// <summary>A white question mark on a blue background is displayed. This is good when asking a question or when asking for confirmation for a routine task.</summary>
        Question = 3,
        /// <summary>A white exclamation point on a blue background is displayed. This is good for when wanting to alert the user about something that isn't considered a warning.</summary>
        Hand = 4,
        /// <summary>A white "i" on a blue background is displayed. This is good for generally informing the user about something, such as a task being completed or the response to a request.</summary>
        Info = 5,
        /// <summary>A red stop sign is displayed. This is good for informing the user that this action isn't valid in the current state, or that an unavoidable major issue has occurred.</summary>
        Stop = 6,
        /// <summary>A yellow lock is displayed. This is good for situations where security or authentication are involved.</summary>
        Lock = 7 //, Check = 8
    }

    /// <summary>
    /// A helper class that can retrieve a <see cref="BitmapImage"/> based on a <see cref="MessageDialogImage"/> value.
    /// </summary>
    public static class MessageDialogImageConverter
    {
        /// <summary>
        /// Return a 32x32 image to use with a message dialog. If the MessageDialogImage argument is "None", then <c>null</c> is returned.
        /// </summary>
        /// <param name="image">The image to display. If "None", then <c>null</c> is returned.</param>
        /// <param name="color">The color to use for the image. Use black or white for high-contrast themes.</param>
        /// <returns>The image, if located, or <c>null</c> if <see cref="MessageDialogImage.None"/> was inputted</returns>
        /// <exception cref="ArgumentException">Thrown if an invalid value is put in for <paramref name="image"/> or <paramref name="color"/></exception>
#if NETCOREAPP
        public static BitmapImage? GetImage(MessageDialogImage image, IconVariation color)
#else
        public static BitmapImage GetImage(MessageDialogImage image, IconVariation color)
#endif
        {
            if (image == MessageDialogImage.None)
            {
                return null;
            }

            string packuri = "pack://application:,,,/SolidShineUi;component/DialogImages/";
            BitmapImage img;

            try
            {
                img = new BitmapImage(new Uri(packuri + image.ToString("f") + color.ToString("f") + ".png", UriKind.Absolute));
            }
            catch (IOException ex)
            {
                throw new ArgumentException("Cannot find an icon with this name.", nameof(image), ex);
            }

            return img;
        }


    }

    /// <summary>
    /// Represents the result of the MessageDialog; specifically, which button that was clicked.
    /// </summary>
    /// <remarks>
    /// Note that since the buttons can be relabeled via the <see cref="MessageDialog.OkButtonText"/>, <see cref="MessageDialog.CancelButtonText"/>, and other properties,
    /// the button names here may not directly relate to the labels that were given.
    /// </remarks>
    public enum MessageDialogResult
    {
        /// <summary>
        /// The OK button was clicked. This is the far-left button on the bottom of the dialog.
        /// </summary>
        OK = 1,
        /// <summary>
        /// The Cancel button was clicked. This is the far-right button on the bottom of the dialog.
        /// </summary>
        Cancel = 2,
        /// <summary>
        /// The first "extra" button was clicked. If visible, this is at the top of the list of buttons in the middle of the dialog.
        /// </summary>
        Extra1 = 3,
        /// <summary>
        /// The second "extra" button was clicked. If visible, this is at the middle of the list of buttons in the middle of the dialog.
        /// </summary>
        Extra2 = 4,
        /// <summary>
        /// The third "extra" button was clicked. If visible, this is at the bottom of the list of buttons in the middle of the dialog.
        /// </summary>
        Extra3 = 5,
        /// <summary>
        /// The Discard button was clicked. If visible, this button is in between the OK and Cancel buttons on the bottom of the dialog.
        /// </summary>
        Discard = 101,
    }

    /// <summary>
    /// Set how many buttons to display at the bottom of the dialog.
    /// </summary>
    /// <remarks>
    /// <see cref="Auto"/> is used by default; if <see cref="Auto"/> is used, the <see cref="MessageDialog.OkButtonText"/>, 
    /// <see cref="MessageDialog.CancelButtonText"/>, and <see cref="MessageDialog.DiscardButtonText"/> properties determine which buttons are displayed.
    /// </remarks>
    public enum MessageDialogButtonDisplay
    {
        /// <summary>
        /// Use the OkButtonText, CancelButtonText, and DiscardButtonText properties to determine which buttons should be displayed. 
        /// If a property is null or empty, then the corresponding button isn't displayed.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// Display only one button, the OK button. Outputs as <c>MessageDialogResult.OK</c> when clicked.
        /// </summary>
        One = 1,
        /// <summary>
        /// Display two buttons, the OK and Cancel buttons. Outputs as <c>MessageDialogResult.OK</c> or <c>MessageDialogResult.Cancel</c> when clicked.
        /// </summary>
        Two = 2,
        /// <summary>
        /// Display three buttons, the OK, Discard, and Cancel buttons. Outputs as <c>MessageDialogResult.OK</c>, 
        /// <c>MessageDialogResult.Discard</c> or <c>MessageDialogResult.Cancel</c> when clicked. 
        /// </summary>
        Three = 3
    }
}
