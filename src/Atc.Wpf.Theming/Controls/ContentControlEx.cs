namespace Atc.Wpf.Theming.Controls;

[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "OK.")]
public class ContentControlEx : ContentControl
{
    public static readonly DependencyProperty ContentCharacterCasingProperty = DependencyProperty.Register(
        nameof(ContentCharacterCasing),
        typeof(CharacterCasing),
        typeof(ContentControlEx),
        new FrameworkPropertyMetadata(CharacterCasing.Normal, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure),
        value => (CharacterCasing)value >= CharacterCasing.Normal && (CharacterCasing)value <= CharacterCasing.Upper);

    /// <summary>
    /// Gets or sets the character casing of the Content.
    /// </summary>
    public CharacterCasing ContentCharacterCasing
    {
        get => (CharacterCasing)GetValue(ContentCharacterCasingProperty);
        set => SetValue(ContentCharacterCasingProperty, value);
    }

    /// <summary>Identifies the <see cref="RecognizesAccessKey"/> dependency property.</summary>
    public static readonly DependencyProperty RecognizesAccessKeyProperty = DependencyProperty.Register(
        nameof(RecognizesAccessKey),
        typeof(bool),
        typeof(ContentControlEx),
        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Determine if the inner ContentPresenter should use AccessText in its style
    /// </summary>
    public bool RecognizesAccessKey
    {
        get => (bool)GetValue(RecognizesAccessKeyProperty);
        set => SetValue(RecognizesAccessKeyProperty, BooleanBoxes.Box(value));
    }

    static ContentControlEx()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ContentControlEx),
            new FrameworkPropertyMetadata(typeof(ContentControlEx)));
    }

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