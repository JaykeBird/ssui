using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace SolidShineUi;

/// <summary>
/// A dialog to display a message to the user, and potentially allowing them to select from a few options via selecting the 
/// appropriate button. Similar to the generic WPF MessageBox.
/// </summary>
public partial class MessageDialog : ThemedUserControl
{
    /// <summary>
    /// Create a new MessageDialog.
    /// </summary>
    public MessageDialog()
    {
        InitializeComponent();

        FontSize = 12;

        Loaded += MessageDialog_Loaded;
    }

    #region Window Loading

    bool _preLoaded = false;

    private void MessageDialog_Loaded(object? sender, RoutedEventArgs e)
    {
        if (_preLoaded) return;

        if (string.IsNullOrEmpty(CancelButtonText))
        {
            btnCancel.IsVisible = false;
        }
        else
        {
            btnCancel.IsVisible = true;
            btnCancel.Content = CancelButtonText;
        }

        if (string.IsNullOrEmpty(DiscardButtonText))
        {
            btnDiscard.IsVisible = false;
        }
        else
        {
            btnDiscard.IsVisible = true;
            btnDiscard.Content = DiscardButtonText;
        }

        if (string.IsNullOrEmpty(OkButtonText))
        {
            // if Cancel button, Discard button, and Ok button are all null/not shown, reset the Ok button and show that
            if (!btnCancel.IsVisible && !btnDiscard.IsVisible)
            {
                OkButtonText = "OK";
                btnOK.IsVisible = true;
            }
            else
            {
                btnOK.IsVisible = false;
            }
        }
        else
        {
            btnOK.Content = OkButtonText;
            btnOK.IsVisible = true;
        }

        bool showChoiceButtons = false;

        if (!string.IsNullOrEmpty(ChoiceButton1Text))
        {
            extraButton1.Content = ChoiceButton1Text;
            extraButton1.IsVisible = true;
            showChoiceButtons = true;
        }

        if (!string.IsNullOrEmpty(ChoiceButton2Text))
        {
            extraButton2.Content = ChoiceButton2Text;
            extraButton2.IsVisible = true;
            showChoiceButtons = true;
        }

        if (!string.IsNullOrEmpty(ChoiceButton3Text))
        {
            extraButton3.Content = ChoiceButton3Text;
            extraButton3.IsVisible = true;
            showChoiceButtons = true;
        }

        if (showChoiceButtons)
        {
            stkExtraButtons.Margin = new Thickness(70, 10, 20, 10);
        }

        if (!string.IsNullOrEmpty(CheckBoxText))
        {
            chkBox.Content = CheckBoxText;
            chkBox.IsVisible = true;
        }

        //Image = Image;
    }

    #endregion

    #region Properties

    #region DialogResult

    private MessageDialogResult _dialogResult = MessageDialogResult.Cancel;

    /// <summary>Get the result of the message dialog, indicating which button the user pressed.</summary>
    public MessageDialogResult DialogResult { get => _dialogResult; private set => SetAndRaise(DialogResultProperty, ref _dialogResult, value); }

    /// <summary>The backing direct property for <see cref="DialogResult"/>. See the related property for details.</summary>
    public static readonly DirectProperty<MessageDialog, MessageDialogResult> DialogResultProperty
        = AvaloniaProperty.RegisterDirect<MessageDialog, MessageDialogResult>(nameof(DialogResult), 
            (s) => s.DialogResult, unsetValue: MessageDialogResult.Cancel);

    #endregion

    #region MessageDialog Appearance / Items

    /// <summary>Get or set the text to display in the OK button. If empty, the button will not be displayed.</summary>
    /// <remarks>
    /// If the text of all bottom-row buttons (<c>OkButtonText</c>, <c>CancelButtonText</c>, and <c>DiscordButtonText</c>) is set to 
    /// null or empty, then the OK button will be shown with the value "OK".
    /// </remarks>
    public string OkButtonText { get => GetValue(OkButtonTextProperty); set => SetValue(OkButtonTextProperty, value); }

    /// <summary>The backing styled property for <see cref="OkButtonText"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> OkButtonTextProperty
        = AvaloniaProperty.Register<MessageDialog, string>(nameof(OkButtonText), "OK");


    /// <summary>Get or set the text to display in the Cancel button. If empty, the button will not be displayed.</summary>
    public string CancelButtonText { get => GetValue(CancelButtonTextProperty); set => SetValue(CancelButtonTextProperty, value); }

    /// <summary>The backing styled property for <see cref="CancelButtonText"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> CancelButtonTextProperty
        = AvaloniaProperty.Register<MessageDialog, string>(nameof(CancelButtonText), "");


    /// <summary>Get or set the text to display in the Discard button. If empty, the button will not be displayed.</summary>
    public string DiscardButtonText { get => GetValue(DiscardButtonTextProperty); set => SetValue(DiscardButtonTextProperty, value); }

    /// <summary>The backing styled property for <see cref="DiscardButtonText"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> DiscardButtonTextProperty
        = AvaloniaProperty.Register<MessageDialog, string>(nameof(DiscardButtonText), "");


    /// <summary>Get or set the text to display in the first choice button. If empty, the button will not be displayed.</summary>
    public string ChoiceButton1Text { get => GetValue(ChoiceButton1TextProperty); set => SetValue(ChoiceButton1TextProperty, value); }

    /// <summary>The backing styled property for <see cref="ChoiceButton1Text"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> ChoiceButton1TextProperty
        = AvaloniaProperty.Register<MessageDialog, string>(nameof(ChoiceButton1Text), "");


    /// <summary>Get or set the text to display in the second choice button. If empty, the button will not be displayed.</summary>
    public string ChoiceButton2Text { get => GetValue(ChoiceButton2TextProperty); set => SetValue(ChoiceButton2TextProperty, value); }

    /// <summary>The backing styled property for <see cref="ChoiceButton2Text"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> ChoiceButton2TextProperty
        = AvaloniaProperty.Register<MessageDialog, string>(nameof(ChoiceButton2Text), "");


    /// <summary>Get or set the text to display in the third choice button. If empty, the button will not be displayed.</summary>
    public string ChoiceButton3Text { get => GetValue(ChoiceButton3TextProperty); set => SetValue(ChoiceButton3TextProperty, value); }

    /// <summary>The backing styled property for <see cref="ChoiceButton3Text"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> ChoiceButton3TextProperty
        = AvaloniaProperty.Register<MessageDialog, string>(nameof(ChoiceButton3Text), "");


    /// <summary>Get or set the text to display for the message in the dialog.</summary>
    public string Message { get => GetValue(MessageProperty); set => SetValue(MessageProperty, value); }

    /// <summary>The backing styled property for <see cref="Message"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> MessageProperty
        = AvaloniaProperty.Register<MessageDialog, string>(nameof(Message), "Message.");


    /// <summary>Get or set the text to display with the checkbox. If empty, the checkbox will not be displayed.</summary>
    /// <remarks>The checkbox can be used to display a "Remember my choice"-style option.</remarks>
    public string CheckBoxText { get => GetValue(CheckBoxTextProperty); set => SetValue(CheckBoxTextProperty, value); }

    /// <summary>The backing styled property for <see cref="CheckBoxText"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> CheckBoxTextProperty
        = AvaloniaProperty.Register<MessageDialog, string>(nameof(CheckBoxText), "");

    /// <summary>Get or set the checked state of the checkbox. The checkbox is only displayed if <c>CheckBoxText</c> is not null or empty.</summary>
    /// <remarks>The checkbox can be used to display a "Remember my choice"-style option.</remarks>
    public bool CheckBoxValue { get => GetValue(CheckBoxValueProperty); set => SetValue(CheckBoxValueProperty, value); }

    /// <summary>The backing styled property for <see cref="CheckBoxValue"/>. See the related property for details.</summary>
    public static readonly StyledProperty<bool> CheckBoxValueProperty
        = AvaloniaProperty.Register<MessageDialog, bool>(nameof(CheckBoxValue), false);

    #endregion

    #region Title

    /// <summary>
    /// Get or set the title to use for this dialog, if used as a window.
    /// </summary>
    public string Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

    /// <summary>The backing styled property for <see cref="Title"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> TitleProperty
        = AvaloniaProperty.Register<MessageDialog, string>(nameof(Title), "");

    #endregion

    #region Closing

    /// <summary>
    /// Request for this dialog to be closed. This raising the <c>Closing</c> event.
    /// <para/>
    /// If this dialog is in a window via <see cref="DisplayAsWindow(WindowBase?)"/>, then it will trigger closing the window. 
    /// Otherwise if you are not using a window, you should then add a listener to the Closing event, so that you can modify
    /// your UI as needed when this Close function is called.
    /// </summary>
    public void Close()
    {
        CancelEventArgs ce = new CancelEventArgs();
        Closing?.Invoke(this, ce);
        if (ce.Cancel) return;

        CloseWindow?.Invoke(this, ce);
    }

    /// <summary>
    /// Raised when the dialog is requested to be closed, with the ability to cancel the closing.
    /// </summary>
    public event CancelEventHandler? Closing;

    private event EventHandler? CloseWindow;

    #endregion

    #endregion

    #region Window

    /// <summary>
    /// Display this dialog as a window.
    /// </summary>
    /// <param name="owner">The owner of the window, if there is one.</param>
    public ThemedWindow DisplayAsWindow(WindowBase? owner = null)
    {
        ThemedWindow tw = owner != null ? new ThemedWindow(owner) : new ThemedWindow();
        tw.Content = this;
        tw.Title = this.Title;

        tw.Width = this.Width;
        tw.Height = this.Height;

        tw.CanResize = false;
        tw.CanMinimize = false;
        tw.ShowInTaskbar = false;
        tw.Topmost = true;

        CloseWindow += (s, e) => tw.Close();

        if (this.SsuiTheme is SsuiAppTheme)
        {
            tw.SsuiTheme = this.SsuiTheme;
        }
        Bind(SsuiThemeProperty, CompiledBinding.Create<ThemedWindow, SsuiTheme?>(win => win.SsuiTheme, source: tw));

        return tw;
    }

    #endregion

    #region ShowDialogAsync

    /// <summary>
    /// Display the MessageDialog window as a modal dialog, and return the <see cref="MessageDialogResult"/> after it is closed.
    /// </summary>
    /// <param name="owner">the window that acts as the owner for this dialog</param>
    /// <returns>A <see cref="MessageDialogResult"/> object representing the button the user pressed</returns>
    public async Task<MessageDialogResult> ShowDialogWindowAsync(Window owner)
    {
        ThemedWindow window = DisplayAsWindow();

        await window.ShowDialog(owner);

        return DialogResult;
    }

    /// <summary>
    /// Display this message dialog.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="owner">The owner window of this dialog.</param>
    /// <param name="title">The window title for this dialog. Use <c>null</c> if already set via property.</param>
    /// <remarks>
    /// Unless the <c>OkButtonText</c>, <c>CancelButtonText</c>, and/or <c>DiscardButtonText</c> properties were changed, this will by default
    /// by just an OK-only message dialog.
    /// </remarks>
    public async Task<MessageDialogResult> ShowDialogWindowAsync(string message, Window owner, string title = "Dialog")
    {
        return await ShowDialogWindowAsync(message, owner, title);
    }

    /// <summary>
    /// Display this message dialog.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="owner">The owner window of this dialog.</param>
    /// <param name="title">The window title for this dialog. Use <c>null</c> if already set via property.</param>
    /// <param name="buttonDisplay">Determine how many buttons should be displayed at the bottom of the dialog, either [OK], [OK] and [Cancel], 
    /// or [OK] [Discard] and [Cancel].</param>
    /// <param name="defaultResult">The button to trigger by default if the user presses the "Enter" or "Space" keys</param>
    /// <param name="okButtonText">The text to use in the OK button. Use <c>null</c> if already set via property.</param>
    /// <param name="cancelButtonText">The text to use in the Cancel button. Use <c>null</c> if already set via property.</param>
    /// <param name="discardButtonText">The text to use in the Discard button. Use <c>null</c> if already set via property.</param>
    /// <param name="choiceButton1Text">The text to use in the first choice button. If this is set to a null or empty string, this button will not be displayed.</param>
    /// <param name="choiceButton2Text">The text to use in the second choice button. If this is set to a null or empty string, this button will not be displayed.</param>
    /// <param name="choiceButton3Text">The text to use in the third choice button. If this is set to a null or empty string, this button will not be displayed.</param>
    /// <param name="checkBoxText">The text to use in the check box. If this is set to a null or empty string, the check box will not be displayed.</param>
    public async Task<MessageDialogResult> ShowDialogWindowAsync(string message, Window owner, string title = "Dialog",
    MessageDialogButtonDisplay buttonDisplay = MessageDialogButtonDisplay.Auto,
        MessageDialogResult defaultResult = MessageDialogResult.Cancel,
        string? okButtonText = null, string? cancelButtonText = null, string? discardButtonText = null,
        string? choiceButton1Text = null, string? choiceButton2Text = null, string? choiceButton3Text = null, string? checkBoxText = null)

    {
        txtMessage.Text = message;

        if (!string.IsNullOrEmpty(title))
        {
            Title = title;
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
                    btnCancel.IsVisible = false;
                }
                else
                {
                    btnCancel.IsVisible = true;
                    btnCancel.Content = CancelButtonText;
                }

                if (string.IsNullOrEmpty(DiscardButtonText))
                {
                    btnDiscard.IsVisible = false;
                }
                else
                {
                    btnDiscard.IsVisible = true;
                    btnDiscard.Content = DiscardButtonText;
                }

                if (string.IsNullOrEmpty(OkButtonText))
                {
                    // if Cancel button, Discard button, and Ok button are all null/not shown, reset the Ok button and show that
                    if (!btnCancel.IsVisible && !btnDiscard.IsVisible)
                    {
                        OkButtonText = "OK";
                        btnOK.IsVisible = true;
                    }
                    else
                    {
                        btnOK.IsVisible = false;
                    }
                }
                else
                {
                    btnOK.Content = OkButtonText;
                    btnOK.IsVisible = true;
                }
                break;
            case MessageDialogButtonDisplay.One:
                btnOK.Content = GetStringOrNull(OkButtonText, "OK");

                btnOK.IsVisible = true;
                btnCancel.IsVisible = false;
                btnDiscard.IsVisible = false;
                break;
            case MessageDialogButtonDisplay.Two:
                btnOK.Content = GetStringOrNull(OkButtonText, "OK");
                btnCancel.Content = GetStringOrNull(CancelButtonText, "Cancel");

                btnOK.IsVisible = true;
                btnCancel.IsVisible = true;
                btnDiscard.IsVisible = false;
                break;
            case MessageDialogButtonDisplay.Three:
                btnOK.Content = GetStringOrNull(OkButtonText, "OK");
                btnCancel.Content = GetStringOrNull(CancelButtonText, "Cancel");
                btnDiscard.Content = GetStringOrNull(DiscardButtonText, "Discard");

                btnOK.IsVisible = true;
                btnCancel.IsVisible = true;
                btnDiscard.IsVisible = true;
                break;
            default:
                // same as "Auto"
                if (string.IsNullOrEmpty(CancelButtonText))
                {
                    btnCancel.IsVisible = false;
                }
                else
                {
                    btnCancel.IsVisible = true;
                    btnCancel.Content = CancelButtonText;
                }

                if (string.IsNullOrEmpty(DiscardButtonText))
                {
                    btnDiscard.IsVisible = false;
                }
                else
                {
                    btnDiscard.IsVisible = true;
                    btnDiscard.Content = DiscardButtonText;
                }

                if (string.IsNullOrEmpty(OkButtonText))
                {
                    // if Cancel button, Discard button, and Ok button are all null/not shown, reset the Ok button and show that
                    if (!btnCancel.IsVisible && !btnDiscard.IsVisible)
                    {
                        OkButtonText = "OK";
                        btnOK.IsVisible = true;
                    }
                    else
                    {
                        btnOK.IsVisible = false;
                    }
                }
                else
                {
                    btnOK.Content = OkButtonText;
                    btnOK.IsVisible = true;
                }
                break;
        }

        // Image = image;

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
            extraButton1.IsVisible = true;
            showChoiceButtons = true;
        }

        if (!string.IsNullOrEmpty(ChoiceButton2Text))
        {
            extraButton2.Content = ChoiceButton2Text;
            extraButton2.IsVisible = true;
            showChoiceButtons = true;
        }

        if (!string.IsNullOrEmpty(ChoiceButton3Text))
        {
            extraButton3.Content = ChoiceButton3Text;
            extraButton3.IsVisible = true;
            showChoiceButtons = true;
        }

        if (!string.IsNullOrEmpty(CheckBoxText))
        {
            chkBox.Content = CheckBoxText;
            chkBox.IsVisible = true;
        }

        if (showChoiceButtons)
        {
            stkExtraButtons.Margin = new Thickness(70, 10, 20, 10);
        }

        // DefaultDialogResult = defaultResult;

        _preLoaded = true;

        return await ShowDialogWindowAsync(owner);
    }

    #endregion

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

    static string GetStringOrNull(string value, string defaultValue)
    {
        return string.IsNullOrEmpty(value) ? defaultValue : value;
    }
}