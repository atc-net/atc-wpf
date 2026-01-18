namespace Atc.Wpf.Components.Dialogs.Helpers;

/// <summary>
/// Provides helper methods for creating and displaying InputDialogBox.
/// </summary>
public static class InputDialogBoxHelper
{
    /// <summary>
    /// Requests text input from the user through an input dialog box.
    /// </summary>
    /// <param name="titleBarText">The text to display in the dialog box title bar.</param>
    /// <param name="labelText">The label text to display above the input field.</param>
    /// <param name="textValue">The initial text value to display in the input field. If <see langword="null" />, the field will be empty.</param>
    /// <param name="isMandatory">A value indicating whether the input field is mandatory. If <see langword="true" />, the field must contain a value for the dialog to be accepted.</param>
    /// <param name="minLength">The minimum required length of the input text. Default is 3 characters.</param>
    /// <returns>
    /// The text entered by the user if the dialog was accepted; otherwise, <see langword="null" /> if the dialog was cancelled or no valid input was provided.
    /// </returns>
    /// <remarks>
    /// This method creates an <see cref="InputDialogBox"/> with a <see cref="LabelTextBox"/> control configured with the specified parameters.
    /// The dialog is displayed modally relative to the application's main window.
    /// </remarks>
    public static string? RequestText(
        string titleBarText,
        string labelText,
        string? textValue = null,
        bool isMandatory = true,
        uint minLength = 3)
    {
        var labelTextBox = new LabelTextBox
        {
            LabelText = labelText,
            IsMandatory = isMandatory,
            MinLength = minLength,
        };

        if (textValue is not null)
        {
            labelTextBox.Text = textValue;
        }

        var dialogBox = new InputDialogBox(
            Application.Current.MainWindow!,
            titleBarText,
            labelTextBox);

        var dialogResult = dialogBox.ShowDialog();
        if (!dialogResult.HasValue || !dialogResult.Value)
        {
            return null;
        }

        return dialogBox.Data is LabelTextBox dialogBoxData
            ? dialogBoxData.Text
            : null;
    }
}