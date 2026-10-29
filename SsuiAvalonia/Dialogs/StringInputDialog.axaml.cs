using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace SolidShineUi;

/// <summary>
/// A dialog for the user to enter in a string (in response to a message or prompt).
/// </summary>
public partial class StringInputDialog : ThemedUserControl
{

    /// <summary>
    /// Create a StringInputDialog with nothing preset.
    /// </summary>
    public StringInputDialog()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Create a StringInputDialog with prefilled values.
    /// </summary>
    /// <param name="title">The title of the window.</param>
    /// <param name="desc">The description to give to the user.</param>
    /// <param name="value">The value to place in the text box. By default, the text box is empty.</param>
    public StringInputDialog(string title, string desc, string value = "")
    {
        InitializeComponent();

        Title = title;
        txtDesc.Text = desc;

        txtValue.Text = value;
    }

    private void FlatWindow_Loaded(object sender, RoutedEventArgs e)
    {
        txtValue.Focus();
        CheckValidation();

        //if (Icon == null && Owner != null && Owner.Icon != null)
        //{
        //    Icon = Owner.Icon.Clone();
        //}
        //else
        //{
        //    ShowIcon = false;
        //}
    }

    #region Properties

    #region Value / Labels

    /// <summary>
    /// Get or set the text value of the input dialog's text box.
    /// </summary>
    public string Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

    /// <summary>The backing styled property for <see cref="Value"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> ValueProperty
        = AvaloniaProperty.Register<StringInputDialog, string>(nameof(Value), "");


    /// <summary>
    /// Get or set the description text to display above the text box. This text should describe what the user should enter into the text box.
    /// </summary>
    /// <remarks>Try to keep the description to about a sentence long. If you do have a lengthier description, you may need to resize the window to make it fit properly.
    /// Ideally, the overall design of the program should make it apparent what the user should enter into the text box without reading the description.
    /// However, the description is helpful to remind the user what is being asked of them here, and also to potentially clarify the types of values that are valid or invalid.</remarks>
    public string Description { get => GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value); }

    /// <summary>The backing styled property for <see cref="Description"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> DescriptionProperty
        = AvaloniaProperty.Register<StringInputDialog, string>(nameof(Description), "Enter a value:");

    #endregion

    #region Behaviors

    /// <summary>
    /// Get the result of the dialog when it is closed. 
    /// "False" refers to the user cancelling the operation, while "True" refers to the user confirming, by clicking "OK" or pressing the Enter key.
    /// </summary>
    public bool DialogResult { get => GetValue(DialogResultProperty); set => SetValue(DialogResultProperty, value); }

    /// <summary>The backing styled property for <see cref="DialogResult"/>. See the related property for details.</summary>
    public static readonly StyledProperty<bool> DialogResultProperty
        = AvaloniaProperty.Register<StringInputDialog, bool>(nameof(DialogResult), false);


    /// <summary>
    /// Get or set whether the Enter key can be used to confirm the dialog. If enabled, pressing down the Enter key will be treated as if the user pressed "OK".
    /// </summary>
    public bool EnterKeyConfirms { get => GetValue(EnterKeyConfirmsProperty); set => SetValue(EnterKeyConfirmsProperty, value); }

    /// <summary>The backing styled property for <see cref="EnterKeyConfirms"/>. See the related property for details.</summary>
    public static readonly StyledProperty<bool> EnterKeyConfirmsProperty
        = AvaloniaProperty.Register<StringInputDialog, bool>(nameof(EnterKeyConfirms), true);


    /// <summary>
    /// Get or set whether the Escape key can be used to cancel the dialog. If enabled, pressing down the Escape key will be treated as if the user pressed "Cancel".
    /// </summary>
    public bool EscapeKeyCancels { get => GetValue(EscapeKeyCancelsProperty); set => SetValue(EscapeKeyCancelsProperty, value); }

    /// <summary>The backing styled property for <see cref="EscapeKeyCancels"/>. See the related property for details.</summary>
    public static readonly StyledProperty<bool> EscapeKeyCancelsProperty
        = AvaloniaProperty.Register<StringInputDialog, bool>(nameof(EscapeKeyCancels), true);


    /// <summary>
    /// Get or set whether all of the text in the text box should be selected when the text box receives focus.
    /// </summary>
    public bool SelectTextOnFocus { get => GetValue(SelectTextOnFocusProperty); set => SetValue(SelectTextOnFocusProperty, value); }

    /// <summary>The backing styled property for <see cref="SelectTextOnFocus"/>. See the related property for details.</summary>
    public static readonly StyledProperty<bool> SelectTextOnFocusProperty
        = AvaloniaProperty.Register<StringInputDialog, bool>(nameof(SelectTextOnFocus), true);

    #endregion

    #region Data Validation

    bool _validationPass = true;

    private Func<string, bool>? _validationFunction = null;

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
    public Func<string, bool>? ValidationFunction { get => _validationFunction; set => SetAndRaise(ValidationFunctionProperty, ref _validationFunction, value); }

    /// <summary>The backing direct property for <see cref="ValidationFunction"/>. See the related property for details.</summary>
    public static readonly DirectProperty<StringInputDialog, Func<string, bool>?> ValidationFunctionProperty
        = AvaloniaProperty.RegisterDirect<StringInputDialog, Func<string, bool>?>(nameof(ValidationFunction), 
            (s) => s.ValidationFunction, (s, v) => s.ValidationFunction = v, unsetValue: null);


    /// <summary>
    /// Get or set the string to display in the UI when the validation returns true (a success), indicating a good and valid string.
    /// </summary>
    public string ValidationSuccessString { get => GetValue(ValidationSuccessStringProperty); set => SetValue(ValidationSuccessStringProperty, value); }

    /// <summary>The backing styled property for <see cref="ValidationSuccessString"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> ValidationSuccessStringProperty
        = AvaloniaProperty.Register<StringInputDialog, string>(nameof(ValidationSuccessString), "");


    /// <summary>
    /// Get or set the string to display in the UI when the validation returns false (a failure), indicating an invalid or unusable string.
    /// </summary>
    public string ValidationFailureString { get => GetValue(ValidationFailureStringProperty); set => SetValue(ValidationFailureStringProperty, value); }

    /// <summary>The backing styled property for <see cref="ValidationFailureString"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> ValidationFailureStringProperty
        = AvaloniaProperty.Register<StringInputDialog, string>(nameof(ValidationFailureString), "");

    void CheckValidation()
    {
        if (ValidationFunction != null)
        {
            bool result = ValidationFunction(txtValue.Text ?? "");
            if (result)
            {
                txtValidation.Text = ValidationSuccessString;
                btnOK.IsEnabled = true;
                _validationPass = true;
            }
            else
            {
                txtValidation.Text = ValidationFailureString;
                btnOK.IsEnabled = false;
                _validationPass = false;
            }
        }
        else
        {
            // if validation is disabled, make sure the OK button isn't stuck disabled
            txtValidation.Text = "";
            btnOK.IsEnabled = true;
            _validationPass = true;
        }
    }

    #endregion

    #region Title

    /// <summary>
    /// Get or set the title to use for this dialog, if used as a window.
    /// </summary>
    public string Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

    /// <summary>The backing styled property for <see cref="Title"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> TitleProperty
        = AvaloniaProperty.Register<StringInputDialog, string>(nameof(Title), "");

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

        CloseWindow += (s, e) => tw.Close();

        if (this.SsuiTheme is SsuiAppTheme)
        {
            tw.SsuiTheme = this.SsuiTheme;
        }
        Bind(SsuiThemeProperty, CompiledBinding.Create<ThemedWindow, SsuiTheme?>(win => win.SsuiTheme, source: tw));

        return tw;
    }

    #endregion

    #region Dialog Actions

    /// <summary>
    /// Accept the current input in <see cref="Value"/>, and attempt to close the dialog.
    /// </summary>
    /// <remarks>
    /// This sets <see cref="DialogResult"/> to <c>true</c>.
    /// </remarks>
    public void Accept()
    {
        if (!_validationPass) return;
        DialogResult = true;
        Close();
    }

    /// <summary>
    /// Cancel the input, and attempt to close the dialog.
    /// </summary>
    /// <remarks>
    /// This sets <see cref="DialogResult"/> to <c>false</c>.
    /// </remarks>
    public void Cancel()
    {
        DialogResult = false;
        Close();
    }

    #endregion

    #region Dialog Buttons

    /// <summary>
    /// Get or set if the dialog buttons ("OK" and "Cancel") should be shown.
    /// <para/>
    /// If this is set to <c>false</c>, you should have your own UI for allowing the user to accept or reject their input via mouse
    /// or touch, and also listen to the <see cref="Closing"/> event to determine when you should hide the dialog.
    /// </summary>
    public bool ShowDialogButtons { get => GetValue(ShowDialogButtonsProperty); set => SetValue(ShowDialogButtonsProperty, value); }

    /// <summary>The backing styled property for <see cref="ShowDialogButtons"/>. See the related property for details.</summary>
    public static readonly StyledProperty<bool> ShowDialogButtonsProperty
        = AvaloniaProperty.Register<StringInputDialog, bool>(nameof(ShowDialogButtons), true);

    /// <summary>
    /// Get or set the text to display for the "OK" button, which is used for accepting the input.
    /// </summary>
    public string OkButtonText { get => GetValue(OkButtonTextProperty); set => SetValue(OkButtonTextProperty, value); }

    /// <summary>The backing styled property for <see cref="OkButtonText"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> OkButtonTextProperty
        = AvaloniaProperty.Register<StringInputDialog, string>(nameof(OkButtonText), "OK");

    /// <summary>
    /// Get or set the text to display for the "Cancel" button, which is used for rejecting the input.
    /// </summary>
    public string CancelButtonText { get => GetValue(CancelButtonTextProperty); set => SetValue(CancelButtonTextProperty, value); }

    /// <summary>The backing styled property for <see cref="CancelButtonText"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> CancelButtonTextProperty
        = AvaloniaProperty.Register<StringInputDialog, string>(nameof(CancelButtonText), "Cancel");



    private void btnOK_Click(object sender, RoutedEventArgs e)
    {
        Accept();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Cancel();
    }

    private void txtValue_KeyDown(object sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.Enter && EnterKeyConfirms)
        {
            Accept();
        }
        else if (e.Key == Avalonia.Input.Key.Escape && EscapeKeyCancels)
        {
            Cancel();
        }
    }

    #endregion

    private void txtValue_TextChanged(object sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        CheckValidation();
    }
}