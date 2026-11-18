namespace Atc.Wpf.Theming.Controls;

/// <summary>
/// A <see cref="ContentControl"/> that adds content character casing and access-key support,
/// and keeps input content hit-testable inside a window chrome.
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "OK.")]
[DefaultStyleKey]
public partial class ContentControlEx : ContentControl
{
    /// <summary>
    /// Gets or sets the character casing of the Content.
    /// </summary>
    [DependencyProperty(
        DefaultValue = CharacterCasing.Normal,
        Flags = FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure,
        ValidateValueCallback = nameof(IsContentCharacterCasingValid))]
    private CharacterCasing contentCharacterCasing;

    /// <summary>
    /// Determine if the inner ContentPresenter should use AccessText in its style
    /// </summary>
    [DependencyProperty(
        DefaultValue = false,
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata)]
    private bool recognizesAccessKey;

    private static bool IsContentCharacterCasingValid(object value)
        => (CharacterCasing)value >= CharacterCasing.Normal && (CharacterCasing)value <= CharacterCasing.Upper;

    /// <inheritdoc />
    protected override void OnContentChanged(
        object oldContent,
        object newContent)
    {
        if (oldContent is IInputElement and DependencyObject oldInputElement)
        {
            BindingOperations.ClearBinding(
                oldInputElement,
                WindowChrome.IsHitTestVisibleInChromeProperty);
        }

        base.OnContentChanged(oldContent, newContent);

        if (newContent is IInputElement and DependencyObject newInputElement)
        {
            BindingOperations.SetBinding(
                newInputElement,
                WindowChrome.IsHitTestVisibleInChromeProperty,
                new Binding
                {
                    Path = new PropertyPath(WindowChrome.IsHitTestVisibleInChromeProperty),
                    Source = this,
                });
        }
    }
}