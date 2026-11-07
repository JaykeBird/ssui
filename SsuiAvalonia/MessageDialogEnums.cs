using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi;

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
