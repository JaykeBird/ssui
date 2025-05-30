﻿using System;
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

namespace SolidShineUi
{
    /// <summary>
    /// A dialog to display a message to the user, and potentially allowing them to select from a few options via selecting the appropriate button. Similar to the generic WPF MessageBox.
    /// </summary>
    public partial class MessageDialog : FlatWindow
    {
        //DispatcherTimer exitTimer = new DispatcherTimer();
        DispatcherTimer invalidTimer = new DispatcherTimer();

        #region Window Actions

        /// <summary>
        /// Create a new MessageDialog.
        /// </summary>
        public MessageDialog()
        {
            InitializeComponent();

            UpdateAppearance();

            invalidTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            invalidTimer.Tick += InvalidTimer_Tick;
        }

        /// <summary>
        /// Create a new MessageDialog, with the ColorScheme property preset.
        /// </summary>
        /// <param name="cs">The ColorScheme to use for this MessageDialog.</param>
        public MessageDialog(ColorScheme cs)
        {
            InitializeComponent();

            SetValue(ColorSchemeProperty, cs);
            UpdateAppearance();

            invalidTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            invalidTimer.Tick += InvalidTimer_Tick;
        }

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
        }

        #endregion

        #region Color Scheme

        /// <summary>
        /// Apply a color scheme to this control. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply.</param>
        public new void ApplyColorScheme(ColorScheme cs)
        {
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }

            UpdateAppearance();
        }

        void UpdateAppearance()
        {
            base.ApplyColorScheme(ColorScheme);

            //grdButtonContainer.Background = BrushFactory.Create(ColorScheme.SecondaryColor);

            btnCancel.ApplyColorScheme(ColorScheme);
            btnOK.ApplyColorScheme(ColorScheme);
            btnDiscard.ApplyColorScheme(ColorScheme);
            extraButton1.ApplyColorScheme(ColorScheme);
            extraButton2.ApplyColorScheme(ColorScheme);
            extraButton3.ApplyColorScheme(ColorScheme);
            chkBox.ApplyColorScheme(ColorScheme);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public new static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(MessageDialog),
                new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// The color scheme to use with the message dialog.
        /// </summary>
        public new ColorScheme ColorScheme
        {
            get
            {
                return (ColorScheme)GetValue(ColorSchemeProperty);
            }
            set
            {
                SetValue(ColorSchemeProperty, value);
            }
        }

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public new static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            ColorScheme cs = (e.NewValue as ColorScheme)!;
#else
            ColorScheme cs = e.NewValue as ColorScheme;
#endif
            if (d is MessageDialog m)
            {
                m.ApplyColorScheme(cs);
            }
        }

        #endregion

        #region Direct Properties

        /// <summary>Get or set the text to display in the OK button. If empty, the button will not be displayed (unless the other buttons are also not displayed, in which case this one will be).</summary>
        public string OkButtonText { get; set; } = "OK";
        /// <summary>Get or set the text to display in the Cancel button. If empty, the button will not be displayed.</summary>
        public string CancelButtonText { get; set; } = "";
        /// <summary>Get or set the text to display in the Discard button. If empty, the button will not be displayed.</summary>
        public string DiscardButtonText { get; set; } = "";

        /// <summary>Get or set the text to display in the first extra button. If empty, the button will not be displayed.</summary>
        public string ExtraButton1Text { get; set; } = "";
        /// <summary>Get or set the text to display in the second extra button. If empty, the button will not be displayed.</summary>
        public string ExtraButton2Text { get; set; } = "";
        /// <summary>Get or set the text to display in the third extra button. If empty, the button will not be displayed.</summary>
        public string ExtraButton3Text { get; set; } = "";

        /// <summary>Get the result of the message dialog, indicating which button the user pressed.</summary>
        public new MessageDialogResult DialogResult { get; private set; } = MessageDialogResult.Cancel;

        //bool _oneButtonDialog = false;

        /// <summary>Get or set the text to display for the message.</summary>
        public string Message
        {
            get
            {
                return txtMessage.Text;
            }
            set
            {
                txtMessage.Text = value;
            }
        }

        /// <summary>Get or set the text to display with the checkbox. If empty, the checkbox will not be displayed.</summary>
        public string CheckBoxText { get; set; } = "";

        /// <summary>Get or set the checked state of the checkbox. Use the checkbox to display a "Remember my choice"-style option.</summary>
        public bool CheckBoxValue
        {
            get
            {
                return chkBox.IsChecked;
            }
            set
            {
                chkBox.IsChecked = value;
            }
        }

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

                IconVariation mio = IconVariation.Color;

                if (ColorScheme.IsHighContrast)
                {
                    mio = IconLoader.SelectVariationFromColorScheme(ColorScheme);
                }

                try
                {
                    imgIcon.Source = GetImage(value, mio); //Icons.GetMessageBoxIcon(image);
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
        /// Display this message dialog. Use the properties such as <c>OkButtonText</c> or <c>CancelButtonText</c> and <c>Message</c> to control the appearance of the message dialog.
        /// </summary>
        public new MessageDialogResult ShowDialog()
        {
            //if (string.IsNullOrEmpty(CancelButtonText))
            //{
            //    _oneButtonDialog = true;

            //    btnCancel.Content = OkButtonText;
            //    btnOK.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    btnCancel.Content = CancelButtonText;
            //    btnOK.Content = OkButtonText;
            //}

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

            bool showExtraButtons = false;

            if (!string.IsNullOrEmpty(ExtraButton1Text))
            {
                extraButton1.Content = ExtraButton1Text;
                extraButton1.Visibility = Visibility.Visible;
                showExtraButtons = true;
            }

            if (!string.IsNullOrEmpty(ExtraButton2Text))
            {
                extraButton2.Content = ExtraButton2Text;
                extraButton2.Visibility = Visibility.Visible;
                showExtraButtons = true;
            }

            if (!string.IsNullOrEmpty(ExtraButton3Text))
            {
                extraButton3.Content = ExtraButton3Text;
                extraButton3.Visibility = Visibility.Visible;
                showExtraButtons = true;
            }

            if (showExtraButtons)
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
        /// <param name="colorScheme">The color scheme to use with the dialog. Use <c>null</c> if already set via property.</param>
        /// <param name="owner">The owner window of this dialog. Use <c>null</c> if already set via property.</param>
        /// <param name="title">The window title for this dialog. Use <c>null</c> if already set via property.</param>
        /// <param name="buttonDisplay">Determine how many buttons should be displayed at the bottom of the dialog, either [OK], [OK] and [Cancel], or [OK] [Discard] and [Cancel].</param>
        /// <param name="image">The image to display with this dialog.</param>
        /// <param name="defaultButton">The button to have selected by default when the dialog opens. (DOESN'T CURRENTLY WORK)</param>
        /// <param name="customOkButtonText">The text to use in the OK button. Use <c>null</c> if already set via property.</param>
        /// <param name="customCancelButtonText">The text to use in the Cancel button. Use <c>null</c> if already set via property.</param>
        /// <param name="customDiscardButtonText">The text to use in the Discard button. Use <c>null</c> if already set via property.</param>
        /// <param name="extraButton1Text">The text to use in the first extra button. If this is set to a null or empty string, this button will not be displayed.</param>
        /// <param name="extraButton2Text">The text to use in the second extra button. If this is set to a null or empty string, this button will not be displayed.</param>
        /// <param name="extraButton3Text">The text to use in the third extra button. If this is set to a null or empty string, this button will not be displayed.</param>
        /// <param name="checkBoxText">The text to use in the check box. If this is set to a null or empty string, the check box will not be displayed.</param>
#if NETCOREAPP
        public MessageDialogResult ShowDialog(string message, ColorScheme? colorScheme = null, Window? owner = null, string title = "Dialog", MessageDialogButtonDisplay buttonDisplay = MessageDialogButtonDisplay.Auto, 
            MessageDialogImage image = MessageDialogImage.None, MessageDialogResult defaultButton = MessageDialogResult.Cancel, 
            string? customOkButtonText = null, string? customCancelButtonText = null, string? customDiscardButtonText = null,
            string? extraButton1Text = null, string? extraButton2Text = null, string? extraButton3Text = null, string? checkBoxText = null)
#else
        public MessageDialogResult ShowDialog(string message, ColorScheme colorScheme = null, Window owner = null, string title = "Dialog", MessageDialogButtonDisplay buttonDisplay = MessageDialogButtonDisplay.Auto,
            MessageDialogImage image = MessageDialogImage.None, MessageDialogResult defaultButton = MessageDialogResult.Cancel,
            string customOkButtonText = null, string customCancelButtonText = null, string customDiscardButtonText = null,
            string extraButton1Text = null, string extraButton2Text = null, string extraButton3Text = null, string checkBoxText = null)
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

            if (!string.IsNullOrEmpty(customOkButtonText))
            {
                OkButtonText = customOkButtonText;
            }
            if (!string.IsNullOrEmpty(customCancelButtonText))
            {
                CancelButtonText = customCancelButtonText;
            }
            if (!string.IsNullOrEmpty(customDiscardButtonText))
            {
                DiscardButtonText = customDiscardButtonText;
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

            if (!string.IsNullOrEmpty(extraButton1Text))
            {
                ExtraButton1Text = extraButton1Text;
            }
            if (!string.IsNullOrEmpty(extraButton2Text))
            {
                ExtraButton2Text = extraButton2Text;
            }
            if (!string.IsNullOrEmpty(extraButton3Text))
            {
                ExtraButton3Text = extraButton3Text;
            }
            if (!string.IsNullOrEmpty(checkBoxText))
            {
                CheckBoxText = checkBoxText;
            }

            bool showExtraButtons = false;

            if (!string.IsNullOrEmpty(ExtraButton1Text))
            {
                extraButton1.Content = ExtraButton1Text;
                extraButton1.Visibility = Visibility.Visible;
                showExtraButtons = true;
            }

            if (!string.IsNullOrEmpty(ExtraButton2Text))
            {
                extraButton2.Content = ExtraButton2Text;
                extraButton2.Visibility = Visibility.Visible;
                showExtraButtons = true;
            }

            if (!string.IsNullOrEmpty(ExtraButton3Text))
            {
                extraButton3.Content = ExtraButton3Text;
                extraButton3.Visibility = Visibility.Visible;
                showExtraButtons = true;
            }

            if (!string.IsNullOrEmpty(CheckBoxText))
            {
                chkBox.Content = CheckBoxText;
                chkBox.Visibility = Visibility.Visible;
            }

            if (showExtraButtons)
            {
                stkExtraButtons.Margin = new Thickness(70, 10, 20, 10);
            }

            // TODO: use defaultButton value to instead handle Enter or Space key press
            //switch (defaultButton)
            //{
            //    case MessageDialogResult.OK:
            //        if (btnOK.Visibility == Visibility.Visible)
            //        {
            //            btnOK.IsDefault = true;
            //        }
            //        else
            //        {
            //            btnCancel.IsDefault = true;
            //        }
            //        break;
            //    case MessageDialogResult.Discard:
            //        btnDiscard.IsDefault = true;
            //        break;
            //    case MessageDialogResult.Cancel:
            //        btnCancel.IsDefault = true;
            //        break;
            //    case MessageDialogResult.Extra1:
            //        if (extraButton1.Visibility == Visibility.Visible)
            //        {
            //            extraButton1.IsDefault = true;
            //            btnCancel.IsDefault = false;
            //            btnOK.IsDefault = false;
            //        }
            //        else
            //        {
            //            btnCancel.IsDefault = true;
            //        }
            //        break;
            //    case MessageDialogResult.Extra2:
            //        if (extraButton2.Visibility == Visibility.Visible)
            //        {
            //            extraButton2.IsDefault = true;
            //            btnCancel.IsDefault = false;
            //            btnOK.IsDefault = false;
            //        }
            //        else
            //        {
            //            btnCancel.IsDefault = true;
            //        }
            //        break;
            //    case MessageDialogResult.Extra3:
            //        if (extraButton3.Visibility == Visibility.Visible)
            //        {
            //            extraButton3.IsDefault = true;
            //            btnCancel.IsDefault = false;
            //            btnOK.IsDefault = false;
            //        }
            //        else
            //        {
            //            btnCancel.IsDefault = true;
            //        }
            //        break;
            //}

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

        //protected override Size MeasureOverride(Size constraint)
        //{
        //    var result = base.MeasureOverride(constraint);
        //    // ... add custom measure code here if desired ...
        //    InvalidateVisual();
        //    return result;
        //}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
#if NETCOREAPP
        string GetStringOrNull(string? value, string defaultValue, bool zeroAsNull = false)
#else
        string GetStringOrNull(string value, string defaultValue, bool zeroAsNull = false)
#endif
        {
            if (zeroAsNull)
            {
                if (value == "0") value = null;
            }

            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        #region Button Presses

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //if (_oneButtonDialog)
            //{
            //    DialogResult = MessageDialogResult.OK;
            //}
            //else
            //{
            //    DialogResult = MessageDialogResult.Cancel;
            //}
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
        /// <summary>A white I on a blue background is displayed. This is good for generally informing the user about something, such as a task being completed or the response to a request.</summary>
        Info = 5,
        /// <summary>A red stop sign is displayed. This is good for informing the user that this action isn't valid in the current state, or that an unavoidable major issue has occurred.</summary>
        Stop = 6,
        /// <summary>A yellow lock is displayed. This is good for situations where security or authentication are involved.</summary>
        Lock = 7 //, Check = 8
    }

    /// <summary>
    /// A helper class that can retrieve a BitmapImage from a MessageDialogImage.
    /// </summary>
    public static class MessageDialogImageConverter
    {
        /// <summary>
        /// Return a 32x32 image to use with a message dialog. If the MessageDialogImage argument is "None", then <c>null</c> is returned.
        /// </summary>
        /// <param name="image">The image to display. If "None", then <c>null</c> is returned.</param>
        /// <param name="color">The color to use for the image. Use black or white for high-contrast themes.</param>
        /// <returns></returns>
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
    /// <see cref="Auto"/> is used by default; if <see cref="Auto"/> is used, the <see cref="MessageDialog.OkButtonText"/>, <see cref="MessageDialog.CancelButtonText"/>, 
    /// and <see cref="MessageDialog.DiscardButtonText"/> properties determine which buttons are displayed.
    /// </remarks>
    public enum MessageDialogButtonDisplay
    {
        /// <summary>
        /// If set, uses the OkButtonText, CancelButtonText, and DiscardButtonText properties to determine which buttons should be displayed. 
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
        /// Display three buttons, the OK, Discard, and Cancel buttons. Outputs as <c>MessageDialogResult.OK</c>, <c>MessageDialogResult.Discard</c> or <c>MessageDialogResult.Cancel</c> when clicked. 
        /// </summary>
        Three = 3
    }
}
