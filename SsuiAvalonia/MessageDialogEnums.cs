using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SolidShineUi.Utils;

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
/// A helper class that can retrieve a <see cref="Bitmap"/> based on a <see cref="MessageDialogImage"/> value.
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

    public static Bitmap? GetImage(this MessageDialogImage image, IconVariation color)
    {
        if (image == MessageDialogImage.None)
        {
            return null;
        }

        string packuri = "avares://SolidShineUi.Avalonia/DialogImages/";
        Bitmap img;

        try
        {
            img = new Bitmap(AssetLoader.Open(new Uri(packuri + image.ToString("f") + color.ToString("f") + ".png", UriKind.Absolute)));
        }
        catch (IOException ex)
        {
            throw new ArgumentException("Cannot find an icon with this name.", nameof(image), ex);
        }

        return img;
    }


}